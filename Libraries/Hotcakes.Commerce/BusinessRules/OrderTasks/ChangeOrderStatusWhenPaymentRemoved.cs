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

using Hotcakes.Commerce.Orders;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// <summary>
    /// Represents the <see cref="OrderTask"/> that performs the ChangeOrderStatusWhenPaymentRemoved workflow operation. The stable task identifier is <c>4aa03a0d-1d38-4f66-ad85-82edf715103b</c>. Execution and rollback preserve existing order workflow behavior, including persistence, <see cref="WorkflowMessage"/> errors, payment activity, email delivery, and rollback limitations.
    /// </summary>
    /// </summary>
    public class ChangeOrderStatusWhenPaymentRemoved : OrderTask
    {
        /// <summary>
        /// Executes this order workflow task using the supplied <see cref="OrderTaskContext"/>. The return value controls whether the workflow continues; order changes, persistence, messages, payments, or email follow the existing implementation.
        /// </summary>
        /// <param name="context">The value used by this member.</param>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.IsRecurring)
            {
                return true;
            }

            if (context.PreviousPaymentStatus == OrderPaymentStatus.Paid &&
                context.Order.PaymentStatus != OrderPaymentStatus.Paid)
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
                    EventLog.LogEvent("Change Order Status When Payment Removed",
                        "Could not find order status with id of " + statusCode, EventLogSeverity.Error);
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to roll back this task using the supplied <see cref="OrderTaskContext"/>. A successful return does not imply that external persistence, payment processing, account creation, or email delivery was reversed.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Rollback(OrderTaskContext context)
        {
            context.Order.StatusCode = OrderStatusCode.Received;
            return true;
        }

        /// <summary>
        /// Gets this task's stable workflow identifier.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskId()
        {
            return "4aa03a0d-1d38-4f66-ad85-82edf715103b";
        }

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskName()
        {
            return "Change Order Status When Payment Removed";
        }

        /// <summary>
        /// Gets the display name used for this workflow step.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string StepName()
        {
            var result = string.Empty;
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        /// <summary>
        /// Creates a new task instance with the same task type and documented constructor defaults.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override Task Clone()
        {
            return new ChangeOrderStatusWhenPaymentRemoved();
        }
    }
}
