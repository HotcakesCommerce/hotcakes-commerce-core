#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Taxes.Providers;

namespace MyCompany.MyTaxProvider
{
    /// <summary>
    ///     This class is used to define the different custom configuration properties
    ///     specific to any tax provider in order to get the result
    ///     from provider
    /// </summary>
    [Serializable]
    public class MyTaxProviderSettings : TaxProviderSettings
    {
        public string TaxProviderProp1
        {
            get { return GetSettingOrEmpty("TaxProviderProp1"); }
            set { AddOrUpdate("TaxProviderProp1", value); }
        }

        public string TaxProviderProp2
        {
            get { return GetSettingOrEmpty("TaxProviderProp2"); }
            set { AddOrUpdate("TaxProviderProp2", value); }
        }
    }
}