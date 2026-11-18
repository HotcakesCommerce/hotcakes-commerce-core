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
using System.Linq;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Payment;
using Hotcakes.Web;
using Hotcakes.Web.Cryptography;

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>
    /// Provides the authoritative in-memory settings façade for one <see cref="Store" />.
    /// </summary>
    /// <remarks>Members map to serialized store-setting keys and typed values. Setters update <see cref="AllSettings" /> only; the owning store or repository must persist changes. Wrapper properties expose related settings groups for this same store. <seealso cref="StoreSetting" /></remarks>
    [Serializable]
    public class StoreSettings
    {
        private readonly Store _Store;

        /// <summary>
        /// Initializes a settings façade for the specified store.
        /// </summary>
        /// <param name="store">The store that owns the settings represented by this façade (<paramref name="store" />).</param>
        /// <remarks>The constructor starts with an empty in-memory collection and creates wrapper objects; it does not load or persist database settings.</remarks>
        public StoreSettings(Store store)
        {
            _Store = store;

            Init(new List<StoreSetting>());
        }

        /// <summary>
        /// Gets or sets the all settings for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public List<StoreSetting> AllSettings { get; set; }

        /// <summary>
        /// Gets or sets the analytics for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsAnalytics Analytics { get; private set; }
        /// <summary>
        /// Gets or sets the mail server for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsMailServer MailServer { get; private set; }
        /// <summary>
        /// Gets or sets the pay pal for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsPayPal PayPal { get; private set; }
        /// <summary>
        /// Gets or sets the face book for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsFaceBook FaceBook { get; private set; }
        /// <summary>
        /// Gets or sets the twitter for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsTwitter Twitter { get; private set; }
        /// <summary>
        /// Gets or sets the google plus for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsGooglePlus GooglePlus { get; private set; }
        /// <summary>
        /// Gets or sets the pinterest for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsPinterest Pinterest { get; private set; }
        /// <summary>
        /// Gets or sets the avalara for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsAvalara Avalara { get; private set; }
        /// <summary>
        /// Gets or sets the urls for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsUrls Urls { get; private set; }
        /// <summary>
        /// Gets or sets the admin roles for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsAdminRoles AdminRoles { get; private set; }
        /// <summary>
        /// Gets or sets the gift card for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>This member is scoped to the <see cref="Store" /> supplied to the constructor.</remarks>
        public StoreSettingsGiftCard GiftCard { get; private set; }

        /// <summary>
        /// Gets or sets the apply v a t rules for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_APPLYVATRULES</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ApplyVATRules
        {
            get { return GetPropBool(Constants.STORESETTING_APPLYVATRULES); }
            set { SetProp(Constants.STORESETTING_APPLYVATRULES, value); }
        }

        /// <summary>
        /// Gets or sets the preserve cart in session for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_PRESERVECARTSESSION</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool PreserveCartInSession
        {
            get { return GetPropBool(Constants.STORESETTING_PRESERVECARTSESSION); }
            set { SetProp(Constants.STORESETTING_PRESERVECARTSESSION, value); }
        }

        /// <summary>
        /// Gets or sets the send abandoned cart emails for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_SENDABANDONEDEMAILS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool SendAbandonedCartEmails
        {
            get { return GetPropBool(Constants.STORESETTING_SENDABANDONEDEMAILS); }
            set { SetProp(Constants.STORESETTING_SENDABANDONEDEMAILS, value); }
        }

        /// <summary>
        /// Gets or sets the abandoned-cart email interval; an unset value defaults to <c>1</c>.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_SENDABANDONEDINTERVAL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int SendAbandonedEmailIn
        {
            get { return GetPropInt(Constants.STORESETTING_SENDABANDONEDINTERVAL, 1); }
            set { SetProp(Constants.STORESETTING_SENDABANDONEDINTERVAL, value); }
        }

        /// <summary>
        /// Gets or sets the enable failed payment notification for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>ENABLED_FAILED_PAYMENT_NOTIFICATION</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool EnableFailedPaymentNotification
        {
            get { return GetPropBool(Constants.ENABLED_FAILED_PAYMENT_NOTIFICATION); }
            set { SetProp(Constants.ENABLED_FAILED_PAYMENT_NOTIFICATION, value); }
        }

        /// <summary>
        /// Gets or sets the API clear-through date; invalid or missing values fall back to one year before the current local time.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_ALLOWAPICLEARPERIOD</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public DateTime AllowApiToClearUntil
        {
            get
            {
                var result = DateTime.Now.AddYears(-1);
                var prop = GetProp(Constants.STORESETTING_ALLOWAPICLEARPERIOD);
                var temp = DateTime.UtcNow.AddYears(-1);
                if (DateTime.TryParse(prop, out temp))
                {
                    return temp.ToUniversalTime();
                }
                return result;
            }
            set
            {
                var temp = value.ToString("u");
                SetProp(Constants.STORESETTING_ALLOWAPICLEARPERIOD, temp);
            }
        }

        /// <summary>
        /// Gets or sets the logo image for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_LOGOIMAGE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string LogoImage
        {
            get { return GetProp(Constants.STORESETTING_LOGOIMAGE); }
            set { SetProp(Constants.STORESETTING_LOGOIMAGE, value); }
        }

        /// <summary>
        /// Gets or sets the use logo image for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_USELOGOIMAGE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool UseLogoImage
        {
            get { return GetPropBool(Constants.STORESETTING_USELOGOIMAGE); }
            set { SetProp(Constants.STORESETTING_USELOGOIMAGE, value); }
        }

        /// <summary>
        /// Gets or sets the logo text, falling back to the owning store name when empty.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_LOGOTEXT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string LogoText
        {
            get
            {
                var s = GetProp(Constants.STORESETTING_LOGOTEXT);
                if (s == string.Empty)
                {
                    s = _Store.StoreName;
                }
                return s;
            }
            set { SetProp(Constants.STORESETTING_LOGOTEXT, value); }
        }

        /// <summary>
        /// Gets or sets the minumum order amount for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_MINIMUMORDERAMOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public decimal MinumumOrderAmount
        {
            get { return GetPropDecimal(Constants.STORESETTING_MINIMUMORDERAMOUNT); }
            set { SetProp(Constants.STORESETTING_MINIMUMORDERAMOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the store-friendly name, falling back to the owning store name when empty.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FRIENDLYNAME</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string FriendlyName
        {
            get
            {
                var result = GetProp(Constants.STORESETTING_FRIENDLYNAME);
                if (result == string.Empty)
                {
                    result = _Store.StoreName;
                }
                return result;
            }
            set { SetProp(Constants.STORESETTING_FRIENDLYNAME, value); }
        }

        /// <summary>
        /// Gets or sets the logo revision; the getter returns at least <c>1</c>.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_LOGOREVISION</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int LogoRevision
        {
            get
            {
                var result = GetPropInt(Constants.STORESETTING_LOGOREVISION);
                if (result < 1)
                {
                    result = 1;
                }
                return result;
            }
            set { SetProp(Constants.STORESETTING_LOGOREVISION, value); }
        }

        /// <summary>
        /// Gets or sets the product enable swatches for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_SWATCHEDENABLED</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ProductEnableSwatches
        {
            get { return GetPropBool(Constants.STORESETTING_SWATCHEDENABLED); }
            set { SetProp(Constants.STORESETTING_SWATCHEDENABLED, value); }
        }

        /// <summary>
        /// Gets or sets the metrics record searches for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_METRICSRECORDSEARCHES</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool MetricsRecordSearches
        {
            get { return GetPropBool(Constants.STORESETTING_METRICSRECORDSEARCHES); }
            set { SetProp(Constants.STORESETTING_METRICSRECORDSEARCHES, value); }
        }

        /// <summary>
        /// Gets or sets the require phone number for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REQUIREPHONENUMBER</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool RequirePhoneNumber
        {
            get { return GetPropBool(Constants.STORESETTING_REQUIREPHONENUMBER); }
            set { SetProp(Constants.STORESETTING_REQUIREPHONENUMBER, value); }
        }

        /// <summary>
        /// Gets or sets the disabled country iso3 codes for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_COUNTRYISO3CODES</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public List<string> DisabledCountryIso3Codes
        {
            get
            {
                var result = new List<string>();
                var data = GetProp(Constants.STORESETTING_COUNTRYISO3CODES);
                var codes = data.Split(',');
                for (var i = 0; i < codes.Length; i++)
                {
                    result.Add(codes[i]);
                }
                return result;
            }
            set
            {
                var data = string.Empty;
                foreach (var code in value)
                {
                    data += code + Constants.COMMA;
                }
                data = data.TrimEnd(',');
                SetProp(Constants.STORESETTING_COUNTRYISO3CODES, data);
            }
        }

        /// <summary>
        /// Gets or sets the quickbooks order account for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_QUICKBOOKSORDERACCOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string QuickbooksOrderAccount
        {
            get { return GetProp(Constants.STORESETTING_QUICKBOOKSORDERACCOUNT); }
            set { SetProp(Constants.STORESETTING_QUICKBOOKSORDERACCOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the quickbooks shipping account for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_QUICKBOOKSSHIPPINGACCOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string QuickbooksShippingAccount
        {
            get { return GetProp(Constants.STORESETTING_QUICKBOOKSSHIPPINGACCOUNT); }
            set { SetProp(Constants.STORESETTING_QUICKBOOKSSHIPPINGACCOUNT, value); }
        }

        // Force SSL for Admin and API
        /// <summary>
        /// Gets or sets whether SSL is forced for the administration area and API.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FORCEADMINSSL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ForceAdminSSL
        {
            get { return GetPropBool(Constants.STORESETTING_FORCEADMINSSL); }
            set { SetProp(Constants.STORESETTING_FORCEADMINSSL, value); }
        }

        internal void Init(List<StoreSetting> allSettings)
        {
            AllSettings = allSettings;

            Analytics = new StoreSettingsAnalytics(this);
            MailServer = new StoreSettingsMailServer(this);
            PayPal = new StoreSettingsPayPal(this);
            FaceBook = new StoreSettingsFaceBook(this);
            Twitter = new StoreSettingsTwitter(this);
            GooglePlus = new StoreSettingsGooglePlus(this);
            Pinterest = new StoreSettingsPinterest(this);
            Avalara = new StoreSettingsAvalara(this);
            Urls = new StoreSettingsUrls(this);
            AdminRoles = new StoreSettingsAdminRoles(this);
            GiftCard = new StoreSettingsGiftCard(this);
        }

        /// <summary>
        /// Builds the URL for the owning store logo using the current revision and security preference.
        /// </summary>
        /// <param name="app">The value used by this operation.</param>
        /// <param name="isSecure">The value used by this operation.</param>
        /// <remarks>This changes in-memory settings only; persistence occurs through the owning store settings workflow.</remarks>
        public string LogoImageFullUrl(HotcakesApplication app, bool isSecure)
        {
            return DiskStorage.StoreLogoUrl(app, LogoRevision, LogoImage, isSecure);
        }

        #region Setter Helpers

        internal void SetProp(string name, string value, bool localized = false)
        {
            var result = new StoreSetting();
            result.StoreId = _Store.Id;
            result.SettingName = name;
            if (localized)
            {
                result.LocalizedSettingValue = value;
            }
            else
            {
                result.SettingValue = value;
            }
            AddOrUpdateLocalSetting(result, localized);
        }

        internal void SetPropEncrypted(string name, string value)
        {
            var crypto = new TripleDesEncryption();
            var encoded = crypto.Encode(value);
            SetProp(name, encoded);
        }

        internal void SetProp(string name, bool value)
        {
            var result = new StoreSetting();
            result.StoreId = _Store.Id;
            result.SettingName = name;
            result.ValueAsBool = value;
            AddOrUpdateLocalSetting(result);
        }

        internal void SetProp(string name, int value, bool localized = false)
        {
            var result = new StoreSetting();
            result.StoreId = _Store.Id;
            result.SettingName = name;
            if (localized)
            {
                result.LocalizedValueAsInteger = value;
            }
            else
            {
                result.ValueAsInteger = value;
            }
            AddOrUpdateLocalSetting(result, localized);
        }

        internal void SetProp(string name, long value)
        {
            var result = new StoreSetting();
            result.StoreId = _Store.Id;
            result.SettingName = name;
            result.ValueAsLong = value;
            AddOrUpdateLocalSetting(result);
        }

        internal void SetProp(string name, decimal value)
        {
            var result = new StoreSetting();
            result.StoreId = _Store.Id;
            result.SettingName = name;
            result.ValueAsDecimal = value;
            AddOrUpdateLocalSetting(result);
        }

        internal void SetProp(string name, Guid value)
        {
            var result = new StoreSetting();
            result.StoreId = _Store.Id;
            result.SettingName = name;
            result.ValueAsGuid = value;
            AddOrUpdateLocalSetting(result);
        }

        // Keeps local settings synchronized with updates during a single request
        // Does not update database
        /// <summary>
        /// Adds or replaces a setting in the owning store's in-memory collection.
        /// </summary>
        /// <param name="s">The value used by this operation.</param>
        /// <param name="preserveNotLocalizedValue">The value used by this operation.</param>
        /// <remarks>This changes in-memory settings only; persistence occurs through the owning store settings workflow.</remarks>
        public void AddOrUpdateLocalSetting(StoreSetting s, bool preserveNotLocalizedValue = false)
        {
            // Search local settings storage for setting
            var found = AllSettings.Where(y => y.SettingName == s.SettingName).FirstOrDefault();
            if (found == null)
            {
                AllSettings.Add(s);
            }
            else
            {
                s.Id = found.Id; // Set Id so we'll get a database update instead of insert
                if (preserveNotLocalizedValue)
                    s.SettingValue = found.SettingValue;
                AllSettings[AllSettings.IndexOf(found)] = s;
            }
        }

        internal string GetProp(string name, string defValue = "", bool localized = false)
        {
            var s = GetSetting(name);
            var settingVal = s.SettingValue;
            if (localized && !string.IsNullOrEmpty(s.LocalizedSettingValue))
            {
                settingVal = s.LocalizedSettingValue;
            }
            return string.IsNullOrEmpty(settingVal) ? defValue : settingVal;
        }


        internal string GetPropEncrypted(string name)
        {
            var s = GetSetting(name);
            var result = s.SettingValue;
            var crypto = new TripleDesEncryption();
            if (result != string.Empty)
            {
                result = crypto.Decode(result);
            }
            return result;
        }

        internal bool GetPropBool(string name)
        {
            var s = GetSetting(name);
            return s.ValueAsBool;
        }

        internal bool GetPropBoolWithDefault(string name, bool defaultValue)
        {
            var s = GetSetting(name);
            if (s.SettingValue == string.Empty)
            {
                SetProp(name, defaultValue);
                return defaultValue;
            }
            return s.ValueAsBool;
        }

        internal int GetPropInt(string name, bool localized = false)
        {
            var s = GetSetting(name);
            var settingVal = s.ValueAsInteger;
            if (localized && !string.IsNullOrEmpty(s.LocalizedSettingValue))
            {
                settingVal = s.LocalizedValueAsInteger;
            }
            return settingVal;
        }

        internal int GetPropInt(string name, int defaultVal, bool localized = false)
        {
            var s = GetSetting(name);
            var settingVal = defaultVal;
            if (!string.IsNullOrEmpty(s.SettingValue))
            {
                settingVal = s.ValueAsInteger;
            }
            if (localized && !string.IsNullOrEmpty(s.LocalizedSettingValue))
            {
                settingVal = s.LocalizedValueAsInteger;
            }
            return settingVal;
        }

        internal decimal GetPropDecimal(string name)
        {
            var result = GetPropDecimal(name, 0);
            return result;
        }

        internal decimal GetPropDecimal(string name, decimal defaultValue)
        {
            var s = GetSetting(name);
            return string.IsNullOrEmpty(s.SettingValue) ? defaultValue : s.ValueAsDecimal;
        }

        internal Guid GetPropGuid(string name)
        {
            var s = GetSetting(name);
            return s.ValueAsGuid;
        }

        internal long GetPropLong(string name)
        {
            var s = GetSetting(name);
            return s.ValueAsLong;
        }

        internal StoreSetting GetSetting(string name)
        {
            // Search local settings storage for setting
            var s = AllSettings.FirstOrDefault(y => y.SettingName == name);
            if (s == null) s = new StoreSetting();
            return s;
        }

        #endregion

        #region Time and Culture

        /// <summary>
        /// Gets or sets the owning store time zone; an empty stored identifier uses the configured default.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_TIMEZONE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public TimeZoneInfo TimeZone
        {
            get
            {
                var id = GetProp(Constants.STORESETTING_TIMEZONE);
                if (string.IsNullOrEmpty(id))
                {
                    id = Constants.STORESETTING_TIMEZONEDEFAULT;
                }
                var t = TimeZoneInfo.FindSystemTimeZoneById(id);
                return t;
            }
            set
            {
                var id = string.Empty;
                if (value != null)
                    id = value.Id;
                SetProp(Constants.STORESETTING_TIMEZONE, id);
            }
        }

        /// <summary>
        /// Gets or sets the owning store currency culture code, using the configured default when empty.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_CULTURECODE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string CurrencyCultureCode
        {
            get
            {
                var cc = GetProp(Constants.STORESETTING_CULTURECODE);
                if (cc == string.Empty)
                {
                    cc = Constants.STORESETTING_CULTUREDEFAULT;
                }
                return cc;
            }
            set { SetProp(Constants.STORESETTING_CULTURECODE, value); }
        }

        #endregion

        #region Rewards Points

        /// <summary>
        /// Gets or sets the issue points for user price for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_ISSUEPOINTSFORUSERPRICE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool IssuePointsForUserPrice
        {
            get { return GetPropBool(Constants.STORESETTING_ISSUEPOINTSFORUSERPRICE); }
            set { SetProp(Constants.STORESETTING_ISSUEPOINTSFORUSERPRICE, value); }
        }

        /// <summary>
        /// Gets or sets the use rewards points for user price for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_USEREWARDPOINTSFORUSERPRICE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool UseRewardsPointsForUserPrice
        {
            get { return GetPropBool(Constants.STORESETTING_USEREWARDPOINTSFORUSERPRICE); }
            set { SetProp(Constants.STORESETTING_USEREWARDPOINTSFORUSERPRICE, value); }
        }

        /// <summary>
        /// Gets or sets the rewards points enabled for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REWARDPOINTSONPURCHASE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool RewardsPointsEnabled
        {
            get { return GetPropBool(Constants.STORESETTING_REWARDPOINTSONPURCHASE); }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSONPURCHASE, value); }
        }

        /// <summary>
        /// Gets or sets the rewards points on products active for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REWARDPOINTSONPRODUCTS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool RewardsPointsOnProductsActive
        {
            get { return GetPropBool(Constants.STORESETTING_REWARDPOINTSONPRODUCTS); }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSONPRODUCTS, value); }
        }

        /// <summary>
        /// Gets or sets points issued per dollar spent; values below <c>1</c> are returned as <c>1</c>.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REWARDPOINTSPERDOLLARSPENT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int RewardsPointsIssuedPerDollarSpent
        {
            get
            {
                var temp = GetPropInt(Constants.STORESETTING_REWARDPOINTSPERDOLLARSPENT);
                if (temp < 1) temp = 1;
                return temp;
            }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSPERDOLLARSPENT, value); }
        }

        /// <summary>
        /// Gets or sets points needed per dollar of credit; values below <c>1</c> are returned as <c>100</c>.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REWARDPOINTSPERDOLLARCREDIT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int RewardsPointsNeededPerDollarCredit
        {
            get
            {
                var temp = GetPropInt(Constants.STORESETTING_REWARDPOINTSPERDOLLARCREDIT);
                if (temp < 1) temp = 100;
                return temp;
            }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSPERDOLLARCREDIT, value); }
        }

        /// <summary>
        /// Gets or sets the reward-points display name, using the configured default when empty.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REWARDPOINTSNAME</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string RewardsPointsName
        {
            get
            {
                var temp = GetProp(Constants.STORESETTING_REWARDPOINTSNAME);
                if (temp == string.Empty) temp = Constants.STORESETTING_REWARDPOINTSNAMEDEFAULT;
                return temp;
            }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSNAME, value); }
        }

        #endregion

        #region Orders and Checkout

        /// <summary>
        /// Gets or sets the force terms agreement for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FORCETERMSAGREEMENT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ForceTermsAgreement
        {
            get { return GetPropBool(Constants.STORESETTING_FORCETERMSAGREEMENT); }
            set { SetProp(Constants.STORESETTING_FORCETERMSAGREEMENT, value); }
        }

        /// <summary>
        /// Gets or sets the max items per order for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_MAXITEMSPERORDER</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int MaxItemsPerOrder
        {
            get { return GetPropInt(Constants.STORESETTING_MAXITEMSPERORDER); }
            set { SetProp(Constants.STORESETTING_MAXITEMSPERORDER, value); }
        }

        /// <summary>
        /// Gets or sets maximum order weight, defaulting to <c>9999</c> when empty.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_MAXWEIGHTPERORDER</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public decimal MaxWeightPerOrder
        {
            get
            {
                var result = GetPropDecimal(Constants.STORESETTING_MAXWEIGHTPERORDER, 9999m);
                return result;
            }
            set { SetProp(Constants.STORESETTING_MAXWEIGHTPERORDER, value); }
        }

        /// <summary>
        /// Gets or sets the allow zero dollar orders for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_ALLOWZERODOLLARORDERS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AllowZeroDollarOrders
        {
            get { return GetPropBool(Constants.STORESETTING_ALLOWZERODOLLARORDERS); }
            set { SetProp(Constants.STORESETTING_ALLOWZERODOLLARORDERS, value); }
        }

        /// <summary>
        /// Gets or sets the automatically issue r m a numbers for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AUTOISSUERMANUMBERS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AutomaticallyIssueRMANumbers
        {
            get { return GetPropBool(Constants.STORESETTING_AUTOISSUERMANUMBERS); }
            set { SetProp(Constants.STORESETTING_AUTOISSUERMANUMBERS, value); }
        }

        /// <summary>
        /// Gets or sets the use child choices adjustments for bundles for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_CHILDCHOICESONBUNDLES</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool UseChildChoicesAdjustmentsForBundles
        {
            get { return GetPropBool(Constants.STORESETTING_CHILDCHOICESONBUNDLES); }
            set { SetProp(Constants.STORESETTING_CHILDCHOICESONBUNDLES, value); }
        }

        #endregion

        #region Handling

        /// <summary>
        /// Gets or sets the handling amount for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_HANDLINGAMOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public decimal HandlingAmount
        {
            get { return GetPropDecimal(Constants.STORESETTING_HANDLINGAMOUNT); }
            set { SetProp(Constants.STORESETTING_HANDLINGAMOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the handling non shipping for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_HANDLINGNONSHIPPING</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool HandlingNonShipping
        {
            get { return GetPropBool(Constants.STORESETTING_HANDLINGNONSHIPPING); }
            set { SetProp(Constants.STORESETTING_HANDLINGNONSHIPPING, value); }
        }

        /// <summary>
        /// Gets or sets the handling type for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_HANDLINGTYPE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int HandlingType
        {
            get { return GetPropInt(Constants.STORESETTING_HANDLINGTYPE); }
            set { SetProp(Constants.STORESETTING_HANDLINGTYPE, value); }
        }

        #endregion

        #region TaxProviders

        /// <summary>
        /// Gets or sets the tax provider enabled for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_TAXPROVIDER</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string TaxProviderEnabled
        {
            get
            {
                var result = string.Empty;
                result = GetProp(Constants.STORESETTING_TAXPROVIDER);
                return result;
            }
            set { SetProp(Constants.STORESETTING_TAXPROVIDER, value); }
        }

        /// <summary>
        /// Gets encrypted provider or gateway settings for the specified identifier.
        /// </summary>
        /// <param name="providerID">The value used by this operation.</param>
        /// <returns>The decrypted settings, or a new empty settings object when absent or unreadable.</returns>
        /// <remarks>Encryption uses the application key. Exceptions during reading, decryption, or deserialization are intentionally swallowed for compatibility.</remarks>
        public TaxProviderSettings TaxProviderSettingsGet(string providerID)
        {
            try
            {
                var encrypted = GetProp(string.Concat(Constants.STORESETTING_TAXPROVIDERSETTING, providerID));
                if (encrypted.Length > 2)
                {
                    var key = KeyManager.GetKey(0);
                    var json = AesEncryption.Decode(encrypted, key);
                    return Json.ObjectFromJson<TaxProviderSettings>(json);
                }
            }
            catch
            {
            }
            return new TaxProviderSettings();
        }

        /// <summary>
        /// Serializes, encrypts, and stages provider or gateway settings for the owning store.
        /// </summary>
        /// <param name="providerID">The value used by this operation.</param>
        /// <param name="settings">The value used by this operation.</param>
        /// <remarks>This changes in-memory settings only; persistence occurs through the owning store settings workflow.</remarks>
        public void TaxProviderSettingsSet(string providerID, TaxProviderSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_TAXPROVIDERSETTING, providerID), encrypted);
        }

        #endregion

        #region Payment

        /// <summary>
        /// Gets or sets the payment methods enabled for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_PAYMENTMETHODS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public List<string> PaymentMethodsEnabled
        {
            get
            {
                var result = new List<string>();
                var data = GetProp(Constants.STORESETTING_PAYMENTMETHODS);
                var methodIds = data.Split(',');
                for (var i = 0; i < methodIds.Length; i++)
                {
                    result.Add(methodIds[i]);
                }
                return result;
            }
            set
            {
                var data = string.Join(Constants.COMMA, value);
                SetProp(Constants.STORESETTING_PAYMENTMETHODS, data);
            }
        }

        /// <summary>
        /// Gets or sets the payment credit card authorize only for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_CREDITAUTHORIZEONLY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool PaymentCreditCardAuthorizeOnly
        {
            get { return GetPropBool(Constants.STORESETTING_CREDITAUTHORIZEONLY); }
            set { SetProp(Constants.STORESETTING_CREDITAUTHORIZEONLY, value); }
        }

        /// <summary>
        /// Gets or sets the payment credit card require c v v for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_CREDITREQUIRECVV</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool PaymentCreditCardRequireCVV
        {
            get { return GetPropBool(Constants.STORESETTING_CREDITREQUIRECVV); }
            set { SetProp(Constants.STORESETTING_CREDITREQUIRECVV, value); }
        }

        /// <summary>
        /// Gets or sets the payment credit card gateway for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_CREDITGATEWAY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string PaymentCreditCardGateway
        {
            get { return GetProp(Constants.STORESETTING_CREDITGATEWAY); }
            set { SetProp(Constants.STORESETTING_CREDITGATEWAY, value); }
        }

        /// <summary>
        /// Gets or sets the recurring-payment gateway identifier. The misspelling is retained for compatibility.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_RECURRINGGATEWAY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string PaymentReccuringGateway
        {
            get { return GetProp(Constants.STORESETTING_RECURRINGGATEWAY); }
            set { SetProp(Constants.STORESETTING_RECURRINGGATEWAY, value); }
        }

        /// <summary>
        /// Gets or sets accepted card types; an empty or invalid stored list is treated as all supported card types.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_ACCEPTEDCARDS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public List<CardType> PaymentAcceptedCards
        {
            get
            {
                var result = new List<CardType>();

                var data = GetProp(Constants.STORESETTING_ACCEPTEDCARDS);

                if (data == string.Empty) return AllCards();

                var cardIds = data.Split(',');
                for (var i = 0; i < cardIds.Length; i++)
                {
                    var temp = 0;
                    if (int.TryParse(cardIds[i], out temp))
                    {
                        try
                        {
                            result.Add((CardType)temp);
                        }
                        catch
                        {
                            return AllCards();
                        }
                    }
                }
                return result;
            }
            set
            {
                var data = string.Empty;
                foreach (var c in value)
                {
                    data += (int)c + Constants.COMMA;
                }
                data = data.TrimEnd(',');
                SetProp(Constants.STORESETTING_ACCEPTEDCARDS, data);
            }
        }

        /// <summary>
        /// Gets or sets the gift card gateway for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_GIFTCARDGATEWAY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string GiftCardGateway
        {
            get { return GetProp(Constants.STORESETTING_GIFTCARDGATEWAY); }
            set { SetProp(Constants.STORESETTING_GIFTCARDGATEWAY, value); }
        }

        /// <summary>
        /// Gets or sets the payment gift card authorize only for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_GIFTCARDAUTHORIZEONLY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool PaymentGiftCardAuthorizeOnly
        {
            get { return GetPropBool(Constants.STORESETTING_GIFTCARDAUTHORIZEONLY); }
            set { SetProp(Constants.STORESETTING_GIFTCARDAUTHORIZEONLY, value); }
        }

        /// <summary>
        /// Gets or sets whether full credit-card numbers may be displayed; treat this as sensitive information.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_DISPLAYFULLCARDNUMBER</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool DisplayFullCreditCardNumbers
        {
            get { return GetPropBool(Constants.STORESETTING_DISPLAYFULLCARDNUMBER); }
            set { SetProp(Constants.STORESETTING_DISPLAYFULLCARDNUMBER, value); }
        }

        private List<CardType> AllCards()
        {
            var result = new List<CardType>();
            result.Add(CardType.Amex);
            result.Add(CardType.Discover);
            result.Add(CardType.MasterCard);
            result.Add(CardType.Visa);
            return result;
        }

        #endregion

        #region Reviews

        /// <summary>
        /// Gets or sets the displayed product review count, defaulting to <c>3</c> below <c>1</c>.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REVIEWCOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ProductReviewCount
        {
            get
            {
                var temp = GetPropInt(Constants.STORESETTING_REVIEWCOUNT);
                if (temp < 1)
                {
                    temp = 3;
                }
                return temp;
            }
            set { SetProp(Constants.STORESETTING_REVIEWCOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the product review moderate for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_REVIEWMODERATE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ProductReviewModerate
        {
            get { return GetPropBool(Constants.STORESETTING_REVIEWMODERATE); }
            set { SetProp(Constants.STORESETTING_REVIEWMODERATE, value); }
        }

        /// <summary>
        /// Gets or sets the allow product reviews for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_ALLOWREVIEWS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AllowProductReviews
        {
            get { return GetPropBool(Constants.STORESETTING_ALLOWREVIEWS); }
            set { SetProp(Constants.STORESETTING_ALLOWREVIEWS, value); }
        }

        #endregion

        #region Shipping

        /// <summary>
        /// Gets or sets the shipping fed ex key for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXKEY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingFedExKey
        {
            get { return GetProp(Constants.STORESETTING_FEDEXKEY); }
            set { SetProp(Constants.STORESETTING_FEDEXKEY, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex password for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXPASSWORD</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingFedExPassword
        {
            get { return GetProp(Constants.STORESETTING_FEDEXPASSWORD); }
            set { SetProp(Constants.STORESETTING_FEDEXPASSWORD, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex account number for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXACCOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingFedExAccountNumber
        {
            get { return GetProp(Constants.STORESETTING_FEDEXACCOUNT); }
            set { SetProp(Constants.STORESETTING_FEDEXACCOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex diagnostics for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXDIAGNOSTICS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingFedExDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_FEDEXDIAGNOSTICS); }
            set { SetProp(Constants.STORESETTING_FEDEXDIAGNOSTICS, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex default packaging for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXDEFAULTPACKAGING</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingFedExDefaultPackaging
        {
            get { return GetPropInt(Constants.STORESETTING_FEDEXDEFAULTPACKAGING); }
            set { SetProp(Constants.STORESETTING_FEDEXDEFAULTPACKAGING, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex drop off type for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXDROPOFFTYPE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingFedExDropOffType
        {
            get { return GetPropInt(Constants.STORESETTING_FEDEXDROPOFFTYPE); }
            set { SetProp(Constants.STORESETTING_FEDEXDROPOFFTYPE, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex force residential rates for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXFORCERESIDENTIAL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingFedExForceResidentialRates
        {
            get { return GetPropBool(Constants.STORESETTING_FEDEXFORCERESIDENTIAL); }
            set { SetProp(Constants.STORESETTING_FEDEXFORCERESIDENTIAL, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex meter number for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXMETERNUMBER</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingFedExMeterNumber
        {
            get { return GetProp(Constants.STORESETTING_FEDEXMETERNUMBER); }
            set { SetProp(Constants.STORESETTING_FEDEXMETERNUMBER, value); }
        }

        /// <summary>
        /// Gets or sets the shipping fed ex use development service url for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_FEDEXDEVURL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingFedExUseDevelopmentServiceUrl
        {
            get { return GetPropBool(Constants.STORESETTING_FEDEXDEVURL); }
            set { SetProp(Constants.STORESETTING_FEDEXDEVURL, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups account number for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSACCOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsAccountNumber
        {
            get { return GetProp(Constants.STORESETTING_UPSACCOUNT); }
            set { SetProp(Constants.STORESETTING_UPSACCOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups default packaging for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSDEFAULTPACKAGING</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsDefaultPackaging
        {
            get { return GetPropInt(Constants.STORESETTING_UPSDEFAULTPACKAGING); }
            set { SetProp(Constants.STORESETTING_UPSDEFAULTPACKAGING, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups default payment for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSDEFAULTPAYMENT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsDefaultPayment
        {
            get { return GetPropInt(Constants.STORESETTING_UPSDEFAULTPAYMENT); }
            set { SetProp(Constants.STORESETTING_UPSDEFAULTPAYMENT, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups default service for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSDEFAULTSERVICE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsDefaultService
        {
            get { return GetPropInt(Constants.STORESETTING_UPSDEFAULTSERVICE); }
            set { SetProp(Constants.STORESETTING_UPSDEFAULTSERVICE, value); }
        }

        /// <summary>
        /// Gets or sets the shipping u p s diagnostics for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSDIAGNOSITCS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUPSDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_UPSDIAGNOSITCS); }
            set { SetProp(Constants.STORESETTING_UPSDIAGNOSITCS, value); }
        }

        /// <summary>
        /// Gets or sets the shipping u p s testing mode for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSTESTINGMODE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUPSTestingMode
        {
            get { return GetPropBool(Constants.STORESETTING_UPSTESTINGMODE); }
            set { SetProp(Constants.STORESETTING_UPSTESTINGMODE, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups force residential for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFORCERESIDENTIAL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUpsForceResidential
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFORCERESIDENTIAL); }
            set { SetProp(Constants.STORESETTING_UPSFORCERESIDENTIAL, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups pickup type for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSPICKUPTYPE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsPickupType
        {
            get { return GetPropInt(Constants.STORESETTING_UPSPICKUPTYPE); }
            set { SetProp(Constants.STORESETTING_UPSPICKUPTYPE, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups skip dimensions for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSSHIPDIMENSIONS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUpsSkipDimensions
        {
            get { return GetPropBool(Constants.STORESETTING_UPSSHIPDIMENSIONS); }
            set { SetProp(Constants.STORESETTING_UPSSHIPDIMENSIONS, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups client id for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSCLIENTID</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsClientId
        {
            get { return GetProp(Constants.STORESETTING_UPSCLIENTID); }
            set { SetProp(Constants.STORESETTING_UPSCLIENTID, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups client secret for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSCLIENTSECRET</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsClientSecret
        {
            get { return GetProp(Constants.STORESETTING_UPSCLIENTSECRET); }
            set { SetProp(Constants.STORESETTING_UPSCLIENTSECRET, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups license for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSLICENSE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsLicense
        {
            get { return GetProp(Constants.STORESETTING_UPSLICENSE); }
            set { SetProp(Constants.STORESETTING_UPSLICENSE, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups username for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSUSERNAME</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsUsername
        {
            get { return GetProp(Constants.STORESETTING_UPSUSERNAME); }
            set { SetProp(Constants.STORESETTING_UPSUSERNAME, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups password for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSPASSWORD</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsPassword
        {
            get { return GetProp(Constants.STORESETTING_UPSPASSWORD); }
            set { SetProp(Constants.STORESETTING_UPSPASSWORD, value); }
        }

        /// <summary>
        /// Gets or sets the shipping u p s write x m l for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSWRITEXML</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUPSWriteXML
        {
            get { return GetPropBool(Constants.STORESETTING_UPSWRITEXML); }
            set { SetProp(Constants.STORESETTING_UPSWRITEXML, value); }
        }

        /// <summary>
        /// Gets or sets the shipping u s postal user id for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_USPSUSERID</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUSPostalUserId
        {
            get { return GetProp(Constants.STORESETTING_USPSUSERID); }
            set { SetProp(Constants.STORESETTING_USPSUSERID, value); }
        }

        /// <summary>
        /// Gets or sets the shipping u s postal diagnostics for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_USPSDIAGNOSITCS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUSPostalDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_USPSDIAGNOSITCS); }
            set { SetProp(Constants.STORESETTING_USPSDIAGNOSITCS, value); }
        }


        #region "UPS Freight"

        /// <summary>
        /// Gets or sets the shipping ups freight default packaging for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFDEFAULTPACKAGING</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsFreightDefaultPackaging
        {
            get { return GetPropInt(Constants.STORESETTING_UPSFDEFAULTPACKAGING); }
            set { SetProp(Constants.STORESETTING_UPSFDEFAULTPACKAGING, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups freight default payment for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFDEFAULTPAYMENT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsFreightDefaultPayment
        {
            get { return GetPropInt(Constants.STORESETTING_UPSFDEFAULTPAYMENT); }
            set { SetProp(Constants.STORESETTING_UPSFDEFAULTPAYMENT, value); }
        }

        /// <summary>
        /// Gets or sets the shipping u p s freight diagnostics for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFDIAGNOSTICS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUPSFreightDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFDIAGNOSTICS); }
            set { SetProp(Constants.STORESETTING_UPSFDIAGNOSTICS, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups freight force residential for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFFORCERESIDENTIAL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUpsFreightForceResidential
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFFORCERESIDENTIAL); }
            set { SetProp(Constants.STORESETTING_UPSFFORCERESIDENTIAL, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups freight skip dimensions for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFSKIPDIMENSIONS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool ShippingUpsFreightSkipDimensions
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFSKIPDIMENSIONS); }
            set { SetProp(Constants.STORESETTING_UPSFSKIPDIMENSIONS, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups freight billing option for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFBILLINGOPTION</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int ShippingUpsFreightBillingOption
        {
            get { return GetPropInt(Constants.STORESETTING_UPSFBILLINGOPTION); }
            set { SetProp(Constants.STORESETTING_UPSFBILLINGOPTION, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups freight handle one unit type for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFHANDLEONEUNITTYPE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsFreightHandleOneUnitType
        {
            get { return GetProp(Constants.STORESETTING_UPSFHANDLEONEUNITTYPE); }
            set { SetProp(Constants.STORESETTING_UPSFHANDLEONEUNITTYPE, value); }
        }

        /// <summary>
        /// Gets or sets the shipping ups freight freight class for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_UPSFFREIGHTCLASS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string ShippingUpsFreightFreightClass
        {
            get { return GetProp(Constants.STORESETTING_UPSFFREIGHTCLASS); }
            set { SetProp(Constants.STORESETTING_UPSFFREIGHTCLASS, value); }
        }

        #endregion
        #endregion

        #region Affiliates

        /// <summary>
        /// Gets or sets the affiliate commission amount for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATECOMMISSIONAMOUNT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public decimal AffiliateCommissionAmount
        {
            get { return GetPropDecimal(Constants.STORESETTING_AFFILIATECOMMISSIONAMOUNT); }
            set { SetProp(Constants.STORESETTING_AFFILIATECOMMISSIONAMOUNT, value); }
        }

        /// <summary>
        /// Gets or sets the affiliate commission type for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATECOMMISSIONTYPE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public AffiliateCommissionType AffiliateCommissionType
        {
            get
            {
                var result = AffiliateCommissionType.PercentageCommission;
                result = (AffiliateCommissionType)GetPropInt(Constants.STORESETTING_AFFILIATECOMMISSIONTYPE);
                return result;
            }
            set { SetProp(Constants.STORESETTING_AFFILIATECOMMISSIONTYPE, (int)value); }
        }

        /// <summary>
        /// Gets or sets the affiliate conflict mode for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATECONFLICTMODE</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public AffiliateConflictMode AffiliateConflictMode
        {
            get
            {
                var result = AffiliateConflictMode.FavorOldAffiliate;
                result = (AffiliateConflictMode)GetPropInt(Constants.STORESETTING_AFFILIATECONFLICTMODE);
                return result;
            }
            set { SetProp(Constants.STORESETTING_AFFILIATECONFLICTMODE, (int)value); }
        }

        /// <summary>
        /// Gets or sets the affiliate referral days for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATEREFERRALDAYS</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public int AffiliateReferralDays
        {
            get { return GetPropInt(Constants.STORESETTING_AFFILIATEREFERRALDAYS); }
            set { SetProp(Constants.STORESETTING_AFFILIATEREFERRALDAYS, value); }
        }

        /// <summary>
        /// Gets or sets whether affiliate referrals require approval; an unset value defaults to <see langword="true" />.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATEREQUIREAPPROVAL</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AffiliateRequireApproval
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATEREQUIREAPPROVAL, true); }
            set { SetProp(Constants.STORESETTING_AFFILIATEREQUIREAPPROVAL, value); }
        }

        /// <summary>
        /// Gets or sets whether child affiliates are displayed; an unset value defaults to <see langword="true" />.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATEDISPLAYCHILDREN</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AffiliateDisplayChildren
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATEDISPLAYCHILDREN, true); }
            set { SetProp(Constants.STORESETTING_AFFILIATEDISPLAYCHILDREN, value); }
        }

        /// <summary>
        /// Gets or sets whether the affiliate ID is shown at checkout; an unset value defaults to <see langword="false" />.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATESHOWIDCHECKOUT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AffiliateShowIDOnCheckout
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATESHOWIDCHECKOUT, false); }
            set { SetProp(Constants.STORESETTING_AFFILIATESHOWIDCHECKOUT, value); }
        }

        /// <summary>
        /// Gets or sets the affiliate agreement text for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATEAGREEMENTTEXT</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string AffiliateAgreementText
        {
            get { return GetProp(Constants.STORESETTING_AFFILIATEAGREEMENTTEXT); }
            set { SetProp(Constants.STORESETTING_AFFILIATEAGREEMENTTEXT, value); }
        }

        /// <summary>
        /// Gets or sets whether affiliate activity is reviewed; an unset value defaults to <see langword="false" />.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AFFILIATEREVIEW</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool AffiliateReview
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATEREVIEW, false); }
            set { SetProp(Constants.STORESETTING_AFFILIATEREVIEW, value); }
        }

        #endregion

        #region Pay Settings

        /// <summary>
        /// Gets encrypted provider or gateway settings for the specified identifier.
        /// </summary>
        /// <param name="methodId">The value used by this operation.</param>
        /// <returns>The decrypted settings, or a new empty settings object when absent or unreadable.</returns>
        /// <remarks>Encryption uses the application key. Exceptions during reading, decryption, or deserialization are intentionally swallowed for compatibility.</remarks>
        public MethodSettings MethodSettingsGet(string methodId)
        {
            try
            {
                var encrypted = GetProp(string.Concat(Constants.STORESETTING_PAYMENTMETHODSETTINGS, methodId));
                if (encrypted.Length > 2)
                {
                    var key = KeyManager.GetKey(0);
                    var json = AesEncryption.Decode(encrypted, key);
                    return Json.ObjectFromJson<MethodSettings>(json);
                }
            }
            catch
            {
            }
            return new MethodSettings();
        }

        /// <summary>
        /// Serializes, encrypts, and stages provider or gateway settings for the owning store.
        /// </summary>
        /// <param name="methodId">The value used by this operation.</param>
        /// <param name="settings">The value used by this operation.</param>
        /// <remarks>This changes in-memory settings only; persistence occurs through the owning store settings workflow.</remarks>
        public void MethodSettingsSet(string methodId, MethodSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_PAYMENTMETHODSETTINGS, methodId), encrypted);
        }

        /// <summary>
        /// Gets encrypted provider or gateway settings for the specified identifier.
        /// </summary>
        /// <param name="ccGatewayId">The value used by this operation.</param>
        /// <returns>The decrypted settings, or a new empty settings object when absent or unreadable.</returns>
        /// <remarks>Encryption uses the application key. Exceptions during reading, decryption, or deserialization are intentionally swallowed for compatibility.</remarks>
        public MethodSettings PaymentSettingsGet(string ccGatewayId)
        {
            try
            {
                var encrypted = GetProp(string.Concat(Constants.STORESETTING_PAYMENTGATEWAYSETTINGS, ccGatewayId));
                if (encrypted.Length > 2)
                {
                    var key = KeyManager.GetKey(0);
                    var json = AesEncryption.Decode(encrypted, key);
                    return Json.ObjectFromJson<MethodSettings>(json);
                }
            }
            catch
            {
            }
            return new MethodSettings();
        }

        /// <summary>
        /// Serializes, encrypts, and stages provider or gateway settings for the owning store.
        /// </summary>
        /// <param name="ccGatewayId">The value used by this operation.</param>
        /// <param name="settings">The value used by this operation.</param>
        /// <remarks>This changes in-memory settings only; persistence occurs through the owning store settings workflow.</remarks>
        public void PaymentSettingsSet(string ccGatewayId, MethodSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_PAYMENTGATEWAYSETTINGS, ccGatewayId), encrypted);
        }

        /// <summary>
        /// Gets encrypted provider or gateway settings for the specified identifier.
        /// </summary>
        /// <param name="gcGatewayId">The value used by this operation.</param>
        /// <returns>The decrypted settings, or a new empty settings object when absent or unreadable.</returns>
        /// <remarks>Encryption uses the application key. Exceptions during reading, decryption, or deserialization are intentionally swallowed for compatibility.</remarks>
        public MethodSettings GiftCardSettingsGet(string gcGatewayId)
        {
            try
            {
                var encrypted = GetProp(string.Concat(Constants.STORESETTING_PAYMENTGIFTCARDSETTINGS, gcGatewayId));
                if (encrypted.Length > 2)
                {
                    var key = KeyManager.GetKey(0);
                    var json = AesEncryption.Decode(encrypted, key);
                    return Json.ObjectFromJson<MethodSettings>(json);
                }
            }
            catch
            {
            }
            return new MethodSettings();
        }

        /// <summary>
        /// Serializes, encrypts, and stages provider or gateway settings for the owning store.
        /// </summary>
        /// <param name="gcGatewayId">The value used by this operation.</param>
        /// <param name="settings">The value used by this operation.</param>
        /// <remarks>This changes in-memory settings only; persistence occurs through the owning store settings workflow.</remarks>
        public void GiftCardSettingsSet(string gcGatewayId, MethodSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_PAYMENTGIFTCARDSETTINGS, gcGatewayId), encrypted);
        }

        /// <summary>
        /// Resolves the configured gift-card gateway and merges its stored settings.
        /// </summary>
        /// <returns>The configured gateway, or a default <see cref="HccGiftCardGateway" /> when no gateway is found.</returns>
        /// <remarks>Resolution uses <see cref="GiftCardGateways.Find(string)" /> and merges <see cref="GiftCardSettingsGet(string)" />.</remarks>
        public GiftCardGateway PaymentCurrentGiftCardProcessor()
        {
            var gateway = GiftCardGateways.Find(GiftCardGateway);

            if (gateway == null)
            {
                gateway = new HccGiftCardGateway();
            }

            var settings = GiftCardSettingsGet(GiftCardGateway);
            gateway.BaseSettings.Merge(settings);

            return gateway;
        }


        /// <summary>
        /// Gets or sets the legacy AES key alias; it uses the same persisted setting as <see cref="AesKey" />.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AESKEY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string AESKey
        {
            get { return GetProp(Constants.STORESETTING_AESKEY); }
            set { SetProp(Constants.STORESETTING_AESKEY, value); }
        }

        /// <summary>
        /// Gets or sets the legacy AES initialization-vector alias; it uses the same persisted setting as <see cref="AesInitVector" />.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AESINITVECTOR</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string AESInitVector
        {
            get { return GetProp(Constants.STORESETTING_AESINITVECTOR); }
            set { SetProp(Constants.STORESETTING_AESINITVECTOR, value); }
        }

        #endregion

        #region Store Protection

        /// <summary>
        /// Gets or sets the AES key used by store protection.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AESKEY</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string AesKey
        {
            get { return GetProp(Constants.STORESETTING_AESKEY); }
            set { SetProp(Constants.STORESETTING_AESKEY, value); }
        }

        /// <summary>
        /// Gets or sets the AES initialization vector used by store protection.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_AESINITVECTOR</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string AesInitVector
        {
            get { return GetProp(Constants.STORESETTING_AESINITVECTOR); }
            set { SetProp(Constants.STORESETTING_AESINITVECTOR, value); }
        }

        /// <summary>
        /// Gets or sets the store closed for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_STORECLOSED</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public bool StoreClosed
        {
            get { return GetPropBool(Constants.STORESETTING_STORECLOSED); }
            set { SetProp(Constants.STORESETTING_STORECLOSED, value); }
        }

        /// <summary>
        /// Gets or sets the store closed description for the owning store.
        /// </summary>
        /// <value>The current value in the in-memory settings collection.</value>
        /// <remarks>The value is stored under <c>STORESETTING_STORECLOSEDDESC</c>. Assigning a value changes <see cref="AllSettings" /> only; it does not write the database.</remarks>
        public string StoreClosedDescription
        {
            get { return GetProp(Constants.STORESETTING_STORECLOSEDDESC); }
            set { SetProp(Constants.STORESETTING_STORECLOSEDDESC, value); }
        }

        #endregion
    }
}
