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

using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class WorkflowNote : OrderTask
    {
        private readonly string Note = string.Empty;

        public WorkflowNote()
        {
        }

        public WorkflowNote(string note)
        {
            Note = note;
        }

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

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskName()
        {
            return "Workflow Note";
        }

        public override string TaskId()
        {
            return "EFF99A7E-5A2B-4216-8A49-13D70A78AAB2";
        }

        public override Task Clone()
        {
            return new WorkflowNote();
        }
    }
}