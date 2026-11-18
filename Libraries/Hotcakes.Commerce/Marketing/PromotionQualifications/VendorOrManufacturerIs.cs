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
using System.Text;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Matches order products by configured vendor or manufacturer identifiers.</summary>
    /// <remarks>
    /// In normal mode, a product matches when either identifier is configured. In negative mode, both identifiers
    /// must be absent from the configured set. Evaluation supports line-item and order scopes only.
    /// </remarks>
    public class VendorOrManufacturerIs : PromotionIdQualificationBase
    {
        /// <summary>Stable serialized type identifier for this qualification.</summary>
        public const string TypeIdString = "C1DA4482-4A3A-4F7A-AD9F-A669E5CA3BAD";

        /// <summary>Gets or sets whether matching is negated.</summary>
        /// <remarks>The value is serialized under <c>VoMIsNotMode</c> as <c>1</c> or <c>0</c>; negative mode requires neither vendor nor manufacturer to match.</remarks>
        public bool IsNotMode
        {
            get
            {
                var all = GetSetting("VoMIsNotMode");

                return all == "1";
            }
            set { SetSetting("VoMIsNotMode", value); }
        }

        /// <summary>Gets the stable serialized type identifier for this qualification.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        /// <summary>Gets the persisted settings key containing vendor and manufacturer identifiers.</summary>
        public override string IdSettingName
        {
            get { return "VendorManufacturerIds"; }
        }

        /// <summary>Builds the display description with resolved manufacturer and vendor names.</summary>
        /// <param name="app">The application context used to resolve manufacturer and vendor identifiers.</param>
        /// <returns>Display text listing resolved configured entities and the active positive or negative mode.</returns>
        /// <remarks>Each identifier is attempted as a manufacturer first; it is treated as a vendor candidate only when that lookup fails.</remarks>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var sb = new StringBuilder();
            sb.Append("When Vendor/Manufacturer Is ");
            sb.Append(IsNotMode ? "Not" : string.Empty);
            sb.Append(":<ul>");

            var ids = CurrentIds() ?? new List<string>();
            var vendorCandidateIds = new List<string>();

            foreach (var bvin in ids)
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var manufacturer = app.ContactServices.Manufacturers.Find(bvin);
                if (manufacturer != null)
                {
                    sb.Append("<li>");
                    sb.Append(manufacturer.DisplayName);
                    sb.Append("<br />");
                }
                else
                {
                    vendorCandidateIds.Add(bvin);
                }
            }

            foreach (var bvin in vendorCandidateIds)
            {
                var vendor = app.ContactServices.Vendors.Find(bvin);
                if (vendor != null)
                {
                    sb.Append("<li>");
                    sb.Append(vendor.DisplayName);
                    sb.Append("<br />");
                }
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>Initializes this ID-based qualification with the lowest relative processing cost.</summary>
        protected override void OnInit()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        /// <summary>Evaluates configured vendor or manufacturer matching for a line item or order.</summary>
        /// <param name="context">The promotion context containing the order, current line item, and catalog services.</param>
        /// <param name="mode">The evaluation scope; <see cref="PromotionQualificationMode.LineItems"/> checks the current line item and <see cref="PromotionQualificationMode.Orders"/> scans order items.</param>
        /// <returns><c>true</c> when the selected scope contains a matching product under the active positive or negative mode; otherwise <c>false</c>.</returns>
        /// <remarks>Null context, order, items, current line item, or product data fails closed. Other modes are unsupported and return <c>false</c>.</remarks>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            var ids = CurrentIds();

            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            if (mode == PromotionQualificationMode.LineItems)
            {
                if (context.CurrentlyProcessingLineItem == null) return false;
                return MeetLineItem(context, context.CurrentlyProcessingLineItem, ids);
            }
    
            if (mode == PromotionQualificationMode.Orders)
            {
                var items = context.Order.Items;
                return items.Any(i => MeetLineItem(context, i, CurrentIds()));
            }

            return false;
        }

        private bool MeetLineItem(PromotionContext context, LineItem li, List<string> ids)
        {
            if (li == null) return false;
            if (context?.HccApp?.CatalogServices == null) return false;

            var prod = context.HccApp.CatalogServices.Products.FindWithCache(li.ProductId);
            if (prod == null) return false;

            var effectiveIds = (ids ?? Enumerable.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x));
            var idSet = new HashSet<string>(effectiveIds, StringComparer.OrdinalIgnoreCase);

            bool IdInSet(string candidate)
            {
                return !string.IsNullOrWhiteSpace(candidate) && idSet.Contains(candidate);
            }

            if (IsNotMode)
            {
                return !IdInSet(prod.VendorId) && !IdInSet(prod.ManufacturerId);
            }

            return IdInSet(prod.VendorId) || IdInSet(prod.ManufacturerId);
        }
    }
}