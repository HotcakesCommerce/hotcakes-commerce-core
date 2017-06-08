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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class ReceivePayments : HccUserControl
    {
        public TransactionEventDelegate TransactionEvent;

        public Order CurrentOrder { get; set; }

        public string RmaId
        {
            get { return Request.QueryString["rmaid"]; }
        }

        private OrderPaymentManager PayManager { get; set; }

        private List<OrderTransaction> currentOrderTransactions { get; set; }
        private OrderPaymentSummary paySummary { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PayManager = new OrderPaymentManager(CurrentOrder, HccApp);
            if (CurrentOrder != null)
            {
                currentOrderTransactions = HccApp.OrderServices.Transactions.FindForOrder(CurrentOrder.bvin);
                paySummary = HccApp.OrderServices.PaymentSummary(CurrentOrder);
            }

            if (!Page.IsPostBack)
            {
                mvPayments.SetActiveView(viewCreditCards);
                LoadCreditCardLists();
            }
        }

        #region Tab buttons

        protected void lnkCC_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewCreditCards);
            LoadCreditCardLists();
        }

        protected void lnkGiftCard_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewGiftCards);
            LoadGiftCardLists();
        }

        protected void lnkPO_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewPO);
            PopulatePOList();
        }

        protected void lnkCompanyAccount_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewCompanyAccount);
            PopulateCompanyAccountList();
        }

        protected void lnkCash_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewCash);
        }

        protected void lnkCheck_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewCheck);
        }

        protected void lnkPayPal_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewPayPal);
            LoadPayPalLists();
        }

        protected void lnkPoints_Click(object sender, EventArgs e)
        {
            mvPayments.SetActiveView(viewPoints);
            LoadPointsLists();
        }

        #endregion

        #region Cash

        protected void btnCashRefund_Click(object sender, EventArgs e)
        {
            var amount = ParseMoney(CashAmount.Text);

            if (!RefundAmountValidate(amount))
            {
                ucMessageBox.ShowWarning("Refund amount should be less then order charged amount.");
                return;
            }

            ShowTransaction(PayManager.CashRefund(amount, RmaId));
        }

        protected void btnCashReceive_Click(object sender, EventArgs e)
        {
            var amount = ParseMoney(CashAmount.Text);
            ShowTransaction(PayManager.CashReceive(amount));
        }

        #endregion

        #region Check

        protected void lnkCheckReturn_Click(object sender, EventArgs e)
        {
            var amount = ParseMoney(CheckAmountField.Text);
            if (!RefundAmountValidate(amount))
            {
                ucMessageBox.ShowWarning("Refund amount should be less then order charged amount.");
                return;
            }
            var checkNumber = CheckNumberField.Text.Trim();
            ShowTransaction(PayManager.CheckReturn(amount, checkNumber, RmaId));
        }

        protected void lnkCheckReceive_Click(object sender, EventArgs e)
        {
            var amount = ParseMoney(CheckAmountField.Text);
            var checkNumber = CheckNumberField.Text.Trim();
            ShowTransaction(PayManager.CheckReceive(amount, checkNumber));
        }

        #endregion

        #region Purchase Order

        protected void lnkPOAdd_Click(object sender, EventArgs e)
        {
            var amount = ParseMoney(PONewAmount.Text);
            var poNumber = PONewNumber.Text.Trim();
            ShowTransaction(PayManager.PurchaseOrderAddInfo(poNumber, amount));
            PopulatePOList();
        }

        protected void lnkPOAccept_Click(object sender, EventArgs e)
        {
            var poNumber = lstPO.SelectedItem.Value;
            ShowTransaction(PayManager.PurchaseOrderAccept(poNumber));
            PopulatePOList();
        }

        private void PopulatePOList()
        {
            var pos = PayManager.PurchaseOrderInfoListAllNonAccepted();
            lstPO.Items.Clear();
            if (pos.Count < 1) lstPO.Items.Add(new ListItem("No Purchase Orders Found.", string.Empty));

            foreach (var t in pos)
            {
                lstPO.Items.Add(new ListItem(t.PurchaseOrderNumber + " - " + t.Amount.ToString("c"),
                    t.PurchaseOrderNumber));
            }
        }

        #endregion

        #region Company Account

        protected void lnkCompanyAccountAdd_Click(object sender, EventArgs e)
        {
            var amount = ParseMoney(CompanyAccountNewAmount.Text);
            var accountNumber = CompanyAccountNewNumber.Text.Trim();
            ShowTransaction(PayManager.CompanyAccountAddInfo(accountNumber, amount));
            PopulateCompanyAccountList();
        }

        protected void lnkCompanyAccountAccept_Click(object sender, EventArgs e)
        {
            var accountNumber = lstCompanyAccount.SelectedItem.Value;
            ShowTransaction(PayManager.CompanyAccountAccept(accountNumber));
            PopulateCompanyAccountList();
        }

        private void PopulateCompanyAccountList()
        {
            var acts = PayManager.CompanyAccountInfoListAllNonAccepted();
            lstCompanyAccount.Items.Clear();
            if (acts.Count < 1) lstCompanyAccount.Items.Add(new ListItem("No Company Accounts Found.", string.Empty));
            foreach (var t in acts)
            {
                lstCompanyAccount.Items.Add(new ListItem(t.CompanyAccountNumber + " - " + t.Amount.ToString("c"),
                    t.CompanyAccountNumber));
            }
        }

        #endregion

        #region Credit Cards

        private void LoadCreditCardLists()
        {
            // List Auths for Collection
            var auths = PayManager.CreditCardHoldListAll();
            lstCreditCardAuths.Items.Clear();
            if (auths.Count < 1)
            {
                lstCreditCardAuths.Items.Add(new ListItem("No Pending Holds", string.Empty));
                lnkCreditCardCaptureAuth.Enabled = false;
                lnkCreditCardVoidAuth.Enabled = false;
            }
            else
            {
                foreach (var t in auths)
                {
                    lstCreditCardAuths.Items.Add(
                        new ListItem(
                            t.CreditCard.CardTypeName + "-" + t.CreditCard.CardNumberLast4Digits + " - " +
                            t.Amount.ToString("c"), t.IdAsString));
                }
                lnkCreditCardCaptureAuth.Enabled = true;
                lnkCreditCardVoidAuth.Enabled = true;
            }


            // List charges for refunds
            var charges = PayManager.CreditCardChargeListAllRefundable();
            lstCreditCardCharges.Items.Clear();
            if (charges.Count < 1)
            {
                lstCreditCardCharges.Items.Add(new ListItem("No Charges to Refund", string.Empty));
                lnkCreditCardRefund.Enabled = false;
            }
            else
            {
                foreach (var t in charges)
                {
                    lstCreditCardCharges.Items.Add(
                        new ListItem(
                            t.CreditCard.CardTypeName + "-" + t.CreditCard.CardNumberLast4Digits + " - " +
                            t.Amount.ToString("c"), t.IdAsString));
                }
                lnkCreditCardRefund.Enabled = true;
            }


            // Load Cards for Charges and Auths
            var cards = PayManager.CreditCardInfoListAll();
            lstCreditCards.Items.Clear();
            if (cards.Count < 1)
            {
                lstCreditCards.Items.Add(new ListItem("No Saved Cards", string.Empty));
                lnkCreditCardCharge.Enabled = false;
            }
            else
            {
                foreach (var t in cards)
                {
                    lstCreditCards.Items.Add(new ListItem(t.CreditCard.CardTypeName + "-"
                                                          + t.CreditCard.CardNumberLast4Digits + " "
                                                          + t.CreditCard.ExpirationMonth + "/"
                                                          + t.CreditCard.ExpirationYearTwoDigits, t.IdAsString));
                }
                lnkCreditCardRefund.Enabled = true;
            }

            CheckIsRefunded();
        }

        protected void lnkCreditCardAddInfo_Click(object sender, EventArgs e)
        {
            var card = ucCreditCardInput.GetCardData();
            ShowTransaction(PayManager.CreditCardAddInfo(card, 0));
            LoadCreditCardLists();
        }

        protected void lnkCreditCardVoidAuth_Click(object sender, EventArgs e)
        {
            var transactionId = lstCreditCardAuths.SelectedItem.Value;
            var amount = ParseMoney(CreditCardAuthAmount.Text);
            if (string.IsNullOrWhiteSpace(CreditCardAuthAmount.Text))
            {
                var authTrans = PayManager.FindTransactionById(transactionId);
                amount = authTrans.Amount;
            }
            var securityCode = CreditCardAuthSecurityCode.Text.Trim();
            ShowTransaction(PayManager.CreditCardVoid(transactionId, amount, securityCode));
            LoadCreditCardLists();
        }

        protected void lnkCreditCardCaptureAuth_Click(object sender, EventArgs e)
        {
            var transactionId = lstCreditCardAuths.SelectedItem.Value;
            var amount = ParseMoney(CreditCardAuthAmount.Text);
            if (string.IsNullOrWhiteSpace(CreditCardAuthAmount.Text))
            {
                var authTrans = PayManager.FindTransactionById(transactionId);
                amount = authTrans.Amount;
            }
            var securityCode = CreditCardAuthSecurityCode.Text.Trim();
            ShowTransaction(PayManager.CreditCardCapture(transactionId, amount, securityCode));
            LoadCreditCardLists();
        }

        protected void lnkCreditCardRefund_Click(object sender, EventArgs e)
        {
            var transactionId = lstCreditCardCharges.SelectedItem.Value;
            var amount = ParseMoney(CreditCardRefundAmount.Text);
            if (!RefundAmountValidate(amount))
            {
                ucMessageBox.ShowWarning("Refund amount should be less then order charged amount.");
                return;
            }
            if (string.IsNullOrWhiteSpace(CreditCardRefundAmount.Text))
            {
                var refTrans = PayManager.FindTransactionById(transactionId);
                amount = refTrans.Amount;
            }
            var securityCode = CreditCardChargeSecurityCode.Text.Trim();
            ShowTransaction(PayManager.CreditCardRefund(transactionId, amount, securityCode, RmaId));
            LoadCreditCardLists();
        }

        protected void lnkCreditCardNewAuth_Click(object sender, EventArgs e)
        {
            var cardId = lstCreditCards.SelectedItem.Value;
            var amount = ParseMoney(CreditCardChargeAmount.Text);
            var securityCode = CreditCardChargeSecurityCode.Text.Trim();
            ShowTransaction(PayManager.CreditCardHold(cardId, amount, securityCode));
            LoadCreditCardLists();
        }

        protected void lnkCreditCardCharge_Click(object sender, EventArgs e)
        {
            var cardId = lstCreditCards.SelectedItem.Value;
            var amount = ParseMoney(CreditCardChargeAmount.Text);
            var securityCode = CreditCardChargeSecurityCode.Text.Trim();
            ShowTransaction(PayManager.CreditCardCharge(cardId, amount, securityCode));
            LoadCreditCardLists();
        }

        #endregion

        #region PayPal

        private void LoadPayPalLists()
        {
            // List Auths for Collection
            var paypalAuths = PayManager.PayPalExpressHoldListAll();
            lstPayPalHold.Items.Clear();
            if (paypalAuths.Count < 1)
            {
                lstPayPalHold.Items.Add(new ListItem("No Pending Holds.", string.Empty));
                lnkPayPalCaptureHold.Enabled = false;
                lnkPayPalVoidHold.Enabled = false;
            }
            else
            {
                foreach (var t in paypalAuths)
                {
                    lstPayPalHold.Items.Add(new ListItem(t.Amount.ToString("c"), t.IdAsString));
                }
                lnkPayPalCaptureHold.Enabled = true;
                lnkPayPalVoidHold.Enabled = true;
            }

            // List charges for refunds
            var charges = PayManager.PayPalExpressListAllRefundable();
            lstPayPalRefund.Items.Clear();
            if (charges.Count < 1)
            {
                lstPayPalRefund.Items.Add(new ListItem("No Charges to Refund", string.Empty));
                lnkPayPalRefund.Enabled = false;
            }
            else
            {
                foreach (var t in charges)
                {
                    lstPayPalRefund.Items.Add(new ListItem(t.Amount.ToString("c"), t.IdAsString));
                }
                lnkPayPalRefund.Enabled = true;
            }

            CheckIsRefunded();
        }

        protected void lnkPayPalVoidHold_Click(object sender, EventArgs e)
        {
            var transactionId = lstPayPalHold.SelectedItem.Value;
            var amount = ParseMoney(PayPalHoldAmount.Text);
            ShowTransaction(PayManager.PayPalExpressVoid(transactionId, amount));
            LoadPayPalLists();
        }

        protected void lnkPayPalCaptureHold_Click(object sender, EventArgs e)
        {
            var transactionId = lstPayPalHold.SelectedItem.Value;
            var amount = ParseMoney(PayPalHoldAmount.Text);
            if (string.IsNullOrWhiteSpace(PayPalHoldAmount.Text))
            {
                var authTrans = PayManager.FindTransactionById(transactionId);
                amount = authTrans.Amount;
            }
            ShowTransaction(PayManager.PayPalExpressCapture(transactionId, amount));
            LoadPayPalLists();
        }

        protected void lnkPayPalRefund_Click(object sender, EventArgs e)
        {
            var transactionId = lstPayPalRefund.SelectedItem.Value;
            var amount = ParseMoney(PayPalRefundAmount.Text);
            if (!RefundAmountValidate(amount))
            {
                ucMessageBox.ShowWarning("Refund amount should be less then order charged amount.");
                return;
            }
            if (string.IsNullOrWhiteSpace(PayPalRefundAmount.Text))
            {
                var refTrans = PayManager.FindTransactionById(transactionId);
                amount = refTrans.Amount;
            }
            ShowTransaction(PayManager.PayPalExpressRefund(transactionId, amount, RmaId));
            LoadPayPalLists();
        }

        #endregion

        #region Reward Points

        private void LoadPointsLists()
        {
            // List Auths for Collection
            var auths = PayManager.RewardsPointsHoldListAll();
            lstPointsHeld.Items.Clear();
            if (auths.Count < 1)
            {
                lstPointsHeld.Items.Add(new ListItem("No Pending Holds", string.Empty));
                lnkPointsCaptureAuth.Enabled = false;
                lnkPointsVoidAuth.Enabled = false;
            }
            else
            {
                foreach (var t in auths)
                {
                    lstPointsHeld.Items.Add(new ListItem(t.Amount.ToString("c"), t.IdAsString));
                }
                lnkPointsCaptureAuth.Enabled = true;
                lnkPointsVoidAuth.Enabled = true;
            }

            // List charges for refunds
            var charges = PayManager.RewardsPointsListAllRefundable();
            lstPointsRefundable.Items.Clear();
            if (charges.Count < 1)
            {
                lstPointsRefundable.Items.Add(new ListItem("No Charges to Refund", string.Empty));
                lnkPointsRefund.Enabled = false;
            }
            else
            {
                foreach (var t in charges)
                {
                    lstPointsRefundable.Items.Add(new ListItem(t.Amount.ToString("c"), t.IdAsString));
                }
                lnkPointsRefund.Enabled = true;
            }

            lblPointsAvailable.Text = PayManager.RewardsPointsAvailableDescription();
            CheckIsRefunded();
        }

        protected void lnkPointsVoidAuth_Click(object sender, EventArgs e)
        {
            var transactionId = lstPointsHeld.SelectedItem.Value;
            var points = ParsePoints(PointsHeldAmount.Text);
            if (string.IsNullOrWhiteSpace(PointsHeldAmount.Text))
            {
                var authTrans = PayManager.FindTransactionById(transactionId);
                points = HccApp.CustomerPointsManager.PointsNeededForPurchaseAmount(authTrans.Amount);
            }
            ShowTransaction(PayManager.RewardsPointsUnHold(transactionId, points));
            LoadPointsLists();
        }

        protected void lnkPointsCaptureAuth_Click(object sender, EventArgs e)
        {
            var transactionId = lstPointsHeld.SelectedItem.Value;
            var points = ParsePoints(PointsHeldAmount.Text);
            if (string.IsNullOrWhiteSpace(PointsHeldAmount.Text))
            {
                var authTrans = PayManager.FindTransactionById(transactionId);
                points = HccApp.CustomerPointsManager.PointsNeededForPurchaseAmount(authTrans.Amount);
            }
            ShowTransaction(PayManager.RewardsPointsCapture(transactionId, points));
            LoadPointsLists();
        }

        protected void lnkPointsNewAuth_Click(object sender, EventArgs e)
        {
            var points = ParsePoints(PointsNewAmountField.Text);
            ShowTransaction(PayManager.RewardsPointsHold(string.Empty, points));
            LoadPointsLists();
        }

        protected void lnkPointsNewCharge_Click(object sender, EventArgs e)
        {
            var points = ParsePoints(PointsNewAmountField.Text);
            ShowTransaction(PayManager.RewardsPointsCharge(string.Empty, points));
            LoadPointsLists();
        }

        protected void lnkPointsRefund_Click(object sender, EventArgs e)
        {
            var transactionId = lstPointsRefundable.SelectedItem.Value;
            var points = ParsePoints(PointsRefundAmount.Text);

            if (string.IsNullOrWhiteSpace(PointsRefundAmount.Text))
            {
                var refTrans = PayManager.FindTransactionById(transactionId);
                if (points > HccApp.CustomerPointsManager.PointsNeededForPurchaseAmount(refTrans.Amount))
                {
                    ucMessageBox.ShowWarning("Refund points value should be less then order charged amount.");
                    return;
                }

                points = HccApp.CustomerPointsManager.PointsNeededForPurchaseAmount(refTrans.Amount);
            }
            ShowTransaction(PayManager.RewardsPointsRefund(transactionId, points, RmaId));
            LoadPointsLists();
        }

        #endregion

        #region Gift Cards

        protected void lnkGiftCardCharge_Click(object sender, EventArgs e)
        {
            var card = GiftCardNew.Text.Trim();
            var amount = ParseMoney(AmountGiftCardNew.Text);
            ShowTransaction(PayManager.GiftCardDecreaseWithCard(card, amount));
            LoadGiftCardLists();
        }

        protected void lnkGiftCardRefund_Click(object sender, EventArgs e)
        {
            var transactionId = lstGiftCardCharges.SelectedItem.Value;
            var amount = ParseMoney(AmountGiftCardRefund.Text);
            if (AmountGiftCardRefund.Text.Trim() == string.Empty)
            {
                var refTrans = PayManager.FindTransactionById(transactionId);
                amount = refTrans.Amount;
            }
            ShowTransaction(PayManager.GiftCardIncrease(transactionId, amount, RmaId));
            LoadGiftCardLists();
        }

        protected void lnkGiftCardBalanceCheck_Click(object sender, EventArgs e)
        {
            var card = GiftCardBalanceCheckCard.Text.Trim();
            var response = PayManager.GiftCardBalanceInquiry(card);
            if (response.Success)
            {
                ucMessageBox.ShowOk("The card balance is " + response.CurrentValue.ToString("C") + " as of " +
                                    DateTime.Now);
            }
            else
            {
                var message = "Unable to check balance.";
                foreach (var m in response.Messages)
                {
                    if (m.Severity == MessageType.Warning ||
                        m.Severity == MessageType.Error)
                    {
                        message += "<br />" + m.Description;
                    }
                }
                ucMessageBox.ShowWarning(message);
            }
        }

        protected void lnkGiftCardHold_Click(object sender, EventArgs e)
        {
            var transId = lstGiftCardCharges.SelectedItem.Value;
            var amount = ParseMoney(AmountGiftCardNew.Text);
            ShowTransaction(PayManager.GiftCardHold(transId, amount));
            LoadGiftCardLists();
        }

        protected void lnkGiftCardCapturHold_Click(object sender, EventArgs e)
        {
            var transactionId = lstGiftCardHold.SelectedItem.Value;
            var amount = ParseMoney(AmountGiftCardHold.Text);
            if (string.IsNullOrWhiteSpace(AmountGiftCardHold.Text))
            {
                var authTrans = PayManager.FindTransactionById(transactionId);
                amount = authTrans.Amount;
            }
            ShowTransaction(PayManager.GiftCardCapture(transactionId, amount));
            LoadGiftCardLists();
        }

        protected void lnkGiftCardVoidHold_Click(object sender, EventArgs e)
        {
            var transactionId = lstGiftCardHold.SelectedItem.Value;
            var amount = ParseMoney(AmountGiftCardHold.Text);
            ShowTransaction(PayManager.GiftCardUnHold(transactionId, amount));
            LoadGiftCardLists();
        }

        private void LoadGiftCardLists()
        {
            // List Auths for Collection
            var giftCardAuth = PayManager.GiftCardHoldListAll();
            lstGiftCardHold.Items.Clear();
            if (giftCardAuth.Count < 1)
            {
                lstGiftCardHold.Items.Add(new ListItem("No Pending Holds.", string.Empty));
                lnkGiftCardCapturHold.Enabled = false;
                lnkGiftCardVoidHold.Enabled = false;
            }
            else
            {
                foreach (var t in giftCardAuth)
                {
                    lstGiftCardHold.Items.Add(new ListItem(t.Amount.ToString("c"), t.IdAsString));
                }
                lnkGiftCardCapturHold.Enabled = true;
                lnkGiftCardVoidHold.Enabled = true;
            }

            // List charges for refunds
            var charges = PayManager.GiftCardChargeListAllRefundable();
            lstGiftCardCharges.Items.Clear();
            if (charges.Count < 1)
            {
                lstGiftCardCharges.Items.Add(new ListItem("No Charges to Refund", string.Empty));
                lnkGiftCardRefund.Enabled = false;
            }
            else
            {
                foreach (var t in charges)
                {
                    lstGiftCardCharges.Items.Add(new ListItem(t.GiftCard.CardNumber + " - " + t.Amount.ToString("c"),
                        t.IdAsString));
                }
                lnkGiftCardRefund.Enabled = true;
            }

            var giftCardProcessor = HccApp.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
            lnkGiftCardHold.Visible = giftCardProcessor.CanAuthorize;

            CheckIsRefunded();
        }

        #endregion

        #region Private

        private decimal ParseMoney(string input)
        {
            decimal val = 0;
            decimal.TryParse(input, NumberStyles.Currency, Thread.CurrentThread.CurrentUICulture, out val);
            return Money.RoundCurrency(val);
        }

        private int ParsePoints(string input)
        {
            var points = 0;
            int.TryParse(input, out points);
            return points;
        }

        private void ShowTransaction(bool result)
        {
            if (result)
            {
                ucMessageBox.ShowInformation("&laquo; Transaction Processed at " + DateTime.Now);
            }
            else
            {
                ucMessageBox.ShowWarning("Could not record transaction. See Administrator!");
            }
            TransactionEvent();
        }

        private void CheckIsRefunded()
        {
            currentOrderTransactions = HccApp.OrderServices.Transactions.FindForOrder(CurrentOrder.bvin);
            if (currentOrderTransactions == null || currentOrderTransactions.Count < 1)
            {
                return;
            }
            var disableRefundlinks = false;
            decimal refundAmount = 0;
            foreach (var t in currentOrderTransactions)
            {
                if (t.Success)
                {
                    if (t.Action == ActionType.CashReturned || t.Action == ActionType.CreditCardRefund
                        || t.Action == ActionType.GiftCardIncrease || t.Action == ActionType.PayPalRefund
                        || t.Action == ActionType.RewardPointsIncrease ||
                        t.Action == ActionType.ThirdPartyPayMethodRefund
                        || t.Action == ActionType.CheckReturned)
                    {
                        refundAmount += t.AmountAppliedToOrder*-1;
                        disableRefundlinks = true;
                    }
                }
            }
            if (disableRefundlinks && refundAmount >= paySummary.AmountCharged)
            {
                lnkCreditCardRefund.Enabled = false;
                CreditCardRefundAmount.Text = string.Empty;
                CreditCardRefundSecurityCode.Text = string.Empty;

                lnkPayPalRefund.Enabled = false;
                PayPalRefundAmount.Text = string.Empty;

                lnkGiftCardRefund.Enabled = false;
                AmountGiftCardRefund.Text = string.Empty;

                lnkPointsRefund.Enabled = false;
                PointsRefundAmount.Text = string.Empty;

                btnCashRefund.Enabled = false;
                CashAmount.Text = string.Empty;

                lnkCheckReturn.Enabled = false;
                CheckAmountField.Text = string.Empty;
            }
        }

        private bool RefundAmountValidate(decimal amount)
        {
            currentOrderTransactions = currentOrderTransactions.OrderByDescending(y => y.TimeStampUtc).ToList();
            if (currentOrderTransactions == null || currentOrderTransactions.Count < 1)
            {
                return true;
            }
            decimal refundAmount = 0;
            foreach (var t in currentOrderTransactions)
            {
                if (t.Success)
                {
                    if (t.Action == ActionType.CashReturned || t.Action == ActionType.CreditCardRefund
                        || t.Action == ActionType.GiftCardIncrease || t.Action == ActionType.PayPalRefund
                        || t.Action == ActionType.RewardPointsIncrease ||
                        t.Action == ActionType.ThirdPartyPayMethodRefund
                        || t.Action == ActionType.CheckReturned)
                    {
                        refundAmount += t.AmountAppliedToOrder*-1;
                    }
                }
            }
            if (refundAmount >= paySummary.AmountCharged)
            {
                return false;
            }

            if (refundAmount + amount > paySummary.AmountCharged)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}