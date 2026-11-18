#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>
    /// <summary>
    /// Represents the <see cref="OrderTask"/> that performs the AddUserAddressesToAddressBook workflow operation. The stable task identifier is <c>f50aaa74-97d7-4642-92e5-b79e31dccc3d</c>. Execution and rollback preserve the existing order workflow behavior, including any persistence, customer-visible <see cref="WorkflowMessage"/> errors, payment activity, email delivery, and rollback limitations.
    /// </summary>
    /// </summary>
    public class AddUserAddressesToAddressBook : OrderTask
    {
        private const string TASK_NAME = "Add current order addresses to user's address book";

        /// <summary>
        /// Executes this order workflow task using the supplied <see cref="OrderTaskContext"/>. The return value controls whether the workflow continues; order changes, persistence, messages, payments, or email follow the existing implementation.
        /// </summary>
        /// <param name="context">The value used by this member.</param>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
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

        /// <summary>
        /// Attempts to roll back this task using the supplied <see cref="OrderTaskContext"/>. A successful return does not imply that external persistence, payment processing, account creation, or email delivery was reversed.
        /// </summary>
        /// <param name="context">The value used by this member.</param>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets this task's stable workflow identifier.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskId()
        {
            return "f50aaa74-97d7-4642-92e5-b79e31dccc3d";
        }

        /// <summary>
        /// Gets the display name registered for this order task.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskName()
        {
            return TASK_NAME;
        }

        /// <summary>
        /// Gets the display name used for this workflow step.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string StepName()
        {
            var result = string.Empty;
            result = TASK_NAME;
            return result;
        }

        /// <summary>
        /// Creates a new task instance with the same task type and its documented constructor defaults.
        /// </summary>
        /// <returns>The value produced by this member.</returns>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
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
