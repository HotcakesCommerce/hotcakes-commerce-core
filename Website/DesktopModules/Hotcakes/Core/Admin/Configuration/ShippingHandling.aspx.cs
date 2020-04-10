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
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class ShippingHandling : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ShippingHandlingSettings");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LocalizeView();

                LoadHandlingSettings();
            }
        }

        private void LoadHandlingSettings()
        {
            HandlingFeeAmountTextBox.Text = HccApp.CurrentStore.Settings.HandlingAmount.ToString("c");
            HandlingRadioButtonList.SelectedIndex = HccApp.CurrentStore.Settings.HandlingType;
            NonShippingCheckBox.Checked = HccApp.CurrentStore.Settings.HandlingNonShipping;
        }

        protected void HandlingFeeAmountCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            decimal temp;

            if (decimal.TryParse(args.Value, NumberStyles.Currency,
                Thread.CurrentThread.CurrentUICulture, out temp))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var handlingFee = decimal.Parse(HandlingFeeAmountTextBox.Text, NumberStyles.Currency);
                HccApp.CurrentStore.Settings.HandlingAmount = Money.RoundCurrency(handlingFee);
                HccApp.CurrentStore.Settings.HandlingType = HandlingRadioButtonList.SelectedIndex;
                HccApp.CurrentStore.Settings.HandlingNonShipping = NonShippingCheckBox.Checked;
                HccApp.UpdateCurrentStore();

                MessageBox1.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
        }

        private void LocalizeView()
        {
            rfvHandlingFeeAmountTextBox.ErrorMessage = Localization.GetString("rfvHandlingFeeAmountTextBox.ErrorMessage");
            cvHandlingFeeAmountTextBox.ErrorMessage = Localization.GetString("cvHandlingFeeAmountTextBox.ErrorMessage");

            if (HandlingRadioButtonList.Items.Count == 0)
            {
                HandlingRadioButtonList.Items.Add(new ListItem(Localization.GetString("PerItem"), "0"));
                HandlingRadioButtonList.Items.Add(new ListItem(Localization.GetString("PerOrder"), "1"));
            }
        }
    }
}