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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Reports.Daily_Sales
{
    partial class View : BaseReportPage
    {
        #region Fields

        protected decimal TotalSub;
        protected decimal TotalDiscounts;
        protected decimal TotalShip;
        protected decimal TotalShipDiscounts;
        protected decimal TotalTax;
        protected decimal TotalGrand;
        protected int TotalCount;

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ucDatePickerNav.SelectedDateChanged += (s, a) => { dgList.CurrentPageIndex = 0; };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageTitle = Localization.GetString("TransactionsByDay");
            PageMessageBox = ucMessageBox;

            if (!IsPostBack)
            {
                ucDatePickerNav.SelectedDate = GetInitialDate();

                LocalizationUtils.LocalizeDataGrid(dgList, Localization);
            }
        }

        protected void dgList_PageIndexChanged(object sender, DataGridPageChangedEventArgs e)
        {
            dgList.CurrentPageIndex = e.NewPageIndex;
        }

        protected void dgList_Edit(object sender, DataGridCommandEventArgs e)
        {
            var orderID = (string) dgList.DataKeys[e.Item.ItemIndex];

            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Orders/ViewOrder.aspx?id=" + orderID, true);
        }

        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var t = (OrderTransaction) e.Item.DataItem;
                if (t == null) return;

                var litTimeStamp = e.Item.FindControl("litTimeStamp") as Literal;
                var litOrderNumber = e.Item.FindControl("litOrderNumber") as Literal;
                var litDescription = e.Item.FindControl("litDescription") as Literal;
                var litAmount = e.Item.FindControl("litAmount") as Literal;
                var litCustomerName = e.Item.FindControl("litCustomerName") as Literal;

                if (litTimeStamp != null)
                {
                    litTimeStamp.Text =
                        TimeZoneInfo.ConvertTimeFromUtc(t.TimeStampUtc, HccApp.CurrentStore.Settings.TimeZone)
                            .ToShortTimeString();
                }

                if (litOrderNumber != null)
                {
                    litOrderNumber.Text = t.OrderNumber;
                }

                if (litDescription != null)
                {
                    var paymentMethod = PaymentMethods.Find(t.MethodId);
                    var methodName = paymentMethod != null ? paymentMethod.MethodName : string.Empty;
                    litDescription.Text = LocalizationUtils.GetActionType(t.Action, methodName);
                }

                if (litAmount != null)
                {
                    litAmount.Text = t.AmountAppliedToOrder.ToString("C");
                }

                if (litCustomerName != null)
                {
                    litCustomerName.Text = "<strong>" + t.TempCustomerName + "</strong><br /><span class=\"tiny\">" +
                                           t.TempCustomerEmail + "</span>";
                }
                TotalSub += t.TempEstimatedItemPortion;
                TotalDiscounts += t.TempEstimatedItemDiscount;
                TotalShip += t.TempEstimatedShippingPortion;
                TotalShipDiscounts += t.TempEstimatedShippingDiscount;
                TotalTax += t.TempEstimatedTaxPortion;
                TotalGrand += t.AmountAppliedToOrder;
            }
            else
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    e.Item.Cells[1].Text = Localization.GetString("Totals");

                    e.Item.Cells[2].Text = string.Format("{0:C}", TotalSub);
                    e.Item.Cells[3].Text = string.Format("{0:C}", TotalDiscounts);
                    e.Item.Cells[4].Text = string.Format("{0:C}", TotalShip);
                    e.Item.Cells[5].Text = string.Format("{0:C}", TotalShipDiscounts);
                    e.Item.Cells[6].Text = string.Format("{0:C}", TotalTax);
                    e.Item.Cells[7].Text = string.Format("{0:C}", TotalGrand);
                }
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
            TotalGrand = 0;
            TotalSub = 0;
            TotalShip = 0;
            TotalTax = 0;
            TotalDiscounts = 0;
            TotalShipDiscounts = 0;

            var selectedDate = ucDatePickerNav.SelectedDate;
            var startDateUtc = ConvertStartDateToUtc(selectedDate);
            var endDateUtc = ConvertEndDateToUtc(selectedDate);

            var totalItems = 0;
            var transactions = HccApp.OrderServices.Transactions.FindForReportByDateRange(startDateUtc, endDateUtc,
                HccApp.CurrentStore.Id, int.MaxValue, 1, ref totalItems);
            TotalCount = totalItems;

            if (TotalCount > 0)
            {
                ProcessOrderPortions(transactions);

                dgList.DataSource = transactions;
                dgList.DataBind();
            }

            ShowNoRecordsMessage(TotalCount == 0);
        }

        // Estimates the portion of other totals before report goes live
        private void ProcessOrderPortions(List<OrderTransaction> transactions)
        {
            if (transactions == null) return;

            var orderIds = (from t in transactions select t.OrderId).Distinct().ToList();
            var orderSnaps = HccApp.OrderServices.Orders.FindManySnapshots(orderIds);

            foreach (var t in transactions)
            {
                var snap = orderSnaps.FirstOrDefault(y => y.bvin == t.OrderId);
                if (snap != null)
                {
                    decimal percentOfTotal = 0;
                    if (snap.TotalGrand > 0) percentOfTotal = t.AmountAppliedToOrder/snap.TotalGrand;

                    t.TempEstimatedHandlingPortion = Math.Round(snap.TotalHandling*percentOfTotal, 2);
                    t.TempEstimatedItemPortion = Math.Round(snap.TotalOrderBeforeDiscounts*percentOfTotal, 2);
                    t.TempEstimatedItemDiscount = Math.Round(snap.TotalOrderDiscounts*percentOfTotal, 2);
                    t.TempEstimatedShippingPortion = Math.Round(snap.TotalShippingBeforeDiscounts*percentOfTotal, 2);
                    t.TempEstimatedShippingDiscount = Math.Round(snap.TotalShippingDiscounts*percentOfTotal, 2);
                    t.TempEstimatedTaxPortion = Math.Round(snap.TotalTax*percentOfTotal, 2);
                    t.TempCustomerEmail = snap.UserEmail;
                    t.TempCustomerName = snap.BillingAddress.LastName + ", " + snap.BillingAddress.FirstName;
                }
            }
        }

        #endregion
    }
}