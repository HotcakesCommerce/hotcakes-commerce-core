#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class GiftCardConfiguration : BaseAdminPage
    {
        private string EditorGiftCardGatewayId
        {
            get { return ViewState["EditorGiftCardGatewayId"] as string; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ViewState.Remove("EditorGiftCardGatewayId");
                else
                    ViewState["EditorGiftCardGatewayId"] = value;
            }
        }

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            Title = Localization.GetString("GiftCardSettings");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnSave.Click += btnSave_Click;
            btnCloseDialog.Click += btnCloseDialog_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadGCGatewayEditor();

            if (!IsPostBack)
            {
                LocalizeView();

                LoadForm();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
            {
                ucMessageBox.ShowOk(Localization.GetString("SaveMessage"));
            }
        }

        protected void btnOptions_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                EditorGiftCardGatewayId = lstGateway.SelectedValue;
                LoadGCGatewayEditor();
                ShowDialog();
            }
        }

        protected void btnCloseDialog_Click(object sender, EventArgs e)
        {
            CloseGCGatewayEditor();
        }

        protected void gatewayEditor_EditingComplete(object sender, EventArgs e)
        {
            CloseGCGatewayEditor();
        }

        #endregion

        #region Implementation

        private bool SaveSettings()
        {
            if (!ValidCardNumber(txtCardNumberFormat.Text))
            {
                ucMessageBox.ShowError(Localization.GetString("CardFormatError"));
                return false;
            }

            var settings = HccApp.CurrentStore.Settings;
            var cardSett = HccApp.CurrentStore.Settings.GiftCard;

            cardSett.GiftCardsEnabled = cbEnableGiftCards.Checked;

            settings.PaymentGiftCardAuthorizeOnly = lstCaptureMode.SelectedIndex == 0;
            settings.GiftCardGateway = lstGateway.SelectedValue;

            cardSett.ExpirationPeriodMonths = Convert.ToInt32(txtExpiration.Text.Trim());
            cardSett.CardNumberFormat = txtCardNumberFormat.Text.Trim();
            cardSett.UseAZSymbols = cbUseSymbols.Checked;

            cardSett.MinAmount = Convert.ToDecimal(txtMinAmount.Text.Trim());
            cardSett.MaxAmount = Convert.ToDecimal(txtMaxAmount.Text.Trim());

            var amounts = txtAmounts.Text.Trim();

            if (!ProcessAmounts(ref amounts, cardSett.MinAmount, cardSett.MaxAmount))
            {
                ucMessageBox.ShowError(Localization.GetString("AmountCompareError"));
                return false;
            }

            cardSett.PredefinedAmounts = amounts;
            txtAmounts.Text = cardSett.PredefinedAmounts.Trim();
            return HccApp.UpdateCurrentStore();
        }

        private bool ValidCardNumber(string formatStr)
        {
            if (string.IsNullOrEmpty(formatStr) || formatStr.Length < 10 || formatStr.Length > 50)
                return false;

            return formatStr.Count(c => c == 'X' || c == 'x') >= 10;
        }

        private bool ProcessAmounts(ref string amounts, decimal min, decimal max)
        {
            var isValid = true;
            var list = amounts.Split(',');

            for (var i = 0; i < list.Length; i++)
            {
                var n = list[i];
                decimal dec = 0;
                if (decimal.TryParse(n, out dec))
                {
                    list[i] = dec.ToString();

                    if (dec > max || dec < min)
                    {
                        isValid = false;
                    }
                }
                else
                {
                    list[i] = null;
                }
            }

            amounts = string.Join(",", list);
            return isValid;
        }

        private void LoadForm()
        {
            PopulateGatewayList();

            var settings = HccApp.CurrentStore.Settings;
            var cardSett = HccApp.CurrentStore.Settings.GiftCard;

            cbEnableGiftCards.Checked = cardSett.GiftCardsEnabled;
            lstCaptureMode.SelectedIndex = settings.PaymentGiftCardAuthorizeOnly ? 0 : 1;

            lstGateway.SelectedValue = settings.GiftCardGateway;
            txtExpiration.Text = cardSett.ExpirationPeriodMonths.ToString();
            txtCardNumberFormat.Text = cardSett.CardNumberFormat;
            cbUseSymbols.Checked = cardSett.UseAZSymbols;
            txtAmounts.Text = cardSett.PredefinedAmounts;
            txtMinAmount.Text = cardSett.MinAmount.ToString();
            txtMaxAmount.Text = cardSett.MaxAmount.ToString();
        }

        private void PopulateGatewayList()
        {
            lstGateway.DataSource = GiftCardGateways.FindAll();
            lstGateway.DataValueField = "Id";
            lstGateway.DataTextField = "Name";
            lstGateway.DataBind();
        }

        private void ShowDialog()
        {
            if (!string.IsNullOrEmpty(EditorGiftCardGatewayId))
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcEditGiftCardGatewayDialog",
                    "hcEditGiftCardGatewayDialog();", true);
            }
        }

        private void LoadGCGatewayEditor()
        {
            if (!string.IsNullOrEmpty(EditorGiftCardGatewayId))
            {
                gatewayEditor.LoadEditor(EditorGiftCardGatewayId);
            }
        }

        private void CloseGCGatewayEditor()
        {
            EditorGiftCardGatewayId = null;
            gatewayEditor.RemoveEditor();
        }

        private void LocalizeView()
        {
            if (lstCaptureMode.Items.Count == 0)
            {
                lstCaptureMode.Items.Add(new ListItem(Localization.GetString("AuthorizeOnly")));
                lstCaptureMode.Items.Add(new ListItem(Localization.GetString("ChargeFullAmount.Text")));

                lstCaptureMode.Items[1].Selected = true;
            }

            rfvExpiration.ErrorMessage = Localization.GetString("rfvExpiration.ErrorMessage");
            cvExpiration.ErrorMessage = Localization.GetString("cvExpiration.ErrorMessage");
            rfvMinAmount.ErrorMessage = Localization.GetString("rfvMinAmount.ErrorMessage");
            cvMinAmount.ErrorMessage = Localization.GetString("cvMinAmount.ErrorMessage");
            rfvMaxAmount.ErrorMessage = Localization.GetString("rfvMaxAmount.ErrorMessage");
            cvMaxAmount.ErrorMessage = Localization.GetString("cvMaxAmount.ErrorMessage");
            rfvGateway.ErrorMessage = Localization.GetString("rfvGateway.ErrorMessage");
        }

        #endregion
    }
}