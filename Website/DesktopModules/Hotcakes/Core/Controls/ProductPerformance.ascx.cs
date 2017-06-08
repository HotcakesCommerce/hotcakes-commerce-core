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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Controls
{
    public partial class ProductPerformance : HccUserControl
    {
        public string ProductId { get; set; }

        public bool EditMode { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProductId))
            {
                var productRepo = Factory.CreateRepo<ProductRepository>();
                var product = productRepo.FindWithCache(ProductId);
                if (product != null)
                {
                    var currentStore = HccRequestContext.Current.CurrentStore;

                    lblProductName.Text = product.ProductName;
                    lblLastModifiedOn.Text =
                        DateHelper.ConvertUtcToStoreTime(currentStore, product.LastUpdated).ToString("MMM dd, yyyy");
                    lblCreatedOn.Text =
                        DateHelper.ConvertUtcToStoreTime(currentStore, product.CreationDateUtc).ToString("MMM dd, yyyy");

                    var performanceUserSelections = new PerformanceUserSelections();
                    ddlRowPeriod.SelectedValue = performanceUserSelections.ProductsPerformacePeriod.ToString();
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            txtProductId.Value = ProductId;

            var urlTemplate = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/Products_Edit.aspx";

            btnAddProduct.NavigateUrl = urlTemplate;
            btnEditProduct.NavigateUrl = string.Concat(urlTemplate, "?id=", ProductId);

            btnAddProduct.Visible = !EditMode;
            btnEditProduct.Visible = EditMode;
        }
    }
}