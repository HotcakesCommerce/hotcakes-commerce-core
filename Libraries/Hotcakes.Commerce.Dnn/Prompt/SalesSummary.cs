#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Dnn.PersonaBar.Library.Prompt;
using Dnn.PersonaBar.Library.Prompt.Attributes;
using Dnn.PersonaBar.Library.Prompt.Models;
using DotNetNuke.Instrumentation;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Dnn.Prompt
{
    [ConsoleCommand("sales-summary", Constants.Namespace, "Displays a summary of the sales in the store.")]
    public class SalesSummary : PromptBase, IConsoleCommand
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(SalesSummary));

        public override ConsoleResultModel Run()
        {
            try
            {
                var list = new List<SalesSummaryOutput>();

                list.Add(MonthSummary());
                list.Add(QuarterSummary());
                list.Add(YearSummary());

                return new ConsoleResultModel
                {
                    Data = list,
                    Output = string.Concat(Constants.OutputPrefix, LocalizeString("ViewReports"))
                };
            }
            catch (Exception e)
            {
                LogError(e);
                return new ConsoleErrorResultModel(string.Concat(Constants.OutputPrefix, LocalizeString("ErrorOccurred")));
            }
        }

        protected override void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);
                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }

        private SalesSummaryOutput MonthSummary()
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var totalTempCount = 0;
            var transactionsData = HccApp.OrderServices.Transactions.FindForReportByDateRange(startDate.ToUniversalTime(),
                endDate.ToUniversalTime(), HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalTempCount);

            var transactionCount = transactionsData.Count;
            var data = ProcessTransactions(transactionsData);

            decimal orderTotal = data.Orders.Sum(order => order.TotalOrderBeforeDiscounts);
            decimal rmaTotal = data.Rmas.Sum(rma => rma.TotalRefundItemAmount);
            var grandTotal = orderTotal - rmaTotal;

            return new SalesSummaryOutput
            {
                TimePeriod = LocalizeString("SalesSummaryMonth"),
                Transactions = transactionCount,
                AvgOrder = (grandTotal / transactionCount).ToString("C"),
                Total = grandTotal.ToString("C")
            };
        }

        private SalesSummaryOutput QuarterSummary()
        {
            var now = DateTime.Now;
            var currQuarter = (now.Month - 1) / 3 + 1;
            var startDate = new DateTime(now.Year, 3 * currQuarter - 2, 1);
            var endDate = new DateTime(now.Year, 3 * currQuarter + 1, 1).AddDays(-1);

            var totalTempCount = 0;
            var transactionsData = HccApp.OrderServices.Transactions.FindForReportByDateRange(startDate.ToUniversalTime(),
                endDate.ToUniversalTime(), HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalTempCount);

            var transactionCount = transactionsData.Count;
            var data = ProcessTransactions(transactionsData);

            decimal orderTotal = data.Orders.Sum(order => order.TotalOrderBeforeDiscounts);
            decimal rmaTotal = data.Rmas.Sum(rma => rma.TotalRefundItemAmount);
            var grandTotal = orderTotal - rmaTotal;

            return new SalesSummaryOutput
            {
                TimePeriod = LocalizeString("SalesSummaryQuarter"),
                Transactions = transactionCount,
                AvgOrder = (grandTotal / transactionCount).ToString("C"),
                Total = grandTotal.ToString("C")
            };
        }

        private SalesSummaryOutput YearSummary()
        {
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, 1, 1);
            var endDate = new DateTime(now.Year, 12, 31);

            var totalTempCount = 0;
            var transactionsData = HccApp.OrderServices.Transactions.FindForReportByDateRange(startDate.ToUniversalTime(),
                endDate.ToUniversalTime(), HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalTempCount);

            var transactionCount = transactionsData.Count;
            var data = ProcessTransactions(transactionsData);

            decimal orderTotal = data.Orders.Sum(order => order.TotalOrderBeforeDiscounts);
            decimal rmaTotal = data.Rmas.Sum(rma => rma.TotalRefundItemAmount);
            var grandTotal = orderTotal - rmaTotal;

            return new SalesSummaryOutput
            {
                TimePeriod = LocalizeString("SalesSummaryYear"),
                Transactions = transactionCount,
                AvgOrder = (grandTotal / transactionCount).ToString("C"),
                Total = grandTotal.ToString("C")
            };
        }

        private TransactionData ProcessTransactions(List<OrderTransaction> data)
        {
            var saleTransactions = data.Where(y => y.AmountAppliedToOrder > 0).ToList();
            var refundTransactions = data.Where(y => y.AmountAppliedToOrder < 0).ToList();

            // Get a list of distinct order BVIN values where the 
            // payment has an audit date inside our report range
            var orderIds = saleTransactions.Select(y => y.OrderId).Distinct().ToList();
            var rmaIds = refundTransactions.Select(y => y.RMABvin).Distinct().ToList();

            var outData = new TransactionData
            {
                Orders = HccApp.OrderServices.Orders.FindMany(orderIds) ?? new List<Order>(),
                Rmas = HccApp.OrderServices.Returns.FindMany(rmaIds) ?? new List<RMA>()
            };

            // Assign transaction time stamps to rmas so that all RMA are counted in report date range
            foreach (var t in refundTransactions)
            {
                var rma = outData.Rmas.FirstOrDefault(y => y.Bvin == t.RMABvin);
                if (rma != null)
                {
                    rma.DateOfReturnUtc = t.TimeStampUtc;
                }
            }

            return outData;
        }
    }

    public class SalesSummaryOutput
    {
        public string TimePeriod { get; set; }
        public string Total { get; set; }
        public int Transactions { get; set; }
        public string AvgOrder { get; set; }
    }

    public class TransactionData
    {
        public List<Order> Orders { get; set; }
        public List<RMA> Rmas { get; set; }
    }
}
