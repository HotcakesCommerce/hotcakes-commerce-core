#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

using System;
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Users_Edit_Address : BaseCustomerPage
    {
        #region Properties

        public string AddressId
        {
            get { return Request.QueryString["id"]; }
        }

        public override string CustomerId
        {
            get { return Request.QueryString["UserID"]; }
        }

        #endregion

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
            SetEditorMode();

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(AddressId))
                {
                    LoadAddressForUser(AddressId);
                }
                else
                {
                    var newAddr = new Address {CountryBvin = WebAppSettings.ApplicationCountryBvin};
                    ucAddressEditor.LoadFromAddress(newAddr);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users_AddressBook.aspx?id=" + CustomerId);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ucAddressEditor.Validate())
            {
                if (SaveCurrentAddress())
                {
                    Response.Redirect("Users_AddressBook.aspx?id=" + CustomerId);
                }
                else
                {
                    Message.ShowError(Localization.GetString("SaveFailure"));
                }
            }
        }

        #endregion

        #region Implementation

        private void SetEditorMode()
        {
            ucAddressEditor.RequireAddress = false;
            ucAddressEditor.RequireCity = false;
            ucAddressEditor.RequireCompany = false;
            ucAddressEditor.RequireFirstName = false;
            ucAddressEditor.RequireLastName = false;
            ucAddressEditor.RequirePhone = false;
            ucAddressEditor.RequirePostalCode = false;
            ucAddressEditor.RequireRegion = false;
            ucAddressEditor.ShowCompanyName = true;
            ucAddressEditor.ShowPhoneNumber = true;
            ucAddressEditor.ShowCounty = true;
        }

        private void LoadAddressForUser(string addressID)
        {
            if (Customer != null)
            {
                var addr = Customer.Addresses.FirstOrDefault(a => a.Bvin == addressID);

                if (addr != null)
                {
                    txtNickName.Text = addr.NickName;
                    ucAddressEditor.LoadFromAddress(addr);
                }
            }
        }

        private bool SaveCurrentAddress()
        {
            var result = false;

            if (Customer != null)
            {
                var temp = ucAddressEditor.GetAsAddress();

                temp.NickName = txtNickName.Text.Trim();

                if (string.IsNullOrEmpty(temp.Bvin))
                {
                    temp.Bvin = Guid.NewGuid().ToString();
                    HccApp.MembershipServices.CheckIfNewAddressAndAddWithUpdate(Customer, temp);
                }
                else
                {
                    Customer.UpdateAddress(temp);
                }

                CreateUserStatus status;
                result = HccApp.MembershipServices.UpdateCustomer(Customer, out status);
            }

            return result;
        }

        #endregion
    }
}