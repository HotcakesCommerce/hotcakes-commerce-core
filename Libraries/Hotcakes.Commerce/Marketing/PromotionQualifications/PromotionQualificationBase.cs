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
            set { _Settings = value; }
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
            return GetSettingArr(key).Select(i => Convert.ToInt32(i)).ToList();
        }

        protected List<string> GetSettingArr(string key)
        {
            var str = GetSetting(key);
            if (!string.IsNullOrEmpty(str))
            {
                return str.Split(',').ToList();
            }
            return new List<string>();
        }

        protected void AddSettingItem(string key, string item)
        {
            AddSettingItems(key, new List<string> {item});
        }

        protected void AddSettingItems(string key, IEnumerable<string> newItems)
        {
            var items = GetSettingArr(key);
            items.AddRange(newItems);
            SetSetting(key, items);
        }

        protected void RemoveSettingItem(string key, string item)
        {
            var items = GetSettingArr(key);
            items.Remove(item);
            SetSetting(key, items);
        }

        protected string GetSetting(string key)
        {
            if (Settings == null) return string.Empty;
            if (!Settings.ContainsKey(key)) return string.Empty;
            var result = Settings[key];
            return result;
        }

        protected int GetSettingAsInt(string key)
        {
            if (Settings == null) return -1;
            var result = GetSetting(key);
            if (result == null) return -1;
            var temp = -1;
            int.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out temp);
            return temp;
        }

        protected decimal GetSettingAsDecimal(string key)
        {
            if (Settings == null) return -1;
            var result = GetSetting(key);
            if (result == null) return -1;
            decimal temp = -1;
            decimal.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out temp);
            return temp;
        }

        protected bool GetSettingAsBool(string key)
        {
            if (Settings == null) return false;
            var result = GetSetting(key);
            if (result == null) return false;
            if (result == ONE) return true;
            return false;
        }

        protected void SetSetting(string key, List<string> ids)
        {
            SetSetting(key, string.Join(",", ids));
        }

        protected void SetSetting(string key, List<int> ids)
        {
            SetSetting(key, string.Join(",", ids));
        }

        protected void SetSetting(string key, string value)
        {
            if (Settings == null) return;
            Settings[key] = value;
        }

        protected void SetSetting(string key, int value)
        {
            if (Settings == null) return;
            Settings[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        protected void SetSetting(string key, decimal value)
        {
            if (Settings == null) return;
            Settings[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        protected void SetSetting(string key, bool value)
        {
            if (Settings == null) return;
            Settings[key] = value ? ONE : ZERO;
        }
    }
}