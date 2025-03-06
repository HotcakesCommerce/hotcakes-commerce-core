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

// TODO: pass in & use date ranges
// TODO: wire up the excel export

namespace Hotcakes.Modules.Core.Admin.Reports.CustomersByProduct
{
    public partial class View : BaseReportPage
    {
        [Serializable]
        private class CustomerProductReportData
        {
            public CustomerProductReportData()
            {
                CustomerId = -1;
                ProductName = string.Empty;
                FirstName = string.Empty;
                LastName = string.Empty;
                EmailAddress = string.Empty;
            }

            public int CustomerId { get; set; }
            public string ProductName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
            public string PhoneNumber { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string Country { get; set; }
            public string PostalCode { get; set; }
        }

        #region Local Properties

        protected int TotalCount { get; set; }
        protected string ProductId { get; set; }

        private string _productName = string.Empty;

        protected string ProductName
        {
            get
            {
                if (!string.IsNullOrEmpty(_productName))
                {
                    return _productName;
                }

                var product = HccApp.CatalogServices.Products.Find(ProductId);

                if (product != null)
                {
                    _productName = product.ProductName;
                }

                return _productName;
            }
        }

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

        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            PageTitle = Localization.GetString("CustomerProductSales");
            PageMessageBox = ucMessageBox;

            if (Request.Params["pid"] != null)
            {
                ProductId = Request.Params["pid"];
            }
            else
            {
                PageMessageBox.ShowError(Localization.GetString("MissingParameters"));
            }
        }

        protected void lnkExport_OnClick(object sender, EventArgs e)
        {
            var customers = GetReportData();
            GenerateExcelFile(customers);
        }

        protected void lnkReturn_OnClick(object sender, EventArgs e)
        {
            var parentReportLinkFormat =
                "/DesktopModules/Hotcakes/Core/Admin/Reports/Daily Product Sales/View.aspx?sd={0}&ed={1}&rt={2}";
            Response.Redirect(string.Format(parentReportLinkFormat,
                Server.UrlEncode(ReferredStartDate.ToString()),
                Server.UrlEncode(ReferredEndDate.ToString()),
                (int) ucDateRangePicker.RangeType));
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
            var customers = GetReportData();

            if (customers != null)
            {
                RenderReport(customers);

                TotalCount = customers.Count();

                ShowNoRecordsMessage(customers.Count == 0);
            }

            lnkExport.Visible = TotalCount > 0;
        }

        private List<CustomerProductReportData> GetReportData()
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

            // get orders that have the product id
            var totalCount = 0;

            var snapOrders = HccApp.OrderServices.Orders.FindByCriteriaPaged(new OrderSearchCriteria
            {
                IsPlaced = true,
                PaymentStatus = OrderPaymentStatus.Paid,
                ShippingStatus = OrderShippingStatus.FullyShipped,
                StartDateUtc = utcStart,
                EndDateUtc = utcEnd
            }, 1, int.MaxValue, ref totalCount);

            if (totalCount > 0)
            {
                var orderIds = snapOrders.Select(o => o.bvin).ToList();
                var orders = HccApp.OrderServices.Orders.FindMany(orderIds);

                var relevantOrders = orders.Where(o => o.Items.Any(li => li.ProductId == ProductId));

                var customerOrders = relevantOrders.GroupBy(o => o.UserID).Select(u => u.First());

                if (customerOrders.Any())
                {
                    var bindableCustomers = new List<CustomerProductReportData>();

                    foreach (var order in customerOrders)
                    {
                        bindableCustomers.Add(new CustomerProductReportData
                        {
                            CustomerId = int.Parse(order.UserID),
                            ProductName = ProductName,
                            FirstName = order.BillingAddress.FirstName,
                            LastName = order.BillingAddress.LastName,
                            EmailAddress = order.UserEmail,
                            PhoneNumber = order.BillingAddress.Phone,
                            Line1 = order.BillingAddress.Line1,
                            Line2 = order.BillingAddress.Line2,
                            City = order.BillingAddress.City,
                            Region = order.BillingAddress.RegionDisplayName,
                            Country = order.BillingAddress.CountryDisplayName,
                            PostalCode = order.BillingAddress.PostalCode
                        });
                    }

                    return bindableCustomers;
                }
            }

            return null;
        }

        private void RenderReport(List<CustomerProductReportData> customers)
        {
            foreach (var customer in customers)
            {
                var row = new TableRow {CssClass = "hcGridRow"};
                row.Cells.AddRange(new[]
                {
                    new TableCell {Text = customer.FirstName},
                    new TableCell {Text = customer.LastName},
                    new TableCell {Text = customer.EmailAddress}
                });

                tblReport.Rows.Add(row);
            }
        }

        private void GenerateExcelFile(List<CustomerProductReportData> customers)
        {
            var oExport = new OrderCustomersExport(HccApp);
            var ids = customers.Select(c => c.CustomerId.ToString()).ToList();
            var iCustomers = HccApp.MembershipServices.Customers.FindMany(ids);
            var productName = customers.Select(c => c.ProductName).FirstOrDefault();
            oExport.ExportToExcel(Response, "Hotcakes_Customers.xlsx", iCustomers, productName);
        }

        #endregion
    }
}