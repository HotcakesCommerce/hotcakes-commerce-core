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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Membership
{
    public class CustomerPointsManager : HccServiceBase
    {
        public CustomerPointsManager(HccRequestContext context)
            : base(context)
        {
        }

        public decimal DollarCreditForPoints(int points)
        {
            var storeSettings = Context.CurrentStore.Settings;
            var pointsNeededForOneDollarCredit = storeSettings.RewardsPointsNeededPerDollarCredit;

            decimal result = 0;
            result = points/(decimal) pointsNeededForOneDollarCredit;
            result = Math.Round(result, 2);
            return result;
        }

        public int PointsNeededForPurchaseAmount(decimal purchaseAmount)
        {
            var storeSettings = Context.CurrentStore.Settings;
            var pointsNeededForOneDollarCredit = storeSettings.RewardsPointsNeededPerDollarCredit;

            var result = int.MaxValue;
            var r1 = purchaseAmount*pointsNeededForOneDollarCredit;
            result = (int) Math.Ceiling(r1);
            return result;
        }

        public int PointsToIssueForSpend(decimal spend)
        {
            var storeSettings = Context.CurrentStore.Settings;
            var pointsIssuedPerDollarSpent = storeSettings.RewardsPointsIssuedPerDollarSpent;

            var spendRounded = Math.Floor(spend);
            var points = spendRounded*pointsIssuedPerDollarSpent;
            return (int) points;
        }

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

        protected IRepoStrategy<hcc_RewardsPoints> CreateStrategy()
        {
            return Factory.Instance.CreateStrategy<hcc_RewardsPoints>();
        }

        protected IRepoStrategy<hcc_RewardsPoints> CreateReadStrategy()
        {
            return Factory.Instance.CreateReadStrategy<hcc_RewardsPoints>();
        }
    }
}