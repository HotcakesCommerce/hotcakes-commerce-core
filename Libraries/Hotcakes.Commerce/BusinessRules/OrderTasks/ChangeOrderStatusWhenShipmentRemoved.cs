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
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class ChangeOrderStatusWhenShipmentRemoved : OrderTask
    {
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Inputs["PreviousShippingStatus"] != null)
            {
                var val = 0;
                var parsed = int.TryParse(context.Inputs["PreviousShippingStatus"].Value, out val);
                if (parsed && val == (int) OrderShippingStatus.FullyShipped)
                {
                    if (context.Order.ShippingStatus != OrderShippingStatus.NonShipping &&
                        context.Order.ShippingStatus != OrderShippingStatus.FullyShipped)
                    {
                        var statusCode = OrderStatusCode.Received;
                        var orderStatus = OrderStatusCode.FindByBvin(statusCode);
                        if (orderStatus != null && orderStatus.Bvin != string.Empty)
                        {
                            context.Order.StatusCode = orderStatus.Bvin;
                            context.Order.StatusName = orderStatus.StatusName;
                        }
                        else
                        {
                            EventLog.LogEvent("Change Order Status When Shipment Removed",
                                "Could not find order status with id of " + statusCode, EventLogSeverity.Error);
                        }
                    }
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
            return "b5d9c29d-2761-439d-92a4-6ae35870ba5b";
        }

        public override string TaskName()
        {
            return "Change Order Status When Shipment Removed";
        }

        public override string StepName()
        {
            var result = string.Empty;
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        public override Task Clone()
        {
            return new ChangeOrderStatusWhenShipmentRemoved();
        }
    }
}