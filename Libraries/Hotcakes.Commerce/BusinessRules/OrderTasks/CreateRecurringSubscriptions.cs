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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class CreateRecurringSubscriptions : OrderTask
    {
        #region Fields

        private HotcakesApplication _app;
        private OrderPaymentManager _payManager;

        #endregion

        #region Overrides

        public override bool Execute(OrderTaskContext context)
        {
            var result = true;

            if (context.Order.IsRecurring)
            {
                _app = context.HccApp;
                _payManager = new OrderPaymentManager(context.Order, _app);
                var infoList = _payManager.RecurringSubscriptionInfoListAll();
                var transactions = _app.OrderServices.Transactions.FindForOrder(context.Order.bvin);

                foreach (var info in infoList)
                {
                    info.CreditCard.SecurityCode = context.Inputs.GetProperty("hcc", "CardSecurityCode");

                    foreach (var li in context.Order.Items)
                    {
                        if (HasSuccessfulLinkedAction(transactions, info, li.Id))
                        {
                            var note = new OrderNote("Skipping creation of subscription. Transaction " + info.Id);
                            context.Order.Notes.Add(note);
                        }
                        else
                        {
                            result &= ProcessTransaction(context, info, li);
                        }
                    }
                }
            }

            return result;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskName()
        {
            return "Create Recurring Subscriptions";
        }

        public override string TaskId()
        {
            return "9E4EF40C-29DD-451F-8F79-F0526273B4EA";
        }

        public override Task Clone()
        {
            return new CreateRecurringSubscriptions();
        }

        #endregion

        #region Implementation

        private bool HasSuccessfulLinkedAction(List<OrderTransaction> transactions, OrderTransaction info,
            long lineItemId)
        {
            return transactions.Any(t => t.Success && !t.Voided
                                         && t.Action == ActionType.RecurringSubscriptionCreate
                                         && t.LinkedToTransaction == info.IdAsString
                                         && t.RefNum2 == lineItemId.ToString());
        }

        private bool ProcessTransaction(OrderTaskContext context, OrderTransaction info, LineItem li)
        {
            var result = true;

            try
            {
                var tResult = _payManager.RecurringSubscriptionCreate(info, li);

                if (!tResult.Succeeded)
                {
                    AddTransactionErrors(context, tResult);
                    result = false;
                }
            }
            catch (Exception ex)
            {
                AddExceptionNote(context, ex, "Exception During Create Recurring Subscription");
            }

            return result;
        }

        private void AddTransactionErrors(OrderTaskContext context, ResultData result)
        {
            foreach (var m in result.Messages)
            {
                if (m.Severity == MessageType.Error || m.Severity == MessageType.Warning)
                {
                    context.Errors.Add(new WorkflowMessage("Payment Error:", m.Description, true));
                }
            }
        }

        #endregion
    }
}