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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Application;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Personalization;
using DotNetNuke.UI.ControlPanels;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Common;
using DotNetNuke.Web.Components.Controllers;
using DotNetNuke.Web.Components.Controllers.Models;
using DotNetNuke.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Globals = DotNetNuke.Common.Globals;
using MenuItem = Hotcakes.Modules.Core.Admin.AppCode.MenuItem;

namespace Hotcakes.Modules.ControlPanel
{
    /// <summary>
    /// </summary>
    public partial class ControlBar : ControlPanelBase
    {
        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
            }

        #region Variables

        protected DnnFileUpload FileUploader;

        protected string CurrentUICulture { get; set; }

        protected string LoginUrl { get; set; }

        protected string LoadTabModuleMessage { get; set; }

        protected string BookmarkModuleCategory
        {
            get { return ControlBarController.Instance.GetBookmarkCategory(PortalSettings.PortalId); }
            }

        protected string BookmarkedModuleKeys
        {
            get
            {
                var bookmarkModules = Personalization.GetProfile("ControlBar", "module" + PortalSettings.PortalId);
                if (bookmarkModules == null)
                {
                    return string.Empty;
                }
                return bookmarkModules.ToString();
            }
        }

        public override bool IsDockable { get; set; }

        public override bool IncludeInControlHierarchy
        {
            get { return base.IncludeInControlHierarchy && (IsPageAdmin() || IsModuleAdmin()); }
            }

        //DnnComboBox variables. DNN 8 has moved this control to
        //other assemblies so cannot be declared directly as it gives
        //control not found error when used with DNN 8
        private dynamic controlBar_SwitchSite;
        private dynamic controlBar_SwitchLanguage;
        private dynamic CategoryList;
        private dynamic VisibilityLst;

        #endregion

        #region Page Events

        protected override void OnLoad(EventArgs e)
        {
            Trace.Write("ControlBar - OnLoad");
            base.OnLoad(e);

            if (Host.EnableModuleOnLineHelp)
            {
                helpLink.Text = string.Format(@"<li><a href=""{0}"" target=""_blank"">{1}</a></li>", Host.HelpURL,
                    GetString("Tool.Help.ToolTip"));
            }

            LoginUrl = ResolveClientUrl("~/Login.aspx");

            if (ControlPanel.Visible && IncludeInControlHierarchy)
            {
                ClientResourceManager.RegisterStyleSheet(Page, "~/DesktopModules/Hotcakes/ControlPanel/ControlBar.css");
                ClientResourceManager.RegisterStyleSheet(Page,
                    "~/DesktopModules/Hotcakes/ControlPanel/PerformanceBar.css");

                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                //Controlbar js is changed on the higher versions of the DNN so in order to fix the JS issue it required to load
                //proper JS file based on the version
                var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version;
                if (versionofDNN >= new Version("7.4.2"))
                {
                    //This JS file hsas been created based on the DNN 7.4.2 version and copied to our hotcakes version
                    ClientResourceManager.RegisterScript(Page,
                        "~/DesktopModules/Hotcakes/ControlPanel/scripts/dnn.controlBar_DNN742.js");
                }
                else
                {
                    ClientResourceManager.RegisterScript(Page,
                        "~/DesktopModules/Hotcakes/ControlPanel/scripts/dnn.controlBar.js");
                }

                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/ControlPanel/scripts/hcc.product.performanceBar.js");
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/ControlPanel/scripts/hcc.category.performanceBar.js");
                ClientResourceManager.RegisterScript(this.Page, "~/Resources/Shared/Scripts/knockout.js");
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/knockout.mapping.js");
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.core.js");
				ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.min.js");
				ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.js");
				ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.Legend.js");
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.product.performance.js");
                ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.category.performance.js");

                // Is there more than one site in this group?
                var multipleSites = GetCurrentPortalsGroup().Count() > 1;
                ClientAPI.RegisterClientVariable(Page, "moduleSharing", multipleSites.ToString().ToLowerInvariant(),
                    true);
            }

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            var multipleSite = false;

            //conrolbar_logo.ImageUrl = ControlBarController.Instance.GetControlBarLogoURL();
            conrolbar_logo.ImageUrl = "~/DesktopModules/Hotcakes/ControlPanel/controlbarimages/admin_logo.png";
            if (!IsPostBack)
            {
                LoadCategoryList();
                multipleSite = LoadSiteList();
                LoadVisibilityList();
                AutoSetUserMode();
                BindPortalsList();
                BindLanguagesList();
            }

            LoadTabModuleMessage = multipleSite
                ? GetString("LoadingTabModuleCE.Text")
                : GetString("LoadingTabModule.Text");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);        

            LoadDNNControls();

            // This has been added to resolve KO undefined error when trial site setup from Marketing site
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/knockout.js", FileOrder.Js.jQuery + 1);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);

