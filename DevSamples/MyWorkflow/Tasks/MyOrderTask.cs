#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Orders;

namespace MyCompany.MyWorkflow.Tasks
{
    public class MyOrderTask : OrderTask
    {
        public override Task Clone()
        {
            return new MyOrderTask();
        }

        public override bool Execute(OrderTaskContext context)
        {
            try
            {
                context.Order.Notes.Add(new OrderNote
                {
                    IsPublic = false,
                    Note = "Hi " + context.Order.ShippingAddress.FirstName
                });
                context.Outputs.Add(new WorkflowMessage(
                    "Hi",
                    context.Order.ShippingAddress.FirstName,
                    true));

                // TODO : My custom logic
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool Rollback(TaskContext context)
        {
            return false;
        }

        public override string TaskId()
        {
            return "{0444BE26-992A-4877-9BB2-E681B1D50353}";
        }

        public override string TaskName()
        {
            return "My order task";
        }

        public override bool Rollback(OrderTaskContext context)
        {
            throw new NotImplementedException();
        }
    }
}