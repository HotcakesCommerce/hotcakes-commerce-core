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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductClone : BaseProductPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.IsNullOrEmpty(ProductId))
                Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Catalog/default.aspx");

            if (!Page.IsPostBack)
            {
                var product = HccApp.CatalogServices.Products.Find(ProductId);
                if (product != null)
                {
                    txtSku.Text = product.Sku + "-COPY";
                    txtSlug.Text = product.UrlSlug + "-COPY";
                    chkProductChoices.Checked = true;
                    chkCategoryPlacement.Checked = true;
                    chkImages.Checked = true;
                }
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Clone Product";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected void btnClone_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Clone();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/DesktopModules/Hotcakes/Core/Admin/Catalog/Products_Edit.aspx?id={0}",
                ProductId));
        }

        protected bool Clone()
        {
            var newSku = txtSku.Text.Trim();
            var newSlug = txtSlug.Text.Trim();
            var newStatus = chkInactive.Checked ? ProductStatus.Disabled : ProductStatus.Active;
            var cloneProductRoles = chkProductRoles.Checked;
            var cloneProductChoices = chkProductChoices.Checked;
            var cloneCategoryPlacement = chkCategoryPlacement.Checked;
            var cloneImages = chkImages.Checked;
            var cloneReviews = chkReviews.Checked;


            var existing = HccApp.CatalogServices.Products.FindBySku(newSku);
            if (existing != null)
            {
                ucMessageBox.ShowError("That SKU is already in use by another product!");
                return false;
            }

            existing = HccApp.CatalogServices.Products.FindBySlug(newSlug);
            if (existing != null)
            {
                ucMessageBox.ShowError("That Slug is already in use by another product!");
                return false;
            }

            HccApp.CatalogServices.CloneProduct(ProductId, newSku, newSlug, newStatus, cloneProductRoles,
                cloneProductChoices, cloneCategoryPlacement, cloneImages, cloneReviews);

            ucMessageBox.ShowOk("Product was cloned");
            return true;
        }
    }
}