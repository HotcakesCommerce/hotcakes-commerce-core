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
using System.Linq;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public abstract class PromotionQualificationBase : IPromotionQualification
    {
        public const string TypeIdAnyProduct = "47B2F15C-137E-4A1C-BAE9-88D0DC1DFF64";
        public const string TypeIdProductBvin = "6B39C94F-1A33-4939-9EC8-BF3EDA744A07";
        public const string TypeIdProductCategory = "3C3132F3-CA5B-4FDE-BF0E-38C82E428096";
        public const string TypeIdProductType = "6BB6ADA9-E296-4C53-9DF7-57C7ABCFE98A";
        public const string TypeIdOrderHasCoupon = "B8B1BF8A-EEB5-4A74-8CCE-7D3C9282BD3D";
        public const string TypeIdAnyOrder = "C8DD095E-F0F4-4A91-9870-823233A2D92B";
        public const string TypeIdOrderHasProducts = "489F961C-8E97-4B78-A5AA-92EAC47BD6F9";
        public const string TypeIdOrderSubTotalIs = "25E02AEA-2FD3-469D-85E5-A6FA0756DDAD";
        public const string TypeIdUserIs = "18A31B99-49E7-43E5-80CA-E1EE64371E1B";
        public const string TypeIdUserIsInGroup = "43A4A2B8-8ECE-4CD2-AA71-BBECC57392A7";
        public const string TypeIdShippingMethodIs = "D9E6B675-1784-4CD2-8041-4D776FE213A7";
        public const string TypeIdAnyShippingMethod = "6453763A-75EA-4FB7-854D-364A5455A68F";
        public const string TypeIdLineItemCategory = "9E33BA5D-C863-45EB-995E-C28349AE26E1";
        private const string ZERO = "0";
        private const string ONE = "1";
        private Dictionary<string, string> _Settings = new Dictionary<string, string>();

        public PromotionQualificationBase()
        {
            Id = 0;
            ProcessingCost = RelativeProcessingCost.Normal;
        }

        public long Id { get; set; }
        public RelativeProcessingCost ProcessingCost { get; set; }
        public abstract Guid TypeId { get; }

        public string CleanTypeId
        {
            get { return TypeId.ToString("D").ToUpperInvariant(); }
        }

        public Dictionary<string, string> Settings
        {
            get { return _Settings; }
            set { _Settings = value ?? new Dictionary<string, string>(); }
        }

        public virtual bool HasOptions
        {
            get { return true; }
        }

        public abstract string FriendlyDescription(HotcakesApplication app);

        public virtual bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            return false;
        }

        protected List<int> GetSettingIds(string key)
        {
            return GetSettingArr(key)
                .Select(s =>
                {
                    int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v);
                    return v;
                })
                .ToList();
        }

        protected List<string> GetSettingArr(string key)
        {
            var str = GetSetting(key);
            if (string.IsNullOrEmpty(str)) return new List<string>();
            return str
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();
        }

        protected void AddSettingItem(string key, string item)
        {
            AddSettingItems(key, new[] { item });
        }

        protected void AddSettingItems(string key, IEnumerable<string> newItems)
        {
            var items = GetSettingArr(key);
            if (newItems != null)
            {
                items.AddRange(newItems.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            }
            SetSetting(key, items);
        }

        protected void RemoveSettingItem(string key, string item)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return;
            }

            var items = GetSettingArr(key);
            var cleaned = item.Trim();
            items.RemoveAll(i => string.Equals(i, cleaned, StringComparison.OrdinalIgnoreCase));
            SetSetting(key, items);
        }

        protected string GetSetting(string key)
        {
            if (Settings == null) return string.Empty;
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (!Settings.TryGetValue(key, out var result)) return string.Empty;
            return result ?? string.Empty;
        }

        protected int GetSettingAsInt(string key)
        {
            var result = GetSetting(key);
            if (string.IsNullOrEmpty(result)) return -1;
            if (int.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp)) return temp;
            return -1;
        }

        protected decimal GetSettingAsDecimal(string key)
        {
            var result = GetSetting(key);
            if (string.IsNullOrEmpty(result)) return -1;
            if (decimal.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp)) return temp;
            return -1;
        }

        protected bool GetSettingAsBool(string key)
        {
            var result = GetSetting(key);
            if (string.IsNullOrEmpty(result)) return false;
            // Preserve original encoding ("1" == true) but accept "true" (case-insensitive) as well.
            if (result == ONE) return true;
            if (bool.TryParse(result, out var b)) return b;
            return false;
        }

        protected void SetSetting(string key, List<string> ids)
        {
            SetSetting(key, ids ?? new List<string>());
        }

        protected void SetSetting(string key, List<int> ids)
        {
            SetSetting(key, ids ?? new List<int>());
        }

        protected void SetSetting(string key, string value)
        {
            if (Settings == null) return;
            if (string.IsNullOrEmpty(key)) return;
            Settings[key] = value ?? string.Empty;
        }

        protected void SetSetting(string key, int value)
        {
            if (Settings == null) return;
            if (string.IsNullOrEmpty(key)) return;
            Settings[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        protected void SetSetting(string key, decimal value)
        {
            if (Settings == null) return;
            if (string.IsNullOrEmpty(key)) return;
            Settings[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        protected void SetSetting(string key, bool value)
        {
            if (Settings == null) return;
            if (string.IsNullOrEmpty(key)) return;
            Settings[key] = value ? ONE : ZERO;
        }

        private void SetSetting(string key, IEnumerable<string> values)
        {
            if (Settings == null) return;
            if (string.IsNullOrEmpty(key)) return;
            if (values == null)
            {
                Settings[key] = string.Empty;
                return;
            }

            var cleaned = values
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToArray();

            Settings[key] = string.Join(",", cleaned);
        }

        private void SetSetting(string key, IEnumerable<int> values)
        {
            if (Settings == null) return;
            if (string.IsNullOrEmpty(key)) return;
            if (values == null)
            {
                Settings[key] = string.Empty;
                return;
            }

            Settings[key] = string.Join(",", values.Select(v => v.ToString(CultureInfo.InvariantCulture)));
        }
    }
}