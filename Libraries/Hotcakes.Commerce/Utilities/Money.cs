#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    public class Money
    {
        public static decimal ApplyDiscountPercent(decimal baseAmount, decimal percentage)
        {
            return RoundCurrency(baseAmount*((100 - percentage)/100));
        }

        public static decimal ApplyIncreasedPercent(decimal baseAmount, decimal percentage)
        {
            return RoundCurrency(baseAmount*((100 + percentage)/100));
        }

        public static decimal GetDiscountAmountByPercent(decimal baseAmount, decimal percentage)
        {
            return RoundCurrency(baseAmount*(percentage/100));
        }

        public static decimal GetDiscountAmount(decimal baseAmount, decimal discountAmount)
        {
            return discountAmount;
        }

        public static decimal ApplyDiscountAmount(decimal baseAmount, decimal discountAmount)
        {
            return baseAmount - discountAmount;
        }

        public static decimal ApplyIncreasedAmount(decimal baseAmount, decimal increaseAmount)
        {
            return baseAmount + increaseAmount;
        }

        public static string FormatCurrency(decimal amount)
        {
            return FormatCurrency(amount, Thread.CurrentThread.CurrentUICulture);
        }

        public static string FormatCurrency(decimal amount, CultureInfo c)
        {
            var digits = c.NumberFormat.CurrencyDecimalDigits;
            return amount.ToString("F0" + digits);
        }

        public static decimal RoundCurrency(decimal amount)
        {
            return RoundCurrency(amount, Thread.CurrentThread.CurrentUICulture);
        }

        public static decimal RoundCurrency(decimal amount, CultureInfo c)
        {
            var digits = c.NumberFormat.CurrencyDecimalDigits;
            return decimal.Round(amount, digits, MidpointRounding.AwayFromZero);
        }

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