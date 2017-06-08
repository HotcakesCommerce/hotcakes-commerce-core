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
using System.Web.UI;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Common.Dnn;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class AffiliatePaymentDialog : UserControl
    {
        #region Properties

        public Action CancelAction { get; set; }
        public Action<AffiliatePayment> AddPaymentAction { get; set; }
        public Action<List<AffiliatePayment>> AddMultiplePaymentsAction { get; set; }
        public Action<AffiliatePayment> UpdatePaymentAction { get; set; }

        protected bool ShowPaymentEditor
        {
            get { return ViewState["ShowPaymentEditor"].ConvertTo(false); }
            set { ViewState["ShowPaymentEditor"] = value; }
        }

        #endregion

        #region Public method

        public void ShowDialog(long? paymentId)
        {
            ucPaymentEditor.PaymentId = paymentId;
            ShowPaymentEditor = true;
        }

        public void ShowDialogForAffiliateId(long affiliateId)
        {
            ucPaymentEditor.AffiliateIDs = new List<long> {affiliateId};
            ucPaymentEditor.PaymentId = null;
            ShowPaymentEditor = true;
        }

        public void ShowDialogForSelection(List<long> affiliateIds)
        {
            ucPaymentEditor.AffiliateIDs = affiliateIds;
            ucPaymentEditor.PaymentId = null;
            ShowPaymentEditor = true;
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ucPaymentEditor.SaveAction += ucPaymentEditor_SaveAction;
            ucPaymentEditor.CancelAction += ucPaymentEditor_CancelAction;
        }

        private void ucPaymentEditor_CancelAction(object sender, EventArgs e)
        {
            ShowPaymentEditor = false;
        }

        private void ucPaymentEditor_SaveAction(object sender, EventArgs e)
        {
            SavePayments(ucPaymentEditor.Payments);
            ShowPaymentEditor = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ShowPaymentDialog();

            base.OnPreRender(e);
        }

        #endregion

        #region Implementation

        protected string GetDialogTitle()
        {
            if (ucPaymentEditor.AffiliateIDs.Count > 1)
            {
                return "Add Multiple Payments";
            }
            return ucPaymentEditor.PaymentId.HasValue ? "Edit Payment" : "Add New Payment";
        }

        private void SavePayments(List<AffiliatePayment> payments)
        {
            if (payments.Count == 1)
            {
                var payment = payments.First();

                if (payment.PaymentAmount > 0)
                {
                    if (ucPaymentEditor.PaymentId.HasValue)
                    {
                        if (UpdatePaymentAction != null)
                            UpdatePaymentAction(payment);
                    }
                    else
                    {
                        if (AddPaymentAction != null)
                            AddPaymentAction(payment);
                    }
                }
            }
            else
            {
                if (AddMultiplePaymentsAction != null)
                    AddMultiplePaymentsAction(payments);
            }
        }

        private void ShowPaymentDialog()
        {
            pnlAddPayment.Visible = ShowPaymentEditor;

            if (ShowPaymentEditor)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hcAddPaymentDialog", "hcAddPaymentDialog();", true);
            }
        }

        #endregion
    }
}