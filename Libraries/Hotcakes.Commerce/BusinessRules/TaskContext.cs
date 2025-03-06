#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    public class TaskContext
    {
        public TaskContext()
        {
            UserId = string.Empty;

            Inputs = new CustomPropertyCollection();
            Errors = new Collection<WorkflowMessage>();
            Outputs = new Collection<WorkflowMessage>();
        }

        public string UserId { get; set; }

        public CustomPropertyCollection Inputs { get; set; }
        public Collection<WorkflowMessage> Errors { get; set; }
        public Collection<WorkflowMessage> Outputs { get; set; }

        public Collection<WorkflowMessage> GetCustomerVisibleErrors()
        {
            var list = Errors.Where(e => e.CustomerVisible).ToList();
            return new Collection<WorkflowMessage>(list);
        }
    }
}