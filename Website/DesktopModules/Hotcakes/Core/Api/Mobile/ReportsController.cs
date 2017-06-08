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
using System.Linq;
using System.Web.Http;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Api.Mobile.Models;

namespace Hotcakes.Modules.Core.Api.Mobile
{
    [Serializable]
    public class ReportsController : HccApiController
    {
        [HttpGet]
        [MobileAuthorize]
        public ReportSummary Summary()
        {
            var allRange = new DateRange {RangeType = DateRangeType.AllDates};
            return new ReportSummary
            {
                StoreName = HccApp.CurrentStore.Settings.FriendlyName,
                ReadyForPaymentCount = GetOrdersReadyForPaymentCount(allRange),
                ReadyForShippingCount = GetOrdersReadyForShippingCount(allRange),
                PeriodSummaries = new List<ReportPeriodSummary>
                {
                    GetPeriodSummary("day"),
                    GetPeriodSummary("week"),
                    GetPeriodSummary("month"),
                    GetPeriodSummary("year")
                }
            };
        }

        [HttpGet]
        [MobileAuthorize]
        public ReportSummary Summary(string period)
        {
            var range = GetDateRange(period);

            return new ReportSummary
            {
                StoreName = HccApp.CurrentStore.Settings.FriendlyName,
                ReadyForPaymentCount = GetOrdersReadyForPaymentCount(range),
                ReadyForShippingCount = GetOrdersReadyForShippingCount(range),
                PeriodSummaries = new List<ReportPeriodSummary>
                {
                    GetPeriodSummary(period)
                }
            };
        }

        #region Implementation

        private int GetOrdersReadyForPaymentCount(DateRange range)
        {
            return HccApp.OrderServices.Orders.GetOrdersReadyForPaymentCount(range.StartDate, range.EndDate);
        }

        private int GetOrdersReadyForShippingCount(DateRange range)
        {
            return HccApp.OrderServices.Orders.GetOrdersReadyForShippingCount(range.StartDate, range.EndDate);
        }

        private int GetOrdersCount(DateRange range, string status)
        {
            var criteria = new OrderSearchCriteria
            {
                StartDateUtc = range.StartDate,
                EndDateUtc = range.EndDate,
                StatusCode = status
            };

            return HccApp.OrderServices.Orders.CountByCriteria(criteria);
        }

        private ReportPeriodSummary GetPeriodSummary(string period)
        {
            var range = GetDateRange(period);
            var series = GetSeries(range)
                .Select(sd => new Point {X = sd.Period, Y = sd.Sum})
                .ToList();

            return new ReportPeriodSummary
            {
                Period = period,
                Total = series.Sum(s => s.Y),
                Series = series,
                OrdersCount = GetOrdersCount(range, string.Empty)
            };
        }

        private List<SalesSummaryData> GetSeries(DateRange range)
        {
            range.CalculateDatesFromType(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                HccApp.CurrentStore.Settings.TimeZone));
            return HccApp.OrderServices.Transactions.FindTotalTransactionsByDateRange(range.StartDate, range.EndDate,
                GetRangeFunction(range.RangeType));
        }

        protected static Func<hcc_OrderTransactions, int> GetRangeFunction(DateRangeType rangeType)
        {
            Func<hcc_OrderTransactions, int> separationFunc;
            switch (rangeType)
            {
                case DateRangeType.AllDates:
                    separationFunc = null;
                    break;
                case DateRangeType.Today:
                    separationFunc = ot => ot.Timestamp.Hour;
                    break;
                case DateRangeType.ThisMonth:
                    separationFunc = ot => ot.Timestamp.Day;
                    break;
                case DateRangeType.YearToDate:
                    separationFunc = ot => ot.Timestamp.Month;
                    break;
                case DateRangeType.ThisWeek:
                default:
                    separationFunc = ot => (int) ot.Timestamp.DayOfWeek;
                    break;
            }

            return separationFunc;
        }

        #endregion
    }
}