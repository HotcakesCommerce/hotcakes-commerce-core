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

using System;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Base class for tasks that process an <see cref="OrderTaskContext"/>.</summary>
    /// <remarks>The <see cref="TaskContext"/> overloads cast to <see cref="OrderTaskContext"/>; workflows must supply the matching context type.</remarks>
    public abstract class OrderTask : Task
    {
        /// <summary>Executes this order task using an order-specific context.</summary>
        /// <param name="context">The context to cast to <see cref="OrderTaskContext"/>.</param>
        /// <returns>The result returned by <see cref="Execute(OrderTaskContext)"/>.</returns>
        public override bool Execute(TaskContext context)
        {
            return Execute((OrderTaskContext) context);
        }

        /// <summary>Rolls back this order task using an order-specific context.</summary>
        /// <param name="context">The context to cast to <see cref="OrderTaskContext"/>.</param>
        /// <returns>The result returned by <see cref="Rollback(OrderTaskContext)"/>.</returns>
        public override bool Rollback(TaskContext context)
        {
            return Rollback((OrderTaskContext) context);
        }

        /// <summary>Gets a context-sensitive task name.</summary>
        /// <param name="context">The current order workflow context.</param>
        /// <returns>By default, the value returned by <see cref="Task.TaskName()"/>.</returns>
        public virtual string TaskName(OrderTaskContext context)
        {
            return TaskName();
        }

        /// <summary>Executes this task for an order.</summary>
        /// <param name="context">The order, request, inputs, and messages shared by the workflow.</param>
        /// <returns><c>true</c> to continue; otherwise, <c>false</c> to trigger rollback.</returns>
        public abstract bool Execute(OrderTaskContext context);

        /// <summary>Rolls back this task's order changes.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns><c>true</c> when rollback succeeds; otherwise, <c>false</c>.</returns>
        public abstract bool Rollback(OrderTaskContext context);

        /// <summary>Adds a non-customer-visible error and a private exception note to the order.</summary>
        /// <param name="context">The order workflow context receiving the error and note.</param>
        /// <param name="ex">The exception being recorded.</param>
        /// <param name="errorMessage">The error name or message stored in the workflow context.</param>
        /// <remarks>The exception details are recorded in both the workflow error and a private order note.</remarks>
        protected void AddExceptionNote(OrderTaskContext context, Exception ex, string errorMessage)
        {
            context.Errors.Add(new WorkflowMessage(errorMessage, ex.Message + ex.StackTrace, false));
            var note = new OrderNote
            {
                IsPublic = false,
                Note = string.Concat("EXCEPTION: ", ex.Message, " | ", ex.StackTrace)
            };
            context.Order.Notes.Add(note);
        }

        /// <summary>Gets a context-sensitive workflow step name.</summary>
        /// <param name="context">The current order workflow context.</param>
        /// <returns>By default, the value returned by <see cref="Task.StepName()"/>.</returns>
        public virtual string StepName(OrderTaskContext context)
        {
            return StepName();
        }
    }
}
