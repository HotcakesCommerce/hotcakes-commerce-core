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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Affiliates : BaseAdminPage
    {
        #region Fields

        protected AffiliateReportTotals Totals = new AffiliateReportTotals();

        #endregion

        #region Properties

        protected int AffiliatesCount
        {
            get { return ViewState["AffiliatesCount"].ConvertTo(0); }
            set { ViewState["AffiliatesCount"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("Affiliates");
            CurrentTab = AdminTabType.Reports;
            ValidateCurrentUserHasPermission(SystemPermissions.ReportsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gvAffiliates.RowDeleting += gvAffiliates_RowDeleting;
            lnkAddPayment.Click += lnkAddPayment_Click;

            ucPaymentDialog.AddPaymentAction = payment => { AddPayments(new List<AffiliatePayment> {payment}); };

            ucPaymentDialog.AddMultiplePaymentsAction = payments => { AddPayments(payments); };

            lnkExportToExcel.Click += lnkExportToExcel_Click;
            lnkApprove.Click += lnkApprove_Click;

            rblFilterMode.SelectedIndexChanged += (s, a) => ucPager.ResetPageNumber();
            btnFind.Click += (s, a) => ucPager.ResetPageNumber();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                lnkApprove.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ApproveConfirm"));
            }
        }

        private void gvAffiliates_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) e.Keys[0];

            HccApp.ContactServices.Affiliates.Delete(id);
        }

        private void lnkAddPayment_Click(object sender, EventArgs e)
        {
            ucPaymentDialog.ShowDialogForSelection(GetSelectedIDs());
        }

        private void lnkExportToExcel_Click(object sender, EventArgs e)
        {
            var criteria = GetCriteria();
            var total = 0;

            var reportData = HccApp.ContactServices.Affiliates.FindAllWithFilter(criteria, 1, int.MaxValue, ref total);
            var affiliatesExport = new AffiliatesExport();

            affiliatesExport.ExportToExcel(Response, "Hotcakes_Affiliates.xlsx", reportData);
        }

        private void lnkApprove_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedIDs();

            foreach (var affiliateId in ids)
            {
                var aff = HccApp.ContactServices.Affiliates.Find(affiliateId);

                if (!aff.Approved)
                {
                    aff.Approved = true;
                    var ui = DnnUserController.Instance.GetUser(DnnGlobal.Instance.GetPortalId(), aff.UserId);
                    var culture = ui.Profile["UsedCulture"] as string;

                    if (string.IsNullOrEmpty(culture))
                    {
                        culture = "en-US";
                    }

                    HccApp.ContactServices.Affiliates.Update(aff);
                    HccApp.ContactServices.AffiliateWasApproved(aff, culture);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            var onlyNonApproved = rblFilterMode.SelectedValue == "OnlyNonApproved";
            var onlyOwed = rblFilterMode.SelectedValue == "OnlyOwed";

            if (onlyOwed || onlyNonApproved)
            {
                txtSearchText.Enabled = false;
                ddlSearchBy.Enabled = false;
                ucDateRangePicker.RangeType = DateRangeType.AllDates;
                ucDateRangePicker.Enabled = false;
                btnFind.Visible = false;
            }
            else
            {
                txtSearchText.Enabled = true;
                ddlSearchBy.Enabled = true;
                ucDateRangePicker.Enabled = true;
                btnFind.Visible = true;
            }

            lnkApprove.Visible = onlyNonApproved;

            BindAffiliates();

            base.OnPreRender(e);
        }

        protected void gvAffiliates_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = Localization.GetString("Company");
                e.Row.Cells[2].Text = Localization.GetString("Affiliate");
                e.Row.Cells[3].Text = Localization.GetString("ID");
                e.Row.Cells[4].Text = Localization.GetString("Sales");
                e.Row.Cells[5].Text = Localization.GetString("Orders");
                e.Row.Cells[6].Text = Localization.GetString("Commission");
                e.Row.Cells[7].Text = Localization.GetString("Owed");
                e.Row.Cells[8].Text = Localization.GetString("Signups");
            }
        }

        protected void Delete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmDelete"));
        }

        #endregion

        #region Implementation

        private List<long> GetSelectedIDs()
        {
            var ids = new List<long>();

            foreach (GridViewRow row in gvAffiliates.Rows)
            {
                var chbItem = row.FindControl("chbItem") as CheckBox;

                if (chbItem.Checked)
                {
                    ids.Add((long) gvAffiliates.DataKeys[row.DataItemIndex].Value);
                }
            }

            return ids;
        }

        private void AddPayments(List<AffiliatePayment> payments)
        {
            var paymentWasAdded = false;

            foreach (var payment in payments)
            {
                if (payment.PaymentAmount > 0)
                {
                    if (HccApp.ContactServices.AffiliatePayments.Create(payment))
                    {
                        paymentWasAdded = true;
                    }
                }
            }

            if (paymentWasAdded)
            {
                ucMessageBox.ShowOk(Localization.GetString("NewPaymentsAdded"));
            }
        }

        private void BindAffiliates()
        {
            var repository = HccApp.ContactServices.Affiliates;
            var rowCount = 0;
            var criteria = GetCriteria();

            Totals = repository.GetTotalsByFilter(criteria);

            gvAffiliates.DataSource = repository.FindAllWithFilter(criteria, ucPager.PageNumber, ucPager.PageSize,
                ref rowCount);
            gvAffiliates.DataBind();

            ucPager.SetRowCount(rowCount);
            AffiliatesCount = rowCount;
        }

        private AffiliateReportCriteria GetCriteria()
        {
            var utcStart = ucDateRangePicker.GetStartDateUtc(HccApp);
            var utcEnd = ucDateRangePicker.GetEndDateUtc(HccApp);
            var onlyNonApproved = rblFilterMode.SelectedValue == "OnlyNonApproved";
            var onlyOwed = rblFilterMode.SelectedValue == "OnlyOwed";

            AffiliateReportCriteria criteria;

            if (onlyNonApproved)
            {
                criteria = new AffiliateReportCriteria
                {
                    ShowOnlyNonApproved = true,
                    SortBy = (AffiliateReportCriteria.SortingType) ddlSortBy.SelectedValue.ConvertTo<int>()
                };
            }
            else if (onlyOwed)
            {
                criteria = new AffiliateReportCriteria
                {
                    ShowCommissionOwed = true,
                    SortBy = (AffiliateReportCriteria.SortingType) ddlSortBy.SelectedValue.ConvertTo<int>()
                };
            }
            else
            {
                criteria = new AffiliateReportCriteria
                {
                    StartDateUtc = utcStart,
                    EndDateUtc = utcEnd,
                    SortBy = (AffiliateReportCriteria.SortingType) ddlSortBy.SelectedValue.ConvertTo<int>(),
                    SearchBy = (AffiliateReportCriteria.SearchType) ddlSearchBy.SelectedValue.ConvertTo<int>(),
                    SearchText = txtSearchText.Text,
                    ShowCommissionOwed = false,
                    ShowOnlyNonApproved = false
                };
            }

            return criteria;
        }

        #endregion
    }
}