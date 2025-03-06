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
using Hotcakes.Commerce.Contacts;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class AddUserAddressesToAddressBook : OrderTask
    {
        private const string TASK_NAME = "Add current order addresses to user's address book";

        public override bool Execute(OrderTaskContext context)
        {
            if (context.UserId != string.Empty)
            {
                var user = context.HccApp.MembershipServices.Customers.Find(context.UserId);

                if (user == null)
                {
                    return true;
                }

                if (user.Addresses != null && user.Addresses.Count > 0)
                {
                    // check to see if the billing address exists yet
                    if (!user.Addresses.Contains(context.Order.BillingAddress))
                    {
                        context.Order.BillingAddress = EnsureGuid(context.Order.BillingAddress);
                        user.Addresses.Add(context.Order.BillingAddress);
                    }

                    // check to see if the shipping address exists yet
                    if (IsAddressValid(context.Order.ShippingAddress) &&
                        !user.Addresses.Contains(context.Order.ShippingAddress))
                    {
                        context.Order.ShippingAddress = EnsureGuid(context.Order.ShippingAddress);
                        user.Addresses.Add(context.Order.ShippingAddress);
                    }
                }
                else
                {
                    if (user.Addresses == null)
                    {
                        user.Addresses = new AddressList();
                    }

                    // add the addresses because there aren't any in the address book yet
                    context.Order.BillingAddress = EnsureGuid(context.Order.BillingAddress);
                    user.Addresses.Add(context.Order.BillingAddress);

                    // only add the shipping address if it's different
                    if (IsAddressValid(context.Order.ShippingAddress) &&
                        !context.Order.BillingAddress.IsEqualTo(context.Order.ShippingAddress))
                    {
                        context.Order.ShippingAddress = EnsureGuid(context.Order.ShippingAddress);
                        user.Addresses.Add(context.Order.ShippingAddress);
                    }
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
            return "f50aaa74-97d7-4642-92e5-b79e31dccc3d";
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
            return new AddUserAddressesToAddressBook();
        }

        private bool IsAddressValid(Address address)
        {
            return !string.IsNullOrEmpty(address.Line1) && !string.IsNullOrEmpty(address.City);
        }

        private Address EnsureGuid(Address address)
        {
            if (address != null && string.IsNullOrEmpty(address.Bvin))
            {
                address.Bvin = Guid.NewGuid().ToString();
            }

            return address;
        }
    }
}