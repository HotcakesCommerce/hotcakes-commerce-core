﻿#region License

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

using DotNetNuke.Abstractions;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Security.Permissions;
using Hotcakes.Commerce.Dnn.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Hotcakes.Modules.ProductReviews
{
    public partial class ProductReviewsView : HotcakesModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ProductReviewsView()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected override string RenderView()
        {
            var slug = Request.Params["slug"];

            if (!string.IsNullOrEmpty(slug))
            {
                return MvcRenderingEngine.Render("ProductReviews", "Index", new {slug});
            }
            if (!TabPermissionController.CanManagePage())
            {
                if (PortalSettings.HomeTabId != Null.NullInteger)
                {
                    if (PortalSettings.HomeTabId != PortalSettings.ActiveTab.TabID)
                    {
                        Response.Redirect(_navigationManager.NavigateURL(PortalSettings.HomeTabId));
                    }
                }
            }
            return string.Empty;
        }
    }
}