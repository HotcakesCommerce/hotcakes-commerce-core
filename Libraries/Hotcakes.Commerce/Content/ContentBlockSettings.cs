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

using System.Collections.Generic;
using Hotcakes.Web.Cryptography;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Provides string settings for a <see cref="ContentBlock"/> with typed and encrypted access helpers.
    /// </summary>
    /// <remarks>Settings are persisted as dictionary keys and string values; helper methods preserve the existing string representations.</remarks>
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

        /// <summary>
        /// Adds a setting or replaces the value of an existing setting.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The setting value.</param>
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

        /// <summary>
        /// Encrypts a value with the commerce key and adds or replaces the setting.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <param name="unencryptedValue">The plaintext value to encrypt.</param>
        public void AddOrUpdateEncrypted(string name, string unencryptedValue)
        {
            var encrypted = Encrypt(unencryptedValue);
            AddOrUpdate(name, encrypted);
        }

        /// <summary>
        /// Gets a setting value or an empty string when the key is absent.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <returns>The stored value, or <see cref="string.Empty"/>.</returns>
        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
            }
            return string.Empty;
        }

        /// <summary>
        /// Decrypts and gets a setting value, or an empty string when the key is absent or cannot be decrypted.
        /// </summary>
        /// <param name="name">The encrypted setting key.</param>
        /// <returns>The decrypted value, or <see cref="string.Empty"/>.</returns>
        public string GetSettingOrEmptyEncrypted(string name)
        {
            if (ContainsKey(name))
            {
                return Decrypt(this[name]);
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets a Boolean setting represented by the string value <c>1</c>.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <returns><see langword="true"/> only when the stored value is <c>1</c>.</returns>
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

        /// <summary>
        /// Stores a Boolean setting as <c>1</c> or <c>0</c>.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The Boolean value to store.</param>
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

        /// <summary>
        /// Parses a decimal setting.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <returns>The parsed decimal, or <c>-1</c> when absent or invalid.</returns>
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

        /// <summary>
        /// Stores a decimal setting using its standard string representation.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The decimal value to store.</param>
        public void SetDecimalSetting(string name, decimal value)
        {
            AddOrUpdate(name, value.ToString());
        }

        /// <summary>
        /// Parses an integer setting.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <param name="defValue">The value returned when the key is absent or invalid.</param>
        /// <returns>The parsed integer or <paramref name="defValue"/>.</returns>
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

        /// <summary>
        /// Stores an integer setting using its standard string representation.
        /// </summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The integer value to store.</param>
        public void SetIntegerSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString());
        }

        /// <summary>
        /// Adds or replaces all values from another settings collection.
        /// </summary>
        /// <param name="otherSettings">The settings to merge; <see langword="null"/> is ignored.</param>
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
