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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Reports.Daily_CC_Activity
{
    public partial class View : BaseReportPage
    {
        #region Fields

        protected int TotalCount;

        #endregion

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = Localization.GetString("CreditCardActivity");
            PageMessageBox = ucMessageBox;
        }

        #endregion

        #region Implementation

        private void ShowNoRecordsMessage(bool show)
        {
            pnlReport.Visible = !show;
            lblNoTransactionsMessage.Visible = show;
        }

        protected override void BindReport()
        {
            var utcStart = ucDateRangePicker.GetStartDateUtc(HccApp);
            var utcEnd = ucDateRangePicker.GetEndDateUtc(HccApp);

            var totalTransactions = 0;

            var transactions = HccApp.OrderServices.Transactions.FindForCreditCardReportByDateRange(utcStart, utcEnd,
                HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalTransactions);

            if (totalTransactions > 0)
            {
                var data = transactions.OrderBy(y => y.CreditCard.CardType).ThenBy(y => y.TimeStampUtc);

                RenderReport(data);
            }

            ShowNoRecordsMessage(TotalCount == 0);
        }

        private void RenderReport(IOrderedEnumerable<OrderTransaction> data)
        {
            decimal TotalGrand = 0;
            var cardtype = "?";
            decimal totalType = 0;

            foreach (var dataitem in data)
            {
                if (!ActionTypeUtils.IsCreditCardTransaction(dataitem.Action) &&
                    !ActionTypeUtils.IsGiftCardTransaction(dataitem.Action)) continue;

                var cardTypeName = dataitem.CreditCard.CardTypeName;

                if (ActionTypeUtils.IsGiftCardTransaction(dataitem.Action))
                {
                    cardTypeName = Localization.GetString("GiftCard");
                }

                if (cardTypeName != cardtype)
                {
                    if (cardtype != "?")
                    {
                        RenderPreviousTotal(totalType);
                    }
                    totalType = 0;
                    RenderHeader(cardTypeName);
                    cardtype = cardTypeName;
                }

                if (dataitem.AmountAppliedToOrder != 0)
                {
                    totalType += dataitem.AmountAppliedToOrder;
                    TotalGrand += dataitem.AmountAppliedToOrder;
                    TotalCount++;

                    var row = new TableRow {CssClass = "hcGridRow hcRight"};

                    row.Cells.AddRange(new[]
                    {
                        new TableCell
                        {
                            Text = DateHelper.ConvertUtcToStoreTime(HccApp, dataitem.TimeStampUtc).ToString()
                        },
                        new TableCell {Text = dataitem.OrderNumber},
                        new TableCell {Text = dataitem.Id.ToString()},
                        new TableCell {Text = "XXXX XXXX XXXX " + dataitem.CreditCard.CardNumberLast4Digits},
                        new TableCell {Text = dataitem.AmountAppliedToOrder.ToString("C")}
                    });

                    tblReport.Rows.Add(row);
                }
            }

            if (cardtype != string.Empty)
            {
                RenderPreviousTotal(totalType);
            }

            var footer = new TableRow {CssClass = "hcGridFooter"};

            footer.Cells.AddRange(new[]
            {
                new TableCell {Text = Localization.GetString("TotalForAllCards"), ColumnSpan = 4},
                new TableCell {Text = TotalGrand.ToString("C")}
            });

            tblReport.Rows.Add(footer);
        }

        private void RenderPreviousTotal(decimal amount)
        {
            var totalRow = new TableRow {CssClass = "hcGridRow"};

            totalRow.Cells.AddRange(new[]
            {
                new TableCell
                {
                    Text = string.Format("<strong>{0}</strong>", amount.ToString("C")),
                    ColumnSpan = 5,
                    CssClass = "hcRight"
                }
            });

            tblReport.Rows.Add(totalRow);
        }

        private void RenderHeader(string cardType)
        {
            var cardName = "";

            switch (cardType)
            {
                case "AMEX":
                    cardName = "AMEX";
                    break;
                case "Visa":
                    cardName = "VISA";
                    break;
                case "GIFTCARD":
                    cardName = "GIFT CARDS";
                    break;
                case "MasterCard":
                    cardName = "MASTERCARD";
                    break;
                case "Discover":
                    cardName = "DISCOVER";
                    break;
                case "Diner's Club":
                    cardName = "DINER'S CLUB";
                    break;
                case "JCB":
                    cardName = "JCB";
                    break;
                default:
                    cardName = string.Format(Localization.GetString("UnknownCardType"), cardType);
                    break;
            }

            var header = new TableRow {CssClass = "hcGridHeader"};

            header.Cells.AddRange(new[]
            {
                new TableCell {Text = string.Format("<strong>{0}</strong>", cardName), ColumnSpan = 5}
            });

            var header2 = new TableRow {CssClass = "hcGridHeader"};

            header2.Cells.AddRange(new[]
            {
                new TableCell {Text = Localization.GetString("Time")},
                new TableCell {Text = Localization.GetString("OrderNumber"), CssClass = "hcRight"},
                new TableCell {Text = Localization.GetString("TransactionNumber"), CssClass = "hcRight"},
                new TableCell {Text = Localization.GetString("CardNumber"), CssClass = "hcRight"},
                new TableCell {Text = Localization.GetString("Amount"), CssClass = "hcRight"}
            });

            tblReport.Rows.Add(header);
            tblReport.Rows.Add(header2);
        }

        #endregion
    }
}