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
    /// <summary>
    /// Configures the simulated responses returned by <see cref="TestGateway"/>.
    /// </summary>
    [Serializable]
    public class TestGatewaySettings : MethodSettings
    {
        private const bool DefaultRespose = true;

        /// <summary>
        /// Gets or sets the simulated result for credit-card hold transactions.
        /// </summary>
        public bool ResponseForHold
        {
            get { return GetBoolSetting("ResponseForHold", DefaultRespose); }
            set { SetBoolSetting("ResponseForHold", value); }
        }

        /// <summary>
        /// Gets or sets the simulated result for credit-card capture transactions.
        /// </summary>
        public bool ResponseForCapture
        {
            get { return GetBoolSetting("ResponseForCapture", DefaultRespose); }
            set { SetBoolSetting("ResponseForCapture", value); }
        }

        /// <summary>
        /// Gets or sets the simulated result for credit-card charge transactions.
        /// </summary>
        public bool ResponseForCharge
        {
            get { return GetBoolSetting("ResponseForCharge", DefaultRespose); }
            set { SetBoolSetting("ResponseForCharge", value); }
        }

        /// <summary>
        /// Gets or sets the simulated result for credit-card refund transactions.
        /// </summary>
        public bool ResponseForRefund
        {
            get { return GetBoolSetting("ResponseForRefund", DefaultRespose); }
            set { SetBoolSetting("ResponseForRefund", value); }
        }

        /// <summary>
        /// Gets or sets the simulated result for credit-card void transactions.
        /// </summary>
        public bool ResponseForVoid
        {
            get { return GetBoolSetting("ResponseForVoid", DefaultRespose); }
            set { SetBoolSetting("ResponseForVoid", value); }
        }
    }
}
