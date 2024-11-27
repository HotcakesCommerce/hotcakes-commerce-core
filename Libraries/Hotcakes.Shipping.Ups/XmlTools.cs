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
using System.Text.RegularExpressions;

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public sealed class XmlTools
    {
        private XmlTools()
        {
        }
       
        public static string CleanPhoneNumber(string sNumber)
        {
            var input = sNumber.Trim();
            input = Regex.Replace(input, @"[^0-9]", string.Empty);
            return input;
        }

        public static string TrimToLength(string input, int maxLength)
        {
            if (input == null)
            {
                return string.Empty;
            }
            if (input.Length < 1)
            {
                return input;
            }
            if (maxLength < 0)
            {
                maxLength = input.Length;
            }

            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength);
            }
            return input;
        }
     
    }
}