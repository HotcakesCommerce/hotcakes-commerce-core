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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class OrderHasNotProducts : PromotionIdQualificationBase
    {
        #region Constants

        public const string TypeIdString = "FAA3EF10-B54E-466D-9495-3990E1828A39";

        #endregion

        #region Init

        public OrderHasNotProducts()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        #endregion

        #region Helper Methods

        private bool MatchAny(List<LineItem> items, List<string> productIds)
        {
            foreach (var li in items)
            {
                if (productIds.Contains(li.ProductId.Trim().ToLowerInvariant()))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Overrides

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When order doesn't have";

            result += ":<ul>";

            foreach (var bvin in CurrentIds())
            {
                var p = app.CatalogServices.Products.FindWithCache(bvin);
                if (p != null)
                {
                    result += "<li>[" + p.Sku + "] " + p.ProductName + "</li>";
                }
            }
            result += "</ul>";
            return result;
        }

        public override string IdSettingName
        {
            get { return "ProductNotIds"; }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Orders &&
                mode != PromotionQualificationMode.LineItems) return false;
            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            return MatchAny(context.Order.Items, CurrentIds());
        }

        #endregion
    }
}