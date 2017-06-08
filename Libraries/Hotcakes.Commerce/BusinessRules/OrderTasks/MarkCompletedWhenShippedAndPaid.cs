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

using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class MarkCompletedWhenShippedAndPaid : OrderTask
    {
        public override bool Execute(OrderTaskContext context)
        {
            if (!context.Order.IsRecurring
                && context.Order.ShippingStatus == OrderShippingStatus.FullyShipped
                && context.Order.PaymentStatus == OrderPaymentStatus.Paid)
            {
                var orderStatus = OrderStatusCode.FindByBvin(OrderStatusCode.Completed);
                if (orderStatus != null)
                {
                    context.Order.StatusCode = orderStatus.Bvin;
                    context.Order.StatusName = orderStatus.StatusName;
                }
            }
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            context.Order.StatusCode = OrderStatusCode.Received;
            return true;
        }

        public override string TaskId()
        {
            return "19e6e637-e651-488d-a54c-11bc249ef28f";
        }

        public override string TaskName()
        {
            return "Mark Completed When Order Is Shipped And Paid";
        }

        public override Task Clone()
        {
            return new MarkCompletedWhenShippedAndPaid();
        }
    }
}