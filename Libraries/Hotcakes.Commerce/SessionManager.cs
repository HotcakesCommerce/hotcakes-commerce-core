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
using System.Web;
using System.Web.SessionState;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;

namespace Hotcakes.Commerce
{
    /// <summary>
    /// Provides the session, cookie, and current-cart accessors used by Hotcakes Commerce.
    /// </summary>
    /// <remarks>
    /// The static members use the current HTTP request. Session-backed values are stored in the
    /// current ASP.NET session, while cookie-backed values are written to the response cookie
    /// collection. The established key names are part of the compatibility contract.
    /// </remarks>
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

        /// <summary>
        /// Gets the current <see cref="HotcakesApplication"/> instance.
        /// </summary>
        public static HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        /// <summary>
        /// Initializes a manager for the specified ASP.NET session state collection.
        /// </summary>
        /// <param name="session">The session collection used by instance-scoped administrative values.</param>
        public SessionManager(HttpSessionState session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets or sets the current administrative language in the <c>HccAdminCurrentLanguage</c> cookie.
        /// </summary>
        public static string AdminCurrentLanguage
        {
            get { return GetCookieString(_AdminCurrentLanguage); }
            set { SetCookieString(_AdminCurrentLanguage, value); }
        }

        /// <summary>
        /// Gets or sets the last visited category identifier in the <c>HccCategoryLastVisited</c> session value.
        /// </summary>
        public static string CategoryLastId
        {
            get { return GetSessionString(CategoryLastIdVariable); }
            set { SetSessionString(CategoryLastIdVariable, value); }
        }

        // Admin Settings

        /// <summary>Gets or sets the administrative product-search keyword in session.</summary>
        public static string AdminProductCriteriaKeyword
        {
            get { return GetSessionString(_AdminProductSearchCriteriaKeyword); }
            set { SetSessionString(_AdminProductSearchCriteriaKeyword, value); }
        }

        /// <summary>Gets or sets the administrative product-search manufacturer filter in session.</summary>
        public static string AdminProductCriteriaManufacturer
        {
            get { return GetSessionString(_AdminProductSearchCriteriaManufacturer); }
            set { SetSessionString(_AdminProductSearchCriteriaManufacturer, value); }
        }

        /// <summary>Gets or sets the administrative product-search vendor filter in session.</summary>
        public static string AdminProductCriteriaVendor
        {
            get { return GetSessionString(_AdminProductSearchCriteriaVendor); }
            set { SetSessionString(_AdminProductSearchCriteriaVendor, value); }
        }

        /// <summary>Gets or sets the administrative product-search category filter in session.</summary>
        public static string AdminProductCriteriaCategory
        {
            get { return GetSessionString(_AdminProductSearchCriteriaCategory); }
            set { SetSessionString(_AdminProductSearchCriteriaCategory, value); }
        }

        /// <summary>Gets or sets the administrative product-search status filter in session.</summary>
        public static string AdminProductCriteriaStatus
        {
            get { return GetSessionString(_AdminProductSearchCriteriaStatus); }
            set { SetSessionString(_AdminProductSearchCriteriaStatus, value); }
        }

        /// <summary>Gets or sets the administrative product-search inventory-status filter in session.</summary>
        public static string AdminProductCriteriaInventoryStatus
        {
            get { return GetSessionString(_AdminProductSearchCriteriaInventoryStatus); }
            set { SetSessionString(_AdminProductSearchCriteriaInventoryStatus, value); }
        }

        /// <summary>Gets or sets the administrative product-search product-type filter in session.</summary>
        public static string AdminProductCriteriaProductType
        {
            get { return GetSessionString(_AdminProductSearchCriteriaProductType); }
            set { SetSessionString(_AdminProductSearchCriteriaProductType, value); }
        }

        /// <summary>
        /// Gets or sets the current product-import log in the instance session.
        /// </summary>
        /// <remarks>A missing value is returned as a new empty list; the list is not automatically persisted until set.</remarks>
        public List<string> AdminProductImportLog
        {
            get { return (List<string>) GetSessionObjectInt(_AdminProductImportLog) ?? new List<string>(); }
            set { SetSessionObjectInt(_AdminProductImportLog, value); }
        }

        /// <summary>
        /// Gets or sets product-import progress in the instance session.
        /// </summary>
        /// <remarks>A missing value is returned as <c>0</c>.</remarks>
        public double AdminProductImportProgress
        {
            get { return (double?) GetSessionObjectInt(_AdminProductImportProgress) ?? 0; }
            set { SetSessionObjectInt(_AdminProductImportProgress, value); }
        }

        /// <summary>Gets or sets the administrative order-search keyword in session.</summary>
        public static string AdminOrderSearchKeyword
        {
            get { return GetSessionString(_AdminOrderSearchKeyword); }
            set { SetSessionString(_AdminOrderSearchKeyword, value); }
        }

        /// <summary>Gets or sets the administrative order-search payment filter in session.</summary>
        public static string AdminOrderSearchPaymentFilter
        {
            get { return GetSessionString(_AdminOrderSearchPaymentFilter); }
            set { SetSessionString(_AdminOrderSearchPaymentFilter, value); }
        }

        /// <summary>Gets or sets the administrative order-search shipping filter in session.</summary>
        public static string AdminOrderSearchShippingFilter
        {
            get { return GetSessionString(_AdminOrderSearchShippingFilter); }
            set { SetSessionString(_AdminOrderSearchShippingFilter, value); }
        }

        /// <summary>Gets or sets the administrative order-search status filter in session.</summary>
        public static string AdminOrderSearchStatusFilter
        {
            get { return GetSessionString(_AdminOrderSearchStatusFilter); }
            set { SetSessionString(_AdminOrderSearchStatusFilter, value); }
        }

        /// <summary>
        /// Gets or sets the administrative order-search date range in session.
        /// </summary>
        /// <remarks>The default is <see cref="DateRangeType.AllDates"/> when no value has been stored.</remarks>
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

        /// <summary>Gets or sets the administrative order-search start date in session.</summary>
        /// <remarks>The default is tomorrow when no value has been stored.</remarks>
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

        /// <summary>Gets or sets the administrative order-search end date in session.</summary>
        /// <remarks>The default is tomorrow when no value has been stored.</remarks>
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

        /// <summary>Gets or sets the last administrative order-search page in session.</summary>
        /// <remarks>The default is page <c>1</c> when no value has been stored.</remarks>
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

        /// <summary>Gets or sets whether administrative order results are newest first in session.</summary>
        /// <remarks>The default is <see langword="true"/> when no value has been stored.</remarks>
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

        /// <summary>Gets or sets the administrative promotion-search keywords in session.</summary>
        public static string AdminPromotionKeywords
        {
            get { return GetSessionString(_AdminPromotionKeywords); }
            set { SetSessionString(_AdminPromotionKeywords, value); }
        }

        /// <summary>Gets or sets the administrative vendor-search keywords in session.</summary>
        public static string AdminVendorKeywords
        {
            get { return GetSessionString(_AdminVendorKeywords); }
            set { SetSessionString(_AdminVendorKeywords, value); }
        }

        /// <summary>Gets or sets the administrative manufacturer-search keywords in session.</summary>
        public static string AdminManufacturerKeywords
        {
            get { return GetSessionString(_AdminManufacturerKeywords); }
            set { SetSessionString(_AdminManufacturerKeywords, value); }
        }

        /// <summary>Gets or sets the administrative customer-search keywords in session.</summary>
        public static string AdminCustomerKeywords
        {
            get { return GetSessionString(_AdminCustomerKeywords); }
            set { SetSessionString(_AdminCustomerKeywords, value); }
        }

        /// <summary>Gets or sets whether disabled promotions are included in administrative results.</summary>
        public static bool AdminPromotionShowDisabled
        {
            get { return (bool?) GetSessionObject(_AdminPromotionShowDisabled) ?? false; }
            set { SetSessionObject(_AdminPromotionShowDisabled, value); }
        }

        /// <summary>Gets or sets the analytics order identifier in session.</summary>
        public static string AnalyticsOrderId
        {
            get { return GetSessionString(_AnalyticsOrderId); }
            set { SetSessionString(_AnalyticsOrderId, value); }
        }

        /// <summary>
        /// Gets or sets the payment-card security code in session.
        /// </summary>
        /// <remarks>This value is retained under the compatibility key <c>CardSecurityCode</c>; callers must treat it as sensitive payment data.</remarks>
        public static string CardSecurityCode
        {
            get { return GetSessionString("CardSecurityCode"); }
            set { SetSessionString("CardSecurityCode", value); }
        }

        /// <summary>
        /// Gets or sets the user-registration password in session.
        /// </summary>
        /// <remarks>This value is retained under the compatibility key <c>UserRegistrationPassword</c>; callers must treat it as sensitive credential data.</remarks>
        public static string UserRegistrationPassword
        {
            get { return GetSessionString("UserRegistrationPassword"); }
            set { SetSessionString("UserRegistrationPassword", value); }
        }

        /// <summary>
        /// Determines whether the supplied application has a current customer.
        /// </summary>
        /// <param name="app">The application whose current customer is checked.</param>
        /// <returns><see langword="true"/> when <paramref name="app"/> has a current customer; otherwise, <see langword="false"/>.</returns>
        public static bool IsUserAuthenticated(HotcakesApplication app)
        {
            return app.CurrentCustomer != null;
        }

        /// <summary>Gets the current session identifier from the configured session GUID cookie.</summary>
        /// <returns>The cookie GUID, or <see langword="null"/> when it is absent or invalid.</returns>
        public static Guid? GetCurrentSessionGuid()
        {
            var sessionGuid = Cookies.GetCookieGuid(
                WebAppSettings.CookieNameSessionGuid,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                Factory.CreateEventLogger());
            return sessionGuid;
        }

        /// <summary>Stores the current session identifier in the configured session GUID cookie.</summary>
        /// <param name="value">The session identifier to store.</param>
        public static void SetCurrentSessionGuid(Guid value)
        {
            Cookies.SetCookieGuid(WebAppSettings.CookieNameSessionGuid,
                value,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                false,
                Factory.CreateEventLogger());
        }

        /// <summary>Gets the current shopping-session identifier from its configured cookie.</summary>
        /// <returns>The cookie GUID, or <see langword="null"/> when it is absent or invalid.</returns>
        public static Guid? GetCurrentShoppingSessionGuid()
        {
            var sessionGuid = Cookies.GetCookieGuid(
                WebAppSettings.CookieNameShoppingSessionGuid,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                Factory.CreateEventLogger());
            return sessionGuid;
        }

        /// <summary>Stores the current shopping-session identifier in its configured cookie.</summary>
        /// <param name="value">The shopping-session identifier to store.</param>
        public static void SetCurrentShoppingSessionGuid(Guid value)
        {
            Cookies.SetCookieGuid(WebAppSettings.CookieNameShoppingSessionGuid,
                value,
                Factory.HttpContext.Request.RequestContext.HttpContext,
                true,
                Factory.CreateEventLogger());
        }

        /// <summary>
        /// Gets the current cart identifier for a store.
        /// </summary>
        /// <param name="currentStore">The store whose cart-preservation setting and identifier are used.</param>
        /// <returns>The cart identifier from session when cart preservation is enabled; otherwise, from the store cart cookie.</returns>
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

        /// <summary>Stores the current cart identifier in session or in the store cart cookie.</summary>
        /// <param name="currentStore">The store whose cart-preservation setting determines the storage location.</param>
        /// <param name="value">The cart identifier to store.</param>
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

        /// <summary>Gets the payment-pending cart identifier from the store-specific cookie.</summary>
        /// <param name="currentStore">The store whose payment-pending cart cookie is read.</param>
        /// <returns>The pending cart identifier, or an empty string when no value is available.</returns>
        public static string GetCurrentPaymentPendingCartId(Store currentStore)
        {
            return GetCookieString(WebAppSettings.CookieNameCartIdPaymentPending(currentStore.Id));
        }

        /// <summary>Stores a payment-pending cart identifier for fourteen days in the store-specific cookie.</summary>
        /// <param name="currentStore">The store whose payment-pending cart cookie is written.</param>
        /// <param name="value">The cart identifier to store.</param>
        public static void SetCurrentPaymentPendingCartId(Store currentStore, string value)
        {
            SetCookieString(WebAppSettings.CookieNameCartIdPaymentPending(currentStore.Id), value, DateTime.Now.AddDays(14));
        }

        /// <summary>Stores an affiliate identifier in the configured affiliate cookie until the specified expiration.</summary>
        /// <param name="id">The affiliate identifier.</param>
        /// <param name="expirationDate">The cookie expiration date and time.</param>
        public static void SetCurrentAffiliateId(long id, DateTime expirationDate)
        {
            SetCookieString(WebAppSettings.CookieNameAffiliateId, id.ToString(), expirationDate);
        }

        /// <summary>Gets the affiliate identifier from the configured affiliate cookie.</summary>
        /// <param name="currentStore">The current store context; retained for API compatibility.</param>
        /// <returns>The parsed affiliate identifier, or <see langword="null"/> when the cookie is absent or not a valid integer.</returns>
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

        /// <summary>Determines whether the current store has a nonblank cart identifier.</summary>
        /// <param name="currentStore">The store whose current cart is checked.</param>
        /// <returns><see langword="true"/> when a non-whitespace cart identifier is available; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>Gets a string value from the current ASP.NET session.</summary>
        /// <param name="variableName">The exact session key to read.</param>
        /// <returns>The stored string, or an empty string when the session, key, or value is unavailable.</returns>
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

        /// <summary>Stores a string in the current ASP.NET session.</summary>
        /// <param name="variableName">The exact session key to write.</param>
        /// <param name="value">The value to store; <see langword="null"/> is stored as an empty string.</param>
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

        /// <summary>Gets an object from the current ASP.NET session.</summary>
        /// <param name="variableName">The exact session key to read.</param>
        /// <returns>The stored object, or <see langword="null"/> when no current session value exists.</returns>
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

        /// <summary>Stores an object in the current ASP.NET session when an HTTP context is available.</summary>
        /// <param name="variableName">The exact session key to write.</param>
        /// <param name="value">The object to store.</param>
        public static void SetSessionObject(string variableName, object value)
        {
            if (Factory.HttpContext != null)
            {
                Factory.HttpContext.Session[variableName] = value;
            }
        }

        /// <summary>Gets a value from the current request's cookie collection.</summary>
        /// <param name="cookieName">The exact cookie name to read.</param>
        /// <returns>The cookie value, or an empty string when the context is unavailable or reading fails.</returns>
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

        /// <summary>Gets a value from a supplied cookie collection.</summary>
        /// <param name="cookieName">The exact cookie name to read.</param>
        /// <param name="cookies">The cookie collection to search.</param>
        /// <returns>The first nonempty matching cookie value, or an empty string when none is available or reading fails.</returns>
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

        /// <summary>Writes a cookie using the default fifty-year expiration.</summary>
        /// <param name="cookieName">The exact cookie name to write.</param>
        /// <param name="value">The cookie value.</param>
        public static void SetCookieString(string cookieName, string value)
        {
            SetCookieString(cookieName, value, null);
        }

        /// <summary>
        /// Writes a cookie to the current response.
        /// </summary>
        /// <param name="cookieName">The exact cookie name to write.</param>
        /// <param name="value">The cookie value.</param>
        /// <param name="expirationDate">The expiration date; when omitted, the cookie expires fifty years from now.</param>
        /// <remarks>The cookie's secure flag follows the store's forced-admin-SSL setting, or the current request when no application is available. Failures are logged and not rethrown.</remarks>
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
