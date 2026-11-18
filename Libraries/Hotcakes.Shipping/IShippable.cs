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
    /// Represents a shippable package, including its value, quantity, weight, and dimensions.
    /// </summary>
    /// <remarks>
    /// Weight and dimensions are interpreted using <see cref="BoxWeightType" /> and <see cref="BoxLengthType" />.
    /// </remarks>
    public interface IShippable
    {
        /// <summary>
        /// Gets or sets the declared monetary value of the package contents.
        /// </summary>
        decimal BoxValue { get; set; }

        /// <summary>
        /// Gets or sets the number of items contained in the package.
        /// </summary>
        int QuantityOfItemsInBox { get; set; }

        /// <summary>
        /// Gets or sets the unit used to interpret <see cref="BoxWeight" />.
        /// </summary>
        WeightType BoxWeightType { get; set; }

        /// <summary>
        /// Gets or sets the weight of the package in the selected <see cref="BoxWeightType" />.
        /// </summary>
        decimal BoxWeight { get; set; }

        /// <summary>
        /// Gets or sets the unit used to interpret the package dimensions.
        /// </summary>
        LengthType BoxLengthType { get; set; }

        /// <summary>
        /// Gets or sets the length of the package in the selected <see cref="BoxLengthType" />.
        /// </summary>
        decimal BoxLength { get; set; }

        /// <summary>
        /// Gets or sets the width of the package in the selected <see cref="BoxLengthType" />.
        /// </summary>
        decimal BoxWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the package in the selected <see cref="BoxLengthType" />.
        /// </summary>
        decimal BoxHeight { get; set; }

        /// <summary>
        /// Creates a copy of this shippable package.
        /// </summary>
        /// <returns>A new <see cref="IShippable" /> containing the copied package data.</returns>
        IShippable CloneShippable();
    }
}
