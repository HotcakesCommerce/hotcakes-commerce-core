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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class StoreInfo : BaseAdminPage
    {
        #region Fields

        private AddressService _addrService;

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("StoreAddress");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _addrService = new AddressService(HccApp.CurrentStore);

            ucAddressEditor.RequirePostalCode = true;
            ucAddressEditor.RequireAddress = true;
            ucAddressEditor.RequireCity = true;
            ucAddressEditor.RequireRegion = true;

            ucAddressEditor.RequireFirstName = false;
            ucAddressEditor.RequireLastName = false;
            ucAddressEditor.RequirePhone = false;
            ucAddressEditor.RequireCompany = false;

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ucMessage.ClearMessage();

            if (Save())
            {
                ucMessage.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
            else
            {
                ucMessage.ShowError(Localization.GetString("SettingsFailed"));
            }
        }

        #endregion

        #region Implementation

        private void LoadData()
        {
            ucAddressEditor.LoadFromAddress(HccApp.ContactServices.Addresses.FindStoreContactAddress());
        }

        private bool Save()
        {
            var addr = ucAddressEditor.GetAsAddress();
            addr.AddressType = AddressTypes.StoreContact;
            var result = false;

            if (string.IsNullOrEmpty(addr.Bvin))
            {
                result = HccApp.ContactServices.Addresses.Create(addr);
            }
            else
            {
                result = HccApp.ContactServices.Addresses.Update(addr);
            }

            return result;
        }

        #endregion
    }
}