#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.Admin.Controls;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class FileVaultDetailsView : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] == null)
                {
                    Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Catalog/default.aspx");
                }
                else
                {
                    ViewState["id"] = Request.QueryString["id"];
                }
                LocalizeView();
                BindProductsGrid();
                PopulateFileInfo();
            }
        }

        private void LocalizeView()
        {
            var localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
            LocalizationUtils.LocalizeGridView(ProductsGridView, localization);
        }

        protected void BindProductsGrid()
        {
            ProductsGridView.DataSource = HccApp.CatalogServices.FindProductsForFile((string) ViewState["id"]);
            ProductsGridView.DataBind();
        }

        protected void PopulateFileInfo()
        {
            var file = HccApp.CatalogServices.ProductFiles.Find((string) ViewState["id"]);
            lblFileName.Text = file.FileName;
            DescriptionTextBox.Text = file.ShortDescription;
        }

        protected void ProductsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var key = (string) ProductsGridView.DataKeys[e.RowIndex].Value;
            var file = HccApp.CatalogServices.ProductFiles.Find((string) ViewState["id"]);
            HccApp.CatalogServices.ProductFiles.RemoveAssociatedProduct(file.Bvin, key);
            BindProductsGrid();
        }

        protected void btnBrowseProducts_Click(object sender, EventArgs e)
        {
            pnlProductPicker.Visible = !pnlProductPicker.Visible;
            if (NewSkuField.Text.Trim().Length > 0)
            {
                if (pnlProductPicker.Visible)
                {
                    ProductPicker1.Keyword = NewSkuField.Text;
                    ProductPicker1.LoadSearch();
                }
            }
        }

        private void AddProductBySku()
        {
            MessageBox1.ClearMessage();
            if (NewSkuField.Text.Trim().Length < 1)
            {
                MessageBox1.ShowWarning(Localization.GetString("NoSkuError"));
            }
            else
            {
                var p = HccApp.CatalogServices.Products.FindBySku(NewSkuField.Text.Trim());
                if (p != null)
                {
                    if (p.Sku == string.Empty)
                    {
                        MessageBox1.ShowWarning(Localization.GetString("InnvalidSku"));
                    }
                    else
                    {
                        var file = HccApp.CatalogServices.ProductFiles.Find((string) ViewState["id"]);
                        if (file != null)
                        {
                            if (HccApp.CatalogServices.ProductFiles.AddAssociatedProduct(file.Bvin, p.Bvin, 0, 0))
                            {
                                MessageBox1.ShowOk(Localization.GetString("ProductAddSuccess"));
                            }
                            else
                            {
                                MessageBox1.ShowError(Localization.GetString("ProductAddFailure"));
                            }
                        }
                    }
                }
            }

            BindProductsGrid();
        }

        protected void btnAddProductBySku_Click(object sender, EventArgs e)
        {
            AddProductBySku();
        }

        protected void btnProductPickerCancel_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();
            pnlProductPicker.Visible = false;
        }

        protected void btnProductPickerOkay_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();
            if (ProductPicker1.SelectedProducts.Count > 0)
            {
                var p = HccApp.CatalogServices.Products.Find(ProductPicker1.SelectedProducts[0]);
                if (p != null)
                {
                    NewSkuField.Text = p.Sku;
                }
                AddProductBySku();
                pnlProductPicker.Visible = false;
            }
            else
            {
                MessageBox1.ShowWarning(Localization.GetString("NoProductSelected"));
            }
        }

        protected void ProductsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Page.IsValid)
            {
                var key = (string) ProductsGridView.DataKeys[e.RowIndex].Value;
                var file = HccApp.CatalogServices.ProductFiles.FindByBvinAndProductBvin((string) ViewState["id"], key);

                var row = ProductsGridView.Rows[e.RowIndex];
                var tb = (TextBox) row.FindControl("MaxDownloadsTextBox");
                var tp = (TimespanPicker) row.FindControl("TimespanPicker");

                if (tb != null)
                {
                    var val = 0;
                    if (int.TryParse(tb.Text, out val))
                    {
                        file.MaxDownloads = val;
                    }
                    else
                    {
                        file.MaxDownloads = 0;
                    }
                }

                if (tp != null)
                {
                    file.SetMinutes(tp.Months, tp.Days, tp.Hours, tp.Minutes);
                }

                if (HccApp.CatalogServices.ProductFiles.Update(file))
                {
                    MessageBox1.ShowOk(Localization.GetString("FileUpdateSuccess"));
                }
                else
                {
                    MessageBox1.ShowError(Localization.GetString("FileUpdateFailure"));
                }
                BindProductsGrid();
            }
        }

        protected void ProductsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var key = (string) ProductsGridView.DataKeys[e.Row.RowIndex].Value;
                var file = HccApp.CatalogServices.ProductFiles.FindByBvinAndProductBvin((string) ViewState["id"], key);

                var tb = (TextBox) e.Row.FindControl("MaxDownloadsTextBox");
                var tp = (TimespanPicker) e.Row.FindControl("TimespanPicker");

                if (tb != null)
                {
                    tb.Text = file.MaxDownloads.ToString();
                }

                if (tp != null)
                {
                    var minutes = file.AvailableMinutes;
                    tp.Months = minutes/43200;
                    minutes = minutes%43200;
                    tp.Days = minutes/1440;
                    minutes = minutes%1440;
                    tp.Hours = minutes/60;
                    minutes = minutes%60;
                    tp.Minutes = minutes;
                }
            }
        }

        protected void SaveImageButton_Click(object sender, EventArgs e)
        {
            var file = HccApp.CatalogServices.ProductFiles.Find((string) ViewState["id"]);
            if (file != null)
            {
                file.ShortDescription = DescriptionTextBox.Text;
                if (HccApp.CatalogServices.ProductFiles.Update(file))
                {
                    MessageBox1.ShowOk(Localization.GetString("FileUpdateSuccess"));
                }
                else
                {
                    MessageBox1.ShowError(Localization.GetString("FileUpdateFailure"));
                }
            }
        }

        protected void CancelImageButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Catalog/FileVault.aspx");
        }

        protected void FileReplaceCancelImageButton_Click(object sender, EventArgs e)
        {
            FilePicker1.Clear();
            ReplacePanel.Visible = false;
        }

        protected void FileReplaceSaveImageButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var file = HccApp.CatalogServices.ProductFiles.Find((string) ViewState["id"]);
                if (file != null)
                {
                    FilePicker1.DownloadOrLinkFile(file, MessageBox1);
                    ReplacePanel.Visible = false;
                    PopulateFileInfo();
                }
                else
                {
                    MessageBox1.ShowError(Localization.GetString("FileUpdateFailure"));
                }
            }
        }

        protected void ReplaceImageButton_Click(object sender, EventArgs e)
        {
            ReplacePanel.Visible = true;
        }

        protected void btnCloseVariants_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();
            pnlProductChoices.Visible = false;
        }
    }
}