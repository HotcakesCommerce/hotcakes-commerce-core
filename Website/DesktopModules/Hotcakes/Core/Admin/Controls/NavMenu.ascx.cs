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
using System.Web.UI;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class NavMenu : HccUserControl
    {
        public Func<List<MenuItem>, List<MenuItem>> ProcessItems;

        public NavMenu()
        {
            Level = 1;
        }

        public int Level { get; set; }
        public string BaseUrl { get; set; }
        public string CurrentUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMenu();
            }
        }

        private void BindMenu()
        {
            var url = GetBaseRelativePath();
            var baseMenuItem = MenuProvider.GetByBaseUrl(HccApp, url, Level);

            if (baseMenuItem != null)
            {
                var items = baseMenuItem.ChildItems;
                if (ProcessItems != null) items = ProcessItems(items.ToList());

                rpMenuItems.DataSource = items;
                rpMenuItems.DataBind();
            }
        }

        protected string GetCurrentClass(IDataItemContainer cont)
        {
            var url = GetCurrentRelativePath();
            var mi = cont.DataItem as MenuItem;
            var isCurrent = MenuProvider.IsCurrent(mi, url);

            if (string.IsNullOrEmpty(mi.Url))
                return isCurrent ? " class='hcCurrent hcDisabled'" : " class='hcDisabled'";
            return isCurrent ? " class='hcCurrent'" : string.Empty;
        }

        protected string GetUrl(IDataItemContainer cont)
        {
            var mi = cont.DataItem as MenuItem;
            return mi.GetUrl();
        }

        private string GetBaseRelativePath()
        {
            return BaseUrl ?? GetRequestRelativePath();
        }

        private string GetCurrentRelativePath()
        {
            return CurrentUrl ?? GetRequestRelativePath();
        }

        private string GetRequestRelativePath()
        {
            return Request.AppRelativeCurrentExecutionFilePath.ToLower()
                .Replace("~/desktopmodules/hotcakes/core/admin/", string.Empty);
        }
    }
}