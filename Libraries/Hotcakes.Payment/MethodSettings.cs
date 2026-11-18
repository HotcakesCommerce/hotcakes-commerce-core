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

namespace Hotcakes.Payment
{
    /// <summary>
    /// Stores named payment-method settings as string key/value pairs.
    /// </summary>
    /// <remarks>
    /// This type also provides helpers for common string, Boolean, and integer settings while
    /// retaining the dictionary serialization contract used by payment gateways.
    /// </remarks>
    [Serializable]
    public class MethodSettings : Dictionary<string, string>
    {
        private const string ZERO = "0";
        private const string ONE = "1";

        /// <summary>
        /// Adds a setting or replaces the value of an existing setting.
        /// </summary>
        /// <param name="name">The setting name.</param>
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
        /// Gets a setting value, or an empty string when the setting is not present.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <returns>The stored value, or <see cref="string.Empty" /> when no value is stored.</returns>
        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets a Boolean setting using the stored numeric representation.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <param name="defaultValue">The value to return when the setting is not present.</param>
        /// <returns><see langword="true" /> when the stored value is the enabled representation; otherwise, the default value or <see langword="false" />.</returns>
        public bool GetBoolSetting(string name, bool defaultValue = false)
        {
            if (ContainsKey(name))
            {
                return this[name] == ONE ? true : false;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets an integer setting.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <param name="defaultValue">The value to return when the setting is absent or cannot be parsed.</param>
        /// <returns>The parsed integer setting, or <paramref name="defaultValue" />.</returns>
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

        /// <summary>
        /// Stores a Boolean setting using the payment settings numeric representation.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <param name="value">The Boolean value to store.</param>
        public void SetBoolSetting(string name, bool value)
        {
            var stringValue = value ? ONE : ZERO;
            AddOrUpdate(name, stringValue);
        }

        /// <summary>
        /// Stores an integer setting as text.
        /// </summary>
        /// <param name="name">The setting name.</param>
        /// <param name="value">The integer value to store.</param>
        public void SetIntSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString());
        }

        /// <summary>
        /// Adds or replaces all settings from another collection.
        /// </summary>
        /// <param name="otherSettings">The settings to merge into this collection.</param>
        /// <remarks>Values from <paramref name="otherSettings" /> replace values with matching names.</remarks>
        public void Merge(MethodSettings otherSettings)
        {
            foreach (var kv in otherSettings)
            {
                AddOrUpdate(kv.Key, kv.Value);
            }
        }
    }
}
