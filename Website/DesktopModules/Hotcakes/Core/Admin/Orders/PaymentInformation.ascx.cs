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
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class PaymentInformation : HccUserControl
    {
        public Order CurrentOrder { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (CurrentOrder != null)
            {
                var paySummary = HccApp.OrderServices.PaymentSummary(CurrentOrder);

                lblPaymentSummary.Text = paySummary.PaymentsSummary;
                if (!CurrentOrder.IsRecurring)
                {
                    lblPaymentAuthorized.Text = paySummary.AmountAuthorized.ToString("C");
                    lblPaymentCharged.Text = paySummary.AmountCharged.ToString("C");
                    lblPaymentRefunded.Text = paySummary.AmountRefunded.ToString("C");
                    lblReturnedItems.Text = paySummary.AmountReturned.ToString("C");
                }
                else
                {
                    lblPaymentAuthorized.Text = Localization.GetString("NotApplicable");
                    lblPaymentCharged.Text = Localization.GetString("NotApplicable");
                    lblPaymentRefunded.Text = Localization.GetString("NotApplicable");
                    lblReturnedItems.Text = Localization.GetString("NotApplicable");
                }
                lblPaymentDue.Text = paySummary.AmountDue.ToString("C");
            }
            else
            {
                var val = 0m;
                lblPaymentAuthorized.Text = val.ToString("C");
                lblPaymentCharged.Text = val.ToString("C");
                lblPaymentRefunded.Text = val.ToString("C");
                lblReturnedItems.Text = val.ToString("C");
                lblPaymentDue.Text = val.ToString("C");
            }
        }
    }
}