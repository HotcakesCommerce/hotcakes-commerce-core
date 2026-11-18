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

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Represents a shipping option and its estimated cost returned by an <see cref="IShippingService" />.
    /// </summary>
    public interface IShippingRate
    {
        /// <summary>
        /// Gets or sets the estimated cost of using this shipping option.
        /// </summary>
        decimal EstimatedCost { get; set; }

        /// <summary>
        /// Gets or sets the human-readable name of the shipping option.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the shipping service that supplied the rate.
        /// </summary>
        string ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the service codes associated with the rate.
        /// </summary>
        string ServiceCodes { get; set; }
    }
}
