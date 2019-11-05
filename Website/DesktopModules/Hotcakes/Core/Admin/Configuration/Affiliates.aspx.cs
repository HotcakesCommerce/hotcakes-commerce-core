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
using System.Threading;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Affiliates : BaseAdminPage
    {
        #region Validator Events

        protected void ValidateCommission(object source, ServerValidateEventArgs args)
        {
            decimal amount = 0;
            var minAmountFlatRate =
                decimal.Parse("0" + Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator + "01");
            var minAmountPercentage =
                decimal.Parse("0" + Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator + "01");
            decimal maxAmountPercentage = 100;
            if (decimal.TryParse(txtCommissionAmount.Text, NumberStyles.Number, Thread.CurrentThread.CurrentUICulture,
                out amount))
            {
                if (lstCommissionType.SelectedValue == "2")
                {
                    if (amount < minAmountFlatRate)
                    {
                        args.IsValid = false;
                    }
                }
                else if (lstCommissionType.SelectedValue == "1")
                {
                    if (amount < minAmountPercentage || amount > maxAmountPercentage)
                    {
                        args.IsValid = false;
                    }
                }
            }
            else
            {
                args.IsValid = false;
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("AffiliateSettings");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadSettings();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (SaveSettings())
                {
                    ucMessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
                }
            }
        }

        #endregion

        #region Implementation

        private void LoadSettings()
        {
            var sett = HccApp.CurrentStore.Settings;

            txtCommissionAmount.Text = sett.AffiliateCommissionAmount.ToString();
            txtReferralDays.Text = sett.AffiliateReferralDays.ToString();

            lstCommissionType.SelectedValue = ((int) sett.AffiliateCommissionType).ToString();
            lstConflictMode.SelectedValue = ((int) sett.AffiliateConflictMode).ToString();

            chkRequireApproval.Checked = sett.AffiliateRequireApproval;
            chkDisplayChildren.Checked = sett.AffiliateDisplayChildren;
            chkShowIDOnCheckout.Checked = sett.AffiliateShowIDOnCheckout;
            txtAgreementText.Text = sett.AffiliateAgreementText;
            chkAffiliateNotify.Checked = sett.AffiliateReview;
        }

        private bool SaveSettings()
        {
            var sett = HccApp.CurrentStore.Settings;
            sett.AffiliateCommissionAmount = Convert.ToDecimal(txtCommissionAmount.Text);
            sett.AffiliateReferralDays = Convert.ToInt32(txtReferralDays.Text);
            var typeSelection = Convert.ToInt32(lstCommissionType.SelectedValue);
            sett.AffiliateCommissionType = (AffiliateCommissionType) typeSelection;
            var conflictSelection = Convert.ToInt32(lstConflictMode.SelectedValue);
            sett.AffiliateConflictMode = (AffiliateConflictMode) conflictSelection;

            sett.AffiliateRequireApproval = chkRequireApproval.Checked;
            sett.AffiliateDisplayChildren = chkDisplayChildren.Checked;
            sett.AffiliateShowIDOnCheckout = chkShowIDOnCheckout.Checked;
            sett.AffiliateAgreementText = txtAgreementText.Text;
            sett.AffiliateReview = chkAffiliateNotify.Checked;

            return HccApp.UpdateCurrentStore();
        }

        #endregion
    }
}