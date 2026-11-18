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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Represents the aggregate and line-level outcome returned by a tax provider operation.
    /// </summary>
    /// <remarks>
    /// A result can carry diagnostic <see cref="Messages"/> even when an operation succeeds. The
    /// aggregate amounts describe the order as a whole, while <see cref="Items"/> contains the
    /// per-line results represented by <see cref="ITaxProviderLineResult"/>.
    /// </remarks>
    public interface ITaxProviderResult
    {
        /// <summary>Gets or sets a value indicating whether the provider operation succeeded.</summary>
        bool Success { get; set; }

        /// <summary>Gets or sets diagnostic or user-facing messages returned by the provider.</summary>
        List<string> Messages { get; set; }

        /// <summary>Gets or sets the total taxable amount used by the provider.</summary>
        decimal TotalAmount { get; set; }

        /// <summary>Gets or sets the shipping tax rate as a decimal fraction.</summary>
        decimal ShippingTaxRate { get; set; }

        /// <summary>Gets or sets the tax amount assessed on shipping.</summary>
        decimal ShippingTax { get; set; }

        /// <summary>Gets or sets the tax amount assessed on order items.</summary>
        decimal ItemsTax { get; set; }

        /// <summary>Gets or sets the aggregate tax amount, normally the item and shipping tax combined.</summary>
        decimal TotalTax { get; set; }

        /// <summary>Gets or sets the provider document code associated with the operation.</summary>
        string DocCode { get; set; }

        /// <summary>Gets or sets the per-line tax results.</summary>
        /// <seealso cref="ITaxProviderLineResult"/>
        List<ITaxProviderLineResult> Items { get; set; }
    }
}
