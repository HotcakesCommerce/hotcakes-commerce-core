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
using System.Text;

namespace Hotcakes.Web.Cryptography
{
    /// <summary>
    /// Provides UTF-8, hexadecimal, and byte-array conversion helpers.
    /// </summary>
    [Serializable]
    public sealed class Conversion
    {
        /// <summary>
        /// Converts a string to its UTF-8 byte representation.
        /// </summary>
        /// <param name="inputCharacters">The string to convert.</param>
        /// <returns>The UTF-8 bytes for <paramref name="inputCharacters"/>.</returns>
        /// <see cref="BytesToString(byte[])"/>
        public static byte[] StringToBytes(string inputCharacters)
        {
            return Encoding.UTF8.GetBytes(inputCharacters);
        }

        /// <summary>
        /// Converts UTF-8 bytes to a string.
        /// </summary>
        /// <param name="utf8Bytes">The UTF-8 bytes to convert.</param>
        /// <returns>The string represented by <paramref name="utf8Bytes"/>.</returns>
        /// <see cref="StringToBytes(string)"/>
        public static string BytesToString(byte[] utf8Bytes)
        {
            return Encoding.UTF8.GetString(utf8Bytes);
        }
        
        /// <summary>
        /// Converts a string of hexadecimal numbers to a byte array.
        /// </summary>
        /// <param name="hexString">The hexadecimal string. Hyphens and spaces are ignored; for example, <c>FF-06</c> becomes bytes <c>0xFF</c> and <c>0x06</c>.</param>
        /// <returns>The bytes represented by <paramref name="hexString"/>.</returns>
        /// <see cref="ByteArrayToHex(byte[])"/>
        public static byte[] HexToByteArray(string hexString)
        {
            var working = hexString.Replace("-", string.Empty);
            working = working.Replace(" ", string.Empty);

            var NumberChars = working.Length;

            var bytes = new byte[NumberChars/2];

            for (var i = 0; i < NumberChars; i += 2)
            {
                bytes[i/2] = Convert.ToByte(working.Substring(i, 2), 16);
            }
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to a hyphen-separated hexadecimal string.
        /// </summary>
        /// <param name="input">The bytes to convert.</param>
        /// <returns>A hyphen-separated hexadecimal representation of <paramref name="input"/>.</returns>
        /// <see cref="HexToByteArray(string)"/>
        public static string ByteArrayToHex(byte[] input)
        {
            return BitConverter.ToString(input);
        }
    }
}
