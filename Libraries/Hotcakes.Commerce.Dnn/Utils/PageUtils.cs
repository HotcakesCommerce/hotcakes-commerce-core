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
using System.IO;
using System.Web.Hosting;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Security.Permissions;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Dnn.Utils
{
    [Serializable]
    public static class PageUtils
    {
        public static bool ShowPageUrls()
        {
            var fupConfig = ProviderConfiguration.GetProviderConfiguration("friendlyUrl");
            var defaultFriendlyUrlProvider = fupConfig.DefaultProvider;
            var showPageUrls = defaultFriendlyUrlProvider == "HccFriendlyUrl";
            return showPageUrls;
        }

        public static void EnsureTabsExist(StoreSettingsUrls sett)
        {
            int? parentId = null;
            Func<int> getParentTabId = () =>
            {
                return parentId.HasValue
                    ? parentId.Value
                    : (parentId = FindOrCreateFirstLevelTab("HotcakesStore")).Value;
            };

            if (sett.CategoryTabId <= 0)
                sett.CategoryTabId = CreateTabFromTemplate("Category", getParentTabId());
            if (sett.ProductTabId <= 0)
                sett.ProductTabId = CreateTabFromTemplate("Product", getParentTabId());
            if (sett.ProductReviewTabId <= 0)
                sett.ProductReviewTabId = CreateTabFromTemplate("ProductReview", getParentTabId());
            if (sett.SearchTabId <= 0)
                sett.SearchTabId = CreateTabFromTemplate("Search", getParentTabId());
            if (sett.CartTabId <= 0)
                sett.CartTabId = CreateTabFromTemplate("Cart", getParentTabId());
            if (sett.CheckoutTabId <= 0)
                sett.CheckoutTabId = CreateTabFromTemplate("Checkout", getParentTabId());

            if (sett.WishListTabId <= 0)
                sett.WishListTabId = CreateTabFromTemplate("WishList", getParentTabId());
            if (sett.OrderHistoryTabId <= 0)
                sett.OrderHistoryTabId = CreateTabFromTemplate("OrderHistory", getParentTabId());
            if (sett.AddressBookTabId <= 0)
                sett.AddressBookTabId = CreateTabFromTemplate("AddressBook", getParentTabId());
        }

        private static int CreateTabFromTemplate(string templateName, int parentTabId)
        {
            var virtualPath = string.Format("~/desktopmodules/hotcakes/core/content/pagetemplates/{0}.page.template",
                templateName);
            var path = HostingEnvironment.MapPath(virtualPath);
            if (File.Exists(path))
            {
                var portalSettings = DnnGlobal.Instance.GetCurrentPortalSettings();

                var doc = new XmlDocument();
                doc.Load(path);
                var tabNode = doc.SelectSingleNode("portal/tab");
                var tabName = tabNode.SelectSingleNode("name").InnerText;

                var tabController = new TabController();
                var tab = tabController.GetTabByName(tabName, portalSettings.PortalId, parentTabId);
                tab = TabController.DeserializeTab(tabNode, tab, portalSettings.PortalId,
                    PortalTemplateModuleAction.Merge);

                return tab.TabID;
            }
            return -1;
        }

        private static TabInfo CreateNewTab(string tabName)
        {
            var portalSettings = PortalSettings.Current;
            var name = GetAvailableTabName(tabName, string.Empty);

            var tab = new TabInfo();
            tab.PortalID = portalSettings.PortalId;
            tab.TabName = name;
            tab.Title = name;
            tab.IsVisible = true;
            tab.DisableLink = false;
            tab.IsDeleted = false;
            tab.IsSuperTab = false;

            foreach (PermissionInfo p in PermissionController.GetPermissionsByTab())
            {
                switch (p.PermissionKey)
                {
                    case "VIEW":
                        AddTabPermission(tab, p, portalSettings.AdministratorRoleId, true);
                        break;
                    case "EDIT":
                        AddTabPermission(tab, p, portalSettings.AdministratorRoleId, true);
                        break;
                }
            }

            var tabController = new TabController();

            tabController.AddTab(tab, true);
            return tab;
        }

        private static void AddTabPermission(TabInfo tab, PermissionInfo permission, int roleId, bool allowed)
        {
            var tpi = new TabPermissionInfo
            {
                PermissionID = permission.PermissionID,
                PermissionKey = permission.PermissionKey,
                PermissionName = permission.PermissionName,
                AllowAccess = allowed,
                RoleID = roleId
            };
            tab.TabPermissions.Add(tpi);
        }

        private static string GetAvailableTabName(string tabName, string parentTabPath)
        {
            var fullPath = parentTabPath + "//" + tabName;
            var tabPath = fullPath;
            var i = 0;
            var dict = TabController.GetTabPathDictionary(PortalSettings.Current.PortalId,
                PortalSettings.Current.CultureCode);

            while (dict.ContainsKey(tabPath))
            {
                tabPath = fullPath + Convert.ToString(++i);
            }

            return tabName + (i > 0 ? Convert.ToString(i) : string.Empty);
        }

        private static int FindOrCreateFirstLevelTab(string tabName)
        {
            var portal = DnnGlobal.Instance.GetCurrentPortal();
            var tabPath = Globals.GenerateTabPath(Null.NullInteger, tabName);

            var tabId = TabController.GetTabByTabPath(portal.PortalID, tabPath, portal.CultureCode);
            var tabController = new TabController();
            if (tabId > Null.NullInteger)
            {
                return tabId;
            }

            return CreateTabFromTemplate(tabName, -1);
        }
    }
}