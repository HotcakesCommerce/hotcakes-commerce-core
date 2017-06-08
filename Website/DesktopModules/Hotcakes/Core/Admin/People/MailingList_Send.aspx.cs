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