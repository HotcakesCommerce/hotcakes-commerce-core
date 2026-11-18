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

namespace Hotcakes.Web.Cryptography
{
    /// <summary>
    /// Provides helpers for converting text and byte arrays to and from Base64.
    /// </summary>
    [Serializable]
    public sealed class Base64
    {
        /// <summary>
        /// Converts a UTF-8 string to a Base64 string.
        /// </summary>
        /// <param name="input">The string to convert.</param>
        /// <returns>The UTF-8 bytes of <paramref name="input"/> encoded as Base64.</returns>
        /// <see cref="ConvertToBase64(byte[])"/>
        /// <see cref="Conversion.StringToBytes(string)"/>
        public static string ConvertStringToBase64(string input)
        {
            var data = Conversion.StringToBytes(input);
            var output = ConvertToBase64(data);
            return output;
        }

        /// <summary>
        /// Converts a Base64 string to a UTF-8 string.
        /// </summary>
        /// <param name="input">The Base64 string to convert.</param>
        /// <returns>The decoded bytes interpreted as a UTF-8 string.</returns>
        /// <see cref="ConvertFromBase64(string)"/>
        /// <see cref="Conversion.BytesToString(byte[])"/>
        public static string ConvertStringFromBase64(string input)
        {
            var data = ConvertFromBase64(input);
            var output = Conversion.BytesToString(data);
            return output;
        }

        /// <summary>
        /// Encodes a byte array as a Base64 string.
        /// </summary>
        /// <param name="input">The bytes to encode.</param>
        /// <returns>The Base64 representation of <paramref name="input"/>.</returns>
        /// <see cref="ConvertFromBase64(string)"/>
        public static string ConvertToBase64(byte[] input)
        {
            var output = string.Empty;
            output = Convert.ToBase64String(input, 0, input.Length);
            return output;
        }

        /// <summary>
        /// Decodes a Base64 string into a byte array.
        /// </summary>
        /// <param name="input">The Base64 string to decode.</param>
        /// <returns>The decoded bytes.</returns>
        /// <exception cref="FormatException">The input is not a valid Base64 string.</exception>
        /// <see cref="ConvertToBase64(byte[])"/>
        public static byte[] ConvertFromBase64(string input)
        {
            var output = Convert.FromBase64String(input);
            return output;
        }
    }
}
