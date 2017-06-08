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

namespace Hotcakes.Web
{
    [Serializable]
    public static class Dates
    {
        public static string FriendlyLocalDateFromUtc(DateTime dutc)
        {
            return FriendlyShortDate(dutc.ToLocalTime(), DateTime.Now.Year);
        }

        // Takes a date and returns a friendly version
        public static string FriendlyShortDate(DateTime d, int currentYear)
        {
            if (d.Year < currentYear)
            {
                return d.ToString("dd MMM yy");
            }
            return d.ToString("dd MMM");
        }

        public static DateTime ZeroOutTime(this DateTime input)
        {
            return new DateTime(input.Year, input.Month, input.Day, 0, 0, 0, 0, input.Kind);
        }

        public static DateTime MaxOutTime(this DateTime input)
        {
            // Note: Only precise to seconds for SQL compatibility
            var result = new DateTime(input.Year, input.Month, input.Day, 23, 59, 59, 0);
            return result;
        }
    }
}