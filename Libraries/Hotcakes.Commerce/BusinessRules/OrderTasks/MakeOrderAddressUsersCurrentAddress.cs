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

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class MakeOrderAddressUsersCurrentAddress : OrderTask
    {
        private const string TASK_NAME = "Make Order Address User's Current Address";

        public override bool Execute(OrderTaskContext context)
        {
            if (context.UserId != string.Empty)
            {
                // see if the customer already exists
                var user = context.HccApp.MembershipServices.Customers.Find(context.UserId);

                // the user doesn't exist yet, so exit the workflow task
                if (user == null) return true;

                // copy the shipping address from the order to the user
                context.Order.ShippingAddress.CopyTo(user.ShippingAddress);

                // copy the billing address if it's different than shipping
                if (!context.Order.BillingAddress.IsEqualTo(context.Order.ShippingAddress))
                {
                    context.Order.BillingAddress.CopyTo(user.BillingAddress);
                }

                // save our changes
                context.HccApp.MembershipServices.UpdateCustomer(user);
            }

            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "7cf8a5b6-3999-41b0-b283-e8280d19f3bb";
        }

        public override string TaskName()
        {
            return TASK_NAME;
        }

        public override string StepName()
        {
            var result = string.Empty;
            result = TASK_NAME;
            return result;
        }

        public override Task Clone()
        {
            return new MakeOrderAddressUsersCurrentAddress();
        }
    }
}