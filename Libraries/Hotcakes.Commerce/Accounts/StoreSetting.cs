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
using System.Globalization;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreSetting
    {
        private const string ZERO = "0";
        private const string ONE = "1";
        private const string TRUE = "TRUE";
        private const string T = "T";

        public StoreSetting()
        {
            Id = -1;
            StoreId = -1;
            ContentCulture = string.Empty;
            SettingName = string.Empty;
            SettingValue = string.Empty;
            LocalizedSettingValue = string.Empty;
        }

        public long Id { get; set; }
        public long StoreId { get; set; }
        public string ContentCulture { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }

        public string LocalizedSettingValue { get; set; }

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