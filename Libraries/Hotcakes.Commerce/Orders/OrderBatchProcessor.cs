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

            // Process all pages, not just the first 1000
            do
            {
                var orders = svc.Orders.FindByCriteriaPaged(criteria, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var o in orders)
                {
                    var ord = svc.Orders.FindForCurrentStore(o.bvin);
                    if (ord != null)
                    {
                        ord.StatusCode = OrderStatusCode.ReadyForPayment;
                        ord.StatusName = "Ready for Payment";
                        svc.Orders.Update(ord);
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
                foreach (var objOrderSnapshot in orders)
                {
                    // Use snapshot bvin directly instead of retrieving full order again
                    var orderBvin = objOrderSnapshot.bvin;

                    // Get transactions and delete them
                    var transactions = app.OrderServices.Transactions.FindForOrder(orderBvin);
                    foreach (var transaction in transactions)
                    {
                        app.OrderServices.Transactions.Delete(transaction.Id);
                    }

                    // Only retrieve full order once for inventory unreservation
                    var objOrder = app.OrderServices.Orders.FindForCurrentStore(orderBvin);
                    if (objOrder != null)
                    {
                        var lstLineItems = objOrder.Items;

                        foreach (var objLineItem in lstLineItems)
                        {
                            // Remove unused inventory retrieval - it's never used
                            app.CatalogServices.InventoryLineItemUnreserveInventory(objLineItem);
                        }

                        app.OrderServices.Orders.Delete(orderBvin);
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

            // Process all pages, not just the first 1000
            do
            {
                var orders = app.OrderServices.Orders.FindByUserId(CustomerID, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var objOrderSnapshot in orders)
                {
                    // Use snapshot bvin directly instead of retrieving full order again
                    var orderBvin = objOrderSnapshot.bvin;

                    // Get transactions and delete them
                    var transactions = app.OrderServices.Transactions.FindForOrder(orderBvin);
                    foreach (var transaction in transactions)
                    {
                        app.OrderServices.Transactions.Delete(transaction.Id);
                    }

                    // Only retrieve full order once for inventory unreservation
                    var objOrder = app.OrderServices.Orders.FindForCurrentStore(orderBvin);
                    if (objOrder != null)
                    {
                        var lstLineItems = objOrder.Items;

                        foreach (var objLineItem in lstLineItems)
                        {
                            // Remove unused inventory retrieval - it's never used
                            app.CatalogServices.InventoryLineItemUnreserveInventory(objLineItem);
                        }

                        app.OrderServices.Orders.Delete(orderBvin);
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

            // Process all pages, not just the first 1000
            do
            {
                var orders = app.OrderServices.Orders.FindByCriteriaPaged(criteria, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var os in orders)
                {
                    // Retrieve full order only once
                    var o = app.OrderServices.Orders.FindForCurrentStore(os.bvin);
                    if (o == null)
                        continue;

                    var payManager = new OrderPaymentManager(o, app);
                    payManager.GiftCardCompleteAllGiftCards();
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
                            o.StatusName = "Ready for Shipping";
                        }
                        app.OrderServices.Orders.Update(o);
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

            // Process all pages, not just the first 1000
            do
            {
                var orders = app.OrderServices.Orders.FindByCriteriaPaged(criteria, currentPage, pageSize, ref totalCount);
                if (orders == null || orders.Count == 0)
                    break;

                foreach (var os in orders)
                {
                    // Retrieve full order only once
                    var o = app.OrderServices.Orders.FindForCurrentStore(os.bvin);
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