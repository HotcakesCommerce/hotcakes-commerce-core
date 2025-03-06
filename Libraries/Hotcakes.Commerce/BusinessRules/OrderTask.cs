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

using System;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules
{
    public abstract class OrderTask : Task
    {
        public override bool Execute(TaskContext context)
        {
            return Execute((OrderTaskContext) context);
        }

        public override bool Rollback(TaskContext context)
        {
            return Rollback((OrderTaskContext) context);
        }

        public virtual string TaskName(OrderTaskContext context)
        {
            return TaskName();
        }

        public abstract bool Execute(OrderTaskContext context);

        public abstract bool Rollback(OrderTaskContext context);

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

        public virtual string StepName(OrderTaskContext context)
        {
            return StepName();
        }
    }
}