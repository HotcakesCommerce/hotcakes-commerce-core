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

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Identifies the predefined periods that a <see cref="DateRange"/> can calculate.
    /// </summary>
    public enum DateRangeType
    {
        /// <summary>No predefined range.</summary>
        None = 0,
        /// <summary>The current day.</summary>
        Today = 1,
        /// <summary>The week containing the supplied current time, beginning on Sunday.</summary>
        ThisWeek = 2,
        /// <summary>The complete calendar week immediately preceding the supplied current time.</summary>
        LastWeek = 3,
        /// <summary>The 31-day period ending on the supplied current day.</summary>
        Last31Days = 4,
        /// <summary>The 60-day period ending on the supplied current day.</summary>
        Last60Days = 5,
        /// <summary>The 120-day period ending on the supplied current day.</summary>
        Last120Days = 6,
        /// <summary>The current calendar year through its final day.</summary>
        YearToDate = 7,
        /// <summary>The complete calendar year immediately preceding the supplied current time.</summary>
        LastYear = 8,
        /// <summary>The broad fixed range from January 1, 1900 through December 31, 3000.</summary>
        AllDates = 9,
        /// <summary>The calendar month containing the supplied current time.</summary>
        ThisMonth = 10,
        /// <summary>The complete calendar month immediately preceding the supplied current time.</summary>
        LastMonth = 11,
        /// <summary>The calendar day immediately preceding the supplied current time.</summary>
        Yesterday = 12,
        /// <summary>A caller-defined range; assigning dates is effective while this value is selected.</summary>
        Custom = 99
    }
}
