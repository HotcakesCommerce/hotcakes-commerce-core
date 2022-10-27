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
using System.Threading.Tasks;
using com.paypal.soap.api;
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

                    PaymentActionCodeType mode = PaymentActionCodeType.Authorization;
                    if (!app.CurrentStore.Settings.PayPal.ExpressAuthorizeOnly)
                    {
                        mode = PaymentActionCodeType.Sale;
                    }

                    var paymentResponse = Task.Run(() => ppAPI.AuthorizeOrder(t.PreviousTransactionNumber)).GetAwaiter().GetResult();
                   

                    if (paymentResponse.StatusCode == HttpStatusCode.Created)
                    {
                        var paymentInfo = paymentResponse.Result<Order>().PurchaseUnits[0].Payments.Authorizations[0];

                        t.Result.Succeeded = true;
                        t.Result.ReferenceNumber = paymentInfo.Id;
                        t.Result.ResponseCode = "OK";
                        t.Result.ResponseCodeDescription = "PayPal Express Payment Authorized Successfully.";
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message("PayPal Express Payment Authorization Failed.", string.Empty,
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
                    t.Result.ReferenceNumber = captureResponse.Result<PayPalCheckoutSdk.Payments.Capture>().Id;

                    if (captureResponse.Result<PayPalCheckoutSdk.Payments.Capture>().Status == "PENDING")
                    {
                        t.Result.Succeeded = true;
                        t.Result.ResponseCode = "PENDING";
                        t.Result.ResponseCodeDescription = "PayPal Express Payment PENDING";
                        t.Result.Messages.Add(new Message("Paypal Express Payment PENDING.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    if (captureResponse.Result<PayPalCheckoutSdk.Payments.Capture>().Status == "DENIED")
                    {
                        t.Result.Succeeded = true;
                        t.Result.ResponseCode = "PENDING";
                        t.Result.ResponseCodeDescription = "PayPal Express Payment DENIED";
                        t.Result.Messages.Add(new Message("Paypal Express Payment DENIED.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    if (captureResponse.Result<PayPalCheckoutSdk.Payments.Capture>().Status == "COMPLETED")
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message("PayPal Express Payment Captured Successfully.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message("An error occurred while trying to capture your PayPal payment.",
                        string.Empty, MessageType.Error));
                    return false;
                }
                t.Result.Succeeded = false;
                t.Result.Messages.Add(new Message("Paypal Express Payment Charge Failed.", string.Empty,
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

                    if (paymentInfo.Status == "COMPLETED")
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message("PayPal Express Payment Charged Successfully.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    if (paymentInfo.Status == "PENDING")
                    {
                        t.Result.Succeeded = true;
                        t.Result.ResponseCode = "PENDING";
                        t.Result.ResponseCodeDescription = "PayPal Express Payment PENDING";
                        t.Result.Messages.Add(new Message("Paypal Express Payment PENDING.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message("An error occurred while trying to charge your PayPal payment.",
                        string.Empty, MessageType.Error));
                    return false;
                }
                t.Result.Succeeded = false;
                t.Result.Messages.Add(new Message("Paypal Express Payment Charge Failed.", string.Empty,
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
                    refundType = "Partial";
                    
                    var refundResponse = Task.Run(() => ppAPI.CapturesRefund(
                        t.PreviousTransactionNumber,
                        t.Amount.ToString("N", CultureInfo.InvariantCulture),
                        app.CurrentStore.Settings.PayPal.Currency)).GetAwaiter().GetResult();
                    if (refundResponse.StatusCode == HttpStatusCode.Created)
                    {
                        t.Result.Succeeded = true;
                        t.Result.Messages.Add(new Message("PayPal Express Payment Refunded Successfully.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message("Paypal Express Payment Refund Failed.", string.Empty,
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
                        t.Result.Messages.Add(new Message("PayPal Express Payment Voided Successfully.", "OK",
                            MessageType.Information));
                        return true;
                    }
                    t.Result.Succeeded = false;
                    t.Result.Messages.Add(new Message("Paypal Express Payment Void Failed.", string.Empty,
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