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
using Hotcakes.Commerce.Accounts;
using Hotcakes.PaypalWebServices;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Utilities
{
    public class PaypalExpressUtilities
    {
        public static PayPalAPI GetPaypalAPI(Store currentStore)
        {
            var APIProfile
                = CreateAPIProfile(currentStore.Settings.PayPal.UserName,
                    currentStore.Settings.PayPal.Password,
                    currentStore.Settings.PayPal.Signature,
                    currentStore.Settings.PayPal.FastSignupEmail,
                    currentStore.Settings.PayPal.Mode);

            return new PayPalAPI(APIProfile);
        }

        private static IAPIProfile CreateAPIProfile(string PayPalUserName, string PayPalPassword, string PayPalSignature,
            string subject, string mode)
        {
            try
            {
                var profile = ProfileFactory.createSignatureAPIProfile();

                if (profile != null)
                {
                    profile.Environment = mode;

                    EventLog.LogEvent("PayPal Express Get Api", "Getting Environment " + mode,
                        EventLogSeverity.Information);

                    profile.APIUsername = PayPalUserName;
                    profile.APIPassword = PayPalPassword;
                    profile.APISignature = PayPalSignature;
                    profile.Subject = subject;
                }
                else
                {
                    EventLog.LogEvent("Paypal API",
                        "Paypal com.paypal.sdk.profiles.ProfileFactory.CreateAPIProfile has failed.",
                        EventLogSeverity.Error);
                }
                return profile;
            }
            catch (Exception ex)
            {
                EventLog.LogEvent("PayPal Utilities", ex.Message + " | " + ex.StackTrace, EventLogSeverity.Warning);
            }
            return null;
        }
    }
}