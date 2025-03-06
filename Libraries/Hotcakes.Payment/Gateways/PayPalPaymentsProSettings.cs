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

namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class PayPalPaymentsProSettings : MethodSettings
    {
        public string PayPalUserName
        {
            get { return GetSettingOrEmpty("PayPalUserName"); }
            set { AddOrUpdate("PayPalUserName", value); }
        }

        public string PayPalPassword
        {
            get { return GetSettingOrEmpty("PayPalPassword"); }
            set { AddOrUpdate("PayPalPassword", value); }
        }

        public string PayPalClientId
        {
            get { return GetSettingOrEmpty("PayPalClientId"); }
            set { AddOrUpdate("PayPalClientId", value); }
        }

        public string PayPalSecret
        {
            get { return GetSettingOrEmpty("PayPalSecret"); }
            set { AddOrUpdate("PayPalSecret", value); }
        }

        public string PayPalSignature
        {
            get { return GetSettingOrEmpty("PayPalSignature"); }
            set { AddOrUpdate("PayPalSignature", value); }
        }

        public string PayPalMode
        {
            get { return GetSettingOrEmpty("PayPalMode"); }
            set { AddOrUpdate("PayPalMode", value); }
        }

        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }

        public string Currency
        {
            get
            {
                var currency = GetSettingOrEmpty("Currency");

                if (string.IsNullOrEmpty(currency))
                {
                    currency = "USD";
                }

                return currency;
            }
            set { AddOrUpdate("Currency", value); }
        }
    }
}