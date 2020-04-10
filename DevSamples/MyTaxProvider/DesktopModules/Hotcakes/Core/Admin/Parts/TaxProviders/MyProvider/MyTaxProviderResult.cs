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

using System.Collections.Generic;
using Hotcakes.Commerce.Taxes.Providers;

namespace MyCompany.MyTaxProvider
{
    /// <summary>
    ///     This class is used to return the result back to the sytem.
    ///     So this data can be used then after to update the order
    /// </summary>
    public class MyTaxProviderResult : ITaxProviderResult
    {
        public MyTaxProviderResult()
        {
            Success = false;
            Messages = new List<string>();
            TotalAmount = 0m;
            ShippingTaxRate = 0m;
            ShippingTax = 0m;
            ItemsTax = 0m;
            TotalTax = 0m;
            DocCode = string.Empty;
            Items = new List<ITaxProviderLineResult>();
        }

        public bool Success { get; set; }

        public List<string> Messages { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ShippingTaxRate { get; set; }

        public decimal ShippingTax { get; set; }

        public decimal ItemsTax { get; set; }

        public decimal TotalTax { get; set; }

        public string DocCode { get; set; }

        public List<ITaxProviderLineResult> Items { get; set; }
    }
}