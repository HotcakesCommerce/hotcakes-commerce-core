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
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Api.Mobile.Models;
using Order = Hotcakes.Modules.Core.Api.Mobile.Models.Order;

namespace Hotcakes.Modules.Core.Api.Mobile
{
    /// <summary>
    /// Provides mobile API operations for searching and managing orders.
    /// </summary>
    [Serializable]
    public class OrdersController : HccApiController
    {
        /// <summary>
        /// Gets a page of order summaries for the requested period and status.
        /// </summary>
        /// <param name="pageSize">The maximum number of orders to return.</param>
        /// <param name="pageNumber">The one-based page number to return.</param>
        /// <param name="period">The period to search, such as <c>day</c>, <c>week</c>, <c>month</c>, <c>year</c>, or <c>all</c>.</param>
        /// <param name="status">The order status name to filter by.</param>
        /// <returns>The order summaries for the requested page.</returns>
        [MobileAuthorize]
        public IEnumerable<OrderBrief> Get(int pageSize = 100, int pageNumber = 1, string period = "year",
            string status = "")
        {
            var totalCount = 0;
            List<OrderSnapshot> data = null;
            var dates = GetDateRange(period);

            if (status == "Ready for payment")
            {
                data = HccApp.OrderServices.Orders.GetReadyForPaymentPaged(dates.StartDate, dates.EndDate, pageNumber,
                    pageSize, ref totalCount);
            }
            else if (status == "Ready for shipping")
            {
                data = HccApp.OrderServices.Orders.GetReadyForShippingPaged(dates.StartDate, dates.EndDate, pageNumber,
                    pageSize, ref totalCount);
            }
            else
            {
                var statusCode = OrderStatusCode.FindAll().FirstOrDefault(o => o.StatusName == status);
                var statusCodeBvin = string.Empty;
                if (statusCode != null)
                    statusCodeBvin = statusCode.Bvin;

                data = GetOrdersData(pageSize, pageNumber, dates, statusCodeBvin, ref totalCount);
            }

            return data.Select(d => new OrderBrief
            {
                OrderId = d.bvin,
                OrderDate = d.TimeOfOrderUtc,
                CustomerName = string.Format("{0} {1}", d.BillingAddress.FirstName, d.BillingAddress.LastName),
                Total = d.TotalGrand,
                OrderNumber = d.OrderNumber,
                StatusCode = d.StatusName,
                PaymentStatus = d.PaymentStatus.ToString(),
                ShippingStatus = d.ShippingStatus.ToString()
            }).ToList();
        }

        /// <summary>
        /// Gets the detailed order identified by its store-scoped ID.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>The detailed order, or <see langword="null"/> when it cannot be found for the current store.</returns>
        [MobileAuthorize]
        public Order Get(string id)
        {
            var data = HccApp.OrderServices.Orders.FindForCurrentStore(id);

            if (data != null)
            {
                var order = HccApp.OrderServices.Orders.FindForCurrentStore(data.bvin);
                var payManager = new OrderPaymentManager(order, HccApp);
                var auths = payManager.CreditCardHoldListAll();
                var paySummary = HccApp.OrderServices.PaymentSummary(order);

                return new Order
                {
                    OrderId = data.bvin,
                    OrderDate = DateHelper.ConvertUtcToStoreTime(HccApp, data.TimeOfOrderUtc),
                    CustomerName = string.Format("{0} {1}", data.BillingAddress.FirstName, data.BillingAddress.LastName),
                    CustomerEmail = data.UserEmail,
                    CustomerPhone = data.BillingAddress.Phone,
                    TotalDiscounts = TotalOrderDiscounts(data),
                    Total = data.TotalGrand,
                    OrderNumber = data.OrderNumber,
                    StatusCode = data.StatusName,
                    PaymentStatus = data.PaymentStatus.ToString(),
                    ShippingStatus = data.ShippingStatus.ToString(),
                    ShippingPrice = data.TotalShippingBeforeDiscounts,
                    Tax = data.TotalTax,
                    Items = data.Items.Select(i => new OrderItem
                    {
                        ProductName = i.ProductName,
                        PricePerItem = i.BasePricePerItem,
                        Quantity = i.Quantity
                    }).ToList(),
                    PendingHolds = auths.Select(a => new HoldTransaction
                    {
                        CardInfo =
                            a.CreditCard.CardTypeName + "-" + a.CreditCard.CardNumberLast4Digits + " - " +
                            a.Amount.ToString("c"),
                        TransactionId = a.IdAsString
                    }).ToList(),
                    SpecialInstructions = data.Instructions,
                    PaymentAmountAuthorized = paySummary.AmountAuthorized,
                    PaymentAmountCharged = paySummary.AmountCharged,
                    PaymentAmountRefunded = paySummary.AmountRefunded,
                    PaymentAmountDue = paySummary.AmountDue
                };
            }
            return null;
        }

