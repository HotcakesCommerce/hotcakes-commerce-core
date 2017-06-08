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
using System.Text;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Orders
{
	public delegate void TransactionEventDelegate();

    partial class OrderPayments : BaseOrderPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Payments";
            CurrentTab = AdminTabType.Orders;
            ValidateCurrentUserHasPermission(SystemPermissions.OrdersEdit);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

			ucPaymentInformation.CurrentOrder = CurrentOrder;
			ucOrderStatusDisplay.CurrentOrder = CurrentOrder;

			ucReceivePayments.Visible = !CurrentOrder.IsRecurring;
			ucRecurringPayments.Visible = CurrentOrder.IsRecurring;

			ucReceivePayments.CurrentOrder = CurrentOrder;
			ucRecurringPayments.CurrentOrder = CurrentOrder;

            ucReceivePayments.TransactionEvent += TransactionHappened;
            ucRecurringPayments.TransactionEvent += TransactionHappened;
        }

        protected void TransactionHappened()
        {
            LoadTransactions();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

			lblOrderNumber.Text = CurrentOrder.OrderNumber;

            LoadTransactions();
            LoadRma();
        }

        private void LoadRma()
        {
            if (!string.IsNullOrWhiteSpace(RmaId))
            {
                var rma = CurrentOrder.Returns.FirstOrDefault(y => y.Bvin == RmaId);
                if (rma != null)
                {
                    ucMessageBox.ShowInformation("Need to Issue Credit of " + rma.TotalGrandRefundAmount.ToString("C") +
                                                 " for RMA Number " + rma.Number);
                }
            }
        }

        private void LoadTransactions()
        {
            var transactions = HccApp.OrderServices.Transactions.FindForOrder(OrderId);
			transactions = transactions.OrderByDescending(y => y.TimeStampUtc).ToList();
            if (transactions == null || transactions.Count < 1)
            {
                litTransactions.Text = "No Transactions Found";
                return;
            }

            var showFullCardNumbers = HccApp.CurrentStore.Settings.DisplayFullCreditCardNumbers;

            var tz = HccApp.CurrentStore.Settings.TimeZone;
            var sb = new StringBuilder();
            foreach (var t in transactions)
            {
                RenderTransaction(t, sb, tz, showFullCardNumbers);
            }
            litTransactions.Text = sb.ToString();
        }


        private void RenderTransaction(OrderTransaction t, StringBuilder sb, TimeZoneInfo timezone, bool showCardNumbers)
        {
            sb.Append("<div class=\"controlarea1");
            if (t.Voided)
            {
                sb.Append(" transactionvoided");
            }
            else
            {
                if (t.Success)
                {
                    sb.Append(" transactionsuccess");
                }
                else
                {
                    sb.Append(" transactionfailed");
                }
            }
            sb.Append("\"><div style=\"overflow:auto;width:100%;\">");

            if (t.Voided)
            {
                sb.Append("VOIDED<br />");
            }
            sb.Append(t.Amount.ToString("c") + " - ");

			var paymentMethod = PaymentMethods.Find(t.MethodId);
			var methodName = paymentMethod != null ? paymentMethod.MethodName : string.Empty;
			var methodInfo = LocalizationUtils.GetActionType(t.Action, methodName);
			sb.Append(methodInfo + "<br />");
            sb.Append(TimeZoneInfo.ConvertTimeFromUtc(t.TimeStampUtc, timezone) + "<br />");
            if (t.Success)
            {
                sb.Append("OK<br />");
            }
            else
            {
                sb.Append("FAILED<br />");
            }
            if (t.Action == ActionType.PurchaseOrderInfo || t.Action == ActionType.PurchaseOrderAccepted)
            {
                sb.Append("PO # " + t.PurchaseOrderNumber + "<br />");
            }
            if (t.Action == ActionType.CheckReceived || t.Action == ActionType.CheckReturned)
            {
                sb.Append("Check # " + t.CheckNumber + "<br />");
            }
			if (ActionTypeUtils.IsCreditCardTransaction(t.Action))
            {
				if (t.CreditCard.IsCardNumberValid())
				{
					if (showCardNumbers)
					{
						sb.Append(t.CreditCard.CardTypeName + " " + t.CreditCard.CardNumber + "<br />");
					}
					else
					{
						sb.Append(t.CreditCard.CardTypeName + " xxxx-xxxx-xxxx-" + t.CreditCard.CardNumberLast4Digits +
								  "<br />");
					}
					sb.Append("exp: " + t.CreditCard.ExpirationMonth + "/" + t.CreditCard.ExpirationYear + "<br />");
				}
				else
				{
					sb.Append("Unknown or Invalid Card Number Entered" + "<br/>");
				}

			}
			if (ActionTypeUtils.IsGiftCardTransaction(t.Action))
			{
				sb.Append("Gift Card " + t.GiftCard.CardNumber + "<br />");
			}
            if (!string.IsNullOrEmpty(t.RefNum1))
            {
                sb.Append("Ref#: " + t.RefNum1 + "<br />");
            }
            if (!string.IsNullOrEmpty(t.RefNum2))
            {
                sb.Append("Ref2#: " + t.RefNum2 + "<br />");
            }
            if (!string.IsNullOrEmpty(t.RMABvin))
            {
                sb.Append("RMA: " + t.RMABvin + "<br />");
            }
            sb.Append(t.Messages);
            sb.Append("</div></div>");
        }
    }
}