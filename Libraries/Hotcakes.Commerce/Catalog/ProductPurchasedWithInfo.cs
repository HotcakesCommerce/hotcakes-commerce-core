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

using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductPurchasedWithInfo
    {
        public ProductPurchasedWithInfo(HotcakesApplication hccApp, Product product)
        {
            ImageUrl = DiskStorage.ProductImageUrlSmall(hccApp, product.Bvin, product.ImageFileSmall,
                hccApp.IsCurrentRequestSecure());
            ProductId = product.Bvin;
            ProductName = product.ProductName;
            UpdatedOn = DateHelper.ConvertUtcToStoreTime(hccApp, product.LastUpdated).ToString("MMM dd, yyyy");
            CreatedOn = DateHelper.ConvertUtcToStoreTime(hccApp, product.CreationDateUtc).ToString("MMM dd, yyyy");
        }

        public string ImageUrl { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string UpdatedOn { get; set; }
        public string CreatedOn { get; set; }

        public int QuantitySold { get; set; }
        public string QuantitySoldPercentageChange { get; set; }
        public string QuantitySoldComparison { get; set; }
        public bool IsQuantitySoldGrowing { get; set; }

        public string Revenue { get; set; }
        public string RevenuePercentageChange { get; set; }
        public string RevenueComparison { get; set; }
        public bool IsRevenueGrowing { get; set; }
    }
}