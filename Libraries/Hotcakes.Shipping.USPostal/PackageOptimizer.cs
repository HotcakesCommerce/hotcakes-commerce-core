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

using System;
using System.Collections.Generic;

namespace Hotcakes.Shipping.USPostal
{
    [Serializable]
    internal class PackageOptimizer
    {
        private readonly decimal _MaxWeight = 70m;

        public PackageOptimizer(decimal maxWeightInPounds)
        {
            _MaxWeight = maxWeightInPounds;
        }

        public bool GirthCheck { get; set; }
        public decimal GirthMaxInInches { get; set; }
        public bool LengthPlusGirthCheck { get; set; }
        public decimal LengthPlusGirthMaxInInches { get; set; }

        public List<IShippable> OptimizePackagesToMaxWeight(IShipment shipment)
        {
            var result = new List<IShippable>();
            var itemsToSplit = new List<IShippable>();

            // drop off oversize items right away
            foreach (var item in shipment.Items)
            {
                if (IsOversized(item))
                {
                    result.Add(item.CloneShippable());
                }
                else
                {
                    itemsToSplit.Add(item);
                }
            }

            IShippable tempPackage = new Shippable();

            foreach (var pak in itemsToSplit)
            {
                if (_MaxWeight - tempPackage.BoxWeight >= pak.BoxWeight)
                {
                    // add to current box
                    tempPackage.BoxWeight += pak.BoxWeight;
                    tempPackage.QuantityOfItemsInBox += pak.QuantityOfItemsInBox;
                    tempPackage.BoxValue += pak.BoxValue;
                }
                else
                {
                    // Save the temp package if it has items
                    if (tempPackage.BoxWeight > 0 || tempPackage.QuantityOfItemsInBox > 0)
                    {
                        result.Add(tempPackage.CloneShippable());
                        tempPackage = new Shippable();
                    }

                    // create new box
                    if (pak.BoxWeight > _MaxWeight)
                    {
                        //Loop to break up > maxWeight Packages
                        var currentItemsInBox = pak.QuantityOfItemsInBox;
                        var currentWeight = pak.BoxWeight;

                        while (currentWeight > 0)
                        {
                            if (currentWeight > _MaxWeight)
                            {
                                var newP = pak.CloneShippable();
                                newP.BoxWeight = _MaxWeight;
                                if (currentItemsInBox > 0)
                                {
                                    currentItemsInBox -= 1;
                                    newP.QuantityOfItemsInBox = 1;
                                }
                                result.Add(newP);
                                currentWeight = currentWeight - _MaxWeight;
                                if (currentWeight < 0)
                                {
                                    currentWeight = 0;
                                }
                            }
                            else
                            {
                                // Create a new shippable box 
                                var newP = pak.CloneShippable();
                                newP.BoxWeight = currentWeight;
                                if (currentItemsInBox > 0)
                                {
                                    newP.QuantityOfItemsInBox = currentItemsInBox;
                                }
                                result.Add(newP);
                                currentWeight = 0;
                            }
                        }
                    }
                    else
                    {
                        tempPackage = pak.CloneShippable();
                    }
                }
            }

            // Save the temp package if it has items
            if (tempPackage.BoxWeight > 0 || tempPackage.QuantityOfItemsInBox > 0)
            {
                result.Add(tempPackage.CloneShippable());
                tempPackage = new Shippable();
            }

            return result;
        }

        private bool IsOversized(IShippable prod)
        {
            var girth = 2m*prod.BoxHeight + 2m*prod.BoxWidth;
            if (GirthCheck)
            {
                if (girth > GirthMaxInInches)
                    return true;
            }
            if (LengthPlusGirthCheck)
            {
                if (girth + prod.BoxLength > LengthPlusGirthMaxInInches)
                    return true;
            }
            return false;
        }
    }
}