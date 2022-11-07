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
using System.Net;
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;
using Hotcakes.PaypalWebServices;
using Hotcakes.Web.Logging;
using Hotcakes.Web.Validation;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using Order = PayPalCheckoutSdk.Orders.Order;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class PayPalExpressCheckoutController : BaseStoreController
    {
        [NonCacheableResponseFilter]
        public ActionResult Index()
        {
            var model = IndexSetup();

            DisplayPaypalExpressMode(model);
            LoadValuesFromForm(model);

            if (model.CurrentOrder.CustomProperties["ViaCheckout"] != null
                && model.CurrentOrder.CustomProperties["ViaCheckout"].Value == "1")
            {
                SavePaymentInfo(model);
                var redirectUrl = ProcessOrder(model);
                if (redirectUrl != null)
                    return redirectUrl;
            }

            return View(model);
        }

        [NonCacheableResponseFilter]
        [ActionName("Index")]
        [HccHttpPost]
        public ActionResult IndexPost()
        {
            var model = IndexSetup();

            DisplayPaypalExpressMode(model);
            LoadValuesFromForm(model);

            if (ValidatePage(model))
            {
                SaveShippingSelections(model);
                // Save Payment Information
                SavePaymentInfo(model);

                var redirectUrl = ProcessOrder(model);
                if (redirectUrl != null)
                    return redirectUrl;
            }
            // Render Error Summary
            foreach (var v in model.Violations)
            {
                FlashFailure(v.ErrorMessage);
            }

            return View(model);
        }

        private CheckoutViewModel IndexSetup()
        {
            var model = new CheckoutViewModel { CurrentOrder = CurrentCart };

            // Agree Checkbox
            if (HccApp.CurrentStore.Settings.ForceTermsAgreement)
            {
                model.ShowAgreeToTerms = true;
                model.AgreedToTerms = false;
                model.AgreedToTermsDescription = Localization.GetString("TermsAndConditionsAgreement");
                model.LabelTerms = Localization.GetString("TermsAndConditions");
            }
            else
            {
                model.ShowAgreeToTerms = false;
                model.AgreedToTerms = true;
            }

            // Populate Countries
            model.Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries();

            return model;
        }

        private void DisplayPaypalExpressMode(CheckoutViewModel model)
        {
            model.PayPalToken = Request.QueryString["token"] ?? string.Empty;
            model.PayPalPayerId = Request.QueryString["payerId"] ?? string.Empty;

            var ppAPI = PaypalExpressUtilities.GetRestPaypalAPI(HccApp.CurrentStore);
            var failed = false;
            PayPalHttp.HttpResponse ppResponse = null;
            try
            {
                if (!GetExpressCheckoutDetails(ppAPI, ref ppResponse, model))
                {
                    failed = true;
                    EventLog.LogEvent("Paypal Express Checkout",
                        "GetExpressCheckoutDetails call failed. Detailed Errors will follow. ", EventLogSeverity.Error);
                    FlashFailure(Localization.GetString("ErrorOccured"));
                    ViewBag.HideCheckout = true;
                }
            }
            finally
            {
                var o = model.CurrentOrder;

                o.CustomProperties.Add("hcc", "PayerID", Request.QueryString["PayerID"]);
                if (!failed)
                {
                    if (ppResponse != null && ppResponse != null &&
                        ppResponse.Result<Order>().Payer != null)
                    {
                        o.UserEmail = ppResponse.Result<Order>().Payer.Email;
                    }
                }
                HccApp.OrderServices.Orders.Update(o);
            }
        }

        private bool GetExpressCheckoutDetails(RestPayPalApi ppAPI, ref PayPalHttp.HttpResponse ppResponse,
            CheckoutViewModel model)
        {//
            ppResponse = System.Threading.Tasks.Task.Run(() => ppAPI.GetOrder(Request.QueryString["token"])).GetAwaiter().GetResult();
            if (ppResponse.StatusCode == HttpStatusCode.OK)
            {
                var paypalOrder = ppResponse.Result<Order>();
                var payerInfo = paypalOrder.Payer;
                var purchaseUnits = paypalOrder.PurchaseUnits;
                var country = HccApp.GlobalizationServices.Countries.FindByISOCode(payerInfo.AddressPortable.CountryCode.ToString());
                model.CurrentOrder.UserEmail = payerInfo.Email;

                if (model.CurrentOrder.HasShippingItems)
                {
                    model.CurrentOrder.ShippingAddress.FirstName = payerInfo.Name.GivenName;
                    model.CurrentOrder.ShippingAddress.LastName = payerInfo.Name.Surname;

                    //model.CurrentOrder.ShippingAddress.Company = payerInfo. ?? string.Empty;
                    model.CurrentOrder.ShippingAddress.Line1 = purchaseUnits[0].ShippingDetail.AddressPortable.AddressLine1 ?? string.Empty;
                    model.CurrentOrder.ShippingAddress.Line2 = purchaseUnits[0].ShippingDetail.AddressPortable.AddressLine2 ?? string.Empty;
                    model.CurrentOrder.ShippingAddress.CountryBvin = country.Bvin ?? string.Empty;
                    model.CurrentOrder.ShippingAddress.City = purchaseUnits[0].ShippingDetail.AddressPortable.AdminArea1 ?? string.Empty;
                    //model.CurrentOrder.ShippingAddress.RegionBvin = payerInfo.Address.StateOrProvince ?? string.Empty;
                    model.CurrentOrder.ShippingAddress.PostalCode = purchaseUnits[0].ShippingDetail.AddressPortable.PostalCode ?? string.Empty;
                    //model.CurrentOrder.ShippingAddress.Phone = purchaseUnits[0].ShippingDetail.AddressPortable.AddressLine3 ?? string.Empty;
                    model.CurrentOrder.ThirdPartyOrderId = paypalOrder.Id;

                    model.CurrentOrder.BillingAddress.FirstName = payerInfo.Name.GivenName;
                    model.CurrentOrder.BillingAddress.LastName = payerInfo.Name.Surname;
                    model.CurrentOrder.BillingAddress.Line1 = payerInfo.AddressPortable.AddressLine1 ?? string.Empty;
                    model.CurrentOrder.BillingAddress.Line2 = payerInfo.AddressPortable.AdminArea2 ?? string.Empty;
                    model.CurrentOrder.BillingAddress.CountryBvin = country.Bvin ?? string.Empty;
                    model.CurrentOrder.BillingAddress.City = payerInfo.AddressPortable.AdminArea1 ?? string.Empty;
                    model.CurrentOrder.BillingAddress.PostalCode = payerInfo.AddressPortable.PostalCode ?? string.Empty;

                    ViewBag.AddressStatus = Localization.GetString("Confirmed");
                }
                return true;
            }
            return false;
        }

        private void LoadValuesFromForm(CheckoutViewModel model)
        {
            // Agree to Terms
            var agreedValue = Request.Form["agreed"];
            if (!string.IsNullOrEmpty(agreedValue))
            {
                model.AgreedToTerms = true;
            }

            var o = model.CurrentOrder;
            if (o.HasShippingItems
                && model.CurrentOrder.CustomProperties["ViaCheckout"] != null
                && model.CurrentOrder.CustomProperties["ViaCheckout"].Value == "1")
                o.ShippingAddress.CopyTo(o.BillingAddress);
            HccApp.CalculateOrderAndSave(o);
            LoadShippingMethodsForOrder(o);
        }

        private void LoadShippingMethodsForOrder(Hotcakes.Commerce.Orders.Order  o)
        {
            var rates = HccApp.OrderServices.FindAvailableShippingRates(o);

            var rateKey = o.ShippingMethodUniqueKey;
            var rateIsAvailable = false;

            // See if rate is available
            if (rateKey.Length > 0)
            {
                foreach (ShippingRateDisplay r in rates)
                {
                    if (r.UniqueKey == rateKey)
                    {
                        rateIsAvailable = true;
                        HccApp.OrderServices.OrdersRequestShippingMethod(r, o);
                    }
                }
            }

            // if it's not availabe, pick the first one or default
            if (rateIsAvailable == false)
            {
                if (rates.Count > 0)
                {
                    HccApp.OrderServices.OrdersRequestShippingMethod(rates[0], o);
                    rateKey = rates[0].UniqueKey;
                }
                else
                {
                    o.ClearShippingPricesAndMethod();
                }
            }

            ViewBag.ShippingRates = HtmlRendering.ShippingRatesToRadioButtons(rates, 300, o.ShippingMethodUniqueKey);
        }

        private RedirectResult ProcessOrder(CheckoutViewModel model)
        {
            HccApp.OrderServices.Orders.Update(model.CurrentOrder);

            // Save as Order
            var c = new OrderTaskContext
            {
                UserId = HccApp.CurrentCustomerId,
                Order = model.CurrentOrder
            };

            if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrder))
            {
                // Clear Cart ID because we're now an order
                SessionManager.SetCurrentCartId(HccApp.CurrentStore, string.Empty);

                // Process Payment
                if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrderPayments))
                {
                    Workflow.RunByName(c, WorkflowNames.ProcessNewOrderAfterPayments);
                    var tempOrder = HccApp.OrderServices.Orders.FindForCurrentStore(model.CurrentOrder.bvin);
                    HccApp.CurrentRequestContext.IntegrationEvents.OrderReceived(tempOrder, HccApp);
                    SessionManager.AnalyticsOrderId = model.CurrentOrder.bvin;
                    return Redirect(Url.RouteHccUrl(HccRoute.Checkout,
                        new { action = "receipt", id = model.CurrentOrder.bvin }));
                }
            }
            return null;
        }

        private bool ValidatePage(CheckoutViewModel model)
        {
            var result = true;
            if (!HccApp.CurrentStore.Settings.PayPal.AllowUnconfirmedAddresses)
            {
                if (string.Compare(ViewBag.AddressStatus, Localization.GetString("Unconfirmed"), true) == 0)
                {
                    model.Violations.Add(new RuleViolation
                    {
                        ControlName = string.Empty,
                        ErrorMessage = Localization.GetString("AddressNotAllowed"),
                        PropertyName = string.Empty,
                        PropertyValue = string.Empty
                    });
                    result = false;
                }
            }
            if (Request.Form["shippingrate"] == null)
            {
                model.Violations.Add(new RuleViolation
                {
                    ControlName = string.Empty,
                    ErrorMessage = Localization.GetString("SelectShipping"),
                    PropertyName = string.Empty,
                    PropertyValue = string.Empty
                });
                result = false;
            }
            if (model.AgreedToTerms == false && model.ShowAgreeToTerms)
            {
                model.Violations.Add(new RuleViolation("Terms", "Terms",
                    Localization.GetString("SiteTermsAgreementError")));
                result = false;
            }
            return result;
        }

        private void SaveShippingSelections(CheckoutViewModel model)
        {
            //Shipping
            var shippingRateKey = Request.Form["shippingrate"];
            HccApp.OrderServices.OrdersRequestShippingMethodByUniqueKey(shippingRateKey, model.CurrentOrder);

            // Save Values so far in case of later errors
            HccApp.CalculateOrder(model.CurrentOrder);

            // Save all the changes to the order
            HccApp.OrderServices.Orders.Update(model.CurrentOrder);
        }

        private void SavePaymentInfo(CheckoutViewModel model)
        {
            var token = model.PayPalToken;
            var payerId = model.PayPalPayerId;
            if (!string.IsNullOrEmpty(payerId))
            {
                // This is to fix a bug with paypal returning multiple payerId's
                payerId = payerId.Split(',')[0];
            }

            var payManager = new OrderPaymentManager(model.CurrentOrder, HccApp);
            payManager.PayPalExpressAddInfo(model.CurrentOrder.TotalGrand, token, payerId);
        }
    }
}