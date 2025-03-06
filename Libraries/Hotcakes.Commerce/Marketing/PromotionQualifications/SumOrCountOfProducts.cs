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
using System.Collections.Generic;
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
            return GetSettingArr("CategoryIds");
        }

        public void AddCategoryId(string id)
        {
            var ids = CategoryIds();

            if (!ids.Contains(id))
            {
                ids.Add(id);
                SaveCategoryIds(ids);
            }
        }

        public void RemoveCategoryId(string id)
        {
            var ids = CategoryIds();

            if (ids.Contains(id))
            {
                ids.Remove(id);
                SaveCategoryIds(ids);
            }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When " + (CalculationMode == SumOrCountMode.CountMode ? "Total Quantity " : "Total Price ");
            result += "of Products within Specified Categories >= ";
            result += SumAmount.ToString();
            return result;
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode == PromotionQualificationMode.Orders)
            {
                var resItems = GetFilteredItems(context);
                if (CalculationMode == SumOrCountMode.SumMode)
                {
                    return resItems.Sum(i => i.LineTotal) >= SumAmount;
                }
                return resItems.Sum(i => i.Quantity) >= SumAmount;
            }

            return false;
        }

        #region Implementation

        private List<LineItem> GetFilteredItems(PromotionContext context)
        {
            var resItems = new List<LineItem>();
            var specCats = CategoryIds();

            foreach (var item in context.Order.Items)
            {
                var cats = context.HccApp.CatalogServices.CategoriesXProducts.FindForProduct(item.ProductId, 1, 100);
                if (cats.Any(c => specCats.Contains(c.CategoryId)))
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