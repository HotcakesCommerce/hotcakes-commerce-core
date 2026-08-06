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
    public abstract class HasProductsQualificationBase : PromotionQualificationBase
    {
        #region Constructor

        public HasProductsQualificationBase()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        #endregion
            
        #region Public methods

        public List<string> GetProductIds()
        {
            // Ensure normalization (trim + lower) to match comparisons elsewhere
            return GetSettingArr("products")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant())
                .ToList();
        }

        public void AddProductIds(IEnumerable<string> bvins)
        {
            if (bvins == null) return;
            var existing = GetProductIds();
            var normalized = bvins
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant())
                .Where(s => !existing.Contains(s))
                .ToList();

            if (normalized.Count == 0) return;

            var combined = existing.Concat(normalized).ToList();
            AddSettingItems("products", combined);
        }

        public void RemoveProductId(string bvin)
        {
            if (string.IsNullOrWhiteSpace(bvin)) return;
            var normalized = bvin.Trim().ToLowerInvariant();
            RemoveSettingItem("products", normalized);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Line Item is:<ul>";
            var ids = GetProductIds();
            if (ids.Count > 0 && app?.CatalogServices?.Products != null)
            {
                foreach (var id in ids)
                {
                    var p = app.CatalogServices.Products.FindWithCache(id);
                    if (p != null)
                    {
                        result += "<li>[" + p.Sku + "] " + p.ProductName + "</li>";
                    }
                }
            }
            result += "</ul>";
            return result;
        }

        #endregion
    }

    public class ProductBvin : HasProductsQualificationBase
    {
        public ProductBvin()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        public ProductBvin(string bvin)
            : this(new List<string> { bvin })
        {
        }

        public ProductBvin(List<string> bvins)
            : this()
        {
            AddProductIds(bvins);
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdProductBvin); }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Product is:<ul>";
            var ids = GetProductIds();
            if (ids.Count > 0 && app?.CatalogServices?.Products != null)
            {
                foreach (var bvin in ids)
                {
                    var p = app.CatalogServices.Products.FindWithCache(bvin);
                    if (p != null)
                    {
                        result += "<li>[" + p.Sku + "] " + p.ProductName + "</li>";
                    }
                }
            }
            result += "</ul>";
            return result;
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Products) return false;
            if (context == null) return false;
            if (context.Product == null) return false;
            if (context.UserPrice == null) return false;

            var match = (context.Product.Bvin ?? string.Empty).Trim().ToLowerInvariant();

            return GetProductIds().Contains(match);
        }
    }
}