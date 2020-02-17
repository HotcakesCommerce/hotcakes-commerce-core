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
    public class CheckForZeroDollarOrders : OrderTask
    {
        public override Task Clone()
        {
            return new TestCreateErrors();
        }

        public override bool Execute(OrderTaskContext context)
        {
            var allowed = context.HccApp.CurrentStore.Settings.AllowZeroDollarOrders;

            if (!allowed)
            {
                if (context.Order.TotalGrand <= 0)
                {
                    var errorMessage = new WorkflowMessage("Error", "Zero dollar orders are not allowed on this store.",
                        true);
                    context.Errors.Add(errorMessage);
                    return false;
                }
            }

            if (context.Order.Items.Count < 1)
            {
                var error2 = new WorkflowMessage("Error",
                    "The system was unable to process your order and may be busy. Please try again.", true);
                context.Errors.Add(error2);
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
            return "A2D0FC29-CC3C-4f6b-ABC8-163D9543A1A8";
        }

        public override string TaskName()
        {
            return "Check for Zero Dollar Orders";
        }
    }
}