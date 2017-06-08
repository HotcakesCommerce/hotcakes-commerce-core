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
using System.Collections.Specialized;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Urls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Modules.ExtensionUrlProvider
{
    [Serializable]
    public class HccExtensionUrlProvider : DotNetNuke.Entities.Urls.ExtensionUrlProvider
    {
        public override bool AlwaysUsesDnnPagePath(int portalId)
        {
            return false;
        }

        public override string ChangeFriendlyUrl(TabInfo tab, string friendlyUrlPath, FriendlyUrlOptions options,
            string cultureCode, ref string endingPageName, out bool useDnnPagePath, ref List<string> messages)
        {
            useDnnPagePath = true;
            if (messages == null)
                messages = new List<string>();
            if (HccRequestContext.Current != null)
            {
                var store = HccRequestContext.Current.CurrentStore;
                if (tab != null && store != null)
                {
                    var urlSettings = store.Settings.Urls;
                    if (urlSettings.ProductTabId == tab.TabID
                        || urlSettings.CategoryTabId == tab.TabID
                        || urlSettings.ProductReviewTabId == tab.TabID)
                    {
                        var slugParamName = "/slug/";
                        var slugStart = friendlyUrlPath.IndexOf(slugParamName);
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
                            return newUrl;
                        }
                        return null;
                    }
                }
            }
            return null;
        }

        public override bool CheckForRedirect(int tabId, int portalid, string httpAlias, Uri requestUri,
            NameValueCollection queryStringCol, FriendlyUrlOptions options, out string redirectLocation,
            ref List<string> messages)
        {
            redirectLocation = null;
            return false;
        }

        public override Dictionary<string, string> GetProviderPortalSettings()
        {
            return null;
        }

        public override string TransformFriendlyUrlToQueryString(string[] urlParms, int tabId, int portalId,
            FriendlyUrlOptions options, string cultureCode, PortalAliasInfo portalAlias, ref List<string> messages,
            out int status, out string location)
        {
            var result = string.Empty;
            status = 200; //OK
            location = null; //no redirect location

            var requestedPath = string.Join("/", urlParms);
            requestedPath = EnsureLeadingChar("/", requestedPath);

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
                }
            }
            return result;
        }
    }
}