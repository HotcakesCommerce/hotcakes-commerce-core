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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Modules.PaymentMethods.CreditCard
{
    partial class Edit : HccPaymentMethodPart
    {
        public override bool HasOwnButtons
        {
            get { return true; }
        }

        private string EditorGatewayId
        {
            get { return ViewState["EditorGatewayId"] as string; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ViewState.Remove("EditorGatewayId");
                else
                    ViewState["EditorGatewayId"] = value;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveData();
                NotifyFinishedEditing();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!string.IsNullOrEmpty(EditorGatewayId))
            {
                gatewayEditor.LoadEditor(EditorGatewayId);
            }
        }

        private void PopulateGatewayList()
        {
            lstGateway.Items.Add(new ListItem(Localization.GetString("GatewayEmptyValue"), string.Empty));
            lstGateway.AppendDataBoundItems = true;
            lstGateway.DataSource = PaymentGateways.FindAll();
            lstGateway.DataValueField = "Id";
            lstGateway.DataTextField = "Name";
            lstGateway.DataBind();
        }

        public override void LoadData()
        {
            // Skip loading of data when we are returning from gateway editor
            // to credit card editor which settings wasn't saved yet
            if (mvEditors.ActiveViewIndex != 0)
                return;

            if (lstGateway.Items.Count == 0)
                PopulateGatewayList();

            var authorizeOnly = HccApp.CurrentStore.Settings.PaymentCreditCardAuthorizeOnly;
            lstCaptureMode.SelectedValue = authorizeOnly ? "1" : "0";

            chkRequireCreditCardSecurityCode.Checked = HccApp.CurrentStore.Settings.PaymentCreditCardRequireCVV;
            chkDisplayFullCardNumber.Checked = false;//HccApp.CurrentStore.Settings.DisplayFullCreditCardNumbers;

            lstGateway.SelectedValue = HccApp.CurrentStore.Settings.PaymentCreditCardGateway;

            var acceptedCards = HccApp.CurrentStore.Settings.PaymentAcceptedCards;
            foreach (var t in acceptedCards)
            {
                switch (t)
                {
                    case CardType.Amex:
                        chkCardAmex.Checked = true;
                        break;
                    case CardType.DinersClub:
                        chkCardDiners.Checked = true;
                        break;
                    case CardType.Discover:
                        chkCardDiscover.Checked = true;
                        break;
                    case CardType.JCB:
                        chkCardJCB.Checked = true;
                        break;
                    case CardType.MasterCard:
                        chkCardMasterCard.Checked = true;
                        break;
                    case CardType.Visa:
                        chkCardVisa.Checked = true;
                        break;
                }
            }
        }

        public override void SaveData()
        {
            var authorizeOnly = lstCaptureMode.SelectedValue == "1";
            HccApp.CurrentStore.Settings.PaymentCreditCardAuthorizeOnly = authorizeOnly;

            HccApp.CurrentStore.Settings.PaymentCreditCardRequireCVV = chkRequireCreditCardSecurityCode.Checked;
            HccApp.CurrentStore.Settings.PaymentCreditCardGateway = lstGateway.SelectedValue;
            HccApp.CurrentStore.Settings.DisplayFullCreditCardNumbers = false;

            // Save Credit Card Types
            var acceptedCards = new List<CardType>();
            if (chkCardAmex.Checked) acceptedCards.Add(CardType.Amex);
            if (chkCardVisa.Checked) acceptedCards.Add(CardType.Visa);
            if (chkCardMasterCard.Checked) acceptedCards.Add(CardType.MasterCard);
            if (chkCardDiscover.Checked) acceptedCards.Add(CardType.Discover);
            if (chkCardDiners.Checked) acceptedCards.Add(CardType.DinersClub);
            if (chkCardJCB.Checked) acceptedCards.Add(CardType.JCB);
            HccApp.CurrentStore.Settings.PaymentAcceptedCards = acceptedCards;
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }

        protected void btnOptions_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                EditorGatewayId = lstGateway.SelectedValue;

                mvEditors.SetActiveView(vGatewayEditor);
                gatewayEditor.LoadEditor(EditorGatewayId);
            }
        }

        protected void gatewayEditor_EditingComplete(object sender, HccPartEventArgs e)
        {
            EditorGatewayId = null;

            mvEditors.SetActiveView(vCreditCardEditor);
            gatewayEditor.RemoveEditor();
        }
    }
}