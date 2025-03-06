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
    public class RunShippingCompleteWorkFlow : OrderTask
    {
        public override Task Clone()
        {
            return new RunShippingCompleteWorkFlow();
        }

        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.ShippingStatus == OrderShippingStatus.FullyShipped)
            {
                return Workflow.RunByName(context, WorkflowNames.ShippingComplete);
            }
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "5ba90d63-ff6c-4ca4-8e34-7705ec3b2728";
        }

        public override string TaskName()
        {
            return "Run Shipping Complete Workflow If Needed";
        }
    }
}