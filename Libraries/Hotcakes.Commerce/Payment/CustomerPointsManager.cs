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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Converts, reads, and mutates reward-point balances for the current store.
    /// Point mutations are recorded as ledger rows; this service does not rewrite prior rows.
    /// </summary>
    public class CustomerPointsManager : HccServiceBase
    {
        /// <summary>
        /// Initializes a manager for the supplied commerce request context.
        /// </summary>
        /// <param name="context">The request context whose current store scopes point operations.</param>
        public CustomerPointsManager(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Calculates the dollar credit represented by a number of points, rounded to two decimals.
        /// </summary>
        /// <param name="points">The number of points to convert.</param>
        /// <returns>The rounded dollar credit using the current store's points-per-dollar setting.</returns>
        public decimal DollarCreditForPoints(int points)
        {
            var storeSettings = Context.CurrentStore.Settings;
            var pointsNeededForOneDollarCredit = storeSettings.RewardsPointsNeededPerDollarCredit;

            decimal result = 0;
            result = points/(decimal) pointsNeededForOneDollarCredit;
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Calculates the points required for a purchase amount, rounding fractional points upward.
        /// </summary>
        /// <param name="purchaseAmount">The purchase amount in store currency.</param>
        /// <returns>The ceiling of the amount multiplied by the current store's points-needed setting.</returns>
        public int PointsNeededForPurchaseAmount(decimal purchaseAmount)
        {
            var storeSettings = Context.CurrentStore.Settings;
            var pointsNeededForOneDollarCredit = storeSettings.RewardsPointsNeededPerDollarCredit;

            var result = int.MaxValue;
            var r1 = purchaseAmount*pointsNeededForOneDollarCredit;
            result = (int) Math.Ceiling(r1);
            return result;
        }

        /// <summary>
        /// Calculates points issued for whole-dollar spend; fractional spend is discarded.
        /// </summary>
        /// <param name="spend">The amount spent.</param>
        /// <returns>The number of points calculated from the current store's issue rate.</returns>
        public int PointsToIssueForSpend(decimal spend)
        {
            var storeSettings = Context.CurrentStore.Settings;
            var pointsIssuedPerDollarSpent = storeSettings.RewardsPointsIssuedPerDollarSpent;

            var spendRounded = Math.Floor(spend);
            var points = spendRounded*pointsIssuedPerDollarSpent;
            return (int) points;
        }

        /// <summary>
        /// Sums all available points ledger entries for a store, including negative entries.
        /// </summary>
        /// <param name="storeId">The store identifier to query.</param>
        /// <returns>The net issued points, or zero when the store has no entries.</returns>
        public int TotalPointsIssuedForStore(long storeId)
        {
            using (var strategy = CreateReadStrategy())
            {
                var result = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .Sum(y => (int?) y.Points);
                return result ?? 0;
            }
        }

        /// <summary>
        /// Sums all held-point ledger entries for a store, including negative entries.
        /// </summary>
        /// <param name="storeId">The store identifier to query.</param>
        /// <returns>The net reserved points, or zero when the store has no entries.</returns>
        public int TotalPointsReservedForStore(long storeId)
        {
            using (var strategy = CreateReadStrategy())
            {
                var result = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .Sum(y => (int?) y.PointsHeld);
                return result ?? 0;
            }
        }

        /// <summary>
        /// Finds the current user's available points in the current store.
        /// </summary>
        /// <param name="userId">The user identifier to query.</param>
        /// <returns>The net available points, or zero when no entries exist.</returns>
        public int FindAvailablePoints(string userId)
        {
            using (var strategy = CreateReadStrategy())
            {
                var result = strategy.GetQuery().AsNoTracking().
                    Where(y => y.StoreId == Context.CurrentStore.Id).
                    Where(y => y.UserId == userId).
                    Sum(y => (int?) y.Points);
                return result ?? 0;
            }
        }

        /// <summary>
        /// Finds points held for a user in the current store.
        /// </summary>
        /// <param name="userId">The user identifier to query.</param>
        /// <returns>The net held points, or zero when no entries exist. The misspelled method name is retained for compatibility.</returns>
        public int FindReserverdPoints(string userId)
        {
            using (var strategy = CreateReadStrategy())
            {
                var result = strategy.GetQuery().AsNoTracking().
                    Where(y => y.StoreId == Context.CurrentStore.Id).
                    Where(y => y.UserId == userId).
                    Sum(y => (int?) y.PointsHeld);
                return result ?? 0;
            }
        }

        /// <summary>
        /// Captures previously held points by adding a negative held-point ledger entry.
        /// </summary>
        /// <param name="userId">The user whose held points are captured.</param>
        /// <param name="points">The number of held points to capture.</param>
        /// <returns><see langword="false"/> when insufficient points are held; otherwise the persistence result.</returns>
        public bool CapturePoints(string userId, int points)
        {
            var currentPoints = FindReserverdPoints(userId);
            if (currentPoints - points < 0)
                return false;

            using (var strategy = CreateStrategy())
            {
                var rewardPoint = new hcc_RewardsPoints
                {
                    PointsHeld = -1*points,
                    StoreId = Context.CurrentStore.Id,
                    UserId = userId,
                    TransactionTime = DateTime.UtcNow
                };
                strategy.Add(rewardPoint);
                return strategy.SubmitChanges();
            }
        }

        /// <summary>
        /// Decreases available points by appending a negative point ledger entry.
        /// </summary>
        /// <param name="userId">The user whose points are decreased.</param>
        /// <param name="points">The number of points to decrease.</param>
        /// <returns><see langword="false"/> when insufficient points are available; otherwise the persistence result.</returns>
        public bool DecreasePoints(string userId, int points)
        {
            var currentPoints = FindAvailablePoints(userId);
            if (currentPoints - points < 0)
                return false;

            using (var strategy = CreateStrategy())
            {
                var rewardPoint = new hcc_RewardsPoints
                {
                    Points = -1*points,
                    StoreId = Context.CurrentStore.Id,
                    UserId = userId,
                    TransactionTime = DateTime.UtcNow
                };
                strategy.Add(rewardPoint);
                return strategy.SubmitChanges();
            }
        }

        /// <summary>
        /// Moves available points into the held balance by appending one ledger entry.
        /// </summary>
        /// <param name="userId">The user whose points are held.</param>
        /// <param name="points">The number of points to hold.</param>
        /// <returns><see langword="false"/> when insufficient points are available; otherwise the persistence result.</returns>
        public bool HoldPoints(string userId, int points)
        {
            var currentPoints = FindAvailablePoints(userId);
            if (currentPoints - points < 0)
                return false;

            using (var strategy = CreateStrategy())
            {
                var rewardPoint = new hcc_RewardsPoints
                {
                    Points = -1*points,
                    PointsHeld = points,
                    StoreId = Context.CurrentStore.Id,
                    UserId = userId,
                    TransactionTime = DateTime.UtcNow
                };
                strategy.Add(rewardPoint);
                return strategy.SubmitChanges();
            }
        }

        /// <summary>
        /// Releases held points back to the available balance by appending one ledger entry.
        /// </summary>
        /// <param name="userId">The user whose points are released.</param>
        /// <param name="points">The number of points to release.</param>
        /// <returns>The persistence result.</returns>
        public bool UnHoldPoints(string userId, int points)
        {
            using (var strategy = CreateStrategy())
            {
                var rewardPoint = new hcc_RewardsPoints
                {
                    Points = points,
                    PointsHeld = -1*points,
                    StoreId = Context.CurrentStore.Id,
                    UserId = userId,
                    TransactionTime = DateTime.UtcNow
                };
                strategy.Add(rewardPoint);
                return strategy.SubmitChanges();
            }
        }

        /// <summary>
        /// Issues points by appending a positive available-point ledger entry.
        /// </summary>
        /// <param name="userId">The user receiving the points.</param>
        /// <param name="points">The number of points to issue.</param>
        /// <returns>The persistence result.</returns>
        public bool IssuePoints(string userId, int points)
        {
            using (var strategy = CreateStrategy())
            {
                var rewardPoint = new hcc_RewardsPoints
                {
                    Points = points,
                    StoreId = Context.CurrentStore.Id,
                    UserId = userId,
                    TransactionTime = DateTime.UtcNow
                };
                strategy.Add(rewardPoint);
                return strategy.SubmitChanges();
            }
        }

        internal void DestroyAllForStore(long storeId)
        {
            using (var strategy = CreateStrategy())
            {
                var rewardPoints = strategy.GetQuery()
                    .Where(y => y.StoreId == storeId)
                    .ToList();
                foreach (var rewardPoint in rewardPoints)
                {
                    strategy.Delete(rewardPoint);
                }
                strategy.SubmitChanges();
            }
        }

        /// <summary>
        /// Creates the writable repository strategy used for reward-point ledger mutations.
        /// </summary>
        /// <returns>A writable reward-points repository strategy.</returns>
        protected IRepoStrategy<hcc_RewardsPoints> CreateStrategy()
        {
            return Factory.Instance.CreateStrategy<hcc_RewardsPoints>();
        }

        /// <summary>
        /// Creates the read-only repository strategy used for reward-point queries.
        /// </summary>
        /// <returns>A read-only reward-points repository strategy.</returns>
        protected IRepoStrategy<hcc_RewardsPoints> CreateReadStrategy()
        {
            return Factory.Instance.CreateReadStrategy<hcc_RewardsPoints>();
        }
    }
}
