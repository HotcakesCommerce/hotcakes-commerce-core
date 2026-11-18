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
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Stores configuration for the Moneris payment method.
    /// </summary>
    /// <remarks>
    /// Each property maps to the exact setting key passed to the inherited
    /// <see cref="MethodSettings"/> accessors. Values are stored as method
    /// settings rather than as additional serialized properties.
    /// </remarks>
    [Serializable]
    public class MonerisSettings : MethodSettings
    {
        /// <summary>
        /// Gets or sets the Hosted Pay Page identifier stored under
        /// <c>HostedPayPageId</c>.
        /// </summary>
        public string HostedPayPageId
        {
            get { return GetSettingOrEmpty("HostedPayPageId"); }
            set { AddOrUpdate("HostedPayPageId", value); }
        }

        /// <summary>
        /// Gets or sets the Hosted Pay Page token stored under
        /// <c>HostedPayPageToken</c>.
        /// </summary>
        public string HostedPayPageToken
        {
            get { return GetSettingOrEmpty("HostedPayPageToken"); }
            set { AddOrUpdate("HostedPayPageToken", value); }
        }

        /// <summary>
        /// Gets or sets the developer-mode flag stored under
        /// <c>DeveloperMode</c>. Reading and writing use the inherited boolean
        /// setting helpers.
        /// </summary>
        public bool DeveloperMode
        {
            get { return GetBoolSetting("DeveloperMode"); }
            set { SetBoolSetting("DeveloperMode", value); }
        }

        /// <summary>
        /// Gets or sets the debug-mode flag stored under <c>DebugMode</c>.
        /// </summary>
        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }
    }
}
