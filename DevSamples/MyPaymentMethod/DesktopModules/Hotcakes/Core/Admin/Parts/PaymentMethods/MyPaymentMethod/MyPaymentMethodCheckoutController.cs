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
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Payment;
using Hotcakes.Web.Validation;

namespace MyCompany.MyPaymentMethod
{
    /// <summary>
    ///     You have to be directed to the page that contains this view after payment processing
    /// </summary>
    [Serializable]
    public class MyPaymentMethodCheckoutController : BaseStoreController
    {
        [NonCacheableResponseFilter]
        public ActionResult Index()
        {
            // Here you can parse response from payment provider if you used hosted pay page scenario

            var settings = new MyPaymentMethodSettings();
            var methodSettings = HccApp.CurrentStore.Settings.MethodSettingsGet(MyPaymentMethod.Id());
            settings.Merge(methodSettings);

            var model = new CheckoutViewModel {CurrentOrder = HccApp.OrderServices.CurrentShoppingCart()};

            var paymentSuccess = true; // this have to be determined from service response
            if (paymentSuccess)
            {
                SavePaymentInfo(model);
                ProcessOrder(model);
            }
            else
            {
                FlashFailure(Localization.GetString("ErrorOccured"));
            }

            // Render Error Summary
            foreach (var v in model.Violations)
            {
                FlashFailure(v.ErrorMessage);
            }

            return View(model);
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

        private void SavePaymentInfo(CheckoutViewModel model)
        {
            var orderTransaction = new OrderTransaction
            {
                Success = true,
                MethodId = MyPaymentMethod.Id(),
                OrderId = model.CurrentOrder.bvin,
                Amount = model.CurrentOrder.TotalGrandAfterStoreCredits(HccApp.OrderServices),
                Action = ActionType.ThirdPartyPayMethodCharge,
                RefNum1 = string.Empty,
                RefNum2 = string.Empty
            };

            HccApp.OrderServices.AddPaymentTransactionToOrder(model.CurrentOrder, orderTransaction);
        }
    }
}