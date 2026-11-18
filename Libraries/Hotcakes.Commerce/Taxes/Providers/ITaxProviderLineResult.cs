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

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Represents the tax calculated for one submitted <see cref="Line"/>.
    /// </summary>
    /// <remarks>
    /// The line number and item code correlate the provider response with the request line. Rates are
    /// decimal fractions (for example, 0.07 represents seven percent), matching the commerce tax model.
    /// </remarks>
    public interface ITaxProviderLineResult
    {
        /// <summary>Gets or sets the submitted line number or provider line identifier.</summary>
        string No { get; set; }

        /// <summary>Gets or sets the submitted item code.</summary>
        string ItemCode { get; set; }

        /// <summary>Gets or sets the calculated tax rate as a decimal fraction.</summary>
        decimal TaxRate { get; set; }

        /// <summary>Gets or sets the calculated tax amount for this line.</summary>
        decimal TaxValue { get; set; }
    }
}
