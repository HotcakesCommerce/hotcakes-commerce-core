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
using System.Web.UI;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class AffiliatePaymentEditor : HccUserControl
    {
        #region Properties

        public List<AffiliatePayment> Payments { get; set; }
        public event EventHandler SaveAction;
        public event EventHandler CancelAction;

        public Control CancelButton
        {
            get { return btnCancel; }
        }

        public List<long> AffiliateIDs
        {
            get { return ViewState["AffiliateIDs"] as List<long> ?? new List<long>(); }
            set { ViewState["AffiliateIDs"] = value; }
        }

        public long? PaymentId
        {
            get { return (long?) ViewState["PaymentId"]; }
            set { ViewState["PaymentId"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (CancelAction != null)
                CancelAction(this, EventArgs.Empty);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (SavePayments())
                {
                    if (SaveAction != null)
                        SaveAction(this, EventArgs.Empty);
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            divAffiliateIDRow.Visible = AffiliateIDs.Count == 0;
            divAmountRow.Visible = AffiliateIDs.Count <= 1;
            BindPaymentData();
        }

        #endregion

        #region Implementation

        private void BindPaymentData()
        {
            if (PaymentId.HasValue)
            {
                var payment = HccApp.ContactServices.AffiliatePayments.Find(PaymentId.Value);
                var aff = HccApp.ContactServices.Affiliates.Find(payment.AffiliateId);

                txtAffiliateID.Text = aff.AffiliateId;
                txtAmount.Text = Money.FormatCurrency(payment.PaymentAmount);
                txtMemo.Text = payment.Notes;
            }
            else if (AffiliateIDs.Count == 1)
            {
                var affTotals = HccApp.ContactServices.Affiliates.GetAffiliateTotals(AffiliateIDs[0],
                    new AffiliateReportCriteria());
                var amount = affTotals.CommissionOwed > 0 ? affTotals.CommissionOwed : 0;
                txtAmount.Text = Money.FormatCurrency(amount);
            }
        }

        private bool SavePayments()
        {
            if (PaymentId.HasValue)
            {
                return UpdatePayment(PaymentId.Value);
            }
            if (AffiliateIDs.Count > 1)
            {
                return AddMultiplePayments();
            }
            return AddOnePayment();
        }

        private bool UpdatePayment(long paymentId)
        {
            var aff = HccApp.ContactServices.Affiliates.FindByAffiliateId(txtAffiliateID.Text);
            var payment = HccApp.ContactServices.AffiliatePayments.Find(paymentId);

            if (aff != null)
            {
                payment.AffiliateId = aff.Id;
                payment.PaymentAmount = decimal.Parse(txtAmount.Text);
                payment.PaymentDateUtc = DateTime.UtcNow;
                payment.Notes = txtMemo.Text;

                if (fuAttachment.HasFile)
                {
                    payment.FileName = DiskStorage.UploadPaymanentsAttachment(HccApp.CurrentStore.Id,
                        fuAttachment.PostedFile);
                }

                Payments = new List<AffiliatePayment> {payment};
                return true;
            }
            ucMessageBox.ShowError(Localization.GetString("InvalidAffiliateId"));
            return false;
        }

        private bool AddMultiplePayments()
        {
            Payments = new List<AffiliatePayment>();

            foreach (var affId in AffiliateIDs)
            {
                var affTotals = HccApp.ContactServices.Affiliates.GetAffiliateTotals(affId,
                    new AffiliateReportCriteria());
                var payment = new AffiliatePayment();
                payment.AffiliateId = affId;
                payment.PaymentAmount = affTotals.CommissionOwed;
                payment.PaymentDateUtc = DateTime.UtcNow;
                payment.Notes = txtMemo.Text;
                payment.FileName = DiskStorage.UploadPaymanentsAttachment(HccApp.CurrentStore.Id,
                    fuAttachment.PostedFile);

                Payments.Add(payment);
            }

            return true;
        }

        private bool AddOnePayment()
        {
            Affiliate aff = null;

            if (AffiliateIDs.Count == 1)
            {
                aff = HccApp.ContactServices.Affiliates.Find(AffiliateIDs[0]);
            }
            else
            {
                aff = HccApp.ContactServices.Affiliates.FindByAffiliateId(txtAffiliateID.Text);
            }

            if (aff == null)
            {
                ucMessageBox.ShowError(Localization.GetString("InvalidAffiliateId"));
                return false;
            }
            if (!aff.Approved)
            {
                ucMessageBox.ShowError(Localization.GetString("AffiliateIdNotVerified"));
                return false;
            }
            var payment = new AffiliatePayment
            {
                AffiliateId = aff.Id,
                PaymentAmount = decimal.Parse(txtAmount.Text),
                PaymentDateUtc = DateTime.UtcNow,
                Notes = txtMemo.Text,
                FileName = DiskStorage.UploadPaymanentsAttachment(HccApp.CurrentStore.Id, fuAttachment.PostedFile)
            };

            Payments = new List<AffiliatePayment> {payment};
            return true;
        }

        #endregion
    }
}