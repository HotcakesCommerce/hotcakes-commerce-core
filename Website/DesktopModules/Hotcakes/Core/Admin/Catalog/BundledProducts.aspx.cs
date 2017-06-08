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
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    /// <summary>
    ///     This class is used to manage functionality of bundling products to a parent product
    /// </summary>
    public partial class BundledProducts : BaseProductPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            PageTitle = "Bundled Products";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvBundledProducts.RowDataBound += gvBundledProducts_RowDataBound;
            gvBundledProducts.RowDeleting += gvBundledProducts_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadItems();
            }
        }

        protected void gvBundledProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var litProductName = e.Row.FindControl("litProductName") as Literal;
                var txtQuantity = e.Row.FindControl("txtQuantity") as TextBox;

                var bundledProductAdv = e.Row.DataItem as BundledProductAdv;

                //
                // preventative code to clean up imported or corrupted data
                // products cannot be bundled to themselves
                //
                if (bundledProductAdv != null && bundledProductAdv.BundledProductId.Equals(ProductId))
                {
                    // remove the bundled product from the parent product
                    HccApp.CatalogServices.BundledProducts.Delete(bundledProductAdv.Id);

                    return;
                }

                if (bundledProductAdv.BundledProduct != null)
                    litProductName.Text = bundledProductAdv.BundledProduct.ProductName;

                txtQuantity.Text = bundledProductAdv.Quantity.ToString();

                e.Row.Attributes["id"] = bundledProductAdv.Id.ToString();
            }
        }

        protected void gvBundledProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) gvBundledProducts.DataKeys[e.RowIndex].Value;
            HccApp.CatalogServices.BundledProducts.Delete(id);

            LoadItems();
        }

        protected void btnAddProducts_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                foreach (var prodBvin in ucProductPicker.SelectedProducts)
                {
                    // Preventative Code Only - this shouldn't be necessary, but is
                    // do not bundle a product to itself to prevent stackoverflow errors in customer views
                    if (prodBvin.Equals(ProductId))
                    {
                        EventLog.LogEvent("Bundled Products", "Product cannot be bundled to itself",
                            EventLogSeverity.Information);
                        continue;
                    }

                    var bundledProduct = new BundledProduct
                    {
                        BundledProductId = prodBvin,
                        ProductId = ProductId,
                        Quantity = 1
                    };

                    HccApp.CatalogServices.BundledProductCreate(bundledProduct);
                }

                LoadItems();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                foreach (GridViewRow item in gvBundledProducts.Rows)
                {
                    var textBox = item.FindControl("txtQuantity") as TextBox;
                    var bundledProductId = (long) gvBundledProducts.DataKeys[item.DataItemIndex].Value;

                    var bundledProduct = HccApp.CatalogServices.BundledProducts.Find(bundledProductId);
                    bundledProduct.Quantity = int.Parse(textBox.Text);
                    HccApp.CatalogServices.BundledProducts.Update(bundledProduct);
                }

                LoadItems();
            }
        }

        /// <summary>
        ///     This method loads the bundled products into the UI
        /// </summary>
        private void LoadItems()
        {
            var product = HccApp.CatalogServices.Products.Find(ProductId);

            // This is a preventative check only and shouldn't be necessary
            // check to ensure that the product is indeed a bundle
            // this check is necessary only for new products that haven't yet been saved
            if (!product.IsBundle)
            {
                // set the product to be a bundle
                product.IsBundle = true;

                // update the product as a bundle
                HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(product);
            }

            gvBundledProducts.DataSource = product.BundledProducts;
            gvBundledProducts.DataBind();
        }
    }
}