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

using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Task = System.Threading.Tasks.Task;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     Provides order, cart, payment, shipping, tax, and order-related workflow operations for the current commerce context.
    /// </summary>
    /// <remarks>
    ///     Operations use repositories created for the supplied <see cref="HccRequestContext"/>. Several methods intentionally
    ///     have side effects, including persistence, payment-status recalculation, workflow execution, inventory unreservation,
    ///     gift-card activation or deactivation, cart-session updates, and mutation of supplied orders and shipping rates.
    /// </remarks>
    public class OrderService : HccServiceBase
    {
        /// <summary>
        ///     Initializes order-related repositories from the supplied request context.
        /// </summary>
        /// <param name="context">The supplied context value.</param>
        public OrderService(HccRequestContext context)
            : base(context)
        {
            Orders = Factory.CreateRepo<OrderRepository>(Context);
            Taxes = Factory.CreateRepo<TaxRepository>(Context);
            TaxSchedules = Factory.CreateRepo<TaxScheduleRepository>(Context);
            ShippingZones = Factory.CreateRepo<ZoneRepository>(Context);
            ShippingMethods = Factory.CreateRepo<ShippingMethodRepository>(Context);
            Transactions = Factory.CreateRepo<OrderTransactionRepository>(Context);
            Returns = Factory.CreateRepo<RMARepository>(Context);
        }

        /// <summary>
        ///     Gets or sets the orders value used by this API.
        /// </summary>
        public OrderRepository Orders { get; protected set; }
        /// <summary>
        ///     Gets or sets the taxes value used by this API.
        /// </summary>
        public TaxRepository Taxes { get; protected set; }
        /// <summary>
        ///     Gets or sets the tax schedules value used by this API.
        /// </summary>
        public TaxScheduleRepository TaxSchedules { get; protected set; }
        /// <summary>
        ///     Gets or sets the shipping zones value used by this API.
        /// </summary>
        public ZoneRepository ShippingZones { get; protected set; }
        /// <summary>
        ///     Gets or sets the shipping methods value used by this API.
        /// </summary>
        public ShippingMethodRepository ShippingMethods { get; protected set; }
        /// <summary>
        ///     Gets or sets the transactions value used by this API.
        /// </summary>
        public OrderTransactionRepository Transactions { get; protected set; }
        /// <summary>
        ///     Gets or sets the returns value used by this API.
        /// </summary>
        public RMARepository Returns { get; protected set; }

        private Order CachedShoppingCart
        {
            get
            {
                //we must check the HttpContext, otherwise this will fail during unit tests
                var current = HttpContext.Current;
                if (current != null)
                {
                    return current.Items["CurrentShoppingCart"] as Order;
                }
                return null;
            }
            set
            {
                var current = HttpContext.Current;
                if (current != null)
                {
                    current.Items["CurrentShoppingCart"] = value;
                }
            }
        }

        // Taxes

        /// <summary>
        ///     Performs the tax schedules destroy operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="scheduleId">The supplied schedule id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool TaxSchedulesDestroy(long scheduleId)
        {
            try
            {
                var taxes = Taxes.FindByTaxSchedule(Context.CurrentStore.Id, scheduleId);
                foreach (var t in taxes)
                {
                    Taxes.Delete(t.Id);
                }
                TaxSchedules.Delete(scheduleId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Shipping
        /// <summary>
        ///     Performs the shipping zone find in list operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="zones">The supplied zones value.</param>
        /// <param name="id">The supplied id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public Zone ShippingZoneFindInList(List<Zone> zones, long id)
        {
            return zones.FirstOrDefault(z => z.Id == id);
        }

        /// <summary>
        ///     Performs the shipping zone add area operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="zoneId">The supplied zone id value.</param>
        /// <param name="countryIso3">The supplied country iso3 value.</param>
        /// <param name="regionAbbreviation">The supplied region abbreviation value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool ShippingZoneAddArea(long zoneId, string countryIso3, string regionAbbreviation)
        {
            var z = ShippingZones.Find(zoneId);
            if (z != null)
            {
                if (z.Id == zoneId)
                {
                    var exists = z.Areas.Any(a => a.CountryIsoAlpha3 == countryIso3 && a.RegionAbbreviation == regionAbbreviation);
                    if (!exists)
                    {
                        z.Areas.Add(new ZoneArea
                        {
                            CountryIsoAlpha3 = countryIso3,
                            RegionAbbreviation = regionAbbreviation
                        });
                        return ShippingZones.Update(z);
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///     Performs the shipping zone remove area operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="zoneId">The supplied zone id value.</param>
        /// <param name="countryIso3">The supplied country iso3 value.</param>
        /// <param name="regionAbbreviation">The supplied region abbreviation value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool ShippingZoneRemoveArea(long zoneId, string countryIso3, string regionAbbreviation)
        {
            var z = ShippingZones.Find(zoneId);
            if (z != null)
            {
                if (z.Id == zoneId)
                {
                    var located = z.Areas.FirstOrDefault(a => a.CountryIsoAlpha3 == countryIso3 && a.RegionAbbreviation == regionAbbreviation);
                    if (located != null)
                    {
                        if (z.Areas.Remove(located))
                        {
                            return ShippingZones.Update(z);
                        }
                    }
                }
            }
            return false;
        }

        //Orders and Items
        /// <summary>
        ///     Performs the orders update item quantity operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="itemId">The supplied item id value.</param>
        /// <param name="quantity">The supplied quantity value.</param>
        /// <param name="o">The supplied o value.</param>
        /// <returns>The value produced by the operation.</returns>
        public SystemOperationResult OrdersUpdateItemQuantity(long itemId, int quantity, Order o)
        {
            var result = new SystemOperationResult
            {
                Success = true
            };

            var item = o.Items.SingleOrDefault(y => y.Id == itemId);
            if (item == null)
            {
                result.Success = false;
                result.Message = "Could not locate that item";
                return result;
            }

            if (item.Quantity == quantity)
            {
                result.Success = true;
                return result;
            }

            if (quantity == 0)
            {
                // remove and return success/failure
                result.Success = o.Items.Remove(item);
                return result;
            }

            item.Quantity = quantity;
            result.Success = true;
            return result;
        }

        private void FilterRatesByShippingMethods(SortableCollection<ShippingRateDisplay> ratesSort, Order order)
        {
            var shippingProviders = ShippingMethods.FindAll(order.StoreId);
            shippingProviders = shippingProviders.OrderBy(s => s.SortOrder).ToList();

            // Build lookup of rates by ShippingMethodId to avoid repeated enumeration and allocations
            var ratesByMethod = new Dictionary<string, List<ShippingRateDisplay>>(StringComparer.OrdinalIgnoreCase);
            foreach (ShippingRateDisplay r in ratesSort)
            {
                var key = r?.ShippingMethodId ?? string.Empty;
                if (!ratesByMethod.TryGetValue(key, out var list))
                {
                    list = new List<ShippingRateDisplay>();
                    ratesByMethod[key] = list;
                }
                list.Add(r);
            }

            var toRemove = new List<ShippingRateDisplay>();
            var subtotalFlagByMethod = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            foreach (var method in shippingProviders)
            {
                var methodKey = method.Bvin ?? string.Empty;
                var filteredRates = ratesByMethod.TryGetValue(methodKey, out var list) ? list : null;
                if (filteredRates == null || filteredRates.Count == 0) continue;

                // preserve a subtotal flag per method (original logic used a single flag that carried across methods)
                var subtotal = subtotalFlagByMethod.ContainsKey(methodKey) && subtotalFlagByMethod[methodKey];

                foreach (var rate in filteredRates)
                {
                    if (rate == null) continue;

                    switch (method.VisibilityMode)
                    {
                        case ShippingVisibilityMode.Always:
                            subtotal = true;
                            break;

                        case ShippingVisibilityMode.Never:
                            toRemove.Add(rate);
                            break;

                        case ShippingVisibilityMode.NoRates:
                            if (subtotal)
                            {
                                toRemove.Add(rate);
                            }
                            else
                            {
                                subtotal = true;
                            }
                            break;

                        case ShippingVisibilityMode.SubtotalAmount:
                         	if (method.VisibilityAmount.HasValue && order.TotalOrderAfterDiscounts > method.VisibilityAmount.Value)
                            {
                                subtotal = true;
                            }
                            else
                            {
                                toRemove.Add(rate);
                            }
                            break;

                        case ShippingVisibilityMode.TotalWeight:
                            if (method.VisibilityAmount.HasValue && order.TotalWeight > method.VisibilityAmount.Value)
                            {
                                subtotal = true;
                            }
                            else
                            {
                                toRemove.Add(rate);
                            }
                            break;

                        default:
                            break;
                    }
                }

                subtotalFlagByMethod[methodKey] = subtotal;
            }

            // Remove collected rates
            foreach (var r in toRemove.Distinct())
            {
                ratesSort.Remove(r);
            }
        }

        /// <summary>
        ///     Performs the find available shipping rates operation using the supplied arguments and current commerce context.
        ///     Rates are adjusted for extra charges and offers, filtered by visibility, and sorted by rate.
        /// </summary>
        /// <param name="order">The supplied order value.</param>
        /// <returns>The value produced by the operation.</returns>
        public SortableCollection<ShippingRateDisplay> FindAvailableShippingRates(Order order)
        {
            return FindAvailableShippingRatesInternal(order);
        }

        /// <summary>
        ///     Performs the find shipping rate by unique key operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="order">The supplied order value.</param>
        /// <returns>The value produced by the operation.</returns>
        public ShippingRateDisplay FindShippingRateByUniqueKey(Order order)
        {
            return FindShippingRateByUniqueKey(order.ShippingMethodUniqueKey, order);
        }

        /// <summary>
        ///     Performs the find shipping rate by unique key operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="key">The supplied key value.</param>
        /// <param name="order">The supplied order value.</param>
        /// <returns>The value produced by the operation.</returns>
        public ShippingRateDisplay FindShippingRateByUniqueKey(string key, Order order)
        {
            var rates = FindAvailableShippingRatesInternal(order, order.ShippingMethodId);

            if (rates == null) return null;
            if (rates.Count < 1) return null;

            return rates
                .OfType<ShippingRateDisplay>()
                .FirstOrDefault(r => r.UniqueKey == key);
        }

        // Orders and Payments
        /// <summary>
        ///     Performs the orders list payment methods operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="order">The supplied order value.</param>
        /// <returns>The value produced by the operation.</returns>
        public string OrdersListPaymentMethods(Order order)
        {
            var allTransactions = Transactions.FindForOrder(order.bvin);
            return OrdersListPaymentMethods(allTransactions);
        }

        /// <summary>
        ///     Performs the orders list payment methods operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="transactions">The supplied transactions value.</param>
        /// <returns>The value produced by the operation.</returns>
        public string OrdersListPaymentMethods(List<OrderTransaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                return "No Payment Methods Selected";
            }

            var found = false;
            var sb = new StringBuilder();

            foreach (var t in transactions)
            {
                var statusClass = (t.Success) ? " transaction-success" : " transaction-failure";
                switch (t.Action)
                {
                    case ActionType.OfflinePaymentRequest:
                        found = true;
                        sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label offline-payment{3}\">{0:C} | {1}</span>", t.Amount, t.RefNum1, t.IdAsString, statusClass);
                        break;
                    //case ActionType.CreditCardInfo:
                    //    found = true;
                    //    sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label credit-card-info{3}\">{0:C} | Credit Card {1}</span>", t.Amount, t.CreditCard.CardNumberLast4Digits, t.IdAsString, statusClass);
                    //    break;
                    case ActionType.CreditCardCapture:
                        found = true;
                        sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label credit-card-capture{3}\">{0:C} | Credit Card {1}</span>", t.Amount, t.CreditCard.CardNumberLast4Digits, t.IdAsString, statusClass);
                        break;
                    case ActionType.CreditCardCharge:
                        found = true;
                        sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label credit-card-charge{3}\">{0:C} | Credit Card {1}</span>", t.Amount, t.CreditCard.CardNumberLast4Digits, t.IdAsString, statusClass);
                        break;
                    case ActionType.CreditCardRefund:
                        found = true;
                        sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label credit-card-refund{3}\">{0:C} | Credit Card {1}</span>", t.Amount, t.CreditCard.CardNumberLast4Digits, t.IdAsString, statusClass);
                        break;
                    case ActionType.CreditCardVoid:
                        found = true;
                        sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label credit-card-void{3}\">{0:C} | Credit Card {1}</span>", t.Amount, t.CreditCard.CardNumberLast4Digits, t.IdAsString, statusClass);
                        break;
                    case ActionType.GiftCardInfo:
                        found = true;
                        sb.AppendFormat("<span id=\"{1}\" class=\"hc-transaction-label gift-card-info{2}\">{0:C} | Gift Card</span>", t.Amount, t.IdAsString, statusClass);
                        break;
                    case ActionType.GiftCardCapture:
                        found = true;
                        sb.AppendFormat("<span id=\"{1}\" class=\"hc-transaction-label gift-card-capture{2}\">{0:C} | Gift Card</span>", t.Amount, t.IdAsString, statusClass);
                        break;
                    case ActionType.GiftCardHold:
                        found = true;
                        sb.AppendFormat("<span id=\"{1}\" class=\"hc-transaction-label gift-card-hold{2}\">{0:C} | Gift Card</span>", t.Amount, t.IdAsString, statusClass);
                        break;
                    case ActionType.PurchaseOrderInfo:
                        found = true;
                        sb.AppendFormat("<span id=\"{2}\" class=\"hc-transaction-label purchase-order{3}\">{0:C} | PO #{1}</span>", t.Amount, t.PurchaseOrderNumber, t.IdAsString, statusClass);
                        break;
                    case ActionType.RecurringSubscriptionInfo:
                        found = true;
                        sb.AppendFormat("<span id=\"{1}\" class=\"hc-transaction-label credit-card-recurring{2}\">Credit Card {0}</span>", t.CreditCard.CardNumberLast4Digits, t.IdAsString, statusClass);
                        break;
                    default:
                        var paymentMethod = PaymentMethods.Find(t.MethodId);

                        if (paymentMethod != null)
                        {
                            found = true;
                            sb.AppendFormat("<span id=\"{1}\" class=\"hc-transaction-label default{2}\">{0}</span>", paymentMethod.MethodName, t.IdAsString, statusClass);
                        }
                        break;
                }
            }

            if (found)
            {
                return sb.ToString();
            }
            return "No Payment Methods Selected";
        }

        /// <summary>
        ///     Performs the add payment transaction to order operation using the supplied arguments and current commerce context.
        ///     The operation persists the transaction, recalculates payment status, runs the payment-changed workflow, and updates the order.
        /// </summary>
        /// <param name="o">The supplied o value.</param>
        /// <param name="t">The supplied t value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool AddPaymentTransactionToOrder(Order o, Transaction t)
        {
            var ot = new OrderTransaction(t);
            return AddPaymentTransactionToOrder(o, ot);
        }

        /// <summary>
        ///     Performs the add payment transaction to order operation using the supplied arguments and current commerce context.
        ///     The operation persists the transaction, recalculates payment status, runs the payment-changed workflow, and updates the order.
        /// </summary>
        /// <param name="o">The supplied o value.</param>
        /// <param name="t">The supplied t value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool AddPaymentTransactionToOrder(Order o, OrderTransaction t)
        {
            // Save Order First if no bvin
            Orders.Upsert(o);

            t.OrderId = o.bvin;
            t.OrderNumber = o.OrderNumber;
            t.StoreId = o.StoreId;

            if (Transactions.Create(t))
            {
                var previous = o.PaymentStatus;
                EvaluatePaymentStatus(o);
                OnPaymentChanged(previous, o);
                return Orders.Update(o);
            }

            return false;
        }

        /// <summary>
        ///     Performs the payment summary operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="o">The supplied o value.</param>
        /// <returns>The value produced by the operation.</returns>
        public OrderPaymentSummary PaymentSummary(Order o)
        {
            var result = new OrderPaymentSummary();
            result.Populate(o, this);
            o.PaymentStatus = EvaluatePaymentStatus(o, result);
            return result;
        }

        /// <summary>
        ///     Performs the evaluate payment status operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="o">The supplied o value.</param>
        /// <returns>The value produced by the operation.</returns>
        public OrderPaymentStatus EvaluatePaymentStatus(Order o)
        {
            var s = new OrderPaymentSummary();
            s.Populate(o, this);
            return EvaluatePaymentStatus(o, s);
        }

        private OrderPaymentStatus EvaluatePaymentStatus(Order o, OrderPaymentSummary s)
        {
            var result = OrderPaymentStatus.Unknown;

            if (s.AmountDue < 0 || s.TotalCredit > o.TotalGrand)
            {
                // Refund Due = Overpaid
                result = OrderPaymentStatus.Overpaid;
            }
            else
            {
                if (s.AmountDue == 0 && s.TotalCredit == o.TotalGrand)
                {
                    result = OrderPaymentStatus.Paid;
                }
                else
                {
                    // Amount Due = positive at this point
                    if (s.TotalCredit > 0)
                    {
                        result = OrderPaymentStatus.PartiallyPaid;
                    }
                    else
                    {
                        result = OrderPaymentStatus.Unpaid;
                    }
                }
            }

            o.PaymentStatus = result;
            return result;
        }

        private void OnPaymentChanged(OrderPaymentStatus previousPaymentStatus, Order o)
        {
            var context = new OrderTaskContext(Context)
            {
                Order = o,
                UserId = o.UserID,
                PreviousPaymentStatus = previousPaymentStatus
            };

            Workflow.RunByName(context, WorkflowNames.PaymentChanged);
        }

        /// <summary>
        ///     Performs the find transactions by third party order id operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="thirdPartyId">The supplied third party id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderTransaction> FindTransactionsByThirdPartyOrderId(string thirdPartyId)
        {
            var result = new List<OrderTransaction>();

            var oid = string.Empty;
            var o = Orders.FindByThirdPartyOrderId(thirdPartyId);
            if (o != null)
            {
                result = Transactions.FindForOrder(o.bvin);
            }

            return result;
        }

        /// <summary>
        ///     Performs the orders request shipping method operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="r">The supplied r value.</param>
        /// <param name="o">The supplied o value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool OrdersRequestShippingMethod(ShippingRateDisplay r, Order o)
        {
            var result = false;
            if (r != null)
            {
                o.ClearShippingPricesAndMethod();
                o.ShippingMethodId = r.ShippingMethodId;
                o.ShippingProviderId = r.ProviderId;
                o.ShippingProviderServiceCode = r.ProviderServiceCode;
                result = true;
            }
            return result;
        }

        /// <summary>
        ///     Performs the orders request shipping method by unique key operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="rateUniqueKey">The supplied rate unique key value.</param>
        /// <param name="o">The supplied o value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool OrdersRequestShippingMethodByUniqueKey(string rateUniqueKey, Order o)
        {
            var rates = FindAvailableShippingRates(o);
            var matchingRate = rates.OfType<ShippingRateDisplay>().FirstOrDefault(r => r.UniqueKey == rateUniqueKey);

            if (matchingRate != null)
            {
                return OrdersRequestShippingMethod(matchingRate, o);
            }

            return false;
        }

        /// <summary>
        ///     Performs the orders delete operation using the supplied arguments and current commerce context.
        ///     Configured tax-document and gift-card cleanup is performed before deletion.
        /// </summary>
        /// <param name="orderBvin">The supplied order bvin value.</param>
        /// <param name="app">The supplied app value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool OrdersDelete(string orderBvin, HotcakesApplication app)
        {
            var o = Orders.FindForCurrentStore(orderBvin);
            if (o == null) return true;

            var currentProvider = TaxProviders.CurrentTaxProvider(app.CurrentStore);
            if (currentProvider != null)
            {
                currentProvider.CancelTaxDocument(o, app.CurrentRequestContext);
            }

            // Disables all giftcards associated with order 
            UpdateGiftCardsStatus(o, false, app);

            return Orders.Delete(o.bvin);
        }

        /// <summary>
        ///     Performs the orders delete with inventory return operation using the supplied arguments and current commerce context.
        ///     Line-item inventory reservations are removed before deletion.
        /// </summary>
        /// <param name="orderBvin">The supplied order bvin value.</param>
        /// <param name="app">The supplied app value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool OrdersDeleteWithInventoryReturn(string orderBvin, HotcakesApplication app)
        {
            var o = Orders.FindForCurrentStore(orderBvin);
            if (o == null) return true;

            // return items to inventory
            foreach (var li in o.Items)
            {
                // We need to unreserve items instead increase inventory 
                app.CatalogServices.InventoryLineItemUnreserveInventory(li);
            }

            return OrdersDelete(o.bvin, app);
        }

        /// <summary>
        ///     Performs the generate new order number operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="storeId">The supplied store id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public int GenerateNewOrderNumber(long storeId)
        {
            using (HccDbContext context = Factory.CreateHccDbContext())
            {
                var results = context.GenerateNewOrderNumber(storeId);
                if (results != null)
                {
                    var number = results.FirstOrDefault();
                    if (number != null)
                    {
                        if (number.HasValue)
                        {
                            return number.Value;
                        }
                    }
                }
                return 0;
            }
        }

        /// <summary>
        ///     Performs the find highlight for order operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="order">The supplied order value.</param>
        /// <returns>The value produced by the operation.</returns>
        public string FindHighlightForOrder(OrderSnapshot order)
        {
            if (order == null || string.IsNullOrWhiteSpace(order.ShippingMethodId))
                return null;
            var method = ShippingMethods.Find(order.ShippingMethodId.Trim());
            if (method == null)
                return null;

            var highlight = method.Settings.GetSettingOrEmpty("highlight");
            return highlight;
        }

        /// <summary>
        ///     Performs the order status changed operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="o">The supplied o value.</param>
        /// <param name="prevStatusCode">The supplied prev status code value.</param>
        public void OrderStatusChanged(Order o, string prevStatusCode)
        {
            var context = new OrderTaskContext(Context)
            {
                Order = o,
                UserId = o.UserID,
                PreviousOrderStatusCode = prevStatusCode
            };

            Workflow.RunByName(context, WorkflowNames.OrderStatusChanged);
        }

        /// <summary>
        ///     Performs the update gift cards status operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="o">The supplied o value.</param>
        /// <param name="enabled">The supplied enabled value.</param>
        /// <param name="app">The supplied app value.</param>
        public void UpdateGiftCardsStatus(Order o, bool enabled, HotcakesApplication app)
        {
            var payManager = new OrderPaymentManager(o, app);

            foreach (var item in o.Items.Where(i => i.IsGiftCard))
            {
                var cards = app.CatalogServices.GiftCards.FindByLineItem(item.Id);

                foreach (var card in cards)
                {
                    if (enabled)
                    {
                        payManager.GiftCardActivate(card.CardNumber);
                    }
                    else
                    {
                        payManager.GiftCardDeactivate(card.CardNumber);
                    }
                }
            }
        }

        /// <summary>
        ///     The current shopping cart.
        /// </summary>
        /// <returns></returns>
        public Order CurrentShoppingCart()
        {
            if (SessionManager.CurrentUserHasCart(Context.CurrentStore))
            {
                var cachedCart = CachedShoppingCart;
                if (cachedCart != null)
                    return cachedCart;

                var cart = Orders.FindForCurrentStore(SessionManager.GetCurrentCartID(Context.CurrentStore));
                if (cart != null && !string.IsNullOrEmpty(cart.bvin) && !cart.IsPlaced)
                {
                    CachedShoppingCart = cart;
                    return cart;
                }
            }
            return null;
        }

        /// <summary>
        ///     Ensures the shopping cart.
        /// </summary>
        /// <returns></returns>
        public Order EnsureShoppingCart()
        {
            var cart = CurrentShoppingCart();
            if (cart == null)
                cart = new Order();

            var customer = Context.CurrentAccount;
            cart.UserEmail = customer != null ? customer.Email : string.Empty;
            cart.UserID = customer != null ? customer.Bvin : string.Empty;
            Orders.Upsert(cart);
            SessionManager.SetCurrentCartId(Context.CurrentStore, cart.bvin);

            return cart;
        }

        /// <summary>
        ///     Performs the invalidate cached cart operation using the supplied arguments and current commerce context.
        /// </summary>
        public void InvalidateCachedCart()
        {
            CachedShoppingCart = null;
        }

        /// <summary>
        ///     Adds to cart.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="listItem">The list item.</param>
        public void AddItemToOrder(Order order, LineItem listItem)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            if (listItem == null)
                throw new ArgumentNullException("listItem");

            if (!CheckItemExistInOrder(order, listItem))
            {
                order.Items.Add(listItem);
            }
        }

        /// <summary>
        ///     Check Item Exist In Cart
        /// </summary>
        /// <param name="order"></param>
        /// <param name="listItem"></param>
        /// <returns></returns>
        private bool CheckItemExistInOrder(Order order, LineItem listItem)
        {
            var productInCartList = order.Items.Where(i => i.ProductId == listItem.ProductId).ToList();
            if (!productInCartList.Any())
            {
                return false;
            }

            // Pre-compute comparison values to avoid repeated operations
            var itemSelectionData = listItem.SelectionData;
            var isGiftCard = listItem.IsGiftCard;
            var isUserSuppliedPrice = listItem.IsUserSuppliedPrice;

            string giftCardEmail = null;
            string giftCardName = null;
            string giftCardMessage = null;

            if (isGiftCard)
            {
                giftCardEmail = listItem.CustomPropGiftCardEmail?.Trim();
                giftCardName = listItem.CustomPropGiftCardName?.Trim();
                giftCardMessage = listItem.CustomPropGiftCardMessage?.Trim();
            }

            foreach (var productInCart in productInCartList)
            {
                if (!itemSelectionData.Equals(productInCart.SelectionData))
                {
                    continue;
                }

                if (isGiftCard)
                {
                    if (productInCart.BasePricePerItem == listItem.BasePricePerItem
                        && string.Equals(productInCart.CustomPropGiftCardEmail?.Trim(), giftCardEmail, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(productInCart.CustomPropGiftCardName?.Trim(), giftCardName, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(productInCart.CustomPropGiftCardMessage?.Trim(), giftCardMessage, StringComparison.OrdinalIgnoreCase))
                    {
                        productInCart.Quantity += listItem.Quantity;
                        return true;
                    }
                }
                else if (isUserSuppliedPrice)
                {
                    if (productInCart.BasePricePerItem == listItem.BasePricePerItem)
                    {
                        productInCart.Quantity += listItem.Quantity;
                        return true;
                    }
                }
                else
                {
                    productInCart.Quantity += listItem.Quantity;
                    productInCart.IsUpchargeAllowed = listItem.IsUpchargeAllowed;
                    return true;
                }
            }

            return false;
        }

        private SortableCollection<ShippingRateDisplay> FindAvailableShippingRatesInternal(Order order,
            string shippingMethodIdFilter = null)
        {
            if (order == null) return new SortableCollection<ShippingRateDisplay>();

            var result = new SortableCollection<ShippingRateDisplay>();

            // Get all the methods that apply to this shipping address and store
            var zones = ShippingZones.FindAllZonesForAddress(order.ShippingAddress, order.StoreId);
            var methods = ShippingMethods.FindForZones(zones);

            if (!string.IsNullOrEmpty(shippingMethodIdFilter))
                methods = methods.Where(m => m.Bvin == shippingMethodIdFilter).ToList();

            // Get Rates for each Method
            if (!order.IsOrderFreeShipping())
            {
                var tasks = new List<Task<Collection<ShippingRateDisplay>>>();
                foreach (var method in methods)
                {
                    var task = Task.Factory.StartNew(hccContext =>
                    {
                        HccRequestContext.Current = hccContext as HccRequestContext;
                        return method.GetRates(order, Context.CurrentStore);
                    },
                        HccRequestContext.Current);
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());

                foreach (var task in tasks)
                {
                    var tempRates = task.Result;
                    if (tempRates != null)
                    {
                        for (var i = 0; i <= tempRates.Count - 1; i++)
                        {
                            var fRate = tempRates[i].GetCopy();
                            result.Add(fRate);
                        }
                    }
                }
            }

            // Pre-calculate order totals and cache them
            var orderTotalHandling = order.TotalHandling;
            var hasAnyNonSaleDiscounts = order.HasAnyNonSaleDiscounts;
            var orderItemsCount = order.Items.Count;
            var isOrderAllItemsFreeShipping = order.IsOrderHasAllItemsQualifiedFreeShipping();

            // Update results with extra ship fees and handling - Single pass optimization
            foreach (ShippingRateDisplay displayRate in result)
            {
                // Tally up extra ship fees
                var totalExtraFees = 0m;
                var shippingMethodId = displayRate.ShippingMethodId;

                foreach (var li in order.Items)
                {
                    // Ensure correct operator precedence and conditions
                    if (li.ExtraShipCharge > 0
                        && !li.MarkedForFreeShipping(shippingMethodId)
                        && (li.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling
                            || li.ShippingCharge == ShippingChargeType.ChargeShipping))
                    {
                        totalExtraFees += li.ExtraShipCharge * li.Quantity;
                    }
                }

                displayRate.Rate += totalExtraFees + orderTotalHandling;
            }

            // Apply promotions to rates here
            var membershipServices = Factory.CreateService<MembershipServices>();
            CustomerAccount currentUser = null;

            if (!string.IsNullOrEmpty(order.UserID)) currentUser = membershipServices.Customers.Find(order.UserID);

            var marketingServices = Factory.CreateService<MarketingService>();
            var offers = marketingServices.Promotions.FindAllPotentiallyActive(DateTime.UtcNow,
                PromotionType.OfferForShipping);


            // Apply shipping offers
            foreach (ShippingRateDisplay displayRate in result)
            {
                foreach (var offer in offers)
                {
                    if (offer.DoNotCombine && hasAnyNonSaleDiscounts) continue;

                    var newRate = offer.ApplyToShippingRate(Context, order, currentUser, displayRate.ShippingMethodId,
                        displayRate.Rate);

                    if (newRate < displayRate.Rate)
                    {
                        var discount = -1 * (newRate - displayRate.Rate);
                        displayRate.PotentialDiscount = discount;
                    }
                }
            }

            //Changes to have free shipping available for single item in cart by promotion set for "Order Items" - 9May2016-Tushar
            var offersForLineItems = marketingServices.Promotions.FindAllPotentiallyActive(DateTime.UtcNow,
                PromotionType.OfferForLineItems);

            // Cache free shipping action lookup
            var freeShippingOffers = new Dictionary<string, List<IPromotionAction>>();
            foreach (var offer in offersForLineItems)
            {
                var freeShippingActions = offer.Actions.Where(
                    p => p.TypeId.ToString() == LineItemFreeShipping.TypeIdString &&
                         p.Settings.ContainsKey("methodids")).ToList();

                if (freeShippingActions.Any())
                {
                    freeShippingOffers[offer.Id.ToString()] = freeShippingActions;
                }
            }

            foreach (ShippingRateDisplay displayRate in result)
            {
                var shippingMethodIdUpper = displayRate.ShippingMethodId?.ToUpperInvariant() ?? string.Empty;

                foreach (var offer in offersForLineItems)
                {
                    var context = new PromotionContext(Context, PromotionType.OfferForLineItems, offer.Id)
                    {
                        Order = order,
                        CurrentCustomer = currentUser,
                        CustomerDescription = offer.CustomerDescription,
                        CurrentShippingMethodId = displayRate.ShippingMethodId,
                        AdjustedShippingRate = displayRate.Rate,
                        OtherOffersApplied = hasAnyNonSaleDiscounts
                    };

                    var isQualified = offer.ApplyForFreeShipping(context);
                    if (orderItemsCount == 1 || isOrderAllItemsFreeShipping)
                    {
                        var offerId = offer.Id.ToString();
                        if (freeShippingOffers.ContainsKey(offerId))
                        {
                            var isFreeShippingAction = freeShippingOffers[offerId].Any(
                                p => p.Settings["methodids"].ToUpperInvariant().Contains(shippingMethodIdUpper));

                            if (isFreeShippingAction && isQualified)
                            {
                                displayRate.PotentialDiscount = displayRate.Rate;
                            }
                        }
                    }
                }
            }
            //End changes to have free shipping available for single item in cart by promotion set for "Order Items" - 9May2016-Tushar

            FilterRatesByShippingMethods(result, order);

            // Sort Rates
            result.Sort("Rate", SortDirection.Ascending);

            if (result.Count < 1)
            {
                if (order.IsOrderFreeShipping())
                {
                    var rateName = orderTotalHandling > 0
                        ? GlobalLocalization.GetString("Handling")
                        : GlobalLocalization.GetString("FreeShipping");
                    result.Add(new ShippingRateDisplay(rateName, "", "", orderTotalHandling,
                        ShippingMethod.MethodFreeShipping));
                }
                else
                {
                    result.Add(new ShippingRateDisplay(GlobalLocalization.GetString("ToBeDetermined"), string.Empty,
                        string.Empty, 0m, ShippingMethod.MethodToBeDetermined));
                }
            }

            return result;
        }

        /// <summary>
        ///     Performs the ensure default zones operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="storeId">The supplied store id value.</param>
        public void EnsureDefaultZones(long storeId)
        {
            CreateAndReassign(ShippingZones.UnitedStatesAll());
            CreateAndReassign(ShippingZones.UnitedStates48Contiguous());
            CreateAndReassign(ShippingZones.UnitedStatesAlaskaAndHawaii());
            CreateAndReassign(ShippingZones.International("USA"));
        }

        private void CreateAndReassign(Zone zone)
        {
            var methods = ShippingMethods.FindForZones(new List<Zone> { zone });

            if (ShippingZones.Create(zone))
            {
                methods.ForEach(m =>
                {
                    m.ZoneId = zone.Id;
                    ShippingMethods.Update(m);
                });
            }
        }

        /// <summary>
        ///     Performs the remove all orders operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="storeId">The supplied store id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool RemoveAllOrders(long storeId)
        {
            using (HccDbContext db = Factory.CreateHccDbContext())
            {
                db.DeleteStoreOrders(storeId);
            }

            return true;
        }

        /// <summary>
        ///     Performs the create order strategy operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <returns>The value produced by the operation.</returns>
        protected IRepoStrategy<Order> CreateOrderStrategy()
        {
            return Factory.Instance.CreateStrategy<Order>();
        }
    }
}