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
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hotcakes.PaypalWebServices;
using Hotcakes.Web.Geography;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;


namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class PayPalPaymentsPro : PaymentGateway
    {
        public PayPalPaymentsPro()
        {
            Settings = new PayPalPaymentsProSettings();
        }

        public override string Name
        {
            get { return "PayPal Payments Pro"; }
        }

        public override string Id
        {
            get { return "0B81046B-7A24-4512-8A6B-6C4C59D4C503"; }
        }

        public PayPalPaymentsProSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        public override void ProcessTransaction(Transaction t)
        {
            try
            {
                switch (t.Action)
                {
                    case ActionType.CreditCardHold:
                        Authorize(t);
                        break;
                    case ActionType.CreditCardCharge:
                        Charge(t);
                        break;
                    case ActionType.CreditCardCapture:
                        Capture(t);
                        break;
                    case ActionType.CreditCardRefund:
                        Refund(t);
                        break;
                    case ActionType.CreditCardVoid:
                        Void(t);
                        break;
                }
            }
            catch (Exception ex)
            {
                t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP_PPP_1001",
                    MessageType.Error));
                t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }

        protected override void Authorize(Transaction t)
        {
            var ppAPI = CreateRestPayPalApi();

            var OrderNumber = t.MerchantInvoiceNumber;
            // Solves Duplicate Order Number Problem
            OrderNumber = OrderNumber + Guid.NewGuid();

            HttpResponse response = Task.Run(() => ppAPI.doDirectCheckout(
                t.Amount.ToString("N", CultureInfo.InvariantCulture),
                t.Customer.LastName,
                t.Customer.FirstName,
                t.Customer.ShipLastName,
                t.Customer.ShipFirstName,
                t.Customer.Street,
                string.Empty,
                t.Customer.City,
                t.Customer.RegionName,
                t.Customer.PostalCode,
                t.Customer.CountryData.IsoCode,
                t.Card.CardTypeName,
                t.Card.CardNumber,
                t.Card.SecurityCode,
                t.Card.ExpirationMonth,
                t.Card.ExpirationYear,
                PayPalConstants.PAYMENT_MODE_AUTHORIZE,
                t.Customer.ShipStreet,
                string.Empty,
                t.Customer.ShipCity,
                t.Customer.ShipRegionName,
                t.Customer.ShipPostalCode,
                t.Customer.ShipCountryData.IsoCode,
                OrderNumber,
                Settings.Currency)).GetAwaiter().GetResult();

            response = Task.Run(() => ppAPI.AuthorizeOrder(response.Result<Order>().Id)).GetAwaiter().GetResult();

            if (Settings.DebugMode)
            {
                var posted = new StringBuilder();
                posted.Append("Amount=" + t.Amount.ToString("N"));
                posted.Append(", Currency=" + Settings.Currency);
                posted.Append(", LastName=" + t.Customer.LastName);
                posted.Append(", FirstName=" + t.Customer.FirstName);
                posted.Append(", ShipLastName=" + t.Customer.ShipLastName);
                posted.Append(", ShipFirstName=" + t.Customer.ShipFirstName);
                posted.Append(", Street=" + t.Customer.Street);
                posted.Append(", City=" + t.Customer.Street);
                posted.Append(", Region=" + t.Customer.RegionName);
                posted.Append(", PostalCode=" + t.Customer.PostalCode);
                //posted.Append(", CountryName=" + ConvertCountry(t.Customer.CountryData));
                posted.Append(", CardTypeName=" + t.Card.CardTypeName);
                posted.Append(", CardNumber=**********" + t.Card.CardNumberLast4Digits);
                posted.Append(", CardExpMonth=" + t.Card.ExpirationMonth);
                posted.Append(", CardExpYear=" + t.Card.ExpirationYear);
                posted.Append(", OrderNumber=" + OrderNumber);

                t.Result.Messages.Add(new Message(posted.ToString(), "DEBUG", MessageType.Information));
            }

            if (response.StatusCode == HttpStatusCode.Created )
            {
                var paymentInfo = response.Result<Order>().PurchaseUnits[0].Payments.Authorizations[0];
                t.Result.ReferenceNumber = paymentInfo.Id;
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Payment Authorization Failed.", string.Empty,
                    MessageType.Warning));
                t.Result.Succeeded = false;
            }
        }

        protected override void Charge(Transaction t)
        {
            var ppAPI = CreateRestPayPalApi();

            var OrderNumber = t.MerchantInvoiceNumber;
            // Solves Duplicate Order Number Problem
            OrderNumber = OrderNumber + Guid.NewGuid();
            
            HttpResponse response = Task.Run(() => ppAPI.doDirectCheckout(
                t.Amount.ToString("N", CultureInfo.InvariantCulture),
                t.Customer.LastName,
                t.Customer.FirstName,
                t.Customer.ShipLastName,
                t.Customer.ShipFirstName,
                t.Customer.Street,
                string.Empty,
                t.Customer.City,
                t.Customer.RegionName,
                t.Customer.PostalCode,
                t.Customer.CountryData.IsoCode,
                t.Card.CardTypeName,
                t.Card.CardNumber,
                t.Card.SecurityCode,
                t.Card.ExpirationMonth,
                t.Card.ExpirationYear,
                PayPalConstants.PAYMENT_MODE_CAPTURE,
                t.Customer.ShipStreet,
                string.Empty,
                t.Customer.ShipCity,
                t.Customer.ShipRegionName,
                t.Customer.ShipPostalCode,
                t.Customer.ShipCountryData.IsoCode,
                OrderNumber,
                Settings.Currency)).GetAwaiter().GetResult();

            response = Task.Run(() => ppAPI.CaptureOrder(response.Result<Order>().Id)).GetAwaiter().GetResult();
            
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var paymentInfo = response.Result<Order>().PurchaseUnits[0].Payments.Captures[0];
                t.Result.ReferenceNumber = paymentInfo.Id;
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Charge Failed.", string.Empty, MessageType.Warning));
                t.Result.Succeeded = false;
            }
        }

        protected override void Capture(Transaction t)
        {
            try
            {
                var ppAPI = CreateRestPayPalApi();

                var OrderNumber = t.MerchantInvoiceNumber + Guid.NewGuid();

                var captureResponse = Task.Run(() => ppAPI.CaptureAuthorizedOrder(t.PreviousTransactionNumber,
                    t.Amount.ToString("N", CultureInfo.InvariantCulture),
                    Settings.Currency,
                    OrderNumber)).GetAwaiter().GetResult();

                if (captureResponse.StatusCode == HttpStatusCode.OK || captureResponse.StatusCode == HttpStatusCode.Created)
                {
                    var capturedInfo = captureResponse.Result<PayPalCheckoutSdk.Payments.Capture>();
                    t.Result.ReferenceNumber = capturedInfo.Id;
                    t.Result.Succeeded = true;

                    switch (capturedInfo.Status)
                    {
                        case PayPalConstants.PAYMENT_STATUS_Pending:
                            t.Result.Succeeded = true;
                            t.Result.ResponseCode = PayPalConstants.PAYMENT_STATUS_Pending;
                            t.Result.ResponseCodeDescription = PayPalConstants.PAYMENT_STATUS_Pending_DESCRIPTION;
                            t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_Pending_DESCRIPTION, "OK",
                                MessageType.Information));
                            break;
                        case PayPalConstants.PAYMENT_STATUS_DENIED:
                            t.Result.Succeeded = true;
                            t.Result.ResponseCode = PayPalConstants.PAYMENT_STATUS_DENIED;
                            t.Result.ResponseCodeDescription = PayPalConstants.PAYMENT_STATUS_DENIED_DESCRIPTION;
                            t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_DENIED_DESCRIPTION, "OK",
                                MessageType.Information));
                            break;
                        case PayPalConstants.PAYMENT_STATUS_COMPLETED:
                            t.Result.Succeeded = true;
                            t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_COMPLETED_DESCRIPTION, "OK",
                                MessageType.Information));
                            break;
                        default:
                            t.Result.Messages.Add(
                                new Message(
                                    PayPalConstants.PAYMENT_STATUS_ERROR_DESCRIPTION,
                                    string.Empty, MessageType.Information));
                            t.Result.Succeeded = false;
                            break;
                    }
                }
                else
                {
                    t.Result.Messages.Add(new Message("Paypal Payment Capture Failed.", string.Empty, MessageType.Warning));
                    t.Result.Succeeded = false;
                }
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
        }

        protected override void Refund(Transaction t)
        {
            try
            {
                var ppAPI = CreateRestPayPalApi();

                if (t.PreviousTransactionNumber != null)
                {
                    var refundType = string.Empty;
                    //per paypal's request, the refund type should always be set to partial

                    var refundResponse = Task.Run(() => ppAPI.CapturesRefund(
                        t.PreviousTransactionNumber,
                        t.Amount.ToString("N", CultureInfo.InvariantCulture),
                        Settings.Currency)).GetAwaiter().GetResult();
                    if (refundResponse.StatusCode == HttpStatusCode.Created)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_REFUND_SUCCESS, "OK",
                            MessageType.Information));
                    }
                    else
                    {
                        t.Result.Succeeded = false;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_REFUND_FAILED, string.Empty,
                            MessageType.Error));
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
        }

        protected override void Void(Transaction t)
        {
            try
            {
                var ppAPI = CreateRestPayPalApi();

                if (t.PreviousTransactionNumber != null)
                {
                    var voidResponse = Task.Run(() => ppAPI.VoidAuthorizedPayment(t.PreviousTransactionNumber)).GetAwaiter().GetResult();
                    if (voidResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_Void_SUCCESS, "OK",
                            MessageType.Information));
                    }
                    else
                    {
                        t.Result.Succeeded = false;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_Void_FAILED, string.Empty,
                            MessageType.Error));
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
        }

        private RestPayPalApi CreateRestPayPalApi()
        {
            return new RestPayPalApi(Settings.PayPalClientId,
                Settings.PayPalSecret, Settings.PayPalMode);
        }


        private void HandleException(Transaction t, Exception ex)
        {
            t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP_PPP_1001", MessageType.Error));
            t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
            t.Result.Succeeded = false;
        }
    }
}