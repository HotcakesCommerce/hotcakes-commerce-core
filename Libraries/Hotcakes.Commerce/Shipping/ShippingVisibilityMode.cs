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

namespace Hotcakes.Commerce.Shipping
{
    public enum ShippingVisibilityMode
    {
        /// <summary>
        ///     The shipping method will always be included in shipping calculation and rate retrieval methods.
        /// </summary>
        Always = 0,

        /// <summary>
        ///     The shipping method will never be included in shipping calculation and rate retrieval methods. This is effectively
        ///     disabling the shipping method.
        /// </summary>
        Never = 1,

        /// <summary>
        ///     This method will only be included in shipping calculation and rate retrieval when the previous methods do not
        ///     return any rates.
        /// </summary>
        NoRates = 2,

        /// <summary>
        ///     The order subtotal must be greater than the specified amount in order to be included in the shipping calculation
        ///     and rate retrieval methods.
        /// </summary>
        SubtotalAmount = 3,

        /// <summary>
        ///     The total weight of all line items must be greater than the specified total in order to be included in the shipping
        ///     calculation and rate retrieval methods.
        /// </summary>
        TotalWeight = 4
    }
}