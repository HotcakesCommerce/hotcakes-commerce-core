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

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class ProductCategory : PromotionQualificationBase
    {
        public ProductCategory() : this(string.Empty)
        {
        }

        public ProductCategory(string categoryId)
        {
            ProcessingCost = RelativeProcessingCost.Highest;
            AddCategoryId(categoryId);
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdProductCategory); }
        }

        public List<string> CurrentCategoryIds()
        {
            var all = GetSetting("CategoryIds") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(all)) return new List<string>();

            return all
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => s.Length > 0)
                .ToList();
        }

        private void SaveCategoryIdsToSettings(IEnumerable<string> typeIds)
        {
            var list = typeIds?.Where(s => !string.IsNullOrWhiteSpace(s)).ToList() ?? new List<string>();
            var all = list.Count == 0 ? string.Empty : string.Join(",", list);
            SetSetting("CategoryIds", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Product Category Is:<ul>";
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

        public void AddCategoryId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var ids = CurrentCategoryIds();
            var possible = id.Trim().ToLowerInvariant();
            if (ids.Contains(possible)) return;

            ids.Add(possible);
            SaveCategoryIdsToSettings(ids);
        }

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

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Products) return false;
            if (context == null) return false;
            if (context.Mode != PromotionType.Sale) return false;
            if (context.Product == null) return false;
            if (context.UserPrice == null) return false;
            if (context.HccApp == null) return false;

            // Note: this only checks the first 100 categories. You're pretty much insane if you're
            // running a promotion on a product by category and it's in more than 100 categories.
            var assignments
                = context.HccApp.CatalogServices.CategoriesXProducts.FindForProduct(context.Product.Bvin, 1, 100);

            foreach (var cross in assignments)
            {
                var match = cross.CategoryId.Trim().ToLowerInvariant();
                if (CurrentCategoryIds().Contains(match)) return true;
            }
            return false;
        }
    }
}