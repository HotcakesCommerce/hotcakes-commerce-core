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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class CustomUrl : BaseAdminPage
    {
        private int currentPage = 1;

        private readonly int pageSize = 50;
        private int rowCount;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Custom Urls";
            CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                GridView1.PageSize = 10;
                LoadUrls();
            }
        }

        private void LoadUrls()
        {
            if (Request.QueryString["page"] != null)
            {
                int.TryParse(Request.QueryString["page"], out currentPage);
                if (currentPage < 1)
                {
                    currentPage = 1;
                }
            }

            var urls = HccApp.ContentServices.CustomUrls.FindAllPaged(currentPage, pageSize, ref rowCount);
            lblResults.Text = rowCount + " Urls Found";
            GridView1.DataSource = urls;
            GridView1.DataBind();

            litPager1.Text = Paging.RenderPagerWithLimits("CustomUrl.aspx?page={0}", currentPage, rowCount, pageSize, 20);
            litPager2.Text = litPager1.Text;
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            msg.ClearMessage();
            var bvin = (string) GridView1.DataKeys[e.RowIndex].Value;
            if (HccApp.ContentServices.CustomUrls.Delete(bvin) == false)
            {
                msg.ShowWarning("Unable to delete this custom Url.");
            }

            LoadUrls();
            e.Cancel = true;
        }

        protected void btnNew_Click(object sender, ImageClickEventArgs e)
        {
            msg.ClearMessage();
            Response.Redirect("CustomUrl_Edit.aspx");
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var bvin = (string) GridView1.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("CustomUrl_Edit.aspx?id=" + bvin);
        }
    }
}