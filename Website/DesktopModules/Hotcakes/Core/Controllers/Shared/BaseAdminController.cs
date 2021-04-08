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
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Controllers.Shared
{
    [Serializable]
    public class BaseAdminController : BaseAppController
    {
        public AdminTabType SelectedTab = AdminTabType.Dashboard;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ValidatAccess();

            // Jquery
            ViewData["JQueryInclude"] = Html.JQueryIncludes(Url.Content("~/DesktopModules/Hotcakes/Core/Scripts"),
                Request.IsSecureConnection);

            ViewData["AppVersion"] = WebAppSettings.AppVersion.ToString();
            ViewData["StoreName"] = HccApp.CurrentStore.Settings.FriendlyName;
            ViewData["HideMenu"] = 0;
            ViewData["HideAdminControlBar"] = 0;
        }

        public void ValidatAccess()
        {
            if (!HccApp.MembershipServices.IsUserLoggedIn())
            {
                Response.Redirect(HccApp.MembershipServices.GetLoginPagePath());
            }
        }
    }
}