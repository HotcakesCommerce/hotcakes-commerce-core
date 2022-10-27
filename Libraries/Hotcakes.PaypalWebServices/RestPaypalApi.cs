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
using System.Threading.Tasks;
using com.paypal.soap.api;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;
using Capture = PayPalCheckoutSdk.Orders.Capture;
using LinkDescription = PayPalCheckoutSdk.Orders.LinkDescription;
using Money = PayPalCheckoutSdk.Orders.Money;

namespace Hotcakes.PaypalWebServices
{
    /// <summary>
    ///     Summary description for PayPalAPI.
    /// </summary>
    [Serializable]
    public class RestPayPalApi
    {
        private string clientId; 
        private string secret; 
        private string mode;

        public RestPayPalApi(string clientId, string secret, string mode)
        {
            this.clientId = clientId;
            this.secret = secret;
            this.mode = mode;
        }

        public HttpClient client()
        {
            PayPalEnvironment environment;

            if (mode == "Sandbox")
            {
                // Creating a sandbox environment
                environment = new SandboxEnvironment(clientId, secret);
            }
            else
            {
                // Creating a sandbox live environment
                environment = new LiveEnvironment(clientId, secret);
            }
            
            // Creating a client for the environment
            PayPalHttpClient client = new PayPalHttpClient(environment);

            return client;
        }

        public async Task<HttpResponse> createOrder(PaymentDetailsItemType[] itemsDetails,
            string itemsTotal, string taxTotal, string shippingTotal,
            string orderTotal, string returnURL, string cancelURL, string paymentAction,
            string currencyCodeType, SolutionTypeType solutionType, string name,
            string countryISOCode, string street1, string street2, string city, string region, string postalCode,
            string phone, string invoiceId, bool isNonShipping)
        {
            HttpResponse response;
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            OrderRequest order = new OrderRequest()
            {
                CheckoutPaymentIntent = paymentAction,

                ApplicationContext = new ApplicationContext
                {
                    CancelUrl = cancelURL,
                    ReturnUrl = returnURL,
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                    {
                        new PurchaseUnitRequest
                        {
                            InvoiceId = invoiceId,
                            AmountWithBreakdown = new AmountWithBreakdown
                            {
                                CurrencyCode = currencyCodeType,
                                Value = orderTotal,
                                AmountBreakdown = new AmountBreakdown
                                {
                                    ItemTotal = new Money
                                    {
                                        CurrencyCode = currencyCodeType,
                                        Value = itemsTotal
                                    },
                                    Shipping = new Money
                                    {
                                        CurrencyCode = currencyCodeType,
                                        Value = shippingTotal
                                    },
                                    TaxTotal = new Money
                                    {
                                        CurrencyCode = currencyCodeType,
                                        Value = taxTotal
                                    },
                                }
                            },
                            ShippingDetail = new ShippingDetail
                            {
                                Name = new Name
                                {
                                    FullName = name
                                },
                                AddressPortable = new AddressPortable
                                {
                                    AddressLine1 = street1,
                                    AddressLine2 = street2,
                                    AdminArea2 = city,
                                    AdminArea1 = region,
                                    PostalCode = postalCode,
                                    CountryCode = countryISOCode,
                                }
                            }
                        }
                    }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            response = await client().Execute(request);

            return response;
        }


        public async Task<HttpResponse> createOrder(PaymentDetailsItemType[] itemsDetails,
            string itemsTotal, string taxTotal, string orderTotal,
            string returnURL, string cancelURL, string paymentAction, string currencyCodeType,
            SolutionTypeType solutionType, string invoiceId, bool isNonShipping)
        {

            HttpResponse response;
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            OrderRequest order = new OrderRequest()
            {
                CheckoutPaymentIntent = paymentAction,
                
                ApplicationContext = new ApplicationContext
                {
                    CancelUrl = cancelURL,
                    ReturnUrl = returnURL,
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest()
                    {
                        InvoiceId = invoiceId,
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = currencyCodeType,
                            Value = orderTotal,
                            AmountBreakdown = new AmountBreakdown()
                            {
                                ItemTotal = new Money
                                {
                                    CurrencyCode = currencyCodeType,
                                    Value = itemsTotal
                                },
                                TaxTotal = new Money
                                {
                                    CurrencyCode = currencyCodeType,
                                    Value = taxTotal
                                },
                            }
                        }
                    }
                }
            };

            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            response = await client().Execute(request);

            return response;
        }


        public async Task<HttpResponse> GetOrder(string orderId)
        {
            OrdersGetRequest request = new OrdersGetRequest(orderId);
            var response = await client().Execute(request);
            return response;
        }

        public async Task<HttpResponse> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());
            var response = await client().Execute(request);
            var result = response.Result<Order>();
            
            return response;
        }

        public async Task<HttpResponse> AuthorizeOrder(string orderId)
        {
            var request = new OrdersAuthorizeRequest(orderId);
            request.Prefer("return=representation");
            request.RequestBody(new AuthorizeRequest());
            var response = await client().Execute(request);
            
            return response;
        }

        public async Task<HttpResponse> CaptureAuthorizedOrder(string AuthorizationId,string value,
            string currencyId, string invoiceId)
        {
            var capture = new CaptureRequest
            {
                Amount = new PayPalCheckoutSdk.Payments.Money()
                {
                    Value = value,
                    CurrencyCode = currencyId
                },
                InvoiceId = invoiceId,
            };
            var request = new AuthorizationsCaptureRequest(AuthorizationId);
            request.Prefer("return=representation");
            request.RequestBody(capture);
            var response = await client().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Payments.Capture>();

            return response;
        }


        public async Task<HttpResponse> CapturesRefund(string CaptureId, string amount,
            string storeCurrency)
        {
            var request = new CapturesRefundRequest(CaptureId);
            request.Prefer("return=representation");
            RefundRequest refundRequest = new RefundRequest()
            {
                Amount = new PayPalCheckoutSdk.Payments.Money()
                {
                    Value = amount,
                    CurrencyCode = storeCurrency
                }
            };
            request.RequestBody(refundRequest);
            var response = await client().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Payments.Refund>();

            return response;
        }


        public async Task<HttpResponse> VoidAuthorizedPayment(string AuthorizationId)
        {
            var request = new AuthorizationsVoidRequest(AuthorizationId);
            var response = await client().Execute(request);
            return response;
        }

    }
}