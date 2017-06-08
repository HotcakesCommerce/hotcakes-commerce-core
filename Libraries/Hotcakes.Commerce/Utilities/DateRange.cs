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

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    ///     Operates on "Local Time" by default. Not TimeZone Safe
    /// </summary>
    public class DateRange
    {
        private DateTime _EndDate = DateTime.Now;

        private DateRangeType _RangeType = DateRangeType.Custom;
        private DateTime _StartDate = DateTime.Now;

        public DateRangeType RangeType
        {
            get { return _RangeType; }
            set
            {
                _RangeType = value;
                CalculateDatesFromType(DateTime.Now);
            }
        }

        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                if (_RangeType == DateRangeType.Custom)
                {
                    _StartDate = value;
                }
            }
        }

        public DateTime EndDate
        {
            get { return _EndDate; }
            set
            {
                if (_RangeType == DateRangeType.Custom)
                {
                    _EndDate = value;
                }
            }
        }

        public void CalculateDatesFromType(DateTime currentTime)
        {
            switch (_RangeType)
            {
                case DateRangeType.AllDates:
                    _StartDate = new DateTime(1900, 1, 1, 0, 0, 0, 0);
                    _EndDate = new DateTime(3000, 12, 31, 23, 59, 59, 0);
                    break;
                case DateRangeType.Custom:
                    break;
                case DateRangeType.Last120Days:
                    _EndDate = new DateTime(currentTime.Date.Year, currentTime.Date.Month, currentTime.Date.Day, 23, 59,
                        59, 0);
                    _StartDate = FindOlderDateByDays(120, _EndDate);
                    break;
                case DateRangeType.Last31Days:
                    _EndDate = new DateTime(currentTime.Date.Year, currentTime.Date.Month, currentTime.Date.Day, 23, 59,
                        59, 0);
                    _StartDate = FindOlderDateByDays(31, _EndDate);
                    break;
                case DateRangeType.Last60Days:
                    _EndDate = new DateTime(currentTime.Date.Year, currentTime.Date.Month, currentTime.Date.Day, 23, 59,
                        59, 0);
                    _StartDate = FindOlderDateByDays(60, _EndDate);
                    break;
                case DateRangeType.LastMonth:
                    var temp = currentTime.AddMonths(-1);
                    _EndDate = new DateTime(temp.Year, temp.Month, DateTime.DaysInMonth(temp.Year, temp.Month), 23, 59,
                        59, 0);
                    _StartDate = new DateTime(temp.Year, temp.Month, 1, 0, 0, 0, 0);
                    break;
                case DateRangeType.LastWeek:
                    _EndDate = FindStartOfWeek(currentTime);
                    _StartDate = _EndDate.AddDays(-7);
                    _EndDate = _EndDate.AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case DateRangeType.LastYear:
                    _StartDate = new DateTime(currentTime.Year - 1, 1, 1, 0, 0, 0, 0);
                    _EndDate = new DateTime(currentTime.Year - 1, 12, 31, 23, 59, 59, 0);
                    break;
                case DateRangeType.None:
                    break;
                case DateRangeType.ThisMonth:
                    _StartDate = new DateTime(currentTime.Year, currentTime.Month, 1, 0, 0, 0, 0);
                    _EndDate = new DateTime(currentTime.Year, currentTime.Month,
                        DateTime.DaysInMonth(currentTime.Year, currentTime.Month), 23, 59, 59, 0);
                    break;
                case DateRangeType.ThisWeek:
                    _StartDate = FindStartOfWeek(currentTime);
                    _EndDate = _StartDate.AddDays(6);
                    _EndDate = _EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    break;
                case DateRangeType.Today:
                    _StartDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0, 0);
                    _EndDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 59, 59, 0);
                    break;
                case DateRangeType.Yesterday:
                    _StartDate =
                        new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0, 0).AddDays(-1);
                    _EndDate =
                        new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 59, 59, 0).AddDays(-1);
                    break;
                case DateRangeType.YearToDate:
                    _StartDate = new DateTime(currentTime.Year, 1, 1, 0, 0, 0, 0);
                    _EndDate = new DateTime(currentTime.Year, 12, 31, 23, 59, 59, 0);
                    break;
            }
        }

        private DateTime FindOlderDateByDays(int daysBack, DateTime fromDate)
        {
            var result = fromDate;
            result = fromDate.AddDays(-1*(daysBack - 1));
            result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0, 0);
            return result;
        }

        private DateTime FindStartOfWeek(DateTime currentDate)
        {
            var result = currentDate;
            switch (currentDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, 0);
                    break;
                case DayOfWeek.Monday:
                    result = currentDate.AddDays(-1);
                    break;
                case DayOfWeek.Tuesday:
                    result = currentDate.AddDays(-2);
                    break;
                case DayOfWeek.Wednesday:
                    result = currentDate.AddDays(-3);
                    break;
                case DayOfWeek.Thursday:
                    result = currentDate.AddDays(-4);
                    break;
                case DayOfWeek.Friday:
                    result = currentDate.AddDays(-5);
                    break;
                case DayOfWeek.Saturday:
                    result = currentDate.AddDays(-6);
                    break;
            }
            result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0, 0);
            return result;
        }
    }
}