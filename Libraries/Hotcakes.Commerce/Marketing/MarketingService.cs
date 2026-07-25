#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing
{
    [Serializable]
    public class MarketingService : HccServiceBase
    {
        #region Constructor

        public MarketingService(HccRequestContext context)
            : base(context)
        {
            Promotions = new PromotionRepository(Context);
        }

        #endregion

        #region Properties

        public PromotionRepository Promotions { get; private set; }

        #endregion

        #region Publuic methods

        public Promotion GetPredefinedPromotion(PreDefinedPromotion type)
        {
            var p = new Promotion();

            switch (type)
            {
                case PreDefinedPromotion.CustomSale:
                    p.Name = "New Custom Sale";
                    p.Mode = PromotionType.Sale;
                    break;
                case PreDefinedPromotion.CustomOfferForLineItems:
                    p.Name = "New Offer for Order Items";
                    p.Mode = PromotionType.OfferForLineItems;
                    break;
                case PreDefinedPromotion.CustomOfferForOrder:
                    p.Name = "New Offer for Order";
                    p.Mode = PromotionType.OfferForOrder;
                    p.AddAction(new OrderTotalAdjustment());
                    break;
                case PreDefinedPromotion.CustomOfferForShipping:
                    p.Name = "New Offer for Shipping";
                    p.Mode = PromotionType.OfferForShipping;
                    p.AddAction(new OrderShippingAdjustment());
                    break;
                case PreDefinedPromotion.CustomAffiliatePromotion:
                    p.Name = "New Affiliate Promotion";
                    p.Mode = PromotionType.Affiliate;
                    p.AddAction(new RewardPointsAjustment());
                    break;

                case PreDefinedPromotion.SaleCategories:
                    p.Name = "New Category Sale";
                    p.Mode = PromotionType.Sale;
                    p.AddQualification(new ProductCategory());
                    p.AddAction(new ProductPriceAdjustment());
                    break;
                case PreDefinedPromotion.SaleProducts:
                    p.Name = "New Product Sale";
                    p.Mode = PromotionType.Sale;
                    p.AddQualification(new ProductBvin());
                    p.AddAction(new ProductPriceAdjustment());
                    break;
                case PreDefinedPromotion.SaleProductType:
                    p.Name = "New Product Type Sale";
                    p.Mode = PromotionType.Sale;
                    p.AddQualification(new ProductType());
                    p.AddAction(new ProductPriceAdjustment());
                    break;
                case PreDefinedPromotion.SaleStorewide:
                    p.Name = "New Storewide Sale";
                    p.Mode = PromotionType.Sale;
                    p.AddQualification(new AnyProduct());
                    p.AddAction(new ProductPriceAdjustment());
                    break;
                case PreDefinedPromotion.SaleUser:
                    p.Name = "New Sale By User";
                    p.Mode = PromotionType.Sale;
                    p.AddQualification(new UserIs());
                    p.AddAction(new ProductPriceAdjustment());
                    break;
                case PreDefinedPromotion.SaleUserGroup:
                    p.Name = "New Sale By Price Group";
                    p.Mode = PromotionType.Sale;
                    p.AddQualification(new UserIsInGroup());
                    p.AddAction(new ProductPriceAdjustment());
                    break;
                case PreDefinedPromotion.OrderDiscountCoupon:
                    p.Name = "New Order Discount With Coupon";
                    p.Mode = PromotionType.OfferForOrder;
                    p.AddQualification(new OrderHasCoupon());
                    p.AddAction(new OrderTotalAdjustment());
                    break;
                case PreDefinedPromotion.OrderDiscountUser:
                    p.Name = "New Order Discount by User";
                    p.Mode = PromotionType.OfferForOrder;
                    p.AddQualification(new UserIs());
                    p.AddAction(new OrderTotalAdjustment());
                    break;
                case PreDefinedPromotion.OrderDiscountUserGroup:
                    p.Name = "New Order Discount by Price Group";
                    p.Mode = PromotionType.OfferForOrder;
                    p.AddQualification(new UserIsInGroup());
                    p.AddAction(new OrderTotalAdjustment());
                    break;
                case PreDefinedPromotion.OrderFreeShipping:
                    p.Name = "New Free Shipping Discount";
                    p.Mode = PromotionType.OfferForShipping;
                    p.AddQualification(new AnyShippingMethod());
                    p.AddAction(new OrderShippingAdjustment(AmountTypes.Percent, -100m));
                    break;
                case PreDefinedPromotion.OrderShippingDiscount:
                    p.Name = "New Shipping Discount";
                    p.Mode = PromotionType.OfferForShipping;
                    p.AddQualification(new AnyShippingMethod());
                    p.AddAction(new OrderShippingAdjustment(AmountTypes.Percent, 0m));
                    break;
                case PreDefinedPromotion.OrderFreeShippingByCategory:
                    p.Name = "New Free Shipping By Category";
                    p.Mode = PromotionType.OfferForLineItems;
                    var pq = new LineItemCategory();
                    pq.CategoryNot = true;
                    p.AddQualification(pq);
                    p.AddAction(new LineItemFreeShipping());
                    break;
                case PreDefinedPromotion.CustomOfferForFreeItems:
                    p.Name = "New Offer For Free Items";
                    p.Mode = PromotionType.OfferForFreeItems;
                    break;
            }

            return p;
        }

        public List<Promotion> FindPromotionsWithCouponCode(string code)
        {
            var result = new List<Promotion>();

            // Convert code to upper case once for comparison
            var codeUpper = code.ToUpperInvariant();

            var promos = Promotions.FindAll();
            foreach (var p in promos)
            {
                // Filter coupon qualifications once per promotion
                var couponQualifications = p.Qualifications
                    .Where(y => y.CleanTypeId == PromotionQualificationBase.TypeIdOrderHasCoupon)
                    .Cast<OrderHasCoupon>()
                    .ToList();

                // Check if any coupon qualification matches the code
                if (couponQualifications.Count > 0)
                {
                    foreach (var q in couponQualifications)
                    {
                        // Use Any() instead of Count() for early termination
                        if (q.CurrentCoupons().Any(y => y.ToUpperInvariant() == codeUpper))
                        {
                            result.Add(p);
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public List<string> FindAllCouponCodes()
        {
            var promos = Promotions.FindAll();

            // Use HashSet for O(1) lookups instead of List's O(n) Contains
            var codesSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var p in promos)
            {
                var couponQualifications = p.Qualifications
                    .Where(y => y.CleanTypeId == PromotionQualificationBase.TypeIdOrderHasCoupon)
                    .Cast<OrderHasCoupon>()
                    .ToList();

                if (couponQualifications.Count > 0)
                {
                    foreach (var q in couponQualifications)
                    {
                        foreach (var code in q.CurrentCoupons())
                        {
                            // HashSet automatically handles duplicates
                            codesSet.Add(code);
                        }
                    }
                }
            }

            // Return as list, maintaining uppercase normalization for backwards compatibility
            return codesSet.Select(c => c.ToUpperInvariant()).ToList();
        }

        // This is used to find a date range for when a coupon code might have been active
        // This is useful to limit the number of orders we need to scan for the 
        // Sales by Coupon Report, otherwise we would need to scan every order from 
        // all time
        public PromotionRangeResult FindDateRangeForCouponCode(string code)
        {
            var result = new PromotionRangeResult();

            var matchingPromos = FindPromotionsWithCouponCode(code);
            if (matchingPromos == null || matchingPromos.Count < 1)
            {
                return result;
            }

            // Use single pass to find min and max dates instead of two separate LINQ queries
            var firstPromo = matchingPromos[0];
            var minStartDate = firstPromo.StartDateUtc;
            var maxEndDate = firstPromo.EndDateUtc;

            for (int i = 1; i < matchingPromos.Count; i++)
            {
                var promo = matchingPromos[i];
                if (promo.StartDateUtc < minStartDate)
                {
                    minStartDate = promo.StartDateUtc;
                }
                if (promo.EndDateUtc > maxEndDate)
                {
                    maxEndDate = promo.EndDateUtc;
                }
            }

            result.StartDateUtc = minStartDate;
            result.EndDateUtc = maxEndDate;

            return result;
        }

        public void ApplyAffiliatePromotions(CustomerAccount acc)
        {
            var now = DateTime.UtcNow;
            var promotions = Promotions.FindAllAffiliatePromotions(now);

            foreach (var prom in promotions)
            {
                prom.ApplyToAffiliate(Context, acc, now);
            }
        }

        public void ApplyOffers(Order order, PromotionType mode)
        {
            var offers = Promotions.FindAllPotentiallyActive(DateTime.UtcNow, mode);

            // Cache the property check to avoid repeated lookups during iteration
            var hasNonSaleDiscounts = order.HasAnyNonSaleDiscounts;

            foreach (var offer in offers)
            {
                // do not apply the offer if the current offer is marked as Do Not Combine, 
                // and other offers appear to be applied already
                if (offer.DoNotCombine && hasNonSaleDiscounts) continue;

                offer.ApplyToOrder(Context, order);
            }
        }

        #endregion
    }

    public class PromotionRangeResult
    {
        public PromotionRangeResult()
        {
            StartDateUtc = new DateTime(2010, 1, 1);
            EndDateUtc = new DateTime(3010, 1, 1);
        }

        public DateTime StartDateUtc { get; set; }

        public DateTime EndDateUtc { get; set; }
    }
}