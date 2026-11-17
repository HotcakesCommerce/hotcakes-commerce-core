#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>
    /// Defines the catalog data or behavior represented by this member.
    /// </summary>
    public class ProductPurchasedWithInfo
    {
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public ProductPurchasedWithInfo(HotcakesApplication hccApp, Product product)
        {
            ImageUrl = DiskStorage.ProductImageUrlSmall(hccApp, product.Bvin, product.ImageFileSmall,
                hccApp.IsCurrentRequestSecure());
            ProductId = product.Bvin;
            ProductName = product.ProductName;
            UpdatedOn = DateHelper.ConvertUtcToStoreTime(hccApp, product.LastUpdated).ToString("MMM dd, yyyy");
            CreatedOn = DateHelper.ConvertUtcToStoreTime(hccApp, product.CreationDateUtc).ToString("MMM dd, yyyy");
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public string UpdatedOn { get; set; }
        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public string CreatedOn { get; set; }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public int QuantitySold { get; set; }
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string QuantitySoldPercentageChange { get; set; }
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string QuantitySoldComparison { get; set; }
        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public bool IsQuantitySoldGrowing { get; set; }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string Revenue { get; set; }
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string RevenuePercentageChange { get; set; }
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public string RevenueComparison { get; set; }
        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public bool IsRevenueGrowing { get; set; }
    }
}