        /// <summary>
        /// Captures a payment against an authorized order transaction.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="transactionId">The payment transaction ID.</param>
        /// <param name="amount">The amount to capture as a decimal string.</param>
        /// <returns>An HTTP response containing the payment-capture result.</returns>
        [MobileAuthorize]
        [HttpGet]
        public HttpResponseMessage CapturePayment(string orderId, string transactionId, string amount)
        {
            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderId);
            var payManager = new OrderPaymentManager(order, HccApp);
            var authTrans = payManager.FindTransactionById(transactionId);
            var dAmount = Convert.ToDecimal(amount);
            var status = payManager.CreditCardCapture(transactionId, dAmount);

            return Request.CreateResponse(HttpStatusCode.OK, status);
        }

        /// <summary>
        /// Gets the shipping providers available to the mobile client.
        /// </summary>
        /// <param name="orderId">The order ID associated with the shipping operation.</param>
        /// <returns>The supported shipping providers.</returns>
        [MobileAuthorize]
        public IEnumerable<Shipper> GetShipperList(string orderId)
        {
            return new List<Shipper>
            {
                new Shipper {Code = "1", Name = "US Postal Service"},
                new Shipper {Code = "2", Name = "FedEx"},
                new Shipper {Code = "3", Name = "US Postal"},
                new Shipper {Code = "0", Name = "Other"}
            };
        }

        /// <summary>
        /// Marks all items in an order as shipped with the supplied tracking information.
        /// </summary>
        /// <param name="orderId">The order ID.</param>
        /// <param name="shipperCode">The shipping provider code.</param>
        /// <param name="number">The shipment tracking number.</param>
        /// <returns>An empty successful HTTP response when the shipment is recorded.</returns>
        [MobileAuthorize]
        [HttpGet]
        public HttpResponseMessage MarkAsShipped(string orderId, string shipperCode, string number)
        {
            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderId);
            var previousShippingStatus = order.ShippingStatus;


            var p = ShipItems(order, number.Trim(), order.ShippingProviderId, shipperCode);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        #region Implementation

        private OrderPackage ShipItems(Commerce.Orders.Order o, string trackingNumber, string serviceProvider,
            string serviceCode)
        {
            var p = new OrderPackage
            {
                ShipDateUtc = DateTime.UtcNow,
                TrackingNumber = trackingNumber,
                ShippingProviderId = serviceProvider,
                ShippingProviderServiceCode = serviceCode
            };
            
            foreach (var li in o.Items)
            {
                if (li != null)
                {
                    var qty = li.Quantity;
                    p.Items.Add(new OrderPackageItem(li.ProductId, li.Id, qty));
                    p.Weight += li.ProductShippingWeight*qty;
                }
            }

            p.WeightUnits = WebAppSettings.ApplicationWeightUnits;
            o.Packages.Add(p);

            HccApp.OrdersShipPackage(p, o);
            o.EvaluateCurrentShippingStatus();
            HccApp.OrderServices.Orders.Update(o);

            return p;
        }

        private List<OrderSnapshot> GetOrdersData(int pageSize, int pageNumber, DateRange range, string statusFilter,
            ref int totalCount)
        {
            var criteria = new OrderSearchCriteria
            {
                StartDateUtc = range.StartDate,
                EndDateUtc = range.EndDate,
                StatusCode = statusFilter,
                SortDescending = true
            };

            return HccApp.OrderServices.Orders.FindByCriteriaPaged(criteria, pageNumber, pageSize, ref totalCount);
        }

        /// <summary>
        ///     return the sum of all discounts for a given order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private decimal TotalOrderDiscounts(Commerce.Orders.Order order)
        {
            decimal discounts = 0;

            //order discounts
            discounts += order.TotalOrderDiscounts;

            foreach (var discount in order.ShippingDiscountDetails)
            {
                discounts += discount.Amount;
            }

            foreach (var lineitem in order.Items)
            {
                foreach (var discount in lineitem.DiscountDetails)
                {
                    discounts += discount.Amount;
                }
            }

            return discounts;
        }

        #endregion
    }
}
