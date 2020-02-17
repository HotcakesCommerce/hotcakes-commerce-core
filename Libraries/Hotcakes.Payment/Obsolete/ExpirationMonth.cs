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
using System.Collections.Generic;

namespace Hotcakes.Payment
{
    [Serializable]
    [Obsolete("Obsolete in 2.0.0. There is no sense in this class")]
    public class ExpirationMonth
    {
        public ExpirationMonth()
        {
            Name = string.Empty;
            Number = 0;
        }

        public ExpirationMonth(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public string Name { get; set; }
        public int Number { get; set; }

        public static List<ExpirationMonth> ListMonths()
        {
            var result = new List<ExpirationMonth>();

            result.Add(new ExpirationMonth("1 JAN", 1));
            result.Add(new ExpirationMonth("2 FEB", 2));
            result.Add(new ExpirationMonth("3 MAR", 3));
            result.Add(new ExpirationMonth("4 APR", 4));
            result.Add(new ExpirationMonth("5 MAY", 5));
            result.Add(new ExpirationMonth("6 JUN", 6));
            result.Add(new ExpirationMonth("7 JUL", 7));
            result.Add(new ExpirationMonth("8 AUG", 8));
            result.Add(new ExpirationMonth("9 SEP", 9));
            result.Add(new ExpirationMonth("10 OCT", 10));
            result.Add(new ExpirationMonth("11 NOV", 11));
            result.Add(new ExpirationMonth("12 DEC", 12));

            return result;
        }
    }
}