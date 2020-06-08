#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Data;
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
using Task = System.Threading.Tasks.Task;

namespace Hotcakes.Commerce.Orders
{
    public class OrderService : HccServiceBase
    {
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

        public OrderRepository Orders { get; protected set; }
        public TaxRepository Taxes { get; protected set; }
        public TaxScheduleRepository TaxSchedules { get; protected set; }
        public ZoneRepository ShippingZones { get; protected set; }
        public ShippingMethodRepository ShippingMethods { get; protected set; }
        public OrderTransactionRepository Transactions { get; protected set; }
        public RMARepository Returns { get; protected set; }

        private Order CachedShoppingCart
        {
            get
            {
                //we must check the HttpContext, otherwise this will fail during unit tests
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Items["CurrentShoppingCart"] != null)
                    {
                        return (Order) HttpContext.Current.Items["CurrentShoppingCart"];
                    }
                }
                return null;
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items["CurrentShoppingCart"] = value;
                }
            }
        }

        // Taxes

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
        public Zone ShippingZoneFindInList(List<Zone> zones, long id)
        {
            Zone result = null;

            foreach (var z in zones)
            {
                if (z.Id == id)
                {
                    return z;
                }
            }

            return result;
        }

        public bool ShippingZoneAddArea(long zoneId, string countryIso3, string regionAbbreviation)
        {
            var z = ShippingZones.Find(zoneId);
            if (z != null)
            {
                if (z.Id == zoneId)
                {
                    var exists = false;
                    foreach (var a in z.Areas)
                    {
                        if (a.CountryIsoAlpha3 == countryIso3 &&
                            a.RegionAbbreviation == regionAbbreviation)
                        {
                            exists = true;
                            break;
                        }
                    }
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

        public bool ShippingZoneRemoveArea(long zoneId, string countryIso3, string regionAbbreviation)
        {
            var z = ShippingZones.Find(zoneId);
            if (z != null)
            {
                if (z.Id == zoneId)
                {
                    ZoneArea located = null;
                    foreach (var a in z.Areas)
                    {
                        if (a.CountryIsoAlpha3 == countryIso3 &&
                            a.RegionAbbreviation == regionAbbreviation)
                        {
                            located = a;
                            break;
                        }
                    }
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
        public SystemOperationResult OrdersUpdateItemQuantity(long itemId, int quantity, Order o)
        {
            var result = new SystemOperationResult();
            result.Success = true;

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
                result.Success = o.Items.Remove(item);
            }

            item.Quantity = quantity;
            result.Success = true;
            return result;
        }

        private void FilterRatesByShippingMethods(SortableCollection<ShippingRateDisplay> ratesSort, Order order)
        {
            var shippingProviders = ShippingMethods.FindAll(order.StoreId);
            shippingProviders = shippingProviders.OrderBy(s => s.SortOrder).ToList();
            var subtotal = false;
            var rates = ratesSort.ToList();

            foreach (var method in shippingProviders)
            {
                var filteredrates = rates.Where(r => r.ShippingMethodId == method.Bvin).ToList();

                foreach (var rate in filteredrates)
                {
                    if (rate != null)
                    {
                        if (method.VisibilityMode == ShippingVisibilityMode.Always)
                        {
                            subtotal = true;
                        }
                        else if (method.VisibilityMode == ShippingVisibilityMode.Never)
                        {
                            ratesSort.Remove(rate);
                        }
                        else if (method.VisibilityMode == ShippingVisibilityMode.NoRates)
                        {
                            if (subtotal)
                            {
                                ratesSort.Remove(rate);
                            }
                            else
                            {
                                subtotal = true;
                            }
                        }
                        else if (method.VisibilityMode == ShippingVisibilityMode.SubtotalAmount)
                        {
                            if (method.VisibilityAmount.HasValue &&
                                order.TotalOrderAfterDiscounts > method.VisibilityAmount.Value)
                            {
                                subtotal = true;
                            }
                            else
                            {
                                ratesSort.Remove(rate);
                            }
                        }
                        else if (method.VisibilityMode == ShippingVisibilityMode.TotalWeight)
                        {
                            if (method.VisibilityAmount.HasValue && order.TotalWeight > method.VisibilityAmount.Value)
                            {
                                subtotal = true;
                            }
                            else
                            {
                                ratesSort.Remove(rate);
                            }
                        }
                    }
                }
            }
        }

        public SortableCollection<ShippingRateDisplay> FindAvailableShippingRates(Order order)
        {
            return FindAvailableShippingRatesInternal(order);
        }

        public ShippingRateDisplay FindShippingRateByUniqueKey(Order order)
        {
            return FindShippingRateByUniqueKey(order.ShippingMethodUniqueKey, order);
        }

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
        public string OrdersListPaymentMethods(Order order)
        {
            var allTransactions = Transactions.FindForOrder(order.bvin);
            return OrdersListPaymentMethods(allTransactions);
        }

        public string OrdersListPaymentMethods(List<OrderTransaction> transactions)
        {
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

        public bool AddPaymentTransactionToOrder(Order o, Transaction t)
        {
            var ot = new OrderTransaction(t);
            return AddPaymentTransactionToOrder(o, ot);
        }

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

        public OrderPaymentSummary PaymentSummary(Order o)
        {
            var result = new OrderPaymentSummary();
            result.Populate(o, this);
            o.PaymentStatus = EvaluatePaymentStatus(o, result);
            return result;
        }

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

        public bool OrdersRequestShippingMethodByUniqueKey(string rateUniqueKey, Order o)
        {
            var result = false;

            var rates = FindAvailableShippingRates(o);
            foreach (ShippingRateDisplay r in rates)
            {
                if (r.UniqueKey == rateUniqueKey)
                {
                    return OrdersRequestShippingMethod(r, o);
                }
            }

            return result;
        }

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

        public int GenerateNewOrderNumber(long storeId)
        {
            using (var context = Factory.CreateHccDbContext())
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
            var result = false;

            var productInCartList = order.Items.Where(i => i.ProductId == listItem.ProductId).ToList();
            if (productInCartList != null && productInCartList.Any())
            {
                foreach (var productInCart in productInCartList)
                {
                    var areEqual = listItem.SelectionData.Equals(productInCart.SelectionData);
                    if (areEqual)
                    {
                        if (listItem.IsGiftCard)
                        {
                            if (productInCart.BasePricePerItem == listItem.BasePricePerItem
                                &&
                                productInCart.CustomPropGiftCardEmail.Trim().ToLower() ==
                                listItem.CustomPropGiftCardEmail.Trim().ToLower()
                                &&
                                productInCart.CustomPropGiftCardName.Trim().ToLower() ==
                                listItem.CustomPropGiftCardName.Trim().ToLower()
                                &&
                                productInCart.CustomPropGiftCardMessage.Trim().ToLower() ==
                                listItem.CustomPropGiftCardMessage.Trim().ToLower())
                            {
                                productInCart.Quantity += listItem.Quantity;
                                result = true;
                            }
                        }
                        else if (listItem.IsUserSuppliedPrice)
                        {
                            if (productInCart.BasePricePerItem == listItem.BasePricePerItem)
                            {
                                productInCart.Quantity += listItem.Quantity;
                                result = true;
                            }
                        }
                        else
                        {
                            productInCart.Quantity += listItem.Quantity;
                            result = true;
                        }
                    }
                }
            }
            return result;
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

            // Update results with extra ship fees and handling
            foreach (ShippingRateDisplay displayRate in result)
            {
                // Tally up extra ship fees
                var totalExtraFees = 0m;
                foreach (var li in order.Items)
                {
                    if (li.ExtraShipCharge > 0 && !li.MarkedForFreeShipping(displayRate.ShippingMethodId) &&
                        li.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling ||
                        li.ShippingCharge == ShippingChargeType.ChargeShipping)
                    {
                        totalExtraFees += li.ExtraShipCharge*li.Quantity;
                    }
                }

                displayRate.Rate += totalExtraFees + order.TotalHandling;
            }


            // Apply promotions to rates here
            var membershipServices = Factory.CreateService<MembershipServices>();
            CustomerAccount currentUser = null;

            if (order.UserID != string.Empty) currentUser = membershipServices.Customers.Find(order.UserID);

            var marketingServices = Factory.CreateService<MarketingService>();
            var offers = marketingServices.Promotions.FindAllPotentiallyActive(DateTime.UtcNow,
                PromotionType.OfferForShipping);

            foreach (ShippingRateDisplay displayRate in result)
            {
                foreach (var offer in offers)
                {
                    if (offer.DoNotCombine && order.HasAnyNonSaleDiscounts) continue;

                    var newRate = offer.ApplyToShippingRate(Context, order, currentUser, displayRate.ShippingMethodId,
                        displayRate.Rate);

                    if (newRate < displayRate.Rate)
                    {
                        var discount = -1*(newRate - displayRate.Rate);

                        displayRate.PotentialDiscount = discount;
                    }
                }
            }

            //Changes to have free shipping available for single item in cart by promotion set for "Order Items" - 9May2016-Tushar

            var offersForLineItems = marketingServices.Promotions.FindAllPotentiallyActive(DateTime.UtcNow,
                PromotionType.OfferForLineItems);
            foreach (ShippingRateDisplay displayRate in result)
            {
                foreach (var offer in offersForLineItems)
                {
                    var context = new PromotionContext(Context, PromotionType.OfferForLineItems, offer.Id)
                    {
                        Order = order,
                        CurrentCustomer = currentUser,
                        CustomerDescription = offer.CustomerDescription,
                        CurrentShippingMethodId = displayRate.ShippingMethodId,
                        AdjustedShippingRate = displayRate.Rate,
                        OtherOffersApplied = order.HasAnyNonSaleDiscounts
                    };

                    var isQualified = offer.ApplyForFreeShipping(context);
                    if (order.Items.Count == 1 || order.IsOrderHasAllItemsQualifiedFreeShipping())
                    {
                        var isFreeShippingAction =
                            offer.Actions.Where(
                                p =>
                                    p.TypeId.ToString() == LineItemFreeShipping.TypeIdString &&
                                    p.Settings.ContainsKey("methodids") &&
                                    p.Settings["methodids"].ToUpperInvariant()
                                        .Contains(displayRate.ShippingMethodId.ToUpperInvariant())).ToList();

                        if (isFreeShippingAction.Count > 0 && isQualified)
                        {
                            displayRate.PotentialDiscount = displayRate.Rate;
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
                    var rateName = order.TotalHandling > 0
                        ? GlobalLocalization.GetString("Handling")
                        : GlobalLocalization.GetString("FreeShipping");
                    result.Add(new ShippingRateDisplay(rateName, "", "", order.TotalHandling,
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

        public void EnsureDefaultZones(long storeId)
        {
            CreateAndReassign(ShippingZones.UnitedStatesAll());
            CreateAndReassign(ShippingZones.UnitedStates48Contiguous());
            CreateAndReassign(ShippingZones.UnitedStatesAlaskaAndHawaii());
            CreateAndReassign(ShippingZones.International("USA"));
        }

        private void CreateAndReassign(Zone zone)
        {
            var methods = ShippingMethods.FindForZones(new List<Zone> {zone});

            if (ShippingZones.Create(zone))
            {
                methods.ForEach(m =>
                {
                    m.ZoneId = zone.Id;
                    ShippingMethods.Update(m);
                });
            }
        }

        public bool RemoveAllOrders(long storeId)
        {
            using (var db = Factory.CreateHccDbContext())
            {
                db.DeleteStoreOrders(storeId);
            }

            return true;
        }

        protected IRepoStrategy<Order> CreateOrderStrategy()
        {
            return Factory.Instance.CreateStrategy<Order>();
        }
    }
}