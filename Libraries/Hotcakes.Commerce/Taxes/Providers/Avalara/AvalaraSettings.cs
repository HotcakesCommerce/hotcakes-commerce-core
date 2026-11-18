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

namespace Hotcakes.Commerce.Taxes.Providers.Avalara
{
    /// <summary>
    /// Stores the Avalara provider configuration using the provider's serialized setting keys.
    /// </summary>
    /// <remarks>
    /// String values are stored under their exact property names. <see cref="DebugMode"/>
    /// uses the inherited Boolean representation: <c>1</c> for <see langword="true"/> and
    /// <c>0</c> for <see langword="false"/>. Missing or other Boolean tokens read as
    /// <see langword="false"/>.
    /// </remarks>
    [Serializable]
    public class AvalaraSettings : TaxProviderSettings
    {
        #region Custom settings

        /// <summary>Gets or sets the Avalara service endpoint stored under <c>ServiceUrl</c>.</summary>
        public string ServiceUrl
        {
            get { return GetSettingOrEmpty("ServiceUrl"); }
            set { AddOrUpdate("ServiceUrl", value); }
        }

        /// <summary>Gets or sets the Avalara account identifier stored under <c>Account</c>.</summary>
        public string Account
        {
            get { return GetSettingOrEmpty("Account"); }
            set { AddOrUpdate("Account", value); }
        }

        /// <summary>Gets or sets the Avalara license credential stored under <c>LicenseKey</c>.</summary>
        /// <remarks>The value is passed to the SOAP client as its license credential.</remarks>
        public string LicenseKey
        {
            get { return GetSettingOrEmpty("LicenseKey"); }
            set { AddOrUpdate("LicenseKey", value); }
        }

        /// <summary>Gets or sets the Avalara company code stored under <c>CompanyCode</c>.</summary>
        public string CompanyCode
        {
            get { return GetSettingOrEmpty("CompanyCode"); }
            set { AddOrUpdate("CompanyCode", value); }
        }

        /// <summary>Gets or sets the shipping tax code stored under <c>ShippingTaxCode</c>.</summary>
        public string ShippingTaxCode
        {
            get { return GetSettingOrEmpty("ShippingTaxCode"); }
            set { AddOrUpdate("ShippingTaxCode", value); }
        }

        /// <summary>Gets or sets the tax-exemption code stored under <c>TaxExemptCode</c>.</summary>
        /// <remarks>This setting is retained as part of the provider configuration contract.</remarks>
        public string TaxExemptCode
        {
            get { return GetSettingOrEmpty("TaxExemptCode"); }
            set { AddOrUpdate("TaxExemptCode", value); }
        }

        /// <summary>Gets or sets whether diagnostic notes and event-log entries are written.</summary>
        /// <remarks>The serialized key is <c>DebugMode</c>; values are exactly <c>1</c> and <c>0</c>.</remarks>
        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }

        #endregion
    }
}
