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

namespace Hotcakes.Payment.Gateways
{
    /// <summary>Stores the credentials, currency, and endpoint options used by <see cref="Ogone"/>.</summary>
    [Serializable]
    public class OgoneSettings : MethodSettings
    {
        /// <summary>Gets or sets the Ogone payment service provider identifier.</summary>
        public string PaymentServiceProviderId
        {
            get { return GetSettingOrEmpty("PaymentServiceProviderId"); }
            set { AddOrUpdate("PaymentServiceProviderId", value); }
        }

        /// <summary>Gets or sets the Ogone user identifier.</summary>
        public string UserId
        {
            get { return GetSettingOrEmpty("UserId"); }
            set { AddOrUpdate("UserId", value); }
        }

        /// <summary>Gets or sets the Ogone password.</summary>
        public string Password
        {
            get { return GetSettingOrEmpty("Password"); }
            set { AddOrUpdate("Password", value); }
        }

        /// <summary>Gets or sets the currency code sent to Ogone.</summary>
        public string Currency
        {
            get { return GetSettingOrEmpty("Currency"); }
            set { AddOrUpdate("Currency", value); }
        }

        /// <summary>Gets or sets a value indicating whether request and response details are added to <see cref="Transaction.Result"/>.</summary>
        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }

        /// <summary>Gets or sets a value indicating whether the Ogone test endpoints are used.</summary>
        public bool DeveloperMode
        {
            get { return GetBoolSetting("DeveloperMode"); }
            set { SetBoolSetting("DeveloperMode", value); }
        }
    }
}
