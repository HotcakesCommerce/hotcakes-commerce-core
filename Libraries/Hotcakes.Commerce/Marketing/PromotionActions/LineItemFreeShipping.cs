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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class LineItemFreeShipping : PromotionActionBase
    {
        public const string TypeIdString = "5f7de3ab-c551-47f3-8ff6-dfe6586be867";

        public LineItemFreeShipping()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public List<string> MethodIds()
        {
            var all = GetSetting("methodids") ?? string.Empty;
            var parts = all.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return parts
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.ToUpperInvariant())
                .ToList();
        }

        private void SaveMethodIdsToSettings(IEnumerable<string> methodIds)
        {
            var normalized = methodIds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpperInvariant());
            var all = string.Join(",", normalized);
            SetSetting("methodids", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var ids = MethodIds();
            if (ids.Count < 1)
            {
                return "Make Qualifying Items Free Shipping";
            }

            var methods = app.OrderServices.ShippingMethods.FindAll(app.CurrentStore.Id);

            var result = "Make Qualifying Items Free Shipping:<ul>";
            foreach (var itemid in ids)
            {
                var displayName = itemid;

                if (methods != null)
                {
                    var m = methods.SingleOrDefault(y => string.Equals(y.Bvin, itemid, StringComparison.OrdinalIgnoreCase));
                    if (m != null)
                    {
                        displayName = m.Name;
                    }
                }
                result += "<li>" + displayName + "</li>";
            }
            result += "</ul>";
            return result;
        }


        public void AddItemId(string itemid)
        {
            if (string.IsNullOrWhiteSpace(itemid)) return;

            var ids = MethodIds();
            var possible = itemid.Trim().ToUpperInvariant();
            if (ids.Contains(possible)) return;
            ids.Add(possible);
            SaveMethodIdsToSettings(ids);
        }

        public void RemoveItemId(string itemid)
        {
            if (string.IsNullOrWhiteSpace(itemid)) return;

            var ids = MethodIds();
            var normalized = itemid.Trim().ToUpperInvariant();
            if (ids.Contains(normalized))
            {
                ids.Remove(normalized);
                SaveMethodIdsToSettings(ids);
            }
        }


        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.Mode != PromotionType.OfferForLineItems) return false;

            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            if (context.CurrentlyProcessingLineItem == null) return false;

            var li = context.CurrentlyProcessingLineItem;
            li.IsMarkedForFreeShipping = true;

            var methodIds = MethodIds();
            if (methodIds.Count > 0)
            {
                var normalizedMethodIds = new HashSet<string>(methodIds, StringComparer.OrdinalIgnoreCase);
                foreach (var methodId in normalizedMethodIds)
                {
                    if (!li.FreeShippingMethodIds.Any(x => string.Equals(x, methodId, StringComparison.OrdinalIgnoreCase)))
                    {
                        li.FreeShippingMethodIds.Add(methodId.ToUpperInvariant());
                    }
                }
            }

            //Changes to have free shipping available for single item in cart by promotion set for "Order Items" - 9May2016-Tushar
            if (context.Order.Items.Count == 1 || context.Order.IsOrderHasAllItemsQualifiedFreeShipping())
            {
                decimal baseShippingRate = 0;
                var currentShippingMethodID = string.Empty;
                if (!string.IsNullOrWhiteSpace(context.CurrentShippingMethodId))
                {
                    baseShippingRate = context.AdjustedShippingRate;
                    currentShippingMethodID = context.CurrentShippingMethodId ?? string.Empty;
                }
                else
                {
                    baseShippingRate = context.Order.TotalShippingBeforeDiscounts;
                    currentShippingMethodID = context.Order.ShippingMethodId ?? string.Empty;
                }


                if (baseShippingRate > 0 &&
                    li.FreeShippingMethodIds.Contains(currentShippingMethodID.ToUpperInvariant()))
                {
                    decimal adjustment = 0;
                    adjustment = Money.GetDiscountAmountByPercent(baseShippingRate, -100);

                    if (!string.IsNullOrWhiteSpace(context.CurrentShippingMethodId))
                        context.AdjustedShippingRate = context.AdjustedShippingRate + adjustment;
                    else
                    {
                        var discountApplied =
                            context.Order.ShippingDiscountDetails
                                .FirstOrDefault(p => p.PromotionId == context.PromotionId);
                        if (discountApplied == null)
                            context.Order.AddShippingDiscount(adjustment, context.CustomerDescription,
                                context.PromotionId, Id);
                    }
                }
            }
            //End changes to have free shipping available for single item in cart by promotion set for "Order Items"- 9May2016-Tushar

            // don't try to add it again, if it's already there
            if (li.DiscountDetails.Any(d => d.PromotionId == context.PromotionId)) return true;

            li.DiscountDetails.Add(new DiscountDetail
            {
                Amount = 0,
                Description = context.CustomerDescription,
                PromotionId = context.PromotionId,
                ActionId = Id,
                DiscountType = PromotionType.OfferForShipping
            });

            return true;
        }
    }
}