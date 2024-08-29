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
using System.Text.RegularExpressions;
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
        /// Process the transaction object to use the PaymentIntents or Charges API depending on the value of the reference number 
        /// </summary>
        /// <param name="t">The Transaction object</param>
        /// <param name="capture">Defines if the charge will be captured automatically</param>
        private void ProcessCreate(Transaction t, bool capture)
        {
            var id = t.Result.ReferenceNumber;
            if (!string.IsNullOrEmpty(id) && IsPaymentIntent(id))
            {
                if (capture)
                {
                    CapturePaymentIntent(t);
                }
                else
                {
                    var paymentIntent = RetrievePaymentIntent(id);
                    if (paymentIntent.Status.Equals(PaymentIntentStatus.RequiresCapture))
                    {
                        t.Result.Succeeded = true;
                        t.Result.ReferenceNumber = id;
                    }
                    else
                    {
                        t.Result.Succeeded = false;
                        t.Result.ResponseCode = "FAIL";
                        t.Result.ResponseCodeDescription = "Stripe Failure";

                    }
                }
            }
            else
            {
                CreateCharge(t, capture);
            }
        }

        /// <summary>
        /// If the Id belongs to a PaymentIntent proceed to capture with the PaymentIntent API
        /// Else continue using Charges API
        /// </summary>
        /// <param name="t">The Transaction object</param>
        private void ProcessCapture(Transaction t)
        {
            var id = t.PreviousTransactionNumber;
            if (!string.IsNullOrEmpty(id) && IsPaymentIntent(id))
            {
                CapturePaymentIntent(t);
            }
            else
            {
                CaptureCharge(t);
            }
        }

        /// <summary>
        /// If the Id belongs to a PaymentIntent proceed to cancel with the PaymentIntent API
        /// Else continue using Charges API
        /// </summary>
        /// <param name="t">The Transaction object</param>
        private void ProcessVoid(Transaction t)
        {
            var id = t.PreviousTransactionNumber;
            if (!string.IsNullOrEmpty(id) && IsPaymentIntent(id))
            {
                CancelPaymentIntent(t);
            }
            else
            {
                CreateRefund(t);
            }
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

            // set these properties if using a card
            //set the charge token
            var chargeToken = CreateCardToken(t);

            var chargeOptions = new ChargeCreateOptions
            {
                // always set these properties
                Amount = (int)(t.Amount * 100),
                Currency = Settings.CurrencyCode,
                // set this if you want to
                Capture = capture,
                Description = t.MerchantDescription,
                Source = chargeToken.Id
            };

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

        /// <summary>
        /// Process Refund based on the transaction reference number type
        /// https://stripe.com/docs/api/refunds/object
        /// </summary>
        /// <param name="t">The Transaction Object</param>
        private void CreateRefund(Transaction t)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var refundService = new RefundService();

            var refundOptions = new RefundCreateOptions
            {
                Amount = (int)(t.Amount * 100)
            };

            if (IsPaymentIntent(t.PreviousTransactionNumber))
            {
                refundOptions.PaymentIntent = t.PreviousTransactionNumber;
            }
            else
            {
                refundOptions.Charge = t.PreviousTransactionNumber;
            }

            var refund = refundService.Create(refundOptions);
            if (refund.Status.Length > 0)
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

            var stripeCharge = chargeService.Capture(t.PreviousTransactionNumber);

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

        private void CapturePaymentIntent(Transaction t)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var paymentIntentService = new PaymentIntentService();
            if (string.IsNullOrEmpty(t.PreviousTransactionNumber))
            {
                t.PreviousTransactionNumber = t.Result.ReferenceNumber;
            }

            var stripeCapture = paymentIntentService.Capture(t.PreviousTransactionNumber);

            if (stripeCapture.Id.Length > 0 && stripeCapture.Amount > 0)
            {
                t.Result.Succeeded = true;
                t.Result.ReferenceNumber = stripeCapture.Id;
            }
            else
            {
                t.Result.Succeeded = false;
                t.Result.ResponseCode = "FAIL";
                t.Result.ResponseCodeDescription = "Stripe Failure";
            }
        }

        /// <summary>
        /// Cancel the PaymentIntent
        /// </summary>
        /// <param name="t">The transaction object</param>
        private void CancelPaymentIntent(Transaction t)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var service = new PaymentIntentService();
            var pi = service.Cancel(t.Result.ReferenceNumber);
            if (pi.Status.Equals(PaymentIntentStatus.Canceled))
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

        /// <summary>
        /// Create a PaymentIntent
        /// </summary>
        /// <param name="request">The payment intent request item</param>
        /// <returns></returns>
        public PaymentIntent CreatePaymentIntent(PaymentIntentRequestItem request)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var options = new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt64(request.TotalAmount * 100),
                Currency = Settings.CurrencyCode,
                CaptureMethod = CaptureMethod.Manual,
            };

            var service = new PaymentIntentService();
            return service.Create(options);
        }

        /// <summary>
        /// Get a PaymentIntent by Id
        /// </summary>
        /// <param name="id">The PaymentIntent Id</param>
        /// <returns>Return the PaymentIntent object</returns>
        public PaymentIntent RetrievePaymentIntent(string id)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var service = new PaymentIntentService();
            return service.Get(id);
        }

        /// <summary>
        /// Create a payment method using the PaymentIntent API
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="cvc"></param>
        /// <param name="expMonth"></param>
        /// <param name="expYear"></param>
        /// <returns>The payment method object</returns>
        public PaymentMethod CreatePaymentMethod(string cardNumber, string cvc, int expMonth, int expYear)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var options = new PaymentMethodCreateOptions
            {
                Type = PaymentMethodType.Card,
                Card = new PaymentMethodCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Cvc = cvc,
                },
            };
            var service = new PaymentMethodService();
            return service.Create(options);
        }

        /// <summary>
        /// Attach a PaymentMethod to a PaymentIntent
        /// </summary>
        /// <param name="paymentMethodId">The Payment Method Id</param>
        /// <param name="paymentIntentId">The Payment Intent Id</param>
        /// <returns>The updated PaymentIntent object</returns>
        public PaymentIntent AttachPaymentMethod(string paymentMethodId, string paymentIntentId)
        {
            StripeConfiguration.ApiKey = Settings.StripeApiKey;

            var options = new PaymentIntentUpdateOptions
            {
                PaymentMethod = paymentMethodId
            };
            var service = new PaymentIntentService();
            service.Update(paymentIntentId, options);
            var result = service.Confirm(paymentIntentId);
            return result;
        }

        public class PaymentIntentRequestItem
        {
            public decimal TotalAmount { get; set; }
        }

        public bool IsPaymentIntent(string id)
        {
            return id.Substring(0, 2).Equals("pi");
        }



        public override void ProcessTransaction(Transaction t)
        {
            try
            {
                switch (t.Action)
                {
                    case ActionType.CreditCardCapture:
                        ProcessCapture(t);
                        break;
                    case ActionType.CreditCardCharge:
                        ProcessCreate(t, true);
                        break;
                    case ActionType.CreditCardHold:
                        ProcessCreate(t, false);
                        break;
                    case ActionType.CreditCardRefund:
                        CreateRefund(t);
                        break;
                    case ActionType.CreditCardVoid:
                        ProcessVoid(t);
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

    #region Constants
    public static class PaymentIntentStatus
    {
        public const string Succeded = "succeded";
        public const string Canceled = "canceled";
        public const string RequiresCapture = "requires_capture";
        public const string RequiresPaymentMethod = "requires_payment_method";
    }

    public static class PaymentMethodType
    {
        public const string Card = "card";
    }

    public static class CaptureMethod
    {
        public const string Manual = "manual";
    }


    #endregion

}