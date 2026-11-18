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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Contains current and previous daily sales totals and their relative changes.</summary>
    public class WeeklySummary
    {
        /// <summary>Gets or sets Monday's current-period sales.</summary>
        public decimal Monday { get; set; }
        /// <summary>Gets or sets Tuesday's current-period sales.</summary>
        public decimal Tuesday { get; set; }
        /// <summary>Gets or sets Wednesday's current-period sales.</summary>
        public decimal Wednesday { get; set; }
        /// <summary>Gets or sets Thursday's current-period sales.</summary>
        public decimal Thursday { get; set; }
        /// <summary>Gets or sets Friday's current-period sales.</summary>
        public decimal Friday { get; set; }
        /// <summary>Gets or sets Saturday's current-period sales.</summary>
        public decimal Saturday { get; set; }
        /// <summary>Gets or sets Sunday's current-period sales.</summary>
        public decimal Sunday { get; set; }

        /// <summary>Gets the sum of the current-period daily sales.</summary>
        public decimal Week
        {
            get
            {
                return Monday
                       + Tuesday
                       + Wednesday
                       + Thursday
                       + Friday
                       + Saturday
                       + Sunday;
            }
        }

        /// <summary>Gets or sets Monday's comparison-period sales.</summary>
        public decimal MondayLast { get; set; }
        /// <summary>Gets or sets Tuesday's comparison-period sales.</summary>
        public decimal TuesdayLast { get; set; }
        /// <summary>Gets or sets Wednesday's comparison-period sales.</summary>
        public decimal WednesdayLast { get; set; }
        /// <summary>Gets or sets Thursday's comparison-period sales.</summary>
        public decimal ThursdayLast { get; set; }
        /// <summary>Gets or sets Friday's comparison-period sales.</summary>
        public decimal FridayLast { get; set; }
        /// <summary>Gets or sets Saturday's comparison-period sales.</summary>
        public decimal SaturdayLast { get; set; }
        /// <summary>Gets or sets Sunday's comparison-period sales.</summary>
        public decimal SundayLast { get; set; }

        /// <summary>Gets the sum of the comparison-period daily sales.</summary>
        public decimal WeekLast
        {
            get
            {
                return MondayLast
                       + TuesdayLast
                       + WednesdayLast
                       + ThursdayLast
                       + FridayLast
                       + SaturdayLast
                       + SundayLast;
            }
        }

        /// <summary>Gets Monday's relative change from the comparison period.</summary>
        public decimal MondayChange
        {
            get { return Change(Monday, MondayLast); }
        }

        /// <summary>Gets Tuesday's relative change from the comparison period.</summary>
        public decimal TuesdayChange
        {
            get { return Change(Tuesday, TuesdayLast); }
        }

        /// <summary>Gets Wednesday's relative change from the comparison period.</summary>
        public decimal WednesdayChange
        {
            get { return Change(Wednesday, WednesdayLast); }
        }

        /// <summary>Gets Thursday's relative change from the comparison period.</summary>
        public decimal ThursdayChange
        {
            get { return Change(Thursday, ThursdayLast); }
        }

        /// <summary>Gets Friday's relative change from the comparison period.</summary>
        public decimal FridayChange
        {
            get { return Change(Friday, FridayLast); }
        }

        /// <summary>Gets Saturday's relative change from the comparison period.</summary>
        public decimal SaturdayChange
        {
            get { return Change(Saturday, SaturdayLast); }
        }

        /// <summary>Gets Sunday's relative change from the comparison period.</summary>
        public decimal SundayChange
        {
            get { return Change(Sunday, SundayLast); }
        }

        /// <summary>Gets the relative change for the whole week.</summary>
        public decimal WeekChange
        {
            get { return Change(Week, WeekLast); }
        }

        private decimal Change(decimal current, decimal previous)
        {
            decimal result = 0;

            var diff = current - previous;

            if (diff == 0) return 0m;
            if (previous == 0) return 1m;

            result = diff/previous;

            return result;
        }

        /// <summary>Creates lines for the daily values, ordered Sunday through Saturday.</summary>
        /// <returns>The daily summary lines.</returns>
        public List<SummaryLine> GetLines()
        {
            var lines = new List<SummaryLine>();

            AddLine(lines, "Sunday", Sunday, SundayLast, SundayChange);
            AddLine(lines, "Monday", Monday, MondayLast, MondayChange);
            AddLine(lines, "Tuesday", Tuesday, TuesdayLast, TuesdayChange);
            AddLine(lines, "Wednesday", Wednesday, WednesdayLast, WednesdayChange);
            AddLine(lines, "Thursday", Thursday, ThursdayLast, ThursdayChange);
            AddLine(lines, "Friday", Friday, FridayLast, FridayChange);
            AddLine(lines, "Saturday", Saturday, SaturdayLast, SaturdayChange);

            return lines;
        }

        private void AddLine(List<SummaryLine> lines, string day, decimal thisweek, decimal lastweek, decimal change)
        {
            lines.Add(new SummaryLine
            {
                WeekDay = day,
                ThisWeek = thisweek,
                LastWeek = lastweek,
                Change = change
            });
        }

        /// <summary>Represents one daily line in a weekly summary.</summary>
        public class SummaryLine
        {
            /// <summary>Gets or sets the day label.</summary>
            public string WeekDay { get; set; }
            /// <summary>Gets or sets the current-period value.</summary>
            public decimal ThisWeek { get; set; }
            /// <summary>Gets or sets the comparison-period value.</summary>
            public decimal LastWeek { get; set; }
            /// <summary>Gets or sets the relative change.</summary>
            public decimal Change { get; set; }
        }
    }


    /// <summary>Calculates weekly sales summaries for a commerce application.</summary>
    public class SalesSummary
    {
        private readonly HotcakesApplication _hccApp;

        /// <summary>Initializes a summary calculator.</summary>
        /// <param name="hccApp">The commerce application used to query transactions.</param>
        public SalesSummary(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

        /// <summary>Populates a summary with sample values for demonstration purposes.</summary>
        /// <param name="result">The summary to populate.</param>
        public void AddSampleData(WeeklySummary result)
        {
            result.Monday = 500.23m;
            result.Tuesday = 750.01m;
            result.Wednesday = 421.75m;
            result.Thursday = 647.00m;
            result.Friday = 541.94m;
            result.Saturday = 354.11m;
            result.Sunday = 402.04m;

            result.MondayLast = 0m; // zero
            result.TuesdayLast = 610.44m; // less
            result.WednesdayLast = 422.01m; // more by a litte
            result.ThursdayLast = 567.17m; // less
            result.FridayLast = 541.94m; // same
            result.SaturdayLast = 414.55m; // more
            result.Sunday = 288.95m; // less
        }

        /// <summary>Gets current-week sales and compares them with the previous week.</summary>
        /// <returns>A weekly summary containing daily totals and relative changes.</returns>
        public WeeklySummary GetWeeklySummary()
        {
            var result = new WeeklySummary();
            var resultLast = new WeeklySummary();

            AddWeekData(result, DateRangeType.ThisWeek);
            AddWeekData(resultLast, DateRangeType.LastWeek);

            result.MondayLast = resultLast.Monday;
            result.TuesdayLast = resultLast.Tuesday;
            result.WednesdayLast = resultLast.Wednesday;
            result.ThursdayLast = resultLast.Thursday;
            result.FridayLast = resultLast.Friday;
            result.SaturdayLast = resultLast.Saturday;
            result.SundayLast = resultLast.Sunday;

            return result;
        }

        private void AddWeekData(WeeklySummary result, DateRangeType weekRangeType)
        {
            var rangeData = new DateRange();
            rangeData.RangeType = weekRangeType;
            rangeData.CalculateDatesFromType(DateHelper.ConvertUtcToStoreTime(_hccApp));

            var totalCount = 0;
            var storeId = _hccApp.CurrentStore.Id;

            var data = _hccApp.OrderServices.Transactions
                .FindForReportByDateRange(rangeData.StartDate.ToUniversalTime(), rangeData.EndDate.ToUniversalTime(),
                    storeId, int.MaxValue, 1, ref totalCount);

            decimal m = 0;
            decimal t = 0;
            decimal w = 0;
            decimal r = 0;
            decimal f = 0;
            decimal s = 0;
            decimal y = 0;

            foreach (var ot in data)
            {
                var timeStamp = DateHelper.ConvertUtcToStoreTime(_hccApp, ot.TimeStampUtc);

                switch (timeStamp.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        m += ot.AmountAppliedToOrder;
                        break;
                    case DayOfWeek.Tuesday:
                        t += ot.AmountAppliedToOrder;
                        break;
                    case DayOfWeek.Wednesday:
                        w += ot.AmountAppliedToOrder;
                        break;
                    case DayOfWeek.Thursday:
                        r += ot.AmountAppliedToOrder;
                        break;
                    case DayOfWeek.Friday:
                        f += ot.AmountAppliedToOrder;
                        break;
                    case DayOfWeek.Saturday:
                        s += ot.AmountAppliedToOrder;
                        break;
                    case DayOfWeek.Sunday:
                        y += ot.AmountAppliedToOrder;
                        break;
                }
            }

            result.Monday = m;
            result.Tuesday = t;
            result.Wednesday = w;
            result.Thursday = r;
            result.Friday = f;
            result.Saturday = s;
            result.Sunday = y;
        }
    }
}
