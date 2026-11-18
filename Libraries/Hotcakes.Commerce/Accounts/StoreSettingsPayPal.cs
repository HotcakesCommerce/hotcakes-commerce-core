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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Provides PayPal settings owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Values are store-scoped and staged through the parent façade. Credential properties are encrypted by the parent façade. The misspelled <c>ClienId</c> member is retained for public compatibility.</remarks>
    [Serializable]
    public class StoreSettingsPayPal
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new PayPal settings façade.</summary>
        /// <param name="s">The owning store settings façade.</param>
        public StoreSettingsPayPal(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets the PayPal username.</summary>
        /// <value>The encrypted <c>PaypalUserName</c> setting.</value>
        public string UserName
        {
            get { return parent.GetPropEncrypted("PaypalUserName"); }
            set { parent.SetPropEncrypted("PaypalUserName", value); }
        }

        /// <summary>Gets or sets the PayPal password.</summary>
        /// <value>The encrypted sensitive <c>PaypalPassword</c> setting.</value>
        public string Password
        {
            get { return parent.GetPropEncrypted("PaypalPassword"); }
            set { parent.SetPropEncrypted("PaypalPassword", value); }
        }

        /// <summary>Gets or sets the PayPal client identifier.</summary>
        /// <value>The encrypted <c>PaypalClientId</c> setting. The member spelling <c>ClienId</c> is retained for compatibility.</value>
        public string ClienId
        {
            get { return parent.GetPropEncrypted("PaypalClientId"); }
            set { parent.SetPropEncrypted("PaypalClientId", value); }
        }

        /// <summary>Gets or sets the PayPal client secret.</summary>
        /// <value>The encrypted sensitive <c>PaypalSecret</c> setting.</value>
        public string Secret
        {
            get { return parent.GetPropEncrypted("PaypalSecret"); }
            set { parent.SetPropEncrypted("PaypalSecret", value); }
        }

        /// <summary>Gets or sets the PayPal API signature.</summary>
        /// <value>The encrypted sensitive <c>PaypalSignature</c> setting.</value>
        public string Signature
        {
            get { return parent.GetPropEncrypted("PaypalSignature"); }
            set { parent.SetPropEncrypted("PaypalSignature", value); }
        }

        /// <summary>Gets or sets whether PayPal Express authorizes without capturing payment.</summary>
        /// <value>The <c>PaypalExpressAuthorizeOnly</c> setting; unset values read as <see langword="false"/>.</value>
        public bool ExpressAuthorizeOnly
        {
            get { return parent.GetPropBool("PaypalExpressAuthorizeOnly"); }
            set { parent.SetProp("PaypalExpressAuthorizeOnly", value); }
        }

        /// <summary>Gets or sets whether unconfirmed addresses are allowed.</summary>
        /// <value>The <c>PaypalAllowUnconfirmedAddresses</c> setting; unset values read as <see langword="false"/>.</value>
        public bool AllowUnconfirmedAddresses
        {
            get { return parent.GetPropBool("PaypalAllowUnconfirmedAddresses"); }
            set { parent.SetProp("PaypalAllowUnconfirmedAddresses", value); }
        }

        /// <summary>Gets or sets whether customers must use a PayPal account.</summary>
        /// <value>The <c>RequirePayPalAccount</c> setting; default is <see langword="true"/>.</value>
        public bool RequirePayPalAccount
        {
            get { return parent.GetPropBoolWithDefault("RequirePayPalAccount", true); }
            set { parent.SetProp("RequirePayPalAccount", value); }
        }

        /// <summary>Gets or sets the PayPal operating mode.</summary>
        /// <value>The <c>PaypalMode</c> setting.</value>
        public string Mode
        {
            get { return parent.GetProp("PaypalMode"); }
            set { parent.SetProp("PaypalMode", value); }
        }

        /// <summary>Gets or sets the PayPal currency code.</summary>
        /// <value>The <c>PaypalCurrency</c> setting.</value>
        public string Currency
        {
            get { return parent.GetProp("PaypalCurrency"); }
            set { parent.SetProp("PaypalCurrency", value); }
        }

        /// <summary>Gets or sets the email used for PayPal fast signup.</summary>
        /// <value>The <c>PayPalFastSignupEmail</c> setting.</value>
        public string FastSignupEmail
        {
            get { return parent.GetProp("PayPalFastSignupEmail"); }
            set { parent.SetProp("PayPalFastSignupEmail", value); }
        }

        /// <summary>Gets or sets whether PayPal omits the order-total breakdown.</summary>
        /// <value>The <c>PayPalNoTotalBreakdown</c> setting; unset values read as <see langword="false"/>.</value>
        public bool NoOrderTotalBreakdown
        {
            get { return parent.GetPropBool("PayPalNoTotalBreakdown"); }
            set { parent.SetProp("PayPalNoTotalBreakdown", value); }
        }
    }
}
