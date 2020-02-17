#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

namespace Hotcakes.Commerce.Shipping
{
    public class DefaultDimensionCalculator : DimensionCalculator
    {
        public override void GenerateDimensions(ShippingGroup shippingGroup)
        {
            if (shippingGroup.Items != null)
            {
                shippingGroup.Length = 0;
                shippingGroup.Weight = 0;
                shippingGroup.Height = 0;
                shippingGroup.Width = 0;

                var dimensionsSet = false;
                if (shippingGroup.Items.Count == 1)
                {
                    if (shippingGroup.Items[0].Quantity == 1)
                    {
                        shippingGroup.Length = shippingGroup.Items[0].ProductShippingLength;
                        shippingGroup.Height = shippingGroup.Items[0].ProductShippingHeight;
                        shippingGroup.Width = shippingGroup.Items[0].ProductShippingWidth;
                        shippingGroup.Weight = shippingGroup.Items[0].GetTotalWeight();
                        dimensionsSet = true;
                    }
                }

                decimal longestDimension = 0;
                decimal totalVolume = 0;
                if (!dimensionsSet)
                {
                    for (var i = 0; i < shippingGroup.Items.Count; i++)
                    {
                        if (shippingGroup.Items[i].ProductShippingLength > longestDimension)
                        {
                            longestDimension = shippingGroup.Items[i].ProductShippingLength;
                        }
                        if (shippingGroup.Items[i].ProductShippingWidth > longestDimension)
                        {
                            longestDimension = shippingGroup.Items[i].ProductShippingWidth;
                        }
                        if (shippingGroup.Items[i].ProductShippingHeight*
                            (shippingGroup.Items[i].Quantity - shippingGroup.Items[i].QuantityShipped) >
                            longestDimension)
                        {
                            longestDimension = shippingGroup.Items[i].ProductShippingHeight*
                                               (shippingGroup.Items[i].Quantity - shippingGroup.Items[i].QuantityShipped);
                        }

                        totalVolume += (shippingGroup.Items[i].Quantity - shippingGroup.Items[i].QuantityShipped)*
                                       shippingGroup.Items[i].ProductShippingLength*
                                       shippingGroup.Items[i].ProductShippingWidth*
                                       shippingGroup.Items[i].ProductShippingHeight;

                        shippingGroup.Weight += shippingGroup.Items[i].GetTotalWeight();
                    }

                    //Estimate Package Size based on Volume
                    shippingGroup.Length = longestDimension;

                    if (longestDimension > 0 & totalVolume > 0)
                    {
                        shippingGroup.Width = (decimal) Math.Sqrt((double) (totalVolume/longestDimension));
                    }

                    shippingGroup.Height = shippingGroup.Width;
                }

                if (shippingGroup.Width < 1)
                {
                    shippingGroup.Width = 1;
                }
                if (shippingGroup.Height < 1)
                {
                    shippingGroup.Height = 1;
                }
                if (shippingGroup.Length < 1)
                {
                    shippingGroup.Length = 1;
                }
            }
        }
    }
}