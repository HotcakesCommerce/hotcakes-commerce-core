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

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>Persists the current order through the commerce order service.</summary>
    /// <remarks>This is an <see cref="OrderTask"/> executed with an <see cref="OrderTaskContext"/>.</remarks>
    public class UpdateOrder : OrderTask
    {
        /// <summary>Updates the current order and returns the order service result.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>The result returned by the order service update.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            return context.HccApp.OrderServices.Orders.Update(context.Order);
        }

        /// <summary>Reports that this order update has no local rollback implementation.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>Gets the stable workflow identifier for this task.</summary>
        /// <returns>The task ID <c>30B9B11C-1621-4779-ABC2-FEC5D280484B</c>.</returns>
        public override string TaskId()
        {
            return "30B9B11C-1621-4779-ABC2-FEC5D280484B";
        }

        /// <summary>Gets the display name of this workflow task.</summary>
        /// <returns><c>Update Order</c>.</returns>
        public override string TaskName()
        {
            return "Update Order";
        }

        /// <summary>Gets the step label, which is the task name.</summary>
        /// <returns><c>Update Order</c>.</returns>
        public override string StepName()
        {
            var result = string.Empty;
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        /// <summary>Creates an independent order-update task instance.</summary>
        /// <returns>A new <see cref="UpdateOrder"/> instance.</returns>
        public override Task Clone()
        {
            return new UpdateOrder();
        }
    }
}
