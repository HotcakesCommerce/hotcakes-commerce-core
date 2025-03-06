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

using System.Collections.Generic;
using Hotcakes.Web.Cryptography;

namespace Hotcakes.Commerce.Content
{
    public class ContentBlockSettings : Dictionary<string, string>
    {
        private const string ZERO = "0";
        private const string ONE = "1";

        private string Encrypt(string input)
        {
            return AesEncryption.Encode(input, KeyManager.GetKey(0));
        }

        private string Decrypt(string input)
        {
            try
            {
                return AesEncryption.Decode(input, KeyManager.GetKey(0));
            }
            catch
            {
            }
            return string.Empty;
        }

        public void AddOrUpdate(string name, string value)
        {
            if (ContainsKey(name))
            {
                this[name] = value;
            }
            else
            {
                Add(name, value);
            }
        }

        public void AddOrUpdateEncrypted(string name, string unencryptedValue)
        {
            var encrypted = Encrypt(unencryptedValue);
            AddOrUpdate(name, encrypted);
        }

        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
            }
            return string.Empty;
        }

        public string GetSettingOrEmptyEncrypted(string name)
        {
            if (ContainsKey(name))
            {
                return Decrypt(this[name]);
            }
            return string.Empty;
        }

        public bool GetBoolSetting(string name)
        {
            if (ContainsKey(name))
            {
                if (this[name] == ONE)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetBoolSetting(string name, bool value)
        {
            if (value)
            {
                AddOrUpdate(name, ONE);
            }
            else
            {
                AddOrUpdate(name, ZERO);
            }
        }

        public decimal GetDecimalSetting(string name)
        {
            decimal result = -1;
            if (ContainsKey(name))
            {
                decimal temp = 0;
                if (decimal.TryParse(this[name], out temp))
                {
                    result = temp;
                }
            }
            return result;
        }

        public void SetDecimalSetting(string name, decimal value)
        {
            AddOrUpdate(name, value.ToString());
        }

        public int GetIntegerSetting(string name, int defValue = -1)
        {
            var result = defValue;
            if (ContainsKey(name))
            {
                var temp = 0;
                if (int.TryParse(this[name], out temp))
                {
                    result = temp;
                }
            }
            return result;
        }

        public void SetIntegerSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString());
        }

        public void Merge(ContentBlockSettings otherSettings)
        {
            if (otherSettings == null) return;

            foreach (var kv in otherSettings)
            {
                AddOrUpdate(kv.Key, kv.Value);
            }
        }
    }
}