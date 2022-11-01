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
using System.Collections.Generic;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Urls;
using Hotcakes.PaypalWebServices;
using com.paypal.soap.api;
using System.Web;
using Hotcakes.Web.Logging;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Payment;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
	public class StartPaypalExpressCheckout : ThirdPartyCheckoutOrderTask
    {

		public override string PaymentMethodId
		{
			get
			{
				return PaymentMethods.PaypalExpressId;
			}
		}

		public override bool ProcessCheckout(OrderTaskContext context)
        {
            if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
            {
                try
                {
                    RestPayPalApi replacePayPal = Utilities.PaypalExpressUtilities.GetRestPaypalAPI(context.HccApp.CurrentStore);

                    string cartReturnUrl = HccUrlBuilder.RouteHccUrl(HccRoute.ThirdPartyPayment, null, Uri.UriSchemeHttps);
                    string cartCancelUrl = HccUrlBuilder.RouteHccUrl(HccRoute.Checkout, null, Uri.UriSchemeHttps);

                    EventLog.LogEvent("PayPal Express Checkout", "CartCancelUrl=" + cartCancelUrl, EventLogSeverity.Information);
                    EventLog.LogEvent("PayPal Express Checkout", "CartReturnUrl=" + cartReturnUrl, EventLogSeverity.Information);

                    string mode = PayPalConstants.PAYMENT_MODE_AUTHORIZE;
                    if (!context.HccApp.CurrentStore.Settings.PayPal.ExpressAuthorizeOnly)
					{
                        mode = PayPalConstants.PAYMENT_MODE_CAPTURE;
                    }

                    // Accelerated boarding
                    if (string.IsNullOrWhiteSpace(context.HccApp.CurrentStore.Settings.PayPal.UserName))
					{
                        mode = PayPalConstants.PAYMENT_MODE_CAPTURE;
					}

					var solutionType = context.HccApp.CurrentStore.Settings.PayPal.RequirePayPalAccount ? SolutionTypeType.Mark : SolutionTypeType.Sole;
					bool isNonShipping = !context.Order.HasShippingItems;

                    bool addressSupplied = false;
                    if (context.Inputs["ViaCheckout"] != null
                        && context.Inputs["ViaCheckout"].Value == "1")
                    {
                        addressSupplied = true;
                        context.Order.CustomProperties.Add("hcc", "ViaCheckout", "1");
                    }

					PaymentDetailsItemType[] itemsDetails = GetOrderItemsDetails(context);

					PayPalHttp.HttpResponse expressResponse;
                    if (addressSupplied)
                    {
                        Contacts.Address address = context.Order.ShippingAddress;

                        // in some cases, this logic will be hit with non-shipping orders, causing an exception
                        if (address == null || string.IsNullOrEmpty(address.Bvin))
                        {
                            // this is a workaround for that use case
                            address = context.Order.BillingAddress;
                        }

                        if (address.CountryData != null)
                        {
                            var ISOCode = address.CountryData.IsoCode;

                            var itemsTotalWithoutTax = context.Order.TotalOrderAfterDiscounts;
							if (context.HccApp.CurrentStore.Settings.ApplyVATRules)
							{
								itemsTotalWithoutTax -= context.Order.ItemsTax;
							}
                            string itemsTotal = itemsTotalWithoutTax.ToString("N", CultureInfo.InvariantCulture);
							string taxTotal = context.Order.TotalTax.ToString("N", CultureInfo.InvariantCulture);
							var shippingTotalWithoutTax = context.Order.TotalShippingAfterDiscounts;
							if (context.HccApp.CurrentStore.Settings.ApplyVATRules)
							{
								shippingTotalWithoutTax -= context.Order.ShippingTax;
							}
							string shippingTotal = shippingTotalWithoutTax.ToString("N", CultureInfo.InvariantCulture);

							string orderTotal = context.Order.TotalGrand.ToString("N", CultureInfo.InvariantCulture);
                            expressResponse = System.Threading.Tasks.Task.Run(() => replacePayPal.createOrder(
                                                    itemsDetails,
                                                    itemsTotal,
                                                    taxTotal,
                                                    shippingTotal,
                                                    orderTotal,
                                                    cartReturnUrl,
                                                    cartCancelUrl,
                                                    mode,
                                                    context.HccApp.CurrentStore.Settings.PayPal.Currency,
                                                    solutionType,
                                                    ($"{address.FirstName} {address.LastName}"),
                                                    ISOCode,
                                                    address.Line1,
                                                    address.Line2,
                                                    address.City,
                                                    address.RegionBvin,
                                                    address.PostalCode,
                                                    address.Phone,
                                                    context.Order.OrderNumber + Guid.NewGuid().ToString(),
                                                    isNonShipping)).GetAwaiter().GetResult(); 
                            if (expressResponse == null)
                            {
                                EventLog.LogEvent("PayPal Express Checkout", "Express Response Was Null!", EventLogSeverity.Error);
                            }
                        }
                        else
                        {
                            EventLog.LogEvent("StartPaypalExpressCheckout", "Country with bvin " + address.CountryBvin + " was not found.", EventLogSeverity.Error);
                            return false;
                        }
                    }
                    else
                    {
						decimal includedTax = 0;
						if (context.HccApp.CurrentStore.Settings.ApplyVATRules)
						{
							includedTax = context.Order.ItemsTax;
						}
						string taxTotal = includedTax.ToString("N", CultureInfo.InvariantCulture);
						var itemsTotalWithoutTax = context.Order.TotalOrderAfterDiscounts;
						if (context.HccApp.CurrentStore.Settings.ApplyVATRules)
						{
							itemsTotalWithoutTax -= context.Order.ItemsTax;
						}
						string itemsTotal = itemsTotalWithoutTax.ToString("N", CultureInfo.InvariantCulture);
						string orderTotal = context.Order.TotalOrderAfterDiscounts.ToString("N", CultureInfo.InvariantCulture);
						expressResponse = System.Threading.Tasks.Task.Run(() => replacePayPal.createOrder(itemsDetails,
                            itemsTotal,
                            taxTotal,
                            orderTotal,
                            cartReturnUrl,
                            cartCancelUrl,
                            mode,
                            context.HccApp.CurrentStore.Settings.PayPal.Currency,
                            solutionType,
                            context.Order.OrderNumber + Guid.NewGuid().ToString(),
                            isNonShipping)).GetAwaiter().GetResult();
                        
                        if (expressResponse == null)
                        {
                            EventLog.LogEvent("PayPal Express Checkout", "Express Response2 Was Null!", EventLogSeverity.Error);
                        }
                    }

                    if (expressResponse.StatusCode == HttpStatusCode.Created /*|| expressResponse.Ack == AckCodeType.SuccessWithWarning*/)
                    {
                        context.Order.ThirdPartyOrderId = expressResponse.Result<Order>().Id;

                        // Recording of this info is handled on the paypal express
                        // checkout page instead of here.
                        //Orders.OrderPaymentManager payManager = new Orders.OrderPaymentManager(context.Order);
                        //payManager.PayPalExpressAddInfo(context.Order.TotalGrand, expressResponse.Token);

                        EventLog.LogEvent("PayPal Express Checkout", "Response SUCCESS", EventLogSeverity.Information);

                        Orders.OrderNote note = new Orders.OrderNote();
                        note.IsPublic = false;
                        note.Note = "Paypal Order Accepted With Paypal Order Number: " + expressResponse.Result<Order>().Id;
                        context.Order.Notes.Add(note);
                        if (context.HccApp.OrderServices.Orders.Update(context.Order))
                        {
							string urlTemplate;
                            if (string.Compare(context.HccApp.CurrentStore.Settings.PayPal.Mode, "Live", true) == 0)
                            {
								urlTemplate = PayPalConstants.LIVE_URL;
                            }
                            else
                            {
								urlTemplate = PayPalConstants.SANDBOX_URL;
                            }
							HttpContextBase httpContext = new HccHttpContextWrapper(HttpContext.Current);
                            httpContext.Response.Redirect(string.Format(urlTemplate, expressResponse.Result<Order>().Id), true);
                        }
                        return true;
                    }
                    else
                    {
                        context.Errors.Add(new WorkflowMessage("Paypal checkout error", GlobalLocalization.GetString("PaypalCheckoutCustomerError"), true));
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent("Paypal Express Checkout", "Exception occurred during call to Paypal: " + ex.ToString(), EventLogSeverity.Error);
                    context.Errors.Add(new WorkflowMessage("Paypal checkout error", GlobalLocalization.GetString("PaypalCheckoutCustomerError"), true));
                    return false;
                }
            }

            return false;
        }

		private PaymentDetailsItemType[] GetOrderItemsDetails(OrderTaskContext context)
		{
			var currency = PayPalAPI.GetCurrencyCodeType(context.HccApp.CurrentStore.Settings.PayPal.Currency);

			var itemDetails = new List<PaymentDetailsItemType>();
			
			var itemDetail = new PaymentDetailsItemType();
			itemDetail.Name = context.HccApp.CurrentStore.Settings.FriendlyName;
			var itemsTotalWithoutTax = context.Order.TotalOrderAfterDiscounts;
			if (context.HccApp.CurrentStore.Settings.ApplyVATRules)
			{
				itemsTotalWithoutTax -= context.Order.ItemsTax;
			}
			itemDetail.Amount = new BasicAmountType();
			itemDetail.Amount.Value = itemsTotalWithoutTax.ToString("N", CultureInfo.InvariantCulture);
			itemDetail.Amount.currencyID = currency;

			itemDetails.Add(itemDetail);

			return itemDetails.ToArray();
		}

		public override Task Clone()
		{
			return new StartPaypalExpressCheckout();
		}

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "56597582-05d0-4b51-bd87-7426a9cf146f";
        }

        public override string TaskName()
        {
            return "Start Paypal Express Checkout";
        }
    }
}
