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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Data;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductChoices_Variants : BaseProductPage
    {
        #region Properties

        private Product _currentProduct;

        protected string EditedVariantId
        {
            get { return ViewState["EditedVariantId"] as string; }
            set { ViewState["EditedVariantId"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit Product Variants";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);

            if (!string.IsNullOrEmpty(ProductId))
            {
                _currentProduct = HccApp.CatalogServices.Products.Find(ProductId);
                BuildEditorControls();
            }

            gvVariants.RowDeleting += gvVariants_RowDeleting;
            gvVariants.RowEditing += gvVariants_RowEditing;
            lnkCancel.Click += lnkCancel_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                HccApp.CatalogServices.VariantsValidate(_currentProduct);
            }
            LoadVariants();
        }

        private void gvVariants_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var bvin = e.Keys[0] as string;
            HccApp.CatalogServices.ProductVariants.Delete(bvin);
            LoadVariants();
        }

        private void gvVariants_RowEditing(object sender, GridViewEditEventArgs e)
        {
            EditedVariantId = gvVariants.DataKeys[e.NewEditIndex].Value as string;
            ShowDialog();
            e.Cancel = true;
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {
            CloseDialog();
        }

        protected void btnGenerateAll_Click(object sender, EventArgs e)
        {
            int possibleCount;
            HccApp.CatalogServices.VariantsGenerateAllPossible(_currentProduct, out possibleCount);

            Response.Redirect("ProductChoices_Variants.aspx?id=" + ProductId);
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var selections = new List<OptionSelection>();
            var variantOptions = _currentProduct.Options.VariantsOnly();

            if (variantOptions.Count > 0)
            {
                foreach (var opt in variantOptions)
                {
                    var ddl = phLists.FindControl("new" + opt.Bvin) as DropDownList;
                    if (ddl != null && ddl.SelectedItem != null)
                    {
                        selections.Add(new OptionSelection(opt.Bvin, ddl.SelectedItem.Value));
                    }
                }

                if (selections.Count == variantOptions.Count)
                {
                    var newV = new Variant();
                    newV.ProductId = _currentProduct.Bvin;
                    newV.Selections.AddRange(selections);
                    var key = newV.UniqueKey();
                    if (!_currentProduct.Variants.Any(v => v.UniqueKey() == key))
                    {
                        HccApp.CatalogServices.ProductVariants.Create(newV);
                    }
                    else
                    {
                        var varDescription = GetVariantDescription(newV);
                        ucMessageBox.ShowWarning(string.Format("Variant '{0}' is already added", varDescription));
                    }
                }
                LoadVariants();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var item = HccApp.CatalogServices.ProductVariants.Find(EditedVariantId);

            if (item != null)
            {
                cvVariantSku.IsValid = true;
                var prodGuid = DataTypeHelper.BvinToNullableGuid(ProductId);
                if (HccApp.CatalogServices.Products.IsSkuExist(txtVariantSku.Text.Trim(), prodGuid))
                {
                    cvVariantSku.IsValid = false;
                    ShowDialog();
                    return;
                }

                item.Sku = txtVariantSku.Text.Trim();
                var p = item.Price;
                if (decimal.TryParse(txtVariantPrice.Text.Trim(), NumberStyles.Currency, CultureInfo.CurrentCulture,
                    out p))
                {
                    item.Price = Money.RoundCurrency(p);
                }

                if (ucVariantImage.HasFile)
                {
                    DiskStorage.CopyProductVariantImage(HccApp.CurrentStore.Id, ProductId, item.Bvin,
                        ucVariantImage.TempImagePath, ucVariantImage.FileName);
                }

                HccApp.CatalogServices.ProductVariants.Update(item);
            }

            CloseDialog();
            LoadVariants();
        }

        #endregion

        #region Implementation

        private void ShowDialog()
        {
            var variant = HccApp.CatalogServices.ProductVariants.Find(EditedVariantId);
            lblVariantDescription.Text = GetVariantDescription(variant);
            txtVariantSku.Text = string.IsNullOrEmpty(variant.Sku) ? _currentProduct.Sku : variant.Sku;
            txtVariantPrice.Text = (variant.Price < 0 ? _currentProduct.SitePrice : variant.Price).ToString("c");

            ucVariantImage.ImageUrl = DiskStorage.ProductVariantImageUrlMedium(HccApp, _currentProduct.Bvin,
                _currentProduct.ImageFileSmall, variant.Bvin, HccApp.IsCurrentRequestSecure());

            pnlEditVariant.Visible = true;
            ClientScript.RegisterStartupScript(Page.GetType(), "hcEditVariantDialog", "hcEditVariantDialog();", true);
        }

        private void CloseDialog()
        {
            pnlEditVariant.Visible = false;
            EditedVariantId = null;
        }

        private void BuildEditorControls()
        {
            phLists.Controls.Clear();

            var options = _currentProduct.Options.VariantsOnly();

            if (options.Count > 0)
            {
                btnNew.Visible = true;
                btnGenerateAll.Visible = true;
                foreach (var opt in options)
                {
                    var ddl = new DropDownList();
                    ddl.ID = "new" + opt.Bvin;
                    ddl.ClientIDMode = ClientIDMode.Static;
                    foreach (var oi in opt.Items)
                    {
                        if (!oi.IsLabel)
                        {
                            ddl.Items.Add(new ListItem(oi.Name, oi.Bvin));
                        }
                    }
                    phLists.Controls.Add(ddl);
                }
            }
            else
            {
                btnNew.Visible = false;
                btnGenerateAll.Visible = false;
                ucMessageBox.ShowInformation(
                    "No any choices marked as VARIANT. To create variant please go to \"Choices - Edit\" page and check checkbox \"This choice changes Inventory, Pictures, Prices and/or SKU\" ");
            }
        }

        private void LoadVariants()
        {
            _currentProduct = HccApp.CatalogServices.Products.Find(ProductId);
            var p = _currentProduct;

            var data = p.Variants.Select(v =>
            {
                var newV = new Variant
                {
                    StoreId = v.StoreId,
                    Bvin = v.Bvin,
                    ProductId = v.ProductId,
                    Price = v.Price < 0 ? p.SitePrice : v.Price,
                    Sku = string.IsNullOrEmpty(v.Sku) ? p.Sku : v.Sku
                };
                newV.Selections.AddRange(v.Selections);
                return newV;
            });

            gvVariants.DataSource = data;
            gvVariants.DataBind();

            var possible = HccApp.CatalogServices.VariantsGetAllPossibleSelections(p.Options);
            var generatedCount = data.Count();
            var toGenerate = Math.Min(possible.Count, WebAppSettings.MaxVariants) - generatedCount;
            lblAllPossibleCount.Text = possible.Count.ToString();
            lblGeneratedCount.Text = generatedCount.ToString();
            lblToGenerateCount.Text = toGenerate.ToString();
            lblGenerateWarning.Visible = possible.Count > WebAppSettings.MaxVariants;

            btnGenerateAll.Visible = toGenerate > 0;
            btnNew.Visible = toGenerate > 0;
        }

        protected string GetVariantName(IDataItemContainer cont)
        {
            var variant = cont.DataItem as Variant;
            return GetVariantDescription(variant);
        }

        private string GetVariantDescription(Variant variant)
        {
            var selections = variant.SelectionNames(_currentProduct.Options);

            return string.Join("/", selections);
        }

        protected string GetVariantImageUrl(IDataItemContainer cont)
        {
            var v = cont.DataItem as Variant;
            var p = _currentProduct;
            return DiskStorage.ProductVariantImageUrlMedium(HccApp, p.Bvin, p.ImageFileSmall, v.Bvin,
                HccApp.IsCurrentRequestSecure());
        }

        #endregion
    }
}