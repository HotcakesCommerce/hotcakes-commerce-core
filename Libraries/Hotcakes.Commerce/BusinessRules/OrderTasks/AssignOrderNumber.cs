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
    public class AssignOrderNumber : OrderTask
    {
        public override bool Execute(OrderTaskContext context)
        {
            // Assign Order Number
            if (string.IsNullOrEmpty(context.Order.OrderNumber))
            {
                context.Order.OrderNumber =
                    context.HccApp.OrderServices.GenerateNewOrderNumber(
                        context.HccApp.CurrentRequestContext.CurrentStore.Id).ToString();

                var note = new OrderNote();
                note.IsPublic = false;
                note.Note = "This order was assigned number " + context.Order.OrderNumber;
                context.Order.Notes.Add(note);
            }
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            EventLog.LogEvent("Order Workflow",
                "Order number " + context.Order.OrderNumber +
                " was assigned but the order was not completed. The cart ID is " + context.Order.bvin,
                EventLogSeverity.Information);
            return true;
        }

        public override string TaskId()
        {
            return "7DA816D7-CC81-4727-8788-0CE911F4A93E";
        }

        public override string TaskName()
        {
            return "Assign Order Number";
        }

        public override string StepName()
        {
            var result = string.Empty;
            result = "Assign Order Number";
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        public override Task Clone()
        {
            return new AssignOrderNumber();
        }
    }
}