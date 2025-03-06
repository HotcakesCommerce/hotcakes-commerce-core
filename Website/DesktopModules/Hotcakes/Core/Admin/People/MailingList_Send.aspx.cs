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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{

    partial class MailingList_Send : BaseAdminPage
    {

        private long CurrentId
        {
            get
            {
                long temp = 0;
                long.TryParse(this.BvinField.Value, out temp);
                return temp;
            }
            set
            {
                this.BvinField.Value = value.ToString();
            }

        }

        protected override void OnPreInit(System.EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Send Email to Mailing List";
            this.CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(Hotcakes.Commerce.Membership.SystemPermissions.PeopleView);
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                PopulateTemplates();
                if (Request.QueryString["id"] != null)
                {
                    this.BvinField.Value = Request.QueryString["id"];
                    LoadList();
                }
                else
                {
                    this.BvinField.Value = string.Empty;
                }
            }
        }
        
        private void PopulateTemplates()
        {
            List<HtmlTemplate> templates = HccApp.ContentServices.GetAllTemplatesForStoreOrDefaults();
            this.EmailTemplateField.DataSource = templates;
            this.EmailTemplateField.DataTextField = "DisplayName";
            this.EmailTemplateField.DataValueField = "Id";
            this.EmailTemplateField.DataBind();
        }

        private void LoadList()
        {
            MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
            if (m != null)
            {
                if (m.Id > 0)
                {
                    this.lblList.Text = m.Name + " (" + m.Members.Count + " members)";
                }
            }
        }
        
        protected void btnCancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("MailingLists.aspx");
        }

        protected void btnPreview_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MessageBox1.ClearMessage();
            Preview();
            MessageBox1.ShowInformation("Preview Generated");
        }

        private void Preview()
        {
            MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
            if (m != null)
            {
                long templateId = 0;
                long.TryParse(this.EmailTemplateField.SelectedValue, out templateId);
                HtmlTemplate t = HccApp.ContentServices.HtmlTemplates.Find(templateId);
                if (t != null)
                {
                    System.Net.Mail.MailMessage p = m.PreviewMessage(t,HccApp);
                    if (p != null)
                    {
                        this.PreviewSubjectField.Text = p.Subject;
                        this.PreviewBodyField.Text = p.Body;
                    }
                }
            }
        }

        protected void btnSend_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MessageBox1.ClearMessage();
            Send();
            MessageBox1.ShowOk("Messages sent to list!");
        }

        private void Send()
        {
            Preview();

            long templateId = 0;
            long.TryParse(this.EmailTemplateField.SelectedValue, out templateId);
            HtmlTemplate t = HccApp.ContentServices.HtmlTemplates.Find(templateId);
            MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
            if (m != null)
            {
                if (t != null)
                {
                    m.SendToList(t, false, HccApp);
                }
            }
        }
    }
}