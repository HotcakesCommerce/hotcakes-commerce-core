#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Orders;
using System.Linq;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
	public class AuthorizeGiftCards : OrderTask
    {
        public override Task Clone()
        {
 	        return new AuthorizeGiftCards();
        }

        public override string TaskId()
		{
			return "0D569BCD-224B-44ab-9BF5-D87E090D2A1A";
		}

		public override string TaskName()
		{
			return "Authorize Gift Certificates";
		}
      
        public override bool Execute(OrderTaskContext context)
        {
            var result = true;
			if (context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth > 0)
            {
				List<OrderTransaction> transactions = context.HccApp.OrderServices.Transactions
														.FindForOrder(context.Order.bvin)
														.OrderByDescending(x => x.TimeStampUtc)
														.ToList();

				decimal dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;

				foreach (OrderTransaction p in transactions)
                {
                    if (p.Action == ActionType.GiftCardInfo)
                    {
                        // if we already have an auth or charge on the card, skip
                        if (p.HasSuccessfulLinkedAction(ActionType.GiftCardDecrease, transactions) ||
                            p.HasSuccessfulLinkedAction(ActionType.GiftCardCapture, transactions) ||
                            p.HasSuccessfulLinkedAction(ActionType.GiftCardHold, transactions))
                        {
                            var note = new OrderNote();
                            note.IsPublic = false;
                            note.Note = "Skipping receive for gift card info because auth or charge already exists. Transaction " + p.Id;
                            context.Order.Notes.Add(note);
                            continue;
                        }

                        try
                        {
							var payManager = new OrderPaymentManager(context.Order, context.HccApp);
							var storeSettings = context.HccApp.CurrentStore.Settings;
							Transaction t = payManager.CreateEmptyTransaction();
                            t.GiftCard.CardNumber = p.GiftCard.CardNumber;
                            t.Amount = p.Amount;

							GiftCardGateway proc = storeSettings.PaymentCurrentGiftCardProcessor();

							if (storeSettings.PaymentGiftCardAuthorizeOnly && proc.CanAuthorize)
                            {
                                t.Action = ActionType.GiftCardHold;
                            }
                            else
                            {
                                t.Action = ActionType.GiftCardDecrease;
                            }

                            proc.ProcessTransaction(t);

                            OrderTransaction ot = new OrderTransaction(t);
                            ot.LinkedToTransaction = p.IdAsString;
                            context.HccApp.OrderServices.AddPaymentTransactionToOrder(context.Order, ot);

							if (t.Result.Succeeded == false) 
							{ 
								result = false; 
							}

                        }
                        catch (Exception ex)
                        {
                            context.Errors.Add(new WorkflowMessage("Exception During Receive Gift Card", ex.Message + ex.StackTrace, false));
                            OrderNote note = new OrderNote();
                            note.IsPublic = false;
                            note.Note = "EXCEPTION: " + ex.Message + " | " + ex.StackTrace;
                            context.Order.Notes.Add(note);
                        }

						dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;
						//Amount required in order is already charged. No need to charge on other transactions
						if (dueAmount <= 0)
						{
							break;
						}

                    }
                }
            }
            else
            {
                var note = new OrderNote();
                note.IsPublic = false;
                note.Note = "Amount due was less than zero. Skipping receive gift cards";
                context.Order.Notes.Add(note);
            }

            if (!result)
            {
                string errorString = "An error occurred while attempting to process your gift card. Please check your payment information and try again";
                context.Errors.Add(new WorkflowMessage("Receive Gift Card Failed", errorString, true));

                // Failure Status Code
                string failCode = OrderStatusCode.OnHold;
                OrderStatusCode c = OrderStatusCode.FindByBvin(failCode);
                if (c != null)
                {
                    context.Order.StatusCode = c.Bvin;
                    context.Order.StatusName = c.StatusName;
                }
            }
            return result;
        }

        public override bool Rollback(OrderTaskContext context)
        {
 	        return true;
        }
    }
}
