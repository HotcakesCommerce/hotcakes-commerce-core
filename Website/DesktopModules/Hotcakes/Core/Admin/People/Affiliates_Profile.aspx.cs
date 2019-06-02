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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    public partial class Affiliates_Profile : BaseAdminPage
    {
        #region Properties

        protected Affiliate Affiliate { get; set; }
        protected AffiliateReportTotals Totals { get; set; }
        protected AffiliateReportTotals TotalsByMonth { get; set; }

        protected long AffiliateId
        {
            get { return Request.QueryString["id"].ConvertTo<long>(-1); }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            PageTitle = Localization.GetString("AffiliateProfile");
            CurrentTab = AdminTabType.Reports;
            ValidateCurrentUserHasPermission(SystemPermissions.ReportsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ucPaymentsDateRange.RangeTypeChanged += (s, a) =>
            {
                ucPaymentsPager.ResetPageNumber();
                LoadPayments();
            };
            ucPaymentsPager.PageChanged += (s, a) => LoadPayments();
            ucOrdersDateRange.RangeTypeChanged += (s, a) =>
            {
                ucOrdersPager.ResetPageNumber();
                LoadOrders();
            };
            ucOrdersPager.PageChanged += (s, a) => LoadOrders();
            ucReferralPager.PageChanged += (s, a) => LoadReferrals();
            btnFind.Click += (s, a) =>
            {
                ucReferralPager.ResetPageNumber();
                LoadReferrals();
            };
            lnkEditProfile.NavigateUrl = "Affiliates_Edit.aspx?id=" + AffiliateId;
            lnkAddPayment.Click += lnkAddPayment_Click;
            ucPaymentDialog.AddPaymentAction = payment =>
            {
                if (HccApp.ContactServices.AffiliatePayments.Create(payment))
                {
                    ucMessageBox.ShowOk(Localization.GetString("NewPaymentAdded"));
                    LoadPayments();
                }
            };
            ucPaymentDialog.UpdatePaymentAction = payment =>
            {
                if (HccApp.ContactServices.AffiliatePayments.Update(payment))
                {
                    ucMessageBox.ShowOk(Localization.GetString("PaymentUpdated"));
                    LoadPayments();
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadAffiliateDetails();

            if (!IsPostBack)
            {
                LoadPayments();
                LoadOrders();
                LoadReferrals();
            }
        }

        private void lnkAddPayment_Click(object sender, EventArgs e)
        {
            ucPaymentDialog.ShowDialogForAffiliateId(AffiliateId);
        }

        protected void gvPayments_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Amount");
                e.Row.Cells[1].Text = Localization.GetString("Date");
                e.Row.Cells[2].Text = Localization.GetString("Memo");
                e.Row.Cells[3].Text = Localization.GetString("Attachment");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {
                    var link = e.Row.Cells[3].Controls[1] as HyperLink;
                    if (link != null)
                    {
                        link.Text = Localization.GetString("Edit");
                    }
                }
                catch
                {
                    // do nothing
                }
            }
        }

        protected void gvOrders_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("GrandTotal");
                e.Row.Cells[1].Text = Localization.GetString("OrderNumber");
                e.Row.Cells[2].Text = Localization.GetString("Commission");
                e.Row.Cells[3].Text = Localization.GetString("Date");
            }
        }

        protected void gvReferrals_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Name");
                e.Row.Cells[1].Text = Localization.GetString("ID");
                e.Row.Cells[2].Text = Localization.GetString("Email");
                e.Row.Cells[3].Text = Localization.GetString("TotalRevenue");
            }
        }

        #endregion

        #region Implementation

        private void LoadAffiliateDetails()
        {
            var aff = HccApp.ContactServices.Affiliates.Find(AffiliateId);
            PageTitle = aff.Address.FirstName + " " + aff.Address.LastName;
            Affiliate = aff;


            var thisMonth = new DateRange {RangeType = DateRangeType.ThisMonth};
            var criteria = new AffiliateReportCriteria
            {
                StartDateUtc = thisMonth.StartDate,
                EndDateUtc = thisMonth.EndDate
            };
            TotalsByMonth = HccApp.ContactServices.Affiliates.GetAffiliateTotals(aff.Id, criteria);
            Totals = HccApp.ContactServices.Affiliates.GetAffiliateTotals(aff.Id, new AffiliateReportCriteria());
        }

        private void LoadPayments()
        {
            var startUtc = ucPaymentsDateRange.GetStartDateUtc(HccApp);
            var endUtc = ucPaymentsDateRange.GetEndDateUtc(HccApp);
            int totalRowCount;
            var payments = HccApp.ContactServices.AffiliatePayments.FindAllPaged(AffiliateId, startUtc, endUtc,
                ucPaymentsPager.PageNumber, ucPaymentsPager.PageSize, out totalRowCount);
            ucPaymentsPager.SetRowCount(totalRowCount);

            payments.ForEach(p => { p.PaymentDateUtc = DateHelper.ConvertUtcToStoreTime(HccApp, p.PaymentDateUtc); });

            gvPayments.DataSource = payments;
            gvPayments.DataBind();
        }

        private void LoadOrders()
        {
            var startUtc = ucOrdersDateRange.GetStartDateUtc(HccApp);
            var endUtc = ucOrdersDateRange.GetEndDateUtc(HccApp);
            var criteria = new OrderSearchCriteria
            {
                AffiliateId = AffiliateId,
                StartDateUtc = startUtc,
                EndDateUtc = endUtc
            };
            var totalRowCount = 0;
            var orders = HccApp.OrderServices.Orders.FindByCriteriaPaged(criteria, ucOrdersPager.PageNumber,
                ucOrdersPager.PageSize, ref totalRowCount);
            ucOrdersPager.SetRowCount(totalRowCount);

            orders.ForEach(o => { o.TimeOfOrderUtc = DateHelper.ConvertUtcToStoreTime(HccApp, o.TimeOfOrderUtc); });

            gvOrders.DataSource = orders;
            gvOrders.DataBind();
        }

        protected string GetCommission(IDataItemContainer cont)
        {
            var o = cont.DataItem as OrderSnapshot;

            if (o != null)
            {
                var comm = Affiliate.CommissionType == AffiliateCommissionType.FlatRateCommission
                    ? Affiliate.CommissionAmount
                    : Math.Round(Affiliate.CommissionAmount/100*o.TotalOrderAfterDiscounts, 2);

                return comm.ToString("c");
            }
            return string.Empty;
        }

        protected string GetAttachmentUrl(IDataItemContainer cont)
        {
            var p = cont.DataItem as AffiliatePayment;

            if (p != null)
            {
                return DiskStorage.PaymentsAttachmentUrl(HccApp.CurrentStore.Id, p.FileName);
            }

            return string.Empty;
        }

        protected bool ShowAttachmentUrl(IDataItemContainer cont)
        {
            var p = cont.DataItem as AffiliatePayment;

            if (p != null)
            {
                return !string.IsNullOrEmpty(p.FileName);
            }

            return false;
        }

        private void LoadReferrals()
        {
            var criteria = new AffiliateReportCriteria
            {
                SearchBy = (AffiliateReportCriteria.SearchType) ddlSearchBy.SelectedValue.ConvertTo<int>(),
                SearchText = txtSearchText.Text,
                ReferralAffiliateID = Affiliate.AffiliateId
            };
            var totalRowCount = 0;
            var referrals = HccApp.ContactServices.Affiliates.FindAllWithFilter(criteria, ucReferralPager.PageNumber,
                ucReferralPager.PageSize, ref totalRowCount);
            ucReferralPager.SetRowCount(totalRowCount);

            gvReferrals.DataSource = referrals;
            gvReferrals.DataBind();
        }

        #endregion
    }
}