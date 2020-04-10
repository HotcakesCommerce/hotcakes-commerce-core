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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class LineItemCategory : PromotionQualificationBase
    {
        public LineItemCategory()
            : this(string.Empty)
        {
        }

        public LineItemCategory(string categoryId)
        {
            ProcessingCost = RelativeProcessingCost.Highest;
            AddCategoryId(categoryId);
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdLineItemCategory); }
        }

        public bool CategoryNot
        {
            get { return GetSettingAsBool("CategoryNot"); }
            set { SetSetting("CategoryNot", value); }
        }

        public decimal SumAmount
        {
            get { return GetSettingAsDecimal("SumAmount"); }
            set { SetSetting("SumAmount", value); }
        }

        public List<string> CurrentCategoryIds()
        {
            return GetSettingArr("CategoryIds");
        }

        private void SaveCategoryIdsToSettings(List<string> typeIds)
        {
            SetSetting("CategoryIds", typeIds);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Item Category Is ";
            if (CategoryNot)
            {
                result += "Not ";
            }
            result += ":<ul>";
            foreach (var bvin in CurrentCategoryIds())
            {
                var c = app.CatalogServices.Categories.Find(bvin);
                if (c != null)
                {
                    result += "<li>" + c.Name + "<br />";
                    result += "<em>" + c.RewriteUrl + "</em></li>";
                }
            }
            result += "</ul>";
            return result;
        }

        public void AddCategoryId(string id)
        {
            var ids = CurrentCategoryIds();

            if (!ids.Contains(id))
            {
                ids.Add(id);
                SaveCategoryIdsToSettings(ids);
            }
        }

        public void RemoveCategoryId(string id)
        {
            var ids = CurrentCategoryIds();

            if (ids.Contains(id))
            {
                ids.Remove(id);
                SaveCategoryIdsToSettings(ids);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            if (mode == PromotionQualificationMode.LineItems)
            {
                if (context.CurrentlyProcessingLineItem == null) return false;

                var li = context.CurrentlyProcessingLineItem;

                return MeetLineItem(context, li);
            }
            if (mode == PromotionQualificationMode.Orders)
            {
                var items = context.Order.Items;
                return items.Any(i => MeetLineItem(context, i));
            }

            return false;
        }

        private bool MeetLineItem(PromotionContext context, LineItem li)
        {
            if (li == null) return false;
            var productBvin = li.ProductId;

            // Note: this only checks the first 100 categories. You're pretty much insane if you're
            // running a promotion on a product by category and it's in more than 100 categories.
            var assignments = context.HccApp.CatalogServices.CategoriesXProducts.FindForProduct(productBvin, 1, 100);

            var found = false;

            foreach (var cross in assignments)
            {
                var match = cross.CategoryId.Trim().ToLowerInvariant();
                if (CurrentCategoryIds().Contains(match))
                {
                    found = true;
                    break;
                }
            }

            if (CategoryNot)
            {
                if (found) return false;
                return true;
            }
            if (found) return true;
            return false;
        }
    }
}