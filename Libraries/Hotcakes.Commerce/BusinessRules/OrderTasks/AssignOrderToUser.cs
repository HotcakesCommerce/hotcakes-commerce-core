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
    /// <summary>
    /// <summary>
    /// Represents the <see cref="OrderTask"/> that performs the AssignOrderToUser workflow operation. The stable task identifier is <c>71EC229F-F0EF-4a6e-9790-9E84DC8DCA09</c>. Execution and rollback preserve the existing order workflow behavior, including any persistence, customer-visible <see cref="WorkflowMessage"/> errors, payment activity, email delivery, and rollback limitations.
    /// </summary>
    /// </summary>
    public class AssignOrderToUser : OrderTask
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
            context.Order.UserID = context.UserId;
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
            context.Order.UserID = string.Empty;
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
            return "71EC229F-F0EF-4a6e-9790-9E84DC8DCA09";
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
            return "Assign Order To User";
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
            result = "Assign Order to User";
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
            return new AssignOrderToUser();
        }
    }
}
