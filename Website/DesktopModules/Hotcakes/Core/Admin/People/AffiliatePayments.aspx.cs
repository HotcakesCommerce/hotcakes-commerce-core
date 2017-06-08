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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class AffiliatePayments : BaseAdminPage
    {
        #region Properties

        protected int AffiliatePaymentsCount
        {
            get { return ViewState["AffiliatePaymentsCount"].ConvertTo(0); }
            set { ViewState["AffiliatePaymentsCount"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("AffiliatePayments");
            CurrentTab = AdminTabType.Reports;
            ValidateCurrentUserHasPermission(SystemPermissions.ReportsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ucPager.PageChanged += (s, a) => BindAffiliatePayments();
            ucDateRangePicker.RangeTypeChanged += (s, a) => BindAffiliatePayments();
            btnFind.Click += (s, a) => BindAffiliatePayments();
            gvAffiliatePayments.RowDeleting += gvAffiliates_RowDeleting;
            gvAffiliatePayments.RowCommand += gvAffiliatePayments_RowCommand;
            lnkAddPayment.Click += lnkAddPayment_Click;

            ucPaymentDialog.AddPaymentAction = payment =>
            {
                if (HccApp.ContactServices.AffiliatePayments.Create(payment))
                {
                    ucMessageBox.ShowOk(Localization.GetString("NewPaymentAdded"));
                }
            };

            ucPaymentDialog.UpdatePaymentAction = payment =>
            {
                if (HccApp.ContactServices.AffiliatePayments.Update(payment))
                {
                    ucMessageBox.ShowOk(Localization.GetString("PaymentUpdated"));
                }
            };
        }

        private void gvAffiliatePayments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditPaymant")
            {
                var id = Convert.ToInt64(e.CommandArgument);
                ucPaymentDialog.ShowDialog(id);
            }
        }

        private void lnkAddPayment_Click(object sender, EventArgs e)
        {
            ucPaymentDialog.ShowDialog(null);
        }

        private void gvAffiliates_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = (long) e.Keys[0];

            HccApp.ContactServices.AffiliatePayments.Delete(id);

            BindAffiliatePayments();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindAffiliatePayments();

            base.OnPreRender(e);
        }

        protected void gvAffiliatePayments_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("PaymentID");
                e.Row.Cells[1].Text = Localization.GetString("AffiliateID");
                e.Row.Cells[2].Text = Localization.GetString("Date");
                e.Row.Cells[3].Text = Localization.GetString("Amount");
                e.Row.Cells[4].Text = Localization.GetString("Memo");
            }
        }

        protected void EditPayment_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void DeletePayment_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("DeleteConfirm"));
        }

        protected void Attachment_OnPreRender(object sender, EventArgs e)
        {
            var link = (HyperLink) sender;
            link.Text = Localization.GetString("Attachment");
        }

        #endregion

        #region Implementation

        protected string GetAttachmentUrl(IDataItemContainer cont)
        {
            var p = cont.DataItem as AffiliatePaymentReportData;

            if (p != null)
            {
                return DiskStorage.PaymentsAttachmentUrl(HccApp.CurrentStore.Id, p.FileName);
            }

            return string.Empty;
        }

        protected bool ShowAttachmentUrl(IDataItemContainer cont)
        {
            var p = cont.DataItem as AffiliatePaymentReportData;

            if (p != null)
            {
                return !string.IsNullOrEmpty(p.FileName);
            }

            return false;
        }

        private void BindAffiliatePayments()
        {
            var repository = HccApp.ContactServices.AffiliatePayments;
            var rowCount = 0;
            var criteria = GetCriteria();

            var payments = repository.FindAllWithFilter(criteria, ucPager.PageNumber, ucPager.PageSize, out rowCount);

            payments.ForEach(p => { p.PaymentDateUtc = DateHelper.ConvertUtcToStoreTime(HccApp, p.PaymentDateUtc); });

            gvAffiliatePayments.DataSource = payments;
            gvAffiliatePayments.DataBind();

            ucPager.SetRowCount(rowCount);
            AffiliatePaymentsCount = rowCount;
        }

        private AffiliatePaymentReportCriteria GetCriteria()
        {
            var utcStart = ucDateRangePicker.GetStartDateUtc(HccApp);
            var utcEnd = ucDateRangePicker.GetEndDateUtc(HccApp);

            var criteria = new AffiliatePaymentReportCriteria
            {
                StartDateUtc = utcStart,
                EndDateUtc = utcEnd,
                SearchBy = (AffiliatePaymentReportCriteria.SearchType) ddlSearchBy.SelectedValue.ConvertTo<int>(),
                SearchText = txtSearchText.Text
            };

            return criteria;
        }

        #endregion
    }
}