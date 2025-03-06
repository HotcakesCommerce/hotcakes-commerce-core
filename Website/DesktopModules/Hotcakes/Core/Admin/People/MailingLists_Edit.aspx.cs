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

using Hotcakes.Commerce.Contacts;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{

    partial class MailingLists_Edit : BaseAdminPage
    {

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                this.NameField.Focus();

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

        private void LoadList()
        {
            MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
                if (m != null)
                {
                    if (m.Id > 0)
                    {
                        this.NameField.Text = m.Name;
                        this.IsPrivateField.Checked = m.IsPrivate;
                        this.GridView1.DataSource = m.Members;
                        this.GridView1.DataBind();
                    }
                }
        }

        private void LoadMembers(MailingList m)
        {
            this.GridView1.DataSource = m.Members;
            this.GridView1.DataBind();
        }

        protected override void OnPreInit(System.EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Edit Mailing List";
            this.CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(Hotcakes.Commerce.Membership.SystemPermissions.PeopleView);
        }

        protected void btnCancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("MailingLists.aspx");
        }

        protected void btnSaveChanges_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                this.lblError.Text = string.Empty;

                if (Save() == true)
                {
                    Response.Redirect("MailingLists.aspx");
                }
            }
        }

        private bool Save()
        {
            bool result = false;

            MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);

            if (m == null) m = new MailingList();

                m.Name = this.NameField.Text.Trim();
                m.IsPrivate = this.IsPrivateField.Checked;

                if (m.Id < 1)
                {
                    result = HccApp.ContactServices.MailingLists.Create(m);
                }
                else
                {
                    result = HccApp.ContactServices.MailingLists.Update(m);
                }

                if (result == false)
                {
                    this.lblError.Text = "Unable to save mailing list. Uknown error.";
                }
                else
                {
                    // Update bvin field so that next save will call updated instead of create
                    CurrentId = m.Id;
                }

            return result;
        }

        protected void btnExport_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Hotcakes.Commerce.Contacts.MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
            this.ImportField.Text = m.ExportToCommaDelimited(this.chkOnlyEmail.Checked);
        }

        protected void btnNew_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.Save() == true)
            {
                Response.Redirect("MailingLists_EditMember.aspx?listid=" + this.BvinField.Value);
            }
        }

        protected void GridView1_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            long id = (long)GridView1.DataKeys[e.RowIndex].Value;
            MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
            if (m != null) m.RemoveMemberById(id);
            HccApp.ContactServices.MailingLists.Update(m);
            LoadMembers(m);
        }

        protected void GridView1_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            if (this.Save() == true)
            {
                long id = (long)GridView1.DataKeys[e.NewEditIndex].Value;
                Response.Redirect("MailingLists_EditMember.aspx?&id=" + id);
            }
        }

        protected void btnImport_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                if (this.Save() == true)
                {
                    MailingList m = HccApp.ContactServices.MailingLists.Find(CurrentId);
                    if (m != null)
                    {
                        m.ImportFromCommaDelimited(this.ImportField.Text);
                        HccApp.ContactServices.MailingLists.Update(m);
                        LoadMembers(m);
                    }
                }
            }
        }

        protected void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = true;
            System.IO.StringReader sw = new System.IO.StringReader(ImportField.Text);
            string splitCharacter = ",";
            string lineToProcess = string.Empty;
            lineToProcess = sw.ReadLine();

            while (lineToProcess != null)
            {
                string[] lineValues = lineToProcess.Split(splitCharacter.ToCharArray());
                if (lineValues.Length > 0)
                {
                    string EmailAddress = lineValues[0];
                    System.Text.RegularExpressions.Regex re
                        = new System.Text.RegularExpressions.Regex("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*",
                                                                    System.Text.RegularExpressions.RegexOptions.Compiled);
                    if (re.Match(EmailAddress).Value != EmailAddress)
                    {
                        args.IsValid = false;
                        return;
                    }
                }
                lineToProcess = sw.ReadLine();
            }
            sw.Dispose();
        }
    }
}