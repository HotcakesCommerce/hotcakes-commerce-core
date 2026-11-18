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
using System.Collections.Specialized;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Urls;
using DotNetNuke.Instrumentation;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Modules.ExtensionUrlProvider
{
    /// <summary>
    /// Integrates Hotcakes product, category, and review URLs with the DNN friendly URL provider pipeline.
    /// </summary>
    [Serializable]
    public class HccExtensionUrlProvider : DotNetNuke.Entities.Urls.ExtensionUrlProvider
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(HccExtensionUrlProvider));

        /// <summary>
        /// Indicates whether DNN should always use its page path for URLs handled by this provider.
        /// </summary>
        /// <param name="portalId">The portal for which DNN is resolving the URL.</param>
        /// <returns><see langword="false"/> so Hotcakes URLs can use their configured friendly path.</returns>
        public override bool AlwaysUsesDnnPagePath(int portalId)
        {
            return false;
        }

        /// <summary>
        /// Converts a Hotcakes slug URL on an eligible tab into the corresponding friendly URL path.
        /// </summary>
        /// <param name="tab">The DNN tab being resolved.</param>
        /// <param name="friendlyUrlPath">The friendly URL path supplied by DNN.</param>
        /// <param name="options">The DNN friendly URL options for the current request.</param>
        /// <param name="cultureCode">The culture code associated with the URL request.</param>
        /// <param name="endingPageName">The ending page name supplied by DNN.</param>
        /// <param name="useDnnPagePath">When this method returns, indicates whether DNN should use its page path.</param>
        /// <param name="messages">Messages collected during URL transformation.</param>
        /// <returns>The transformed path when the request targets a configured Hotcakes tab and contains a slug; otherwise, <see langword="null"/>.</returns>
        public override string ChangeFriendlyUrl(TabInfo tab, string friendlyUrlPath, FriendlyUrlOptions options,
            string cultureCode, ref string endingPageName, out bool useDnnPagePath, ref List<string> messages)
        {
            useDnnPagePath = true;
            if (messages == null)
                messages = new List<string>();

            Logger.Debug(string.Format("HccExtensionUrlProvider.ChangeFriendlyUrl - tabId={0}; path={1}; culture={2}; useDnnPagePath={3}", tab != null ? tab.TabID : -1, friendlyUrlPath ?? "(null)", cultureCode ?? "(null)", useDnnPagePath));

            var store = HccRequestContext.Current != null ? HccRequestContext.Current.CurrentStore : null;
            if (HccRequestContext.Current != null)
                Logger.Debug(string.Format("HccExtensionUrlProvider.ChangeFriendlyUrl - request context available; storeId={0}", store != null ? store.Id : -1));

            if (store == null && tab != null && friendlyUrlPath != null && friendlyUrlPath.IndexOf("/slug/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var lookupContext = new HccRequestContext();
                if (!string.IsNullOrWhiteSpace(cultureCode))
                    lookupContext.MainContentCulture = cultureCode;
                lookupContext.FallbackContentCulture = string.Empty;
                var portalSettings = new PortalSettings(tab.PortalID);
                var accountServices = Factory.CreateService<AccountService>(lookupContext);
                store = accountServices.GetStoreByUrl(portalSettings.DefaultPortalAlias);
                Logger.Debug(string.Format("HccExtensionUrlProvider.ChangeFriendlyUrl - ambient context unavailable; portalId={0}; alias={1}; fallback storeId={2}", tab.PortalID, portalSettings.DefaultPortalAlias ?? "(null)", store != null ? store.Id : -1));
            }

            if (store != null && tab != null)
            {
                    var urlSettings = store.Settings.Urls;
                    Logger.Debug(string.Format("HccExtensionUrlProvider.ChangeFriendlyUrl - configured tabs product={0}; category={1}; review={2}; current={3}", urlSettings.ProductTabId, urlSettings.CategoryTabId, urlSettings.ProductReviewTabId, tab.TabID));
                    if (urlSettings.ProductTabId == tab.TabID
                        || urlSettings.CategoryTabId == tab.TabID
                        || urlSettings.ProductReviewTabId == tab.TabID)
                    {
                        var slugParamName = "/slug/";
                        var slugStart = friendlyUrlPath == null ? -1 : friendlyUrlPath.IndexOf(slugParamName, StringComparison.OrdinalIgnoreCase);
                        if (slugStart > -1)
                        {
                            var slugEnd = friendlyUrlPath.IndexOf("/", slugStart + slugParamName.Length);
                            if (slugEnd == -1)
                                slugEnd = friendlyUrlPath.Length;

                            var slug = friendlyUrlPath.Substring(slugStart + slugParamName.Length,
                                slugEnd - (slugStart + slugParamName.Length));

                            var slugPart = "/slug/" + slug;
                            var spareArgs = friendlyUrlPath.Trim('/');
                            spareArgs = friendlyUrlPath.Replace(slugPart, string.Empty).Trim('/');

                            var newUrl = "/" + slug;

                            if (!string.IsNullOrWhiteSpace(spareArgs))
                                newUrl += "?" + CreateQueryStringFromParameters(spareArgs.Split('/'), -1).TrimStart('&');
                            Logger.Debug(string.Format("HccExtensionUrlProvider.ChangeFriendlyUrl - transformed path={0}; result={1}; useDnnPagePath={2}", friendlyUrlPath, newUrl, useDnnPagePath));
                            return newUrl;
                        }
                        Logger.Debug("HccExtensionUrlProvider.ChangeFriendlyUrl - eligible tab but /slug/ marker was not found; returning null");
                        return null;
                    }
                    Logger.Debug("HccExtensionUrlProvider.ChangeFriendlyUrl - current tab is not a Hotcakes product/category/review tab; returning null");
                }
            else
                Logger.Debug("HccExtensionUrlProvider.ChangeFriendlyUrl - tab or store unavailable; returning null");
            return null;
        }

        /// <summary>
        /// Determines whether the request requires a redirect before DNN resolves the friendly URL.
        /// </summary>
        /// <param name="tabId">The DNN tab identifier for the request.</param>
        /// <param name="portalid">The portal identifier for the request.</param>
        /// <param name="httpAlias">The HTTP alias associated with the request.</param>
        /// <param name="requestUri">The requested URI.</param>
        /// <param name="queryStringCol">The query string parameters from the request.</param>
        /// <param name="options">The DNN friendly URL options for the current request.</param>
        /// <param name="redirectLocation">When this method returns, contains the redirect destination when a redirect is required.</param>
        /// <param name="messages">Messages collected while checking the request.</param>
        /// <returns><see langword="false"/> because this provider does not request redirects.</returns>
        public override bool CheckForRedirect(int tabId, int portalid, string httpAlias, Uri requestUri,
            NameValueCollection queryStringCol, FriendlyUrlOptions options, out string redirectLocation,
            ref List<string> messages)
        {
            redirectLocation = null;
            return false;
        }

        /// <summary>
        /// Gets the portal settings exposed by this URL provider.
        /// </summary>
        /// <returns><see langword="null"/> because this provider does not define portal settings.</returns>
        public override Dictionary<string, string> GetProviderPortalSettings()
        {
            return null;
        }

        /// <summary>
        /// Converts the path segments of an eligible Hotcakes friendly URL into query-string parameters.
        /// </summary>
        /// <param name="urlParms">The path segments parsed from the friendly URL.</param>
        /// <param name="tabId">The DNN tab identifier associated with the request.</param>
        /// <param name="portalId">The portal identifier associated with the request.</param>
        /// <param name="options">The DNN friendly URL options for the current request.</param>
        /// <param name="cultureCode">The culture code associated with the URL request; when empty, the portal culture is used.</param>
        /// <param name="portalAlias">The portal alias used to locate the Hotcakes store.</param>
        /// <param name="messages">Messages collected during URL transformation.</param>
        /// <param name="status">When this method returns, contains the HTTP status for the transformation.</param>
        /// <param name="location">When this method returns, contains a redirect location when one is produced.</param>
        /// <returns>The query-string representation of the path for an eligible Hotcakes tab, or an empty string when the store or tab is not eligible.</returns>
        public override string TransformFriendlyUrlToQueryString(string[] urlParms, int tabId, int portalId,
            FriendlyUrlOptions options, string cultureCode, PortalAliasInfo portalAlias, ref List<string> messages,
            out int status, out string location)
        {
            if (string.IsNullOrEmpty(cultureCode))
            {
                var PortalSetting = PortalController.Instance.GetPortal(portalId);
                cultureCode = PortalSetting.CultureCode;
            }
            var result = string.Empty;
            status = 200; //OK
            location = null; //no redirect location

            var requestedPath = string.Join("/", urlParms);
            requestedPath = EnsureLeadingChar("/", requestedPath);
            Logger.Debug(string.Format("HccExtensionUrlProvider.TransformFriendlyUrlToQueryString - tabId={0}; portalId={1}; path={2}; culture={3}", tabId, portalId, requestedPath, cultureCode ?? "(null)"));

            var context = new HccRequestContext();
            var accountServices = Factory.CreateService<AccountService>(context);

            if (!string.IsNullOrWhiteSpace(cultureCode))
                context.MainContentCulture = cultureCode;
            context.FallbackContentCulture = string.Empty;

            var store = accountServices.GetStoreByUrl(portalAlias.HTTPAlias);
            if (store != null)
            {
                var urlSettings = store.Settings.Urls;
                if (urlSettings.ProductTabId == tabId
                    || urlSettings.CategoryTabId == tabId
                    || urlSettings.ProductReviewTabId == tabId)
                {
                    var position = -1;
                    if (urlParms.Length%2 == 1)
                    {
                        position = 0;
                        result = "slug=" + urlParms[0];
                    }
                    result += CreateQueryStringFromParameters(urlParms, position);
                    Logger.Debug(string.Format("HccExtensionUrlProvider.TransformFriendlyUrlToQueryString - transformed result={0}; status={1}", result, status));
                }
                else
                    Logger.Debug("HccExtensionUrlProvider.TransformFriendlyUrlToQueryString - tab is not a Hotcakes product/category/review tab");
            }
            else
                Logger.Debug("HccExtensionUrlProvider.TransformFriendlyUrlToQueryString - store not found for portal alias");
            return result;
        }
    }
}
