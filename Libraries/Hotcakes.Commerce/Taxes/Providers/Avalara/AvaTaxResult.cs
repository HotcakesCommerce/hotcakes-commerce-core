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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Taxes.Providers.Avalara
{
    /// <summary>
    /// Contains the aggregate and line-level result returned by the Avalara tax provider.
    /// </summary>
    /// <remarks>
    /// A newly constructed result represents failure, has empty messages and document code,
    /// and has zero-valued totals. The gateway changes these values after each AvaTax operation.
    /// </remarks>
    [Serializable]
    public class AvaTaxResult : ITaxProviderResult
    {
        /// <summary>
        /// Initializes a result with failure status, empty collections, and zero totals.
        /// </summary>
        public AvaTaxResult()
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

        /// <summary>Gets or sets whether the Avalara operation completed successfully.</summary>
        public bool Success { get; set; }

        /// <summary>Gets or sets the operation or service messages returned by Avalara.</summary>
        /// <remarks>Gateway failures add a fixed operation message followed by formatted service messages.</remarks>
        public List<string> Messages { get; set; }

        /// <summary>Gets or sets the document total returned by AvaTax.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Gets or sets the tax rate returned for the shipping line.</summary>
        /// <remarks>The value is a decimal fraction, not a percentage display value.</remarks>
        public decimal ShippingTaxRate { get; set; }

        /// <summary>Gets or sets the tax amount returned for the shipping line.</summary>
        public decimal ShippingTax { get; set; }

        /// <summary>Gets or sets the tax total for non-shipping lines.</summary>
        /// <remarks>For a successful calculation this is <see cref="TotalTax"/> minus <see cref="ShippingTax"/>.</remarks>
        public decimal ItemsTax { get; set; }

        /// <summary>Gets or sets the total tax returned by AvaTax.</summary>
        public decimal TotalTax { get; set; }

        /// <summary>Gets or sets the AvaTax document code.</summary>
        /// <remarks>Commit-related operations preserve the document code supplied by the caller.</remarks>
        public string DocCode { get; set; }

        /// <summary>Gets or sets the non-shipping line results returned by AvaTax.</summary>
        /// <remarks>The list contains <see cref="AvaTaxLineResult"/> instances for calculated order lines.</remarks>
        public List<ITaxProviderLineResult> Items { get; set; }
    }
}
