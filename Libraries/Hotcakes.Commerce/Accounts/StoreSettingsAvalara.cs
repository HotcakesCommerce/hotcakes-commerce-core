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
    /// <summary>Provides Avalara tax-service settings owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Values are store-scoped and staged through the parent façade. <see cref="Password"/> and <see cref="LicenseKey"/> are sensitive credentials and must be protected as secrets. Persistence occurs when the caller saves store settings.</remarks>
    [Serializable]
    public class StoreSettingsAvalara
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new Avalara settings façade.</summary>
        /// <param name="s">The owning store settings façade.</param>
        public StoreSettingsAvalara(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether Avalara integration is enabled.</summary>
        /// <value>The <c>AvalaraEnabled</c> setting; unset values read as <see langword="false"/>.</value>
        public bool Enabled
        {
            get { return parent.GetPropBool("AvalaraEnabled"); }
            set { parent.SetProp("AvalaraEnabled", value); }
        }

        /// <summary>Gets or sets whether Avalara diagnostic mode is enabled.</summary>
        /// <value>The <c>AvalaraDebugMode</c> setting; unset values read as <see langword="false"/>.</value>
        public bool DebugMode
        {
            get { return parent.GetPropBool("AvalaraDebugMode"); }
            set { parent.SetProp("AvalaraDebugMode", value); }
        }

        /// <summary>Gets or sets the Avalara account username.</summary>
        /// <value>The <c>AvalaraUsername</c> setting.</value>
        public string Username
        {
            get { return parent.GetProp("AvalaraUsername"); }
            set { parent.SetProp("AvalaraUsername", value); }
        }

        /// <summary>Gets or sets the Avalara account password.</summary>
        /// <value>The sensitive <c>AvalaraPassword</c> setting.</value>
        public string Password
        {
            get { return parent.GetProp("AvalaraPassword"); }
            set { parent.SetProp("AvalaraPassword", value); }
        }

        /// <summary>Gets or sets the Avalara service URL.</summary>
        /// <value>The <c>AvalaraServiceUrl</c> setting.</value>
        public string ServiceUrl
        {
            get { return parent.GetProp("AvalaraServiceUrl"); }
            set { parent.SetProp("AvalaraServiceUrl", value); }
        }

        /// <summary>Gets or sets the Avalara account number.</summary>
        /// <value>The <c>AvalaraAccount</c> setting.</value>
        public string Account
        {
            get { return parent.GetProp("AvalaraAccount"); }
            set { parent.SetProp("AvalaraAccount", value); }
        }

        /// <summary>Gets or sets the Avalara license key.</summary>
        /// <value>The sensitive <c>AvalaraLicenseKey</c> setting.</value>
        public string LicenseKey
        {
            get { return parent.GetProp("AvalaraLicenseKey"); }
            set { parent.SetProp("AvalaraLicenseKey", value); }
        }

        /// <summary>Gets or sets the Avalara company code.</summary>
        /// <value>The <c>AvalaraCompanyCode</c> setting.</value>
        public string CompanyCode
        {
            get { return parent.GetProp("AvalaraCompanyCode"); }
            set { parent.SetProp("AvalaraCompanyCode", value); }
        }

        /// <summary>Gets or sets the shipping tax code sent to Avalara.</summary>
        /// <value>The <c>AvalaraShippingTaxCode</c> setting.</value>
        public string ShippingTaxCode
        {
            get { return parent.GetProp("AvalaraShippingTaxCode"); }
            set { parent.SetProp("AvalaraShippingTaxCode", value); }
        }

        /// <summary>Gets or sets the tax-exempt code sent to Avalara.</summary>
        /// <value>The <c>AvalaraTaxExemptCode</c> setting.</value>
        public string TaxExemptCode
        {
            get { return parent.GetProp("AvalaraTaxExemptCode"); }
            set { parent.SetProp("AvalaraTaxExemptCode", value); }
        }
    }
}
