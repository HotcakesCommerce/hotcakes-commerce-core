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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class EmailTemplates : BaseAdminPage
    {
        private const string DELETE_CONFIRM_FORMAT = "return hcConfirm(event,'{0}');";

        #region Implementation

        private void LoadTemplates()
        {
            gvTemplates.DataSource = HccApp.ContentServices.GetAllTemplatesForStoreOrDefaults();
            gvTemplates.DataBind();
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("EmailTemplates");
            CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvTemplates.RowDataBound += gvTemplates_RowDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadTemplates();
            }
        }

        private void gvTemplates_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var templ = e.Row.DataItem as HtmlTemplate;
                var btnDelete = e.Row.FindControl("btnDelete");
                btnDelete.Visible = templ.TemplateType == HtmlTemplateType.Custom;
            }
        }

        protected void gvTemplates_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            ucMessageBox.ClearMessage();
            var templateId = (long) gvTemplates.DataKeys[e.RowIndex].Value;

            if (!HccApp.ContentServices.HtmlTemplates.Delete(templateId))
            {
                ucMessageBox.ShowWarning(Localization.GetString("CannotDelete"));
            }

            LoadTemplates();
        }

        protected void gvTemplates_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var templateId = (long) gvTemplates.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("EmailTemplates_Edit.aspx?id=" + templateId);
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmailTemplates_Edit.aspx?id=0");
        }

        protected void gvTemplates_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("TemplateName");
            }
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = string.Format(DELETE_CONFIRM_FORMAT, Localization.GetString("DeleteConfirm"));
        }

        #endregion
    }
}