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
using com.paypal.sdk.profiles;
using com.paypal.sdk.services;
using com.paypal.soap.api;

namespace Hotcakes.PaypalWebServices
{
    /// <summary>
    ///     Summary description for PayPalAPI.
    /// </summary>
    [Serializable]
    public class PayPalAPI
    {
        private readonly CallerServices caller;

        public PayPalAPI(IAPIProfile PayPalProfile)
        {
            caller = new CallerServices {APIProfile = PayPalProfile};
        }

        public static CurrencyCodeType GetCurrencyCodeType(string currencyCode)
        {
            if (Enum.IsDefined(typeof (CurrencyCodeType), currencyCode))
            {
                return (CurrencyCodeType) Enum.Parse(typeof (CurrencyCodeType), currencyCode, true);
            }
            return CurrencyCodeType.USD;
        }

        public static CountryCodeType GetCountryCode(string isoCode)
        {
            if (Enum.IsDefined(typeof (CountryCodeType), isoCode))
            {
                return (CountryCodeType) Enum.Parse(typeof (CountryCodeType), isoCode, true);
            }
            return CountryCodeType.US;
        }

        public TransactionSearchResponseType TransactionSearch(DateTime startDate, DateTime endDate)
        {
            // Create the request object
            var concreteRequest = new TransactionSearchRequestType
            {
                StartDate = startDate.ToUniversalTime(),
                EndDate = endDate.AddDays(1).ToUniversalTime(),
                EndDateSpecified = true
            };

            //end date inclusive
            return (TransactionSearchResponseType) caller.Call("TransactionSearch", concreteRequest);
        }

        public GetTransactionDetailsResponseType GetTransactionDetails(string trxID)
        {
            // Create the request object
            var concreteRequest = new GetTransactionDetailsRequestType {TransactionID = trxID};

            return (GetTransactionDetailsResponseType) caller.Call("GetTransactionDetails", concreteRequest);
        }

        public RefundTransactionResponseType RefundTransaction(string trxID, string refundType, string amount,
            CurrencyCodeType storeCurrency)
        {
            // Create the request object
            var concreteRequest = new RefundTransactionRequestType
            {
                TransactionID = trxID,
                RefundType = RefundType.Partial,
                RefundTypeSpecified = true,
                Amount = new BasicAmountType
                {
                    currencyID = storeCurrency,
                    Value = amount
                }
            };

            return (RefundTransactionResponseType) caller.Call("RefundTransaction", concreteRequest);
        }

