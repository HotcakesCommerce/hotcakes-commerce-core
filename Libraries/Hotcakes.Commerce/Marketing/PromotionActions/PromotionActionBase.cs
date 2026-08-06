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

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public abstract class PromotionActionBase : IPromotionAction
    {
        private Dictionary<string, string> _Settings = new Dictionary<string, string>();

        private const string ZERO = "0";
        private const string ONE = "1";
            
        public PromotionActionBase()
        {
            Id = 0;
        }

        public long Id { get; set; }
        public abstract Guid TypeId { get; }

        public Dictionary<string, string> Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }

        public abstract string FriendlyDescription(HotcakesApplication app);

        public virtual bool ApplyAction(PromotionContext context)
        {
            return false;
        }

        public virtual bool CancelAction(PromotionContext context)
        {
            return true;
        }

        protected string GetSetting(string key)
        {
            var s = Settings;
            if (s == null) return string.Empty;
            return s.TryGetValue(key, out var result) ? (result ?? string.Empty) : string.Empty;
        }

        protected int GetSettingAsInt(string key)
        {
            var s = Settings;
            if (s == null) return -1;
            if (!s.TryGetValue(key, out var result) || string.IsNullOrEmpty(result)) return -1;
            return int.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : -1;
        }

        protected decimal GetSettingAsDecimal(string key)
        {
            var s = Settings;
            if (s == null) return -1;
            if (!s.TryGetValue(key, out var result) || string.IsNullOrEmpty(result)) return -1;
            return decimal.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : -1;
        }

        protected bool GetSettingAsBool(string key)
        {
            var s = Settings;
            if (s == null) return false;
            if (!s.TryGetValue(key, out var result)) return false;
            return string.Equals(result, ONE, StringComparison.Ordinal);
        }

        protected void SetSetting(string key, string value)
        {
            var s = Settings;
            if (s == null) return;
            s[key] = value;
        }

        protected void SetSetting(string key, int value)
        {
            var s = Settings;
            if (s == null) return;
            s[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        protected void SetSetting(string key, decimal value)
        {
            var s = Settings;
            if (s == null) return;
            s[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        protected void SetSetting(string key, bool value)
        {
            if (Settings == null) return;
            Settings[key] = value ? ONE : ZERO;
        }
    }
}