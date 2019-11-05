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
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Payment.Methods;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Payment;
using Hotcakes.Web.Logging;
using Hotcakes.Web.Validation;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class OgoneCheckoutController : BaseStoreController
    {
        [NonCacheableResponseFilter]
        public ActionResult Index()
        {
            var settings = new OgoneSettings();
            var methodSettings = HccApp.CurrentStore.Settings.MethodSettingsGet(PaymentMethods.OgoneId);
            settings.Merge(methodSettings);

            var response = GetOgoneResponse();

            if (settings.DebugMode)
                EventLog.LogEvent("Ogone Checkout", Request.RawUrl, EventLogSeverity.Debug);

            var parameters = GetOgoneHashableParameters();
            var shaSignCalc = OgoneUtils.CalculateShaHash(parameters, settings.HashAlgorithm, settings.ShaOutPassPhrase);

            var model = new CheckoutViewModel();
            model.CurrentOrder = HccApp.OrderServices.CurrentShoppingCart();
            if (shaSignCalc == response.ShaSignature)
            {
                // SHA Signature Match
                if (response.Status == 5 || response.Status == 51 ||
                    response.Status == 9 || response.Status == 91)
                {
                    // Approved
                    SavePaymentInfo(model, response);
                    ProcessOrder(model);
                }
                else
                {
                    // Declined
                    FlashFailure(Localization.GetString("ErrorOccured"));
                }
            }
            else
            {
                // SHA Signature Mismatch
                FlashFailure(Localization.GetString("ShaSignatureMismatch.Text"));
            }

            // Render Error Summary
            foreach (var v in model.Violations)
            {
                FlashFailure(v.ErrorMessage);
            }

            return View(model);
        }

        private NameValueCollection GetOgoneHashableParameters()
        {
            var parameters = new NameValueCollection();
            foreach (string key in Request.QueryString.Keys)
            {
                var upperKey = key.ToUpper();
                if (OgoneUtils.ShaOutParams.Contains(upperKey))
                {
                    parameters.Add(upperKey, Request.QueryString[key]);
                }
            }
            return parameters;
        }

        private OgoneResponse GetOgoneResponse()
        {
            var response = new OgoneResponse();

            response.OrderId = Request.QueryString["orderID"];
            var amount = Request.QueryString["amount"];
            if (!string.IsNullOrWhiteSpace(amount))
                response.Amount = decimal.Parse(amount, CultureInfo.InvariantCulture);
            response.Currency = Request.QueryString["currency"];
            response.PaymentMethod = Request.QueryString["PM"];
            response.Acceptance = Request.QueryString["ACCEPTANCE"];
            var status = Request.QueryString["STATUS"];
            if (!string.IsNullOrWhiteSpace(status))
                response.Status = int.Parse(status, CultureInfo.InvariantCulture);
            response.CardNumber = Request.QueryString["CARDNO"];
            response.PayId = Request.QueryString["PAYID"];
            response.NcError = Request.QueryString["NCERROR"];
            response.Brand = Request.QueryString["BRAND"];
            response.ShaSignature = Request.QueryString["SHASIGN"];
            return response;
        }

        private void ProcessOrder(CheckoutViewModel model)
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
                    Response.Redirect(Url.RouteHccUrl(HccRoute.Checkout,
                        new {action = "receipt", id = model.CurrentOrder.bvin}));
                }
            }
            else
            {
                var workflowErrAdded = false;
                // Show Errors
                foreach (var item in c.GetCustomerVisibleErrors())
                {
                    model.Violations.Add(new RuleViolation("Workflow", item.Name, item.Description));
                    workflowErrAdded = true;
                }

                if (!workflowErrAdded)
                {
                    model.Violations.Add(new RuleViolation("Workflow", "Internal Error",
                        Localization.GetString("InternalErrorOccured")));
                }
            }
        }

        private void SavePaymentInfo(CheckoutViewModel model, OgoneResponse ogoneReponse)
        {
            var orderTransaction = new OrderTransaction
            {
                Success = true,
                MethodId = PaymentMethods.OgoneId,
                OrderId = model.CurrentOrder.bvin,
                Amount = ogoneReponse.Amount,
                Action = ActionType.ThirdPartyPayMethodCharge,
                RefNum1 = ogoneReponse.PaymentMethod + " " + ogoneReponse.OrderId,
                RefNum2 = ogoneReponse.PayId
            };

            HccApp.OrderServices.AddPaymentTransactionToOrder(model.CurrentOrder, orderTransaction);
        }
    }
}