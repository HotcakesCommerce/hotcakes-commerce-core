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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Reporting;

namespace Hotcakes.Commerce
{
    /// <summary>Provides reporting-function adapters for an order repository strategy.</summary>
    public class RepoStrategyFunctions
    {
        /// <summary>Executes the promotions activity report for a store and UTC date range.</summary>
        /// <param name="repo">The repository strategy used to execute the database function.</param><param name="storeId">The store identifier.</param><param name="startDateUtc">The inclusive report start date in UTC.</param><param name="endDateUtc">The report end date in UTC.</param>
        /// <returns>The function's result rows as returned by the repository; null behavior is determined by <see cref="IRepoStrategy{T}"/>.</returns>
        public static IList<hcc_GetPromotionsActivityList_Result> GetPromotionsActivityList(
            IRepoStrategy<hcc_Order> repo, long storeId, DateTime startDateUtc, DateTime endDateUtc)
        {
            return repo.ExecFunction<hcc_GetPromotionsActivityList_Result>("GetPromotionsActivityList",
                new
                {
                    StoreId = storeId,
                    StartDateUtc = startDateUtc,
                    EndDateUtc = endDateUtc
                });
        }

        /// <summary>Executes the bounced-order report grouped by UTC day and maps each row to a <see cref="BounceItem"/>.</summary>
        /// <param name="repo">The repository strategy used to execute the database function.</param><param name="storeId">The store identifier.</param><param name="rangeStart">The report range start.</param><param name="rangeEnd">The report range end.</param><param name="productGuid">An optional product filter.</param><param name="categoryGuid">An optional category filter.</param>
        /// <returns>A list containing each result row's non-null year, month, day, and count mapped to <see cref="BounceItem.Year"/>, <see cref="BounceItem.Month"/>, <see cref="BounceItem.Day"/>, and <see cref="BounceItem.Amount"/>.</returns>
        /// <remarks>The mapping dereferences nullable result fields; the database function must return values for Year, Month, Day, and Count.</remarks>
        public static List<BounceItem> GetBouncedListByDay(IRepoStrategy<hcc_Order> repo, long storeId,
            DateTime rangeStart, DateTime rangeEnd, Guid? productGuid = null, Guid? categoryGuid = null)
        {
            var bouncedList = new List<BounceItem>();
            var result = repo.ExecFunction<hcc_GetBouncedListByDay_Result>("GetBouncedListByDay",
                new
                {
                    storeId,
                    rangeStart,
                    rangeEnd,
                    productGuid,
                    categoryGuid
                }
                );
            foreach (var item in result)
            {
                bouncedList.Add(new BounceItem
                {
                    Year = item.Year.Value,
                    Month = item.Month.Value,
                    Day = item.Day.Value,
                    Amount = item.Count.Value
                });
            }
            return bouncedList;
        }

        /// <summary>Executes the bounced-order report grouped by hour and maps each row to a <see cref="BounceItem"/>.</summary>
        /// <param name="repo">The repository strategy used to execute the database function.</param><param name="storeId">The store identifier.</param><param name="rangeStart">The report range start.</param><param name="rangeEnd">The report range end.</param><param name="productGuid">An optional product filter.</param><param name="categoryGuid">An optional category filter.</param>
        /// <returns>A list containing each result row's non-null day, hour, and count mapped to <see cref="BounceItem.Day"/>, <see cref="BounceItem.Hour"/>, and <see cref="BounceItem.Amount"/>.</returns>
        /// <remarks>The mapping dereferences nullable result fields; the database function must return values for Day, Hour, and Count.</remarks>
        public static List<BounceItem> GetBouncedListByHour(IRepoStrategy<hcc_Order> repo, long storeId,
            DateTime rangeStart, DateTime rangeEnd, Guid? productGuid = null, Guid? categoryGuid = null)
        {
            var bouncedList = new List<BounceItem>();
            var result = repo.ExecFunction<hcc_GetBouncedListByHour_Result>("GetBouncedListByHour",
                new
                {
                    storeId,
                    rangeStart,
                    rangeEnd,
                    productGuid,
                    categoryGuid
                }
                );
            foreach (var item in result)
            {
                bouncedList.Add(new BounceItem
                {
                    Day = item.Day.Value,
                    Hour = item.Hour.Value,
                    Amount = item.Count.Value
                });
            }
            return bouncedList;
        }
    }
}
