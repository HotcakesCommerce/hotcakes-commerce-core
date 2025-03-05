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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductUpSellCrossSell : BaseProductPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvRelatedProducts.RowDataBound += gvRelatedProducts_RowDataBound;
            gvRelatedProducts.RowDeleting += gvRelatedProducts_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadItems();
            }
        }

        protected void gvRelatedProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var imgProduct = e.Row.FindControl("imgProduct") as Image;
                var litProductName = e.Row.FindControl("litProductName") as Literal;

                var prodRelat = e.Row.DataItem as ProductRelationship;
                var product = HccApp.CatalogServices.Products.Find(prodRelat.RelatedProductId);
                if (product != null)
                {
                    imgProduct.ImageUrl = DiskStorage.ProductImageUrlSmall(HccApp, product.Bvin, product.ImageFileSmall,
                        Page.Request.IsSecureConnection);
                    imgProduct.AlternateText = product.ImageFileSmallAlternateText;
                    litProductName.Text = product.ProductName;
                }

                e.Row.Attributes["id"] = prodRelat.RelatedProductId;
            }
        }

        protected void gvRelatedProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var relationshipId = (long) gvRelatedProducts.DataKeys[e.RowIndex].Value;
            HccApp.CatalogServices.ProductRelationships.Delete(relationshipId);

            LoadItems();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var selectedBvins = ucProductPicker.SelectedProducts;
                foreach (var productBvin in selectedBvins)
                {
                    HccApp.CatalogServices.ProductRelationships.RelateProducts(ProductId, productBvin, false);
                }

                LoadItems();
            }
        }

        private void LoadItems()
        {
            gvRelatedProducts.DataSource = HccApp.CatalogServices.ProductRelationships.FindForProduct(ProductId);
            gvRelatedProducts.DataBind();
        }
    }
}