            //page will be null if the control panel initial twice, it will be removed in the second time.
            if (Page != null)
            {
               ID = "ControlBar";

                var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version;
               if (versionofDNN < new Version("8.0"))
               {
                   LoadGettingStarted();
               }

                FileUploader = new DnnFileUpload {ID = "fileUploader", SupportHost = false};
                Page.Form.Controls.Add(FileUploader);

                LoadCustomMenuItems();

                LoadPerformanceBar();
            }
        }

        private void LoadGettingStarted()
        {
           // ID = "ControlBar";
           // var gettingStarted = DnnGettingStarted.GetCurrent(Page);
           // if (gettingStarted == null)
           // {
           //     gettingStarted = new DnnGettingStarted();
           //     Page.Form.Controls.Add(gettingStarted);
           // }
        }
      
        /// <summary>
        ///     Create the DnnComboBox from proper assemblies based on the DNN version.
        ///     DnnComboBox belongs to DotNetNuke.Web.dll in the DNN version below 8.0 and
        ///     DnnComboBox belongs to DotNetNuke.Web.Deprecated.dll in the DNN version >= 8.0
        /// </summary>
        private void LoadDNNControls()
        {
            var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version; //Get current DNN version
            var AssemblyPath = string.Empty;
            if (versionofDNN < new Version("8.0"))
                //Check the current version of the DNN and load the appropriate assembly
            {
                AssemblyPath = HttpContext.Current.Server.MapPath(Path.Combine("bin", "DotNetNuke.Web.dll"));
            }
            else
            {
                AssemblyPath = HttpContext.Current.Server.MapPath(Path.Combine("bin", "DotNetNuke.Web.Deprecated.dll"));
            }

            //Load the assembly dynamically
            var assembly = Assembly.LoadFrom(AssemblyPath);

            //Create combobox control with reflection 
            var type = "DnnComboBox";
            controlBar_SwitchLanguage =
                Activator.CreateInstance(assembly.GetType("DotNetNuke.Web.UI.WebControls." + type));
            controlBar_SwitchSite = Activator.CreateInstance(assembly.GetType("DotNetNuke.Web.UI.WebControls." + type));
            CategoryList = Activator.CreateInstance(assembly.GetType("DotNetNuke.Web.UI.WebControls." + type));
            VisibilityLst = Activator.CreateInstance(assembly.GetType("DotNetNuke.Web.UI.WebControls." + type));

            //Set properties as it was present in ascx control
            controlBar_SwitchSite.ID = "controlBar_SwitchSite";
            controlBar_SwitchSite.ClientIDMode = ClientIDMode.Static;
            controlBar_SwitchSite.Skin = "DnnBlack";
            controlBar_SwitchSite.ViewStateMode = ViewStateMode.Disabled;

            //Add control back to placeholder in the ascx control dynamically
            phcontrolbar_switchsite.Controls.Add(controlBar_SwitchSite);

            //Set properties as it was present in ascx control
            controlBar_SwitchLanguage.ID = "controlBar_SwitchLanguage";
            controlBar_SwitchLanguage.ClientIDMode = ClientIDMode.Static;
            controlBar_SwitchLanguage.Skin = "DnnBlack";
            controlBar_SwitchLanguage.ViewStateMode = ViewStateMode.Disabled;

            //Add control back to placeholder in the ascx control dynamically
            phcontrolBar_SwitchLanguage.Controls.Add(controlBar_SwitchLanguage);

            //Set properties as it was present in ascx control
            CategoryList.ID = "CategoryList";
            CategoryList.ClientIDMode = ClientIDMode.Static;
            CategoryList.Skin = "DnnBlack";
            CategoryList.ViewStateMode = ViewStateMode.Disabled;
            CategoryList.DataTextField = "Name";
            CategoryList.DataValueField = "Name";
            CategoryList.OnClientSelectedIndexChanged = "dnn.controlBar.ControlBar_Module_CategoryList_Changed";

            //Add control back to placeholder in the ascx control dynamically
            phCategoryList.Controls.Add(CategoryList);

            //Set properties as it was present in ascx control
            VisibilityLst.ID = "VisibilityLst";
            VisibilityLst.ClientIDMode = ClientIDMode.Static;
            VisibilityLst.Skin = "DnnBlack";
            VisibilityLst.CssClass = "dnnLeftComboBox";
            VisibilityLst.ViewStateMode = ViewStateMode.Disabled;
            VisibilityLst.Enabled = false;

            //Add control back to placeholder in the ascx control dynamically
            phVisibilityLst.Controls.Add(VisibilityLst);
        }

        #endregion

        #region Private Methods

        private void LoadCustomMenuItems()
        {
            foreach (var menuItem in ControlBarController.Instance.GetCustomMenuItems())
            {
                var liElement = new HtmlGenericControl("li");
                liElement.Attributes.Add("id", menuItem.ID + "_tab");

                var control = Page.LoadControl(menuItem.Source);
                control.ID = menuItem.ID;

                liElement.Controls.Add(control);

                CustomMenuItems.Controls.Add(liElement);
            }
        }

        private void LoadPerformanceBar()
        {
            var tabId = PortalSettings.ActiveTab.TabID;
            var store = HccRequestContext.Current.CurrentStore;
            var slug = Request.Params["slug"];

            if (store != null && !string.IsNullOrEmpty(slug))
            {
                if (store.Settings.Urls.ProductTabId == tabId)
                {
                    var performanceBar = LoadControl("ProductPerformanceBar.ascx");
                    phPerformanceBar.Controls.Add(performanceBar);
                }
                if (store.Settings.Urls.CategoryTabId == tabId)
                {
                    var performanceBar = LoadControl("CategoryPerformanceBar.ascx");
                    phPerformanceBar.Controls.Add(performanceBar);
                }
            }
        }

        private void LoadCategoryList()
        {
            var termController = Util.GetTermController();
            var terms =
                termController.GetTermsByVocabulary("Module_Categories")
                    .OrderBy(t => t.Weight)
                    .Where(t => t.Name != "< None >")
                    .ToList();
            var allTerm = new Term("All", Localization.GetString("AllCategories", LocalResourceFile));
            terms.Add(allTerm);
            CategoryList.DataSource = terms;
            CategoryList.DataBind();
            if (!IsPostBack)
            {
                CategoryList.Select(!string.IsNullOrEmpty(BookmarkedModuleKeys) ? BookmarkModuleCategory : "All", false);
            }
        }

        private bool LoadSiteList()
        {
            // Is there more than one site in this group?
            var multipleSites = GetCurrentPortalsGroup().Count() > 1;
            if (multipleSites)
            {
                PageList.Services.GetTreeMethod = "ItemListService/GetPagesInPortalGroup";
                PageList.Services.GetNodeDescendantsMethod = "ItemListService/GetPageDescendantsInPortalGroup";
                PageList.Services.SearchTreeMethod = "ItemListService/SearchPagesInPortalGroup";
                PageList.Services.GetTreeWithNodeMethod = "ItemListService/GetTreePathForPageInPortalGroup";
                PageList.Services.SortTreeMethod = "ItemListService/SortPagesInPortalGroup";
            }

            PageList.UndefinedItem = new ListItem(SharedConstants.Unspecified, string.Empty);
            PageList.OnClientSelectionChanged.Add("dnn.controlBar.ControlBar_Module_PageList_Changed");
            return multipleSites;
        }

        private void LoadVisibilityList()
        {
            var items = new Dictionary<string, string>
            {
                {"0", GetString("PermissionView")},
                {"1", GetString("PermissionEdit")}
            };

            VisibilityLst.Items.Clear();
            VisibilityLst.DataValueField = "key";
            VisibilityLst.DataTextField = "value";
            VisibilityLst.DataSource = items;
            VisibilityLst.DataBind();
        }

        private static IEnumerable<PortalInfo> GetCurrentPortalsGroup()
        {
            var groups = PortalGroupController.Instance.GetPortalGroups().ToArray();

            var result = (from @group in groups
                          select PortalGroupController.Instance.GetPortalsByGroup(@group.PortalGroupId)
                              into portals
                              where portals.Any(x => x.PortalID == PortalSettings.Current.PortalId)
                              select portals.ToArray()).FirstOrDefault();

            // Are we in a group of one?
            if (result == null || result.Length == 0)
            {
                result = new[] {PortalController.Instance.GetPortal(PortalSettings.Current.PortalId)};
            }

            return result;
        }

        private void AutoSetUserMode()
        {
            var tabId = PortalSettings.ActiveTab.TabID;
            var portalId = PortalSettings.Current.PortalId;
            var pageId = string.Format("{0}:{1}", portalId, tabId);

            var cookie = Request.Cookies["StayInEditMode"];
            if (cookie != null && cookie.Value == "YES")
            {
                if (PortalSettings.Current.UserMode != PortalSettings.Mode.Edit)
                {
                    SetUserMode("EDIT");
                    SetLastPageHistory(pageId);
                    Response.Redirect(Request.RawUrl, true);
                }

                return;
            }

            var lastPageId = GetLastPageHistory();
            var isShowAsCustomError = Request.QueryString.AllKeys.Contains("aspxerrorpath");

            if (lastPageId != pageId && !isShowAsCustomError)
            {
                // navigate between pages
                if (PortalSettings.Current.UserMode != PortalSettings.Mode.View)
                {
                    SetUserMode("VIEW");
                    SetLastPageHistory(pageId);
                    Response.Redirect(Request.RawUrl, true);
                }
            }

            if (!isShowAsCustomError)
            {
                SetLastPageHistory(pageId);
            }
        }

        private void SetLastPageHistory(string pageId)
        {
            var newCookie = new HttpCookie("LastPageId", pageId);
            Response.Cookies.Add(newCookie);
        }

        private string GetLastPageHistory()
        {
            var cookie = Request.Cookies["LastPageId"];
            if (cookie != null)
                return cookie.Value;

            return "NEW";
        }

        private void SetLanguage(bool update, string currentCulture)
        {
            if (update)
            {
                Personalization.SetProfile("Usability", "UICulture", currentCulture);
            }
        }

        private void BindPortalsList()
        {
            // Create the DnnComboBoxItem from proper assemblies based on the DNN version.
            // DnnComboBoxItem belongs to DotNetNuke.Web.dll in the DNN version < 8.0
            // DnnComboBoxItem belongs to DotNetNuke.Web.Deprecated.dll in the DNN version >= 8.0 
            var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version; // Get DNN version

            var AssemblyPath = string.Empty;
            if (versionofDNN < new Version("8.0")) //Check if DNN Versio is not 8 
            {
                AssemblyPath = HttpContext.Current.Server.MapPath(Path.Combine("bin", "DotNetNuke.Web.dll"));
            }
            else
            {
                AssemblyPath = HttpContext.Current.Server.MapPath(Path.Combine("bin", "DotNetNuke.Web.Deprecated.dll"));
            }

            var assembly = Assembly.LoadFrom(AssemblyPath); // Load assembly dynamically
            dynamic cmbBoxItem;
            foreach (var portal in LoadPortalsList())
            {
                //Create instance of the DnnComboBoxItem
                var type = "DnnComboBoxItem";
                cmbBoxItem = Activator.CreateInstance(assembly.GetType("DotNetNuke.Web.UI.WebControls." + type));

                //Set required properties for the DnnComboBoxItem
                cmbBoxItem.Text = portal[0];
                cmbBoxItem.Value = portal[1];
                controlBar_SwitchSite.Items.Add(cmbBoxItem);
            }
        }


        private void BindLanguagesList()
        {
            if (ShowSwitchLanguagesPanel())
            {
                // Create the DnnComboBoxItem from proper assemblies based on the DNN version.
                // DnnComboBoxItem belongs to DotNetNuke.Web.dll in the DNN version < 8.0
                // DnnComboBoxItem belongs to DotNetNuke.Web.Deprecated.dll in the DNN version >= 8.0 
                var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version; // Get DNN version
                var AssemblyPath = string.Empty;
                if (versionofDNN < new Version("8.0")) //Check if DNN Versio is not 8 
                {
                    AssemblyPath = HttpContext.Current.Server.MapPath(Path.Combine("bin", "DotNetNuke.Web.dll"));
                }
                else
                {
                    AssemblyPath =
                        HttpContext.Current.Server.MapPath(Path.Combine("bin", "DotNetNuke.Web.Deprecated.dll"));
                }

                var assembly = Assembly.LoadFrom(AssemblyPath); // Load assembly dynamically
                dynamic cmbBoxItem;
                var type = "DnnComboBoxItem";
                const string FlagImageUrlFormatString = "~/images/Flags/{0}.gif";
                foreach (var lang in LoadLanguagesList())
                {
                    //Create instance of the DnnComboBoxItem and set required properties
                    cmbBoxItem = Activator.CreateInstance(assembly.GetType("DotNetNuke.Web.UI.WebControls." + type));
                    cmbBoxItem.Text = lang[0];
                    cmbBoxItem.Value = lang[1];
                    //var item = new DnnComboBoxItem(lang[0], lang[1]);
                    cmbBoxItem.ImageUrl = string.Format(FlagImageUrlFormatString, cmbBoxItem.Value);
                    if (lang[2] == "true")
                    {
                        cmbBoxItem.Selected = true;
                    }

                    controlBar_SwitchLanguage.Items.Add(cmbBoxItem);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected string GetUpgradeIndicator()
        {
            UserInfo objUser = UserController.Instance.GetCurrentUserInfo();

            if (objUser != null && objUser.IsSuperUser)
            {
                var upgradeIndicator =
                    ControlBarController.Instance.GetUpgradeIndicator(DotNetNukeContext.Current.Application.Version,
                        Request.IsLocal, Request.IsSecureConnection);
                if (upgradeIndicator == null)
                {
                    return String.Empty;
                }
                return GetUpgradeIndicatorButton(upgradeIndicator);
            }

            return string.Empty;
        }

        private string GetUpgradeIndicatorButton(UpgradeIndicatorViewModel upgradeIndicator)
        {
            return string.Format("<a id=\"{0}\" href=\"#\" onclick=\"{1}\" class=\"{2}\"><img src=\"{3}\" alt=\"{4}\" title=\"{5}\"/></a>",
                upgradeIndicator.ID, upgradeIndicator.WebAction, upgradeIndicator.CssClass, ResolveClientUrl(upgradeIndicator.ImageUrl), upgradeIndicator.AltText, upgradeIndicator.ToolTip);
        }

        protected string GetTabURL(List<string> additionalParams, string toolName, bool isHostTool,
                                   string moduleFriendlyName, string controlKey, bool showAsPopUp)
        {
            var portalId = isHostTool ? Null.NullInteger : PortalSettings.PortalId;

            var strURL = string.Empty;

            if (additionalParams == null)
            {
                additionalParams = new List<string>();
            }

            var moduleInfo = ModuleController.Instance.GetModuleByDefinition(portalId, moduleFriendlyName);

            if (moduleInfo != null)
            {
                var isHostPage = portalId == Null.NullInteger;
                if (!string.IsNullOrEmpty(controlKey))
                {
                    additionalParams.Insert(0, "mid=" + moduleInfo.ModuleID);
                    if (showAsPopUp && PortalSettings.EnablePopUps)
                    {
                        additionalParams.Add("popUp=true");
                    }
                }

                var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
                strURL = Globals.NavigateURL(moduleInfo.TabID, isHostPage, PortalSettings, controlKey, currentCulture,
                    additionalParams.ToArray());
            }

            return strURL;
        }

        protected string GetTabURL(string tabName, bool isHostTool)
        {
            return GetTabURL(tabName, isHostTool, null);
        }

        protected string GetTabURL(string tabName, bool isHostTool, int? parentId)
        {
            if (isHostTool && !UserController.Instance.GetCurrentUserInfo().IsSuperUser)
            {
                return "javascript:void(0);";
            }

            var portalId = isHostTool ? Null.NullInteger : PortalSettings.PortalId;
            return GetTabURL(tabName, portalId, parentId);
        }

        protected string GetTabURL(string tabName, int portalId, int? parentId)
        {
            var tab = parentId.HasValue
                ? TabController.Instance.GetTabByName(tabName, portalId, parentId.Value)
                : TabController.Instance.GetTabByName(tabName, portalId);

            if (tab != null)
            {
                return tab.FullUrl;
            }

            return string.Empty;
        }

        protected string GetString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }

        protected string GetAdminBaseMenu()
        {
            var tabs = AdminBaseTabs;
            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                var hideBookmark = AdminBookmarkItems.Contains(tab.TabName);
                sb.Append(GetMenuItem(tab, false, hideBookmark));
            }

            return sb.ToString();
        }

        protected string GetAdminAdvancedMenu()
        {
            var tabs = AdminAdvancedTabs;
            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                var hideBookmark = AdminBookmarkItems.Contains(tab.TabName);
                sb.Append(GetMenuItem(tab, false, hideBookmark));
            }

            return sb.ToString();
        }

        protected string GetBookmarkItems(string title)
        {
            List<string> bookmarkItems;
            var isHostTool = title == "host";
            if (isHostTool)
                bookmarkItems = HostBookmarkItems;
            else
                bookmarkItems = AdminBookmarkItems;

            if (bookmarkItems != null && bookmarkItems.Any())
            {
                var sb = new StringBuilder();
                foreach (var itemKey in bookmarkItems)
                {
                    sb.Append(GetMenuItem(itemKey, isHostTool, true));
                }
                return sb.ToString();
            }

            return string.Empty;
        }

        protected string GetMenuItem(string tabName, bool isHostTool)
        {
            if (isHostTool && !UserController.Instance.GetCurrentUserInfo().IsSuperUser)
            {
                return string.Empty;
            }

            List<TabInfo> tabList = null;
            if (isHostTool)
            {
                if (_hostTabs == null) GetHostTabs();
                tabList = _hostTabs;
            }
            else
            {
                if (_adminTabs == null) GetAdminTabs();
                tabList = _adminTabs;
            }

            var tab = tabList.SingleOrDefault(t => t.TabName == tabName);
            return GetMenuItem(tab);
        }

        protected string GetMenuItem(string tabName, bool isHostTool, bool isRemoveBookmark, bool isHideBookmark = false)
        {
            if (isHostTool && !UserController.Instance.GetCurrentUserInfo().IsSuperUser)
            {
                return string.Empty;
            }

            List<TabInfo> tabList = null;
            if (isHostTool)
            {
                if (_hostTabs == null) GetHostTabs();
                tabList = _hostTabs;
            }
            else
            {
                if (_adminTabs == null) GetAdminTabs();
                tabList = _adminTabs;
            }

            var tab = tabList.SingleOrDefault(t => t.TabName == tabName);
            return GetMenuItem(tab, isRemoveBookmark, isHideBookmark);
        }

        protected string GetMenuItem(TabInfo tab, bool isRemoveBookmark = false, bool isHideBookmark = false)
        {
            if (tab == null) return string.Empty;
            if (tab.IsVisible && !tab.IsDeleted && !tab.DisableLink)
            {
                var name = !string.IsNullOrEmpty(tab.LocalizedTabName) ? tab.LocalizedTabName : tab.Title;
                var linkClass = DotNetNukeContext.Current.Application.Name == "DNNCORP.CE" &&
                                tab.FullUrl.Contains("ProfessionalFeatures")
                    ? "class=\"PE\""
                    : string.Empty;
                if (!isRemoveBookmark)
                {
                    if (!isHideBookmark)
                        return
                            string.Format(
                                "<li data-tabname=\"{3}\"><a href=\"{0}\" {4}>{1}</a><a href=\"javascript:void(0)\" class=\"bookmark\" title=\"{2}\"><span></span></a></li>",
                                             tab.FullUrl,
                                             name,
                                             ClientAPI.GetSafeJSString(GetString("Tool.AddToBookmarks.ToolTip")),
                                             ClientAPI.GetSafeJSString(tab.TabName),
                                             linkClass);
                    return
                        string.Format(
                            "<li data-tabname=\"{3}\"><a href=\"{0}\" {4}>{1}</a><a href=\"javascript:void(0)\" class=\"bookmark hideBookmark\" data-title=\"{2}\"><span></span></a></li>",
                                            tab.FullUrl,
                                            name,
                                            ClientAPI.GetSafeJSString(GetString("Tool.AddToBookmarks.ToolTip")),
                                            ClientAPI.GetSafeJSString(tab.TabName),
                                            linkClass);
                }
                return
                    string.Format(
                        "<li data-tabname=\"{3}\"><a href=\"{0}\" {4}>{1}</a><a href=\"javascript:void(0)\" class=\"removeBookmark\" title=\"{2}\"><span></span></a></li>",
                                        tab.FullUrl,
                                        name,
                                        ClientAPI.GetSafeJSString(GetString("Tool.RemoveFromBookmarks.ToolTip")),
                                        ClientAPI.GetSafeJSString(tab.TabName),
                                        linkClass);
            }
            return string.Empty;
        }

        protected string GetHostBaseMenu()
        {
            var tabs = HostBaseTabs;

            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                var hideBookmark = HostBookmarkItems.Contains(tab.TabName);
                sb.Append(GetMenuItem(tab, false, hideBookmark));
            }

            return sb.ToString();
        }

        protected string GetHostAdvancedMenu()
        {
            var tabs = HostAdvancedTabs;
            var sb = new StringBuilder();
            foreach (var tab in tabs)
            {
                var hideBookmark = HostBookmarkItems.Contains(tab.TabName);
                sb.Append(GetMenuItem(tab, false, hideBookmark));
            }

            return sb.ToString();
        }

        protected bool ShowSwitchLanguagesPanel()
        {
            if (PortalSettings.AllowUserUICulture && PortalSettings.ContentLocalizationEnabled)
            {
                if (CurrentUICulture == null)
                {
                    var oCulture = Personalization.GetProfile("Usability", "UICulture");

                    if (oCulture != null)
                    {
                        CurrentUICulture = oCulture.ToString();
                    }
                    else
                    {
                        var l = new Localization();
                        CurrentUICulture = l.CurrentUICulture;
                    }
                }

                var cultureListItems = Localization.LoadCultureInListItems(CultureDropDownTypes.NativeName,
                    CurrentUICulture, string.Empty, false);
                return cultureListItems.Count() > 1;
            }

            return false;
        }

        protected bool CheckPageQuota()
        {
            var objUser = UserController.Instance.GetCurrentUserInfo();
            return (objUser != null && objUser.IsSuperUser) || PortalSettings.PageQuota == 0 ||
                   PortalSettings.Pages < PortalSettings.PageQuota;
        }

        protected string BuildToolUrl(string toolName, bool isHostTool, string moduleFriendlyName,
                                      string controlKey, string navigateUrl, bool showAsPopUp)
        {
            if (isHostTool && !UserController.Instance.GetCurrentUserInfo().IsSuperUser)
            {
                return "javascript:void(0);";
            }

            if (!string.IsNullOrEmpty(navigateUrl))
            {
                return navigateUrl;
            }

            var returnValue = "javascript:void(0);";
            switch (toolName)
            {
                case "PageSettings":
                    if (TabPermissionController.CanManagePage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab",
                            "action=edit&activeTab=settingTab");
                    }
                    break;
                case "CopyPage":
                    if (TabPermissionController.CanCopyPage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab",
                            "action=copy&activeTab=copyTab");
                    }
                    break;
                case "DeletePage":
                    if (TabPermissionController.CanDeletePage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab", "action=delete");
                    }
                    break;
                case "PageTemplate":
                    if (TabPermissionController.CanManagePage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab",
                            "action=edit&activeTab=advancedTab");
                    }
                    break;
                case "PageLocalization":
                    if (TabPermissionController.CanManagePage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab",
                            "action=edit&activeTab=localizationTab");
                    }
                    break;
                case "PagePermission":
                    if (TabPermissionController.CanAdminPage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "Tab",
                            "action=edit&activeTab=permissionsTab");
                    }
                    break;
                case "ImportPage":
                    if (TabPermissionController.CanImportPage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ImportTab");
                    }
                    break;
                case "ExportPage":
                    if (TabPermissionController.CanExportPage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "ExportTab");
                    }
                    break;
                case "NewPage":
                    if (TabPermissionController.CanAddPage())
                    {
                        returnValue = Globals.NavigateURL("Tab", "activeTab=settingTab");
                    }
                    break;
                case "PublishPage":
                    if (TabPermissionController.CanAdminPage())
                    {
                        returnValue = Globals.NavigateURL(PortalSettings.ActiveTab.TabID);
                    }
                    break;
                default:
                    if (!string.IsNullOrEmpty(moduleFriendlyName))
                    {
                        var additionalParams = new List<string>();
                        returnValue = GetTabURL(additionalParams, toolName, isHostTool,
                                                moduleFriendlyName, controlKey, showAsPopUp);
                    }
                    break;
            }
            return returnValue;
        }

        protected virtual bool ActiveTabHasChildren()
        {
            var children = TabController.GetTabsByParent(PortalSettings.ActiveTab.TabID,
                PortalSettings.ActiveTab.PortalID);

            if ((children == null) || children.Count < 1)
            {
                return false;
            }

            return true;
        }

        protected string SpecialClassWhenNotInViewMode()
        {
            return UserMode == PortalSettings.Mode.View ? string.Empty : "controlBar_editPageInEditMode";
        }

        protected string CheckedWhenStayInEditMode()
        {
            var checkboxState = string.Empty;
            var cookie = Request.Cookies["StayInEditMode"];
            if (cookie != null && cookie.Value == "YES")
            {
                checkboxState = "checked='checked'";
            }

            if (UserMode == PortalSettings.Mode.Layout)
            {
                checkboxState += " disabled='disabled'";
            }

            return checkboxState;
        }

        protected string GetEditButtonLabel()
        {
            return UserMode == PortalSettings.Mode.Edit
                ? GetString("Tool.CloseEditMode.Text")
                : GetString("Tool.EditThisPage.Text");
        }

        protected string CheckedWhenInLayoutMode()
        {
            return UserMode == PortalSettings.Mode.Layout ? "checked='checked'" : string.Empty;
        }

        protected string GetPublishActionText()
        {
            return TabPublishingController.Instance.IsTabPublished(TabController.CurrentPage.TabID,
                PortalSettings.PortalId)
                    ? ClientAPI.GetSafeJSString(GetString("Tool.UnpublishPage.Text"))
                    : ClientAPI.GetSafeJSString(GetString("Tool.PublishPage.Text"));
        }

        protected IEnumerable<string[]> LoadPaneList()
        {
            var panes = PortalSettings.Current.ActiveTab.Panes;
            var resultPanes = new List<string[]>();

            if (panes.Count < 4)
            {
                foreach (var p in panes)
                {
                    var topPane = new[]
                    {
                        string.Format(GetString("Pane.AddTop.Text"), p),
                        p.ToString(),
                        "TOP"
                    };

                    var botPane = new[]
                    {
                        string.Format(GetString("Pane.AddBottom.Text"), p),
                        p.ToString(),
                        "BOTTOM"
                    };

                    resultPanes.Add(topPane);
                    resultPanes.Add(botPane);
                }
            }
            else
            {
                foreach (var p in panes)
                {
                    var botPane = new[]
                    {
                        string.Format(GetString("Pane.Add.Text"), p),
                        p.ToString(),
                        "BOTTOM"
                    };

                    resultPanes.Add(botPane);
                }
            }

            return resultPanes;
        }

        protected string GetModeForAttribute()
        {
            return UserMode.ToString().ToUpperInvariant();
        }

        protected string GetButtonConfirmMessage(string toolName)
        {
            return
                ClientAPI.GetSafeJSString(Localization.GetString("Tool." + toolName + ".ConfirmText", LocalResourceFile));
        }

        protected string GetPublishConfirmHeader()
        {
            return TabPublishingController.Instance.IsTabPublished(TabController.CurrentPage.TabID,
                PortalSettings.PortalId)
                    ? ClientAPI.GetSafeJSString(GetButtonConfirmHeader("UnpublishPage"))
                    : ClientAPI.GetSafeJSString(GetButtonConfirmHeader("PublishPage"));
        }

        protected string GetPublishConfirmText()
        {
            return TabPublishingController.Instance.IsTabPublished(TabController.CurrentPage.TabID,
                PortalSettings.PortalId)
                    ? ClientAPI.GetSafeJSString(GetButtonConfirmMessage("UnpublishPage"))
                    : ClientAPI.GetSafeJSString(GetButtonConfirmMessage("PublishPage"));
        }

        protected string GetTabPublishing()
        {
            return TabPublishingController.Instance.IsTabPublished(TabController.CurrentPage.TabID,
                PortalSettings.PortalId)
                ? "true"
                : "false";
        }

        protected string PreviewPopup()
        {
            var previewUrl = string.Format("{0}/Default.aspx?ctl={1}&previewTab={2}&TabID={2}",
                                        Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias),
                                        "MobilePreview",
                                        PortalSettings.ActiveTab.TabID);

            if (PortalSettings.EnablePopUps)
            {
                return UrlUtils.PopUpUrl(previewUrl, this, PortalSettings, true, false, 660, 800);
            }
                return string.Format("location.href = \"{0}\"", previewUrl);
            }

        protected string GetButtonConfirmHeader(string toolName)
        {
            return
                ClientAPI.GetSafeJSString(Localization.GetString("Tool." + toolName + ".ConfirmHeader",
                    LocalResourceFile));
        }

        protected IEnumerable<string[]> LoadPortalsList()
        {
            var portals = PortalController.Instance.GetPortals();

            var result = new List<string[]>();
            foreach (var portal in portals)
            {
                var pi = portal as PortalInfo;

                if (pi != null)
                {
                    string[] p =
                    {
                        pi.PortalName,
                        pi.PortalID.ToString()
                    };

                    result.Add(p);
                }
            }

            return result;
        }

        protected IEnumerable<string[]> LoadLanguagesList()
        {
            var result = new List<string[]>();

            if (PortalSettings.AllowUserUICulture)
            {
                if (CurrentUICulture == null)
                {
                    var oCulture = Personalization.GetProfile("Usability", "UICulture");

                    if (oCulture != null)
                    {
                        CurrentUICulture = oCulture.ToString();
                    }
                    else
                    {
                        var l = new Localization();
                        CurrentUICulture = l.CurrentUICulture;
                        SetLanguage(true, CurrentUICulture);
                    }
                }


                var cultureListItems = Localization.LoadCultureInListItems(CultureDropDownTypes.NativeName,
                    CurrentUICulture, string.Empty, false);
                foreach (var cultureItem in cultureListItems)
                {
                    var selected = cultureItem.Value == CurrentUICulture ? "true" : "false";
                    string[] p =
                                     {
                                         cultureItem.Text,
                                         cultureItem.Value,
                                         selected
                                     };
                    result.Add(p);
                }
            }

            return result;
        }

        #endregion

        #region public Methods

        public string GetCommerce()
        {
            var menu = string.Empty;
            menu = "<li> " +
                   "<a href='/DesktopModules/Hotcakes/Core/Admin/default.aspx' resourcekey=\"Tool.Commerce\">" +
                   GetString("Tool.Commerce.Text") + " </a> " +
                     "<div class='subNav advanced'>";

            menu += GetCommerceMenu();

            menu += " </div> " +
                " </li> ";
            return menu;
        }

        public string GetCommerceMenu()
        {
            var renderMenu = string.Empty;
            var rednerSubMenu = string.Empty;
            var menuItems = MenuProvider.GetFilteredMenuItems(HccApp);
            var count = 0;
            var subMenu = string.Empty;
            if (menuItems.Count > 0)
            {
                foreach (var menu in menuItems)
                {
                    if (menu.HiddenInDnnMenu)
                        continue;

                    var cssActive = string.Empty;
                    if (count == 0)
                    {
                        cssActive = "active";
                        renderMenu += "<ul class=\"subNavToggle\">";
                    }
                    renderMenu += "<li class=\"" + cssActive + " hcDnnMenuIcon" + (count + 1) +
                                  "\"> <a href=\"#controlbar_commerce_basic_" + count + "\" title =\"" + menu.Text +
                                  "\"> <span></span></a></li>";
                    rednerSubMenu += GetCommerceSubMenu(cssActive, menu.ChildItems, count);
                    count++;
                }
            }
            return renderMenu + "</ul>" + rednerSubMenu;
        }

        public string GetCommerceSubMenu(string cssActive, List<MenuItem> childItems, int index)
        {
            var menu = string.Empty;
            if (childItems.Any())
            {
                menu += " <dl id=\"controlbar_commerce_basic_" + index + "\" class=\"" + cssActive + "\"> " +
                            " <dd> " +
                                " <ul> ";
                foreach (var subMenu in childItems)
                {
                    if (subMenu.HiddenInDnnMenu)
                        continue;

                    menu += string.Format("<li data-tabname=\"{2}\"><a href=\"{0}\">{1}</a></li>",
                                        ResolveUrl(subMenu.GetUrl()),
                                        subMenu.Text,
                                        subMenu.Text);
                }
                menu += "</ul> " +
                        " </dd> " +
                    " </dl> ";
            }

            return menu;
        }

        #endregion

        #region Menu Items Properties

        private List<string> _adminBookmarkItems;

        protected List<string> AdminBookmarkItems
        {
            get
            {
                if (_adminBookmarkItems == null)
                {
                    var bookmarkItems = Personalization.GetProfile("ControlBar", "admin" + PortalSettings.PortalId);

                    _adminBookmarkItems = bookmarkItems != null
                        ? bookmarkItems.ToString().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()
                                                : new List<string>();
                }

                return _adminBookmarkItems;
            }
        }

        private List<string> _hostBookmarkItems;

        protected List<string> HostBookmarkItems
        {
            get
            {
                if (_hostBookmarkItems == null)
                {
                    var bookmarkItems = Personalization.GetProfile("ControlBar", "host" + PortalSettings.PortalId);

                    _hostBookmarkItems = bookmarkItems != null
                        ? bookmarkItems.ToString().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList()
                                            : new List<string>();
                }

                return _hostBookmarkItems;
            }
        }

        private List<TabInfo> _adminTabs;
        private List<TabInfo> _adminBaseTabs;
        private List<TabInfo> _adminAdvancedTabs;
        private List<TabInfo> _hostTabs;
        private List<TabInfo> _hostBaseTabs;
        private List<TabInfo> _hostAdvancedTabs;

        protected List<TabInfo> AdminBaseTabs
        {
            get
            {
                if (_adminBaseTabs == null)
                {
                    GetAdminTabs();
                }
                return _adminBaseTabs;
            }
        }

        protected List<TabInfo> AdminAdvancedTabs
        {
            get
            {
                if (_adminAdvancedTabs == null)
                {
                    GetAdminTabs();
                }
                return _adminAdvancedTabs;
            }
        }

        protected List<TabInfo> HostBaseTabs
        {
            get
            {
                if (_hostBaseTabs == null)
                {
                    GetHostTabs();
                }
                return _hostBaseTabs;
            }
        }

        protected List<TabInfo> HostAdvancedTabs
        {
            get
            {
                if (_hostAdvancedTabs == null)
                {
                    GetHostTabs();
                }
                return _hostAdvancedTabs;
            }
        }

        private void GetHostTabs()
        {
            var hostTab = TabController.GetTabByTabPath(Null.NullInteger, "//Host", string.Empty);
            var hosts = TabController.GetTabsByParent(hostTab, -1);

            var professionalTab = TabController.Instance.GetTabByName("Professional Features", -1);
            List<TabInfo> professionalTabs;
            if (professionalTab != null)
            {
                professionalTabs = TabController.GetTabsByParent(professionalTab.TabID, -1);
            }
            else
            {
                professionalTabs = new List<TabInfo>();
            }

            _hostTabs = new List<TabInfo>();
            _hostTabs.AddRange(hosts);
            _hostTabs.AddRange(professionalTabs);
            _hostTabs = _hostTabs.OrderBy(t => t.LocalizedTabName).ToList();

            _hostBaseTabs = new List<TabInfo>();
            _hostAdvancedTabs = new List<TabInfo>();

            foreach (var tabInfo in _hostTabs)
            {
                switch (tabInfo.TabName)
                {
                    case "Host Settings":
                    case "Site Management":
                    case "File Management":
                    case "Extensions":
                    case "Dashboard":
                    case "Health Monitoring":
                    case "Technical Support":
                    case "Knowledge Base":
                    case "Software and Documentation":
                        _hostBaseTabs.Add(tabInfo);
                        break;
                    default:
                        _hostAdvancedTabs.Add(tabInfo);
                        break;
                }
            }
        }

        private void GetAdminTabs()
        {
            var adminTab = TabController.GetTabByTabPath(PortalSettings.PortalId, "//Admin", string.Empty);
            _adminTabs =
                TabController.GetTabsByParent(adminTab, PortalSettings.PortalId)
                    .OrderBy(t => t.LocalizedTabName)
                    .ToList();

            _adminBaseTabs = new List<TabInfo>();
            _adminAdvancedTabs = new List<TabInfo>();

            foreach (var tabInfo in _adminTabs)
            {
                switch (tabInfo.TabName)
                {
                    case "Site Settings":
                    case "Pages":
                    case "Security Roles":
                    case "User Accounts":
                    case "File Management":
                    case "Recycle Bin":
                    case "Log Viewer":
                        _adminBaseTabs.Add(tabInfo);
                        break;
                    default:
                        _adminAdvancedTabs.Add(tabInfo);
                        break;
                }
            }
        }

        #endregion
    }
}
