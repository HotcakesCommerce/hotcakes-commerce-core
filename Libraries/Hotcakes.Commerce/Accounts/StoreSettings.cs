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
    [Serializable]
    public class StoreSettings
    {
        private readonly Store _Store;

        public StoreSettings(Store store)
        {
            _Store = store;

            Init(new List<StoreSetting>());
        }

        public List<StoreSetting> AllSettings { get; set; }

        public StoreSettingsAnalytics Analytics { get; private set; }
        public StoreSettingsMailServer MailServer { get; private set; }
        public StoreSettingsPayPal PayPal { get; private set; }
        public StoreSettingsFaceBook FaceBook { get; private set; }
        public StoreSettingsTwitter Twitter { get; private set; }
        public StoreSettingsGooglePlus GooglePlus { get; private set; }
        public StoreSettingsPinterest Pinterest { get; private set; }
        public StoreSettingsAvalara Avalara { get; private set; }
        public StoreSettingsUrls Urls { get; private set; }
        public StoreSettingsAdminRoles AdminRoles { get; private set; }
        public StoreSettingsGiftCard GiftCard { get; private set; }

        public bool ApplyVATRules
        {
            get { return GetPropBool(Constants.STORESETTING_APPLYVATRULES); }
            set { SetProp(Constants.STORESETTING_APPLYVATRULES, value); }
        }

        public bool PreserveCartInSession
        {
            get { return GetPropBool(Constants.STORESETTING_PRESERVECARTSESSION); }
            set { SetProp(Constants.STORESETTING_PRESERVECARTSESSION, value); }
        }

        public bool SendAbandonedCartEmails
        {
            get { return GetPropBool(Constants.STORESETTING_SENDABANDONEDEMAILS); }
            set { SetProp(Constants.STORESETTING_SENDABANDONEDEMAILS, value); }
        }

        public int SendAbandonedEmailIn
        {
            get { return GetPropInt(Constants.STORESETTING_SENDABANDONEDINTERVAL, 1); }
            set { SetProp(Constants.STORESETTING_SENDABANDONEDINTERVAL, value); }
        }

        public bool EnableFailedPaymentNotification
        {
            get { return GetPropBool(Constants.ENABLED_FAILED_PAYMENT_NOTIFICATION); }
            set { SetProp(Constants.ENABLED_FAILED_PAYMENT_NOTIFICATION, value); }
        }

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

        public string LogoImage
        {
            get { return GetProp(Constants.STORESETTING_LOGOIMAGE); }
            set { SetProp(Constants.STORESETTING_LOGOIMAGE, value); }
        }

        public bool UseLogoImage
        {
            get { return GetPropBool(Constants.STORESETTING_USELOGOIMAGE); }
            set { SetProp(Constants.STORESETTING_USELOGOIMAGE, value); }
        }

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

        public decimal MinumumOrderAmount
        {
            get { return GetPropDecimal(Constants.STORESETTING_MINIMUMORDERAMOUNT); }
            set { SetProp(Constants.STORESETTING_MINIMUMORDERAMOUNT, value); }
        }

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

        public bool ProductEnableSwatches
        {
            get { return GetPropBool(Constants.STORESETTING_SWATCHEDENABLED); }
            set { SetProp(Constants.STORESETTING_SWATCHEDENABLED, value); }
        }

        public bool MetricsRecordSearches
        {
            get { return GetPropBool(Constants.STORESETTING_METRICSRECORDSEARCHES); }
            set { SetProp(Constants.STORESETTING_METRICSRECORDSEARCHES, value); }
        }

        public bool RequirePhoneNumber
        {
            get { return GetPropBool(Constants.STORESETTING_REQUIREPHONENUMBER); }
            set { SetProp(Constants.STORESETTING_REQUIREPHONENUMBER, value); }
        }

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

        public string QuickbooksOrderAccount
        {
            get { return GetProp(Constants.STORESETTING_QUICKBOOKSORDERACCOUNT); }
            set { SetProp(Constants.STORESETTING_QUICKBOOKSORDERACCOUNT, value); }
        }

        public string QuickbooksShippingAccount
        {
            get { return GetProp(Constants.STORESETTING_QUICKBOOKSSHIPPINGACCOUNT); }
            set { SetProp(Constants.STORESETTING_QUICKBOOKSSHIPPINGACCOUNT, value); }
        }

        // Force SSL for Admin and API
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

        public bool IssuePointsForUserPrice
        {
            get { return GetPropBool(Constants.STORESETTING_ISSUEPOINTSFORUSERPRICE); }
            set { SetProp(Constants.STORESETTING_ISSUEPOINTSFORUSERPRICE, value); }
        }

        public bool UseRewardsPointsForUserPrice
        {
            get { return GetPropBool(Constants.STORESETTING_USEREWARDPOINTSFORUSERPRICE); }
            set { SetProp(Constants.STORESETTING_USEREWARDPOINTSFORUSERPRICE, value); }
        }

        public bool RewardsPointsEnabled
        {
            get { return GetPropBool(Constants.STORESETTING_REWARDPOINTSONPURCHASE); }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSONPURCHASE, value); }
        }

        public bool RewardsPointsOnProductsActive
        {
            get { return GetPropBool(Constants.STORESETTING_REWARDPOINTSONPRODUCTS); }
            set { SetProp(Constants.STORESETTING_REWARDPOINTSONPRODUCTS, value); }
        }

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

        public bool ForceTermsAgreement
        {
            get { return GetPropBool(Constants.STORESETTING_FORCETERMSAGREEMENT); }
            set { SetProp(Constants.STORESETTING_FORCETERMSAGREEMENT, value); }
        }

        public int MaxItemsPerOrder
        {
            get { return GetPropInt(Constants.STORESETTING_MAXITEMSPERORDER); }
            set { SetProp(Constants.STORESETTING_MAXITEMSPERORDER, value); }
        }

        public decimal MaxWeightPerOrder
        {
            get
            {
                var result = GetPropDecimal(Constants.STORESETTING_MAXWEIGHTPERORDER, 9999m);
                return result;
            }
            set { SetProp(Constants.STORESETTING_MAXWEIGHTPERORDER, value); }
        }

        public bool AllowZeroDollarOrders
        {
            get { return GetPropBool(Constants.STORESETTING_ALLOWZERODOLLARORDERS); }
            set { SetProp(Constants.STORESETTING_ALLOWZERODOLLARORDERS, value); }
        }

        public bool AutomaticallyIssueRMANumbers
        {
            get { return GetPropBool(Constants.STORESETTING_AUTOISSUERMANUMBERS); }
            set { SetProp(Constants.STORESETTING_AUTOISSUERMANUMBERS, value); }
        }

        public bool UseChildChoicesAdjustmentsForBundles
        {
            get { return GetPropBool(Constants.STORESETTING_CHILDCHOICESONBUNDLES); }
            set { SetProp(Constants.STORESETTING_CHILDCHOICESONBUNDLES, value); }
        }

        #endregion

        #region Handling

        public decimal HandlingAmount
        {
            get { return GetPropDecimal(Constants.STORESETTING_HANDLINGAMOUNT); }
            set { SetProp(Constants.STORESETTING_HANDLINGAMOUNT, value); }
        }

        public bool HandlingNonShipping
        {
            get { return GetPropBool(Constants.STORESETTING_HANDLINGNONSHIPPING); }
            set { SetProp(Constants.STORESETTING_HANDLINGNONSHIPPING, value); }
        }

        public int HandlingType
        {
            get { return GetPropInt(Constants.STORESETTING_HANDLINGTYPE); }
            set { SetProp(Constants.STORESETTING_HANDLINGTYPE, value); }
        }

        #endregion

        #region TaxProviders

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

        public void TaxProviderSettingsSet(string providerID, TaxProviderSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_TAXPROVIDERSETTING, providerID), encrypted);
        }

        #endregion

        #region Payment

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

        public bool PaymentCreditCardAuthorizeOnly
        {
            get { return GetPropBool(Constants.STORESETTING_CREDITAUTHORIZEONLY); }
            set { SetProp(Constants.STORESETTING_CREDITAUTHORIZEONLY, value); }
        }

        public bool PaymentCreditCardRequireCVV
        {
            get { return GetPropBool(Constants.STORESETTING_CREDITREQUIRECVV); }
            set { SetProp(Constants.STORESETTING_CREDITREQUIRECVV, value); }
        }

        public string PaymentCreditCardGateway
        {
            get { return GetProp(Constants.STORESETTING_CREDITGATEWAY); }
            set { SetProp(Constants.STORESETTING_CREDITGATEWAY, value); }
        }

        public string PaymentReccuringGateway
        {
            get { return GetProp(Constants.STORESETTING_RECURRINGGATEWAY); }
            set { SetProp(Constants.STORESETTING_RECURRINGGATEWAY, value); }
        }

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

        public string GiftCardGateway
        {
            get { return GetProp(Constants.STORESETTING_GIFTCARDGATEWAY); }
            set { SetProp(Constants.STORESETTING_GIFTCARDGATEWAY, value); }
        }

        public bool PaymentGiftCardAuthorizeOnly
        {
            get { return GetPropBool(Constants.STORESETTING_GIFTCARDAUTHORIZEONLY); }
            set { SetProp(Constants.STORESETTING_GIFTCARDAUTHORIZEONLY, value); }
        }

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

        public bool ProductReviewModerate
        {
            get { return GetPropBool(Constants.STORESETTING_REVIEWMODERATE); }
            set { SetProp(Constants.STORESETTING_REVIEWMODERATE, value); }
        }

        public bool AllowProductReviews
        {
            get { return GetPropBool(Constants.STORESETTING_ALLOWREVIEWS); }
            set { SetProp(Constants.STORESETTING_ALLOWREVIEWS, value); }
        }

        #endregion

        #region Shipping

        public string ShippingFedExKey
        {
            get { return GetProp(Constants.STORESETTING_FEDEXKEY); }
            set { SetProp(Constants.STORESETTING_FEDEXKEY, value); }
        }

        public string ShippingFedExPassword
        {
            get { return GetProp(Constants.STORESETTING_FEDEXPASSWORD); }
            set { SetProp(Constants.STORESETTING_FEDEXPASSWORD, value); }
        }

        public string ShippingFedExAccountNumber
        {
            get { return GetProp(Constants.STORESETTING_FEDEXACCOUNT); }
            set { SetProp(Constants.STORESETTING_FEDEXACCOUNT, value); }
        }

        public bool ShippingFedExDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_FEDEXDIAGNOSTICS); }
            set { SetProp(Constants.STORESETTING_FEDEXDIAGNOSTICS, value); }
        }

        public int ShippingFedExDefaultPackaging
        {
            get { return GetPropInt(Constants.STORESETTING_FEDEXDEFAULTPACKAGING); }
            set { SetProp(Constants.STORESETTING_FEDEXDEFAULTPACKAGING, value); }
        }

        public int ShippingFedExDropOffType
        {
            get { return GetPropInt(Constants.STORESETTING_FEDEXDROPOFFTYPE); }
            set { SetProp(Constants.STORESETTING_FEDEXDROPOFFTYPE, value); }
        }

        public bool ShippingFedExForceResidentialRates
        {
            get { return GetPropBool(Constants.STORESETTING_FEDEXFORCERESIDENTIAL); }
            set { SetProp(Constants.STORESETTING_FEDEXFORCERESIDENTIAL, value); }
        }

        public string ShippingFedExMeterNumber
        {
            get { return GetProp(Constants.STORESETTING_FEDEXMETERNUMBER); }
            set { SetProp(Constants.STORESETTING_FEDEXMETERNUMBER, value); }
        }

        public bool ShippingFedExUseDevelopmentServiceUrl
        {
            get { return GetPropBool(Constants.STORESETTING_FEDEXDEVURL); }
            set { SetProp(Constants.STORESETTING_FEDEXDEVURL, value); }
        }

        public string ShippingUpsAccountNumber
        {
            get { return GetProp(Constants.STORESETTING_UPSACCOUNT); }
            set { SetProp(Constants.STORESETTING_UPSACCOUNT, value); }
        }

        public int ShippingUpsDefaultPackaging
        {
            get { return GetPropInt(Constants.STORESETTING_UPSDEFAULTPACKAGING); }
            set { SetProp(Constants.STORESETTING_UPSDEFAULTPACKAGING, value); }
        }

        public int ShippingUpsDefaultPayment
        {
            get { return GetPropInt(Constants.STORESETTING_UPSDEFAULTPAYMENT); }
            set { SetProp(Constants.STORESETTING_UPSDEFAULTPAYMENT, value); }
        }

        public int ShippingUpsDefaultService
        {
            get { return GetPropInt(Constants.STORESETTING_UPSDEFAULTSERVICE); }
            set { SetProp(Constants.STORESETTING_UPSDEFAULTSERVICE, value); }
        }

        public bool ShippingUPSDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_UPSDIAGNOSITCS); }
            set { SetProp(Constants.STORESETTING_UPSDIAGNOSITCS, value); }
        }

        public bool ShippingUPSTestingMode
        {
            get { return GetPropBool(Constants.STORESETTING_UPSTESTINGMODE); }
            set { SetProp(Constants.STORESETTING_UPSTESTINGMODE, value); }
        }

        public bool ShippingUpsForceResidential
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFORCERESIDENTIAL); }
            set { SetProp(Constants.STORESETTING_UPSFORCERESIDENTIAL, value); }
        }

        public int ShippingUpsPickupType
        {
            get { return GetPropInt(Constants.STORESETTING_UPSPICKUPTYPE); }
            set { SetProp(Constants.STORESETTING_UPSPICKUPTYPE, value); }
        }

        public bool ShippingUpsSkipDimensions
        {
            get { return GetPropBool(Constants.STORESETTING_UPSSHIPDIMENSIONS); }
            set { SetProp(Constants.STORESETTING_UPSSHIPDIMENSIONS, value); }
        }

        public string ShippingUpsClientId
        {
            get { return GetProp(Constants.STORESETTING_UPSCLIENTID); }
            set { SetProp(Constants.STORESETTING_UPSCLIENTID, value); }
        }

        public string ShippingUpsClientSecret
        {
            get { return GetProp(Constants.STORESETTING_UPSCLIENTSECRET); }
            set { SetProp(Constants.STORESETTING_UPSCLIENTSECRET, value); }
        }

        public string ShippingUpsLicense
        {
            get { return GetProp(Constants.STORESETTING_UPSLICENSE); }
            set { SetProp(Constants.STORESETTING_UPSLICENSE, value); }
        }

        public string ShippingUpsUsername
        {
            get { return GetProp(Constants.STORESETTING_UPSUSERNAME); }
            set { SetProp(Constants.STORESETTING_UPSUSERNAME, value); }
        }

        public string ShippingUpsPassword
        {
            get { return GetProp(Constants.STORESETTING_UPSPASSWORD); }
            set { SetProp(Constants.STORESETTING_UPSPASSWORD, value); }
        }

        public bool ShippingUPSWriteXML
        {
            get { return GetPropBool(Constants.STORESETTING_UPSWRITEXML); }
            set { SetProp(Constants.STORESETTING_UPSWRITEXML, value); }
        }

        public string ShippingUSPostalUserId
        {
            get { return GetProp(Constants.STORESETTING_USPSUSERID); }
            set { SetProp(Constants.STORESETTING_USPSUSERID, value); }
        }

        public bool ShippingUSPostalDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_USPSDIAGNOSITCS); }
            set { SetProp(Constants.STORESETTING_USPSDIAGNOSITCS, value); }
        }


        #region "UPS Freight"

        public int ShippingUpsFreightDefaultPackaging
        {
            get { return GetPropInt(Constants.STORESETTING_UPSFDEFAULTPACKAGING); }
            set { SetProp(Constants.STORESETTING_UPSFDEFAULTPACKAGING, value); }
        }

        public int ShippingUpsFreightDefaultPayment
        {
            get { return GetPropInt(Constants.STORESETTING_UPSFDEFAULTPAYMENT); }
            set { SetProp(Constants.STORESETTING_UPSFDEFAULTPAYMENT, value); }
        }

        public bool ShippingUPSFreightDiagnostics
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFDIAGNOSTICS); }
            set { SetProp(Constants.STORESETTING_UPSFDIAGNOSTICS, value); }
        }

        public bool ShippingUpsFreightForceResidential
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFFORCERESIDENTIAL); }
            set { SetProp(Constants.STORESETTING_UPSFFORCERESIDENTIAL, value); }
        }

        public bool ShippingUpsFreightSkipDimensions
        {
            get { return GetPropBool(Constants.STORESETTING_UPSFSKIPDIMENSIONS); }
            set { SetProp(Constants.STORESETTING_UPSFSKIPDIMENSIONS, value); }
        }

        public int ShippingUpsFreightBillingOption
        {
            get { return GetPropInt(Constants.STORESETTING_UPSFBILLINGOPTION); }
            set { SetProp(Constants.STORESETTING_UPSFBILLINGOPTION, value); }
        }

        public string ShippingUpsFreightHandleOneUnitType
        {
            get { return GetProp(Constants.STORESETTING_UPSFHANDLEONEUNITTYPE); }
            set { SetProp(Constants.STORESETTING_UPSFHANDLEONEUNITTYPE, value); }
        }

        public string ShippingUpsFreightFreightClass
        {
            get { return GetProp(Constants.STORESETTING_UPSFFREIGHTCLASS); }
            set { SetProp(Constants.STORESETTING_UPSFFREIGHTCLASS, value); }
        }

        #endregion
        #endregion

        #region Affiliates

        public decimal AffiliateCommissionAmount
        {
            get { return GetPropDecimal(Constants.STORESETTING_AFFILIATECOMMISSIONAMOUNT); }
            set { SetProp(Constants.STORESETTING_AFFILIATECOMMISSIONAMOUNT, value); }
        }

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

        public int AffiliateReferralDays
        {
            get { return GetPropInt(Constants.STORESETTING_AFFILIATEREFERRALDAYS); }
            set { SetProp(Constants.STORESETTING_AFFILIATEREFERRALDAYS, value); }
        }

        public bool AffiliateRequireApproval
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATEREQUIREAPPROVAL, true); }
            set { SetProp(Constants.STORESETTING_AFFILIATEREQUIREAPPROVAL, value); }
        }

        public bool AffiliateDisplayChildren
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATEDISPLAYCHILDREN, true); }
            set { SetProp(Constants.STORESETTING_AFFILIATEDISPLAYCHILDREN, value); }
        }

        public bool AffiliateShowIDOnCheckout
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATESHOWIDCHECKOUT, false); }
            set { SetProp(Constants.STORESETTING_AFFILIATESHOWIDCHECKOUT, value); }
        }

        public string AffiliateAgreementText
        {
            get { return GetProp(Constants.STORESETTING_AFFILIATEAGREEMENTTEXT); }
            set { SetProp(Constants.STORESETTING_AFFILIATEAGREEMENTTEXT, value); }
        }

        public bool AffiliateReview
        {
            get { return GetPropBoolWithDefault(Constants.STORESETTING_AFFILIATEREVIEW, false); }
            set { SetProp(Constants.STORESETTING_AFFILIATEREVIEW, value); }
        }

        #endregion

        #region Pay Settings

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

        public void MethodSettingsSet(string methodId, MethodSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_PAYMENTMETHODSETTINGS, methodId), encrypted);
        }

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

        public void PaymentSettingsSet(string ccGatewayId, MethodSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_PAYMENTGATEWAYSETTINGS, ccGatewayId), encrypted);
        }

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

        public void GiftCardSettingsSet(string gcGatewayId, MethodSettings settings)
        {
            var json = Json.ObjectToJson(settings);
            var key = KeyManager.GetKey(0);
            var encrypted = AesEncryption.Encode(json, key);
            SetProp(string.Concat(Constants.STORESETTING_PAYMENTGIFTCARDSETTINGS, gcGatewayId), encrypted);
        }

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


        public string AESKey
        {
            get { return GetProp(Constants.STORESETTING_AESKEY); }
            set { SetProp(Constants.STORESETTING_AESKEY, value); }
        }

        public string AESInitVector
        {
            get { return GetProp(Constants.STORESETTING_AESINITVECTOR); }
            set { SetProp(Constants.STORESETTING_AESINITVECTOR, value); }
        }

        #endregion

        #region Store Protection

        public string AesKey
        {
            get { return GetProp(Constants.STORESETTING_AESKEY); }
            set { SetProp(Constants.STORESETTING_AESKEY, value); }
        }

        public string AesInitVector
        {
            get { return GetProp(Constants.STORESETTING_AESINITVECTOR); }
            set { SetProp(Constants.STORESETTING_AESINITVECTOR, value); }
        }

        public bool StoreClosed
        {
            get { return GetPropBool(Constants.STORESETTING_STORECLOSED); }
            set { SetProp(Constants.STORESETTING_STORECLOSED, value); }
        }

        public string StoreClosedDescription
        {
            get { return GetProp(Constants.STORESETTING_STORECLOSEDDESC); }
            set { SetProp(Constants.STORESETTING_STORECLOSEDDESC, value); }
        }

        #endregion
    }
}