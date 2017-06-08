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
using System.Linq;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Reports.Summary_Report
{
    public partial class View : BaseReportPage
    {
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PageTitle = Localization.GetString("SummaryReport");
            PageMessageBox = ucMessageBox;
        }

        #endregion

        internal class SummaryReportItem
        {
            internal SummaryReportItem()
            {
            }

            internal SummaryReportItem(string description, decimal? details, decimal? total)
            {
                Description = description;
                Details = details;
                Total = total;
            }

            public string Description { get; set; }
            public decimal? Details { get; set; }
            public decimal? Total { get; set; }
        }

        internal class CCSummaryItem
        {
            internal CCSummaryItem(string cartType, int count, decimal amount)
            {
                CartType = cartType;
                Count = count;
                Amount = amount;
            }

            public string CartType { get; set; }
            public int Count { get; set; }
            public decimal Amount { get; set; }
        }

        #region Fields

        protected List<OrderTransaction> TransactionsData;

        protected decimal RepGrandTotal;
        protected decimal RepCreditAll;
        protected decimal RepCCGrandTotal;
        protected int ItemsCount;

        #endregion

        #region Implementation

        private void ShowNoRecordsMessage(bool show)
        {
            pnlReportData.Visible = !show;
            lblNoTransactionsMessage.Visible = show;
        }

        protected override void BindReport()
        {
            var utcStart = ucDateRangePicker.GetStartDateUtc(HccApp);
            var utcEnd = ucDateRangePicker.GetEndDateUtc(HccApp);

            var totalTempCount = 0;
            TransactionsData = HccApp.OrderServices.Transactions.FindForReportByDateRange(utcStart,
                utcEnd, HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalTempCount);

            List<Order> orders;
            List<RMA> rmaData;
            ProcessTransactions(out orders, out rmaData);

            ItemsCount = orders.Count + rmaData.Count;

            if (ItemsCount > 0)
            {
                RenderReport(orders, rmaData);
                RenderCCSummary(TransactionsData);
            }

            ShowNoRecordsMessage(ItemsCount == 0);
        }

        private void ProcessTransactions(out List<Order> orders, out List<RMA> rmaData)
        {
            var saleTransactions = TransactionsData.Where(y => y.AmountAppliedToOrder > 0).ToList();
            var refundTransactions = TransactionsData.Where(y => y.AmountAppliedToOrder < 0).ToList();

            // Get a list of distinct order BVIN values where the 
            // payment has an auditdate inside our report range
            var orderIds = saleTransactions.Select(y => y.OrderId).Distinct().ToList();
            var rmaIds = refundTransactions.Select(y => y.RMABvin).Distinct().ToList();

            // Pull the orders            
            orders = HccApp.OrderServices.Orders.FindMany(orderIds) ?? new List<Order>();
            rmaData = HccApp.OrderServices.Returns.FindMany(rmaIds) ?? new List<RMA>();

            // Assign transaction time stamps to rmas so that all RMA are counted in report date range
            foreach (var t in refundTransactions)
            {
                var rma = rmaData.FirstOrDefault(y => y.Bvin == t.RMABvin);
                if (rma != null) rma.DateOfReturnUtc = t.TimeStampUtc;
            }
        }

        private void RenderReport(List<Order> orders, List<RMA> rmaData)
        {
            decimal shippingTotal = 0;
            decimal handlingTotal = 0;
            decimal taxTotal = 0;
            decimal creditShipping = 0;
            decimal creditTax = 0;
            decimal itemsTotal = 0;
            decimal creditItems = 0;
            var data = new List<SummaryReportItem>();

            foreach (var dataitem in orders)
            {
                itemsTotal += dataitem.TotalOrderBeforeDiscounts;

                if (dataitem.TotalShippingAfterDiscounts > 0)
                {
                    // We skip calculation for zero shipping
                    shippingTotal += dataitem.TotalShippingAfterDiscounts - dataitem.TotalHandling;
                    handlingTotal += dataitem.TotalHandling;
                }

                taxTotal += dataitem.TotalTax;
                RepGrandTotal += dataitem.TotalGrand;

                data.Add(new SummaryReportItem(string.Empty, dataitem.TotalOrderBeforeDiscounts, null));
            }

            foreach (var returnItem in rmaData)
            {
                RepCreditAll += returnItem.TotalGrandRefundAmount;

                creditItems += returnItem.TotalRefundItemAmount;
                creditShipping += returnItem.TotalRefundShippingAmount;
                creditTax += returnItem.TotalRefundTaxAmount;

                data.Add(new SummaryReportItem(string.Empty, -1 * returnItem.TotalRefundItemAmount, null));
            }

            RepCreditAll = -1*RepCreditAll;

            data.Insert(0,
                new SummaryReportItem(Localization.GetString("TotalMerchandise"), null, itemsTotal - creditItems));
            data.Add(new SummaryReportItem(Localization.GetString("Shipping"), null, shippingTotal));
            data.Add(new SummaryReportItem(Localization.GetString("ShippingReturns"), null, -1*creditShipping));
            data.Add(new SummaryReportItem(Localization.GetString("Handling"), null, handlingTotal));
            data.Add(new SummaryReportItem(Localization.GetString("Tax"), null, taxTotal));
            data.Add(new SummaryReportItem(Localization.GetString("TaxReturn"), null, -1*creditTax));

            rpSummary.DataSource = data;
            rpSummary.DataBind();
        }

        private void RenderCCSummary(List<OrderTransaction> tranData)
        {
            var data = new List<CCSummaryItem>();
            var cardtype = string.Empty;
            decimal totalType = 0;
            var transactionCount = 0;

            foreach (var dataitem in tranData)
            {
                if (!ActionTypeUtils.IsCreditCardTransaction(dataitem.Action))
                {
                    continue;
                }

                if (dataitem.CreditCard.CardTypeName != cardtype)
                {
                    if (cardtype != string.Empty)
                    {
                        data.Add(new CCSummaryItem(cardtype, transactionCount, totalType));
                    }
                    totalType = 0;
                    transactionCount = 0;
                    cardtype = dataitem.CreditCard.CardTypeName;
                }

                if (dataitem.AmountAppliedToOrder != 0)
                {
                    totalType += dataitem.AmountAppliedToOrder;
                    RepCCGrandTotal += dataitem.AmountAppliedToOrder;
                    transactionCount += 1;
                }
            }

            if (!string.IsNullOrEmpty(cardtype))
            {
                data.Add(new CCSummaryItem(cardtype, transactionCount, totalType));
            }

            rpCCSummary.DataSource = data;
            rpCCSummary.DataBind();
        }

        #endregion
    }
}