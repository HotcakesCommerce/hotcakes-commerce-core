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

using System.Collections.ObjectModel;
using System.Linq;

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Contains the state shared by tasks while a <see cref="Workflow"/> runs.</summary>
    /// <remarks><see cref="Inputs"/>, <see cref="Errors"/>, and <see cref="Outputs"/> are workflow-scoped collections; callers must not assume they are persisted automatically.</remarks>
    public class TaskContext
    {
        /// <summary>Initializes an empty task context with empty input, error, and output collections.</summary>
        public TaskContext()
        {
            UserId = string.Empty;

            Inputs = new CustomPropertyCollection();
            Errors = new Collection<WorkflowMessage>();
            Outputs = new Collection<WorkflowMessage>();
        }

        /// <summary>Gets or sets the identifier of the user associated with the workflow.</summary>
        /// <value>An empty string when no user is associated with the context.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets task input values shared through the workflow.</summary>
        /// <value>The custom property collection for workflow inputs.</value>
        public CustomPropertyCollection Inputs { get; set; }
        /// <summary>Gets or sets all messages produced while processing the workflow.</summary>
        /// <value>The workflow error collection, including messages not intended for customers.</value>
        public Collection<WorkflowMessage> Errors { get; set; }
        /// <summary>Gets or sets messages produced as workflow output.</summary>
        /// <value>The workflow output collection.</value>
        public Collection<WorkflowMessage> Outputs { get; set; }

        /// <summary>Returns a new collection containing only errors marked for customer display.</summary>
        /// <returns>A snapshot of <see cref="Errors"/> entries whose <see cref="WorkflowMessage.CustomerVisible"/> value is <c>true</c>.</returns>
        /// <remarks>Changing the returned collection does not remove messages from <see cref="Errors"/>.</remarks>
        public Collection<WorkflowMessage> GetCustomerVisibleErrors()
        {
            var list = Errors.Where(e => e.CustomerVisible).ToList();
            return new Collection<WorkflowMessage>(list);
        }
    }
}
