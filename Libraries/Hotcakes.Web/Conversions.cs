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
    /// Provides conversions between common measurement units used by the commerce APIs.
    /// </summary>
    /// <seealso cref="Dates"/>
    [Serializable]
    public static class Conversions
    {
        /// <summary>
        /// Converts a distance in inches to centimeters.
        /// </summary>
        /// <param name="inches">The distance, in inches.</param>
        /// <returns>The equivalent distance in centimeters.</returns>
        /// <seealso cref="CentimetersToInches(decimal)"/>
        public static decimal InchesToCentimeters(decimal inches)
        {
            var centimeters = 0m;
            centimeters = inches*2.54m;
            return centimeters;
        }

        /// <summary>
        /// Converts a distance in centimeters to inches.
        /// </summary>
        /// <param name="centimeters">The distance, in centimeters.</param>
        /// <returns>The equivalent distance in inches.</returns>
        /// <seealso cref="InchesToCentimeters(decimal)"/>
        public static decimal CentimetersToInches(decimal centimeters)
        {
            var inches = 0m;
            inches = centimeters*0.3937m;
            return inches;
        }

        /// <summary>
        /// Converts a weight in pounds to kilograms.
        /// </summary>
        /// <param name="pounds">The weight, in pounds.</param>
        /// <returns>The equivalent weight in kilograms.</returns>
        /// <seealso cref="KilogramsToPounds(decimal)"/>
        public static decimal PoundsToKilograms(decimal pounds)
        {
            var kilograms = 0m;
            kilograms = pounds*2.2046m;
            return kilograms;
        }

        /// <summary>
        /// Converts a weight in kilograms to pounds.
        /// </summary>
        /// <param name="kilograms">The weight, in kilograms.</param>
        /// <returns>The equivalent weight in pounds.</returns>
        /// <seealso cref="PoundsToKilograms(decimal)"/>
        public static decimal KilogramsToPounds(decimal kilograms)
        {
            var pounds = 0m;
            pounds = kilograms*0.4536m;
            return pounds;
        }

        /// <summary>
        /// Converts the fractional portion of a pound value to ounces.
        /// </summary>
        /// <param name="pounds">The decimal weight in pounds.</param>
        /// <returns>The fractional-pound portion expressed in ounces.</returns>
        /// <seealso cref="OuncesToDecimalPounds(decimal)"/>
        public static decimal DecimalPoundsToOunces(decimal pounds)
        {
            // Get only Partial Pounds
            var remainder = pounds%1;

            return remainder*16;
        }

        /// <summary>
        /// Converts a weight in ounces to decimal pounds.
        /// </summary>
        /// <param name="ounces">The weight, in ounces.</param>
        /// <returns>The equivalent weight in pounds.</returns>
        /// <seealso cref="DecimalPoundsToOunces(decimal)"/>
        public static decimal OuncesToDecimalPounds(decimal ounces)
        {
            var result = ounces/16.0m;
            return result;
        }
    }
}
