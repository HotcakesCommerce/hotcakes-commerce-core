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

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class ApplyMinimumOrderAmount : OrderTask
    {
        public override Task Clone()
        {
            return new ApplyMinimumOrderAmount();
        }

        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.TotalOrderBeforeDiscounts <
                context.HccApp.CurrentRequestContext.CurrentStore.Settings.MinumumOrderAmount)
            {
                context.Errors.Add(new WorkflowMessage("Minimum Order Amount",
                    context.HccApp.CurrentRequestContext.CurrentStore.Settings.MinumumOrderAmount.ToString("c"), true));
                return false;
            }
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "a07ed476-3165-4842-a4bf-ab40c8054501";
        }

        public override string TaskName()
        {
            return "Apply Minimum Order Amount";
        }
    }
}