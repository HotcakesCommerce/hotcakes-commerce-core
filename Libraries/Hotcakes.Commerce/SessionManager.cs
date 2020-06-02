#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Web;
using System.Web.SessionState;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Licensing.Common.DataContracts;
using Hotcakes.Web;

namespace Hotcakes.Commerce
{
    public class SessionManager
    {
        private const string CategoryLastIdVariable = "HccCategoryLastVisited";

        // Admin Settings
        private const string _AdminProductSearchCriteriaKeyword = "HccAdminProductSearchCriteriaKeyword";
        private const string _AdminProductSearchCriteriaManufacturer = "HccAdminProductSearchCriteriaManufacturer";
        private const string _AdminProductSearchCriteriaVendor = "HccAdminProductSearchCriteriaVendor";
        private const string _AdminProductSearchCriteriaCategory = "HccAdminProductSearchCriteriaCategory";
        private const string _AdminProductSearchCriteriaStatus = "HccAdminProductSearchCriteriaStatus";
        private const string _AdminProductSearchCriteriaInventoryStatus = "HccAdminProductSearchCriteriaInventoryStatus";
        private const string _AdminProductSearchCriteriaProductType = "HccAdminProductSearchCriteriaProductType";
        private const string _AdminProductImportProgress = "HccAdminProductImportProgress";
        private const string _AdminProductImportLog = "HccAdminProductImportLog";

        private const string _AdminOrderSearchKeyword = "HccAdminOrderSearchKeyword";
        private const string _AdminOrderSearchPaymentFilter = "HccAdminOrderSearchPaymentFilter";
        private const string _AdminOrderSearchShippingFilter = "HccAdminOrderSearchShippingFilter";
        private const string _AdminOrderSearchStatusFilter = "HccAdminOrderSearchStatusFilter";
        private const string _AdminOrderSearchDateRange = "HccAdminOrderSearchDateRange";
        private const string _AdminOrderSearchStartDate = "HccAdminOrderSearchStartDate";
        private const string _AdminOrderSearchEndDate = "HccAdminOrderSearchEndDate";
        private const string _AdminOrderSearchLastPage = "HccAdminOrderSearchLastPage";
        private const string _AdminOrderSearchNewestFirst = "HccAdminOrderSearchNewestFirst";

        private const string _AdminPromotionKeywords = "HccAdminPromotionKeywords";
        private const string _AdminPromotionShowDisabled = "HccAdminPromotionShowDisabled";
        private const string _AnalyticsOrderId = "HccAnalyticsOrderId";
        private const string _AdminCurrentLanguage = "HccAdminCurrentLanguage";

        private const string _AdminVendorKeywords = "HccAdminVendorKeywords";

        private const string _AdminManufacturerKeywords = "HccAdminManufacturerKeywords";

        private const string _AdminCustomerKeywords = "HccAdminCustomerKeywords";
        private readonly HttpSessionState _session;

        public static HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        public SessionManager(HttpSessionState session)
        {
            _session = session;
        }

        public static string AdminCurrentLanguage
        {
            get { return GetCookieString(_AdminCurrentLanguage); }
            set { SetCookieString(_AdminCurrentLanguage, value); }
        }

        public static string CategoryLastId
        {
            get { return GetSessionString(CategoryLastIdVariable); }
            set { SetSessionString(CategoryLastIdVariable, value); }
        }

        // Admin Settings

        public static string AdminProductCriteriaKeyword
        {
            get { return GetSessionString(_AdminProductSearchCriteriaKeyword); }
            set { SetSessionString(_AdminProductSearchCriteriaKeyword, value); }
        }

        public static string AdminProductCriteriaManufacturer
        {
            get { return GetSessionString(_AdminProductSearchCriteriaManufacturer); }
            set { SetSessionString(_AdminProductSearchCriteriaManufacturer, value); }
        }

        public static string AdminProductCriteriaVendor
        {
            get { return GetSessionString(_AdminProductSearchCriteriaVendor); }
            set { SetSessionString(_AdminProductSearchCriteriaVendor, value); }
        }

