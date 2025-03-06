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
    public class StoreSettingsAvalara
    {
        private readonly StoreSettings parent;

        public StoreSettingsAvalara(StoreSettings s)
        {
            parent = s;
        }

        public bool Enabled
        {
            get { return parent.GetPropBool("AvalaraEnabled"); }
            set { parent.SetProp("AvalaraEnabled", value); }
        }

        public bool DebugMode
        {
            get { return parent.GetPropBool("AvalaraDebugMode"); }
            set { parent.SetProp("AvalaraDebugMode", value); }
        }

        public string Username
        {
            get { return parent.GetProp("AvalaraUsername"); }
            set { parent.SetProp("AvalaraUsername", value); }
        }

        public string Password
        {
            get { return parent.GetProp("AvalaraPassword"); }
            set { parent.SetProp("AvalaraPassword", value); }
        }

        public string ServiceUrl
        {
            get { return parent.GetProp("AvalaraServiceUrl"); }
            set { parent.SetProp("AvalaraServiceUrl", value); }
        }

        public string Account
        {
            get { return parent.GetProp("AvalaraAccount"); }
            set { parent.SetProp("AvalaraAccount", value); }
        }

        public string LicenseKey
        {
            get { return parent.GetProp("AvalaraLicenseKey"); }
            set { parent.SetProp("AvalaraLicenseKey", value); }
        }

        public string CompanyCode
        {
            get { return parent.GetProp("AvalaraCompanyCode"); }
            set { parent.SetProp("AvalaraCompanyCode", value); }
        }

        public string ShippingTaxCode
        {
            get { return parent.GetProp("AvalaraShippingTaxCode"); }
            set { parent.SetProp("AvalaraShippingTaxCode", value); }
        }

        public string TaxExemptCode
        {
            get { return parent.GetProp("AvalaraTaxExemptCode"); }
            set { parent.SetProp("AvalaraTaxExemptCode", value); }
        }
    }
}