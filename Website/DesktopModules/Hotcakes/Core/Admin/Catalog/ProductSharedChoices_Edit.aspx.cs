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
using System.Web.UI;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Catalog.Options;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductSharedChoices_Edit : BaseAdminPage
    {
        private string ChoiceId
        {
            get { return Request.QueryString["id"]; }
        }

        private Option Choice { get; set; }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            Choice = HccApp.CatalogServices.ProductOptions.Find(ChoiceId);
            ItemsEditor.OptionId = Choice.Bvin;

            if (Choice.OptionType == OptionTypes.DropDownList | Choice.OptionType == OptionTypes.RadioButtonList)
            {
                trVariant.Visible = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadItem();
            }
        }

        private void LoadItem()
        {
            txtName.Text = Choice.Name;
            chkHideName.Checked = Choice.NameIsHidden;
            chkVariant.Checked = Choice.IsVariant;

            switch (Choice.OptionType)
            {
                case OptionTypes.DropDownList:
                case OptionTypes.RadioButtonList:
                case OptionTypes.CheckBoxes:
                    viewMain.SetActiveView(viewItems);
                    break;
                case OptionTypes.Html:
                    viewMain.SetActiveView(viewHtml);
                    HtmlEditor1.Text = Choice.TextSettings.GetSettingOrEmpty("html");
                    break;
                case OptionTypes.TextInput:
                    viewMain.SetActiveView(viewTextInput);
                    var ti = (TextInput) Choice.Processor;
                    ColumnsField.Text = ti.GetColumns(Choice);
                    RowsField.Text = ti.GetRows(Choice);
                    MaxLengthField.Text = ti.GetMaxLength(Choice);
                    break;
                case OptionTypes.FileUpload:
                    viewMain.SetActiveView(viewFileUpload);
                    var fu = (Commerce.Catalog.Options.FileUpload) Choice.Processor;
                    MultipleFilesField.Checked = fu.GetMultipleFiles(Choice);
                    break;
            }
        }

        protected void btnSaveOption_Click(object sender, ImageClickEventArgs e)
        {
            Choice.Name = txtName.Text;
            Choice.NameIsHidden = chkHideName.Checked;
            Choice.IsVariant = chkVariant.Checked;

            switch (Choice.OptionType)
            {
                case OptionTypes.CheckBoxes:
                    break;
                case OptionTypes.DropDownList:
                    break;
                case OptionTypes.FileUpload:
                    var fu = (Commerce.Catalog.Options.FileUpload) Choice.Processor;
                    fu.SetMultipleFiles(Choice, MultipleFilesField.Checked);
                    break;
                case OptionTypes.Html:
                    Choice.TextSettings.AddOrUpdate("html", HtmlEditor1.Text);
                    break;
                case OptionTypes.RadioButtonList:
                    break;
                case OptionTypes.TextInput:
                    var ti = (TextInput) Choice.Processor;
                    ti.SetColumns(Choice, ColumnsField.Text);
                    ti.SetRows(Choice, RowsField.Text);
                    ti.SetMaxLength(Choice, MaxLengthField.Text);
                    break;
            }

            var success = HccApp.CatalogServices.ProductOptions.Update(Choice);
            if (success)
            {
                HccApp.CatalogServices.VariantsValidateForSharedOption(Choice);
                MessageBox1.ShowOk("Changes Saved!");
            }

            ItemsEditor.LoadItems();
        }
    }
}