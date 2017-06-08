#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    [Serializable]
    public class Shippable : IShippable
    {
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

        public decimal BoxValue { get; set; }
        public int QuantityOfItemsInBox { get; set; }
        public WeightType BoxWeightType { get; set; }
        public decimal BoxWeight { get; set; }
        public LengthType BoxLengthType { get; set; }
        public decimal BoxLength { get; set; }
        public decimal BoxWidth { get; set; }
        public decimal BoxHeight { get; set; }


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