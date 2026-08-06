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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class SumOrCountOfProducts : PromotionQualificationBase
    {
        public const string TypeIdString = "F8DB4172-1707-46E6-84CF-24009FAE1951";

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public SumOrCountMode CalculationMode
        {
            get { return GetSettingAsBool("CalculationMode") ? SumOrCountMode.CountMode : SumOrCountMode.SumMode; }
            set { SetSetting("CalculationMode", value == SumOrCountMode.CountMode); }
        }

        public decimal SumAmount
        {
            get { return GetSettingAsDecimal("SumAmount"); }
            set { SetSetting("SumAmount", value); }
        }

        public List<string> CategoryIds()
        {
            var arr = GetSettingArr("CategoryIds") ?? new List<string>();
            return arr;
        }

        public void AddCategoryId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var cleaned = id.Trim();
            var ids = CategoryIds();

            if (ids.Contains(cleaned, StringComparer.OrdinalIgnoreCase)) return;

            ids.Add(cleaned);
            SaveCategoryIds(ids);
        }

        public void RemoveCategoryId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var cleaned = id.Trim();
            var ids = CategoryIds();

            var removed = ids.RemoveAll(x => string.Equals(x, cleaned, StringComparison.OrdinalIgnoreCase));
            if (removed > 0)
            {
                SaveCategoryIds(ids);
            }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var modeText = CalculationMode == SumOrCountMode.CountMode ? "Total Quantity " : "Total Price ";
            return $"When {modeText}of Products within Specified Categories >= {SumAmount.ToString(CultureInfo.InvariantCulture)}";
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context?.Order?.Items == null) return false;
            if (mode != PromotionQualificationMode.Orders) return false;

            var filtered = GetFilteredItems(context);

            if (CalculationMode == SumOrCountMode.SumMode)
            {
                return filtered.Sum(i => i.LineTotal) >= SumAmount;
            }

            // CountMode: compare total quantity against SumAmount (SumAmount stored as decimal)
            return filtered.Sum(i => (decimal)i.Quantity) >= SumAmount;
        }

        #region Implementation

        private List<LineItem> GetFilteredItems(PromotionContext context)
        {
            var resItems = new List<LineItem>();
            if (context == null || context.Order == null || context.Order.Items == null) return resItems;

            var specCats = CategoryIds();
            if (specCats == null || specCats.Count == 0) return resItems;

            var specSet = new HashSet<string>(specCats, StringComparer.OrdinalIgnoreCase);

            foreach (var item in context.Order.Items)
            {
                if (item == null) continue;

                var cats = context.HccApp?.CatalogServices?.CategoriesXProducts?.FindForProduct(item.ProductId, 1, 100);
                if (cats == null) continue;

                if (cats.Any(c => specSet.Contains(c.CategoryId)))
                {
                    resItems.Add(item);
                }
            }

            return resItems;
        }

        private void SaveCategoryIds(List<string> typeIds)
        {
            SetSetting("CategoryIds", typeIds);
        }

        #endregion
    }

    public enum SumOrCountMode
    {
        SumMode = 0,
        CountMode = 1
    }
}