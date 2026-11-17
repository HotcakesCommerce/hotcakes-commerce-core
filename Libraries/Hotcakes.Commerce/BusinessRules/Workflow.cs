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
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Executes an ordered collection of business-rule tasks.</summary>
    /// <remarks>Execution stops at the first failure or non-thread-abort exception. The failing step and all prior steps are rolled back in reverse order.</remarks>
    public class Workflow
    {
        private readonly List<Task> _Tasks = new List<Task>();

        /// <summary>Initializes a workflow from an ordered task array.</summary>
        /// <param name="tasks">The tasks to execute in array order; <c>null</c> creates an empty workflow.</param>
        public Workflow(Task[] tasks)
        {
            if (tasks != null)
            {
                _Tasks = tasks.ToList();
            }
        }

        /// <summary>Runs each task in sequence using the supplied context.</summary>
        /// <param name="c">The context shared by all tasks and rollback operations.</param>
        /// <returns><c>true</c> when every task succeeds; otherwise, <c>false</c>.</returns>
        /// <remarks>Non-<see cref="ThreadAbortException"/> exceptions become non-customer-visible <see cref="WorkflowMessage"/> errors and are logged before rollback.</remarks>
        public bool Run(TaskContext c)
        {
            var result = true;

            for (var i = 0; i <= _Tasks.Count - 1; i++)
            {
                var taskResult = false;
                try
                {
                    taskResult = _Tasks[i].Execute(c);
                }
                catch (Exception ex)
                {
                    if (!(ex is ThreadAbortException))
                    {
                        taskResult = false;
                        c.Errors.Add(new WorkflowMessage("EXCEPTION", ex.Message, false));
                        EventLog.LogEvent(ex);
                    }
                }
                if (taskResult == false)
                {
                    result = false;
                    Rollback(i, c);
                    return false;
                }
            }

            return result;
        }

        /// <summary>Creates and runs the workflow identified by <paramref name="name"/>.</summary>
        /// <param name="c">The order context used to resolve the application and run the workflow.</param>
        /// <param name="name">The workflow identifier to create through the context's factory.</param>
        /// <returns><c>true</c> when the selected workflow completes; otherwise, <c>false</c>.</returns>
        public static bool RunByName(OrderTaskContext c, WorkflowNames name)
        {
            var result = false;
            var wf = c.HccApp.WorkflowFactory.CreateWorkflow(name);
            result = wf.Run(c);
            return result;
        }

        private bool Rollback(int startFromStepIndex, TaskContext c)
        {
            var result = true;

            for (var i = startFromStepIndex; i >= 0; i += -1)
            {
                if (_Tasks[i].Rollback(c) == false)
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
