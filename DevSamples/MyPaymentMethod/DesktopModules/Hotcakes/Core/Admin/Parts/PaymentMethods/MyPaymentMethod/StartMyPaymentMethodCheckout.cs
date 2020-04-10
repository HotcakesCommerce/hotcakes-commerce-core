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
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Urls;
using Hotcakes.Web.Logging;

namespace MyCompany.MyPaymentMethod
{
    /// <summary>
    ///     This task is used to start processing of order poayment with custom payment method
    /// </summary>
    public class StartMyPaymentMethodCheckout : ThirdPartyCheckoutOrderTask
    {
        public override string PaymentMethodId
        {
            get { return MyPaymentMethod.Id(); }
        }

        public override bool ProcessCheckout(OrderTaskContext context)
        {
            if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
            {
                try
                {
                    var settings = new MyPaymentMethodSettings();
                    var methodSettings = context.HccApp.CurrentStore.Settings.MethodSettingsGet(PaymentMethodId);
                    settings.Merge(methodSettings);

                    // Here you can do custom processing of your payment.

                    // It can be direct post to payment service or redirection to hosted payment page
                    // In either case you have to end up on HccUrlBuilder.RouteHccUrl(HccRoute.ThirdPartyPayment) page
                    // So either you have to do such redirect here on your own
                    // or make sure that third party hosted pay page will make it in case of successfull or failed payment

                    HttpContextBase httpContext = new HccHttpContextWrapper(HttpContext.Current);
                    httpContext.Response.Redirect(HccUrlBuilder.RouteHccUrl(HccRoute.ThirdPartyPayment));
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent("My Custom Checkout", "Exception occurred during call to Moneris: " + ex,
                        EventLogSeverity.Error);
                    context.Errors.Add(new WorkflowMessage("My Custom Checkout Error",
                        GlobalLocalization.GetString("MonerisCheckoutError"), true));
                    return false;
                }
            }

            return false;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override Task Clone()
        {
            return new StartMyPaymentMethodCheckout();
        }

        public override string TaskId()
        {
            return "E9B1A204-7C61-4664-A043-71BF43E24259";
        }

        public override string TaskName()
        {
            return "Start My Custom Checkout";
        }
    }
}