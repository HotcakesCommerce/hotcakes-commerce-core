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
    /// <summary>Adds a private <see cref="OrderNote"/> to the current order during workflow execution.</summary>
    /// <remarks>This is an <see cref="OrderTask"/> executed with an <see cref="OrderTaskContext"/>.</remarks>
    /// <remarks>The note text is supplied when the task is constructed and is not exposed as a public property.</remarks>
    public class WorkflowNote : OrderTask
    {
        private readonly string Note = string.Empty;

        /// <summary>Initializes a workflow-note task with an empty note.</summary>
        public WorkflowNote()
        {
        }

        /// <summary>Initializes a workflow-note task with the specified note text.</summary>
        /// <param name="note">The text to add as a private order note when the task executes.</param>
        public WorkflowNote(string note)
        {
            Note = note;
        }

        /// <summary>Adds the configured text as a non-public note to the current order.</summary>
        /// <param name="context">The order workflow context containing the order to modify.</param>
        /// <returns>Always <see langword="true"/> after adding the note to the order's in-memory notes.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            var note = new OrderNote
            {
                IsPublic = false,
                Note = Note
            };
            context.Order.Notes.Add(note);
            return true;
        }

        /// <summary>Reports that the added order note has no local rollback implementation.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>Always <see langword="true"/>; the note is not removed here.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>Gets the display name of this workflow task.</summary>
        /// <returns><c>Workflow Note</c>.</returns>
        public override string TaskName()
        {
            return "Workflow Note";
        }

        /// <summary>Gets the stable workflow identifier for this task.</summary>
        /// <returns>The task ID <c>EFF99A7E-5A2B-4216-8A49-13D70A78AAB2</c>.</returns>
        public override string TaskId()
        {
            return "EFF99A7E-5A2B-4216-8A49-13D70A78AAB2";
        }

        /// <summary>Creates an independent workflow-note task with its default empty note.</summary>
        /// <returns>A new <see cref="WorkflowNote"/> instance.</returns>
        public override Task Clone()
        {
            return new WorkflowNote();
        }
    }
}
