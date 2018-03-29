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
using System.Linq;
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
            get { return GetPropBool("ApplyVATRules"); }
            set { SetProp("ApplyVATRules", value); }
        }

        public bool PreserveCartInSession
        {
            get { return GetPropBool("PreserveCartInSession"); }
            set { SetProp("PreserveCartInSession", value); }
        }

        public bool SendAbandonedCartEmails
        {
            get { return GetPropBool("SendAbandonedCartEmails"); }
            set { SetProp("SendAbandonedCartEmails", value); }
        }

        public int SendAbandonedEmailIn
        {
            get { return GetPropInt("SendAbandonedEmailIn", 1); }
            set { SetProp("SendAbandonedEmailIn", value); }
        }

        public DateTime AllowApiToClearUntil
        {
            get
            {
                var result = DateTime.Now.AddYears(-1);
                var prop = GetProp("AllowApiToClearUntil");
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
                SetProp("AllowApiToClearUntil", temp);
            }
        }

        public string LogoImage
        {
            get { return GetProp("LogoImage"); }
            set { SetProp("LogoImage", value); }
        }

        public bool UseLogoImage
        {
            get { return GetPropBool("UseLogoImage"); }
            set { SetProp("UseLogoImage", value); }
        }

        public string LogoText
        {
            get
            {
                var s = GetProp("LogoText");
                if (s == string.Empty)
                {
                    s = _Store.StoreName;
                }
                return s;
            }
            set { SetProp("LogoText", value); }
        }

        public decimal MinumumOrderAmount
        {
            get { return GetPropDecimal("MinimumOrderAmount"); }
            set { SetProp("MinimumOrderAmount", value); }
        }

        public string FriendlyName
        {
            get
            {
                var result = GetProp("FriendlyName");
                if (result == string.Empty)
                {
                    result = _Store.StoreName;
                }
                return result;
            }
            set { SetProp("FriendlyName", value); }
        }

        public int LogoRevision
        {
            get
            {
                var result = GetPropInt("LogoRevision");
                if (result < 1)
                {
                    result = 1;
                }
                return result;
            }
            set { SetProp("LogoRevision", value); }
        }

        public bool ProductEnableSwatches
        {
            get { return GetPropBool("ProductEnableSwatches"); }
            set { SetProp("ProductEnableSwatches", value); }
        }

        public bool MetricsRecordSearches
        {
            get { return GetPropBool("MetricsRecordSearches"); }
            set { SetProp("MetricsRecordSearches", value); }
        }

        public List<string> DisabledCountryIso3Codes
        {
            get
            {
                var result = new List<string>();
                var data = GetProp("DisabledCountryIso3Codes");
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
                    data += code + ",";
                }
                data = data.TrimEnd(',');
                SetProp("DisabledCountryIso3Codes", data);
            }
        }

        public string QuickbooksOrderAccount
        {
            get { return GetProp("QuickbooksOrderAccount"); }
            set { SetProp("QuickbooksOrderAccount", value); }
        }

        public string QuickbooksShippingAccount
        {
            get { return GetProp("QuickbooksShippingAccount"); }
            set { SetProp("QuickbooksShippingAccount", value); }
        }

        // Force SSL for Admin and API
        public bool ForceAdminSSL
        {
            get { return GetPropBool("ForceAdminSSL"); }
            set { SetProp("ForceAdminSSL", value); }
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
                var id = GetProp("TimeZone");
                if (string.IsNullOrEmpty(id))
                {
                    id = "Eastern Standard Time";
                }
                var t = TimeZoneInfo.FindSystemTimeZoneById(id);
                return t;
            }
            set
            {
                var id = string.Empty;
                if (value != null)
                    id = value.Id;
                SetProp("TimeZone", id);
            }
        }

        public string CurrencyCultureCode
        {
            get
            {
                var cc = GetProp("CultureCode");
                if (cc == string.Empty)
                {
                    cc = "en-US";
                }
                return cc;
            }
            set { SetProp("CultureCode", value); }
        }

        #endregion

        #region Rewards Points

        public bool IssuePointsForUserPrice
        {
            get { return GetPropBool("IssuePointsForUserPrice"); }
            set { SetProp("IssuePointsForUserPrice", value); }
        }

        public bool UseRewardsPointsForUserPrice
        {
            get { return GetPropBool("UseRewardsPointsForUserPrice"); }
            set { SetProp("UseRewardsPointsForUserPrice", value); }
        }

        public bool RewardsPointsEnabled
        {
            get { return GetPropBool("RewardsPointsOnPurchasesActive"); }
            set { SetProp("RewardsPointsOnPurchasesActive", value); }
        }

        public bool RewardsPointsOnProductsActive
        {
            get { return GetPropBool("RewardsPointsOnProductsActive"); }
            set { SetProp("RewardsPointsOnProductsActive", value); }
        }

        public int RewardsPointsIssuedPerDollarSpent
        {
            get
            {
                var temp = GetPropInt("RewardsPointsIssuedPerDollarSpent");
                if (temp < 1) temp = 1;
                return temp;
            }
            set { SetProp("RewardsPointsIssuedPerDollarSpent", value); }
        }

        public int RewardsPointsNeededPerDollarCredit
        {
            get
            {
                var temp = GetPropInt("RewardsPointsNeededPerDollarCredit");
                if (temp < 1) temp = 100;
                return temp;
            }
            set { SetProp("RewardsPointsNeededPerDollarCredit", value); }
        }

        public string RewardsPointsName
        {
            get
            {
                var temp = GetProp("RewardsPointsName");
                if (temp == string.Empty) temp = "Reward Points";
                return temp;
            }
            set { SetProp("RewardsPointsName", value); }
        }

        #endregion

        #region Orders and Checkout

        public bool ForceTermsAgreement
        {
            get { return GetPropBool("ForceTermsAgreement"); }
            set { SetProp("ForceTermsAgreement", value); }
        }

        public int MaxItemsPerOrder
        {
            get { return GetPropInt("MaxItemsPerOrder"); }
            set { SetProp("MaxItemsPerOrder", value); }
        }

        public decimal MaxWeightPerOrder
        {
            get
            {
                var result = GetPropDecimal("MaxWeightPerOrder", 9999m);
                return result;
            }
            set { SetProp("MaxWeightPerOrder", value); }
        }

        public bool AllowZeroDollarOrders
        {
            get { return GetPropBool("AllowZeroDollarOrders"); }
            set { SetProp("AllowZeroDollarOrders", value); }
        }

        public bool AutomaticallyIssueRMANumbers
        {
            get { return GetPropBool("AutomaticallyIssueRMANumbers"); }
            set { SetProp("AutomaticallyIssueRMANumbers", value); }
        }

        public bool UseChildChoicesAdjustmentsForBundles
        {
            get { return GetPropBool("UseChildChoicesAdjustmentsForBundles"); }
            set { SetProp("UseChildChoicesAdjustmentsForBundles", value); }
        }

        #endregion

        #region Handling

        public decimal HandlingAmount
        {
            get { return GetPropDecimal("HandlingAmount"); }
            set { SetProp("HandlingAmount", value); }
        }

        public bool HandlingNonShipping
        {
            get { return GetPropBool("HandlingNonShipping"); }
            set { SetProp("HandlingNonShipping", value); }
        }

        public int HandlingType
        {
            get { return GetPropInt("HandlingType"); }
            set { SetProp("HandlingType", value); }
        }

        #endregion

        #region TaxProviders

        public string TaxProviderEnabled
        {
            get
            {
                var result = string.Empty;
                result = GetProp("TaxProvidersEnabled");
                return result;
            }
            set { SetProp("TaxProvidersEnabled", value); }
        }

        public TaxProviderSettings TaxProviderSettingsGet(string providerID)
        {
            try
            {
                var encrypted = GetProp("taxprovsetting" + providerID);
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
            SetProp("taxprovsetting" + providerID, encrypted);
        }

        #endregion

        #region Payment

        public List<string> PaymentMethodsEnabled
        {
            get
            {
                var result = new List<string>();
                var data = GetProp("PaymentMethodsEnabled");
                var methodIds = data.Split(',');
                for (var i = 0; i < methodIds.Length; i++)
                {
                    result.Add(methodIds[i]);
                }
                return result;
            }
            set
            {
                var data = string.Join(",", value);
                SetProp("PaymentMethodsEnabled", data);
            }
        }

        public bool PaymentCreditCardAuthorizeOnly
        {
            get { return GetPropBool("PaymentCreditCardAuthorizeOnly"); }
            set { SetProp("PaymentCreditCardAuthorizeOnly", value); }
        }

        public bool PaymentCreditCardRequireCVV
        {
            get { return GetPropBool("PaymentCreditCardRequireCVV"); }
            set { SetProp("PaymentCreditCardRequireCVV", value); }
        }

        public string PaymentCreditCardGateway
        {
            get { return GetProp("PaymentCreditCardGateway"); }
            set { SetProp("PaymentCreditCardGateway", value); }
        }

        public string PaymentReccuringGateway
        {
            get { return GetProp("PaymentReccuringGateway"); }
            set { SetProp("PaymentReccuringGateway", value); }
        }

        public List<CardType> PaymentAcceptedCards
        {
            get
            {
                var result = new List<CardType>();

                var data = GetProp("PaymentAcceptedCards");

                if (data == string.Empty) return AllCards();

                var cardIds = data.Split(',');
                for (var i = 0; i < cardIds.Length; i++)
                {
                    var temp = 0;
                    if (int.TryParse(cardIds[i], out temp))
                    {
                        try
                        {
                            result.Add((CardType) temp);
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
                    data += (int) c + ",";
                }
                data = data.TrimEnd(',');
                SetProp("PaymentAcceptedCards", data);
            }
        }

        public string GiftCardGateway
        {
            get { return GetProp("GiftCardGateway"); }
            set { SetProp("GiftCardGateway", value); }
        }

        public bool PaymentGiftCardAuthorizeOnly
        {
            get { return GetPropBool("PaymentGiftCardAuthorizeOnly"); }
            set { SetProp("PaymentGiftCardAuthorizeOnly", value); }
        }

        public bool DisplayFullCreditCardNumbers
        {
            get { return GetPropBool("PaymentDisplayFullCreditCardNumbers"); }
            set { SetProp("PaymentDisplayFullCreditCardNumbers", value); }
        }

        private List<CardType> AllCards()
        {
            var result = new List<CardType>();
            result.Add(CardType.Amex);
            //result.Add(CardType.DinersClub);
            result.Add(CardType.Discover);
            //result.Add(CardType.JCB);
            //result.Add(CardType.Maestro);
            result.Add(CardType.MasterCard);
            //result.Add(CardType.Solo);
            //result.Add(CardType.Switch);
            result.Add(CardType.Visa);
            return result;
        }

        #endregion

        #region Reviews

        public int ProductReviewCount
        {
            get
            {
                var temp = GetPropInt("ProductReviewCount");
                if (temp < 1)
                {
                    temp = 3;
                }
                return temp;
            }
            set { SetProp("ProductReviewCount", value); }
        }

        public bool ProductReviewModerate
        {
            get { return GetPropBool("ProductReviewModerate"); }
            set { SetProp("ProductReviewModerate", value); }
        }

        public bool AllowProductReviews
        {
            get { return GetPropBool("AllowProductReviews"); }
            set { SetProp("AllowProductReviews", value); }
        }

        #endregion

        #region Shipping

        public string ShippingFedExKey
        {
            get { return GetProp("ShippingFedExKey"); }
            set { SetProp("ShippingFedExKey", value); }
        }

        public string ShippingFedExPassword
        {
            get { return GetProp("ShippingFedExPassword"); }
            set { SetProp("ShippingFedExPassword", value); }
        }

        public string ShippingFedExAccountNumber
        {
            get { return GetProp("ShippingFedExAccountNumber"); }
            set { SetProp("ShippingFedExAccountNumber", value); }
        }

        public bool ShippingFedExDiagnostics
        {
            get { return GetPropBool("ShippingFedExDiagnostics"); }
            set { SetProp("ShippingFedExDiagnostics", value); }
        }

        public int ShippingFedExDefaultPackaging
        {
            get { return GetPropInt("ShippingFedExDefaultPackaging"); }
            set { SetProp("ShippingFedExDefaultPackaging", value); }
        }

        public int ShippingFedExDropOffType
        {
            get { return GetPropInt("ShippingFedExDropOffType"); }
            set { SetProp("ShippingFedExDropOffType", value); }
        }

        public bool ShippingFedExForceResidentialRates
        {
            get { return GetPropBool("ShippingFedExForceResidentialRates"); }
            set { SetProp("ShippingFedExForceResidentialRates", value); }
        }

        public string ShippingFedExMeterNumber
        {
            get { return GetProp("ShippingFedExMeterNumber"); }
            set { SetProp("ShippingFedExMeterNumber", value); }
        }

        public bool ShippingFedExUseDevelopmentServiceUrl
        {
            get { return GetPropBool("ShippingFedExUseDevelopmentServiceUrl"); }
            set { SetProp("ShippingFedExUseDevelopmentServiceUrl", value); }
        }

        public string ShippingUpsAccountNumber
        {
            get { return GetProp("ShippingUpsAccountNumber"); }
            set { SetProp("ShippingUpsAccountNumber", value); }
        }

        public int ShippingUpsDefaultPackaging
        {
            get { return GetPropInt("ShippingUpsDefaultPackaging"); }
            set { SetProp("ShippingUpsDefaultPackaging", value); }
        }

        public int ShippingUpsDefaultPayment
        {
            get { return GetPropInt("ShippingUpsDefaultPayment"); }
            set { SetProp("ShippingUpsDefaultPayment", value); }
        }

        public int ShippingUpsDefaultService
        {
            get { return GetPropInt("ShippingUpsDefaultService"); }
            set { SetProp("ShippingUpsDefaultService", value); }
        }

        public bool ShippingUPSDiagnostics
        {
            get { return GetPropBool("ShippingUPSDiagnostics"); }
            set { SetProp("ShippingUPSDiagnostics", value); }
        }

        public bool ShippingUpsForceResidential
        {
            get { return GetPropBool("ShippingUpsForceResidential"); }
            set { SetProp("ShippingUpsForceResidential", value); }
        }

        public string ShippingUpsLicense
        {
            get { return GetProp("Shipping_UPS_License"); }
            set { SetProp("Shipping_UPS_License", value); }
        }

        public string ShippingUpsPassword
        {
            get { return GetProp("Shipping_UPS_Password"); }
            set { SetProp("Shipping_UPS_Password", value); }
        }

        public int ShippingUpsPickupType
        {
            get { return GetPropInt("Shipping_UPS_Pickup_Type"); }
            set { SetProp("Shipping_UPS_Pickup_Type", value); }
        }

        public bool ShippingUpsSkipDimensions
        {
            get { return GetPropBool("ShippingUpsSkipDimensions"); }
            set { SetProp("ShippingUpsSkipDimensions", value); }
        }

        public string ShippingUpsUsername
        {
            get { return GetProp("Shipping_UPS_Username"); }
            set { SetProp("Shipping_UPS_Username", value); }
        }

        public bool ShippingUPSWriteXML
        {
            get { return GetPropBool("ShippingUPSWriteXML"); }
            set { SetProp("ShippingUPSWriteXML", value); }
        }

        public string ShippingUSPostalUserId
        {
            get { return GetProp("ShippingUSPostalUserId"); }
            set { SetProp("ShippingUSPostalUserId", value); }
        }

        public bool ShippingUSPostalDiagnostics
        {
            get { return GetPropBool("ShippingUSPostalDiagnostics"); }
            set { SetProp("ShippingUSPostalDiagnostics", value); }
        }


        #region "UPS Freight"

        public int ShippingUpsFreightDefaultPackaging
        {
            get { return GetPropInt("ShippingUpsFreightDefaultPackaging"); }
            set { SetProp("ShippingUpsFreightDefaultPackaging", value); }
        }

        public int ShippingUpsFreightDefaultPayment
        {
            get { return GetPropInt("ShippingUpsFreightDefaultPayment"); }
            set { SetProp("ShippingUpsFreightDefaultPayment", value); }
        }

        public bool ShippingUPSFreightDiagnostics
        {
            get { return GetPropBool("ShippingUPSFreightDiagnostics"); }
            set { SetProp("ShippingUPSFreightDiagnostics", value); }
        }

        public bool ShippingUpsFreightForceResidential
        {
            get { return GetPropBool("ShippingUpsFreightForceResidential"); }
            set { SetProp("ShippingUpsFreightForceResidential", value); }
        }

        public bool ShippingUpsFreightSkipDimensions
        {
            get { return GetPropBool("ShippingUpsFreightSkipDimensions"); }
            set { SetProp("ShippingUpsFreightSkipDimensions", value); }
        }

        public int ShippingUpsFreightBillingOption
        {
            get { return GetPropInt("ShippingUpsFreightBillingOption"); }
            set { SetProp("ShippingUpsFreightBillingOption", value); }
        }

        public string ShippingUpsFreightHandleOneUnitType
        {
            get { return GetProp("ShippingUpsFreightHandleOneUnitType"); }
            set { SetProp("ShippingUpsFreightHandleOneUnitType", value); }
        }

        public string ShippingUpsFreightFreightClass
        {
            get { return GetProp("ShippingUpsFreightFreightClass"); }
            set { SetProp("ShippingUpsFreightFreightClass", value); }
        }

        #endregion
        #endregion

        #region Affiliates

        public decimal AffiliateCommissionAmount
        {
            get { return GetPropDecimal("AffiliateCommissionAmount"); }
            set { SetProp("AffiliateCommissionAmount", value); }
        }

        public AffiliateCommissionType AffiliateCommissionType
        {
            get
            {
                var result = AffiliateCommissionType.PercentageCommission;
                result = (AffiliateCommissionType) GetPropInt("AffiliateCommissionType");
                return result;
            }
            set { SetProp("AffiliateCommissionType", (int) value); }
        }

        public AffiliateConflictMode AffiliateConflictMode
        {
            get
            {
                var result = AffiliateConflictMode.FavorOldAffiliate;
                result = (AffiliateConflictMode) GetPropInt("AffiliateConflictMode");
                return result;
            }
            set { SetProp("AffiliateConflictMode", (int) value); }
        }

        public int AffiliateReferralDays
        {
            get { return GetPropInt("AffiliateReferralDays"); }
            set { SetProp("AffiliateReferralDays", value); }
        }

        public bool AffiliateRequireApproval
        {
            get { return GetPropBoolWithDefault("AffiliateRequireApproval", true); }
            set { SetProp("AffiliateRequireApproval", value); }
        }

        public bool AffiliateDisplayChildren
        {
            get { return GetPropBoolWithDefault("AffiliateDisplayChildren", true); }
            set { SetProp("AffiliateDisplayChildren", value); }
        }

        public bool AffiliateShowIDOnCheckout
        {
            get { return GetPropBoolWithDefault("AffiliateShowIDOnCheckout", false); }
            set { SetProp("AffiliateShowIDOnCheckout", value); }
        }

        public string AffiliateAgreementText
        {
            get { return GetProp("AffiliatAgreementText"); }
            set { SetProp("AffiliatAgreementText", value); }
        }

        public bool AffiliateReview
        {
            get { return GetPropBoolWithDefault("AffiliateReview", false); }
            set { SetProp("AffiliateReview", value); }
        }

        #endregion

        #region Pay Settings

        public MethodSettings MethodSettingsGet(string methodId)
        {
            try
            {
                var encrypted = GetProp("methodsettings" + methodId);
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
            SetProp("methodsettings" + methodId, encrypted);
        }

        public MethodSettings PaymentSettingsGet(string ccGatewayId)
        {
            try
            {
                var encrypted = GetProp("paysettings" + ccGatewayId);
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
            SetProp("paysettings" + ccGatewayId, encrypted);
        }

        public MethodSettings GiftCardSettingsGet(string gcGatewayId)
        {
            try
            {
                var encrypted = GetProp("gcpaysettings" + gcGatewayId);
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
            SetProp("gcpaysettings" + gcGatewayId, encrypted);
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

        #endregion

        #region Store Protection

        public bool StoreClosed
        {
            get { return GetPropBool("StoreClosed"); }
            set { SetProp("StoreClosed", value); }
        }

        public string StoreClosedDescription
        {
            get { return GetProp("StoreClosedDescription"); }
            set { SetProp("StoreClosedDescription", value); }
        }

        #endregion
    }
}