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
    /// Describes the value, quantity, weight, and dimensions of one shipment box.
    /// </summary>
    /// <seealso cref="IShippable" />
    [Serializable]
    public class Shippable : IShippable
    {
        /// <summary>
        /// Initializes a shippable box with zero measurements and the default imperial units.
        /// </summary>
        public Shippable()
        {
            BoxValue = 0;
            QuantityOfItemsInBox = 0;
            BoxWeightType = WeightType.Pounds;
            BoxWeight = 0;
            BoxLengthType = LengthType.Inches;
            BoxLength = 0;
            BoxWidth = 0;
            BoxHeight = 0;
        }

        /// <summary>Gets or sets the monetary value of the contents of the box.</summary>
        public decimal BoxValue { get; set; }

        /// <summary>Gets or sets the number of items in the box.</summary>
        public int QuantityOfItemsInBox { get; set; }

        /// <summary>Gets or sets the unit used by <see cref="BoxWeight" />.</summary>
        public WeightType BoxWeightType { get; set; }

        /// <summary>Gets or sets the weight of the box contents.</summary>
        public decimal BoxWeight { get; set; }

        /// <summary>Gets or sets the unit used by the box dimensions.</summary>
        public LengthType BoxLengthType { get; set; }

        /// <summary>Gets or sets the length of the box.</summary>
        public decimal BoxLength { get; set; }

        /// <summary>Gets or sets the width of the box.</summary>
        public decimal BoxWidth { get; set; }

        /// <summary>Gets or sets the height of the box.</summary>
        public decimal BoxHeight { get; set; }

        /// <summary>
        /// Creates a value copy of this shippable box.
        /// </summary>
        /// <returns>A new <see cref="IShippable" /> containing the same box values.</returns>
        public IShippable CloneShippable()
        {
            var clone = new Shippable
            {
                BoxValue = BoxValue,
                QuantityOfItemsInBox = QuantityOfItemsInBox,
                BoxWeightType = BoxWeightType,
                BoxWeight = BoxWeight,
                BoxLengthType = BoxLengthType,
                BoxLength = BoxLength,
                BoxWidth = BoxWidth,
                BoxHeight = BoxHeight
            };
            return clone;
        }
    }
}
