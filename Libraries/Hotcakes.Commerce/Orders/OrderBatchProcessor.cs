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

namespace Hotcakes.Commerce.Orders
{
    public class OrderBatchProcessor
    {
        public static void AcceptAllNewOrders(OrderService svc)
        {
            var criteria = new OrderSearchCriteria();
            criteria.IsPlaced = true;
            criteria.StatusCode = OrderStatusCode.Received;
            var pageSize = 1000;
            var totalCount = 0;
            var currentPage = 1;

            var ordersRepo = svc.Orders;

            // Process all pages, not just the first 1000
            do
            {
                var orders = ordersRepo.FindByCriteriaPaged(criteria, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var o in orders)
                {
                    var ord = ordersRepo.FindForCurrentStore(o.bvin);
                    if (ord != null)
                    {
                        // Avoid unnecessary updates by checking current status
                        if (ord.StatusCode != OrderStatusCode.ReadyForPayment)
                        {
                            ord.StatusCode = OrderStatusCode.ReadyForPayment;
                            ord.StatusName = "Ready for Payment";
                            ordersRepo.Update(ord);
                        }
                    }
                }

                currentPage++;
            } while ((currentPage - 1) * pageSize < totalCount);
        }

        public static bool RemoveAllOrders(HotcakesApplication app)
        {
            var orders = app.OrderServices.Orders.FindAll();
            if (orders != null)
            {
                var transactionsRepo = app.OrderServices.Transactions;
                var ordersRepo = app.OrderServices.Orders;
                var catalog = app.CatalogServices;

                foreach (var objOrderSnapshot in orders)
                {
                    // Use snapshot bvin directly instead of retrieving full order again
                    var orderBvin = objOrderSnapshot.bvin;

                    // Get transactions and delete them
                    var transactions = transactionsRepo.FindForOrder(orderBvin);
                    foreach (var transaction in transactions)
                    {
                        transactionsRepo.Delete(transaction.Id);
                    }

                    // Only retrieve full order once for inventory unreservation
                    var objOrder = ordersRepo.FindForCurrentStore(orderBvin);
                    if (objOrder != null)
                    {
                        var lstLineItems = objOrder.Items;

                        foreach (var objLineItem in lstLineItems)
                        {
                            catalog.InventoryLineItemUnreserveInventory(objLineItem);
                        }

                        ordersRepo.Delete(orderBvin);
                    }
                }
            }

            return true;
        }

        public static bool RemoveAllOrdersOfCustomer(HotcakesApplication app, string CustomerID)
        {
            var totalCount = 0;
            var pageSize = 1000;
            var currentPage = 1;

            var transactionsRepo = app.OrderServices.Transactions;
            var ordersRepo = app.OrderServices.Orders;
            var catalog = app.CatalogServices;

            // Process all pages, not just the first 1000
            do
            {
                var orders = ordersRepo.FindByUserId(CustomerID, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var objOrderSnapshot in orders)
                {
                    // Use snapshot bvin directly instead of retrieving full order again
                    var orderBvin = objOrderSnapshot.bvin;

                    // Get transactions and delete them
                    var transactions = transactionsRepo.FindForOrder(orderBvin);
                    foreach (var transaction in transactions)
                    {
                        transactionsRepo.Delete(transaction.Id);
                    }

                    // Only retrieve full order once for inventory unreservation
                    var objOrder = ordersRepo.FindForCurrentStore(orderBvin);
                    if (objOrder != null)
                    {
                        var lstLineItems = objOrder.Items;

                        foreach (var objLineItem in lstLineItems)
                        {
                            catalog.InventoryLineItemUnreserveInventory(objLineItem);
                        }

                        ordersRepo.Delete(orderBvin);
                    }
                }

                currentPage++;
            } while ((currentPage - 1) * pageSize < totalCount);

            return true;
        }

        public static void CollectPaymentAndShipPendingOrders(HotcakesApplication app)
        {
            var criteria = new OrderSearchCriteria();
            criteria.IsPlaced = true;
            criteria.StatusCode = OrderStatusCode.ReadyForPayment;
            var pageSize = 1000;
            var totalCount = 0;
            var currentPage = 1;

            var ordersRepo = app.OrderServices.Orders;

            // Process all pages, not just the first 1000
            do
            {
                var orders = ordersRepo.FindByCriteriaPaged(criteria, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var os in orders)
                {
                    // Retrieve full order only once
                    var o = ordersRepo.FindForCurrentStore(os.bvin);
                    if (o == null)
                        continue;

                    var payManager = new OrderPaymentManager(o, app);
                    payManager.GiftCardCompleteAllGiftCards();
                    payManager.CreditCardCompleteAllCreditCards();
                    payManager.PayPalExpressCompleteAllPayments();

                    if (o.PaymentStatus == OrderPaymentStatus.Paid ||
                        o.PaymentStatus == OrderPaymentStatus.Overpaid)
                    {
                        var newStatus = o.ShippingStatus == OrderShippingStatus.FullyShipped
                            ? OrderStatusCode.Completed
                            : OrderStatusCode.ReadyForShipping;

                        var newStatusName = newStatus == OrderStatusCode.Completed ? "Completed" : "Ready for Shipping";

                        // Only update when status actually changes to avoid redundant DB writes
                        if (o.StatusCode != newStatus || o.StatusName != newStatusName)
                        {
                            o.StatusCode = newStatus;
                            o.StatusName = newStatusName;
                            ordersRepo.Update(o);
                        }
                    }
                }

                currentPage++;
            } while ((currentPage - 1) * pageSize < totalCount);
        }


        public static bool ChargeSingleOrder(HotcakesApplication app, Order o)
        {
            if (o == null) return false;

            // Create payment manager only once and reuse
            var payManager = new OrderPaymentManager(o, app);
            payManager.GiftCardCompleteAllGiftCards();
            app.OrderServices.Orders.Update(o);

            // Reuse the same payment manager instance
            payManager.CreditCardCompleteAllCreditCards();
            payManager.PayPalExpressCompleteAllPayments();

            if (o.PaymentStatus == OrderPaymentStatus.Paid ||
                o.PaymentStatus == OrderPaymentStatus.Overpaid)
            {
                if (o.ShippingStatus == OrderShippingStatus.FullyShipped)
                {
                    o.StatusCode = OrderStatusCode.Completed;
                    o.StatusName = "Completed";
                }
                else
                {
                    o.StatusCode = OrderStatusCode.ReadyForShipping;
                    o.StatusName = "Ready to Ship";
                }
                app.OrderServices.Orders.Update(o);
                return true;
            }
            return false;
        }


        public static void MarkAllToBeShippedOrdersAsComplete(HotcakesApplication app)
        {
            var criteria = new OrderSearchCriteria();
            criteria.IsPlaced = true;
            criteria.StatusCode = OrderStatusCode.ReadyForShipping;
            var pageSize = 1000;
            var totalCount = 0;
            var currentPage = 1;

            var ordersRepo = app.OrderServices.Orders;

            // Process all pages, not just the first 1000
            do
            {
                var orders = ordersRepo.FindByCriteriaPaged(criteria, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var os in orders)
                {
                    // Retrieve full order only once
                    var o = ordersRepo.FindForCurrentStore(os.bvin);
                    if (o == null)
                        continue;

                    o.MoveToNextStatus();
                    app.OrderServices.Orders.Update(o);
                }

                currentPage++;
            } while ((currentPage - 1) * pageSize < totalCount);
        }
    }
}