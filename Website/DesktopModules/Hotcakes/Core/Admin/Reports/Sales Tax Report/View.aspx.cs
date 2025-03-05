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
using System.Web;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Orders;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Reports.Sales_Tax_Report
{
    public partial class View : BaseReportPage
    {
        protected int TaxCount;

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageTitle = Localization.GetString("SalesTaxReport");
            PageMessageBox = ucMessageBox;
            btnDownloadReport.Click += btnDownloadReport_Click;

            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var orders = GetAllSelectedOrders();
            var reportData = new List<TaxReportLine>();

            foreach (var order in orders)
            {
                if (order.StatusCode == OrderStatusCode.Completed ||
                    order.StatusCode == OrderStatusCode.ReadyForShipping)
                {
                    var reportLine = reportData.
                        Where(rl => rl.CountryBvin == order.ShippingAddress.CountryBvin).
                        Where(rl => rl.RegionBvin == order.ShippingAddress.RegionBvin).
                        FirstOrDefault();

                    if (reportLine == null)
                    {
                        reportLine = new TaxReportLine
                        {
                            CountryBvin = order.ShippingAddress.CountryBvin,
                            RegionBvin = order.ShippingAddress.RegionBvin,
                            CountryName = order.ShippingAddress.CountryDisplayName,
                            RegionName = order.ShippingAddress.RegionDisplayName
                        };

                        reportData.Add(reportLine);
                    }

                    reportLine.TotalTax += order.TotalTax;
                }
            }

            TaxCount = reportData.Count;

            gvTaxReport.DataSource = reportData;
            gvTaxReport.DataBind();

            ShowNoRecordsMessage(TaxCount == 0);
        }

        private void btnDownloadReport_Click(object sender, EventArgs e)
        {
            var orders = GetAllSelectedOrders();

            var response = HttpContext.Current.Response;

            var filename = "SalesTaxReport.csv";
            CsvWriter.InitHttpResponse(response, filename);

            using (var csv = new CsvWriter(response.OutputStream))
            {
                csv.WriteLine(Localization.GetString("OrderNumber"), Localization.GetString("OrderDate"),
                    Localization.GetString("OrderTotal"), Localization.GetString("TaxTotal"),
                    Localization.GetString("CountryName"), Localization.GetString("RegionName"),
                    Localization.GetString("PostalCode"));

                foreach (var order in orders)
                {
                    if (order.StatusCode == OrderStatusCode.Completed ||
                        order.StatusCode == OrderStatusCode.ReadyForShipping)
                    {
                        csv.WriteLine(order.OrderNumber, order.TimeOfOrderUtc.ToString(), order.TotalGrand.ToString("C"),
                            order.TotalTax.ToString("C"), order.ShippingAddress.CountryDisplayName,
                            order.ShippingAddress.RegionDisplayName, order.ShippingAddress.PostalCode);
                    }
                }
            }

            response.Flush();
            response.Close();
        }

        protected void gvTaxReport_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Country");
                e.Row.Cells[1].Text = Localization.GetString("Region");
                e.Row.Cells[2].Text = Localization.GetString("TotalTax");
            }
        }

        #endregion

        #region Implementation

        private void ShowNoRecordsMessage(bool show)
        {
            pnlReportData.Visible = !show;
            lblNoTransactionsMessage.Visible = show;
        }

        protected override void BindReport()
        {
            LoadData();
        }

        private List<OrderSnapshot> GetAllSelectedOrders()
        {
            var creteria = new OrderSearchCriteria
            {
                StartDateUtc = ucDateRangePicker.GetStartDateUtc(HccApp),
                EndDateUtc = ucDateRangePicker.GetEndDateUtc(HccApp)
            };

            var orders = HccApp.OrderServices.Orders.FindByCriteria(creteria);

            return orders;
        }

        #endregion
    }
}