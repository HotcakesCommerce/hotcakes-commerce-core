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
    /// Represents the tax returned for one non-shipping line by AvaTax.
    /// </summary>
    /// <remarks>
    /// Instances are placed in <see cref="AvaTaxResult.Items"/> in the same order as the
    /// corresponding order lines. The final AvaTax line is interpreted as shipping and is
    /// therefore not represented by this type.
    /// </remarks>
    [Serializable]
    public class AvaTaxLineResult : ITaxProviderLineResult
    {
        /// <summary>Gets or sets the provider line number returned by AvaTax.</summary>
        public string No { get; set; }

        /// <summary>Gets or sets the item code associated with the AvaTax line.</summary>
        public string ItemCode { get; set; }

        /// <summary>Gets or sets the effective tax rate returned for the line.</summary>
        /// <remarks>The value is a decimal fraction, not a percentage display value.</remarks>
        public decimal TaxRate { get; set; }

        /// <summary>Gets or sets the tax amount returned for the line.</summary>
        public decimal TaxValue { get; set; }
    }
}
