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
using Stripe;

namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class StripeProcessor : PaymentGateway
    {
        public StripeProcessor()
        {
            Settings = new StripeSettings();
        }

        public override string Name
        {
            get { return "Stripe"; }
        }

        public override string Id
        {
            get { return "15011DF5-13DA-42BE-9DFF-31C71ED64D4A"; }
        }

        public StripeSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        /// <summary>
        ///     Creates the charge.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="capture">
        ///     Whether or not to immediately capture the charge. When false, the charge issues an
        ///     authorization(or pre-authorization), and will need to be captured later
        /// </param>
        private void CreateCharge(Transaction t, bool capture)
        {
            StripeConfiguration.SetApiKey(Settings.StripeApiKey);

            var chargeOptions = new StripeChargeCreateOptions();

            // always set these properties
            chargeOptions.Amount = (int) (t.Amount*100);
            chargeOptions.Currency = Settings.CurrencyCode;

            // set this if you want to
            chargeOptions.Capture = capture;
            chargeOptions.Currency = Settings.CurrencyCode;
            chargeOptions.Description = t.MerchantDescription;

            chargeOptions.Source = new StripeSourceOptions();

            // set these properties if using a card
            chargeOptions.Source.Number = t.Card.CardNumber;
            chargeOptions.Source.ExpirationYear = t.Card.ExpirationYear.ToString();
            chargeOptions.Source.ExpirationMonth = t.Card.ExpirationMonthPadded;
            //myCharge.CardAddressCountry = "US";             // optional
            if (t.Customer.Street.Length > 0)
                chargeOptions.Source.AddressLine1 = t.Customer.Street; // optional
            //myCharge.CardAddressLine2 = "Apt 24";           // optional
            //myCharge.CardAddressState = "NC";               // optional
            if (t.Customer.PostalCode.Length > 0)
                chargeOptions.Source.AddressZip = t.Customer.PostalCode; // optional
            chargeOptions.Source.Name = t.Card.CardHolderName; // optional
            if (!string.IsNullOrEmpty(t.Card.SecurityCode))
            {
                chargeOptions.Source.Cvc = t.Card.SecurityCode; // optional
            }

            // set this property if using a customer
            //myCharge.CustomerId = *customerId*;

            // set this property if using a token
            //myCharge.TokenId = *tokenId*;

            var chargeService = new StripeChargeService();

            var stripeCharge = chargeService.Create(chargeOptions);

            if (stripeCharge.Id.Length > 0 && stripeCharge.Amount > 0)
            {
                t.Result.Succeeded = true;
                t.Result.ReferenceNumber = stripeCharge.Id;
            }
            else
            {
                t.Result.Succeeded = false;
                t.Result.ResponseCode = "FAIL";
                t.Result.ResponseCodeDescription = "Stripe Failure";
            }
        }

        private void CreateRefund(Transaction t)
        {
            StripeConfiguration.SetApiKey(Settings.StripeApiKey);

            var refundService = new StripeRefundService();

            var refundOptions = new StripeRefundCreateOptions();
            refundOptions.Amount = (int) (t.Amount*100);

            var refund = refundService.Create(t.PreviousTransactionNumber, refundOptions);
            if (refund.Id.Length > 0)
            {
                t.Result.Succeeded = true;
                t.Result.ReferenceNumber = refund.Id;
            }
            else
            {
                t.Result.Succeeded = false;
                t.Result.ResponseCode = "FAIL";
                t.Result.ResponseCodeDescription = "Stripe Failure";
            }
        }

        private void CaptureCharge(Transaction t)
        {
            StripeConfiguration.SetApiKey(Settings.StripeApiKey);

            var chargeService = new StripeChargeService();

            var stripeCharge = chargeService.Capture(t.PreviousTransactionNumber, (int) (t.Amount*100));

            if (stripeCharge.Id.Length > 0 && stripeCharge.Amount > 0)
            {
                t.Result.Succeeded = true;
                t.Result.ReferenceNumber = stripeCharge.Id;
            }
            else
            {
                t.Result.Succeeded = false;
                t.Result.ResponseCode = "FAIL";
                t.Result.ResponseCodeDescription = "Stripe Failure";
            }
        }

        public override void ProcessTransaction(Transaction t)
        {
            try
            {
                switch (t.Action)
                {
                    case ActionType.CreditCardCapture:
                        CaptureCharge(t);
                        break;
                    case ActionType.CreditCardCharge:
                        CreateCharge(t, true);
                        break;
                    case ActionType.CreditCardHold:
                        CreateCharge(t, false);
                        break;
                    case ActionType.CreditCardRefund:
                        CreateRefund(t);
                        break;
                    case ActionType.CreditCardVoid:
                        CreateRefund(t);
                        break;
                }
            }
            catch (StripeException stripeEx)
            {
                t.Result.Messages.Add(new Message("Stripe Error: " + stripeEx.StripeError.Message,
                    "STRIPE" + stripeEx.StripeError.Code, MessageType.Error));
                t.Result.Succeeded = false;
            }
            catch (Exception ex)
            {
                t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP_SP_1001",
                    MessageType.Error));
                t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }
    }
}