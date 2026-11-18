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
    /// Represents the <see cref="OrderTask"/> that performs the AssignOrderNumber workflow operation. The stable task identifier is <c>7DA816D7-CC81-4727-8788-0CE911F4A93E</c>. Execution and rollback preserve the existing order workflow behavior, including any persistence, customer-visible <see cref="WorkflowMessage"/> errors, payment activity, email delivery, and rollback limitations.
    /// </summary>
    /// </summary>
    public class AssignOrderNumber : OrderTask
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

        /// <summary>
        /// Attempts to roll back this task using the supplied <see cref="OrderTaskContext"/>. A successful return does not imply that external persistence, payment processing, account creation, or email delivery was reversed.
        /// </summary>
        /// <param name="context">The value used by this member.</param>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Rollback(OrderTaskContext context)
        {
            EventLog.LogEvent("Order Workflow",
                "Order number " + context.Order.OrderNumber +
                " was assigned but the order was not completed. The cart ID is " + context.Order.bvin,
                EventLogSeverity.Information);
            return true;
        }

        /// <summary>
        /// Gets this task's stable workflow identifier.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskId()
        {
            return "7DA816D7-CC81-4727-8788-0CE911F4A93E";
        }

        /// <summary>
        /// Gets the display name registered for this order task.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskName()
        {
            return "Assign Order Number";
        }

        /// <summary>
        /// Gets the display name used for this workflow step.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
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

        /// <summary>
        /// Creates a new task instance with the same task type and its documented constructor defaults.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override Task Clone()
        {
            return new AssignOrderNumber();
        }
    }
}
