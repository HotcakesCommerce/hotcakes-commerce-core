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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Stores a tax provider's string settings and provides compatibility helpers for common value types.
    /// </summary>
    /// <remarks>
    /// This type is a serializable <see cref="Dictionary{TKey,TValue}"/> with string keys and values;
    /// it does not define a separate serialization format. Boolean values written by
    /// <see cref="SetBoolSetting"/> are exactly <c>"1"</c> and <c>"0"</c>. Integer values are written
    /// with <see cref="int.ToString()"/> and parsed with <see cref="int.TryParse(string, out int)"/>.
    /// Missing or invalid values use the requested defaults where the corresponding getter supports one.
    /// </remarks>
    [Serializable]
    public class TaxProviderSettings : Dictionary<string, string>
    {
        private const string ZERO = "0";
        private const string ONE = "1";

        /// <summary>Adds a setting or replaces the value of an existing setting.</summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The string value to store.</param>
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

        /// <summary>Gets a setting value or an empty string when the key is absent.</summary>
        /// <param name="name">The setting key.</param>
        /// <returns>The stored value, or <see cref="string.Empty"/> when no value is stored for the key.</returns>
        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
            }
            return string.Empty;
        }

        /// <summary>Gets a Boolean setting encoded as <c>"1"</c> or not encoded as <c>"1"</c>.</summary>
        /// <param name="name">The setting key.</param>
        /// <param name="defaultValue">The value returned when the key is absent.</param>
        /// <returns><see langword="true"/> only when the stored value is exactly <c>"1"</c>; otherwise the stored key returns <see langword="false"/>.</returns>
        public bool GetBoolSetting(string name, bool defaultValue = false)
        {
            if (ContainsKey(name))
            {
                return this[name] == ONE ? true : false;
            }
            return defaultValue;
        }

        /// <summary>Gets an integer setting, falling back when the key is absent or cannot be parsed.</summary>
        /// <param name="name">The setting key.</param>
        /// <param name="defaultValue">The value returned when the key is absent or invalid.</param>
        /// <returns>The parsed integer or <paramref name="defaultValue"/>.</returns>
        public int GetIntSetting(string name, int defaultValue = 0)
        {
            var result = defaultValue;
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

        /// <summary>Stores a Boolean setting as exactly <c>"1"</c> for true or <c>"0"</c> for false.</summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The Boolean value to store.</param>
        public void SetBoolSetting(string name, bool value)
        {
            var stringValue = value ? ONE : ZERO;
            AddOrUpdate(name, stringValue);
        }

        /// <summary>Stores an integer setting using <see cref="int.ToString()"/>.</summary>
        /// <param name="name">The setting key.</param>
        /// <param name="value">The integer value to store.</param>
        public void SetIntSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString());
        }

        /// <summary>
        /// Copies all settings from another collection into this collection.
        /// </summary>
        /// <param name="otherSettings">The settings to copy.</param>
        /// <remarks>
        /// Existing keys are overwritten and keys not present in <paramref name="otherSettings"/> are
        /// retained. The source collection is not modified.
        /// </remarks>
        public void Merge(TaxProviderSettings otherSettings)
        {
            foreach (var kv in otherSettings)
            {
                AddOrUpdate(kv.Key, kv.Value);
            }
        }
    }
}
