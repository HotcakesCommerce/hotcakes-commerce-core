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
using System.Globalization;
using System.Linq;
using System.Threading;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class RecurringPayments : HccUserControl
    {
        public TransactionEventDelegate TransactionEvent;
        public Order CurrentOrder { get; set; }
        private OrderPaymentManager PayManager { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            PayManager = new OrderPaymentManager(CurrentOrder, HccApp);

            LoadItemsList();
        }


        protected void lnkCC_Click(object sender, EventArgs e)
        {
        }

        protected void lnkUpdateCreditCard_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var cardData = ucCreditCardInput.GetCardData();

                var allSucceeded = true;
                foreach (var li in CurrentOrder.Items)
                {
                    if (!li.RecurringBilling.IsCancelled)
                    {
                        var result = PayManager.RecurringSubscriptionUpdate(li.Id, cardData);

                        allSucceeded &= result.Succeeded;

                        if (!result.Succeeded)
                        {
                            ucMessageBox.ShowError(string.Format("Subscription '{0}' update failed.", li.ProductName));
                        }
                    }
                }

                if (allSucceeded)
                    ucMessageBox.ShowOk("Subsciption credit card data successfully updated");

                TransactionEvent();
            }
        }

        protected void lnkRegisterPayment_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var lineItemId = long.Parse(ddlItems.SelectedValue);
                var lineItem = CurrentOrder.Items.FirstOrDefault(li => li.Id == lineItemId);

                var amount = ParseMoney(txtAmount.Text.Trim());
                if (amount == 0)
                    amount = lineItem.LineTotal;

                var subscriptionId = PayManager.GetSubscriptionByLineItem(lineItemId);
                var transaction = new Transaction
                {
                    Amount = amount,
                    Action = ActionType.RecurringPayment,
                    Customer = new CustomerData
                    {
                        FirstName = CurrentOrder.BillingAddress.FirstName,
                        LastName = CurrentOrder.BillingAddress.LastName,
                        Email = CurrentOrder.UserEmail,
                        Company = CurrentOrder.BillingAddress.Company,
                        Street = CurrentOrder.BillingAddress.Street,
                        City = CurrentOrder.BillingAddress.City,
                        RegionName = CurrentOrder.BillingAddress.RegionSystemName,
                        PostalCode = CurrentOrder.BillingAddress.PostalCode,
                        CountryName = CurrentOrder.BillingAddress.CountrySystemName,
                        Phone = CurrentOrder.BillingAddress.Phone
                    },
                    Result = new ResultData
                    {
                        Succeeded = true,
                        ReferenceNumber = subscriptionId
                    }
                };

                PayManager.RecurringPaymentReceive(transaction);

                ucMessageBox.ShowOk("Recurring payment successfully registred");

                TransactionEvent();
            }
        }


        private void LoadItemsList()
        {
            ddlItems.DataValueField = "Id";
            ddlItems.DataTextField = "ProductName";
            ddlItems.DataSource = CurrentOrder.Items;
            ddlItems.DataBind();
        }

        private decimal ParseMoney(string input)
        {
            decimal val = 0;
            decimal.TryParse(input, NumberStyles.Currency, Thread.CurrentThread.CurrentUICulture, out val);
            return Money.RoundCurrency(val);
        }
    }
}