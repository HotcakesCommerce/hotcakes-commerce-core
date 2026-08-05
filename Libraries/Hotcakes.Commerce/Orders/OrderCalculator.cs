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

using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.Geography;
using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Commerce.Orders
{
    public class OrderCalculator : IOrderCalculator
    {
        #region Constructor

        public OrderCalculator(HotcakesApplication app)
        {
            SkipRepricing = false;
            SkipDiscounts = false;
            _app = app;
        }

        #endregion

        #region Public methods

        public bool Calculate(Order order)
        {
            ResetValues(order);

            if (!SkipRepricing)
            {
                // Reprice items for user and apply 'Sale' promotion
                RepriceItemsForUser(order);
            }

            if (!SkipDiscounts)
            {
                ApplyVolumeDiscounts(order);
                CalculateItemsPrices(order);

                ApplyOffers(order, PromotionType.OfferForFreeItems);

                //Comment this as shipping line items free caluclation cannot be done without shipping method id available.
                ApplyOffers(order, PromotionType.OfferForLineItems);
                CalculateItemsPrices(order);

                ApplyOffers(order, PromotionType.OfferForOrder);
            }
            else
            {
                CalculateItemsPrices(order);
            }

            // Calculate Handling, merge with Shipping for display
            CalculateHandlingAmount(order);
            CalculateShipping(order);

            if (!SkipDiscounts)
            {
                ApplyOffers(order, PromotionType.OfferForShipping);
            }

            //Add apply offer after shipping amount calculated.
            ApplyOffers(order, PromotionType.OfferForLineItems);

            // Distribute shipping cost to each line item that is shippable
            CalculateLineItemShippingPortions(order);
            CalculateTaxes(order);

            // Distribute order discounts between lineitems if order is recurring
            if (order.IsRecurring)
            {
                DistributeShipping(order);
                DistributeOrderDiscounts(order);
            }

            //Add apply upcharges to de order
            ApplyUpcharges(order);

            return true;
        }

        #endregion

        #region Constants

        private const string VOLUME_DISCOUNT_GLOBAL_TEXT = "Volume Discount";
        private const string VOLUME_DISCOUNT_LOCALIZATION_KEY = "VolumeDiscount";
        private const string PERCENT_CHANGED_FORMAT = "p0";
        private const string DEVELOPER_ID = "hcc";
        private const string FREE_ITEMS_KEY = "outfreeitems";
        private const string FREE_PROMOTIONS_KEY = "freePromotions";

        #endregion

        #region Fields

        private readonly HotcakesApplication _app;
        public bool SkipRepricing { get; set; }
        public bool SkipDiscounts { get; set; }

        #endregion

        #region Implementation

        private void ResetFreeItemsFlag(Order order)
        {
            if (order != null)
            {
                foreach (var prop in order.CustomProperties)
                {
                    if (prop.DeveloperId == DEVELOPER_ID)
                    {
                        if (prop.Key == FREE_ITEMS_KEY || prop.Key == FREE_PROMOTIONS_KEY)
                        {
                            prop.Value = string.Empty;
                        }
                    }
                }
            }
        }

        private void ResetValues(Order order)
        {
            ResetFreeItemsFlag(order);

            order.TotalShippingBeforeDiscounts = 0;
            order.ItemsTax = 0;
            order.ShippingTaxRate = 0;
            order.ShippingTax = 0;
            order.TotalTax = 0;
            order.TotalHandling = 0;

            order.ClearDiscounts();

            order.ApplyVATRules = _app.CurrentStore.Settings.ApplyVATRules;
        }

        private void RepriceItemsForUser(Order order)
        {
            foreach (var li in order.Items)
            {
                if (!li.IsUserSuppliedPrice && !li.IsGiftCard)
                {
                    var price = _app.PriceProduct(li.ProductId, order.UserID, li.SelectionData);

                    // Null check because if the item isn't in the catalog
                    // we will get back a null user specific price. 
                    //
                    // TODO: In the future it may be a good idea to add an option
                    // allowing merchant to select if they would like to allow
                    // items not in the catalog to exist in carts or if we should
                    // just remove items from the cart with a warning here.
                    if (price == null) continue;

                    li.BasePricePerItem = price.BasePrice;

                    // Cache count and discount details list reference
                    var priceDiscountDetails = price.DiscountDetails;
                    var discountDetailsCount = priceDiscountDetails.Count;

                    if (discountDetailsCount > 0)
                    {
                        var liQuantity = li.Quantity;
                        for (var i = 0; i < discountDetailsCount; i++)
                        {
                            var discount = priceDiscountDetails[i];
                            li.DiscountDetails.Add(new DiscountDetail
                            {
                                Amount = discount.Amount * liQuantity,
                                Description = discount.Description,
                                DiscountType = PromotionType.Sale
                            });
                        }
                    }
                }
            }
        }

        private void ApplyVolumeDiscounts(Order order)
        {
            // Count up how many of each item in order
            var quantityMap = new Dictionary<string, int>();
            var orderItems = order.Items;
            var itemsCount = orderItems.Count;

            for (var i = 0; i < itemsCount; i++)
            {
                var item = orderItems[i];
                if (!item.IsUserSuppliedPrice && !item.IsGiftCard)
                {
                    if (quantityMap.ContainsKey(item.ProductId))
                    {
                        quantityMap[item.ProductId] += item.Quantity;
                    }
                    else
                    {
                        quantityMap.Add(item.ProductId, item.Quantity);
                    }
                }
            }

            if (quantityMap.Count == 0) return;

            // Cache the volume discount global text lookup
            var volumeDiscountGlobalText = GlobalLocalization.GetString(VOLUME_DISCOUNT_LOCALIZATION_KEY);
            if (string.IsNullOrEmpty(volumeDiscountGlobalText))
            {
                volumeDiscountGlobalText = VOLUME_DISCOUNT_GLOBAL_TEXT;
            }

            var catalogServices = _app.CatalogServices;

            // Check for discounts on each item
            foreach (var kvp in quantityMap)
            {
                var productId = kvp.Key;
                var quantity = kvp.Value;

                var volumeDiscounts = catalogServices.VolumeDiscounts.FindByProductId(productId);

                if (volumeDiscounts.Count == 0) continue;

                // Locate the correct discount in the chart of discounts
                var volumeDiscountToApply = volumeDiscounts.LastOrDefault(vd => quantity >= vd.Qty);

                if (volumeDiscountToApply == null) continue;

                // Cache product lookup
                var p = catalogServices.Products.FindWithCache(productId);
                if (p == null) continue;

                var sitePrice = p.SitePrice;
                var volumeDiscountAmount = volumeDiscountToApply.Amount;

                // Now we have to go through the entire order and discount all items
                // Traversal through all items is required because few line items of same product may be present in the cart
                for (var i = 0; i < itemsCount; i++)
                {
                    var item = orderItems[i];
                    if (item.ProductId == productId)
                    {
                        var itemQuantity = item.Quantity;
                        var adjustedPricePerItem = item.BasePricePerItem + item.TotalDiscounts() / itemQuantity;
                        var alreadyDiscounted = sitePrice > adjustedPricePerItem;
                        var hasDiscounts = item.DiscountDetails.Count > 0;

                        if (!alreadyDiscounted || !hasDiscounts)
                        {
                            // item isn't discounted yet so apply the exact price the merchant set
                            var toDiscount = -1 * (adjustedPricePerItem - volumeDiscountAmount);
                            toDiscount = toDiscount * itemQuantity;

                            item.DiscountDetails.Add(new DiscountDetail
                            {
                                Amount = toDiscount,
                                Description = volumeDiscountGlobalText,
                                DiscountType = PromotionType.VolumeDiscount
                            });
                        }
                        else
                        {
                            // item is already discounted (probably by user group) so figure out
                            // the percentage of volume discount instead
                            var originalPriceChange = sitePrice - volumeDiscountAmount;
                            var percentChange = originalPriceChange / sitePrice;
                            var newDiscount = -1 * percentChange * adjustedPricePerItem;
                            newDiscount = newDiscount * itemQuantity;

                            item.DiscountDetails.Add(new DiscountDetail
                            {
                                Amount = newDiscount,
                                Description =
                                    percentChange.ToString(PERCENT_CHANGED_FORMAT) + volumeDiscountGlobalText,
                                DiscountType = PromotionType.VolumeDiscount
                            });
                        }
                    }
                }
            }
        }

        private void ApplyUpcharges(Order order)
        {
            var catalogServices = _app.CatalogServices;
            var orderItems = order.Items;
            var itemsCount = orderItems.Count;

            for (var i = 0; i < itemsCount; i++)
            {
                var item = orderItems[i];
                if (item.IsUpchargeAllowed)
                {
                    var p = catalogServices.Products.FindWithCache(item.ProductId);

                    if (p != null && p.AllowUpcharge)
                    {
                        item.LineTotal = item.LineTotal + item.TotalUpcharge();
                    }
                }
            }
        }

        private void CalculateItemsPrices(Order order)
        {
            var orderItems = order.Items;
            var itemsCount = orderItems.Count;

            for (var i = 0; i < itemsCount; i++)
            {
                var li = orderItems[i];
                li.LineTotal = li.LineTotalWithoutDiscounts + li.TotalDiscounts();

                if (li.LineTotal < 0) li.LineTotal = 0;

                li.AdjustedPricePerItem = Money.RoundCurrency(li.LineTotal / li.Quantity);
            }
        }

        private void ApplyOffers(Order order, PromotionType mode)
        {
            if (mode == PromotionType.OfferForShipping)
            {
                string[] skipOfferForShipping = { ShippingMethod.MethodUnknown, ShippingMethod.MethodToBeDetermined };

                if (skipOfferForShipping.Contains(order.ShippingMethodId)) return;
            }

            _app.MarketingServices.ApplyOffers(order, mode);
        }

        private void CalculateHandlingAmount(Order order)
        {
            decimal totalHandling = 0;
            var store = _app.CurrentStore;
            var storeSettings = store.Settings;
            var handlingType = storeSettings.HandlingType;

            if (handlingType == (int)HandlingMode.PerItem)
            {
                decimal amount = 0;
                var handlingNonShipping = storeSettings.HandlingNonShipping;
                var orderItems = order.Items;
                var itemsCount = orderItems.Count;

                for (var i = 0; i < itemsCount; i++)
                {
                    var item = orderItems[i];
                    if (item.IsNonShipping)
                    {
                        if (handlingNonShipping)
                        {
                            amount += item.Quantity;
                        }
                    }
                    else if (item.ShippingCharge == ShippingChargeType.ChargeHandling ||
                             item.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling)
                    {
                        amount += item.Quantity;
                    }
                }

                totalHandling = storeSettings.HandlingAmount * amount;
            }
            else if (handlingType == (int)HandlingMode.PerOrder)
            {
                var handlingNonShipping = storeSettings.HandlingNonShipping;
                var orderItems = order.Items;
                var itemsCount = orderItems.Count;

                // charge handling if there aren't non shipping items
                if (handlingNonShipping)
                {
                    if (itemsCount > 0)
                    {
                        totalHandling = RecalculateHandlingPerLineItemSettings(storeSettings.HandlingAmount, orderItems);
                    }
                }
                else
                {
                    var hasChargeableItems = false;
                    for (var i = 0; i < itemsCount; i++)
                    {
                        var item = orderItems[i];
                        if (!item.IsNonShipping &&
                            (item.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling ||
                             item.ShippingCharge == ShippingChargeType.ChargeHandling))
                        {
                            hasChargeableItems = true;
                            break;
                        }
                    }

                    if (hasChargeableItems)
                    {
                        totalHandling = RecalculateHandlingPerLineItemSettings(storeSettings.HandlingAmount, orderItems);
                    }
                }
            }

            order.TotalHandling = totalHandling;
        }

        private decimal RecalculateHandlingPerLineItemSettings(decimal perOrderHandlingAmount, List<LineItem> lineItems)
        {
            // determine how many line items allow handling to be charged
            var lineItemsCount = lineItems.Count;
            var itemsToChargeFor = 0;

            for (var i = 0; i < lineItemsCount; i++)
            {
                var item = lineItems[i];
                if (item.ShippingCharge == ShippingChargeType.ChargeHandling ||
                    item.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling)
                {
                    itemsToChargeFor++;
                }
            }

            if (itemsToChargeFor < lineItemsCount)
            {
                // determine what the per-item handling fee is
                var handlingPerItem = perOrderHandlingAmount / lineItemsCount;

                // determine the pro-rated handling fee for items that allow handling charges
                return Money.RoundCurrency(handlingPerItem * itemsToChargeFor);
            }
            // no changes in the handling are necessary
            return perOrderHandlingAmount;
        }

        private void CalculateShipping(Order order)
        {
            ShippingRateDisplay rate = null;

            if (!string.IsNullOrEmpty(order.ShippingMethodId) && order.HasShippingItems)
            {
                rate = _app.OrderServices.FindShippingRateByUniqueKey(order);
            }

            if (rate != null)
            {
                order.ShippingMethodDisplayName = rate.DisplayName ?? string.Empty;
                order.ShippingProviderId = rate.ProviderId;
                order.ShippingProviderServiceCode = rate.ProviderServiceCode;
                order.TotalShippingBeforeDiscounts = Money.RoundCurrency(rate.Rate);
            }
            else
            {
                order.ClearShippingPricesAndMethod();
                order.TotalShippingBeforeDiscounts = order.TotalHandling;
            }

            if (order.TotalShippingBeforeDiscountsOverride >= 0)
            {
                order.TotalShippingBeforeDiscounts = order.TotalShippingBeforeDiscountsOverride;
            }
        }

        /// <summary>
        ///     Distributes shipping cost to each line item that is shippable
        /// </summary>
        /// <param name="order">The order.</param>
        private void CalculateLineItemShippingPortions(Order order)
        {
            var totalShipping =
                Money.RoundCurrency(order.TotalShippingBeforeDiscounts + order.TotalShippingDiscounts -
                                    order.TotalHandling);
            var totalHandling = order.TotalHandling;

            if (totalShipping < 0)
            {
                // NOTE: We have different behaviour for "LineItemFreeShipping" and "OrderShippingAjustment" actions:
                //   1. "LineItemFreeShipping" action sets shipping to zero, but leave handling
                //   2. "OrderShippingAjustment" action decreases shipping and handling together
                // Example:
                //   Order with 2 items; shipping per item=$2; handling per order=$1
                //   - LineItemFreeShipping: shipping before discount=$5, shipping after discount=$1 
                //   - OrderShippingAjustment(%50): shipping before discount=$5, shipping after discount=$2.5

                totalHandling += totalShipping;
                totalShipping = 0;

                if (totalHandling < 0)
                {
                    totalHandling = 0;
                }
            }

            order.TotalShippingAfterDiscounts = totalShipping + totalHandling;

            var orderItems = order.Items;
            var itemsCount = orderItems.Count;

            // Clear shipping portion
            for (var i = 0; i < itemsCount; i++)
            {
                orderItems[i].ShippingPortion = 0;
            }

            var currentStoreSettings = _app.CurrentStore.Settings;
            var handlingNonShipping = currentStoreSettings.HandlingNonShipping;

            // Add handling portions
            var itemsToDistributeHandling = handlingNonShipping
                ? orderItems
                : orderItems.Where(
                    y =>
                        y.ShippingStatus != OrderShippingStatus.NonShipping &&
                        (y.ShippingCharge == ShippingChargeType.ChargeHandling ||
                         y.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling)).ToList();

            DistributeShippingPortions(itemsToDistributeHandling, totalHandling);

            var itemsToDistributeShipping = handlingNonShipping
                ? orderItems
                : orderItems.Where(
                    y =>
                        y.ShippingStatus != OrderShippingStatus.NonShipping &&
                        (y.ShippingCharge == ShippingChargeType.ChargeShipping ||
                         y.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling)).ToList();

            // Add shipping portions
            var shippingMethodId = order.ShippingMethodId;
            itemsToDistributeShipping =
                itemsToDistributeShipping.Where(i => !i.MarkedForFreeShipping(shippingMethodId)).ToList();

            DistributeShippingPortions(itemsToDistributeShipping, totalShipping);
        }

        private void DistributeShippingPortions(List<LineItem> items, decimal totalShipping)
        {
            var itemsCount = items.Count;
            if (itemsCount == 0) return;

            var totalValueOfItems = items.Sum(i => i.LineTotal);
            decimal totalApplied = 0;

            for (var i = 0; i < itemsCount; i++)
            {
                var item = items[i];

                if (i == itemsCount - 1)
                {
                    // last item
                    item.ShippingPortion += totalShipping - totalApplied;
                }
                else
                {
                    if (totalValueOfItems == 0) continue;

                    var part = Money.RoundCurrency(totalShipping * item.LineTotal / totalValueOfItems);

                    item.ShippingPortion += part;
                    totalApplied += part;
                }
            }
        }

        private void CalculateTaxes(Order order)
        {
            order.ClearTaxes();

            var taxProviderId = _app.CurrentStore.Settings.TaxProviderEnabled;

            if (!string.IsNullOrEmpty(taxProviderId))
            {
                var provider = TaxProviders.CurrentTaxProvider(_app.CurrentStore);

                provider.GetTaxes(order, _app.CurrentRequestContext);
            }
            else
            {
                TaxOrder(order);
            }
        }

        private void TaxOrder(Order order)
        {
            TaxItems(order.ItemsAsITaxable(), order.BillingAddress, order.ShippingAddress, order.TotalOrderDiscounts, order.UserID);

            var isTaxRateSame = true;
            decimal taxRate = -1;
            decimal itemsTax = 0;
            decimal shippingTax = 0;
            decimal totalShippingAfterDiscounts = 0;

            var orderItems = order.Items;
            var itemsCount = orderItems.Count;

            for (var i = 0; i < itemsCount; i++)
            {
                var li = orderItems[i];
                itemsTax += li.TaxPortion;
                shippingTax += li.ShippingTaxPortion;
                totalShippingAfterDiscounts += li.ShippingPortion;

                if (isTaxRateSame)
                {
                    var liShippingTaxRate = li.ShippingTaxRate;
                    if (liShippingTaxRate != taxRate && taxRate != -1)
                    {
                        isTaxRateSame = false;
                    }
                    taxRate = liShippingTaxRate;
                }
            }

            order.ItemsTax = itemsTax;
            order.ShippingTax = shippingTax;
            order.TotalShippingAfterDiscounts = totalShippingAfterDiscounts;

            if (totalShippingAfterDiscounts != 0)
            {
                var applyVATRules = _app.CurrentStore.Settings.ApplyVATRules;

                if (isTaxRateSame)
                {
                    order.ShippingTaxRate = taxRate;
                }
                else
                {
                    if (applyVATRules)
                    {
                        order.ShippingTaxRate = 0;

                        var taxAmount = shippingTax;
                        var remainAmount = totalShippingAfterDiscounts - taxAmount;

                        if (taxAmount != 0 && remainAmount != 0)
                        {
                            order.ShippingTaxRate = taxAmount / remainAmount;
                        }
                    }
                    else
                    {
                        order.ShippingTaxRate = shippingTax / totalShippingAfterDiscounts;
                    }
                }
            }
            else
            {
                order.ShippingTaxRate = 0;
            }

            order.TotalTax = itemsTax + shippingTax;
        }

        private void TaxItems(List<ITaxable> items, IAddress billingAddress, IAddress shippingAddress, decimal totalOrderDiscounts, string userId)
        {
            var applyVATRules = _app.CurrentStore.Settings.ApplyVATRules;
            var storeId = _app.CurrentStore.Id;
            var orderServices = _app.OrderServices;
            var taxSchedules = orderServices.TaxSchedules;
            var taxes = orderServices.Taxes;
            var membershipServices = _app.MembershipServices;

            decimal discount = 0;
            decimal qty = 0;

            if (totalOrderDiscounts != 0)
            {
                var itemsCount = items.Count;
                for (var i = 0; i < itemsCount; i++)
                {
                    var item = items[i];
                    if (item.IsTaxExempt == false && item.TaxSchedule != -1)
                    {
                        if (taxSchedules.FindForThisStore(item.TaxSchedule) != null)
                        {
                            qty += item.Quantity;
                        }
                    }
                }

                if (qty != 0)
                {
                    discount = totalOrderDiscounts / qty;
                }
            }

            // Cache user lookup if needed
            CustomerAccount user = null;
            var userCached = false;

            var itemsCount2 = items.Count;
            for (var i = 0; i < itemsCount2; i++)
            {
                var item = items[i];

                if (item.IsTaxExempt) continue;

                ITaxSchedule schedule = taxSchedules.FindForThisStore(item.TaxSchedule);

                if (schedule == null) continue;

                //Get best match by address
                var taxationAddress = item.IsNonShipping ? billingAddress : shippingAddress;
                var tax = taxes.FindByAdress(storeId, schedule.TaxScheduleId(), taxationAddress);

                var defaultRate = schedule.TaxScheduleDefaultRate() / 100;
                var defaultShippingRate = schedule.TaxScheduleDefaultShippingRate() / 100;
                decimal rate = 0;
                decimal shippingRate = 0;

                if (applyVATRules)
                {
                    rate = defaultRate;
                    shippingRate = defaultShippingRate;
                }

                if (tax != null)
                {
                    // Cache user lookup
                    if (!userCached)
                    {
                        user = membershipServices.Customers.Find(userId);
                        userCached = true;
                    }

                    var taxExemptUser = user != null ? user.TaxExempt : false;

                    if (!taxExemptUser)
                    {
                        rate = tax.Rate / 100;

                        if (tax.ApplyToShipping) shippingRate = tax.ShippingRate / 100;
                    }
                }

                item.SetTaxRate(rate);
                item.SetShippingTaxRate(shippingRate);

                var lineItemTotalDiscount = item.Quantity * discount;

                if (tax != null)
                {
                    if (applyVATRules)
                    {
                        if (rate != defaultRate)
                        {
                            //Subtract included tax
                            var lineTotal = item.LineTotal;
                            var lineTotalVAT = Money.RoundCurrency(lineTotal - lineTotal / (1 + defaultRate));

                            item.LineTotal = lineTotal - lineTotalVAT;

                            //Add new tax value
                            item.TaxPortion = Money.RoundCurrency(item.LineTotal * rate);

                            item.LineTotal += item.TaxPortion;
                        }
                        else
                        {
                            var lineTotal = item.LineTotal;

                            item.TaxPortion = Money.RoundCurrency(lineTotal - lineTotal / (1 + rate));
                        }

                        if (shippingRate != defaultShippingRate)
                        {
                            //Subtract tax from shipping portion always since rates may differ
                            var shippingPortion = item.ShippingPortion;
                            var shippingPortionVAT =
                                Money.RoundCurrency(shippingPortion - shippingPortion / (1 + defaultShippingRate));

                            item.ShippingPortion = shippingPortion - shippingPortionVAT;

                            item.ShippingTaxPortion = Money.RoundCurrency(item.ShippingPortion * shippingRate);

                            item.ShippingPortion += item.ShippingTaxPortion;
                        }
                        else
                        {
                            var shippingPortion = item.ShippingPortion;

                            item.ShippingTaxPortion =
                                Money.RoundCurrency(shippingPortion - shippingPortion / (1 + shippingRate));
                        }
                    }
                    else
                    {
                        var lineTotalTax = Money.RoundCurrency((item.LineTotal + lineItemTotalDiscount) * rate);

                        item.TaxPortion = lineTotalTax;

                        var shippingPortionTax = Money.RoundCurrency(item.ShippingPortion * shippingRate);

                        item.ShippingTaxPortion = shippingPortionTax;
                    }
                }

                item.AdjustedPricePerItem = Money.RoundCurrency(item.LineTotal / item.Quantity);
            }
        }

        private void DistributeOrderDiscounts(Order order)
        {
            var orderItems = order.Items;
            var itemsCount = orderItems.Count;
            var totalOrderDiscounts = order.TotalOrderDiscounts;
            var totalValueOfItems = orderItems.Sum(i => i.LineTotal);
            decimal totalApplied = 0;

            for (var i = 0; i < itemsCount; i++)
            {
                var lineItem = orderItems[i];

                if (i == itemsCount - 1)
                {
                    // last item
                    lineItem.LineTotal += totalOrderDiscounts - totalApplied;
                }
                else
                {
                    if (totalValueOfItems != 0)
                    {
                        var part = Money.RoundCurrency(totalOrderDiscounts * lineItem.LineTotal / totalValueOfItems);

                        lineItem.LineTotal += part;

                        totalApplied += part;
                    }
                }

                lineItem.AdjustedPricePerItem = Money.RoundCurrency(lineItem.LineTotal / lineItem.Quantity);
            }
        }

        private void DistributeShipping(Order order)
        {
            var orderItems = order.Items;
            var itemsCount = orderItems.Count;

            for (var i = 0; i < itemsCount; i++)
            {
                var orderItem = orderItems[i];
                orderItem.LineTotal += orderItem.ShippingPortion;
            }
        }

        #endregion
    }
}