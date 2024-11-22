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
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Data;

namespace Hotcakes.Modules.Core.Admin.Reports.PaymentFailures
{
    public partial class View : BaseReportPage
    {
        private const string USER_NAME_FORMAT = "{0} {1}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            rpCarts.ItemDataBound += rpCarts_ItemDataBound;
            rpProducts.ItemDataBound += rpProducts_ItemDataBound;

            lnkExportToExcel.Click += lnkExportToExcel_Click;
            lnkDownloadContacts.Click += lnkDownloadContacts_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageTitle = Localization.GetString("PaymentFailures");
            PageMessageBox = ucMessageBox;
        }

        protected override void BindReport()
        {
            var startDate = ucDateRangePicker.StartDate;
            var endDate = ucDateRangePicker.EndDate;
            var pageNumber = ucPager.PageNumber;
            var pageSize = ucPager.PageSize;
            int totalCount;

            var showAgregatedReport = bool.Parse(rblAgregatedReport.SelectedValue);
            rpCarts.Visible = !showAgregatedReport;
            rpProducts.Visible = showAgregatedReport;
            if (showAgregatedReport)
            {
                var reportingService = Factory.CreateService<ReportingService>();
                var resultProducts = reportingService.GetPaymentFailure(startDate, endDate, pageNumber, pageSize,
                    out totalCount);

                var isDataPresent = resultProducts != null && resultProducts.Count > 0;
                divNavBottom.Visible = isDataPresent;
                lblNoCartsMessage.Visible = !isDataPresent;
                pnlReportsData.Visible = isDataPresent;

                rpProducts.DataSource = resultProducts;
                rpProducts.DataBind();
            }
            else
            {
                var result = HccApp.OrderServices.Orders.FindPaymentFailure(startDate, endDate, pageNumber,
                    pageSize, out totalCount);

                var isDataPresent = result != null && result.Count > 0;
                divNavBottom.Visible = isDataPresent;
                lblNoCartsMessage.Visible = !isDataPresent;
                pnlReportsData.Visible = isDataPresent;

                rpCarts.DataSource = result;
                rpCarts.DataBind();
            }

            ucPager.SetRowCount(totalCount);
        }

        private void rpCarts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var tdId = e.Item.FindControl("tdId") as HtmlTableCell;
                var tdDate = e.Item.FindControl("tdDate") as HtmlTableCell;
                var hlUser = e.Item.FindControl("hlUser") as HyperLink;
                var lbUser = e.Item.FindControl("lbUser") as Label;
                var hlCart = e.Item.FindControl("hlCart") as HyperLink;
                var rpCartItems = e.Item.FindControl("rpCartItems") as Repeater;

                var cart = e.Item.DataItem as Order;
                tdId.InnerText = cart.Id.ToString();

                var user = HccApp.MembershipServices.Customers.Find(cart.UserID);

                tdDate.InnerText = DateHelper.ConvertUtcToStoreTime(HccApp, cart.TimeOfOrderUtc).ToString("d");
                if (user != null)
                {
                    hlUser.Text = string.Format(USER_NAME_FORMAT, user.FirstName, user.LastName);
                    if (string.IsNullOrEmpty(hlUser.Text.Trim()))
                    {
                        hlUser.Text = Localization.GetString("NoName");
                    }
                    hlUser.NavigateUrl = string.Format("../../People/Users_Edit.aspx?id={0}&returnUrl={1}", user.Bvin,
                        "Y");
                }
                else
                {
                    lbUser.Text = Localization.GetString("Anonymous");
                }

                hlCart.NavigateUrl = string.Format("../../Orders/CreateOrder.aspx?id={0}&returnUrl={1}", cart.bvin, "Y");

