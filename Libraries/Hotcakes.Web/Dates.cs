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

namespace Hotcakes.Web
{
    /// <summary>
    /// Provides date formatting and boundary helpers for <see cref="DateTime"/> values.
    /// </summary>
    [Serializable]
    public static class Dates
    {
        /// <summary>
        /// Converts a UTC date to local time and formats it with <see cref="FriendlyShortDate(DateTime, int)"/>.
        /// </summary>
        /// <param name="dutc">The date and time expressed in UTC.</param>
        /// <returns>A friendly local-date string.</returns>
        public static string FriendlyLocalDateFromUtc(DateTime dutc)
        {
            return FriendlyShortDate(dutc.ToLocalTime(), DateTime.Now.Year);
        }

        /// <summary>
        /// Formats a date with a two-digit year when it is earlier than the supplied current year.
        /// </summary>
        /// <param name="d">The date to format.</param>
        /// <param name="currentYear">The year used to determine whether to include the year.</param>
        /// <returns>A friendly date string in either <c>dd MMM</c> or <c>dd MMM yy</c> format.</returns>
        /// <seealso cref="FriendlyLocalDateFromUtc(DateTime)"/>
        public static string FriendlyShortDate(DateTime d, int currentYear)
        {
            if (d.Year < currentYear)
            {
                return d.ToString("dd MMM yy");
            }
            return d.ToString("dd MMM");
        }

        /// <summary>
        /// Returns the date with its time components set to midnight while preserving its <see cref="DateTime.Kind"/>.
        /// </summary>
        /// <param name="input">The date and time to normalize.</param>
        /// <returns>A <see cref="DateTime"/> representing the same date at midnight.</returns>
        public static DateTime ZeroOutTime(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, 0, 0, 0, 0, input.Kind);
        }

        /// <summary>
        /// Returns the date with its time set to 23:59:59 for SQL-compatible precision.
        /// </summary>
        /// <param name="input">The date and time to normalize.</param>
        /// <returns>A <see cref="DateTime"/> representing the same date at the end of the day.</returns>
        /// <seealso cref="ZeroOutTime(DateTime)"/>
        public static DateTime MaxOutTime(this DateTime input)
        {
            // Note: Only precise to seconds for SQL compatibility
            var result = new DateTime(input.Year, input.Month, input.Day, 23, 59, 59, 0);
            return result;
        }
    }
}
