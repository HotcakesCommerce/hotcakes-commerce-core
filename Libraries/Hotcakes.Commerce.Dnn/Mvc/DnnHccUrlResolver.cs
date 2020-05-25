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
using System.Linq;
using System.Web;
using System.Web.Routing;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Dnn.Mvc
{
    [Serializable]
    public class DnnHccUrlResolver : IHccUrlResolver
    {
        #region Properties

        public PortalSettings CurrentPortalSettings
        {
            get { return DnnGlobal.Instance.GetCurrentPortalSettings(); }
        }

        #endregion

        public string RouteHccUrl(HccRoute route, string actionName, string controllerName, string protocol,
            string hostName, string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection,
            RequestContext requestContext, bool includeImplicitMvcValues)
        {
            return GenerateUrl(route, null, null, protocol, null, null, routeValues, routeCollection, requestContext,
                false);
        }

        public string GetStoreDataVirtualPath(long storeId)
        {
            var httpContext = HttpContext.Current;
            var cacheKey = string.Format("Hotcakes_StorePortal_{0}", storeId);
            var portalId = 0;

            if (httpContext == null || httpContext.Cache[cacheKey] == null)
            {
                var accountServices = Factory.CreateService<AccountService>();
                var store = accountServices.Stores.FindByIdWithCache(storeId);
                var portalController = new PortalController();
                var portalInfo = portalController.GetPortal(store.StoreGuid);
                portalId = portalInfo != null ? portalInfo.PortalID : Null.NullInteger;

                if (httpContext != null)
                    httpContext.Cache.Insert(cacheKey, portalId);
            }
            else
            {
                portalId = (int) httpContext.Cache[cacheKey];
            }
            return string.Format("~/Portals/{0}/Hotcakes/Data/", portalId);
        }

        public string GetStoreRootUrl(Store store, bool secure)
        {
            var protocol = secure ? "https://" : "http://";

            var portalController = new PortalController();
            var portalInfo = portalController.GetPortal(store.StoreGuid);
            var portalSettings = new PortalSettings(portalInfo);
            return protocol + portalSettings.DefaultPortalAlias + "/";
        }

        private string GenerateUrl(HccRoute route, string actionName, string controllerName, string protocol,
            string hostName, string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection,
            RequestContext requestContext, bool includeImplicitMvcValues)
        {
            // Code that search DNN module corresponding to concrete route have to be here
            // Route params have to be added to the url too.

            var urlInfo = new DnnUrlInfo();
            if (route == HccRoute.Login)
            {
                return Globals.LoginURL((string) routeValues["returnUrl"], false);
            }
            if (route == HccRoute.Terms)
            {
                return Globals.NavigateURL("Terms");
            }
            if (route == HccRoute.Logoff)
            {
                return Globals.NavigateURL("LogOff");
            }
            if (route == HccRoute.SendPassword)
            {
                return Globals.NavigateURL("SendPassword");
            }
            if (route == HccRoute.UserProfile)
            {
                var userId = 0;
                if (int.TryParse((string) routeValues["userId"], out userId))
                    return Globals.UserProfileURL(userId);
                throw new ApplicationException("UserId is not a number");
            }
            urlInfo = GetTabId(route);

            var paramsList = new List<string>();
            if (!string.IsNullOrEmpty(urlInfo.ControlKey))
            {
                var moduleCtl = new ModuleController();
                var mInfo = moduleCtl.GetTabModules(urlInfo.TabId)
                    .Where(m => m.Value.DesktopModule.ModuleName == urlInfo.ModuleName)
                    .Select(m => m.Value)
                    .FirstOrDefault();

                if (mInfo != null)
                {
                    paramsList.Add("mid=" + mInfo.ModuleID);
                }
            }

            if (routeValues != null)
            {
                foreach (var newQueryItem in routeValues)
                {
                    var isParamValid = newQueryItem.Value != null;
                    if (newQueryItem.Value is string)
                        isParamValid = !string.IsNullOrWhiteSpace(newQueryItem.Value as string);

                    if (isParamValid)
                    {
                        var parameter = string.Format("{0}={1}", newQueryItem.Key, newQueryItem.Value);
                        paramsList.Add(parameter);
                    }
                }
            }

            var portalSettings = CurrentPortalSettings;
            var isSuperTab = Globals.IsHostTab(urlInfo.TabId);
            var parameters = paramsList.ToArray();

            var navigateUrl = string.Empty;
            if (PortalSettings.Current != null)
            {
                navigateUrl = Globals.NavigateURL(urlInfo.TabId, isSuperTab, portalSettings, urlInfo.ControlKey, null,
                    parameters);
            }
            else
            {
                navigateUrl = NavigateUrl(urlInfo.TabId, isSuperTab, portalSettings, urlInfo.ControlKey, parameters);
            }
            return navigateUrl;
        }

        private string NavigateUrl(int tabId, bool isSuperTab, PortalSettings settings, string controlKey,
            string[] additionalParameters)
        {
            var url = Globals.ApplicationURL(tabId);
            if (!string.IsNullOrEmpty(controlKey))
            {
                url = url + "&ctl=" + controlKey;
            }
            if (additionalParameters != null)
            {
                url =
                    additionalParameters.Where(delegate(string parameter) { return !string.IsNullOrEmpty(parameter); })
                        .Aggregate(url, delegate(string current, string parameter) { return current + "&" + parameter; });
            }
            if (isSuperTab)
            {
                url = url + "&portalid=" + settings.PortalId;
            }

            return Globals.ResolveUrl(url);
        }

        private DnnUrlInfo GetTabId(HccRoute route)
        {
            StoreSettingsUrls urlStoreSettings = null;
            if (HccRequestContext.Current.CurrentStore != null)
            {
                HccRequestContext localizedContext = HccRequestContextUtils.GetContextWithCulture(HccRequestContext.Current, CurrentPortalSettings.CultureCode);
                urlStoreSettings = localizedContext.CurrentStore.Settings.Urls;
            }

            var urlInfo = new DnnUrlInfo();
            switch (route)
            {
                case HccRoute.Cart:
                    urlInfo.TabId = urlStoreSettings.CartTabId;
                    break;
                case HccRoute.Product:
                    urlInfo.TabId = urlStoreSettings.ProductTabId;
                    break;
                case HccRoute.ProductReview:
                    urlInfo.TabId = urlStoreSettings.ProductReviewTabId;
                    break;
                case HccRoute.Category:
                    urlInfo.TabId = urlStoreSettings.CategoryTabId;
                    break;
                case HccRoute.Checkout:
                    urlInfo.TabId = urlStoreSettings.CheckoutTabId;
                    break;
                case HccRoute.CheckoutPayPal:
                    urlInfo.TabId = urlStoreSettings.CheckoutTabId;
                    urlInfo.ControlKey = "PayPalCheckout";
                    urlInfo.ModuleName = "Hotcakes.Checkout";
                    break;
                case HccRoute.ThirdPartyPayment:
                    urlInfo.TabId = urlStoreSettings.CheckoutTabId;
                    urlInfo.ControlKey = "ThirdPartyPayment";
                    urlInfo.ModuleName = "Hotcakes.Checkout";
                    break;
                case HccRoute.WishList:
                    urlInfo.TabId = urlStoreSettings.WishListTabId;
                    break;
                case HccRoute.Search:
                    urlInfo.TabId = urlStoreSettings.SearchTabId;
                    break;
                case HccRoute.OrderHistory:
                    urlInfo.TabId = urlStoreSettings.OrderHistoryTabId;
                    break;
                case HccRoute.AddressBook:
                    urlInfo.TabId = urlStoreSettings.AddressBookTabId;
                    break;
                case HccRoute.EditUserProfile:
                    urlInfo.TabId = CurrentPortalSettings.UserTabId;
                    urlInfo.ControlKey = "Profile";
                    break;
                case HccRoute.AffiliateRegistration:
                    urlInfo.TabId = GetTabIdByModuleName("Hotcakes.AffiliateRegistration");
                    break;
                case HccRoute.AffiliateDashboard:
                    urlInfo.TabId = GetTabIdByModuleName("Hotcakes.AffiliateDashboard");
                    break;
                case HccRoute.Home:
                    urlInfo.TabId = CurrentPortalSettings.HomeTabId;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return urlInfo;
        }

        private int GetTabIdByModuleName(string moduleName)
        {
            var portalId = CurrentPortalSettings.PortalId;
            var dModule = DesktopModuleController.GetDesktopModuleByModuleName(moduleName, portalId);
            var modDef = ModuleDefinitionController.GetModuleDefinitionsByDesktopModuleID(dModule.DesktopModuleID);

            var moduleController = new ModuleController();
            var modules = moduleController.GetModulesByDefinition(portalId, modDef.First().Value.FriendlyName);

            var currentCulture = HccRequestContext.Current.MainContentCulture;
            var module = modules.OfType<ModuleInfo>().FirstOrDefault(m => m.CultureCode == currentCulture);
            if (module == null)
                module = modules.OfType<ModuleInfo>().FirstOrDefault();
            return module.TabID;
        }
    }
}