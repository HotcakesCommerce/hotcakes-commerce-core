#region License

// Distributed under the MIT License
// ============================================================
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

namespace Hotcakes.PaypalWebServices
{
    public class PayPalConstants
    {
        //Payments Status
        public const string PAYMENT_STATUS_COMPLETED = "COMPLETED";
        public const string PAYMENT_STATUS_COMPLETED_DESCRIPTION = "PayPal Express Payment Captured Successfully";
        public const string PAYMENT_STATUS_Pending = "PENDING";
        public const string PAYMENT_STATUS_Pending_DESCRIPTION = "PayPal Express Payment PENDING";
        public const string PAYMENT_STATUS_DENIED = "DENIED";
        public const string PAYMENT_STATUS_DENIED_DESCRIPTION = "PayPal Express Payment DENIED";
        public const string PAYMENT_STATUS_ERROR_DESCRIPTION =
            "An error occurred while trying to capture your PayPal payment.";
        public const string PAYMENT_CHARGE_ERROR = "An error occurred while trying to charge your PayPal payment.";
        public const string PAYMENT_ERROR = "Paypal Express Payment Charge Failed.";

        //Payment Authorize
        public const string PAYMENT_AUTHORIZE_SUCCESS = "PayPal Express Payment Authorized Successfully.";
        public const string PAYMENT_AUTHORIZE_FAILED = "PayPal Express Payment Authorization Failed.";


        //Payment Refund
        public const string PAYMENT_REFUND_SUCCESS = "PayPal Express Payment Refunded Successfully.";
        public const string PAYMENT_REFUND_FAILED = "Paypal Express Payment Refund Failed.";

        //Payment Void
        public const string PAYMENT_Void_SUCCESS = "PayPal Express Payment Voided Successfully.";
        public const string PAYMENT_Void_FAILED = "Paypal Express Payment Void Failed.";


        //Payment Mode
        public const string PAYMENT_MODE_AUTHORIZE = "AUTHORIZE";
        public const string PAYMENT_MODE_CAPTURE = "CAPTURE";

        //Template URL
        public const string LIVE_URL = "https://www.paypal.com/checkoutnow?token={0}";
        public const string SANDBOX_URL = "https://www.sandbox.paypal.com/checkoutnow?token={0}";
        
    }
}