        public DoDirectPaymentResponseType DoDirectPayment(string paymentAmount, string buyerBillingLastName,
            string buyerBillingFirstName,
            string buyerShippingLastName, string buyerShippingFirstName, string buyerBillingAddress1,
            string buyerBillingAddress2,
            string buyerBillingCity, string buyerBillingState, string buyerBillingPostalCode,
            CountryCodeType buyerBillingCountryCode,
            string creditCardType, string creditCardNumber, string CVV2, int expMonth, int expYear,
            PaymentActionCodeType paymentAction,
            string ipAddress, string buyerShippingAddress1, string buyerShippingAddress2, string buyerShippingCity,
            string buyerShippingState,
            string buyerShippingPostalCode, CountryCodeType buyerShippingCountryCode, string invoiceId,
            CurrencyCodeType storeCurrency)
        {
            // Create the request object
            var pp_Request = new DoDirectPaymentRequestType
            {
                DoDirectPaymentRequestDetails = new DoDirectPaymentRequestDetailsType
                {
                    IPAddress = ipAddress,
                    MerchantSessionId = string.Empty,
                    PaymentAction = paymentAction,
                    CreditCard = new CreditCardDetailsType
                    {
                        CreditCardNumber = creditCardNumber
                    }
                }
            };

            // Create the request details object

            switch (creditCardType)
            {
                case "Visa":
                    pp_Request.DoDirectPaymentRequestDetails.CreditCard.CreditCardType = CreditCardTypeType.Visa;
                    break;
                case "MasterCard":
                    pp_Request.DoDirectPaymentRequestDetails.CreditCard.CreditCardType = CreditCardTypeType.MasterCard;
                    break;
                case "Discover":
                    pp_Request.DoDirectPaymentRequestDetails.CreditCard.CreditCardType = CreditCardTypeType.Discover;
                    break;
                case "Amex":
                    pp_Request.DoDirectPaymentRequestDetails.CreditCard.CreditCardType = CreditCardTypeType.Amex;
                    break;
            }

            pp_Request.DoDirectPaymentRequestDetails.CreditCard.CVV2 = CVV2;
            pp_Request.DoDirectPaymentRequestDetails.CreditCard.ExpMonth = expMonth;
            pp_Request.DoDirectPaymentRequestDetails.CreditCard.ExpYear = expYear;
            pp_Request.DoDirectPaymentRequestDetails.CreditCard.ExpMonthSpecified = true;
            pp_Request.DoDirectPaymentRequestDetails.CreditCard.ExpYearSpecified = true;

            pp_Request.DoDirectPaymentRequestDetails.CreditCard.CardOwner = new PayerInfoType
            {
                Payer = string.Empty,
                PayerID = string.Empty,
                PayerStatus = PayPalUserStatusCodeType.unverified,
                PayerCountry = CountryCodeType.US,
                Address = new AddressType
                {
                    Street1 = buyerBillingAddress1,
                    Street2 = buyerBillingAddress2,
                    CityName = buyerBillingCity,
                    StateOrProvince = buyerBillingState,
                    PostalCode = buyerBillingPostalCode,
                    Country = buyerBillingCountryCode,
                    CountrySpecified = true
                }
            };

            pp_Request.DoDirectPaymentRequestDetails.PaymentDetails = new PaymentDetailsType
            {
                ShipToAddress = new AddressType
                {
                    Name = buyerShippingFirstName + " " +
                           buyerShippingLastName,
                    Street1 = buyerShippingAddress1,
                    Street2 = buyerShippingAddress2,
                    CityName = buyerShippingCity,
                    StateOrProvince = buyerShippingState,
                    PostalCode = buyerShippingPostalCode,
                    Country = buyerShippingCountryCode,
                    CountrySpecified = true
                }
            };

            pp_Request.DoDirectPaymentRequestDetails.CreditCard.CardOwner.PayerName = new PersonNameType
            {
                FirstName = buyerBillingFirstName,
                LastName = buyerBillingLastName
            };

            pp_Request.DoDirectPaymentRequestDetails.PaymentDetails.OrderTotal = new BasicAmountType();
            pp_Request.DoDirectPaymentRequestDetails.PaymentDetails.InvoiceID = invoiceId;

            pp_Request.DoDirectPaymentRequestDetails.PaymentDetails.OrderTotal.currencyID = storeCurrency;
            pp_Request.DoDirectPaymentRequestDetails.PaymentDetails.OrderTotal.Value = paymentAmount;
            pp_Request.DoDirectPaymentRequestDetails.PaymentDetails.ButtonSource = "HotcakesCommerce_Cart_DP_US";

            return (DoDirectPaymentResponseType) caller.Call("DoDirectPayment", pp_Request);
        }


