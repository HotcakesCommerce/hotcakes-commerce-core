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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Hotcakes.Shipping
{
    [Serializable]
    public class ServiceSettings : Dictionary<string, string>, ISerializable
    {
        private const string ZERO = "0";
        private const string ONE = "1";

        public ServiceSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ServiceSettings()
        {
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

        public string GetSettingOrEmpty(string name)
        {
            if (ContainsKey(name))
            {
                return this[name];
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

        public void SetDecimalSetting(string name, decimal value)
        {
            AddOrUpdate(name, value.ToString(CultureInfo.InvariantCulture));
        }

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

        public void SetIntegerSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString(CultureInfo.InvariantCulture));
        }

        public void SetSetting(string name, string value)
        {
            AddOrUpdate(name, value.ToString(CultureInfo.InvariantCulture));
        }
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