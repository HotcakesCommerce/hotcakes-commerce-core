﻿#region License

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
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Xml.XPath;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class MenuProvider
    {
        private const string localResourceFile =
            "~/DesktopModules/Hotcakes/Core/Admin/App_LocalResources/AdminControlBar.resx";
        private const string _MenuFileVirtualPath = "~/DesktopModules/Hotcakes/Core/Admin/Menu.xml";
        private static ILocalizationHelper localization { get; set; }
        private static List<MenuItem> _menuItems;

        public static List<MenuItem> MenuItems
        {
            get
            {
                var cacheKey = string.Concat("_menuItems", HttpContext.Current.Request.Cookies["language"].Value);
                if (DataCache.GetCache(cacheKey) != null)
                    _menuItems = (List<MenuItem>)DataCache.GetCache(cacheKey);
                else
                {
                    var _menuFilePath = HostingEnvironment.MapPath(_MenuFileVirtualPath);
                    var xml = XElement.Load(_menuFilePath);
                    _menuItems = ParseXml(xml);
                    DataCache.SetCache(cacheKey, _menuItems);
                }
                return _menuItems;
            }
        }

        public static List<MenuItem> GetMenuItemsByPath(string path)
        {
            var _menuFilePath = HostingEnvironment.MapPath(_MenuFileVirtualPath);
            var xml = XElement.Load(_menuFilePath);
            if (!string.IsNullOrEmpty(path))
            {
                var el = xml.XPathSelectElement(path);
                if (el != null)
                    xml = el;
            }
            var items = ParseXml(xml);
            return items;
        }

        public static List<MenuItem> GetFilteredMenuItems(HotcakesApplication hccApp)
        {
            return MenuItems
                .Where(
                    mi =>
                        string.IsNullOrEmpty(mi.PermissionToken) ||
                        hccApp.MembershipServices.HasCurrentUserPermission(mi.PermissionToken, hccApp))
                .ToList();
        }

        public static List<MenuItem> GetFilteredMenuItems(string path, HotcakesApplication hccApp)
        {
            return GetMenuItemsByPath(path)
                .Where(
                    mi =>
                        string.IsNullOrEmpty(mi.PermissionToken) ||
                        hccApp.MembershipServices.HasCurrentUserPermission(mi.PermissionToken, hccApp))
                .ToList();
        }

        public static MenuItem GetByBaseUrl(HotcakesApplication hccApp, string url, int level = 1)
        {
            MenuItem item = null;
            var items = GetFilteredMenuItems(hccApp);

            while (level > 0)
            {
                item = items.FirstOrDefault(mi => IsCurrent(mi, url));

                if (item == null)
                {
                    return null;
                }

                items = item.ChildItems;
                level--;
            }

            return item;
        }

        public static bool IsCurrent(MenuItem mi, string url)
        {
            if (!string.IsNullOrEmpty(mi.BaseUrl))
            {
                return url.StartsWith(mi.BaseUrl, StringComparison.InvariantCultureIgnoreCase);
            }

            if (!string.IsNullOrEmpty(mi.Url))
            {
                var miUrl = mi.Url.Replace(".aspx", string.Empty);
                if (miUrl.IndexOf("?") > -1)
                {
                    miUrl = miUrl.Substring(0, miUrl.IndexOf("?"));
                }
                return url.StartsWith(miUrl, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        private static List<MenuItem> ParseXml(XElement el)
        {
            if (!el.HasElements)
            {
                return new List<MenuItem>();
            }

            localization = Factory.Instance.CreateLocalizationHelper(localResourceFile);

            return el.Elements()
                .Select(e => new MenuItem
                {
                    Name = e.GetAttributeValue("Name"),
                    //Localize Text String that Loaded from Menu.xml
                    Text = localization.GetString(e.GetAttributeValue("Text")),
                    BaseUrl = e.GetAttributeValue("BaseUrl"),
                    Url = e.GetAttributeValue("Url"),
                    PermissionToken = e.GetAttributeValue("Permission"),
                    HiddenInDnnMenu = ParseBool(e.GetAttributeValue("HiddenInDnnMenu")),
                    ChildItems = ParseXml(e)
                }).ToList();
        }

        private static bool ParseBool(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return bool.Parse(value);
        }
    }
}