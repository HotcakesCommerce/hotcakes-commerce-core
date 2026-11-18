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
using System.Globalization;
using System.Threading;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Provides currency arithmetic, rounding, and display helpers.
    /// </summary>
    public class Money
    {
        /// <summary>Applies a percentage discount and rounds the result using the current UI culture's currency precision.</summary>
        /// <param name="baseAmount">The amount before the discount.</param>
        /// <param name="percentage">The discount percentage.</param>
        /// <returns>The discounted, rounded amount.</returns>
        public static decimal ApplyDiscountPercent(decimal baseAmount, decimal percentage)
        {
            return RoundCurrency(baseAmount*((100 - percentage)/100));
        }

        /// <summary>Applies a percentage increase and rounds the result using the current UI culture's currency precision.</summary>
        /// <param name="baseAmount">The amount before the increase.</param>
        /// <param name="percentage">The increase percentage.</param>
        /// <returns>The increased, rounded amount.</returns>
        public static decimal ApplyIncreasedPercent(decimal baseAmount, decimal percentage)
        {
            return RoundCurrency(baseAmount*((100 + percentage)/100));
        }

        /// <summary>Calculates a percentage discount and rounds it using the current UI culture's currency precision.</summary>
        /// <param name="baseAmount">The amount on which the discount is calculated.</param>
        /// <param name="percentage">The discount percentage.</param>
        /// <returns>The rounded discount amount.</returns>
        public static decimal GetDiscountAmountByPercent(decimal baseAmount, decimal percentage)
        {
            return RoundCurrency(baseAmount*(percentage/100));
        }

        /// <summary>Returns a fixed discount amount.</summary>
        /// <param name="baseAmount">The original amount; retained for API symmetry and not used in the calculation.</param>
        /// <param name="discountAmount">The fixed discount amount.</param>
        /// <returns><paramref name="discountAmount"/> unchanged.</returns>
        public static decimal GetDiscountAmount(decimal baseAmount, decimal discountAmount)
        {
            return discountAmount;
        }

        /// <summary>Subtracts a fixed discount amount from a base amount.</summary>
        /// <param name="baseAmount">The amount before the discount.</param>
        /// <param name="discountAmount">The fixed amount to subtract.</param>
        /// <returns>The amount after subtraction.</returns>
        public static decimal ApplyDiscountAmount(decimal baseAmount, decimal discountAmount)
        {
            return baseAmount - discountAmount;
        }

        /// <summary>Adds a fixed increase amount to a base amount.</summary>
        /// <param name="baseAmount">The amount before the increase.</param>
        /// <param name="increaseAmount">The fixed amount to add.</param>
        /// <returns>The amount after addition.</returns>
        public static decimal ApplyIncreasedAmount(decimal baseAmount, decimal increaseAmount)
        {
            return baseAmount + increaseAmount;
        }

        /// <summary>Formats an amount with the current UI culture's number of currency decimal places.</summary>
        /// <param name="amount">The amount to format.</param>
        /// <returns>The formatted fixed-point amount.</returns>
        /// <seealso cref="FormatCurrency(decimal, CultureInfo)"/>
        public static string FormatCurrency(decimal amount)
        {
            return FormatCurrency(amount, Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>Formats an amount with the specified culture's currency decimal precision.</summary>
        /// <param name="amount">The amount to format.</param>
        /// <param name="c">The culture whose currency precision is used.</param>
        /// <returns>The formatted fixed-point amount.</returns>
        public static string FormatCurrency(decimal amount, CultureInfo c)
        {
            var digits = c.NumberFormat.CurrencyDecimalDigits;
            return amount.ToString("F0" + digits);
        }

        /// <summary>Rounds an amount to the current UI culture's currency decimal precision.</summary>
        /// <param name="amount">The amount to round.</param>
        /// <returns>The rounded amount, using midpoint-away-from-zero rounding.</returns>
        /// <seealso cref="RoundCurrency(decimal, CultureInfo)"/>
        public static decimal RoundCurrency(decimal amount)
        {
            return RoundCurrency(amount, Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>Rounds an amount to the specified culture's currency decimal precision.</summary>
        /// <param name="amount">The amount to round.</param>
        /// <param name="c">The culture whose currency precision is used.</param>
        /// <returns>The rounded amount, using midpoint-away-from-zero rounding.</returns>
        public static decimal RoundCurrency(decimal amount, CultureInfo c)
        {
            var digits = c.NumberFormat.CurrencyDecimalDigits;
            return decimal.Round(amount, digits, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Produces a compact display string for an amount, using thousand, million, or billion suffixes when needed.
        /// </summary>
        /// <param name="amount">The amount to display. The value is rounded upward before formatting.</param>
        /// <param name="format">The format provider used by numeric formatting.</param>
        /// <param name="digitsMax">The maximum number of leading digits before a suffix is selected.</param>
        /// <param name="formatString">The numeric format prefix, such as <c>F</c>.</param>
        /// <returns>A compact amount string with an optional <c>k</c>, <c>m</c>, or <c>b</c> suffix.</returns>
        public static string GetFriendlyAmount(decimal amount, IFormatProvider format, int digitsMax = 3,
            string formatString = "F")
        {
            amount = Math.Ceiling(amount);
            var digits = Math.Floor(Math.Log10((double) amount) + 1);
            var diff = digits - digitsMax;

            if (diff > 0)
            {
                if (digits <= digitsMax + 3)
                {
                    var d = digitsMax + 3 - digits;
                    return (amount/1000).ToString(formatString + d) + "k";
                }
                if (digits <= digitsMax + 6)
                {
                    var d = digitsMax + 6 - digits;
                    return (amount/1000000).ToString(formatString + d) + "m";
                }
                if (digits <= digitsMax + 9)
                {
                    var d = digitsMax + 9 - digits;
                    return (amount/1000000000).ToString(formatString + d) + "b";
                }
            }

            return amount.ToString(formatString + "0");
        }
    }
}
