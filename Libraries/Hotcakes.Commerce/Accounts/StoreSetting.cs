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
using System.Globalization;

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Represents StoreSetting data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class StoreSetting
    {
        private const string ZERO = "0";
        private const string ONE = "1";
        private const string TRUE = "TRUE";
        private const string T = "T";

        /// <summary>Performs the StoreSetting operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public StoreSetting()
        {
            Id = -1;
            StoreId = -1;
            ContentCulture = string.Empty;
            SettingName = string.Empty;
            SettingValue = string.Empty;
            LocalizedSettingValue = string.Empty;
        }

        /// <summary>Gets or sets Id.</summary>
        /// <remarks>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</remarks>
        /// <value>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</value>
        public long Id { get; set; }
        /// <summary>Gets or sets StoreId.</summary>
        /// <remarks>The identifier of the store that owns this record. Store data is isolated by this value.</remarks>
        /// <value>The identifier of the store that owns this record. Store data is isolated by this value.</value>
        public long StoreId { get; set; }
        /// <summary>Gets or sets ContentCulture.</summary>
        /// <remarks>The culture name associated with the localized setting value.</remarks>
        /// <value>The culture name associated with the localized setting value.</value>
        public string ContentCulture { get; set; }
        /// <summary>Gets or sets SettingName.</summary>
        /// <remarks>The serialized store-setting key. This name is part of the persistence contract and must remain stable.</remarks>
        /// <value>The serialized store-setting key. This name is part of the persistence contract and must remain stable.</value>
        public string SettingName { get; set; }
        /// <summary>Gets or sets SettingValue.</summary>
        /// <remarks>The non-localized serialized value for the setting.</remarks>
        /// <value>The non-localized serialized value for the setting.</value>
        public string SettingValue { get; set; }

        /// <summary>Gets or sets LocalizedSettingValue.</summary>
        /// <remarks>The localized serialized value for the setting, using the culture named by <see cref="ContentCulture"/>.</remarks>
        /// <value>The localized serialized value for the setting, using the culture named by <see cref="ContentCulture"/>.</value>
        public string LocalizedSettingValue { get; set; }

        /// <summary>Gets or sets the boolean view of <see cref="SettingValue"/>.</summary>
        /// <value>The serialized value is <c>1</c> when <see langword="true"/> and <c>0</c> when <see langword="false"/>. Any value other than <c>1</c> reads as <see langword="false"/>.</value>
        public bool SettingValueAsBool
        {
            get
            {
                if (SettingValue == ONE)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    SettingValue = ONE;
                else
                    SettingValue = ZERO;
            }
        }

        /// <summary>Gets or sets the localized setting value as an integer.</summary>
        /// <value>The parsed integer, or <c>-1</c> when <see cref="LocalizedSettingValue"/> cannot be parsed.</value>
        public int LocalizedValueAsInteger
        {
            get
            {
                var result = -1;
                int.TryParse(LocalizedSettingValue, out result);
                return result;
            }
            set { LocalizedSettingValue = value.ToString(); }
        }

        /// <summary>Gets or sets the non-localized setting value as an integer.</summary>
        /// <value>The parsed integer, or <c>-1</c> when <see cref="SettingValue"/> cannot be parsed.</value>
        public int ValueAsInteger
        {
            get
            {
                var result = -1;
                int.TryParse(SettingValue, out result);
                return result;
            }
            set { SettingValue = value.ToString(); }
        }

        /// <summary>Gets or sets the non-localized setting value as a 64-bit integer.</summary>
        /// <value>The parsed value, or <c>-1</c> when <see cref="SettingValue"/> cannot be parsed.</value>
        public long ValueAsLong
        {
            get
            {
                long result = -1;
                long.TryParse(SettingValue, out result);
                return result;
            }
            set { SettingValue = value.ToString(); }
        }

        /// <summary>Gets or sets the non-localized setting value as a decimal.</summary>
        /// <value>The invariant-culture parsed value, or <c>-1</c> when the value is empty or cannot be parsed.</value>
        public decimal ValueAsDecimal
        {
            get
            {
                decimal result = -1;
                if (string.IsNullOrEmpty(SettingValue)) return result;
                decimal.TryParse(SettingValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
                return result;
            }
            set { SettingValue = value.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>Gets or sets the non-localized setting value as a <see cref="Guid"/>.</summary>
        /// <value>The parsed identifier, or <see cref="Guid.Empty"/> when the value is empty or cannot be parsed.</value>
        public Guid ValueAsGuid
        {
            get
            {
                var result = Guid.Empty;
                if (string.IsNullOrEmpty(SettingValue)) return Guid.Empty;
                Guid.TryParse(SettingValue, out result);
                return result;
            }
            set { SettingValue = value.ToString(); }
        }

        /// <summary>Gets or sets the non-localized setting value as a Boolean.</summary>
        /// <value>Reads as <see langword="true"/> only for <c>1</c>, <c>T</c>, or <c>TRUE</c>; other values read as <see langword="false"/>. Setters persist <c>1</c> or <c>0</c>.</value>
        public bool ValueAsBool
        {
            get
            {
                var result = false;
                if (SettingValue.Trim().ToUpperInvariant() == ONE)
                {
                    result = true;
                }
                if (SettingValue.Trim().ToUpperInvariant() == T)
                {
                    result = true;
                }
                if (SettingValue.Trim().ToUpperInvariant() == TRUE)
                {
                    result = true;
                }
                return result;
            }
            set
            {
                if (value)
                {
                    SettingValue = ONE;
                }
                else
                {
                    SettingValue = ZERO;
                }
            }
        }
    }
}
