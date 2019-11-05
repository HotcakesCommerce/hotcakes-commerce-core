#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class ProductEditMenu : UserControl
    {
        public enum MenuItemType
        {
            General,
            AdditionalImages,
            Categories,
            CustomerChoices,
            Variants,
            Files,
            Inventory,
            UpSellCrossSell,
            BundledProducts,
            InfoTabs,
            ProductReviews,
            VolumeDiscounts,
            Roles,
            Performance
        }

        public MenuItemType SelectedMenuItem { get; set; }

        public new BaseProductPage Page
        {
            get { return base.Page as BaseProductPage; }
        }

        public bool? IsBundle
        {
            get { return (bool?) ViewState["IsBundle"]; }
            set { ViewState["IsBundle"] = value; }
        }

        public bool IsNew
        {
            get { return string.IsNullOrEmpty(Request.Params["id"]); }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ProcessLinks();
        }

        protected string GetCurrentCssClass(MenuItemType itemType)
        {
            var css = string.Empty;
            if (SelectedMenuItem == itemType)
            {
                css = "hcCurrent";
            }
            else if (IsNew && itemType != MenuItemType.General)
            {
                css = "hcDisabled";
            }

            return string.IsNullOrEmpty(css) ? string.Empty : string.Format("class='{0}'", css);
        }

        private void ProcessLinks()
        {
            //detecting visibility
            var isBundle = false;
            if (IsBundle.HasValue)
            {
                isBundle = IsBundle.Value;
            }
            else if (Page != null && !string.IsNullOrEmpty(Page.ProductId))
            {
                var product = Page.HccApp.CatalogServices.Products.FindWithCache(Page.ProductId);
                if (product != null)
                {
                    isBundle = product.IsBundle;
                }
            }

            liCustomerChoices.Visible = !isBundle;
            liVariants.Visible = !isBundle;
            liInventory.Visible = !isBundle;
            liBundledProducts.Visible = isBundle;

            if (!IsPostBack)
            {
                //assigning URLs and enable/disable flags
                PrepareHyperLink(hypGeneral);
                PrepareHyperLink(hypAdditionalImages);
                PrepareHyperLink(hypCategories);
                PrepareHyperLink(hypCustomerChoices);
                PrepareHyperLink(hypVariants);
                PrepareHyperLink(hypFiles);
                PrepareHyperLink(hypInventory);
                PrepareHyperLink(hypUpSellCrossSell);
                PrepareHyperLink(hypBundledProducts);
                PrepareHyperLink(hypInfoTabs);
                PrepareHyperLink(hypProductReviews);
                PrepareHyperLink(hypVolumeDiscounts);
                PrepareHyperLink(hypRoles);
                PrepareHyperLink(hypPerformance);
            }
        }

        private void PrepareHyperLink(HyperLink hyperLink)
        {
            if (!IsNew)
            {
                hyperLink.NavigateUrl += "?id=" + Request.Params["id"];
            }
            else if (hyperLink != hypGeneral) //First page must be always enabled
            {
                hyperLink.Enabled = false;
            }
        }
    }
}