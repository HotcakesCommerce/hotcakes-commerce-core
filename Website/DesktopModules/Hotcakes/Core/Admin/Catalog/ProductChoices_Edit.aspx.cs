#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Catalog.Options;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductChoices_Edit : BaseProductPage
    {
        #region Properties

        private Product _localProduct;
        private Option _option;

        protected string OptionId
        {
            get { return Request.QueryString["cid"]; }
        }

        #endregion

        #region Event handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);

            _localProduct = HccApp.CatalogServices.Products.Find(ProductId);
            _option = _localProduct.Options.FirstOrDefault(y => y.Bvin == OptionId);
            ItemsEditor.OptionId = _option.Bvin;
            trVariant.Visible = _option.OptionType == OptionTypes.DropDownList ||
                                _option.OptionType == OptionTypes.RadioButtonList;
            trRequired.Visible = _option.OptionType == OptionTypes.DropDownList ||
                                 _option.OptionType == OptionTypes.RadioButtonList ||
                                 _option.OptionType == OptionTypes.CheckBoxes;

            chkRequired.CheckedChanged += chkRequired_CheckedChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadOption();
            }
        }

        private void chkRequired_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRequired.Checked && _option.OptionType == OptionTypes.DropDownList)
            {
                if (!_option.Items.Any(i => i.IsDefault && i.IsLabel))
                {
                    var maxSortOrder = _option.Items.Max(i => (int?) i.SortOrder) ?? 0;
                    _option.Items.Add(new OptionItem
                    {
                        IsDefault = true,
                        IsLabel = true,
                        Name = Localization.GetString("SelectOne"),
                        OptionBvin = _option.Bvin,
                        SortOrder = maxSortOrder + 1
                    });

                    SaveOption();
                    MoveLastItemToTop();
                }
            }
        }


        protected void btnSaveOption_Click(object sender, EventArgs e)
        {
            SaveOption();
        }

        protected void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            if (SaveOption())
            {
                Response.Redirect("ProductChoices.aspx?id=" + ProductId);
            }
        }

        #endregion

        #region Implementation

        private void MoveLastItemToTop()
        {
            _option.Items.Insert(0, _option.Items.Last());
            _option.Items.RemoveAt(_option.Items.Count - 1);
            HccApp.CatalogServices.ProductOptions.ResortOptionItems(_option.Bvin,
                _option.Items.Select(i => i.Bvin).ToList());
            ItemsEditor.LoadItems();
        }

        private void LoadOption()
        {
            NameField.Text = _option.Name;
            chkHideName.Checked = _option.NameIsHidden;
            chkVariant.Checked = _option.IsVariant;
            chkIsColorSwatch.Checked = _option.IsColorSwatch;
            chkRequired.Checked = _option.IsRequired;

            switch (_option.OptionType)
            {
                case OptionTypes.DropDownList:
                case OptionTypes.RadioButtonList:
                case OptionTypes.CheckBoxes:
                    viewMain.SetActiveView(viewItems);
                    break;
                case OptionTypes.Html:
                    viewMain.SetActiveView(viewHtml);
                    HtmlEditor1.Text = _option.TextSettings.GetSettingOrEmpty("html");
                    break;
                case OptionTypes.TextInput:
                    viewMain.SetActiveView(viewTextInput);
                    var ti = (TextInput) _option.Processor;
                    ColumnsField.Text = ti.GetColumns(_option);
                    RowsField.Text = ti.GetRows(_option);
                    MaxLengthField.Text = ti.GetMaxLength(_option);
                    break;
                case OptionTypes.FileUpload:
                    viewMain.SetActiveView(viewFileInput);
                    var fu = (Commerce.Catalog.Options.FileUpload) _option.Processor;
                    MultipleFilesField.Checked = fu.GetMultipleFiles(_option);
                    break;
            }
        }


        private bool SaveOption()
        {
            ucMessageBox.ClearMessage();
            _option.Name = NameField.Text;
            _option.NameIsHidden = chkHideName.Checked;
            _option.IsVariant = chkVariant.Checked;
            _option.IsColorSwatch = chkIsColorSwatch.Checked;
            _option.IsRequired = chkRequired.Checked;

            switch (_option.OptionType)
            {
                case OptionTypes.Html:
                    _option.TextSettings.AddOrUpdate("html", HtmlEditor1.Text);
                    break;
                case OptionTypes.TextInput:
                    var ti = (TextInput) _option.Processor;
                    ti.SetColumns(_option, ColumnsField.Text);
                    ti.SetRows(_option, RowsField.Text);
                    ti.SetMaxLength(_option, MaxLengthField.Text);
                    break;
                case OptionTypes.FileUpload:
                    var fu = (Commerce.Catalog.Options.FileUpload) _option.Processor;
                    fu.SetMultipleFiles(_option, MultipleFilesField.Checked);
                    break;
            }
            var success = HccApp.CatalogServices.Products.Update(_localProduct);
            if (success)
            {
                HccApp.CatalogServices.VariantsValidate(_localProduct);
                ucMessageBox.ShowOk(Localization.GetString("SuccessMessage"));
            }
            else
            {
                ucMessageBox.ShowWarning(Localization.GetString("ErrorMessage"));
            }

            ItemsEditor.LoadItems();

            return success;
        }

        #endregion
    }
}