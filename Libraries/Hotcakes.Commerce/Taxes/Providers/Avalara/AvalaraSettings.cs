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

namespace Hotcakes.Commerce.Taxes.Providers.Avalara
{
    [Serializable]
    public class AvalaraSettings : TaxProviderSettings
    {
        #region Custom settings

        public string ServiceUrl
        {
            get { return GetSettingOrEmpty("ServiceUrl"); }
            set { AddOrUpdate("ServiceUrl", value); }
        }

        public string Account
        {
            get { return GetSettingOrEmpty("Account"); }
            set { AddOrUpdate("Account", value); }
        }

        public string LicenseKey
        {
            get { return GetSettingOrEmpty("LicenseKey"); }
            set { AddOrUpdate("LicenseKey", value); }
        }

        public string CompanyCode
        {
            get { return GetSettingOrEmpty("CompanyCode"); }
            set { AddOrUpdate("CompanyCode", value); }
        }

        public string ShippingTaxCode
        {
            get { return GetSettingOrEmpty("ShippingTaxCode"); }
            set { AddOrUpdate("ShippingTaxCode", value); }
        }

        public string TaxExemptCode
        {
            get { return GetSettingOrEmpty("TaxExemptCode"); }
            set { AddOrUpdate("TaxExemptCode", value); }
        }

        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }

        #endregion
    }
}