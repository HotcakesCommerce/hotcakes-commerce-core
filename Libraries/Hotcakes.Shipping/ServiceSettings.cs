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
using System.Globalization;
using System.Runtime.Serialization;

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Stores shipping-service configuration values as string pairs and provides typed accessors
    /// for the numeric and Boolean values used by shipping services.
    /// </summary>
    /// <remarks>
    /// Values are stored using invariant culture so settings can be serialized and exchanged
    /// consistently across server cultures. The class also retains the dictionary behavior of
    /// <see cref="Dictionary{TKey, TValue}"/> for settings that do not have a typed accessor.
    /// </remarks>
    [Serializable]
    public class ServiceSettings : Dictionary<string, string>, ISerializable
    {
        private const string ZERO = "0";
        private const string ONE = "1";

        /// <summary>
        /// Initializes settings from serialized dictionary data.
        /// </summary>
        /// <param name="info">The serialization data containing the setting entries.</param>
        /// <param name="context">The source and destination context for the serialization.</param>
        /// <seealso cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/>
        public ServiceSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes an empty collection of shipping-service settings.
        /// </summary>
        /// <seealso cref="AddOrUpdate(string, string)"/>
        public ServiceSettings()
        {
        }

        /// <summary>
        /// Adds a setting or replaces the value of an existing setting.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <param name="value">The string value to store.</param>
        /// <seealso cref="GetSettingOrEmpty(string)"/>
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
        /// Gets a setting value, or an empty string when the key is not present.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <returns>The stored value, or <see cref="string.Empty"/> when no value exists for the key.</returns>
        /// <seealso cref="AddOrUpdate(string, string)"/>
        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets a Boolean setting represented by the serialized values written by
        /// <see cref="SetBoolSetting(string, bool)"/>.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <returns><see langword="true"/> only when the stored value is the true setting representation; otherwise, <see langword="false"/>.</returns>
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
        /// Stores a Boolean setting using the class's serialized numeric representation.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <param name="value">The Boolean value to store.</param>
        /// <seealso cref="GetBoolSetting(string)"/>
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
        /// Gets a decimal setting using invariant-culture parsing.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <returns>The parsed decimal value, or zero when the key is absent or its value is not a valid decimal.</returns>
        /// <seealso cref="SetDecimalSetting(string, decimal)"/>
        public decimal GetDecimalSetting(string name)
        {
            decimal result = 0;
            if (ContainsKey(name))
            {
                decimal temp = 0;
                if (decimal.TryParse(this[name], NumberStyles.Float, CultureInfo.InvariantCulture, out temp))
                {
                    result = temp;
                }
            }
            return result;
        }

        /// <summary>
        /// Stores a decimal setting using invariant-culture formatting.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <param name="value">The decimal value to store.</param>
        /// <seealso cref="GetDecimalSetting(string)"/>
        public void SetDecimalSetting(string name, decimal value)
        {
            AddOrUpdate(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets an integer setting using invariant-culture parsing.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <returns>The parsed integer value, or zero when the key is absent or its value is not a valid integer.</returns>
        /// <seealso cref="SetIntegerSetting(string, int)"/>
        public int GetIntegerSetting(string name)
        {
            var result = 0;
            if (ContainsKey(name))
            {
                var temp = 0;
                if (int.TryParse(this[name], NumberStyles.Integer, CultureInfo.InvariantCulture, out temp))
                {
                    result = temp;
                }
            }
            return result;
        }

        /// <summary>
        /// Stores an integer setting using invariant-culture formatting.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <param name="value">The integer value to store.</param>
        /// <seealso cref="GetIntegerSetting(string)"/>
        public void SetIntegerSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Stores a string setting, replacing any existing value for the key.
        /// </summary>
        /// <param name="name">The case-sensitive setting key.</param>
        /// <param name="value">The string value to store.</param>
        /// <seealso cref="AddOrUpdate(string, string)"/>
        public void SetSetting(string name, string value)
        {
            AddOrUpdate(name, value.ToString(CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// Adds or replaces all entries from another settings collection.
        /// </summary>
        /// <param name="otherSettings">The settings to merge; <see langword="null"/> is ignored.</param>
        /// <seealso cref="AddOrUpdate(string, string)"/>
        public void Merge(ServiceSettings otherSettings)
        {
            if (otherSettings == null) return;

            foreach (var kv in otherSettings)
            {
                AddOrUpdate(kv.Key, kv.Value);
            }
        }
    }
}
