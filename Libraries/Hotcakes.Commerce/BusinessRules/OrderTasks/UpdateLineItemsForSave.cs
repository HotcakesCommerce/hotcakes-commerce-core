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
    public class UpdateLineItemsForSave : OrderTask
    {
        public override bool Execute(OrderTaskContext context)
        {
            context.Order.EvaluateCurrentShippingStatus();
            context.HccApp.OrderServices.Orders.Update(context.Order);
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "6E27D187-F367-40f8-8231-160B6E22AB86";
        }

        public override string TaskName()
        {
            return "Update Line Items for Save";
        }

        public override string StepName()
        {
            var result = string.Empty;
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        public override Task Clone()
        {
            return new UpdateLineItemsForSave();
        }
    }
}