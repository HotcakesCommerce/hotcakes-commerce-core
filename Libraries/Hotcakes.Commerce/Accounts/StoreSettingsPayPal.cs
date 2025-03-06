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

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreSettingsPayPal
    {
        private readonly StoreSettings parent;

        public StoreSettingsPayPal(StoreSettings s)
        {
            parent = s;
        }

        public string UserName
        {
            get { return parent.GetPropEncrypted("PaypalUserName"); }
            set { parent.SetPropEncrypted("PaypalUserName", value); }
        }

        public string Password
        {
            get { return parent.GetPropEncrypted("PaypalPassword"); }
            set { parent.SetPropEncrypted("PaypalPassword", value); }
        }

        public string ClienId
        {
            get { return parent.GetPropEncrypted("PaypalClientId"); }
            set { parent.SetPropEncrypted("PaypalClientId", value); }
        }

        public string Secret
        {
            get { return parent.GetPropEncrypted("PaypalSecret"); }
            set { parent.SetPropEncrypted("PaypalSecret", value); }
        }

        public string Signature
        {
            get { return parent.GetPropEncrypted("PaypalSignature"); }
            set { parent.SetPropEncrypted("PaypalSignature", value); }
        }

        public bool ExpressAuthorizeOnly
        {
            get { return parent.GetPropBool("PaypalExpressAuthorizeOnly"); }
            set { parent.SetProp("PaypalExpressAuthorizeOnly", value); }
        }

        public bool AllowUnconfirmedAddresses
        {
            get { return parent.GetPropBool("PaypalAllowUnconfirmedAddresses"); }
            set { parent.SetProp("PaypalAllowUnconfirmedAddresses", value); }
        }

        public bool RequirePayPalAccount
        {
            get { return parent.GetPropBoolWithDefault("RequirePayPalAccount", true); }
            set { parent.SetProp("RequirePayPalAccount", value); }
        }

        public string Mode
        {
            get { return parent.GetProp("PaypalMode"); }
            set { parent.SetProp("PaypalMode", value); }
        }

        public string Currency
        {
            get { return parent.GetProp("PaypalCurrency"); }
            set { parent.SetProp("PaypalCurrency", value); }
        }

        public string FastSignupEmail
        {
            get { return parent.GetProp("PayPalFastSignupEmail"); }
            set { parent.SetProp("PayPalFastSignupEmail", value); }
        }
    }
}