        public SetExpressCheckoutResponseType SetExpressCheckout(PaymentDetailsItemType[] itemsDetails,
            string itemsTotal, string taxTotal, string orderTotal,
            string returnURL, string cancelURL, PaymentActionCodeType paymentAction, CurrencyCodeType currencyCodeType,
            SolutionTypeType solutionType, string invoiceId, bool isNonShipping)
        {
            // Create the request object
            var pp_request = new SetExpressCheckoutRequestType
            {
                // Create the request details object
                SetExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType
                {
                    CancelURL = cancelURL,
                    ReturnURL = returnURL,
                    NoShipping = isNonShipping ? "1" : "0",
                    SolutionType = solutionType,
                    SolutionTypeSpecified = true
                }
            };

            //pp_request.SetExpressCheckoutRequestDetails.ReqBillingAddress = (isNonShipping ? "1" : "0");

            var paymentDetails = new PaymentDetailsType
            {
                InvoiceID = invoiceId,
                PaymentAction = paymentAction,
                PaymentActionSpecified = true,
                PaymentDetailsItem = itemsDetails,
                ItemTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = itemsTotal
                },
                TaxTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = taxTotal
                },
                OrderTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = orderTotal
                }
            };

            pp_request.SetExpressCheckoutRequestDetails.PaymentDetails = new[] {paymentDetails};

            return (SetExpressCheckoutResponseType) caller.Call("SetExpressCheckout", pp_request);
        }

        public SetExpressCheckoutResponseType SetExpressCheckout(PaymentDetailsItemType[] itemsDetails,
            string itemsTotal, string taxTotal, string shippingTotal,
            string orderTotal, string returnURL, string cancelURL, PaymentActionCodeType paymentAction,
            CurrencyCodeType currencyCodeType, SolutionTypeType solutionType, string name,
            string countryISOCode, string street1, string street2, string city, string region, string postalCode,
            string phone, string invoiceId, bool isNonShipping)
        {
            // Create the request object
            var pp_request = new SetExpressCheckoutRequestType
            {
                SetExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType
                {
                    CancelURL = cancelURL,
                    ReturnURL = returnURL,
                    AddressOverride = "1",
                    NoShipping = isNonShipping ? "1" : "0",
                    SolutionType = solutionType,
                    SolutionTypeSpecified = true
                }
            };

            // Create the request details object

            //pp_request.SetExpressCheckoutRequestDetails.ReqBillingAddress = (isNonShipping ? "1" : "0");

            var paymentDetails = new PaymentDetailsType
            {
                InvoiceID = invoiceId,
                PaymentAction = paymentAction,
                PaymentActionSpecified = true,
                PaymentDetailsItem = itemsDetails,
                ItemTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = itemsTotal
                },
                TaxTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = taxTotal
                },
                ShippingTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = shippingTotal
                },
                OrderTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = orderTotal
                },
                ShipToAddress = new AddressType
                {
                    AddressStatusSpecified = false,
                    AddressOwnerSpecified = false,
                    Street1 = street1,
                    Street2 = street2,
                    CityName = city,
                    StateOrProvince = region,
                    PostalCode = postalCode,
                    CountrySpecified = true,
                    Country = GetCountryCode(countryISOCode),
                    Phone = phone,
                    Name = name
                }
            };

            pp_request.SetExpressCheckoutRequestDetails.PaymentDetails = new[] {paymentDetails};

            return (SetExpressCheckoutResponseType) caller.Call("SetExpressCheckout", pp_request);
        }


        public GetExpressCheckoutDetailsResponseType GetExpressCheckoutDetails(string token)
        {
            // Create the request object
            var pp_request = new GetExpressCheckoutDetailsRequestType {Token = token};
            
            return (GetExpressCheckoutDetailsResponseType) caller.Call("GetExpressCheckoutDetails", pp_request);
        }

        public DoExpressCheckoutPaymentResponseType DoExpressCheckoutPayment(string token, string payerID,
            string paymentAmount, PaymentActionCodeType paymentAction, CurrencyCodeType currencyCodeType,
            string invoiceId)
        {
            // Create the request object
            var pp_request = new DoExpressCheckoutPaymentRequestType
            {
                DoExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType
                {
                    Token = token,
                    PayerID = payerID
                }
            };

            // Create the request details object

            var paymentDetails = new PaymentDetailsType
            {
                InvoiceID = invoiceId,
                PaymentAction = paymentAction,
                PaymentActionSpecified = true,
                OrderTotal = new BasicAmountType
                {
                    currencyID = currencyCodeType,
                    Value = paymentAmount
                }
            };
            paymentDetails.ButtonSource = "HotcakesCommerce_Cart_EC_US";

            pp_request.DoExpressCheckoutPaymentRequestDetails.PaymentDetails = new[] {paymentDetails};
            return (DoExpressCheckoutPaymentResponseType) caller.Call("DoExpressCheckoutPayment", pp_request);
        }

        public DoVoidResponseType DoVoid(string authorizationId, string note)
        {
            var pp_request = new DoVoidRequestType
            {
                AuthorizationID = authorizationId,
                Note = note
            };
            return (DoVoidResponseType) caller.Call("DoVoid", pp_request);
        }

        public DoCaptureResponseType DoCapture(string authorizationId, string note, string value,
            CurrencyCodeType currencyId, string invoiceId)
        {
            var pp_request = new DoCaptureRequestType
            {
                AuthorizationID = authorizationId,
                Note = note,
                Amount = new BasicAmountType
                {
                    Value = value,
                    currencyID = currencyId
                },
                InvoiceID = invoiceId
            };
            return (DoCaptureResponseType) caller.Call("DoCapture", pp_request);
        }
    }
}