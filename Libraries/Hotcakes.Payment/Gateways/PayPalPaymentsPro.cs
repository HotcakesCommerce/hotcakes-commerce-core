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
using System.Globalization;
using System.Text;
using com.paypal.sdk.profiles;
using com.paypal.soap.api;
using Hotcakes.PaypalWebServices;
using Hotcakes.Web.Geography;

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
            var ppAPI = CreatePaypalAPI();

            var OrderNumber = t.MerchantInvoiceNumber;
            // Solves Duplicate Order Number Problem
            OrderNumber = OrderNumber + Guid.NewGuid();

            var authResponse = ppAPI.DoDirectPayment(
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
                ConvertCountry(t.Customer.CountryData),
                t.Card.CardTypeName,
                t.Card.CardNumber,
                t.Card.SecurityCode,
                t.Card.ExpirationMonth,
                t.Card.ExpirationYear,
                PaymentActionCodeType.Authorization,
                t.Customer.IpAddress,
                t.Customer.ShipStreet,
                string.Empty,
                t.Customer.ShipCity,
                t.Customer.ShipRegionName,
                t.Customer.ShipPostalCode,
                ConvertCountry(t.Customer.ShipCountryData),
                OrderNumber,
                ConvertCurrency(Settings.Currency));

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
                posted.Append(", CountryName=" + ConvertCountry(t.Customer.CountryData));
                posted.Append(", CardTypeName=" + t.Card.CardTypeName);
                posted.Append(", CardNumber=**********" + t.Card.CardNumberLast4Digits);
                posted.Append(", CardExpMonth=" + t.Card.ExpirationMonth);
                posted.Append(", CardExpYear=" + t.Card.ExpirationYear);
                posted.Append(", OrderNumber=" + OrderNumber);

                t.Result.Messages.Add(new Message(posted.ToString(), "DEBUG", MessageType.Information));
            }

            if (authResponse.Ack == AckCodeType.Success || authResponse.Ack == AckCodeType.SuccessWithWarning)
            {
                t.Result.ReferenceNumber = authResponse.TransactionID;
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Payment Authorization Failed.", string.Empty,
                    MessageType.Warning));
                foreach (var ppError in authResponse.Errors)
                {
                    t.Result.Messages.Add(new Message(ppError.LongMessage, ppError.ErrorCode, MessageType.Error));
                }
                t.Result.Succeeded = false;
            }
        }

        protected override void Charge(Transaction t)
        {
            var ppAPI = CreatePaypalAPI();

            var OrderNumber = t.MerchantInvoiceNumber;
            // Solves Duplicate Order Number Problem
            OrderNumber = OrderNumber + Guid.NewGuid();

            var chargeResponse = ppAPI.DoDirectPayment(
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
                ConvertCountry(t.Customer.CountryData),
                t.Card.CardTypeName,
                t.Card.CardNumber,
                t.Card.SecurityCode,
                t.Card.ExpirationMonth,
                t.Card.ExpirationYear,
                PaymentActionCodeType.Sale,
                t.Customer.IpAddress,
                t.Customer.ShipStreet,
                string.Empty,
                t.Customer.ShipCity,
                t.Customer.ShipRegionName,
                t.Customer.ShipPostalCode,
                ConvertCountry(t.Customer.ShipCountryData),
                OrderNumber,
                ConvertCurrency(Settings.Currency));

            if (chargeResponse.Ack == AckCodeType.Success || chargeResponse.Ack == AckCodeType.SuccessWithWarning)
            {
                t.Result.ReferenceNumber = chargeResponse.TransactionID;
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Charge Failed.", string.Empty, MessageType.Warning));
                foreach (var ppError in chargeResponse.Errors)
                {
                    t.Result.Messages.Add(new Message(ppError.LongMessage, ppError.ErrorCode, MessageType.Error));
                }
                t.Result.Succeeded = false;
            }
        }

        protected override void Capture(Transaction t)
        {
            var ppAPI = CreatePaypalAPI();

            var OrderNumber = t.MerchantInvoiceNumber;

            var captureResponse = ppAPI.DoCapture(t.PreviousTransactionNumber,
                "Thank you for your payment.",
                t.Amount.ToString("N", CultureInfo.InvariantCulture),
                ConvertCurrency(Settings.Currency),
                OrderNumber);

            if (captureResponse.Ack == AckCodeType.Success || captureResponse.Ack == AckCodeType.SuccessWithWarning)
            {
                t.Result.ReferenceNumber = captureResponse.DoCaptureResponseDetails.PaymentInfo.TransactionID;
                t.Result.Succeeded = true;

                switch (captureResponse.DoCaptureResponseDetails.PaymentInfo.PaymentStatus)
                {
                    case PaymentStatusCodeType.Pending:
                        t.Result.Messages.Add(new Message("PayPal Pro Payment Pending", string.Empty,
                            MessageType.Information));
                        t.Result.Succeeded = true;
                        break;
                    case PaymentStatusCodeType.InProgress:
                        t.Result.Messages.Add(new Message("PayPal Pro Payment In Progress", string.Empty,
                            MessageType.Information));
                        t.Result.Succeeded = true;
                        break;
                    case PaymentStatusCodeType.None:
                        t.Result.Messages.Add(new Message("PayPal Pro Payment No Status Yet", string.Empty,
                            MessageType.Information));
                        t.Result.Succeeded = true;
                        break;
                    case PaymentStatusCodeType.Processed:
                        t.Result.Messages.Add(new Message("PayPal Pro Charged Successfully", string.Empty,
                            MessageType.Information));
                        t.Result.Succeeded = true;
                        break;
                    case PaymentStatusCodeType.Completed:
                        t.Result.Messages.Add(new Message("PayPal Pro Charged Successfully", string.Empty,
                            MessageType.Information));
                        t.Result.Succeeded = true;
                        break;
                    default:
                        t.Result.Messages.Add(
                            new Message(
                                "An error occurred while attempting to capture this PayPal Payments Pro payment",
                                string.Empty, MessageType.Information));
                        t.Result.Succeeded = false;
                        break;
                }
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Payment Capture Failed.", string.Empty, MessageType.Warning));
                foreach (var ppError in captureResponse.Errors)
                {
                    t.Result.Messages.Add(new Message(ppError.LongMessage, ppError.ErrorCode, MessageType.Error));
                }
                t.Result.Succeeded = false;
            }
        }

        protected override void Refund(Transaction t)
        {
            var ppAPI = CreatePaypalAPI();

            //per paypal's request, the refund type should always be set to partial
            var refundType = "Partial";
            var refundResponse = ppAPI.RefundTransaction(
                t.PreviousTransactionNumber,
                refundType,
                t.Amount.ToString("N", CultureInfo.InvariantCulture),
                ConvertCurrency(Settings.Currency));

            if (refundResponse.Ack == AckCodeType.Success || refundResponse.Ack == AckCodeType.SuccessWithWarning)
            {
                t.Result.ReferenceNumber = refundResponse.RefundTransactionID;
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Payment Refund Failed.", string.Empty, MessageType.Warning));
                foreach (var ppError in refundResponse.Errors)
                {
                    t.Result.Messages.Add(new Message(ppError.LongMessage, ppError.ErrorCode, MessageType.Error));
                }
                t.Result.Succeeded = false;
            }
        }

        protected override void Void(Transaction t)
        {
            var ppAPI = CreatePaypalAPI();

            var voidResponse = ppAPI.DoVoid(t.PreviousTransactionNumber, "Transaction Voided");

            if (voidResponse.Ack == AckCodeType.Success || voidResponse.Ack == AckCodeType.SuccessWithWarning)
            {
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Paypal Payment Void Failed.", string.Empty, MessageType.Warning));
                foreach (var ppError in voidResponse.Errors)
                {
                    t.Result.Messages.Add(new Message(ppError.LongMessage, ppError.ErrorCode, MessageType.Error));
                }
                t.Result.Succeeded = false;
            }
        }

        private PayPalAPI CreatePaypalAPI()
        {
            var profile = ProfileFactory.createSignatureAPIProfile();
            if (profile != null)
            {
                profile.APIUsername = Settings.PayPalUserName;
                profile.APIPassword = Settings.PayPalPassword;
                profile.Subject = string.Empty;
                profile.Environment = Settings.PayPalMode;
                profile.APISignature = Settings.PayPalSignature;
            }
            else
            {
                throw new ArgumentException("Paypal com.paypal.sdk.profiles.ProfileFactory.CreateAPIProfile has failed.");
            }
            return new PayPalAPI(profile);
        }

        private CountryCodeType ConvertCountry(ICountry country)
        {
            var result = CountryCodeType.US;

            if (country == null)
                return result;

            if (Enum.IsDefined(typeof (CountryCodeType), country.IsoCode))
            {
                result = (CountryCodeType) Enum.Parse(typeof (CountryCodeType), country.IsoCode, true);
            }

            return result;
        }

        private CurrencyCodeType ConvertCurrency(string currency)
        {
            var currencyCode = CurrencyCodeType.USD;

            if (string.IsNullOrEmpty(currency)) return currencyCode;

            if (Enum.IsDefined(typeof (CurrencyCodeType), currency))
            {
                currencyCode = (CurrencyCodeType) Enum.Parse(typeof (CurrencyCodeType), currency, true);
            }

            return currencyCode;
        }
    }
}