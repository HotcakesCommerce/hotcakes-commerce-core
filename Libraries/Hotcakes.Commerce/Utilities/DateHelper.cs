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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Reporting;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Provides date and time conversions based on a store's configured time zone and common sales-period ranges.
    /// </summary>
    public class DateHelper
    {
        /// <summary>
        /// Converts a UTC value to the time zone configured for the specified store.
        /// </summary>
        /// <param name="store">The store whose configured time zone is used.</param>
        /// <param name="utcTime">The UTC value to convert; when <see langword="null"/>, the current UTC time is used.</param>
        /// <returns>The converted date and time in the store's configured time zone.</returns>
        public static DateTime ConvertUtcToStoreTime(Store store, DateTime? utcTime = null)
        {
            var time = utcTime.HasValue ? utcTime.Value : DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeFromUtc(time, store.Settings.TimeZone);
        }

        /// <summary>
        /// Converts a date and time in a store's configured time zone to UTC.
        /// </summary>
        /// <param name="store">The store whose configured time zone is used.</param>
        /// <param name="storeTime">The date and time expressed in the store's configured time zone.</param>
        /// <returns>The equivalent UTC date and time.</returns>
        public static DateTime ConvertStoreTimeToUtc(Store store, DateTime storeTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(storeTime, store.Settings.TimeZone);
        }

        /// <summary>
        /// Converts a UTC value to the time zone configured for the current store in an application.
        /// </summary>
        /// <param name="hccApp">The application providing the current store.</param>
        /// <param name="utcTime">The UTC value to convert; when <see langword="null"/>, the current UTC time is used.</param>
        /// <returns>The converted date and time in the current store's configured time zone.</returns>
        /// <seealso cref="ConvertUtcToStoreTime(Store, DateTime?)"/>
        public static DateTime ConvertUtcToStoreTime(HotcakesApplication hccApp, DateTime? utcTime = null)
        {
            return ConvertUtcToStoreTime(hccApp.CurrentStore, utcTime);
        }

        /// <summary>
        /// Converts a date and time in the current store's configured time zone to UTC.
        /// </summary>
        /// <param name="hccApp">The application providing the current store.</param>
        /// <param name="storeTime">The date and time expressed in the current store's configured time zone.</param>
        /// <returns>The equivalent UTC date and time.</returns>
        /// <seealso cref="ConvertStoreTimeToUtc(Store, DateTime)"/>
        public static DateTime ConvertStoreTimeToUtc(HotcakesApplication hccApp, DateTime storeTime)
        {
            return ConvertStoreTimeToUtc(hccApp.CurrentStore, storeTime);
        }

        /// <summary>
        /// Creates a UTC date range for a standard sales period.
        /// </summary>
        /// <param name="period">The sales period to include.</param>
        /// <param name="previous"><see langword="true"/> to return the period immediately before the current period; otherwise, return the current period.</param>
        /// <returns>A <see cref="DateRange"/> whose end is the final second of the applicable UTC day. Unsupported period values leave the range at its default dates.</returns>
        public static DateRange GetDateRange(SalesPeriod period, bool previous = false)
        {
            var dr = new DateRange();
            dr.EndDate = DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

            switch (period)
            {
                case SalesPeriod.Year:
                    if (previous)
                    {
                        dr.EndDate = dr.EndDate.AddMonths(-12);
                    }

                    dr.StartDate = dr.EndDate.AddMonths(-12);
                    break;
                case SalesPeriod.Quarter:
                    if (previous)
                    {
                        dr.EndDate = dr.EndDate.AddMonths(-3);
                    }
                    dr.StartDate = dr.EndDate.AddMonths(-3);
                    break;
                case SalesPeriod.Month:
                    if (previous)
                    {
                        dr.EndDate = dr.EndDate.AddDays(-30);
                    }
                    dr.StartDate = dr.EndDate.AddDays(-30);
                    break;
                case SalesPeriod.Week:
                    if (previous)
                    {
                        dr.EndDate = dr.EndDate.AddDays(-7);
                    }
                    dr.StartDate = dr.EndDate.AddDays(-7);
                    break;
                default:
                    break;
            }

            return dr;
        }
    }
}
