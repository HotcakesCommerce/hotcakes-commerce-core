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

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Describes one taxable order line sent to a tax provider.
    /// </summary>
    /// <remarks>
    /// <see cref="Amount"/> is the line amount and <see cref="Qty"/> is the quantity; the selected
    /// provider determines how the remaining classification, address, exemption, and reference fields
    /// are interpreted. Shipping is represented by a line when the provider integration requires it.
    /// </remarks>
    [Serializable]
    public class Line
    {
        /// <summary>Gets or sets the line number or provider-facing line identifier.</summary>
        public string No { get; set; }

        /// <summary>Gets or sets the address from which this line originates.</summary>
        /// <seealso cref="BaseAddress"/>
        public BaseAddress OriginAddress { get; set; }

        /// <summary>Gets or sets the address to which this line is destined.</summary>
        /// <seealso cref="BaseAddress"/>
        public BaseAddress DestinationAddress { get; set; }

        /// <summary>Gets or sets the product or item code.</summary>
        public string ItemCode { get; set; }

        /// <summary>Gets or sets the tax-classification code.</summary>
        public string TaxCode { get; set; }

        /// <summary>Gets or sets the quantity represented by the line.</summary>
        public decimal Qty { get; set; }

        /// <summary>Gets or sets the monetary amount represented by the line.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets a value indicating whether the line has been discounted.</summary>
        public bool Discounted { get; set; }

        /// <summary>Gets or sets the revenue account reference.</summary>
        public string RevAcct { get; set; }

        /// <summary>Gets or sets the first provider-specific reference value.</summary>
        public string Ref1 { get; set; }

        /// <summary>Gets or sets the second provider-specific reference value.</summary>
        public string Ref2 { get; set; }

        /// <summary>Gets or sets the exemption number or exemption reference.</summary>
        public string ExemptionNo { get; set; }

        /// <summary>Gets or sets the customer usage classification.</summary>
        public string CustomerUsageType { get; set; }

        /// <summary>Gets or sets the human-readable line description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets a value indicating whether <see cref="Amount"/> includes tax.</summary>
        public bool TaxIncluded { get; set; }
    }
}
