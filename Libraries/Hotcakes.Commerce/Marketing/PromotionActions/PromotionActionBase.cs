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