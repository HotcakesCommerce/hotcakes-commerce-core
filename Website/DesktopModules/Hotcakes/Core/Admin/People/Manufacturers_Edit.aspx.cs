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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Manufacturers_Edit : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetEditorMode();

            if (!Page.IsPostBack)
            {
                DisplayNameField.Focus();

                if (Request.QueryString["id"] != null)
                {
                    BvinField.Value = Request.QueryString["id"];
                    InitializeForm();

                    if (BvinField.Value == "0")
                    {
                        BvinField.Value = string.Empty;
                        EmailTemplateDropDownList.SelectedValue =
                            HccApp.ContentServices.GetHtmlTemplateOrDefault(HtmlTemplateType.DropShippingNotice)
                                .Id.ToString();
                    }
                    else
                    {
                        LoadManufacturer();
                    }
                }
            }
        }

        private void InitializeForm()
        {
            EmailTemplateDropDownList.DataSource = HccApp.ContentServices.GetAllTemplatesForStoreOrDefaults();
            EmailTemplateDropDownList.DataTextField = "DisplayName";
            EmailTemplateDropDownList.DataValueField = "Id";
            EmailTemplateDropDownList.DataBind();
        }

        private void SetEditorMode()
        {
            AddressEditor1.RequireAddress = false;
            AddressEditor1.RequireCity = false;
            AddressEditor1.RequireCompany = false;
            AddressEditor1.RequireFirstName = false;
            AddressEditor1.RequireLastName = false;
            AddressEditor1.RequirePhone = false;
            AddressEditor1.RequirePostalCode = false;
            AddressEditor1.RequireRegion = false;
            AddressEditor1.ShowCompanyName = true;
            AddressEditor1.ShowPhoneNumber = true;
            AddressEditor1.ShowCounty = true;
        }

        private void LoadManufacturer()
        {
            var m = HccApp.ContactServices.Manufacturers.Find(BvinField.Value);

            if (m != null)
            {
                if (m.Bvin != string.Empty)
                {
                    DisplayNameField.Text = m.DisplayName;
                    EmailField.Text = m.EmailAddress;
                    AddressEditor1.LoadFromAddress(m.Address);
                    EmailTemplateDropDownList.SelectedValue = m.DropShipEmailTemplateId;
                }
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("EditManufacturer");
            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.PeopleView);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Manufacturers.aspx");
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                Response.Redirect("Manufacturers.aspx");
            }
        }

        private bool Save()
        {
            var result = false;

            var m = HccApp.ContactServices.Manufacturers.Find(BvinField.Value);

            if (m == null) m = new VendorManufacturer();

            m.DisplayName = DisplayNameField.Text.Trim();
            m.EmailAddress = EmailField.Text.Trim();
            m.Address = AddressEditor1.GetAsAddress();
            m.DropShipEmailTemplateId = EmailTemplateDropDownList.SelectedValue;

            if (BvinField.Value == string.Empty)
            {
                result = HccApp.ContactServices.Manufacturers.Create(m);
            }
            else
            {
                result = HccApp.ContactServices.Manufacturers.Update(m);
            }

            if (result == false)
            {
                MessageBox1.ShowError(Localization.GetString("SaveFailure"));
            }
            else
            {
                // Update bvin field so that next save will call updated instead of create
                BvinField.Value = m.Bvin;
            }

            return result;
        }
    }
}