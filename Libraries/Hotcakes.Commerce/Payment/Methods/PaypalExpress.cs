#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    [Serializable]
    public class PaypalExpress : PaymentMethod
    {
        public override string MethodId
        {
            get { return Id(); }
        }

        public override string MethodName
        {
            get { return "PayPalExpress"; }
        }

        public override int SortIndex
        {
            get { return 200; }
        }

        public override bool PayBeforePlacement
        {
            get { return true; }
        }

        public static string Id()
        {
            return PaymentMethods.PaypalExpressId;
        }

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