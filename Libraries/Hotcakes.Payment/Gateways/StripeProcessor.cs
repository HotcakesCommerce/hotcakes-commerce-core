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
        /// Creates the token required by Stripe in order to send credit card information to processed for a transaction
        /// </summary>
        /// <param name="t">The transaction object</param>
        /// <returns>A StripeToken object that can be associated with the charge that is sent to Stripe</returns>
        private Token CreateCardToken(Transaction t)
        {
            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = t.Card.CardNumber,
                    ExpMonth = t.Card.ExpirationMonthPadded,
                    ExpYear = t.Card.ExpirationYear.ToString(),
                    Cvc = t.Card.SecurityCode,
                    Name = t.Card.CardHolderName,
                    AddressLine1 = t.Customer.Street,
                    AddressZip = t.Customer.PostalCode
                },
            };
            var tokenService = new TokenService();
            var stripeToken = tokenService.Create(options);
            return stripeToken;
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
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var chargeOptions = new ChargeCreateOptions();

            // always set these properties
            chargeOptions.Amount = (int)(t.Amount * 100);
            chargeOptions.Currency = Settings.CurrencyCode;

            // set this if you want to
            chargeOptions.Capture = capture;
            chargeOptions.Currency = Settings.CurrencyCode;
            chargeOptions.Description = t.MerchantDescription;

            // set these properties if using a card
            //set the charge token
            var chargeToken = CreateCardToken(t);

            chargeOptions.Source = chargeToken.Id;

            var chargeService = new ChargeService();

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
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var refundService = new RefundService();

            var refundOptions = new RefundCreateOptions();
            refundOptions.Amount = (int)(t.Amount * 100);

            var refund = refundService.Create(refundOptions);
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
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var chargeService = new ChargeService();

            var stripeCharge = chargeService.Capture(t.Result.ReferenceNumber);

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

        public void CreatePaymentIntent(Transaction t)
        {
            StripeConfiguration.ApiKey = "sk_test_51KjFl3LWG7Wf1eHay7bIJnf3b8FmWopOjJVe9rN3APpo2jvvqj55DmVfGfcndqgRKCJGHYh2MHu1QR4MIstEubxq004U9HgTJN";
            var pmService = new PaymentMethodService();
            var pmOptions = new PaymentMethodCreateOptions()
            {
                Card = new PaymentMethodCardOptions()
                {
                    Cvc = t.Card.SecurityCode,
                    Number = t.Card.CardNumber,
                    ExpMonth = t.Card.ExpirationMonth,
                    ExpYear = t.Card.ExpirationYear
                },
                BillingDetails = new PaymentMethodBillingDetailsOptions()
                {
                    Address = new AddressOptions() { Line1 = t.Customer.Street, PostalCode = t.Customer.PostalCode }
                }

            };
            var pm = pmService.Create(pmOptions);

            var options = new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt64(t.Amount * 100),
                Currency = Settings.CurrencyCode,
                PaymentMethod = pm.Id,
                Confirm = true
            };

            var service = new PaymentIntentService();
            var pi = service.Create(options);
            if (pi.Status == "succeeded")
            {
                t.Result.Succeeded = true;
                t.Result.ReferenceNumber = pi.Id;
            }
            else
            {
                t.Result.Succeeded = false;
                t.Result.ResponseCode = pi.Status;
                t.Result.ResponseCodeDescription = "Stripe Failure";
            }
        }

        public class PaymentIntentRequestItem
        {
            public long TotalAmmount { get; set; }
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
                        CreatePaymentIntent(t);
                        break;
                    case ActionType.CreditCardHold:
                        CreatePaymentIntent(t);
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