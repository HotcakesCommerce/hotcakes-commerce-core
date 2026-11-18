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

using System.Linq;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// <summary>
    /// Represents the <see cref="OrderTask"/> that performs the CheckForOrderMaximums workflow operation. The stable task identifier is <c>D39354D5-6CD5-4a77-95E4-15D7609AA164</c>. Execution and rollback preserve existing order workflow behavior, including persistence, <see cref="WorkflowMessage"/> errors, payment activity, email delivery, and rollback limitations.
    /// </summary>
    /// </summary>
    public class CheckForOrderMaximums : OrderTask
    {
        /// <summary>
        /// Creates a new task instance with the same task type and documented constructor defaults.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override Task Clone()
        {
            return new CheckForOrderMaximums();
        }

        /// <summary>
        /// Executes this order workflow task using the supplied <see cref="OrderTaskContext"/>. The return value controls whether the workflow continues; order changes, persistence, messages, payments, or email follow the existing implementation.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Execute(OrderTaskContext context)
        {
            var maxItems = context.HccApp.CurrentRequestContext.CurrentStore.Settings.MaxItemsPerOrder;
            if (maxItems <= 0)
                maxItems = 99999;
            var maxWeight = context.HccApp.CurrentRequestContext.CurrentStore.Settings.MaxWeightPerOrder;
            if (maxWeight <= 0)
                maxWeight = 99999;

            var totalItems = context.Order.Items.Sum(y => y.Quantity);
            var totalWeight = context.Order.TotalWeightOfShippingItems();

            if (totalItems > maxItems || totalWeight > maxWeight)
            {
                var maxMessage = GlobalLocalization.GetString("MaxOrderMessage");
                context.Errors.Add(new WorkflowMessage("Order Too Large", maxMessage, true));
                return false;
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
            return "D39354D5-6CD5-4a77-95E4-15D7609AA164";
        }

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskName()
        {
            return "Check For Order Maximums";
        }
    }
}
