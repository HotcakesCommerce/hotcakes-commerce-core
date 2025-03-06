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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hotcakes.Web.Cryptography
{
    [Serializable]
    public class TripleDesEncryption
    {
        private readonly byte[] _IV = new byte[8]; //7 in vb

        // Old Default Key
        private readonly string _Key =
            "EDBE6BF8A92A417cBCD3DB23120861B5DE780BA44DB44166888707607A2A16FBBADFD3E111D54396A5701CE43E0EC3FFAE5543370AF54228B65CB87D7E346048";

        private readonly byte[] _KeyBytes = new byte[24]; //23 in vb

        public TripleDesEncryption()
        {
            var arrKey = new byte[_Key.Length];
            arrKey = Conversion.StringToBytes(_Key);
            var arrHash = new byte[arrKey.Length];
            arrHash = ConvertToHash(arrKey);
            if (!SetKeys(arrHash))
            {
                throw new ArgumentException("Triple DES Encryption Key Failed to Set");
            }
        }

        public TripleDesEncryption(string requestedKey)
        {
            if (requestedKey != null)
            {
                _Key = requestedKey;
            }
            var arrKey = new byte[_Key.Length];
            arrKey = Conversion.StringToBytes(_Key);
            var arrHash = new byte[arrKey.Length];
            arrHash = ConvertToHash(arrKey);
            if (!SetKeys(arrHash))
            {
                throw new ArgumentException("Triple DES Encryption Key Failed to Set");
            }
        }

        public string Encode(string message)
        {
            var sOutput = string.Empty;

            try
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }

                // Convert input to byte array
                var arrInput = new byte[message.Length];
                arrInput = Conversion.StringToBytes(message);

                TripleDESCryptoServiceProvider TripleDESProvider;

                ICryptoTransform TripleDESEncryptor;
                CryptoStream TripleDESStream;
                MemoryStream outStream;
                TripleDESProvider = new TripleDESCryptoServiceProvider();
                TripleDESEncryptor = TripleDESProvider.CreateEncryptor(_KeyBytes, _IV);
                outStream = new MemoryStream();
                TripleDESStream = new CryptoStream(outStream, TripleDESEncryptor, CryptoStreamMode.Write);
                TripleDESStream.Write(arrInput, 0, arrInput.Length);
                TripleDESStream.FlushFinalBlock();

                if (outStream.Length == 0)
                {
                    sOutput = string.Empty;
                }
                else
                {
                    sOutput = Base64.ConvertToBase64(outStream.ToArray());
                }

                TripleDESStream.Close();
            }
            catch
            {
            }

            return sOutput;
        }

        public string Decode(string message)
        {
            var sOutput = string.Empty;

            byte[] arrInput;
            TripleDESCryptoServiceProvider TripleDESProvider;
            ICryptoTransform TripleDESDecryptor;
            CryptoStream TripleDESStream = null;
            MemoryStream outStream;

            try
            {
                arrInput = Base64.ConvertFromBase64(message);
                TripleDESProvider = new TripleDESCryptoServiceProvider();
                TripleDESDecryptor = TripleDESProvider.CreateDecryptor(_KeyBytes, _IV);
                outStream = new MemoryStream();
                TripleDESStream = new CryptoStream(outStream, TripleDESDecryptor, CryptoStreamMode.Write);
                TripleDESStream.Write(arrInput, 0, arrInput.Length);
                TripleDESStream.FlushFinalBlock();

                if (outStream.Length == 0)
                {
                    sOutput = string.Empty;
                }
                else
                {
                    sOutput = Encoding.ASCII.GetString(outStream.GetBuffer(), 0, Convert.ToInt32(outStream.Length));
                }
            }
            catch
            {
            }

            return sOutput;
        }

        private static byte[] ConvertToHash(byte[] arraryInput)
        {
            byte[] arrOutput;
            var sha = new SHA256Managed();
            arrOutput = sha.ComputeHash(arraryInput);
            return arrOutput;
        }

        private bool SetKeys(byte[] arraryHash)
        {
            if (arraryHash.Length < 32)
            {
                throw new ArgumentOutOfRangeException("Encryption Key Length is Too Short");
            }

            try
            {
                var i = 0;
                for (i = 0; i < 8; i++)
                {
                    _IV[i] = arraryHash[i];
                }
                for (i = 8; i < 32; i++)
                {
                    _KeyBytes[i - 8] = arraryHash[i];
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}