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

    partial class MailingLists_EditMember : BaseAdminPage
    {

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                this.EmailAddressField.Focus();

                if (Request.QueryString["id"] != null)
                {
                    this.BvinField.Value = Request.QueryString["id"];
                    LoadMember();
                }
                else
                {
                    this.BvinField.Value = string.Empty;
                    if (Request.QueryString["ListID"] != null)
                    {
                        this.ListIDField.Value = Request.QueryString["ListID"];
                    }
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
        private long CurrentListId
        {
            get
            {
                long temp = 0;
                long.TryParse(this.ListIDField.Value, out temp);
                return temp;
            }
            set
            {
                this.ListIDField.Value = value.ToString();
            }

        }
        protected override void OnPreInit(System.EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Edit Mailing List Member";
            this.CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(Hotcakes.Commerce.Membership.SystemPermissions.PeopleView);
        }

        private void LoadMember()
        {
            MailingListMember m = HccApp.ContactServices.MailingLists.FindMemberOnlyById(CurrentId);
            
            if (m != null)
            {
                if (m.Id > 0)
                {
                    this.EmailAddressField.Text = m.EmailAddress;
                    this.FirstNameField.Text = m.FirstName;
                    this.LastNameField.Text = m.LastName;
                    this.CurrentListId = m.ListId;                                    
                }
            }

        }

        protected void btnCancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("MailingLists_Edit.aspx?id=" + CurrentListId.ToString());
        }

        protected void btnSaveChanges_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.Save() == true)
            {
                Response.Redirect("MailingLists_Edit.aspx?id=" + CurrentListId.ToString());
            }
        }

        private bool Save()
        {
            bool result = false;

            MailingListMember m = HccApp.ContactServices.MailingLists.FindMemberOnlyById(CurrentId);

            if (m == null) m = new MailingListMember();
            
            
                string originalEmail = m.EmailAddress;

                m.EmailAddress = this.EmailAddressField.Text.Trim();
                m.FirstName = this.FirstNameField.Text.Trim();
                m.LastName = this.LastNameField.Text.Trim();
                m.ListId = CurrentListId;

                if (m.Id < 1)
                {
                    if (HccApp.ContactServices.MailingLists.CheckMembership(m.ListId, m.EmailAddress))
                    {
                        this.lblError.Text = "That email address already belongs to this mailing list. Select another email address";
                    }
                    else
                    {
                        result = HccApp.ContactServices.MailingLists.CreateMemberOnly(m);
                    }
                }
                else
                {
                    if (m.EmailAddress != originalEmail)
                    {
                        if (HccApp.ContactServices.MailingLists.CheckMembership(m.ListId, m.EmailAddress))
                        {
                            this.lblError.Text = "That email address already belongs to this mailing list. Select another email address";
                        }
                        else
                        {
                            result = HccApp.ContactServices.MailingLists.UpdateMemberOnly(m);
                        }
                    }
                    else
                    {
                        result = HccApp.ContactServices.MailingLists.UpdateMemberOnly(m);
                    }
                }

                if (result)
                {
                    // Update bvin field so that next save will call updated instead of create
                    this.CurrentId = m.Id;
                }
            
            return result;
        }

    }
}