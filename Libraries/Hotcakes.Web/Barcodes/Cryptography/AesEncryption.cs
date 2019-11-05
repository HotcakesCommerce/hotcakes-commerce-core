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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hotcakes.Web.Cryptography
{
    [Serializable]
    public sealed class AesEncryption
    {
        private static readonly byte[] IV =
        {
            0x63, 0x49, 0x41, 0x2F, 0xCE, 0x44, 0xF1, 0x6E, 0x5A, 0x32, 0x05, 0xC4,
            0x82, 0x93, 0x12, 0xF5
        };

        public static string Encode(string message, string key)
        {
            var result = string.Empty;

            try
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                // Convert input to byte array
                var messageBytes = Conversion.StringToBytes(message);
                var keyBytes = Conversion.HexToByteArray(key);

                var provider = new RijndaelManaged();
                var transform = provider.CreateEncryptor(keyBytes, IV);
                var stream = new MemoryStream();
                var crypto = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                crypto.Write(messageBytes, 0, messageBytes.Length);
                crypto.FlushFinalBlock();

                if (stream.Length > 0)
                {
                    result = Base64.ConvertToBase64(stream.ToArray());
                }
                crypto.Close();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Hotcakes.Web.Cryptography.AesEncryption: " + ex.Message);
            }

            return result;
        }

        public static string Decode(string encoded, string key)
        {
            var result = string.Empty;

            try
            {
                var encodedBytes = Base64.ConvertFromBase64(encoded);
                var keyBytes = Conversion.HexToByteArray(key);

                var provider = new RijndaelManaged();
                var transform = provider.CreateDecryptor(keyBytes, IV);
                var stream = new MemoryStream();
                var crypto = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                crypto.Write(encodedBytes, 0, encodedBytes.Length);
                crypto.FlushFinalBlock();

                if (stream.Length > 0)
                {
                    result = Encoding.UTF8.GetString(stream.GetBuffer(), 0, Convert.ToInt32(stream.Length));
                }

                if (crypto != null)
                {
                    crypto.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Hotcakes.Web.Cryptography.AesEncryption.Decode: " + ex.Message);
            }

            return result;
        }
    }
}