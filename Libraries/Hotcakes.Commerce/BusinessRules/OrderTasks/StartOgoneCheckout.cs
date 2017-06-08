#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2017 Hotcakes Commerce, LLC
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
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Payment.Methods;
using Hotcakes.Commerce.Urls;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class StartOgoneCheckout : ThirdPartyCheckoutOrderTask
    {
        // Since user can always enter come unicode characters
        // and because some payment method names contains unicode chars ( e.g. 'Sofort Überweisung' )
        // we are using UTF8 urls only
        private const string ProductionUrl = "https://secure.ogone.com/ncol/prod/orderstandard.asp";
        private const string DevelopmentUrl = "https://secure.ogone.com/ncol/test/orderstandard.asp";
        private const string ProductionUTF8Url = "https://secure.ogone.com/ncol/prod/orderstandard_utf8.asp";
        private const string DevelopmentUTF8Url = "https://secure.ogone.com/ncol/test/orderstandard_utf8.asp";

        public override string PaymentMethodId
        {
            get { return PaymentMethods.OgoneId; }
        }

        public override bool ProcessCheckout(OrderTaskContext context)
        {
            if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
            {
                try
                {
                    var settings = new OgoneSettings();
                    var methodSettings = context.HccApp.CurrentStore.Settings.MethodSettingsGet(PaymentMethodId);
                    settings.Merge(methodSettings);

                    var order = context.Order;

                    var decimalAmount = order.TotalGrandAfterStoreCredits(context.HccApp.OrderServices);
                    var intAmount = (int) (decimalAmount * 100);

                    var parameters = new NameValueCollection();
                    parameters.Add("PSPID", settings.PaymentServiceProviderId);
                    // We can't use order.bvin here because Ogone won't allow us try to pay failed order again
                    parameters.Add("ORDERID", "order" + DateTime.UtcNow.Ticks);
                    parameters.Add("AMOUNT", intAmount.ToString());
                    var regionInfo = new RegionInfo(context.HccApp.CurrentStore.Settings.CurrencyCultureCode);
                    parameters.Add("CURRENCY", regionInfo.ISOCurrencySymbol);
                    var language = CultureInfo.CurrentCulture.Name.Replace("-", "_");
                    parameters.Add("LANGUAGE", language);
                    parameters.Add("EMAIL", order.UserEmail);

                    var address = order.BillingAddress;
                    parameters.Add("CN", string.Join(" ", address.FirstName, address.LastName));
                    parameters.Add("OWNERADDRESS", string.Join(", ", address.Line2, address.Line1));
                    parameters.Add("OWNERZIP", address.PostalCode);
                    parameters.Add("OWNERTOWN", address.City);
                    parameters.Add("OWNERCTY", address.CountryDisplayName);
                    parameters.Add("OWNERTELNO", address.Phone);

                    parameters.Add("OPERATION", "SAL");
                    if (!string.IsNullOrWhiteSpace(settings.TemplatePage))
                    {
                        parameters.Add("TP", settings.TemplatePage);
                    }

                    var returnUrl = HccUrlBuilder.RouteHccUrl(HccRoute.ThirdPartyPayment);
                    var cancelUrl = HccUrlBuilder.RouteHccUrl(HccRoute.Checkout);
                    parameters.Add("ACCEPTURL", returnUrl);
                    parameters.Add("DECLINEURL", returnUrl);
                    parameters.Add("EXCEPTIONURL", returnUrl);
                    parameters.Add("CANCELURL", cancelUrl);

                    var shaSign = OgoneUtils.CalculateShaHash(parameters, settings.HashAlgorithm,
                        settings.ShaInPassPhrase);
                    parameters.Add("SHASIGN", shaSign);

                    HttpContextBase httpContext = new HccHttpContextWrapper(HttpContext.Current);

                    var url = settings.DeveloperMode ? DevelopmentUTF8Url : ProductionUTF8Url;
                    var urlParams = Url.BuldQueryString(parameters);

                    if (settings.DebugMode)
                    {
                        EventLog.LogEvent("Ogone Checkout", urlParams, EventLogSeverity.Debug);
                    }

                    var urlTemplate = "{0}?{1}";
                    httpContext.Response.Redirect(string.Format(urlTemplate, url, urlParams), true);
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent("Ogone Checkout", "Exception occurred during call to Ogone: " + ex,
                        EventLogSeverity.Error);
                    context.Errors.Add(new WorkflowMessage("Ogone Checkout Error",
                        GlobalLocalization.GetString("OgoneCheckoutError"), true));
                    return false;
                }
            }

            return false;
        }

        public override Task Clone()
        {
            return new StartOgoneCheckout();
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "D2690A55-8AAE-4935-9696-75BFA9D8014D";
        }

        public override string TaskName()
        {
            return "Start Ogone Checkout";
        }

        #region Private

        #endregion
    }
}