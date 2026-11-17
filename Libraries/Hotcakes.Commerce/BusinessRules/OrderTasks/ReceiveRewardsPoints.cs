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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// Applies rewards points to the outstanding amount due on an order.
    /// </summary>
    /// <remarks>
    /// The task identifier is <c>A912F623-9210-4DDB-B8E1-9E366E9520F9</c>. It processes reward-point information
    /// transactions newest first, skips transactions with a successful hold or decrease, and uses the store's
    /// authorize-only setting to choose a hold or charge. Rollback does not restore spent or held points.
    /// </remarks>
    public class ReceiveRewardsPoints : OrderTask
    {

        /// <summary>
        /// Creates a new rewards-points receiving task.
        /// </summary>
        /// <returns>A new <see cref="ReceiveRewardsPoints"/> instance.</returns>
        public override Task Clone()
        {
            return new ReceiveRewardsPoints();
        }

        /// <summary>
        /// Applies reward points to outstanding order charges until the amount due is covered.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> containing the order and commerce services.</param>
        /// <returns>Always <see langword="true"/>; exceptions are added to the workflow errors.</returns>
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

        /// <summary>
        /// Does not reverse reward-point holds or charges created during execution.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the stable task identifier used to register this task in workflows.
        /// </summary>
        /// <returns><c>A912F623-9210-4DDB-B8E1-9E366E9520F9</c>.</returns>
        public override string TaskId()
        {
            return "A912F623-9210-4DDB-B8E1-9E366E9520F9";
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Receive Rewards Points</c>.</returns>
        public override string TaskName()
        {
            return "Receive Rewards Points";
        }

    }
}