        public static string AdminProductCriteriaCategory
        {
            get { return GetSessionString(_AdminProductSearchCriteriaCategory); }
            set { SetSessionString(_AdminProductSearchCriteriaCategory, value); }
        }

        public static string AdminProductCriteriaStatus
        {
            get { return GetSessionString(_AdminProductSearchCriteriaStatus); }
            set { SetSessionString(_AdminProductSearchCriteriaStatus, value); }
        }

        public static string AdminProductCriteriaInventoryStatus
        {
            get { return GetSessionString(_AdminProductSearchCriteriaInventoryStatus); }
            set { SetSessionString(_AdminProductSearchCriteriaInventoryStatus, value); }
        }

        public static string AdminProductCriteriaProductType
        {
            get { return GetSessionString(_AdminProductSearchCriteriaProductType); }
            set { SetSessionString(_AdminProductSearchCriteriaProductType, value); }
        }

        public List<string> AdminProductImportLog
        {
            get { return (List<string>) GetSessionObjectInt(_AdminProductImportLog) ?? new List<string>(); }
            set { SetSessionObjectInt(_AdminProductImportLog, value); }
        }

        public double AdminProductImportProgress
        {
            get { return (double?) GetSessionObjectInt(_AdminProductImportProgress) ?? 0; }
            set { SetSessionObjectInt(_AdminProductImportProgress, value); }
        }

        public static string AdminOrderSearchKeyword
        {
            get { return GetSessionString(_AdminOrderSearchKeyword); }
            set { SetSessionString(_AdminOrderSearchKeyword, value); }
        }

        public static string AdminOrderSearchPaymentFilter
        {
            get { return GetSessionString(_AdminOrderSearchPaymentFilter); }
            set { SetSessionString(_AdminOrderSearchPaymentFilter, value); }
        }

        public static string AdminOrderSearchShippingFilter
        {
            get { return GetSessionString(_AdminOrderSearchShippingFilter); }
            set { SetSessionString(_AdminOrderSearchShippingFilter, value); }
        }

        public static string AdminOrderSearchStatusFilter
        {
            get { return GetSessionString(_AdminOrderSearchStatusFilter); }
            set { SetSessionString(_AdminOrderSearchStatusFilter, value); }
        }

        public static DateRangeType AdminOrderSearchDateRange
        {
            get
            {
                var result = DateRangeType.AllDates;
                if (GetSessionObject(_AdminOrderSearchDateRange) != null)
                {
                    result = (DateRangeType) GetSessionObject(_AdminOrderSearchDateRange);
                }
                return result;
            }
            set { SetSessionObject(_AdminOrderSearchDateRange, value); }
        }

        public static DateTime AdminOrderSearchStartDate
        {
            get
            {
                var result = DateTime.Now.AddDays(1);
                if (GetSessionObject(_AdminOrderSearchStartDate) != null)
                {
                    result = (DateTime) GetSessionObject(_AdminOrderSearchStartDate);
                }
                return result;
            }
            set { SetSessionObject(_AdminOrderSearchStartDate, value); }
        }

        public static DateTime AdminOrderSearchEndDate
        {
            get
            {
                var result = DateTime.Now.AddDays(1);
                if (GetSessionObject(_AdminOrderSearchEndDate) != null)
                {
                    result = (DateTime) GetSessionObject(_AdminOrderSearchEndDate);
                }
                return result;
            }
            set { SetSessionObject(_AdminOrderSearchEndDate, value); }
        }

        public static int AdminOrderSearchLastPage
        {
            get
            {
                var result = 1;
                var o = GetSessionObject(_AdminOrderSearchLastPage);
                if (o != null)
                {
                    result = (int) o;
                }
                return result;
            }
            set { SetSessionObject(_AdminOrderSearchLastPage, value); }
        }

        public static bool AdminOrderSearchNewestFirst
        {
            get
            {
                var temp = GetSessionObject(_AdminOrderSearchNewestFirst);
                if (temp != null) return (bool) temp;
                return true;
            }
            set { SetSessionObject(_AdminOrderSearchNewestFirst, value); }
        }

