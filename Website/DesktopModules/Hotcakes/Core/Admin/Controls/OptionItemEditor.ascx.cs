#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class OptionItemEditor : HccUserControl
    {
        #region Properties

        public string CurrentOptionItemId
        {
            get { return ViewState["OptionItemId"] as string; }
            set { ViewState["OptionItemId"] = value; }
        }

        public string OptionId { get; set; }

        #endregion

        #region Event Handler

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvItems.RowDataBound += gvItems_RowDataBound;
            gvItems.RowEditing += gvItems_RowEditing;
            gvItems.RowDeleting += gvItems_RowDeleting;
            lnkEditorCancel.Click += (s, a) => CloseItemEditor();
            lnkEditorSave.Click += lnkEditorSave_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                LocalizeView();
                LoadItems();
            }
        }

        private void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var oi = e.Row.DataItem as OptionItem;
                e.Row.Attributes["id"] = oi.Bvin;

                var chkBoxes = e.Row.Cells.OfType<TableCell>().SelectMany(cell => cell.Controls.OfType<CheckBox>());

                foreach (var curCheckBox in chkBoxes)
                {
                    curCheckBox.Enabled = true;
                    curCheckBox.Attributes.Add("onclick", "return false;");
                }
            }
        }

        private void gvItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var bvin = e.Keys[0] as string;
            HccApp.CatalogServices.ProductOptions.DeleteOptionItem(bvin);
            LoadItems();
        }

        private void gvItems_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var bvin = gvItems.DataKeys[e.NewEditIndex].Value as string;

            var oi = HccApp.CatalogServices.ProductOptions.OptionItemFind(bvin);
            txtItemName.Text = oi.Name;
            txtItemPrice.Text = Money.FormatCurrency(oi.PriceAdjustment);
            txtItemWeight.Text = oi.WeightAdjustment.ToString("f3");
            cbItemIsDefault.Checked = oi.IsDefault;
            cbItemIsLabel.Checked = oi.IsLabel;

            ShowItemEditor(bvin);
            e.Cancel = true;
        }

        private void lnkEditorSave_Click(object sender, EventArgs e)
        {
            var oi = HccApp.CatalogServices.ProductOptions.OptionItemFind(CurrentOptionItemId);
            oi.Name = txtItemName.Text;
            oi.PriceAdjustment = Money.RoundCurrency(decimal.Parse(txtItemPrice.Text));
            oi.WeightAdjustment = Math.Round(decimal.Parse(txtItemWeight.Text), 3);
            oi.IsDefault = cbItemIsDefault.Checked;
            oi.IsLabel = cbItemIsLabel.Checked;

            HccApp.CatalogServices.ProductOptions.OptionItemUpdate(oi);

            LoadItems();
            CloseItemEditor();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNewName.Text))
            {
                if (OptionId != string.Empty)
                {
                    var opt = HccApp.CatalogServices.ProductOptions.Find(OptionId);
                    if (opt != null)
                    {
                        opt.AddItem(txtNewName.Text.Trim());
                        HccApp.CatalogServices.ProductOptions.Update(opt);
                        txtNewName.Text = string.Empty;
                        txtNewName.Focus();
                    }
                }
            }
            LoadItems();
        }

        #endregion

        #region Implementation

        public void LocalizeView()
        {
            txtNewName.Attributes.Add("placeholder", Localization.GetString("txtNewName.Placeholder"));

            var localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
            LocalizationUtils.LocalizeGridView(gvItems, localization);
        }

        public void LoadItems()
        {
            var opt = HccApp.CatalogServices.ProductOptions.Find(OptionId);
            if (opt != null)
            {
                gvItems.DataSource = opt.Items;
                gvItems.DataBind();
            }
        }

        private void ShowItemEditor(string bvin)
        {
            CurrentOptionItemId = bvin;
            pnlEditItem.Visible = true;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcEditItemDialog", "hcEditItemDialog();", true);
        }

        private void CloseItemEditor()
        {
            CurrentOptionItemId = null;
            pnlEditItem.Visible = false;
        }

        #endregion
    }
}