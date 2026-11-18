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
    /// <summary>Provides normalized product-ID settings and description behavior.</summary>
    public abstract class HasProductsQualificationBase : PromotionQualificationBase
    {
        #region Constructor

        /// <summary>Initializes the base with lower processing cost.</summary>
        public HasProductsQualificationBase()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        #endregion
            
        #region Public methods

        /// <summary>Gets configured product BVINs, trimmed and normalized to lowercase.</summary>
        public List<string> GetProductIds()
        {
            // Ensure normalization (trim + lower) to match comparisons elsewhere
            return GetSettingArr("products")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToLowerInvariant())
                .ToList();
        }

        /// <summary>Adds unique non-empty product BVINs to the products setting.</summary>
        /// <param name="bvins">The product BVINs to add.</param>
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

            AddSettingItems("products", normalized);
        }

        /// <summary>Removes one product BVIN from the products setting.</summary>
        /// <param name="bvin">The product BVIN to remove.</param>
        public void RemoveProductId(string bvin)
        {
            if (string.IsNullOrWhiteSpace(bvin)) return;
            var normalized = bvin.Trim().ToLowerInvariant();
            RemoveSettingItem("products", normalized);
        }

        /// <summary>Builds a description listing the configured product IDs.</summary>
        /// <param name="app">The application context used to resolve products.</param>
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

    /// <summary>Matches a product by its BVIN in product evaluation mode.</summary>
    public class ProductBvin : HasProductsQualificationBase
    {
        /// <summary>Initializes an empty product-BVIN qualification.</summary>
        public ProductBvin()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        /// <summary>Initializes a qualification containing one product BVIN.</summary>
        /// <param name="bvin">The product BVIN.</param>
        public ProductBvin(string bvin)
            : this(new List<string> { bvin })
        {
        }

        /// <summary>Initializes a qualification containing the supplied product BVINs.</summary>
        /// <param name="bvins">The product BVINs.</param>
        public ProductBvin(List<string> bvins)
            : this()
        {
            AddProductIds(bvins);
        }

        /// <summary>Gets the stable product-BVIN qualification identifier.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdProductBvin); }
        }

        /// <summary>Builds a description listing the configured product IDs.</summary>
        /// <param name="app">The application context used to resolve products.</param>
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

        /// <summary>Matches a product context whose normalized BVIN is configured.</summary>
        /// <param name="context">The promotion context.</param>
        /// <param name="mode">The evaluation scope; only product scope matches.</param>
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