        public static string AdminPromotionKeywords
        {
            get { return GetSessionString(_AdminPromotionKeywords); }
            set { SetSessionString(_AdminPromotionKeywords, value); }
        }

        public static string AdminVendorKeywords
        {
            get { return GetSessionString(_AdminVendorKeywords); }
            set { SetSessionString(_AdminVendorKeywords, value); }
        }

        public static string AdminManufacturerKeywords
        {
            get { return GetSessionString(_AdminManufacturerKeywords); }
            set { SetSessionString(_AdminManufacturerKeywords, value); }
        }

        public static string AdminCustomerKeywords
        {
            get { return GetSessionString(_AdminCustomerKeywords); }
            set { SetSessionString(_AdminCustomerKeywords, value); }
        }

        public static bool AdminPromotionShowDisabled
        {
            get { return (bool?) GetSessionObject(_AdminPromotionShowDisabled) ?? false; }
            set { SetSessionObject(_AdminPromotionShowDisabled, value); }
        }

        public static string AnalyticsOrderId
        {
            get { return GetSessionString(_AnalyticsOrderId); }
            set { SetSessionString(_AnalyticsOrderId, value); }
        }

        public static CredentialsDTO MarketingCredentials
        {
            get { return (CredentialsDTO) GetSessionObject("MarketingCredentials"); }
            set { SetSessionObject("MarketingCredentials", value); }
        }

        public static string CardSecurityCode
        {
            get { return GetSessionString("CardSecurityCode"); }
            set { SetSessionString("CardSecurityCode", value); }
        }

        public static string UserRegistrationPassword
        {
            get { return GetSessionString("UserRegistrationPassword"); }
            set { SetSessionString("UserRegistrationPassword", value); }
        }

        public static bool IsUserAuthenticated(HotcakesApplication app)
        {
            return app.CurrentCustomer != null;
        }

        public static Guid? GetCurrentSessionGuid()
        {
            var sessionGuid = Cookies.GetCookieGuid(
                WebAppSettings.CookieNameSessionGuid,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                Factory.CreateEventLogger());
            return sessionGuid;
        }

        public static void SetCurrentSessionGuid(Guid value)
        {
            Cookies.SetCookieGuid(WebAppSettings.CookieNameSessionGuid,
                value,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                false,
                Factory.CreateEventLogger());
        }

        public static Guid? GetCurrentShoppingSessionGuid()
        {
            var sessionGuid = Cookies.GetCookieGuid(
                WebAppSettings.CookieNameShoppingSessionGuid,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                Factory.CreateEventLogger());
            return sessionGuid;
        }

        public static void SetCurrentShoppingSessionGuid(Guid value)
        {
            Cookies.SetCookieGuid(WebAppSettings.CookieNameShoppingSessionGuid,
                value,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                true,
                Factory.CreateEventLogger());
        }

        public static string GetCurrentCartID(Store currentStore)
        {
            var result = string.Empty;

            if (currentStore.Settings.PreserveCartInSession)
            {
                result = GetSessionString(WebAppSettings.CookieNameCartId(currentStore.Id));
            }
            else
            {
                result = GetCookieString(WebAppSettings.CookieNameCartId(currentStore.Id));
            }

            return result;
        }

        public static void SetCurrentCartId(Store currentStore, string value)
        {
            if (currentStore.Settings.PreserveCartInSession)
            {
                SetSessionString(WebAppSettings.CookieNameCartId(currentStore.Id), value);
            }
            else
            {
                SetCookieString(WebAppSettings.CookieNameCartId(currentStore.Id), value);
            }
        }

        public static string GetCurrentPaymentPendingCartId(Store currentStore)
        {
            return GetCookieString(WebAppSettings.CookieNameCartIdPaymentPending(currentStore.Id));
        }

        public static void SetCurrentPaymentPendingCartId(Store currentStore, string value)
        {
            SetCookieString(WebAppSettings.CookieNameCartIdPaymentPending(currentStore.Id), value, DateTime.Now.AddDays(14));
        }

