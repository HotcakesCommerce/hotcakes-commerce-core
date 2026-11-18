#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Threading.Tasks;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;
using Hotcakes.PaypalWebServices;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Built-in PayPal Express payment method that invokes the PayPal REST order and capture APIs.
    /// API failures are recorded on the transaction result, including the implementation's exception details.
    /// </summary>
    [Serializable]
    public class PaypalExpress : PaymentMethod
    {
        /// <summary>Gets the stable PayPal Express method identifier.</summary>
        public override string MethodId
        {
            get { return Id(); }
        }

        /// <summary>Gets the serialized display name, <c>PayPalExpress</c>.</summary>
        public override string MethodName
        {
            get { return "PayPalExpress"; }
        }

        /// <summary>Gets the checkout ordering value, 200.</summary>
        public override int SortIndex
        {
            get { return 200; }
        }

        /// <summary>Gets <see langword="true"/> because this method pays before order placement.</summary>
        public override bool PayBeforePlacement
        {
            get { return true; }
        }

        /// <summary>Returns the stable identifier used by <see cref="PaymentMethods"/>.</summary>
        /// <returns>The PayPal Express method identifier.</returns>
        public static string Id()
        {
            return PaymentMethods.PaypalExpressId;
        }

        /// <summary>
        /// Authorizes the transaction's previous PayPal order and copies the authorization reference to the result.
        /// A transaction without a previous order is not authorized.
        /// </summary>
        /// <param name="t">The transaction to update.</param>
        /// <param name="app">The application providing the current store and PayPal configuration.</param>
        /// <returns><see langword="true"/> only when PayPal returns HTTP Created for the authorization.</returns>
        public bool Authorize(Transaction t, HotcakesApplication app)
        {
            try
            {
                var ppAPI = PaypalExpressUtilities.GetRestPaypalAPI(app.CurrentStore);

                if (t.PreviousTransactionNumber != null)
                {
                    var OrderNumber = t.MerchantInvoiceNumber + Guid.NewGuid();

                    var paymentResponse = Task.Run(() => ppAPI.AuthorizeOrder(t.PreviousTransactionNumber)).GetAwaiter().GetResult();
                   

                    if (paymentResponse.StatusCode == HttpStatusCode.Created)
                    {
                        var paymentInfo = paymentResponse.Result<Order>().PurchaseUnits[0].Payments.Authorizations[0];

                        t.Result.Succeeded = true;
                        t.Result.ReferenceNumber = paymentInfo.Id;
                        t.Result.ResponseCode = "OK";
                        t.Result.ResponseCodeDescription = PayPalConstants.PAYMENT_AUTHORIZE_SUCCESS;
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_AUTHORIZE_FAILED, string.Empty,
                        MessageType.Error));
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }

            return false;
        }

        /// <summary>
        /// Captures the transaction's previous authorization and records PayPal pending, denied, or completed status messages.
        /// Pending and denied statuses are retained as successful results to match the existing integration behavior.
        /// </summary>
        /// <param name="t">The transaction to update.</param>
        /// <param name="app">The application providing the current store and PayPal configuration.</param>
        /// <returns><see langword="true"/> for an accepted HTTP response with a recognized capture status.</returns>
        public bool Capture(Transaction t, HotcakesApplication app)
        {
            try
            {
                var ppAPI = PaypalExpressUtilities.GetRestPaypalAPI(app.CurrentStore);

                var OrderNumber = t.MerchantInvoiceNumber + Guid.NewGuid();

                var captureResponse = Task.Run(() => ppAPI.CaptureAuthorizedOrder(t.PreviousTransactionNumber,
                    t.Amount.ToString("N", CultureInfo.InvariantCulture),
                    app.CurrentStore.Settings.PayPal.Currency,
                    OrderNumber)).GetAwaiter().GetResult();

                if (captureResponse.StatusCode == HttpStatusCode.OK || captureResponse.StatusCode == HttpStatusCode.Created)
                {
                    var capturedInfo = captureResponse.Result<PayPalCheckoutSdk.Payments.Capture>();
                    t.Result.ReferenceNumber = capturedInfo.Id;

                    if (capturedInfo.Status == PayPalConstants.PAYMENT_STATUS_Pending)
                    {
                        t.Result.Succeeded = true;
                        t.Result.ResponseCode = PayPalConstants.PAYMENT_STATUS_Pending;
                        t.Result.ResponseCodeDescription = PayPalConstants.PAYMENT_STATUS_Pending_DESCRIPTION;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_Pending_DESCRIPTION, "OK",
                            MessageType.Information));
                        return true;
                    }
                    if (capturedInfo.Status == PayPalConstants.PAYMENT_STATUS_DENIED)
                    {
                        t.Result.Succeeded = true;
                        t.Result.ResponseCode = PayPalConstants.PAYMENT_STATUS_DENIED;
                        t.Result.ResponseCodeDescription = PayPalConstants.PAYMENT_STATUS_DENIED_DESCRIPTION;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_DENIED_DESCRIPTION, "OK",
                            MessageType.Information));
                        return true;
                    }
                    if (capturedInfo.Status == PayPalConstants.PAYMENT_STATUS_COMPLETED)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_COMPLETED_DESCRIPTION, "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_ERROR_DESCRIPTION,
                        string.Empty, MessageType.Error));
                    return false;
                }
                t.Result.Succeeded = false;
                t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_ERROR, string.Empty,
                    MessageType.Error));
                return false;
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
            return false;
        }

        /// <summary>
        /// Captures the transaction's existing PayPal order without a separate authorization.
        /// Completed and pending captures are successful according to the current integration semantics.
        /// </summary>
        /// <param name="t">The transaction to update.</param>
        /// <param name="app">The application providing the current store and PayPal configuration.</param>
        /// <returns><see langword="true"/> when PayPal returns HTTP OK or Created with a completed or pending capture.</returns>
        public bool Charge(Transaction t, HotcakesApplication app)
        {
            try
            {
                var ppAPI = PaypalExpressUtilities.GetRestPaypalAPI(app.CurrentStore);


                HttpResponse paymentResponse;
                //there was no authorization so we just need to do a direct sale
                
                paymentResponse = Task.Run(() => ppAPI.CaptureOrder(t.PreviousTransactionNumber)).GetAwaiter().GetResult();

                if (paymentResponse.StatusCode == HttpStatusCode.Created || paymentResponse.StatusCode == HttpStatusCode.OK)
                {
                    var paymentInfo = paymentResponse.Result<Order>().PurchaseUnits[0].Payments.Captures[0];

                    t.Result.ReferenceNumber = paymentInfo.Id;

                    if (paymentInfo.Status == PayPalConstants.PAYMENT_STATUS_COMPLETED)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_COMPLETED_DESCRIPTION, "OK",
                            MessageType.Information));
                        return true;
                    }
                    if (paymentInfo.Status == PayPalConstants.PAYMENT_STATUS_Pending)
                    {
                        t.Result.Succeeded = true;
                        t.Result.ResponseCode = PayPalConstants.PAYMENT_STATUS_Pending;
                        t.Result.ResponseCodeDescription = PayPalConstants.PAYMENT_STATUS_Pending_DESCRIPTION;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_STATUS_Pending_DESCRIPTION, "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_CHARGE_ERROR,
                        string.Empty, MessageType.Error));
                    return false;
                }
                t.Result.Succeeded = false;
                t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_ERROR, string.Empty,
                    MessageType.Error));
                
                return false;
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
            return false;
        }

        /// <summary>
        /// Requests a partial refund for the transaction's previous PayPal capture.
        /// </summary>
        /// <param name="t">The transaction containing the refund amount and previous reference.</param>
        /// <param name="app">The application providing the current store and PayPal configuration.</param>
        /// <returns><see langword="true"/> only when PayPal returns HTTP Created.</returns>
        public bool Refund(Transaction t, HotcakesApplication app)
        {
            try
            {
                var ppAPI = PaypalExpressUtilities.GetRestPaypalAPI(app.CurrentStore);

                if (t.PreviousTransactionNumber != null)
                {
                    var refundType = string.Empty;
                    //per paypal's request, the refund type should always be set to partial

                    var refundResponse = Task.Run(() => ppAPI.CapturesRefund(
                        t.PreviousTransactionNumber,
                        t.Amount.ToString("N", CultureInfo.InvariantCulture),
                        app.CurrentStore.Settings.PayPal.Currency)).GetAwaiter().GetResult();
                    if (refundResponse.StatusCode == HttpStatusCode.Created)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_REFUND_SUCCESS, "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_REFUND_FAILED, string.Empty,
                        MessageType.Error));
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
            return false;
        }

        /// <summary>
        /// Voids the transaction's previous PayPal authorization.
        /// </summary>
        /// <param name="t">The transaction to update.</param>
        /// <param name="app">The application providing the current store and PayPal configuration.</param>
        /// <returns><see langword="true"/> only when PayPal returns HTTP NoContent.</returns>
        public bool Void(Transaction t, HotcakesApplication app)
        {
            try
            {
                var ppAPI = PaypalExpressUtilities.GetRestPaypalAPI(app.CurrentStore);

                if (t.PreviousTransactionNumber != null)
                {
                    var voidResponse = Task.Run(() => ppAPI.VoidAuthorizedPayment(t.PreviousTransactionNumber)).GetAwaiter().GetResult();
                    if (voidResponse.StatusCode == HttpStatusCode.NoContent)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_Void_SUCCESS, "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message(PayPalConstants.PAYMENT_Void_FAILED, string.Empty,
                        MessageType.Error));
                    return false;
                }
            }
            catch (Exception ex)
            {
                HandleException(t, ex);
            }
            return false;
        }

        private void HandleException(Transaction t, Exception ex)
        {
            t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP_PPP_1001", MessageType.Error));
            t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
            t.Result.Succeeded = false;
        }
    }
}
