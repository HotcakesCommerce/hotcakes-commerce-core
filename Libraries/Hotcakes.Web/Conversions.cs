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
    public static class Conversions
    {
        /// <summary>
        ///     Converts inches to centimeters
        /// </summary>
        /// <param name="inches">Inches to convert</param>
        /// <returns>Inches converted to centimeters</returns>
        public static decimal InchesToCentimeters(decimal inches)
        {
            var centimeters = 0m;
            centimeters = inches*2.54m;
            return centimeters;
        }

        /// <summary>
        ///     Converts centimeters to inches
        /// </summary>
        /// <param name="centimeters">Centimeters to convert</param>
        /// <returns>Centimeters converted to inches</returns>
        public static decimal CentimetersToInches(decimal centimeters)
        {
            var inches = 0m;
            inches = centimeters*0.3937m;
            return inches;
        }

        /// <summary>
        ///     Converts pounds to kilograms
        /// </summary>
        /// <param name="pounds">Pound amount to convert</param>
        /// <returns>Pounds converted to Kilograms</returns>
        public static decimal PoundsToKilograms(decimal pounds)
        {
            var kilograms = 0m;
            kilograms = pounds*2.2046m;
            return kilograms;
        }

        /// <summary>
        ///     Converts Kilograms to Pounds
        /// </summary>
        /// <param name="kilograms">Kilogram amount to convert</param>
        /// <returns>Kilograms converted to pounds</returns>
        public static decimal KilogramsToPounds(decimal kilograms)
        {
            var pounds = 0m;
            pounds = kilograms*0.4536m;
            return pounds;
        }

        /// <summary>
        ///     Converts the non-whole pounds portion of a decial number to ounces
        /// </summary>
        /// <param name="pounds">The decimal representation of pounds to be converted</param>
        /// <returns>Only the non-whole pound portion of the pounds converted to ounces</returns>
        public static decimal DecimalPoundsToOunces(decimal pounds)
        {
            // Get only Partial Pounds
            var remainder = pounds%1;

            return remainder*16;
        }

        // Converts ounces to decimal pounds
        public static decimal OuncesToDecimalPounds(decimal ounces)
        {
            var result = ounces/16.0m;
            return result;
        }
    }
}