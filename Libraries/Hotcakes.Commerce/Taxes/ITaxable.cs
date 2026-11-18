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

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>
    /// Describes a taxable line and the tax values calculated for it.
    /// </summary>
    /// <remarks>Tax rates supplied to the setter methods are decimal calculation factors; stored schedule and zone rates are percentage values and are converted before use.</remarks>
    public interface ITaxable
    {
        /// <summary>Gets a value indicating whether this line is exempt from tax.</summary>
        bool IsTaxExempt { get; }
        /// <summary>Gets a value indicating whether this line is excluded from shipping-tax calculations.</summary>
        bool IsNonShipping { get; }
        /// <summary>Gets the original price of one item before tax adjustments.</summary>
        decimal BasePricePerItem { get; }
        /// <summary>Gets the quantity represented by the line.</summary>
        int Quantity { get; }
        /// <summary>Gets the tax schedule identifier assigned to the line.</summary>
        long TaxSchedule { get; }
        /// <summary>Gets or sets the line total used by tax calculations.</summary>
        decimal LineTotal { get; set; }
        /// <summary>Gets or sets the item price after tax-related adjustment.</summary>
        decimal AdjustedPricePerItem { get; set; }
        /// <summary>Gets or sets the calculated tax portion for the line.</summary>
        decimal TaxPortion { get; set; }
        /// <summary>Gets or sets the calculated tax portion attributable to shipping.</summary>
        decimal ShippingTaxPortion { get; set; }
        /// <summary>Gets or sets the shipping amount used for the line.</summary>
        decimal ShippingPortion { get; set; }
        /// <summary>Sets the item tax calculation factor.</summary>
        /// <param name="taxRate">The decimal factor used in calculation, such as 0.07 for a seven-percent rate.</param>
        /// <returns>The assigned tax calculation factor.</returns>
        decimal SetTaxRate(decimal taxRate);
        /// <summary>Sets the shipping tax calculation factor.</summary>
        /// <param name="taxRate">The decimal factor used in calculation, such as 0.07 for a seven-percent rate.</param>
        /// <returns>The assigned shipping tax calculation factor.</returns>
        decimal SetShippingTaxRate(decimal taxRate);
        /// <summary>Clears the tax values calculated for the line.</summary>
        void ClearTaxValue();
    }
}
