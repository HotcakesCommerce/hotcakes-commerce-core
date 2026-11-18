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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    /// Specifies the basis and calculation mode used by a <see cref="PriceGroup"/>.
    /// </summary>
    /// <remarks>
    /// The numeric values are serialized and persisted; retain them for compatibility with existing price groups and DTOs.
    /// </remarks>
    public enum PricingTypes
    {
        /// <summary>Subtracts a percentage of the product MSRP from the list price basis.</summary>
        PercentageOffListPrice = 0,
        /// <summary>Subtracts a fixed amount from the product MSRP.</summary>
        AmountOffListPrice = 1,
        /// <summary>Adds a percentage to the product cost.</summary>
        PercentageAboveCost = 2,
        /// <summary>Adds a fixed amount to the product cost.</summary>
        AmountAboveCost = 3,
        /// <summary>Subtracts a percentage of the current site price.</summary>
        PercentageOffSitePrice = 4,
        /// <summary>Subtracts a fixed amount from the current site price.</summary>
        AmountOffSitePrice = 5
    }
}
