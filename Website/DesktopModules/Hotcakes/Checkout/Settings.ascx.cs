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
using DotNetNuke.Entities.Modules;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Dnn.Web;
using System.Web.UI.WebControls;

namespace Hotcakes.Modules.Checkout
{
    public partial class Settings : HotcakesSettingsBase
    {
        #region Implementation

        /// <summary>
        ///     Fill dropdown lists
        /// </summary>
        private void FillForm()
        {
            var checkoutViews = DnnPathHelper.GetViewNames("Checkout");
            var payPalCheckoutViews = DnnPathHelper.GetViewNames("PayPalExpressCheckout");
            var notSetText = LocalizeString("NoneSelectedText");

            ViewComboBox.Items.Add(new ListItem(notSetText, string.Empty));
            ViewComboBox.AppendDataBoundItems = true;
            ViewComboBox.DataSource = checkoutViews;
            ViewComboBox.DataBind();

            ReceiptViewComboBox.Items.Add(new ListItem(notSetText, string.Empty));
            ReceiptViewComboBox.AppendDataBoundItems = true;
            ReceiptViewComboBox.DataSource = checkoutViews;
            ReceiptViewComboBox.DataBind();

            PaymentErrorViewComboBox.Items.Add(new ListItem(notSetText, string.Empty));
            PaymentErrorViewComboBox.AppendDataBoundItems = true;
            PaymentErrorViewComboBox.DataSource = checkoutViews;
            PaymentErrorViewComboBox.DataBind();

            PayPalViewComboBox.Items.Add(new ListItem(notSetText, string.Empty));
            PayPalViewComboBox.AppendDataBoundItems = true;
            PayPalViewComboBox.DataSource = payPalCheckoutViews;
            PayPalViewComboBox.DataBind();
        }

        #endregion

        #region ModuleSettingsBase overrides

        public override void LoadSettings()
        {
            if (!IsPostBack)
            {
                FillForm();

                var viewText = Convert.ToString(ModuleSettings["View"]);
                var receiptViewText = Convert.ToString(ModuleSettings["ReceiptView"]);
                var paymentErrorViewText = Convert.ToString(ModuleSettings["PaymentErrorView"]);
                var payPalViewText = Convert.ToString(ModuleSettings["PayPalView"]);

                ViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(viewText))
                {
                    ViewComboBox.SelectedValue = viewText;
                    ViewContentLabel.Text = viewText;
                }

                ReceiptViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(receiptViewText))
                {
                    ReceiptViewComboBox.SelectedValue = receiptViewText;
                    ReceiptViewContentLabel.Text = receiptViewText;
                }

                PaymentErrorViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(paymentErrorViewText))
                {
                    PaymentErrorViewComboBox.SelectedValue = paymentErrorViewText;
                    PaymentErrorViewContentLabel.Text = paymentErrorViewText;
                }

                PayPalViewContentLabel.Text = LocalizeString("NoneSelectedText");
                if (!string.IsNullOrEmpty(payPalViewText))
                {
                    PayPalViewComboBox.SelectedValue = payPalViewText;
                    PayPalViewContentLabel.Text = payPalViewText;
                }
            }
        }

        public override void UpdateSettings()
        {
            var controller = new ModuleController();
            controller.UpdateModuleSetting(ModuleId, "View", ViewComboBox.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "ReceiptView", ReceiptViewComboBox.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "PaymentErrorView", PaymentErrorViewComboBox.SelectedValue);
            controller.UpdateModuleSetting(ModuleId, "PayPalView", PayPalViewComboBox.SelectedValue);
        }

        #endregion
    }
}