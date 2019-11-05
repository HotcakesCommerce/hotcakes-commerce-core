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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Reports.Sales_By_Date
{
    partial class View : BaseReportPage
    {
        #region Fields

        protected decimal TotalGrand;
        protected int TotalCount;

        private decimal TotalSub;
        private decimal TotalShip;
        private decimal TotalHandling;
        private decimal TotalTax;
        private decimal TotalShipDiscounts;
        private decimal TotalDiscounts;
        private bool _isExcelExport;

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = Localization.GetString("OrdersByDate");
            PageMessageBox = ucMessageBox;
            lnkExport.Click += lnkExport_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LocalizationUtils.LocalizeDataGrid(dgList, Localization);
        }

        private void lnkExport_Click(object sender, EventArgs e)
        {
            _isExcelExport = true;
            TotalSub = 0;
            TotalShip = 0;
            TotalHandling = 0;
            TotalTax = 0;
            TotalGrand = 0;
            TotalCount = 0;
            TotalDiscounts = 0;
            TotalShipDiscounts = 0;

            var orders = BuildReportData();

            GenerateExcelFile(orders);
        }

        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var order = (OrderSnapshot) e.Item.DataItem;

                var lblDate = (Label) e.Item.FindControl("lblDate");

                if (lblDate != null)
                {
                    lblDate.Text =
                        TimeZoneInfo.ConvertTimeFromUtc(order.TimeOfOrderUtc, HccApp.CurrentStore.Settings.TimeZone)
                            .ToShortDateString();
                }

                var lnkViewOrder = (HyperLink) e.Item.FindControl("lnkViewOrder");

                if (lnkViewOrder != null)
                {
                    lnkViewOrder.NavigateUrl = "~/DesktopModules/Hotcakes/Core/Admin/Orders/ViewOrder.aspx?id=" +
                                               order.bvin;
                    lnkViewOrder.Text = Localization.GetString("ViewOrder");
                }

                if (order.OrderNumber == "DayTotal")
                {
                    var cell = new TableCell
                    {
                        ColumnSpan = 7,
                        Text = Localization.GetString("DayTotal")
                    };
                    e.Item.Cells.Clear();
                    e.Item.Cells.Add(cell);
                    e.Item.Cells.Add(new TableCell {Text = order.TotalGrand.ToString("C")});
                    e.Item.ControlStyle.CssClass = "hcGridFooter";
                }
                else if (order.OrderNumber == "MonthTotal")
                {
                    var cell = new TableCell
                    {
                        ColumnSpan = 7,
                        Text = Localization.GetString("MonthTotal")
                    };
                    e.Item.Cells.Clear();
                    e.Item.Cells.Add(cell);
                    e.Item.Cells.Add(new TableCell {Text = order.TotalGrand.ToString("C")});
                    e.Item.ControlStyle.CssClass = "hcGridFooter";
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                e.Item.Cells[0].Text = Localization.GetString("Totals");
                e.Item.Cells[2].Text = TotalSub.ToString("C");
                e.Item.Cells[3].Text = TotalDiscounts.ToString("C");
                e.Item.Cells[4].Text = TotalShip.ToString("C");
                e.Item.Cells[5].Text = TotalShipDiscounts.ToString("C");
                e.Item.Cells[6].Text = TotalTax.ToString("C");
                e.Item.Cells[7].Text = TotalGrand.ToString("C");
            }
        }

        #endregion

        #region Implementation

        private void ShowNoRecordsMessage(bool show)
        {
            pnlReportData.Visible = !show;
            lblNoTransactionsMessage.Visible = show;
            lnkExport.Visible = !show;
        }

        protected override void BindReport()
        {
            // Skip twice report building
            if (!_isExcelExport)
            {
                TotalSub = 0;
                TotalShip = 0;
                TotalHandling = 0;
                TotalTax = 0;
                TotalGrand = 0;
                TotalCount = 0;
                TotalDiscounts = 0;
                TotalShipDiscounts = 0;

                var orders = BuildReportData();

                if (orders.Count > 0)
                {
                    dgList.DataSource = orders;
                    dgList.DataBind();
                }

                ShowNoRecordsMessage(orders.Count == 0);
            }
        }

        private List<OrderSnapshot> BuildReportData()
        {
            var c = new OrderSearchCriteria();

            var utcStart = DateRangeField.GetStartDateUtc(HccApp);
            var utcEnd = DateRangeField.GetEndDateUtc(HccApp);

            c.StartDateUtc = utcStart;
            c.EndDateUtc = utcEnd;

            var orders = HccApp.OrderServices.Orders.FindByCriteria(c);

            TotalCount = orders.Count;

            foreach (var o in orders)
            {
                TotalSub += o.TotalOrderBeforeDiscounts;
                TotalDiscounts += o.TotalOrderDiscounts;
                TotalShip += o.TotalShippingBeforeDiscounts;
                TotalShipDiscounts += o.TotalShippingDiscounts;
                TotalHandling += o.TotalHandling;
                TotalTax += o.TotalTax;
                TotalGrand += o.TotalGrand;
            }

            var i = 0;
            var month = string.Empty;
            var monthTotal = 0m;
            var dayTotal = 0m;
            var day = string.Empty;

            if (orders.Count > 0)
            {
                var zonedTimeOfOrder = DateHelper.ConvertUtcToStoreTime(HccApp, orders[0].TimeOfOrderUtc);
                month = string.Concat(zonedTimeOfOrder.Month, ":", zonedTimeOfOrder.Year);
                day = string.Concat(zonedTimeOfOrder.DayOfYear, ":", zonedTimeOfOrder.Year);

                while (i <= orders.Count - 1)
                {
                    var zonedTime = DateHelper.ConvertUtcToStoreTime(HccApp, orders[i].TimeOfOrderUtc);
                    monthTotal = monthTotal + orders[i].TotalGrand;
                    dayTotal = dayTotal + orders[i].TotalGrand;

                    if (string.Concat(zonedTime.DayOfYear, ":", zonedTime.Year) != day)
                    {
                        day = string.Concat(zonedTime.DayOfYear, ":", zonedTime.Year);

                        // we need to insert a day total
                        var order = new OrderSnapshot();

                        order.OrderNumber = "DayTotal";
                        order.TotalGrand = dayTotal - orders[i].TotalGrand;

                        dayTotal = orders[i].TotalGrand;
                        orders.Insert(i, order);
                        i += 1;
                    }

                    if (string.Concat(zonedTime.Month, ":", zonedTime.Year) != month)
                    {
                        month = string.Concat(zonedTime.Month, ":", zonedTime.Year);

                        // we need to insert a month total
                        var order = new OrderSnapshot();

                        order.OrderNumber = "MonthTotal";
                        order.TotalGrand = monthTotal - orders[i].TotalGrand;

                        monthTotal = orders[i].TotalGrand;
                        orders.Insert(i, order);
                        i += 1;
                    }

                    i += 1;
                }

                if (dayTotal > 0)
                {
                    // we need to insert a day total
                    var order = new OrderSnapshot();

                    order.OrderNumber = "DayTotal";
                    order.TotalGrand = dayTotal;
                    orders.Add(order);
                }

                if (monthTotal > 0)
                {
                    var order = new OrderSnapshot();

                    order.OrderNumber = "MonthTotal";
                    order.TotalGrand = monthTotal;
                    orders.Add(order);
                }
            }

            return orders;
        }

        private void GenerateExcelFile(List<OrderSnapshot> orders)
        {
            var oExport = new OrdersExport(HccApp);
            oExport.ExportToExcel(Response, "Hotcakes_Orders.xlsx",
                orders.Where(o => o.OrderNumber != "DayTotal" && o.OrderNumber != "MonthTotal").ToList());
        }

        #endregion
    }
}