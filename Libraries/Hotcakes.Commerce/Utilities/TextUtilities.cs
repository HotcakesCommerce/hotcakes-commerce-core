#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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

namespace Hotcakes.Commerce.Utilities
{
    public class TextUtilities
    {
        private const string list = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const char zero = '0';

        public static string GetStringRepresentation(int number)
        {
            var tmp = 0;
            var num = Math.Abs(number);
            var val = string.Empty;
            do
            {
                //gets number for new base
                tmp = num%26;
                //gets digit in new base
                switch (tmp)
                {
                    case 0:
                        val = val.Insert(0, zero.ToString());
                        break;
                    default:
                        val = val.Insert(0, list.Substring(tmp - 1, 1));
                        break;
                }
                //retrieves next number
                num = num/26;
            } while (num != 0);
            return val;
        }
    }
}