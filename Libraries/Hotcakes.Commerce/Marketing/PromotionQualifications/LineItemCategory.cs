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
using System.Linq;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Matches order line items by their assigned categories.</summary>
    /// <remarks>Category lookup examines only the first 100 assignments and supports a negated mode.</remarks>
    public class LineItemCategory : PromotionQualificationBase
    {
        /// <summary>Initializes an empty category qualification.</summary>
        public LineItemCategory()
            : this(string.Empty)
        {
        }

        /// <summary>Initializes a category qualification with one category identifier.</summary>
        /// <param name="categoryId">The category identifier.</param>
        public LineItemCategory(string categoryId)
        {
            ProcessingCost = RelativeProcessingCost.Highest;
            AddCategoryId(categoryId);
        }

        /// <summary>Gets the stable line-item category qualification identifier.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdLineItemCategory); }
        }

        /// <summary>Gets or sets whether matching means the line item is not in the configured categories.</summary>
        public bool CategoryNot
        {
            get { return GetSettingAsBool("CategoryNot"); }
            set { SetSetting("CategoryNot", value); }
        }

        /// <summary>Gets or sets the persisted sum threshold setting; this rule does not use it during matching.</summary>
        public decimal SumAmount
        {
            get { return GetSettingAsDecimal("SumAmount"); }
            set { SetSetting("SumAmount", value); }
        }

        /// <summary>Gets configured category IDs, normalized to lowercase.</summary>
        public List<string> CurrentCategoryIds()
        {
            var all = GetSetting("CategoryIds") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(all)) return new List<string>();

            return all
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => s.Length > 0)
                .ToList();
        }

        private void SaveCategoryIdsToSettings(IEnumerable<string> typeIds)
        {
            var list = typeIds?.Where(s => !string.IsNullOrWhiteSpace(s))
                       .Select(s => s.Trim().ToLowerInvariant())
                       .ToList() ?? new List<string>();
            var all = list.Count == 0 ? string.Empty : string.Join(",", list);
            SetSetting("CategoryIds", all);
        }

        /// <summary>Builds a category description, resolving names through the catalog.</summary>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Item Category Is ";
            if (CategoryNot)
            {
                result += "Not ";
            }
            result += ":<ul>";

            var ids = CurrentCategoryIds();
            if (ids.Count > 0 && app?.CatalogServices?.Categories != null)
            {
                foreach (var bvin in ids)
                {
                    var c = app.CatalogServices.Categories.Find(bvin);
                    if (c != null)
                    {
                        result += "<li>" + c.Name + "<br />";
                        result += "<em>" + c.RewriteUrl + "</em></li>";
                    }
                }
            }

            result += "</ul>";
            return result;
        }

        /// <summary>Adds a normalized category ID unless it is blank or already present.</summary>
        public void AddCategoryId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var ids = CurrentCategoryIds();
            var normalized = id.Trim().ToLowerInvariant();
            if (ids.Contains(normalized)) return;

            ids.Add(normalized);
            SaveCategoryIdsToSettings(ids);
        }

        /// <summary>Removes a normalized category ID when present.</summary>
        public void RemoveCategoryId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var ids = CurrentCategoryIds();
            var normalized = id.Trim().ToLowerInvariant();
            if (ids.Remove(normalized))
            {
                SaveCategoryIdsToSettings(ids);
            }
        }

        /// <summary>Matches the current line item or any order item by category.</summary>
        /// <remarks>In line-item mode a current line item is required; order mode scans all items.</remarks>
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
            if (context?.HccApp?.CatalogServices?.CategoriesXProducts == null) return false;

            var productBvin = li.ProductId;
            if (string.IsNullOrWhiteSpace(productBvin)) return false;

            // Note: this only checks the first 100 categories. You're pretty much insane if you're
            // running a promotion on a product by category and it's in more than 100 categories.
            var assignments = context.HccApp.CatalogServices.CategoriesXProducts.FindForProduct(productBvin, 1, 100)
                              ?? new List<Catalog.CategoryProductAssociation>();

            var categoryIds = CurrentCategoryIds();
            var found = assignments
                .Select(cross => (cross.CategoryId ?? string.Empty).Trim().ToLowerInvariant())
                .Any(catId => categoryIds.Contains(catId));

            if (CategoryNot)
            {
                return !found;
            }
            return found;
        }
    }
}