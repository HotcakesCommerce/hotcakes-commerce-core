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

using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// Runs the payment-complete workflow when an order is paid or recurring.
    /// </summary>
    /// <remarks>
    /// The public type spelling intentionally remains <c>RunWorkFlowIfPaid</c>. Its task identifier is
    /// <c>6565A823-1591-4069-8005-2FEE50E4C9E9</c>. A child-workflow failure adds the customer-visible
    /// <c>An Error Occurred While Trying To Process Your Request, Please Try Again.</c> message and the task also
    /// registers the order-paid analytics event. Rollback does not undo child-workflow work or analytics.
    /// </remarks>
    public class RunWorkFlowIfPaid : OrderTask
    {
        /// <summary>
        /// Creates a new paid-order workflow task.
        /// </summary>
        /// <returns>A new <see cref="RunWorkFlowIfPaid"/> instance.</returns>
        public override Task Clone()
        {
            return new RunWorkFlowIfPaid();
        }

        /// <summary>
        /// Runs the payment-complete workflow for paid or recurring orders and records the paid analytics event.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> passed to the child workflow.</param>
        /// <returns>The child workflow result when applicable; otherwise <see langword="true"/>.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.PaymentStatus == OrderPaymentStatus.Paid || context.Order.IsRecurring)
            {
                var val = Workflow.RunByName(context, WorkflowNames.PaymentComplete);
                
                if (!val)
                {
                    context.Errors.Add(new WorkflowMessage("Error",
                        "An Error Occurred While Trying To Process Your Request, Please Try Again.", true));
                }
                
                context.HccApp.AnalyticsService.RegisterEvent(context.HccApp.CurrentCustomerId, ActionTypes.OrderPaid,
                    null);

                return val;
            }
            return true;
        }

        /// <summary>
        /// Does not reverse payment-complete workflow work or the analytics event.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the stable task identifier used to register this task in workflows.
        /// </summary>
        /// <returns><c>6565A823-1591-4069-8005-2FEE50E4C9E9</c>.</returns>
        public override string TaskId()
        {
            return "6565A823-1591-4069-8005-2FEE50E4C9E9";
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Run Payment Complete Workflow if Paid</c>.</returns>
        public override string TaskName()
        {
            return "Run Payment Complete Workflow if Paid";
        }
    }
}