                rpCartItems.DataSource = cart.Items;
                rpCartItems.DataBind();
            }
        }

        private void rpProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var hlProduct = e.Item.FindControl("hlProduct") as HyperLink;
                var lblProduct = e.Item.FindControl("lblProduct") as Label;
                var tdQuantity = e.Item.FindControl("tdQuantity") as HtmlTableCell;
                var tdCount = e.Item.FindControl("tdCount") as HtmlTableCell;
                var lblContactsCount = e.Item.FindControl("lblContactsCount") as Label;
                var lnkContactsCount = e.Item.FindControl("lnkContactsCount") as LinkButton;

                var abandonedProduct = e.Item.DataItem as AbandonedProduct;

                var productBvin = DataTypeHelper.GuidToBvin(abandonedProduct.ProductGuid);
                var product = HccApp.CatalogServices.Products.FindWithCache(productBvin);
                var productExists = product != null;
                hlProduct.Visible = productExists;
                lblProduct.Visible = !productExists;
                hlProduct.Text = abandonedProduct.ProductName;
                hlProduct.NavigateUrl = string.Format("../../Catalog/Products_Performance.aspx?id={0}&returnUrl={1}",
                    abandonedProduct.ProductGuid, "Y");
                lblProduct.Text = abandonedProduct.ProductName;

                tdQuantity.InnerText = abandonedProduct.Quantity.ToString();
                tdCount.InnerText = abandonedProduct.CartsCount.ToString();

                var contactsExist = abandonedProduct.ContactsCount > 0;
                lblContactsCount.Visible = !contactsExist;
                lnkContactsCount.Visible = contactsExist;
                lblContactsCount.Text = abandonedProduct.ContactsCount.ToString();
                lnkContactsCount.Text = abandonedProduct.ContactsCount.ToString();
                lnkContactsCount.CommandArgument = abandonedProduct.ProductGuid.ToString();
            }
        }

        private void lnkExportToExcel_Click(object sender, EventArgs e)
        {
            var startDate = ucDateRangePicker.StartDate;
            var endDate = ucDateRangePicker.EndDate;
            int totalCount;

            var showAgregatedReport = bool.Parse(rblAgregatedReport.SelectedValue);
            if (showAgregatedReport)
            {
                var reportingService = Factory.CreateService<ReportingService>();
                var abandonedProducts = reportingService.GetPaymentFailure(startDate, endDate, 1, int.MaxValue,
                    out totalCount);

                var abandonedCartsExport = new AbandonedCartsExport(HccApp);
                abandonedCartsExport.ExportToExcel(Response, "AbandonedCartsByProducts.xlsx", abandonedProducts);
            }
            else
            {
                var abandonedCarts = HccApp.OrderServices.Orders.FindAbandonedCarts(startDate, endDate, 1, int.MaxValue,
                    out totalCount);

                var abandonedCartsExport = new AbandonedCartsExport(HccApp);
                abandonedCartsExport.ExportToExcel(Response, "AbandonedCartsByCarts.xlsx", abandonedCarts);
            }
        }

        private void lnkDownloadContacts_Click(object sender, EventArgs e)
        {
            var startDate = ucDateRangePicker.StartDate;
            var endDate = ucDateRangePicker.EndDate;

            var userIds = HccApp.OrderServices.Orders.FindAbandonedCartsUsers(startDate, endDate);
            var users = HccApp.MembershipServices.Customers.FindMany(userIds);

            var response = HttpContext.Current.Response;

            var filename = "Contacts.csv";
            CsvWriter.InitHttpResponse(response, filename);

            using (var csv = new CsvWriter(response.OutputStream))
            {
                csv.WriteLine(Localization.GetString("FirstName"), Localization.GetString("LastName"),
                    Localization.GetString("Email"));

                foreach (var user in users)
                {
                    csv.WriteLine(user.FirstName, user.LastName, user.Email);
                }
            }

            response.Flush();
            response.Close();
        }

        protected void lnkContactsCount_Command(object sender, CommandEventArgs e)
        {
            var startDate = ucDateRangePicker.StartDate;
            var endDate = ucDateRangePicker.EndDate;
            var productId = (string) e.CommandArgument;

            ucContactAbandonedCartUsers.ShowContactDialog(startDate, endDate, productId);
        }
    }
}