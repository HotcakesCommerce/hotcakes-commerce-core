#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>Calculates the payment totals, credits, authorizations, refunds, and balance due for an order.</summary>
    /// <remarks>Summary values are derived from successful, non-voided transaction classifications and pending RMA refunds. Recurring orders calculate the grand total from elapsed billing intervals rather than using only the original order total.</remarks>
    public class OrderPaymentSummary
    {
        /// <summary>Gets the amount represented by the applicable transaction classification.</summary>
        public decimal AmountAuthorized { get; private set; }
        /// <summary>Gets the amount represented by the applicable transaction classification.</summary>
        public decimal AmountCharged { get; private set; }
        /// <summary>Gets the amount represented by the applicable transaction classification.</summary>
        public decimal AmountRefunded { get; private set; }
        /// <summary>Gets the amount represented by the applicable transaction classification.</summary>
        public decimal AmountDue { get; private set; }
        /// <summary>Gets the amount represented by the applicable transaction classification.</summary>
        public decimal AmountDueWithAuth { get; private set; }
        /// <summary>Gets the amount represented by the applicable transaction classification.</summary>
        public decimal AmountReturned { get; private set; }
        /// <summary>Gets or sets the applicable order or payment value.</summary>
        public string PaymentsSummary { get; private set; }
        /// <summary>Gets or sets the applicable order or payment value.</summary>
        public decimal TotalCredit { get; private set; }

        /// <summary>Clears the applicable order transaction data.</summary>
        public void Clear()
        {
            AmountAuthorized = 0m;
            AmountCharged = 0m;
            AmountRefunded = 0m;
            AmountDue = 0m;
            AmountDueWithAuth = 0m;
            PaymentsSummary = string.Empty;
            TotalCredit = 0m;
            AmountReturned = 0m;
        }

        /// <summary>Populates this object from the supplied source data.</summary>
        /// <param name="o">The order whose transactions and returns are summarized.</param>
        /// <param name="svc">The order service used to load transactions and payment-method display data.</param>
        public void Populate(Order o, OrderService svc)
        {
            Clear();

            var transactions = svc.Transactions.FindForOrder(o.bvin);
            foreach (var t in transactions)
            {
                TotalCredit += t.AmountAppliedToOrder;
                switch (t.Action)
                {
                    case ActionType.GiftCardCapture:
                    case ActionType.GiftCardDecrease:
                    case ActionType.CashReceived:
                    case ActionType.CheckReceived:
                    case ActionType.CreditCardCapture:
                    case ActionType.CreditCardCharge:
                    case ActionType.PayPalCapture:
                    case ActionType.PayPalCharge:
                    case ActionType.PurchaseOrderAccepted:
                    case ActionType.CompanyAccountAccepted:
                    case ActionType.RewardPointsCapture:
                    case ActionType.RewardPointsDecrease:
                    case ActionType.ThirdPartyPayMethodCapture:
                    case ActionType.ThirdPartyPayMethodCharge:
                    case ActionType.RecurringPayment:
                        AmountCharged += t.AmountAppliedToOrder;
                        break;
                    case ActionType.CreditCardHold:
                    case ActionType.GiftCardHold:
                    case ActionType.PayPalHold:
                    case ActionType.RewardPointsHold:
                    case ActionType.ThirdPartyPayMethodHold:
                        AmountAuthorized += t.AmountHeldForOrder;
                        break;
                    case ActionType.CashReturned:
                    case ActionType.CheckReturned:
                    case ActionType.CreditCardRefund:
                    case ActionType.GiftCardIncrease:
                    case ActionType.PayPalRefund:
                    case ActionType.RewardPointsIncrease:
                    case ActionType.ThirdPartyPayMethodRefund:
                        AmountRefunded += -1*t.AmountAppliedToOrder;
                        break;
                    case ActionType.GiftCardUnHold:
                        AmountAuthorized += -1*t.AmountHeldForOrder;
                        break;
                }
            }

            if (o.Returns != null)
            {
                foreach (var r in o.Returns)
                {
                    if (r.Status == RMAStatus.NeedRefund)
                    {
                        AmountReturned += r.TotalGrandRefundAmount;
                    }
                }
            }

            PaymentsSummary = svc.OrdersListPaymentMethods(transactions);

            decimal totalGrand = 0;
            if (!o.IsRecurring)
            {
                totalGrand = o.TotalGrand;
            }
            else
            {
                var timeOfOrder = o.TimeOfOrderUtc;
                var now = DateTime.UtcNow;

                foreach (var lineItem in o.Items)
                {
                    var paymentsCount = lineItem.GetRecurTimes(timeOfOrder, now);
                    totalGrand += lineItem.LineTotal*paymentsCount;
                }
            }

            if (o.StatusCode != OrderStatusCode.Cancelled || AmountRefunded < AmountCharged)
            {
                AmountDue = totalGrand - TotalCredit - AmountReturned;
                AmountDueWithAuth = AmountDue - AmountAuthorized;
            }

            if (o.StatusCode == OrderStatusCode.Cancelled && AmountRefunded > 0 && AmountRefunded >= AmountCharged)
            {
                AmountDue = 0;
                AmountDueWithAuth = 0;
            }
        }
    }
}
