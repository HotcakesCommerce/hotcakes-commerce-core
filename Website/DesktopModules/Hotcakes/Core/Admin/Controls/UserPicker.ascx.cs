#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Controls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;
using System.Linq;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class UserPicker : HccUserControl
    {
        public delegate void UserSelectedDelegate(object sender, UserSelectedEventArgs e);

        protected IMessageBox _messageBox;

        private int _tabIndex = -1;

        private int _usernameFieldSize = 15;

        public int UserNameFieldSize
        {
            get { return _usernameFieldSize; }
            set { _usernameFieldSize = value; }
        }

        public string UserName
        {
            get { return UserNameField.Text; }
            set { UserNameField.Text = value; }
        }

        public IMessageBox MessageBox
        {
            get { return _messageBox; }
            set { _messageBox = value; }
        }

        public int TabIndex
        {
            get { return _tabIndex; }
            set { _tabIndex = value; }
        }

        public event UserSelectedDelegate UserSelected;

        protected override void OnLoad(EventArgs e)
        {
            UserNameField.Columns = _usernameFieldSize;
            base.OnLoad(e);
            if (_tabIndex != -1)
            {
                FilterField.TabIndex = (short) _tabIndex;
                btnGoUserSearch.TabIndex = (short) (_tabIndex + 1);
                btnBrowserUserCancel.TabIndex = (short) (_tabIndex + 2);

                NewUserEmailField.TabIndex = (short) (_tabIndex + 3);
                NewUserFirstNameField.TabIndex = (short) (_tabIndex + 4);
                NewUserLastNameField.TabIndex = (short) (_tabIndex + 5);
                NewUserTaxExemptField.TabIndex = (short) (_tabIndex + 6);

                btnNewUserCancel.TabIndex = (short) (_tabIndex + 7);
                btnNewUserSave.TabIndex = (short) (_tabIndex + 8);
            }
        }

        protected void btnBrowseUsers_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            pnlNewUser.Visible = false;
            if (pnlUserBrowser.Visible)
            {
                pnlUserBrowser.Visible = false;
            }
            else
            {
                pnlUserBrowser.Visible = true;
                LoadUsers();
            }
        }

        protected void btnBrowserUserCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            pnlUserBrowser.Visible = false;
        }

        protected void btnNewUserCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            pnlNewUser.Visible = false;
        }

        protected void btnNewUser_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            pnlUserBrowser.Visible = false;
            if (pnlNewUser.Visible)
            {
                pnlNewUser.Visible = false;
            }
            else
            {
                pnlNewUser.Visible = true;
            }
        }

        protected void btnGoUserSearch_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            LoadUsers();
            FilterField.Focus();
        }

        private void LoadUsers()
        {
            List<CustomerAccount> users;
            var count = 0;
            users = HccApp.MembershipServices.Customers.FindByFilter(FilterField.Text.Trim(), 0, 50, ref count);
            GridView1.DataSource = users;
            GridView1.DataBind();
        }

        protected void btnNewUserSave_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            var u = new CustomerAccount();
            u.Email = NewUserEmailField.Text.Trim();
            u.FirstName = NewUserFirstNameField.Text.Trim();
            u.LastName = NewUserLastNameField.Text.Trim();
            u.Password = PasswordGenerator.GeneratePassword(12);
            u.TaxExempt = NewUserTaxExemptField.Checked;
            var createResult = new CreateUserStatus();
            if (HccApp.MembershipServices.CreateCustomer(u, out createResult, u.Password))
            {
                UserNameField.Text = u.Email;
                ValidateUser();
                pnlNewUser.Visible = false;
            }
            else
            {
                switch (createResult)
                {
                    case CreateUserStatus.DuplicateUsername:
                        if (MessageBox != null)
                            MessageBox.ShowWarning(string.Format(Localization.GetString("UsernameInvalid"), NewUserEmailField.Text.Trim()));
                        break;
                    case CreateUserStatus.InvalidPassword:
                        if (MessageBox != null)
                        {
                            MessageBox.ShowWarning(Localization.GetString("InvalidPassword"));
                        }

                        break;
                    default:
                        if (MessageBox != null)
                        {
                            MessageBox.ShowWarning(Localization.GetString("AccountCreateError"));
                        }
                        break;
                }
            }
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();
            var bvin = (string) GridView1.DataKeys[e.NewEditIndex].Value;
            var u = HccApp.MembershipServices.Customers.Find(bvin);
            if (u != null)
            {
                UserNameField.Text = u.Email;
            }
            ValidateUser();
            pnlUserBrowser.Visible = false;
        }

        protected void btnValidateUser_Click(object sender, EventArgs e)
        {
            if (MessageBox != null) MessageBox.ClearMessage();

            ValidateUser();
        }

        private void ValidateUser()
        {
            var users = HccApp.MembershipServices.Customers.FindByEmail(UserNameField.Text.Trim());
            var u = users != null ? users.FirstOrDefault() : null;
            if (u != null)
            {
                if (u.Bvin != string.Empty)
                {
                    var args = new UserSelectedEventArgs();
                    args.UserAccount = u;
                    if (UserSelected != null)
                    {
                        UserSelected(this, args);
                    }
                }
                else
                {
                    if (MessageBox != null)
                    {
                        MessageBox.ShowWarning(string.Format(Localization.GetString("UserNotFound"), UserNameField.Text.Trim()));
                    }
                }
            }
        }
    }
}