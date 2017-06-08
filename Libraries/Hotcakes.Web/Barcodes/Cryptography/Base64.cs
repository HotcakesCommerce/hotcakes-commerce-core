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

namespace Hotcakes.Web.Cryptography
{
    [Serializable]
    public sealed class Base64
    {
        public static string ConvertStringToBase64(string input)
        {
            var data = Conversion.StringToBytes(input);
            var output = ConvertToBase64(data);
            return output;
        }

        public static string ConvertStringFromBase64(string input)
        {
            var data = ConvertFromBase64(input);
            var output = Conversion.BytesToString(data);
            return output;
        }

        public static string ConvertToBase64(byte[] input)
        {
            var output = string.Empty;
            output = Convert.ToBase64String(input, 0, input.Length);
            return output;
        }

        public static byte[] ConvertFromBase64(string input)
        {
            var output = Convert.FromBase64String(input);
            return output;
        }
    }
}