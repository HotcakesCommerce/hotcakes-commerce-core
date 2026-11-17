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

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// Runs the shipping-complete workflow when the order is fully shipped.
    /// </summary>
    /// <remarks>
    /// The public type spelling intentionally remains <c>RunShippingCompleteWorkFlow</c>. Its task identifier is
    /// <c>5ba90d63-ff6c-4ca4-8e34-7705ec3b2728</c>. The child workflow is skipped until
    /// <see cref="OrderShippingStatus.FullyShipped"/>; rollback does not undo child-workflow side effects.
    /// </remarks>
    public class RunShippingCompleteWorkFlow : OrderTask
    {
        /// <summary>
        /// Creates a new shipping-complete workflow task.
        /// </summary>
        /// <returns>A new <see cref="RunShippingCompleteWorkFlow"/> instance.</returns>
        public override Task Clone()
        {
            return new RunShippingCompleteWorkFlow();
        }

        /// <summary>
        /// Runs the shipping-complete workflow only when the order is fully shipped.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> passed to the child workflow.</param>
        /// <returns>The child workflow result when fully shipped; otherwise <see langword="true"/>.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.ShippingStatus == OrderShippingStatus.FullyShipped)
            {
                return Workflow.RunByName(context, WorkflowNames.ShippingComplete);
            }
            return true;
        }

        /// <summary>
        /// Does not reverse work performed by the shipping-complete workflow.
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
        /// <returns><c>5ba90d63-ff6c-4ca4-8e34-7705ec3b2728</c>.</returns>
        public override string TaskId()
        {
            return "5ba90d63-ff6c-4ca4-8e34-7705ec3b2728";
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Run Shipping Complete Workflow If Needed</c>.</returns>
        public override string TaskName()
        {
            return "Run Shipping Complete Workflow If Needed";
        }
    }
}
