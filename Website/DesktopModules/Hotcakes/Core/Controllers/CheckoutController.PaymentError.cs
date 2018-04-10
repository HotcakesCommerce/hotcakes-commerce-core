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

using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
	public partial class CheckoutController
	{
		#region Public methods

		[HccHttpGet]
		[NonCacheableResponseFilter]
		public ActionResult PaymentError()
		{
			CheckoutViewModel model = PaymentErrorSetup();
			if (ValidateSession(model))
			{
				return Redirect(Url.RouteHccUrl(HccRoute.Checkout));
			}


			RenderErrorSummary(model);
			return View(model);
		}

		[HccHttpPost]
		[NonCacheableResponseFilter]
		[ActionName("PaymentError")]
		public ActionResult PaymentErrorPost()
		{
            var model = PaymentErrorSetup();

			if (ValidateSession(model))
			{
				
				return Redirect(Url.RouteHccUrl(HccRoute.Checkout));
			}


			// Load Post Data and Update Order
			LoadAddressFromForm("billing", model.CurrentOrder.BillingAddress);
			LoadPaymentFromForm(model);
			HccApp.OrderServices.Orders.Update(model.CurrentOrder);

			// Validate Data
			if (ValidatePaymentErrorOrder(model))
			{
				AddPaymentInfoTransaction(model);
				HccApp.OrderServices.Orders.Update(model.CurrentOrder);

				ProcessPaymentErrorOrder(model);
			}

			RenderErrorSummary(model);

			return View(model);
		}

		[HccHttpGet]
		public ActionResult Cancel()
		{
            var bvin = SessionManager.GetCurrentPaymentPendingCartId(HccApp.CurrentStore);
			if (bvin.Trim().Length < 1)
			{
				Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
			}

            var currentOrder = HccApp.OrderServices.Orders.FindForCurrentStore(bvin);
			if (currentOrder != null)
			{
				currentOrder.StatusCode = OrderStatusCode.Cancelled;
				currentOrder.StatusName = "Cancelled";
				currentOrder.IsPlaced = false;

                var n = new OrderNote();
				n.IsPublic = true;
				n.Note = "Cancelled by Customer";
				currentOrder.Notes.Add(n);

				HccApp.OrderServices.Orders.Update(currentOrder);
            }

            // clear the cart id, because now the cancelled cart is technically an abandoned cart
            SessionManager.SetCurrentCartId(HccApp.CurrentStore, string.Empty);

			return Redirect(Url.RouteHccUrl(HccRoute.Cart));
		}

		#endregion

		#region Implementation

		private CheckoutViewModel PaymentErrorSetup()
		{
            var model = new CheckoutViewModel();
			LoadPendingOrder(model);

			// Populate Countries
			model.Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries();
			model.PaymentViewModel.AcceptedCardTypes = HccApp.CurrentStore.Settings.PaymentAcceptedCards;

			return model;
		}

		private void LoadPendingOrder(CheckoutViewModel model)
		{
            var bvin = SessionManager.GetCurrentPaymentPendingCartId(HccApp.CurrentStore);
			if (bvin.Trim().Length < 1)
				Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));

            var result = HccApp.OrderServices.Orders.FindForCurrentStore(bvin);
			if (result == null)
			{
				Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
			}
			model.CurrentOrder = result;

			if (result.Items.Count == 0)
			{
				Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
			}

			// Email
			model.IsLoggedIn = false;
            if (SessionManager.IsUserAuthenticated(HccApp))
			{
				model.IsLoggedIn = true;
				model.CurrentCustomer = HccApp.CurrentCustomer;
				model.CurrentOrder.UserEmail = HccApp.CurrentCustomer.Email;

				// Copy customer addresses to order
				model.CurrentCustomer.ShippingAddress.CopyTo(model.CurrentOrder.ShippingAddress);
				if (!model.BillShipSame)
				{
                    var billAddr = model.CurrentCustomer.BillingAddress;
					billAddr.CopyTo(model.CurrentOrder.BillingAddress);
				}
			}

			// Payment
			model.PaymentViewModel = LoadPaymentsModel(model.CurrentOrder);
		}

		private bool ValidatePaymentErrorOrder(CheckoutViewModel model)
		{
			model.Violations.AddRange(ValidateAddress(model.CurrentOrder.BillingAddress, "Billing"));
			model.Violations.AddRange(ValidatePayment(model));
            return model.Violations.Count <= 0;
		}

		private void ProcessPaymentErrorOrder(CheckoutViewModel model)
		{
			// Save as Order
            var c = new OrderTaskContext
            {
                UserId = HccApp.CurrentCustomerId,
                Order = model.CurrentOrder
            };
			c.Inputs.SetProperty("hcc", "CardSecurityCode", model.PaymentViewModel.DataCreditCard.SecurityCode);

			if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrderPayments))
			{
				// Clear Pending Cart ID because payment is good
				SessionManager.SetCurrentPaymentPendingCartId(HccApp.CurrentStore, string.Empty);

				// Process Post Payment Stuff
				Workflow.RunByName(c, WorkflowNames.ProcessNewOrderAfterPayments);
                var tempOrder = HccApp.OrderServices.Orders.FindForCurrentStore(model.CurrentOrder.bvin);
				HccApp.CurrentRequestContext.IntegrationEvents.OrderReceived(tempOrder, HccApp);
				SessionManager.AnalyticsOrderId = model.CurrentOrder.bvin;
                Response.Redirect(Url.RouteHccUrl(HccRoute.Checkout,
                    new {action = "receipt", id = model.CurrentOrder.bvin}));
			}
		}

		private bool ValidateSession(CheckoutViewModel model)
		{
			//If session is timeout (DNN User) dont allow to see anything.
			if (model.IsLoggedIn == false || SessionManager.IsUserAuthenticated(this.HccApp) == false)
			{
				//model.Violations.Add(new RuleViolation("Session expired", "", "Oops! It looks like your shopping session has expired. Your order has not been placed. You'll need to place your order again. We're sorry for the inconvenience."));

				string bvin = SessionManager.GetCurrentPaymentPendingCartId(HccApp.CurrentStore);
				if (bvin.Trim().Length < 1)
				{
					Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
				}

				Order currentOrder = HccApp.OrderServices.Orders.FindForCurrentStore(bvin);
				if (currentOrder != null)
				{
					currentOrder.StatusCode = OrderStatusCode.Cancelled;
					currentOrder.StatusName = "Cancelled";
					currentOrder.IsPlaced = false;

					OrderNote n = new OrderNote();
					n.IsPublic = true;
					n.Note = "Order cancelled by the system due to a session timeout";
					currentOrder.Notes.Add(n);
					HccApp.OrderServices.Orders.Update(currentOrder);
					
					//currentOrder.StatusCode = OrderStatusCode.SessionTimeout;
					SessionManager.SetCurrentCartId(HccApp.CurrentStore, currentOrder.bvin);
					SessionManager.SetCurrentPaymentPendingCartId(HccApp.CurrentStore, string.Empty);
					SessionManager.SetSessionObject("SessionError_" + currentOrder.bvin, bool.TrueString);
				}
				return true;
				//return Redirect(Url.RouteHccUrl(HccRoute.Cart));
			}
			return false;
		}

		#endregion
	}
}