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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.UI.ControlPanels;
using Hotcakes.Commerce;
using Hotcakes.Modules.Core.Admin.AppCode;
using MenuItem = Hotcakes.Modules.Core.Admin.AppCode.MenuItem;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Controllers;
using System.Reflection;

namespace Hotcakes.Modules.ControlPanel
{
    public partial class AdminControlBar : ControlPanelBase
    {
        #region Protected Methods

        protected string GetUrl(IDataItemContainer cont)
        {
            var mi = cont.DataItem as MenuItem;
            return ResolveUrl(mi.GetUrl());
        }

        #endregion

        #region Properties

        public HotcakesApplication HccApp
        {
            get { return HccPage.HccApp; }
        }

        public IHccPage HccPage
        {
            get { return (IHccPage) Page; }
        }

        #endregion

        #region Page Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                BindMenu();
            }

            conrolbar_logo.ImageUrl = "~/DesktopModules/Hotcakes/ControlPanel/controlbarimages/admin_logo.png";
            aHostAdmin.Visible = HccApp.MembershipServices.IsSuperUserLoggedIn();
            aHostAdmin.HRef = VirtualPathUtility.ToAbsolute("~/DesktopModules/Hotcakes/Core/Admin/HostAdmin.aspx");

            UserInfo objUser = UserController.Instance.GetCurrentUserInfo();
            if (objUser != null && objUser.IsSuperUser)
            {
                Version version = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "bin\\Hotcakes.Commerce.dll").GetName().Version;                
                updateService.ImageUrl = "https://hotcakes.org/DesktopModules/HCC/UpdateService/update.aspx?version=" + version.Major.ToString("00") + version.Minor.ToString("00") + version.Build.ToString("00") + "&type=Module&name=Hotcakes&no=1&id=" + HostController.Instance.GetString("GUID");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        #endregion

        #region Private Method

        private void BindMenu()
        {
            rpMenuTabs.DataSource = MenuProvider.GetFilteredMenuItems(HccApp);
            rpMenuTabs.DataBind();
        }


        //Binding the data
        protected void rpMenuTabs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var menu = (MenuItem) e.Item.DataItem;
                if (menu.ChildItems.Any())
                {
                    var repSubMenu = e.Item.FindControl("rpMenuItems") as Repeater;
                    if (repSubMenu != null)
                    {
                        repSubMenu.DataSource = menu.ChildItems;
                        repSubMenu.DataBind();
                    }
                }
            }
        }

        #endregion
    }
}