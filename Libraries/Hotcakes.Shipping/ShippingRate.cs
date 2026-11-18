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

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Represents a shipping option and its estimated cost.
    /// </summary>
    /// <seealso cref="IShippingRate" />
    [Serializable]
    public class ShippingRate : IShippingRate
    {
        /// <summary>
        /// Initializes an empty shipping rate.
        /// </summary>
        public ShippingRate()
        {
            EstimatedCost = 0;
            DisplayName = string.Empty;
            ServiceId = string.Empty;
            ServiceCodes = string.Empty;
        }

        /// <summary>
        /// Gets or sets the estimated cost of the shipping option.
        /// </summary>
        public decimal EstimatedCost { get; set; }

        /// <summary>
        /// Gets or sets the name shown to users for the shipping option.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the service that supplied the rate.
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the service codes associated with the rate.
        /// </summary>
        public string ServiceCodes { get; set; }
    }
}
