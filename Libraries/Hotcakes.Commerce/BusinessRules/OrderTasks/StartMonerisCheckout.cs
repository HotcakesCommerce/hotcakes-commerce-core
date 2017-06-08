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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Payment.Methods;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class StartMonerisCheckout : ThirdPartyCheckoutOrderTask
    {
        public const string DevelopmentUrl = "https://esqa.moneris.com/HPPDP/index.php";
        public const string ProductionUrl = "https://www3.moneris.com/HPPDP/index.php";

        public override string PaymentMethodId
        {
            get { return PaymentMethods.MonerisId; }
        }

        public override bool ProcessCheckout(OrderTaskContext context)
        {
            if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
            {
                try
                {
                    var settings = new MonerisSettings();
                    var methodSettings = context.HccApp.CurrentStore.Settings.MethodSettingsGet(PaymentMethodId);
                    settings.Merge(methodSettings);

                    var order = context.Order;
                    var amount = order.TotalGrandAfterStoreCredits(context.HccApp.OrderServices);

                    var parameters = new NameValueCollection();
                    parameters.Add("ps_store_id", settings.HostedPayPageId);
                    parameters.Add("hpp_key", settings.HostedPayPageToken);
                    parameters.Add("hpp_preload", "");
                    parameters.Add("charge_total", amount.ToString("F2", CultureInfo.InvariantCulture));

                    parameters.Add("lang", CultureInfo.CurrentCulture.Name);
                    parameters.Add("hst", order.TotalTax.ToString("F2", CultureInfo.InvariantCulture));
                    parameters.Add("shipping_cost",
                        order.TotalShippingAfterDiscounts.ToString("F2", CultureInfo.InvariantCulture));
                    parameters.Add("email", Text.TrimToLength(order.UserEmail, 50));
                    parameters.Add("note",
                        string.Format(CultureInfo.InvariantCulture, "Order discounts: {0:F2}", order.TotalOrderDiscounts));

                    parameters.Add("ship_first_name", Text.TrimToLength(order.ShippingAddress.FirstName, 30));
                    parameters.Add("ship_last_name", Text.TrimToLength(order.ShippingAddress.LastName, 30));
                    parameters.Add("ship_company_name", Text.TrimToLength(order.ShippingAddress.Company, 30));
                    parameters.Add("ship_address_one",
                        Text.TrimToLength(string.Join(", ", order.ShippingAddress.Line2, order.ShippingAddress.Line1),
                            30));
                    parameters.Add("ship_city", Text.TrimToLength(order.ShippingAddress.City, 30));
                    if (order.ShippingAddress.RegionData != null)
                        parameters.Add("ship_state_or_province",
                            Text.TrimToLength(order.ShippingAddress.RegionData.DisplayName, 30));
                    parameters.Add("ship_postal_code", Text.TrimToLength(order.ShippingAddress.PostalCode, 30));
                    parameters.Add("ship_country", Text.TrimToLength(order.ShippingAddress.CountryData.DisplayName, 30));
                    parameters.Add("ship_phone", Text.TrimToLength(order.ShippingAddress.Phone, 30));

                    parameters.Add("bill_first_name", Text.TrimToLength(order.BillingAddress.FirstName, 30));
                    parameters.Add("bill_last_name", Text.TrimToLength(order.BillingAddress.LastName, 30));
                    parameters.Add("bill_company_name", Text.TrimToLength(order.BillingAddress.Company, 30));
                    parameters.Add("bill_address_one",
                        string.Join(", ", order.BillingAddress.Line2, order.BillingAddress.Line1));
                    parameters.Add("bill_city", Text.TrimToLength(order.BillingAddress.City, 30));
                    if (order.BillingAddress.RegionData != null)
                        parameters.Add("bill_state_or_province",
                            Text.TrimToLength(order.BillingAddress.RegionData.DisplayName, 30));
                    parameters.Add("bill_postal_code", Text.TrimToLength(order.BillingAddress.PostalCode, 30));
                    parameters.Add("bill_country", Text.TrimToLength(order.BillingAddress.CountryData.DisplayName, 30));
                    parameters.Add("bill_phone", Text.TrimToLength(order.BillingAddress.Phone, 30));

                    for (var i = 0; i < order.Items.Count; i++)
                    {
                        var item = order.Items[i];
                        parameters.Add("id" + (i + 1), item.ProductSku);
                        parameters.Add("description" + (i + 1), item.ProductName);
                        parameters.Add("quantity" + (i + 1), item.Quantity.ToString());
                        parameters.Add("price" + (i + 1),
                            item.AdjustedPricePerItem.ToString("F2", CultureInfo.InvariantCulture));
                        parameters.Add("subtotal" + (i + 1), item.LineTotal.ToString("F2", CultureInfo.InvariantCulture));
                    }

                    MonerisHPPDPResponse monerisResponse;
                    var url = settings.DeveloperMode ? DevelopmentUrl : ProductionUrl;
                    using (var client = new WebClient())
                    {
                        if (settings.DebugMode)
                            EventLog.LogEvent("Moneris Checkout", Url.BuldQueryString(parameters),
                                EventLogSeverity.Debug);

                        var responseBytes = client.UploadValues(url, "POST", parameters);
                        var responseBody = Encoding.UTF8.GetString(responseBytes);

                        if (settings.DebugMode)
                            EventLog.LogEvent("Moneris Checkout", responseBody, EventLogSeverity.Debug);

                        using (var configFile = new StringReader(responseBody))
                        {
                            var serializer = new XmlSerializer(typeof (MonerisHPPDPResponse));
                            monerisResponse = (MonerisHPPDPResponse) serializer.Deserialize(configFile);
                        }
                    }

                    var urlTemplate = "{0}?hpp_id={1}&ticket={2}&hpp_preload";
                    HttpContextBase httpContext = new HccHttpContextWrapper(HttpContext.Current);
                    httpContext.Response.Redirect(
                        string.Format(urlTemplate, url, monerisResponse.HostedPayPageId, monerisResponse.Ticket), true);
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent("Moneris Checkout", "Exception occurred during call to Moneris: " + ex,
                        EventLogSeverity.Error);
                    context.Errors.Add(new WorkflowMessage("Moneris Checkout Error",
                        GlobalLocalization.GetString("MonerisCheckoutError"), true));
                    return false;
                }
            }

            return false;
        }

        public override Task Clone()
        {
            return new StartMonerisCheckout();
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "CD1E286D-3476-48E8-A227-FB5DDBA5A241";
        }

        public override string TaskName()
        {
            return "Start Moneris Checkout";
        }
    }
}