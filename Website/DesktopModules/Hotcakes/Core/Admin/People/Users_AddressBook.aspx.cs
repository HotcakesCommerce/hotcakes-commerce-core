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
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    public partial class Users_AddressBook : BaseCustomerPage
    {
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = Localization.GetString("AddressBook");
            InitNavMenu(ucNavMenu);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadAddresses();
            }
        }

        protected void btnNewAddress_Click(object sender, EventArgs e)
        {
            Response.Redirect("users_edit_address.aspx?userID=" + CustomerId);
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
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmDelete"));
        }

        protected void gvAddress_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            CustomerAccount u;

            u = HccApp.MembershipServices.Customers.Find(CustomerId);

            if (u != null)
            {
                var bvin = (string) gvAddress.DataKeys[e.RowIndex].Value;

                u.DeleteAddress(bvin);

                CreateUserStatus status;

                HccApp.MembershipServices.UpdateCustomer(u, out status);

                LoadAddresses();
            }
        }

        protected void gvAddress_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            var bvin = (string) gvAddress.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("Users_Edit_Address.aspx?userID=" + CustomerId + "&id=" + bvin);
        }

        protected void gvAddress_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("NickName");
                e.Row.Cells[1].Text = Localization.GetString("Name");
            }
        }

        #endregion

        #region Implementation

        private void LoadAddresses()
        {
            var u = HccApp.MembershipServices.Customers.Find(CustomerId);

            if (u != null)
            {
                gvAddress.DataSource = u.Addresses;
                gvAddress.DataBind();
            }
        }

        protected string GetDisplayName(object FirstName, object LastName)
        {
            if (FirstName != null || LastName != null)
            {
                var displayName = string.Empty;
                displayName = FirstName == null ? string.Empty : FirstName.ToString();

                if (!string.IsNullOrEmpty(displayName))
                {
                    displayName += " ";
                }

                displayName += LastName == null ? string.Empty : LastName.ToString();

                if (string.IsNullOrEmpty(displayName))
                {
                    return Localization.GetString("NoDisplayName");
                }

                return displayName;
            }

            return Localization.GetString("NoDisplayName");
        }

        protected string GetNickName(object NickName)
        {
            if (NickName != null)
            {
                if (!string.IsNullOrEmpty(NickName.ToString()))
                {
                    return NickName.ToString();
                }
                return Localization.GetString("NoDisplayName");
            }

            return Localization.GetString("NoDisplayName");
        }

        #endregion
    }
}