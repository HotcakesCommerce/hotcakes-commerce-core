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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Taxes.Providers
{
    [Serializable]
    public class TaxProviderSettings : Dictionary<string, string>
    {
        private const string ZERO = "0";
        private const string ONE = "1";

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

        public bool GetBoolSetting(string name, bool defaultValue = false)
        {
            if (ContainsKey(name))
            {
                return this[name] == ONE ? true : false;
            }
            return defaultValue;
        }

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

        public void SetBoolSetting(string name, bool value)
        {
            var stringValue = value ? ONE : ZERO;
            AddOrUpdate(name, stringValue);
        }

        public void SetIntSetting(string name, int value)
        {
            AddOrUpdate(name, value.ToString());
        }

        public void Merge(TaxProviderSettings otherSettings)
        {
            foreach (var kv in otherSettings)
            {
                AddOrUpdate(kv.Key, kv.Value);
            }
        }
    }
}