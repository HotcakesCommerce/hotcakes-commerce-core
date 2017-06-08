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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    public class WeeklySummary
    {
        public decimal Monday { get; set; }
        public decimal Tuesday { get; set; }
        public decimal Wednesday { get; set; }
        public decimal Thursday { get; set; }
        public decimal Friday { get; set; }
        public decimal Saturday { get; set; }
        public decimal Sunday { get; set; }

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

        public decimal MondayLast { get; set; }
        public decimal TuesdayLast { get; set; }
        public decimal WednesdayLast { get; set; }
        public decimal ThursdayLast { get; set; }
        public decimal FridayLast { get; set; }
        public decimal SaturdayLast { get; set; }
        public decimal SundayLast { get; set; }

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

        public decimal MondayChange
        {
            get { return Change(Monday, MondayLast); }
        }

        public decimal TuesdayChange
        {
            get { return Change(Tuesday, TuesdayLast); }
        }

        public decimal WednesdayChange
        {
            get { return Change(Wednesday, WednesdayLast); }
        }

        public decimal ThursdayChange
        {
            get { return Change(Thursday, ThursdayLast); }
        }

        public decimal FridayChange
        {
            get { return Change(Friday, FridayLast); }
        }

        public decimal SaturdayChange
        {
            get { return Change(Saturday, SaturdayLast); }
        }

        public decimal SundayChange
        {
            get { return Change(Sunday, SundayLast); }
        }

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

        public class SummaryLine
        {
            public string WeekDay { get; set; }
            public decimal ThisWeek { get; set; }
            public decimal LastWeek { get; set; }
            public decimal Change { get; set; }
        }
    }


    public class SalesSummary
    {
        private readonly HotcakesApplication _hccApp;

        public SalesSummary(HotcakesApplication hccApp)
        {
            _hccApp = hccApp;
        }

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