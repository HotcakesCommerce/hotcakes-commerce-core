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
    /// <summary>Matches order items by the product type loaded for each product.</summary>
    /// <remarks>Missing product types use the all-zero GUID representation; exclusion mode reverses membership.</remarks>
    public class ProductTypeIs : PromotionIdQualificationBase
    {
        /// <summary>Stable serialized type identifier for this qualification.</summary>
        public const string TypeIdString = "04065B1E-5217-41EF-9801-7AB5244A3373";

        /// <summary>Gets or sets whether configured product types are excluded.</summary>
        public bool IsNotMode
        {
            get
            {
                var all = GetSetting("PrTIsNotMode");

                return all == "1";
            }
            set { SetSetting("PrTIsNotMode", value); }
        }

        /// <summary>Gets the stable product-type order qualification identifier.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        /// <summary>Gets the product-type identifier setting key.</summary>
        public override string IdSettingName
        {
            get { return "ProductTypeIds"; }
        }

        /// <summary>Builds a description of the configured product types.</summary>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Product Type is" + (IsNotMode ? " not" : string.Empty) + ":<ul>";
            var ids = CurrentIds();

            if (ids.Count > 0 && app?.CatalogServices?.ProductTypes != null)
            {
                foreach (var bvin in ids)
                {
                    var c = app.CatalogServices.ProductTypes.Find(bvin);
                    if (c != null)
                    {
                        result += "<li>" + c.ProductTypeName + "<br />";
                    }
                }
            }

            result += "</ul>";

            return result;
        }

        /// <summary>Sets the lowest processing cost during construction.</summary>
        protected override void OnInit()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        /// <summary>Matches the current line item or any order item by loaded product type.</summary>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            var ids = CurrentIds();

            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            if (mode == PromotionQualificationMode.LineItems)
            {
                if (context.CurrentlyProcessingLineItem == null) return false;

                var li = context.CurrentlyProcessingLineItem;

                return MeetLineItem(context, li, ids);
            }
            if (mode == PromotionQualificationMode.Orders)
            {
                var items = context.Order.Items;
                return items.Any(i => MeetLineItem(context, i, ids));
            }

            return false;
        }

        private bool MeetLineItem(PromotionContext context, LineItem li, List<string> ids)
        {
            if (context?.HccApp?.CatalogServices == null) return false;

            var prod = context.HccApp.CatalogServices.Products.FindWithCache(li.ProductId);

            if (prod == null)
            {
                return false;
            }

            var type = string.IsNullOrWhiteSpace(prod.ProductTypeId)
                ? "00000000-0000-0000-0000-000000000000"
                : prod.ProductTypeId;

            var normalized = type.Trim().ToLowerInvariant();

            if (IsNotMode)
            {
                return !ids.Contains(normalized);
            }
            return ids.Contains(normalized);
        }
    }
}