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
            var result = new List<string>();
            var all = GetSetting("methodids");
            var parts = all.Split(',');
            foreach (var s in parts)
            {
                if (s != string.Empty)
                {
                    result.Add(s.Trim().ToUpperInvariant());
                }
            }
            return result;
        }

        private void SaveMethodIdsToSettings(List<string> methodIds)
        {
            var all = string.Empty;
            foreach (var s in methodIds)
            {
                if (s != string.Empty)
                {
                    all += s.Trim().ToUpperInvariant() + ",";
                }
            }
            all = all.TrimEnd(',');
            SetSetting("methodids", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            if (MethodIds().Count < 1)
            {
                return "Make Qualifying Items Free Shipping";
            }
            var methods = app.OrderServices.ShippingMethods.FindAll(app.CurrentStore.Id);

            var result = "Make Qualifying Items Free Shipping:<ul>";
            foreach (var itemid in MethodIds())
            {
                var displayName = itemid;

                if (methods != null)
                {
                    var m = methods.SingleOrDefault(y => y.Bvin.ToUpperInvariant() == itemid.ToUpperInvariant());
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
            var _ItemIds = MethodIds();

            var possible = itemid.Trim().ToUpperInvariant();
            if (possible == string.Empty) return;
            if (_ItemIds.Contains(possible)) return;
            _ItemIds.Add(possible);
            SaveMethodIdsToSettings(_ItemIds);
        }

        public void RemoveItemId(string itemid)
        {
            var _ItemIds = MethodIds();
            if (_ItemIds.Contains(itemid.Trim().ToUpperInvariant()))
            {
                _ItemIds.Remove(itemid.Trim().ToUpperInvariant());
                SaveMethodIdsToSettings(_ItemIds);
            }
        }


        public override bool ApplyAction(PromotionContext context)
        {
            if (context.Mode != PromotionType.OfferForLineItems) return false;

            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            if (context.CurrentlyProcessingLineItem == null) return false;

            var li = context.CurrentlyProcessingLineItem;
            li.IsMarkedForFreeShipping = true;
            if (MethodIds().Count > 0)
            {
                foreach (var methodId in MethodIds())
                {
                    if (!li.FreeShippingMethodIds.Contains(methodId))
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
                    currentShippingMethodID = context.CurrentShippingMethodId;
                }
                else
                {
                    baseShippingRate = context.Order.TotalShippingBeforeDiscounts;
                    currentShippingMethodID = context.Order.ShippingMethodId;
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