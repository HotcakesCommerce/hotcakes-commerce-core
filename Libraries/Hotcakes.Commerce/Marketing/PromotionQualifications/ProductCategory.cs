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
using System.Collections.Generic;

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
            var result = new List<string>();
            var all = GetSetting("CategoryIds");
            var parts = all.Split(',');
            foreach (var s in parts)
            {
                if (s != string.Empty)
                {
                    result.Add(s);
                }
            }
            return result;
        }

        private void SaveCategoryIdsToSettings(List<string> typeIds)
        {
            var all = string.Empty;
            foreach (var s in typeIds)
            {
                if (s != string.Empty)
                {
                    all += s + ",";
                }
            }
            all = all.TrimEnd(',');
            SetSetting("CategoryIds", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Product Category Is:<ul>";
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
            var _Ids = CurrentCategoryIds();

            var possible = id.Trim().ToLowerInvariant();
            if (possible == string.Empty) return;
            if (_Ids.Contains(possible)) return;
            _Ids.Add(possible);
            SaveCategoryIdsToSettings(_Ids);
        }

        public void RemoveCategoryId(string id)
        {
            var _Ids = CurrentCategoryIds();
            if (_Ids.Contains(id))
            {
                _Ids.Remove(id);
                SaveCategoryIdsToSettings(_Ids);
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