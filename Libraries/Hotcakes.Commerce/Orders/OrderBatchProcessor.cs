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

            var orders = svc.Orders.FindByCriteriaPaged(criteria, 1, pageSize, ref totalCount);
            if (orders != null)
            {
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
            }
        }

        public static bool RemoveAllOrders(HotcakesApplication app)
        {
            var orders = app.OrderServices.Orders.FindAll();
            if (orders != null)
            {
                foreach (var objOrderSnapshot in orders)
                {
                    var objOrder = app.OrderServices.Orders.FindForCurrentStore(objOrderSnapshot.bvin);

                    app.OrderServices.Transactions.FindForOrder(objOrderSnapshot.bvin)
                        .ForEach(p => app.OrderServices.Transactions.Delete(p.Id));

                    var lstLineItems = objOrder.Items;

                    foreach (var objLineItem in lstLineItems)
                    {
                        var inv =
                            app.CatalogServices.ProductInventories.FindByProductIdAndVariantId(objLineItem.ProductId,
                                objLineItem.VariantId);

                        app.CatalogServices.InventoryLineItemUnreserveInventory(objLineItem);
                    }

                    app.OrderServices.Orders.Delete(objOrder.bvin);
                }
            }

            return true;
        }

        public static bool RemoveAllOrdersOfCustomer(HotcakesApplication app, string CustomerID)
        {
            var totalCount = 0;
            var orders = app.OrderServices.Orders.FindByUserId(CustomerID, 1, 1000, ref totalCount);

            if (orders != null)
            {
                foreach (var objOrderSnapshot in orders)
                {
                    var objOrder = app.OrderServices.Orders.FindForCurrentStore(objOrderSnapshot.bvin);

                    app.OrderServices.Transactions.FindForOrder(objOrderSnapshot.bvin)
                        .ForEach(p => app.OrderServices.Transactions.Delete(p.Id));

                    var lstLineItems = objOrder.Items;

                    foreach (var objLineItem in lstLineItems)
                    {
                        var inv =
                            app.CatalogServices.ProductInventories.FindByProductIdAndVariantId(objLineItem.ProductId,
                                objLineItem.VariantId);

                        app.CatalogServices.InventoryLineItemUnreserveInventory(objLineItem);
                    }

                    app.OrderServices.Orders.Delete(objOrderSnapshot.bvin);
                }
            }

            return true;
        }

        public static void CollectPaymentAndShipPendingOrders(HotcakesApplication app)
        {
            var criteria = new OrderSearchCriteria();
            criteria.IsPlaced = true;
            criteria.StatusCode = OrderStatusCode.ReadyForPayment;
            var pageSize = 1000;
            var totalCount = 0;

            var orders = app.OrderServices.Orders.FindByCriteriaPaged(criteria, 1, pageSize, ref totalCount);
            if (orders != null)
            {
                foreach (var os in orders)
                {
                    var o = app.OrderServices.Orders.FindForCurrentStore(os.bvin);
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
            }
        }


        public static bool ChargeSingleOrder(HotcakesApplication app, Order o)
        {
            if (o == null) return false;

            var payManager = new OrderPaymentManager(o, app);
            payManager.GiftCardCompleteAllGiftCards();
            app.OrderServices.Orders.Update(o);

            payManager = new OrderPaymentManager(o, app);
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

            var orders = app.OrderServices.Orders.FindByCriteriaPaged(criteria, 1, pageSize, ref totalCount);
            if (orders != null)
            {
                foreach (var os in orders)
                {
                    var o = app.OrderServices.Orders.FindForCurrentStore(os.bvin);
                    o.MoveToNextStatus();
                    app.OrderServices.Orders.Update(o);
                }
            }
        }
    }
}