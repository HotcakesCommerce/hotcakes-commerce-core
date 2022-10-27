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

using System;
using System.Collections.Generic;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;
using System.Linq;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{

	public class ReceivePaypalExpressPayments : OrderTask
	{

		public override Task Clone()
		{
			return new ReceivePaypalExpressPayments();
		}

		public override bool Execute(OrderTaskContext context)
		{
			bool result = true;

            if (context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth > 0)
            {

				//Use the last transaction entered by customer first
                List<OrderTransaction> transactions = context.HccApp.OrderServices.Transactions
																	.FindForOrder(context.Order.bvin)
																	.OrderByDescending(x => x.TimeStampUtc)
																	.ToList();

				decimal dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;

				foreach (OrderTransaction p in transactions)
                {
                    if (p.Action == ActionType.PayPalExpressCheckoutInfo)
                    {
                        // if we already have an auth or charge on the card, skip
                        if (p.HasSuccessfulLinkedAction(ActionType.PayPalHold, transactions) ||
                            p.HasSuccessfulLinkedAction(ActionType.PayPalCharge, transactions))
                        {
                            continue;
                        }

                        try
                        {
							OrderPaymentManager payManager = new OrderPaymentManager(context.Order, context.HccApp);
							decimal amount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;
							result = context.HccApp.CurrentRequestContext.CurrentStore.Settings.PayPal.ExpressAuthorizeOnly ? payManager.PayPalExpressHold(p, amount) : payManager.PayPalExpressCharge(p, amount);

							if (result == true)
							{
								dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;
								//Amount required in order is already charged. No need to charge on other transactions
								if (dueAmount <= 0)
								{
									break;
								}
							}
                        }
                        catch (Exception ex)
                        {
                            context.Errors.Add(new WorkflowMessage("Exception During Receive Paypal Express Payments", ex.Message + ex.StackTrace, false));
                        }
                    }
                }
			}

			if (result == false)
			{
				var errorString = "An error occurred while attempting to process your Paypal Express payment. Please check your payment information and try again";
				context.Errors.Add(new WorkflowMessage("Receive Card Failed", errorString, true));

				// Failure Status Code
				var failCode = OrderStatusCode.OnHold;
				var c = OrderStatusCode.FindByBvin(failCode);
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

		public override string TaskId()
		{
			return "37e0b27e-567f-4ee3-95ec-c7e5a66bfb26";
		}

		public override string TaskName()
		{
			return "Receive Paypal Express Payments";
		}

	}
}
