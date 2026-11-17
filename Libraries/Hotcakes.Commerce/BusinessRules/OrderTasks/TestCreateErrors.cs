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
    /// <summary>Test <see cref="OrderTask"/> that appends six customer-visible workflow errors to the context.</summary>
    /// <remarks>This task is intended to exercise error rendering and always fails execution; rollback remains a no-op.</remarks>
    public class TestCreateErrors : OrderTask
    {
        /// <summary>Creates an independent error-generation task instance.</summary>
        /// <returns>A new <see cref="TestCreateErrors"/> instance.</returns>
        public override Task Clone()
        {
            return new TestCreateErrors();
        }

        /// <summary>Adds error messages numbered 0 through 5 to the workflow context.</summary>
        /// <param name="context">The order workflow context receiving the <see cref="WorkflowMessage"/> instances.</param>
        /// <returns>Always <see langword="false"/> after adding all six errors.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            for (var i = 0; i <= 5; i++)
            {
                var errorMessage = new WorkflowMessage("Error", "This error is error number " + i, true);
                context.Errors.Add(errorMessage);
            }
            return false;
        }

        /// <summary>Reports that generated workflow errors have no rollback operation.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>Gets the stable workflow identifier for this task.</summary>
        /// <returns>The task ID <c>a8f7e43f-6fea-4a02-a4aa-74af6ef6320c</c>.</returns>
        public override string TaskId()
        {
            return "a8f7e43f-6fea-4a02-a4aa-74af6ef6320c";
        }

        /// <summary>Gets the display name of this workflow task.</summary>
        /// <returns><c>Test Create Errors</c>.</returns>
        public override string TaskName()
        {
            return "Test Create Errors";
        }
    }
}
