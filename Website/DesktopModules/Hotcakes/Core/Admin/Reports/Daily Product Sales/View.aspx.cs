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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Reports.Daily_Product_Sales
{
    public partial class View : BaseReportPage
    {
        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = Localization.GetString("ProductSales");
            PageMessageBox = ucMessageBox;
        }

        #endregion

        private class ReportData
        {
            public ReportData()
            {
                LineItemBvin = string.Empty;
                OrderBvin = string.Empty;
                ProductId = string.Empty;
                LineTotal = 0;
                UnitPrice = 0;
                Quantity = 0;
                Sku = string.Empty;
                ProductName = string.Empty;
                VariantId = string.Empty;
                BasePrice = 0;
                RefundItems = 0;
                LineDiscountsAsPositiveNumber = 0;

                LineTotalWithoutDiscounts = 0;
            }

            public string LineItemBvin { get; set; }
            public string OrderBvin { get; set; }
            public string ProductId { get; set; }
            public decimal LineTotal { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Quantity { get; set; }
            public string Sku { get; set; }
            public string ProductName { get; set; }

            public string VariantId { get; set; }
            public decimal BasePrice { get; set; }
            public decimal RefundItems { get; set; }

            public decimal LineTotalWithoutDiscounts { get; set; }
            public decimal LineDiscountsAsPositiveNumber { get; set; }
            public DateTime Date { get; set; }
        }

        #region Fields

        protected int TotalCount { get; set; }

        private DateTime ReferredStartDate
        {
            get
            {
                if (Request.QueryString["sd"] != null)
                {
                    return DateTime.Parse(Server.UrlDecode(Request.QueryString["sd"]));
                }

                return DateTime.MinValue;
            }
        }

        private DateTime ReferredEndDate
        {
            get
            {
                if (Request.QueryString["ed"] != null)
                {
                    return DateTime.Parse(Server.UrlDecode(Request.QueryString["ed"]));
                }

                return DateTime.MinValue;
            }
        }

        private DateRangeType ReferredRangeType
        {
            get
            {
                if (Request.QueryString["rt"] != null)
                {
                    try
                    {
                        return (DateRangeType) int.Parse(Request.QueryString["rt"]);
                    }
                    catch
                    {
                        // just default to this week if someone is messing with the URL
                        return DateRangeType.ThisWeek;
                    }
                }

                return DateRangeType.Custom;
            }
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
            if (!Page.IsPostBack && ReferredStartDate > DateTime.MinValue && ReferredEndDate > DateTime.MinValue)
            {
                ucDateRangePicker.RangeType = ReferredRangeType;
                if (ReferredRangeType == DateRangeType.Custom)
                {
                    ucDateRangePicker.StartDate = ReferredStartDate;
                    ucDateRangePicker.EndDate = ReferredEndDate;
                }
            }

            var utcStart = ucDateRangePicker.GetStartDateUtc(HccApp);
            var utcEnd = ucDateRangePicker.GetEndDateUtc(HccApp);

            var totalTempCount = 0;
            var transactions = HccApp.OrderServices.Transactions.FindForReportByDateRange(utcStart,
                utcEnd, HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalTempCount);

            var data = ProcessTransactions(transactions);

            if (data.Count > 0)
            {
                RenderReport(data);
            }

            ShowNoRecordsMessage(data.Count == 0);
        }

        private List<ReportData> ProcessTransactions(List<OrderTransaction> transactions)
        {
            var saleTransactions = transactions.Where(y => y.AmountAppliedToOrder > 0).ToList();
            var refundTransactions = transactions.Where(y => y.AmountAppliedToOrder < 0).ToList();

            // Get a list of distinct order BVIN values where the 
            // payment has an auditdate inside our report range
            var orderdata = saleTransactions.Select(y => y.OrderId).Distinct().ToList();
            var returnOrderData = refundTransactions.Select(y => y.OrderId).Distinct().ToList();
            var rmaData = refundTransactions.Select(y => y.RMABvin).Distinct().ToList();

            // Pull the orders
            var saleOrders = HccApp.OrderServices.Orders.FindMany(orderdata);
            var rmas = HccApp.OrderServices.Returns.FindMany(rmaData);
            // Assign transaction time stamps to rmas so that all RMA are counted in report date range
            foreach (var t in refundTransactions)
            {
                var rma = rmas.FirstOrDefault(y => y.Bvin == t.RMABvin);
                if (rma != null) rma.DateOfReturnUtc = t.TimeStampUtc;
            }
            var returnOrders = HccApp.OrderServices.Orders.FindMany(returnOrderData);

            // Pull the line item data for the orders found in the step above
            var data = new List<ReportData>();
            foreach (var o in saleOrders)
            {
                if (o.StatusCode == OrderStatusCode.Completed ||
                    o.StatusCode == OrderStatusCode.ReadyForShipping)
                {
                    if (o.IsPlaced)
                    {
                        var matchingItems = o.Items;
                        foreach (var li in matchingItems)
                        {
                            var temp = new ReportData
                            {
                                LineItemBvin = li.Id.ToString(),
                                OrderBvin = li.OrderBvin,
                                ProductId = li.ProductId,
                                LineTotal = li.LineTotal,
                                UnitPrice = li.AdjustedPricePerItem,
                                BasePrice = li.BasePricePerItem,
                                Quantity = li.Quantity,
                                Sku = li.ProductSku,
                                ProductName = li.ProductName,
                                VariantId = li.VariantId,
                                LineDiscountsAsPositiveNumber = li.TotalDiscounts()*-1,
                                LineTotalWithoutDiscounts = li.LineTotalWithoutDiscounts,
                                Date = o.TimeOfOrderUtc
                            };
                            data.Add(temp);
                        }
                    }
                }
            }

            // Returns
            foreach (var rma in rmas)
            {
                var rmaOrder = returnOrders.FirstOrDefault(y => y.bvin == rma.OrderBvin);

                foreach (var ri in rma.Items)
                {
                    if (ri.RefundAmount == 0 && ri.RefundGiftWrapAmount == 0 &&
                        ri.RefundShippingAmount == 0 && ri.RefundTaxAmount == 0) continue;

                    var retData = new ReportData
                    {
                        Quantity = -1*ri.QuantityReceived,
                        RefundItems = ri.RefundAmount,
                        LineItemBvin = ri.LineItemId.ToString()
                    };

                    if (rmaOrder == null) continue;
                    var li = rmaOrder.GetLineItem(ri.LineItemId);
                    if (li == null) continue;

                    retData.OrderBvin = rma.OrderBvin;
                    retData.ProductId = li.ProductId;
                    retData.LineTotal = li.LineTotal;
                    retData.UnitPrice = li.AdjustedPricePerItem;
                    retData.BasePrice = li.BasePricePerItem;
                    retData.Sku = li.ProductSku;
                    retData.ProductName = li.ProductName;
                    retData.VariantId = li.VariantId;
                    retData.Date = rmaOrder.TimeOfOrderUtc;

                    // Add returned Item Data to all data
                    data.Add(retData);
                }
            }
            return data;
        }

        private void RenderReport(List<ReportData> data)
        {
            decimal GrandTotal = 0;
            decimal GrandDiscountsTotal = 0;
            decimal GrandQty = 0;

            decimal classTotal = 0;
            decimal classDiscountsTotal = 0;
            decimal classQty = 0;

            // Render Products
            var localproducts = data;

            var uniqueskus = (from p in localproducts
                select p.Sku).Distinct().ToList();

            TotalCount = uniqueskus.Count;

            var childReportLinkFormat =
                "<a href=\"/DesktopModules/Hotcakes/Core/Admin/Reports/Customers by Product/View.aspx?pid={1}&sd={2}&ed={3}&rt={4}\">{0}</a>";

            foreach (var asku in uniqueskus)
            {
                decimal skuQty = 0;
                decimal skuTotal = 0;
                decimal skuDiscountsTotal = 0;
                var productName = string.Empty;

                var allskus = localproducts.Where(p => p.Sku == asku);
                foreach (var p in allskus)
                {
                    productName = p.ProductName;
                    skuQty += p.Quantity;

                    if (p.RefundItems > 0)
                    {
                        // Return
                        skuTotal -= p.RefundItems;
                    }
                    else
                    {
                        // Sale
                        var basetotal = p.LineTotalWithoutDiscounts;
                        skuTotal += basetotal;
                        skuDiscountsTotal += p.LineDiscountsAsPositiveNumber;
                    }
                }
                decimal avgPrice = 0;
                if (skuQty != 0) avgPrice = Math.Abs(skuTotal)/Math.Abs(skuQty);

                var row = new TableRow {CssClass = "hcGridRow"};

                var cellProductId = allskus.FirstOrDefault().ProductId;
                var cellStartDate = Server.UrlEncode(ucDateRangePicker.GetStartDateUtc(HccApp).ToString());
                var cellEndDate = Server.UrlEncode(ucDateRangePicker.GetEndDateUtc(HccApp).ToString());
                var cellDateRangeType = ((int) ucDateRangePicker.RangeType).ToString();

                row.Cells.AddRange(new[]
                {
                    new TableCell
                    {
                        Text = string.Format(childReportLinkFormat,
                            asku,
                            cellProductId,
                            cellStartDate,
                            cellEndDate,
                            cellDateRangeType)
                    },
                    new TableCell
                    {
                        Text = string.Format(childReportLinkFormat,
                            productName,
                            cellProductId,
                            cellStartDate,
                            cellEndDate,
                            cellDateRangeType)
                    },
                    new TableCell {Text = Math.Round(skuQty, 0).ToString(), CssClass = "hcRight"},
                    new TableCell {Text = avgPrice.ToString("C"), CssClass = "hcRight"},
                    new TableCell {Text = skuTotal.ToString("C"), CssClass = "hcRight"},
                    new TableCell {Text = skuDiscountsTotal.ToString("C"), CssClass = "hcRight"},
                    new TableCell {Text = (skuTotal - skuDiscountsTotal).ToString("C"), CssClass = "hcRight"}
                });

                tblReport.Rows.Add(row);

                classTotal += skuTotal;
                classQty += skuQty;
                classDiscountsTotal += skuDiscountsTotal;
            }

            // Render Footer
            GrandTotal += classTotal;
            GrandQty += classQty;
            GrandDiscountsTotal += classDiscountsTotal;

            decimal grandAverage = 0;
            if (GrandQty != 0) grandAverage = GrandTotal/Math.Abs(GrandQty);

            var footer = new TableFooterRow {CssClass = "hcGridFooter"};
            footer.Cells.AddRange(new[]
            {
                new TableCell {Text = Localization.GetString("Total"), CssClass = "hcRight", ColumnSpan = 2},
                new TableCell {Text = Math.Round(GrandQty, 0).ToString(), CssClass = "hcRight"},
                new TableCell {Text = grandAverage.ToString("C"), CssClass = "hcRight"},
                new TableCell {Text = GrandTotal.ToString("C"), CssClass = "hcRight"},
                new TableCell {Text = GrandDiscountsTotal.ToString("C"), CssClass = "hcRight"},
                new TableCell {Text = (GrandTotal - GrandDiscountsTotal).ToString("C"), CssClass = "hcRight"}
            });
            tblReport.Rows.Add(footer);
        }

        #endregion
    }
}