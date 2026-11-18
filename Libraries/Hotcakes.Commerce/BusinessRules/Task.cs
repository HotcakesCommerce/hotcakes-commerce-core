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

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>
    /// Defines one executable step in a <see cref="Workflow"/>.
    /// </summary>
    /// <remarks>
    /// Tasks execute in workflow order. A task returning <c>false</c> stops the workflow and causes the
    /// workflow to invoke <see cref="Rollback(TaskContext)"/> for the executed range in reverse order.
    /// </remarks>
    public abstract class Task
    {
        /// <summary>Gets the human-readable name of this task.</summary>
        /// <returns>The task name used by administrative and diagnostic callers.</returns>
        public abstract string TaskName();

        /// <summary>Gets the stable identifier used to identify this task.</summary>
        /// <returns>The task identifier.</returns>
        public abstract string TaskId();

        /// <summary>Executes the task against the supplied workflow context.</summary>
        /// <param name="context">The context shared by the workflow's tasks.</param>
        /// <returns><c>true</c> when processing may continue; otherwise, <c>false</c>.</returns>
        public abstract bool Execute(TaskContext context);

        /// <summary>Reverses this task's work when a workflow step fails.</summary>
        /// <param name="context">The context shared by the workflow's tasks.</param>
        /// <returns><c>true</c> when rollback succeeds; otherwise, <c>false</c>.</returns>
        /// <remarks>Rollback is called in reverse workflow order and may be called for the failing task.</remarks>
        public abstract bool Rollback(TaskContext context);

        /// <summary>Gets the display name for this workflow step.</summary>
        /// <returns>By default, the value returned by <see cref="TaskName()"/>.</returns>
        public virtual string StepName()
        {
            return TaskName();
        }

        /// <summary>Creates an independent task instance with the same task definition.</summary>
        /// <returns>A cloned task instance.</returns>
        public abstract Task Clone();
    }
}