        public static void SetCurrentAffiliateId(long id, DateTime expirationDate)
        {
            SetCookieString(WebAppSettings.CookieNameAffiliateId, id.ToString(), expirationDate);
        }

        public static long? CurrentAffiliateID(Store currentStore)
        {
            var refVal = GetCookieString(WebAppSettings.CookieNameAffiliateId);

            if (!string.IsNullOrEmpty(refVal))
            {
                long id = 0;
                if (long.TryParse(refVal, out id))
                {
                    return id;
                }
            }

            return null;
        }

        public static bool CurrentUserHasCart(Store currentStore)
        {
            var cartId = GetCurrentCartID(currentStore);
            return !string.IsNullOrWhiteSpace(cartId);
        }

        #region " Private Set and Get "

        private object GetSessionObjectInt(string variableName)
        {
            object result = null;

            if (_session[variableName] != null)
            {
                result = _session[variableName];
            }

            return result;
        }

        private void SetSessionObjectInt(string variableName, object value)
        {
            _session[variableName] = value;
        }

        public static string GetSessionString(string variableName)
        {
            var result = string.Empty;

            var temp = GetSessionObject(variableName);
            if (temp != null)
            {
                result = (string) GetSessionObject(variableName);
            }

            return result;
        }

        public static void SetSessionString(string variableName, string value)
        {
            if (Factory.HttpContext != null)
            {
                if (Factory.HttpContext.Session != null)
                {
                    Factory.HttpContext.Session[variableName] = value ?? string.Empty;
                }
            }
        }

        public static object GetSessionObject(string variableName)
        {
            object result = null;


            if (Factory.HttpContext != null)
            {
                if (Factory.HttpContext.Session != null)
                {
                    if (Factory.HttpContext.Session[variableName] != null)
                    {
                        result = Factory.HttpContext.Session[variableName];
                    }
                }
            }

            return result;
        }

        public static void SetSessionObject(string variableName, object value)
        {
            if (Factory.HttpContext != null)
            {
                Factory.HttpContext.Session[variableName] = value;
            }
        }

        public static string GetCookieString(string cookieName)
        {
            try
            {
                if (Factory.HttpContext != null)
                {
                    var cookies = Factory.HttpContext.Request.Cookies;
                    return GetCookieString(cookieName, cookies);
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }

        public static string GetCookieString(string cookieName, HttpCookieCollection cookies)
        {
            var result = string.Empty;

            if (cookies == null) return string.Empty;

            try
            {
                var checkCookie = cookies[cookieName];
                if (checkCookie != null)
                {
                    result = checkCookie.Value;
                }
                if (string.IsNullOrEmpty(result))
                {
                    for (var i = 0; i < cookies.Count; i++)
                    {
                        checkCookie = cookies[i];
                        if (checkCookie.Name == cookieName)
                        {
                            result = checkCookie.Value;

                            if (!string.IsNullOrEmpty(result))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        public static void SetCookieString(string cookieName, string value)
        {
            SetCookieString(cookieName, value, null);
        }

        public static void SetCookieString(string cookieName, string value, DateTime? expirationDate)
        {
	        if (Factory.HttpContext != null)
	        {
		        try
                {
                    var isSecure = false;
                    if (HccApp != null)
                    { 
                        isSecure = HccApp.CurrentStore.Settings.ForceAdminSSL;
                    }
                    else
                    {
                        isSecure = Factory.HttpContext.Request.IsSecureConnection;
                    }

                    var saveCookie = new HttpCookie(cookieName, value);

                    if (expirationDate.HasValue)
                    {
                        saveCookie.Expires = expirationDate.Value;
                    }
                    else
                    {
                        saveCookie.Expires = DateTime.Now.AddYears(50);
                    }

                    saveCookie.Secure = isSecure;

			        Factory.HttpContext.Request.Cookies.Remove(cookieName);
			        Factory.HttpContext.Response.Cookies.Add(saveCookie);
		        }
		        catch (Exception Ex)
		        {
			        EventLog.LogEvent(Ex);
		        }
	        }
        }

        #endregion
    }
}