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
using System.Text;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class ReceiveRewardsPoints : OrderTask
    {

        public override Task Clone()
        {
            return new ReceiveRewardsPoints();
        }

        public override bool Execute(OrderTaskContext context)
        {
            bool result = true;
			if (context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth > 0)
            {
				CustomerPointsManager pointsManager = new CustomerPointsManager(context.HccApp.CurrentRequestContext);
                OrderPaymentManager payManager = new OrderPaymentManager(context.Order, context.HccApp);

				List<OrderTransaction> transactions = context.HccApp.OrderServices.Transactions
													.FindForOrder(context.Order.bvin)
													.OrderByDescending(x => x.TimeStampUtc)
													.ToList();

				decimal dueAmount = context.HccApp.OrderServices.PaymentSummary(context.Order).AmountDueWithAuth;

				foreach (OrderTransaction p in transactions)
                {
                    if (p.Action == ActionType.RewardPointsInfo)
                    {
                        // if we already have an auth or charge on the card, skip
                        if (p.HasSuccessfulLinkedAction(ActionType.RewardPointsDecrease, transactions) ||
                            p.HasSuccessfulLinkedAction(ActionType.RewardPointsHold, transactions))
                        {
                            continue;
                        }

                        try
                        {
							int points = pointsManager.PointsNeededForPurchaseAmount(p.Amount);
							if (context.HccApp.CurrentRequestContext.CurrentStore.Settings.PaymentCreditCardAuthorizeOnly)
							{
								payManager.RewardsPointsHold(p, points);
							}
							else
							{
								payManager.RewardsPointsCharge(p, points);
							}
                        }
                        catch (Exception ex)
                        {
                            context.Errors.Add(new WorkflowMessage("Exception During Receive Rewards Points", ex.Message + ex.StackTrace, false));
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
            
            return result;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "A912F623-9210-4DDB-B8E1-9E366E9520F9";
        }

        public override string TaskName()
        {
            return "Receive Rewards Points";
        }

    }
}
