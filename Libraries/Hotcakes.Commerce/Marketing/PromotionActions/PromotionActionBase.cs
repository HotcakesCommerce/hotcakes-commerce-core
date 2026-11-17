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
    /// <summary>
    /// Provides shared persistence and setting conversion behavior for promotion actions.
    /// </summary>
    /// <remarks>
    /// Settings are stored as invariant-culture strings. The default application behavior returns
    /// <see langword="false"/> and the default cancellation behavior returns <see langword="true"/>;
    /// concrete actions override these operations when they have an effect to apply or undo.
    /// </remarks>
    public abstract class PromotionActionBase : IPromotionAction
    {
        private Dictionary<string, string> _Settings = new Dictionary<string, string>();

        private const string ZERO = "0";
        private const string ONE = "1";
            
        /// <summary>Initializes an unsaved promotion action.</summary>
        public PromotionActionBase()
        {
            Id = 0;
        }

        /// <summary>Gets or sets the persisted action identifier.</summary>
        public long Id { get; set; }

        /// <summary>Gets the stable serialized identifier for the concrete action type.</summary>
        public abstract Guid TypeId { get; }

        /// <summary>Gets or sets the persisted string settings for the action.</summary>
        public Dictionary<string, string> Settings
        {
            get { return _Settings; }
            set { _Settings = value; }
        }

        /// <summary>Gets an HTML description of the concrete promotion action.</summary>
        /// <param name="app">The application context used to resolve display information.</param>
        /// <returns>The action description.</returns>
        public abstract string FriendlyDescription(HotcakesApplication app);

        /// <summary>Applies the concrete action; the base implementation performs no action.</summary>
        /// <param name="context">The promotion evaluation context.</param>
        /// <returns><see langword="false"/> because no effect is applied by the base implementation.</returns>
        public virtual bool ApplyAction(PromotionContext context)
        {
            return false;
        }

        /// <summary>Cancels the concrete action; the base implementation has nothing to undo.</summary>
        /// <param name="context">The promotion evaluation context.</param>
        /// <returns><see langword="true"/> because there is no base effect to roll back.</returns>
        public virtual bool CancelAction(PromotionContext context)
        {
            return true;
        }

        /// <summary>Reads a setting, returning an empty string when it is absent or null.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <returns>The setting value or an empty string.</returns>
        protected string GetSetting(string key)
        {
            var s = Settings;
            if (s == null) return string.Empty;
            return s.TryGetValue(key, out var result) ? (result ?? string.Empty) : string.Empty;
        }

        /// <summary>Reads a setting as an invariant-culture integer.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <returns>The parsed value, or <c>-1</c> when missing or invalid.</returns>
        protected int GetSettingAsInt(string key)
        {
            var s = Settings;
            if (s == null) return -1;
            if (!s.TryGetValue(key, out var result) || string.IsNullOrEmpty(result)) return -1;
            return int.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : -1;
        }

        /// <summary>Reads a setting as an invariant-culture decimal.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <returns>The parsed value, or <c>-1</c> when missing or invalid.</returns>
        protected decimal GetSettingAsDecimal(string key)
        {
            var s = Settings;
            if (s == null) return -1;
            if (!s.TryGetValue(key, out var result) || string.IsNullOrEmpty(result)) return -1;
            return decimal.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp) ? temp : -1;
        }

        /// <summary>Reads a Boolean setting encoded as <c>1</c>.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <returns><see langword="true"/> only when the stored value is exactly <c>1</c>.</returns>
        protected bool GetSettingAsBool(string key)
        {
            var s = Settings;
            if (s == null) return false;
            if (!s.TryGetValue(key, out var result)) return false;
            return string.Equals(result, ONE, StringComparison.Ordinal);
        }

        /// <summary>Stores a string setting without changing its key or value.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <param name="value">The value to store.</param>
        protected void SetSetting(string key, string value)
        {
            var s = Settings;
            if (s == null) return;
            s[key] = value;
        }

        /// <summary>Stores an integer setting using invariant-culture formatting.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <param name="value">The value to store.</param>
        protected void SetSetting(string key, int value)
        {
            var s = Settings;
            if (s == null) return;
            s[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Stores a decimal setting using invariant-culture formatting.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <param name="value">The value to store.</param>
        protected void SetSetting(string key, decimal value)
        {
            var s = Settings;
            if (s == null) return;
            s[key] = value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>Stores a Boolean setting as <c>1</c> or <c>0</c>.</summary>
        /// <param name="key">The persisted setting key.</param>
        /// <param name="value">The value to store.</param>
        protected void SetSetting(string key, bool value)
        {
            if (Settings == null) return;
            Settings[key] = value ? ONE : ZERO;
        }
    }
}