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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.Geography;

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
                var freeItemFlag =
                    order.CustomProperties.FirstOrDefault(s => s.DeveloperId == DEVELOPER_ID && s.Key == FREE_ITEMS_KEY);

                if (freeItemFlag != null)
                {
                    freeItemFlag.Value = string.Empty;
                }

                var flag =
                    order.CustomProperties.FirstOrDefault(
                        s => s.DeveloperId == DEVELOPER_ID && s.Key == FREE_PROMOTIONS_KEY);

                if (flag != null)
                {
                    flag.Value = string.Empty;
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
                    // In the future it may be a good idea to add an option
                    // allowing merchant to select if they would like to allow
                    // items not in the catalog to exist in carts or if we should
                    // just remove items from the cart with a warning here.
                    if (price == null) continue;

                    li.BasePricePerItem = price.BasePrice;

                    foreach (var discount in price.DiscountDetails)
                    {
                        li.DiscountDetails.Add(new DiscountDetail
                        {
                            Amount = discount.Amount*li.Quantity,
                            Description = discount.Description,
                            DiscountType = PromotionType.Sale
                        });
                    }
                }
            }
        }

        private void ApplyVolumeDiscounts(Order order)
        {
            // Count up how many of each item in order
            var quantityMap = new Dictionary<string, int>();

            foreach (var item in order.Items)
            {
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

            // Check for discounts on each item
            foreach (var productId in quantityMap.Keys)
            {
                var volumeDiscounts = _app.CatalogServices.VolumeDiscounts.FindByProductId(productId);

                if (volumeDiscounts.Count == 0) continue;

                // Locate the correct discount in the chart of discounts
                var quantity = quantityMap[productId];
                var volumeDiscountToApply = volumeDiscounts.LastOrDefault(vd => quantity >= vd.Qty);

                if (volumeDiscountToApply == null) continue;

                // Now we have to go through the entire order and discount all items
                // Traversal through all items is requred because few line items of same product may be present in the cart
                foreach (var item in order.Items)
                {
                    if (item.ProductId == productId)
                    {
                        var p = _app.CatalogServices.Products.FindWithCache(item.ProductId);

                        if (p != null)
                        {
                            var adjustedPricePerItem = item.BasePricePerItem + item.TotalDiscounts()/item.Quantity;
                            var alreadyDiscounted = p.SitePrice > adjustedPricePerItem;

                            var volumeDiscountGlobalText = GlobalLocalization.GetString(VOLUME_DISCOUNT_LOCALIZATION_KEY);

                            if (string.IsNullOrEmpty(volumeDiscountGlobalText))
                            {
                                volumeDiscountGlobalText = VOLUME_DISCOUNT_GLOBAL_TEXT;
                            }

                            if (!alreadyDiscounted || !item.DiscountDetails.Any())
                            {
                                // item isn't discounted yet so apply the exact price the merchant set
                                var toDiscount = -1*(adjustedPricePerItem - volumeDiscountToApply.Amount);

                                toDiscount = toDiscount*item.Quantity;

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
                                var originalPriceChange = p.SitePrice - volumeDiscountToApply.Amount;
                                var percentChange = originalPriceChange/p.SitePrice;
                                var newDiscount = -1*percentChange*adjustedPricePerItem;

                                newDiscount = newDiscount*item.Quantity;

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
        }

        private void CalculateItemsPrices(Order order)
        {
            foreach (var li in order.Items)
            {
                li.LineTotal = li.LineTotalWithoutDiscounts + li.TotalDiscounts();

                if (li.LineTotal < 0) li.LineTotal = 0;

                li.AdjustedPricePerItem = Money.RoundCurrency(li.LineTotal/li.Quantity);
            }
        }

        private void ApplyOffers(Order order, PromotionType mode)
        {
            if (mode == PromotionType.OfferForShipping)
            {
                string[] skipOfferForShipping = {ShippingMethod.MethodUnknown, ShippingMethod.MethodToBeDetermined};

                if (skipOfferForShipping.Contains(order.ShippingMethodId)) return;
            }

            _app.MarketingServices.ApplyOffers(order, mode);
        }

        private void CalculateHandlingAmount(Order order)
        {
            decimal totalHandling = 0;
            var store = _app.CurrentStore;

            if (store.Settings.HandlingType == (int) HandlingMode.PerItem)
            {
                decimal amount = 0;

                foreach (var item in order.Items)
                {
                    if (item.IsNonShipping)
                    {
                        if (store.Settings.HandlingNonShipping)
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

                totalHandling = store.Settings.HandlingAmount*amount;
            }
            else if (store.Settings.HandlingType == (int) HandlingMode.PerOrder)
            {
                // charge handling if there aren't non shipping items
                if (store.Settings.HandlingNonShipping)
                {
                    if (order.Items.Count > 0)
                    {
                        totalHandling = RecalculateHandlingPerLineItemSettings(store.Settings.HandlingAmount,
                            order.Items);
                    }
                }
                else
                {
                    if (
                        order.Items.Any(
                            i =>
                                !i.IsNonShipping && i.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling ||
                                i.ShippingCharge == ShippingChargeType.ChargeHandling))
                    {
                        totalHandling = RecalculateHandlingPerLineItemSettings(store.Settings.HandlingAmount,
                            order.Items);
                    }
                }
            }

            order.TotalHandling = totalHandling;
        }

        private decimal RecalculateHandlingPerLineItemSettings(decimal perOrderHandlingAmount, List<LineItem> lineItems)
        {
            // determine how many line items allow handling to be charged
            var itemsToChargeFor =
                lineItems.Count(
                    i =>
                        i.ShippingCharge == ShippingChargeType.ChargeHandling ||
                        i.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling);

            if (itemsToChargeFor < lineItems.Count)
            {
                // determine what the per-item handling fee is
                var handlingPerItem = perOrderHandlingAmount/lineItems.Count;

                // determine the pro-rated handling fee for items that allow handling charges
                return Money.RoundCurrency(handlingPerItem*itemsToChargeFor);
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

            // Clear shipping portion
            foreach (var item in order.Items)
            {
                item.ShippingPortion = 0;
            }

            // Add handling portions
            var itemsToDistributeHandling = _app.CurrentStore.Settings.HandlingNonShipping
                ? order.Items
                : order.Items.Where(
                    y =>
                        y.ShippingStatus != OrderShippingStatus.NonShipping &&
                        y.ShippingCharge == ShippingChargeType.ChargeHandling ||
                        y.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling).ToList();

            DistributeShippingPortions(itemsToDistributeHandling, totalHandling);

            var itemsToDistributeShipping = _app.CurrentStore.Settings.HandlingNonShipping
                ? order.Items
                : order.Items.Where(
                    y =>
                        y.ShippingStatus != OrderShippingStatus.NonShipping &&
                        y.ShippingCharge == ShippingChargeType.ChargeShipping ||
                        y.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling).ToList();

            // Add shipping portions
            itemsToDistributeShipping =
                itemsToDistributeShipping.Where(i => !i.MarkedForFreeShipping(order.ShippingMethodId)).ToList();

            DistributeShippingPortions(itemsToDistributeShipping, totalShipping);
        }

        private void DistributeShippingPortions(List<LineItem> items, decimal totalShipping)
        {
            var itemsCount = items.Count;
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

                    var part = Money.RoundCurrency(totalShipping*item.LineTotal/totalValueOfItems);

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
            TaxItems(order.ItemsAsITaxable(), order.BillingAddress, order.ShippingAddress, order.TotalOrderDiscounts);

            var isTaxRateSame = true;
            decimal taxRate = -1;
            order.TotalShippingAfterDiscounts = 0;

            foreach (var li in order.Items)
            {
                order.ItemsTax += li.TaxPortion;
                order.ShippingTax += li.ShippingTaxPortion;
                order.TotalShippingAfterDiscounts += li.ShippingPortion;

                if (!isTaxRateSame) continue;

                if (li.ShippingTaxRate != taxRate && taxRate != -1)
                {
                    isTaxRateSame = false;
                }

                taxRate = li.ShippingTaxRate;
            }

            if (order.TotalShippingAfterDiscounts != 0)
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

                        var taxAmount = order.ShippingTax;
                        var remainAmount = order.TotalShippingAfterDiscounts - taxAmount;

                        if (taxAmount != 0 && remainAmount != 0)
                        {
                            order.ShippingTaxRate = taxAmount/remainAmount;
                        }
                    }
                    else
                    {
                        order.ShippingTaxRate = order.ShippingTax/order.TotalShippingAfterDiscounts;
                    }
                }
            }
            else
            {
                order.ShippingTaxRate = 0;
            }

            order.TotalTax = order.ItemsTax + order.ShippingTax;
        }

        private void TaxItems(List<ITaxable> items, IAddress billingAddress, IAddress shippingAddress,decimal totalOrderDiscounts)
        {
            var applyVATRules = _app.CurrentStore.Settings.ApplyVATRules;
            decimal discount = 0;
            if (totalOrderDiscounts != 0)
            {
                var qty = items.Where(i => i.IsTaxExempt == false && i.TaxSchedule != -1 && _app.OrderServices.TaxSchedules.FindForThisStore(i.TaxSchedule) != null).Sum(i => i.Quantity);
                discount = totalOrderDiscounts / qty;
            }

            foreach (var item in items)
            {
                if (item.IsTaxExempt) continue;

                ITaxSchedule schedule = _app.OrderServices.TaxSchedules.FindForThisStore(item.TaxSchedule);

                if (schedule == null) continue;

                //Get best match by address
                var taxationAddress = item.IsNonShipping ? billingAddress : shippingAddress;
                var tax = _app.OrderServices.Taxes.FindByAdress(_app.CurrentStore.Id, schedule.TaxScheduleId(),
                    taxationAddress);

                var defaultRate = schedule.TaxScheduleDefaultRate()/100;
                var defaultShippingRate = schedule.TaxScheduleDefaultShippingRate()/100;
                decimal rate = 0;
                decimal shippingRate = 0;

                if (applyVATRules)
                {
                    rate = defaultRate;
                    shippingRate = defaultShippingRate;
                }

                if (tax != null)
                {
                    var user = _app.CurrentCustomer;
                    var taxExemptUser = user != null ? user.TaxExempt : false;

                    if (!taxExemptUser)
                    {
                        rate = tax.Rate/100;

                        if (tax.ApplyToShipping) shippingRate = tax.ShippingRate/100;
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
                            var lineTotalVAT = Money.RoundCurrency(lineTotal - lineTotal/(1 + defaultRate));

                            item.LineTotal = lineTotal - lineTotalVAT;

                            //Add new tax value
                            item.TaxPortion = Money.RoundCurrency(item.LineTotal*rate);

                            item.LineTotal += item.TaxPortion;
                        }
                        else
                        {
                            var lineTotal = item.LineTotal;

                            item.TaxPortion = Money.RoundCurrency(lineTotal - lineTotal/(1 + rate));
                        }

                        if (shippingRate != defaultShippingRate)
                        {
                            //Subtract tax from shipping portion always since rates may differ
                            var shippingPortion = item.ShippingPortion;
                            var shippingPortionVAT =
                                Money.RoundCurrency(shippingPortion - shippingPortion/(1 + defaultShippingRate));

                            item.ShippingPortion = shippingPortion - shippingPortionVAT;

                            item.ShippingTaxPortion = Money.RoundCurrency(item.ShippingPortion*shippingRate);

                            item.ShippingPortion += item.ShippingTaxPortion;
                        }
                        else
                        {
                            var shippingPortion = item.ShippingPortion;

                            item.ShippingTaxPortion =
                                Money.RoundCurrency(shippingPortion - shippingPortion/(1 + shippingRate));
                        }
                    }
                    else
                    {
                        var lineTotalTax = Money.RoundCurrency((item.LineTotal + lineItemTotalDiscount) * rate);

                        item.TaxPortion = lineTotalTax;

                        var shippingPortionTax = Money.RoundCurrency(item.ShippingPortion*shippingRate);

                        item.ShippingTaxPortion = shippingPortionTax;
                    }
                }

                item.AdjustedPricePerItem = Money.RoundCurrency(item.LineTotal/item.Quantity);
            }
        }

        private void DistributeOrderDiscounts(Order order)
        {
            var itemsCount = order.Items.Count;
            var totalOrderDiscounts = order.TotalOrderDiscounts;
            var totalValueOfItems = order.Items.Sum(i => i.LineTotal);
            decimal totalApplied = 0;

            for (var i = 0; i < itemsCount; i++)
            {
                var lineItem = order.Items[i];

                if (i == itemsCount - 1)
                {
                    // last item
                    lineItem.LineTotal += totalOrderDiscounts - totalApplied;
                }
                else
                {
                    if (totalValueOfItems != 0)
                    {
                        var part = Money.RoundCurrency(totalOrderDiscounts*lineItem.LineTotal/totalValueOfItems);

                        lineItem.LineTotal += part;

                        totalApplied += part;
                    }
                }

                lineItem.AdjustedPricePerItem = Money.RoundCurrency(lineItem.LineTotal/lineItem.Quantity);
            }
        }

        private void DistributeShipping(Order order)
        {
            foreach (var orderItem in order.Items)
            {
                orderItem.LineTotal += orderItem.ShippingPortion;
            }
        }

        #endregion
    }
}