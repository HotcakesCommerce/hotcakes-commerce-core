#region License

// Distributed under the MIT License
// ============================================================
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

using System.Collections.Generic;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{

    partial class MailingLists : BaseAdminPage
    {

        protected override void OnPreInit(System.EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Mailing Lists";
            this.CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(Hotcakes.Commerce.Membership.SystemPermissions.PeopleView);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadMailingLists();
            }
        }

        private void LoadMailingLists()
        {
            List<Hotcakes.Commerce.Contacts.MailingListSnapShot> m;
            m = HccApp.ContactServices.MailingLists.FindAll();
            this.GridView1.DataSource = m;
            this.GridView1.DataBind();
            if (m.Count == 1)
            {
                this.lblResults.Text = "1 list found";
            }
            else
            {
                this.lblResults.Text = m.Count + " lists found";
            }
        }

        protected void GridView1_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            long id = (long)GridView1.DataKeys[e.RowIndex].Value;
            HccApp.ContactServices.MailingLists.Delete(id);

            LoadMailingLists();
        }

        protected void btnNew_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("MailingLists_edit.aspx");
        }

        protected void GridView1_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            long id = (long)GridView1.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("MailingLists_edit.aspx?id=" + id);
        }
    }
}