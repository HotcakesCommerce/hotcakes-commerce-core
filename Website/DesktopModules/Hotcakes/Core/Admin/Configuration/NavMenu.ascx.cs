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
using System.Web.UI.WebControls;
using Hotcakes.Modules.Core.Admin.AppCode;
using MenuItem = Hotcakes.Modules.Core.Admin.AppCode.MenuItem;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class NavMenu : HccUserControl
    {
        public string Path { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            rpMenuItems.ItemDataBound += rpMenuItems_ItemDataBound;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMenu();
            }
        }

        private void rpMenuItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var menuItem = e.Item.DataItem as MenuItem;

                var hyperLink = e.Item.FindControl("hyperLink") as HyperLink;

                hyperLink.Text = menuItem.Text;
                hyperLink.NavigateUrl = menuItem.GetUrl();
            }
        }

        private void BindMenu()
        {
            rpMenuItems.DataSource = MenuProvider.GetFilteredMenuItems(Path, HccApp);
            rpMenuItems.DataBind();
        }
    }
}