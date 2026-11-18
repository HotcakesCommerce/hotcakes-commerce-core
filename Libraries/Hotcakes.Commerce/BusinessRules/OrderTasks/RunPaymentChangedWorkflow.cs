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
    /// Runs the workflow registered under <see cref="WorkflowNames.PaymentChanged"/>.
    /// </summary>
    /// <remarks>
    /// The task identifier is <c>00918101-ED47-4A9D-B9E3-567A78E49FA1</c>. The child workflow's result is returned
    /// unchanged. Rollback does not reverse work performed by that workflow.
    /// </remarks>
    public class RunPaymentChangedWorkflow : OrderTask
    {
        /// <summary>
        /// Executes the payment-changed workflow for the supplied context.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> passed to the child workflow.</param>
        /// <returns>The result returned by the payment-changed <see cref="Workflow"/>.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            return Workflow.RunByName(context, WorkflowNames.PaymentChanged);
        }

        /// <summary>
        /// Does not reverse work performed by the payment-changed workflow.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Run PaymentChanged Workflow</c>.</returns>
        public override string TaskName()
        {
            return "Run PaymentChanged Workflow";
        }

        /// <summary>
        /// Gets the stable task identifier used to register this task in workflows.
        /// </summary>
        /// <returns><c>00918101-ED47-4A9D-B9E3-567A78E49FA1</c>.</returns>
        public override string TaskId()
        {
            return "00918101-ED47-4A9D-B9E3-567A78E49FA1";
        }

        /// <summary>
        /// Creates a new payment-changed workflow task.
        /// </summary>
        /// <returns>A new <see cref="RunPaymentChangedWorkflow"/> instance.</returns>
        public override Task Clone()
        {
            return new RunPaymentChangedWorkflow();
        }
    }
}
