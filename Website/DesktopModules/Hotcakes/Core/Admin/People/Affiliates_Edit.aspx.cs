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
using System.Globalization;
using System.Web.Security;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Urls;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;
using ErrorTypes = Hotcakes.Modules.Core.Admin.AppCode.ErrorTypes;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Affiliates_Edit : BaseAdminPage
    {
        #region Properties

        protected long? AffiliateId
        {
            get { return Request.QueryString["id"].ConvertToNullable<long>(); }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            PageTitle = AffiliateId.HasValue
                ? Localization.GetString("EditAffiliate")
                : Localization.GetString("NewAffiliate");
            CurrentTab = AdminTabType.Reports;
            ValidateCurrentUserHasPermission(SystemPermissions.ReportsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitAddressEditor();
            PageMessageBox = ucMessageBox;

            if (!IsPostBack)
            {
                if (AffiliateId.HasValue)
                {
                    LoadAffiliate();
                }
                else
                {
                    LoadDefaultValues();
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            valAffiliateID.ValidationExpression = AffiliateRepository.AffiliateIDValidationExpression;
            valReferralID.ValidationExpression = AffiliateRepository.AffiliateIDValidationExpression;
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (IsValid && Save())
            {
                Response.Redirect("Affiliates.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Affiliates.aspx");
        }

        #endregion

        #region Implementation

        private void InitAddressEditor()
        {
            ucAddress.RequireAddress = false;
            ucAddress.RequireCity = false;
            ucAddress.RequireCompany = false;
            ucAddress.RequireFirstName = true;
            ucAddress.RequireLastName = true;
            ucAddress.RequirePhone = false;
            ucAddress.RequirePostalCode = false;
            ucAddress.RequireRegion = false;
            ucAddress.ShowCompanyName = true;
            ucAddress.ShowPhoneNumber = true;
            ucAddress.ShowCounty = true;
            ucAddress.ShowAddressLine2 = false;
        }

        private void LoadDefaultValues()
        {
            lstCommissionType.ClearSelection();

            switch (HccApp.CurrentStore.Settings.AffiliateCommissionType)
            {
                case AffiliateCommissionType.PercentageCommission:
                case AffiliateCommissionType.None:
                    lstCommissionType.SelectedValue = "1";
                    break;
                case AffiliateCommissionType.FlatRateCommission:
                    lstCommissionType.SelectedValue = "2";
                    break;
                default:
                    lstCommissionType.SelectedValue = "1";
                    break;
            }

            CommissionAmountField.Text = HccApp.CurrentStore.Settings.AffiliateCommissionAmount.ToString("N");
            txtReferralDays.Text = HccApp.CurrentStore.Settings.AffiliateReferralDays.ToString();

            txtUsername.Focus();
        }

        private void LoadAffiliate()
        {
            var aff = HccApp.ContactServices.Affiliates.Find(AffiliateId.Value);

            if (aff != null)
            {
                var dnnUserDeleted = aff.UserId < 0;
                chkEnabled.Checked = aff.Enabled;
                chkApproved.Checked = aff.Approved;
                txtUsername.Text = aff.Username;
                txtEmail.Text = aff.Email;
                txtAffiliateID.Text = aff.AffiliateId;
                txtReferralId.Text = aff.ReferralAffiliateId;
                lstCommissionType.ClearSelection();

                switch (aff.CommissionType)
                {
                    case AffiliateCommissionType.PercentageCommission:
                        lstCommissionType.SelectedValue = "1";
                        break;
                    case AffiliateCommissionType.FlatRateCommission:
                    case AffiliateCommissionType.None:
                        lstCommissionType.SelectedValue = "2";
                        break;
                    default:
                        lstCommissionType.SelectedValue = "1";
                        break;
                }

                CommissionAmountField.Text = aff.CommissionAmount.ToString("N");
                txtReferralDays.Text = aff.ReferralDays.ToString();
                TaxIdField.Text = aff.TaxId;
                DriversLicenseField.Text = aff.DriversLicenseNumber;
                WebsiteUrlField.Text = aff.WebSiteUrl;
                NotesTextBox.Text = aff.Notes;

                if (!string.IsNullOrEmpty(aff.AffiliateId))
                {
                    SampleUrlLabel.Text = HccApp.CurrentStore.RootUrl() + "?" + WebAppSettings.AffiliateQueryStringName +
                                          "=" + aff.AffiliateId;
                }

                ucAddress.LoadFromAddress(aff.Address);

                txtUsername.Enabled = false;
                divPassword.Visible = false;
                divPassword2.Visible = false;
                lnkDnnUserProfile.Visible = !dnnUserDeleted;
                lnkDnnUserProfile.NavigateUrl = HccUrlBuilder.RouteHccUrl(HccRoute.EditUserProfile,
                    new {userId = aff.UserId});

                if (aff.Approved)
                {
                    chkApproved.Enabled = false;
                }

                if (dnnUserDeleted)
                {
                    btnSaveChanges.Visible = false;
                }
            }
        }

        private bool Save()
        {
            Affiliate aff = null;
            var prevApprovementStatus = false;

            if (AffiliateId.HasValue)
            {
                aff = HccApp.ContactServices.Affiliates.Find(AffiliateId.Value);
                prevApprovementStatus = aff.Approved;
            }
            else
            {
                aff = new Affiliate
                {
                    Username = txtUsername.Text.Trim(),
                    Password = txtPassword.Text
                };
            }

            aff.Email = txtEmail.Text;
            aff.Enabled = chkEnabled.Checked;

            if (chkApproved.Checked) aff.Approved = true;

            aff.AffiliateId = txtAffiliateID.Text.Trim();
            aff.ReferralAffiliateId = txtReferralId.Text.Trim();

            var typeSelection = int.Parse(lstCommissionType.SelectedValue);

            aff.CommissionType = (AffiliateCommissionType) typeSelection;
            aff.CommissionAmount = decimal.Parse(CommissionAmountField.Text, NumberStyles.Currency);
            aff.ReferralDays = int.Parse(txtReferralDays.Text);
            aff.TaxId = TaxIdField.Text.Trim();
            aff.DriversLicenseNumber = DriversLicenseField.Text.Trim();
            aff.WebSiteUrl = WebsiteUrlField.Text.Trim();
            aff.Address = ucAddress.GetAsAddress();
            aff.Notes = NotesTextBox.Text;

            var userStatus = CreateUserStatus.None;
            var status = AffiliateRepository.UpdateStatus.UnknownError;

            if (AffiliateId.HasValue)
            {
                status = HccApp.ContactServices.Affiliates.Update(aff);
            }
            else
            {
                status = HccApp.ContactServices.Affiliates.Create(aff, ref userStatus);
            }

            switch (status)
            {
                case AffiliateRepository.UpdateStatus.Success:
                    if (!prevApprovementStatus && aff.Approved)
                    {
                        var ui = DnnUserController.Instance.GetUser(DnnGlobal.Instance.GetPortalId(), aff.UserId);
                        var culture = ui.Profile["UsedCulture"] as string;

                        if (string.IsNullOrEmpty(culture))
                        {
                            culture = "en-US";
                        }

                        HccApp.ContactServices.AffiliateWasApproved(aff, culture);
                    }
                    if (!string.IsNullOrEmpty(txtReferralId.Text))
                    {
                        var refaff = HccApp.ContactServices.Affiliates.FindByAffiliateId(txtReferralId.Text.Trim());
                        if (refaff == null)
                        {
                            ShowMessage(Localization.GetString("UnknownReferralAffiliateID"), ErrorTypes.Warning);
                            return false;
                        }
                    }
                    break;
                case AffiliateRepository.UpdateStatus.DuplicateAffiliateID:
                    ShowMessage(Localization.GetString("DuplicateAffiliateID"), ErrorTypes.Error);
                    break;
                case AffiliateRepository.UpdateStatus.UserCreateFailed:
                    HandleUserCreateFailedStatus(userStatus);
                    break;
                default:
                    ShowMessage(string.Format(Localization.GetString("AffiliateCreateError"), userStatus),
                        ErrorTypes.Error);
                    break;
            }

            return status == AffiliateRepository.UpdateStatus.Success;
        }

        private void HandleUserCreateFailedStatus(CreateUserStatus userStatus)
        {
            switch (userStatus)
            {
                case CreateUserStatus.InvalidPassword:
                    ShowMessage(
                        string.Format(Localization.GetString("InvalidPassword"),
                            Membership.Provider.MinRequiredPasswordLength), ErrorTypes.Error);
                    break;
                case CreateUserStatus.DuplicateUsername:
                    ShowMessage(Localization.GetString("DuplicateUsername"), ErrorTypes.Error);
                    break;
                case CreateUserStatus.DuplicateEmail:
                    ShowMessage(Localization.GetString("DuplicateEmail"), ErrorTypes.Error);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}