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

using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// Issues rewards points for the eligible portion of a paid order.
    /// </summary>
    /// <remarks>
    /// The task identifier is <c>21850F22-2A10-4CFA-BFF9-813CF448E07D</c>. The task skips orders already marked
    /// with <c>HccRewardsPointsIssued</c>, stores with rewards disabled, and orders without a user identifier.
    /// Successful issuance sets that marker and persists the order. Rollback does not revoke points.
    /// </remarks>
    public class IssueRewardsPoints : OrderTask
    {
        /// <summary>
        /// Creates a new rewards-points issuance task.
        /// </summary>
        /// <returns>A new <see cref="IssueRewardsPoints"/> instance.</returns>
        public override Task Clone()
        {
            return new IssueRewardsPoints();
        }

        /// <summary>
        /// Calculates and issues eligible rewards points, then records that issuance on the order.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> containing the order, store, and customer context.</param>
        /// <returns>Always <see langword="true"/> when processing reaches this task.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            var order = context.Order;
            if (order != null)
            {
                var hccApp = context.HccApp;
                var store = hccApp.CurrentStore;

                if (order.CustomProperties.HccRewardsPointsIssued ||
                    !store.Settings.RewardsPointsEnabled ||
                    string.IsNullOrEmpty(context.UserId)) // skip if there is no user account
                {
                    return true;
                }

                var pointsAmount = GetPointsAmount(order, hccApp);
                var orderTotal = GetOrderTotal(order, store);

                if (orderTotal > pointsAmount)
                {
                    var pointsManager = new CustomerPointsManager(hccApp.CurrentRequestContext);

                    var pointsToIssue = pointsManager.PointsToIssueForSpend(orderTotal - pointsAmount);
                    pointsManager.IssuePoints(order.UserID, pointsToIssue);

                    order.CustomProperties.HccRewardsPointsIssued = true;
                    hccApp.OrderServices.Orders.Update(order);
                }
            }

            return true;
        }

        /// <summary>
        /// Does not reverse points issued by <see cref="Execute(OrderTaskContext)"/>.
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
        /// <returns><c>21850F22-2A10-4CFA-BFF9-813CF448E07D</c>.</returns>
        public override string TaskId()
        {
            return "21850F22-2A10-4CFA-BFF9-813CF448E07D";
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Issue Rewards Points</c>.</returns>
        public override string TaskName()
        {
            return "Issue Rewards Points";
        }

        #region Implementation

        private decimal GetOrderTotal(Order order, Store store)
        {
            decimal total = 0;

            if (store.Settings.IssuePointsForUserPrice)
            {
                total = order.TotalOrderAfterDiscounts;
            }
            else
            {
                total = order.TotalOrderWithoutUserPricedProducts;
            }
            return total;
        }

        private decimal GetPointsAmount(Order order, HotcakesApplication hccApp)
        {
            decimal amount = 0;
            foreach (var t in hccApp.OrderServices.Transactions.FindForOrder(order.bvin))
            {
                if (t.Action == ActionType.RewardPointsInfo)
                {
                    amount += t.Amount;
                    break;
                }
            }
            return amount;
        }

        #endregion
    }
}
