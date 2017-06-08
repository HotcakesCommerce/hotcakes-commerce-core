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
using System.Security.Cryptography;
using System.Text;

namespace Hotcakes.Web.Cryptography
{
    [Serializable]
    public sealed class Hashing
    {
        public static string Md5Hash(string message)
        {
            return Base64.ConvertToBase64(Md5HashToBytes(message));
        }

        public static byte[] Md5HashToBytes(string message)
        {
            //The string we wish to encrypt
            var plainText = message;

            //The array of bytes that will contain the encrypted value of strPlainText
            byte[] hashedDataBytes;

            //The encoder class used to convert strPlainText to an array of bytes
            var encoder = new UTF8Encoding();

            //Create an instance of the MD5CryptoServiceProvider class
            var md5Hasher = new MD5CryptoServiceProvider();

            //Call ComputeHash, passing in the plain-text string as an array of bytes
            //The return value is the encrypted value, as an array of bytes
            hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(plainText));

            //Dispose of hasher            
            md5Hasher = null;

            return hashedDataBytes;
        }

        public static string Md5Hash(string message, string salt)
        {
            return Md5Hash(salt + message);
        }
    }
}