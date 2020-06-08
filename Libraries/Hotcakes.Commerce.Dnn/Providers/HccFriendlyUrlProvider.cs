#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Services.Url.FriendlyUrl;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Dnn.Providers
{
    [Serializable]
    public class HccFriendlyUrlProvider : DNNFriendlyUrlProvider
    {
        private const string ProviderType = "friendlyUrl";
        private const string RegexMatchExpression = "[^a-zA-Z0-9 ]";
        private const string REGEX_MATCH = "regexMatch";
        private const string URLREWRITE_ORIGINALURL = "UrlRewrite:OriginalUrl";
        private const string WWW_DOT = "www.";
        private const string SLUG = "slug";
        private const string TABID = "tabid";

        private readonly ProviderConfiguration _providerConfiguration =
            ProviderConfiguration.GetProviderConfiguration(ProviderType);

        private readonly string _regexMatch;

        public HccFriendlyUrlProvider()
        {
            var objProvider = (Provider) _providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            if (!string.IsNullOrEmpty(objProvider.Attributes[REGEX_MATCH]))
            {
                _regexMatch = objProvider.Attributes[REGEX_MATCH];
            }
            else
            {
                _regexMatch = RegexMatchExpression;
            }
        }

        public override string FriendlyUrl(TabInfo tab, string path)
        {
            var _portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            return FriendlyUrl(tab, path, Globals.glbDefaultPage, _portalSettings);
        }

        public override string FriendlyUrl(TabInfo tab, string path, string pageName)
        {
            var _portalSettings = PortalController.Instance.GetCurrentPortalSettings();
            return FriendlyUrl(tab, path, pageName, _portalSettings);
        }

        public override string FriendlyUrl(TabInfo tab, string path, string pageName, PortalSettings settings)
        {
            return FriendlyUrl(tab, path, pageName, settings.PortalAlias.HTTPAlias);
        }

        public override string FriendlyUrl(TabInfo tab, string path, string pageName, string portalAlias)
        {
            var currentStore = HccRequestContext.Current.CurrentStore;
            if (currentStore != null)
            {
                if (tab != null)
                {
                    var urlConfig = new UrlConfigSettings();

                    var locales = Factory.Instance.CreateStoreSettingsProvider().GetLocales();
                    foreach (var local in locales)
                    {
                        var context = new HccRequestContext(local.Code);
                        var accountServices = Factory.CreateService<AccountService>(context);
                        var store = accountServices.Stores.FindByIdWithCache(currentStore.Id);
                        urlConfig = store.Settings.Urls.UrlConfigs
                            .FirstOrDefault(uc => uc.TabId == tab.TabID);

                        if (urlConfig != null)
                        {
                            break;
                        }
                    }

                    if (urlConfig != null && !string.IsNullOrEmpty(urlConfig.CustomUrl))
                    {
                        var queryStringDic = GetQueryStringDictionary(path);
                        var friendlyPath = GetFriendlyAlias(path, portalAlias, true);

                        return GetFriendlyQueryString(tab, friendlyPath, pageName, urlConfig);
                    }
                }
            }

            return base.FriendlyUrl(tab, path, pageName, portalAlias);
        }

        private string GetFriendlyAlias(string path, string portalAlias, bool isPagePath)
        {
            var friendlyPath = path;
            var matchString = string.Empty;
            if (portalAlias != Null.NullString)
            {
                if (HttpContext.Current.Items[URLREWRITE_ORIGINALURL] != null)
                {
                    var httpAlias = Globals.AddHTTP(portalAlias).ToLowerInvariant();
                    var originalUrl = HttpContext.Current.Items[URLREWRITE_ORIGINALURL].ToString().ToLowerInvariant();
                    httpAlias = Globals.AddPort(httpAlias, originalUrl);
                    if (originalUrl.StartsWith(httpAlias))
                    {
                        matchString = httpAlias;
                    }
                    if (string.IsNullOrEmpty(matchString))
                    {
                        //Manage the special case where original url contains the alias as
                        //http://www.domain.com/Default.aspx?alias=www.domain.com/child"
                        var portalMatch = Regex.Match(originalUrl, string.Concat("^?alias=", portalAlias), RegexOptions.IgnoreCase);
                        if (!ReferenceEquals(portalMatch, Match.Empty))
                        {
                            matchString = httpAlias;
                        }
                    }

                    if (string.IsNullOrEmpty(matchString))
                    {
                        //Manage the special case of child portals 
                        //http://www.domain.com/child/default.aspx
                        var tempurl = HttpContext.Current.Request.Url.Host + Globals.ResolveUrl(friendlyPath);
                        if (!tempurl.Contains(portalAlias))
                        {
                            matchString = httpAlias;
                        }
                    }

                    if (string.IsNullOrEmpty(matchString))
                    {
                        // manage the case where the current hostname is www.domain.com and the portalalias is domain.com
                        // (this occurs when www.domain.com is not listed as portal alias for the portal, but domain.com is)
                        var wwwHttpAlias = Globals.AddHTTP(string.Concat(WWW_DOT, portalAlias));
                        if (originalUrl.StartsWith(wwwHttpAlias))
                        {
                            matchString = wwwHttpAlias;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(matchString))
            {
                if (path.IndexOf("~") != -1)
                {
                    if (matchString.EndsWith("/"))
                    {
                        friendlyPath = friendlyPath.Replace("~/", matchString);
                    }
                    else
                    {
                        friendlyPath = friendlyPath.Replace("~", matchString);
                    }
                }
                else
                {
                    friendlyPath = matchString + friendlyPath;
                }
            }
            else
            {
                friendlyPath = Globals.ResolveUrl(friendlyPath);
            }
            if (friendlyPath.StartsWith("//") && isPagePath)
            {
                friendlyPath = friendlyPath.Substring(1);
            }
            return friendlyPath;
        }

        private string GetFriendlyQueryString(TabInfo tab, string path, string pageName, UrlConfigSettings urlConfig)
        {
            var friendlyPath = path;
            var queryStringMatch = Regex.Match(friendlyPath, "(.[^\\\\?]*)\\\\?(.*)", RegexOptions.IgnoreCase);
            var queryStringSpecialChars = string.Empty;
            if (!ReferenceEquals(queryStringMatch, Match.Empty))
            {
                friendlyPath = queryStringMatch.Groups[1].Value;
                var queryStringDic = GetQueryStringDictionary(path);


                var customUrl = string.Empty;
                if (urlConfig != null)
                {
                    customUrl = urlConfig.CustomUrl;
                }
                var replacePath = string.Empty;
                if (!string.IsNullOrEmpty(customUrl))
                {
                    replacePath = customUrl.TrimStart('/');
                }

                if (queryStringDic.ContainsKey(SLUG))
                {
                    replacePath = string.Concat(replacePath, "/", HttpContext.Current.Server.UrlDecode(queryStringDic[SLUG].ToLower()));
                }

                friendlyPath = Regex.Replace(friendlyPath, Globals.glbDefaultPage, replacePath, RegexOptions.IgnoreCase);

                var queryString = queryStringMatch.Groups[2].Value.Replace("&amp;", "&");
                if (queryString.StartsWith("?"))
                {
                    queryString = queryString.TrimStart(Convert.ToChar("?"));
                }
                var nameValuePairs = queryString.Split(Convert.ToChar("&"));
                for (var i = 0; i <= nameValuePairs.Length - 1; i++)
                {
                    var pair = nameValuePairs[i].Split(Convert.ToChar("="));

                    if (pair[0] == SLUG || pair[0] == TABID)
                        continue;

                    //Rewrite into URL, contains only alphanumeric and the % or space
                    if (string.IsNullOrEmpty(queryStringSpecialChars))
                    {
                        queryStringSpecialChars = string.Concat(pair[0], "=", pair[1]);
                    }
                    else
                    {
                        queryStringSpecialChars = string.Concat(queryStringSpecialChars, "&", pair[0], "=", HttpContext.Current.Server.UrlDecode(pair[1]));
                    }
                }
            }
            if (!string.IsNullOrEmpty(queryStringSpecialChars))
            {
                return string.Concat(friendlyPath, "?", queryStringSpecialChars);
            }
            return friendlyPath;
        }

        private Dictionary<string, string> GetQueryStringDictionary(string path)
        {
            var parts = path.Split('?');
            var results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (parts.Length == 2)
            {
                foreach (var part in parts[1].Split('&'))
                {
                    var keyvalue = part.Split('=');
                    if (keyvalue.Length == 2)
                    {
                        results[keyvalue[0]] = keyvalue[1];
                    }
                }
            }

            return results;
        }
    }
}