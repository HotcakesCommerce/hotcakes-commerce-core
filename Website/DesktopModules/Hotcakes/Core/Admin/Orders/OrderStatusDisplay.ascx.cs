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
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class OrderStatusDisplay : HccUserControl
    {
        public Order CurrentOrder { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                lstStatus.DataSource = OrderStatusCode.FindAll();
                lstStatus.DataValueField = "Bvin";
                lstStatus.DataTextField = "StatusDisplayName";
                lstStatus.DataBind();
            }
        }

        protected void lstStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentOrder != null)
            {
                var prevStatusCode = CurrentOrder.StatusCode;
                var newStatusCode = OrderStatusCode.FindByBvin(lstStatus.SelectedValue);

                CurrentOrder.StatusCode = newStatusCode.Bvin;
                CurrentOrder.StatusName = newStatusCode.StatusName;

                HccApp.OrderServices.Orders.Update(CurrentOrder);
                HccApp.OrderServices.OrderStatusChanged(CurrentOrder, prevStatusCode);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (CurrentOrder != null)
            {
                litPaymentStatus.Text = LocalizationUtils.GetOrderPaymentStatus(CurrentOrder.PaymentStatus);
                litShippingStatus.Text = LocalizationUtils.GetOrderShippingStatus(CurrentOrder.ShippingStatus);

                if (lstStatus.Items.FindByValue(CurrentOrder.StatusCode) != null)
                {
                    lstStatus.ClearSelection();
                    lstStatus.Items.FindByValue(CurrentOrder.StatusCode).Selected = true;
                }
            }
        }
    }
}