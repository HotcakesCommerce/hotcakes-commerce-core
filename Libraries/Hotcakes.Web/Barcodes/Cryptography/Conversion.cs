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
using System.Text;

namespace Hotcakes.Web.Cryptography
{
    [Serializable]
    public sealed class Conversion
    {
        // Converts a plain text string into an array of bytes
        public static byte[] StringToBytes(string inputCharacters)
        {
            return Encoding.UTF8.GetBytes(inputCharacters);
        }

        // Converts an array of bytes to a string
        public static string BytesToString(byte[] utf8Bytes)
        {
            return Encoding.UTF8.GetString(utf8Bytes);
        }
        
        // Converts as string containing hex numbers into an array of bytes
        // Example: "FF-06" would convert to a byte[] {0xFF, 0x06} OR byte[] {255, 6}
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

        public static string ByteArrayToHex(byte[] input)
        {
            return BitConverter.ToString(input);
        }
    }
}