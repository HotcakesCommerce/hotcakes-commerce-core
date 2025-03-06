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

namespace Hotcakes.Commerce.Catalog
{
    [Serializable]
    public class ProductSearchCriteria
    {
        public ProductSearchCriteria()
        {
            Keyword = string.Empty;
            ManufacturerId = string.Empty;
            VendorId = string.Empty;
            Status = ProductStatus.NotSet;
            InventoryStatus = ProductInventoryStatus.NotSet;
            ProductTypeId = string.Empty;
            CategoryId = string.Empty;
            NotCategoryId = string.Empty;
            DisplayInactiveProducts = false;
            DisplayBundles = true;
            DisplayGiftCards = true;
            DisplayRecurring = true;
            CategorySort = CategorySortOrder.None;
            DisplayProductWithChoice = true;
        }

        public bool RoleFiltering { get; set; }
        public string Keyword { get; set; }
        public string ManufacturerId { get; set; }
        public string VendorId { get; set; }
        public ProductStatus Status { get; set; }
        public ProductInventoryStatus InventoryStatus { get; set; }
        public string ProductTypeId { get; set; }
        public string CategoryId { get; set; }
        public string NotCategoryId { get; set; }
        public bool DisplayInactiveProducts { get; set; }
        public bool DisplayBundles { get; set; }
        public bool DisplayGiftCards { get; set; }
        public bool DisplayRecurring { get; set; }
        public bool NotCategorized { get; set; }
        public CategorySortOrder CategorySort { get; set; }
        public bool DisplayProductWithChoice { get; set; }

        public ProductSearchCriteria Clone()
        {
            var result = new ProductSearchCriteria();

            result.CategoryId = CategoryId;
            result.InventoryStatus = InventoryStatus;
            result.Keyword = Keyword;
            result.ManufacturerId = ManufacturerId;
            result.NotCategoryId = NotCategoryId;
            result.ProductTypeId = ProductTypeId;
            result.Status = Status;
            result.VendorId = VendorId;
            result.DisplayInactiveProducts = DisplayInactiveProducts;
            result.DisplayBundles = DisplayBundles;
            result.DisplayGiftCards = DisplayGiftCards;
            result.DisplayRecurring = DisplayRecurring;

            result.DisplayProductWithChoice = DisplayProductWithChoice;
            return result;
        }
    }
}