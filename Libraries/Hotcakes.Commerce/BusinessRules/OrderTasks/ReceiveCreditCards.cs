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

using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;
using System;
using System.Collections.Generic;
using Hotcakes.Commerce.Payment;
using System.Linq;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{

	public class ReceiveCreditCards : OrderTask
	{
		public override bool Execute(OrderTaskContext context)
		{
			bool result = true;
			if (context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth > 0)
			{
				//Use the last transaction entered by customer first
				List<OrderTransaction> transactions = context.HccApp.OrderServices.Transactions
																.FindForOrder(context.Order.bvin)
																.OrderByDescending( x => x.TimeStampUtc)
																.ToList();

				decimal dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;

				foreach (OrderTransaction p in transactions)
				{
					if (p.Action == ActionType.CreditCardInfo)
					{
						// if we already have an auth or charge on the card, skip
						if (p.HasSuccessfulLinkedAction(ActionType.CreditCardCharge, transactions) ||
							p.HasSuccessfulLinkedAction(ActionType.CreditCardHold, transactions))
						{
							OrderNote note = new OrderNote();
							note.IsPublic = false;
							note.Note = "Skipping receive for credit card info because auth or charge already exists. Transaction " + p.Id;
							context.Order.Notes.Add(note);
							continue;
						}
						result &= ProcessTransaction(context, p);

						if (result == true)
						{
							dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;
							//Due amount is already charged, no need to charge other cards
							if (dueAmount <= 0)
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				OrderNote note = new OrderNote();
				note.IsPublic = false;
				note.Note = "Amount due was less than zero. Skipping receive credit cards";
				context.Order.Notes.Add(note);
			}

			if (!result)
			{
				string errorString = "An error occurred while attempting to process your credit card. Please check your payment information and try again";
				context.Errors.Add(new WorkflowMessage("Receive Card Failed", errorString, true));

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

		private bool ProcessTransaction(OrderTaskContext context, OrderTransaction p)
		{
			bool result = true;

			try
			{
				var payManager = new OrderPaymentManager(context.Order, context.HccApp);
				Transaction t = payManager.CreateEmptyTransaction();
				t.Card = p.CreditCard;
				t.Card.SecurityCode = context.Inputs.GetProperty("hcc", "CardSecurityCode");
				t.Amount = p.Amount;

				if (context.HccApp.CurrentStore.Settings.PaymentCreditCardAuthorizeOnly)
				{
					t.Action = ActionType.CreditCardHold;
				}
				else
				{
					t.Action = ActionType.CreditCardCharge;
				}

				PaymentGateway proc = PaymentGateways.CurrentPaymentProcessor(context.HccApp.CurrentStore);
				proc.ProcessTransaction(t);

				OrderTransaction ot = new OrderTransaction(t);
				ot.LinkedToTransaction = p.IdAsString;
				context.HccApp.OrderServices.AddPaymentTransactionToOrder(context.Order, ot);

				if (!t.Result.Succeeded || t.Action == ActionType.CreditCardIgnored)
				{
					foreach (var m in t.Result.Messages)
					{
						if (m.Severity == MessageType.Error ||
							m.Severity == MessageType.Warning)
						{
							context.Errors.Add(new WorkflowMessage("Payment Error:", m.Description, true));
						}
					}
					result = false;
				}

			}
			catch (Exception ex)
			{
				context.Errors.Add(new WorkflowMessage("Exception During Receive Credit Card", ex.Message + ex.StackTrace, false));
				OrderNote note = new OrderNote();
				note.IsPublic = false;
				note.Note = "EXCEPTION: " + ex.Message + " | " + ex.StackTrace;
				context.Order.Notes.Add(note);
			}

			return result;
		}

		public override bool Rollback(OrderTaskContext context)
		{
			return true;
		}

		public override string TaskId()
		{
			return "253305A6-87A4-4bcc-AA98-8A082E2D8162";
		}

		public override string TaskName()
		{
			return "Receive Credit Cards";
		}
		public override Task Clone()
		{
			return new ReceiveCreditCards();
		}

	}
}

