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
using Hotcakes.Shipping.Services;

namespace Hotcakes.Shipping
{
    [Serializable]
    public class Service
    {
        public static List<IShippingService> FindAll()
        {
            var result = new List<IShippingService>
            {
                new FlatRatePerItem(),
                new FlatRatePerOrder(),
                new PerItem(),
                new RateTableByItemCount(),
                new RateTableByTotalPrice(),
                new RateTableByTotalWeight(),
                new RatePerWeightFormula()
            };

            return result;
        }

        public static IShippingService FindById(string id)
        {
            switch (id)
            {
                case "41B590A7-003C-48d1-8446-EAE93C156AA1":
                    return new PerItem();
                case "301AA2B8-F43C-42fe-B77E-A7E1CB1DD40E":
                    return new FlatRatePerOrder();
                case "3D6623E7-1E2C-444d-B860-A8F542133093":
                    return new FlatRatePerItem();
                case "06C22589-14A7-470f-88EC-AF559D040A7A":
                    return new RateTableByTotalWeight();
                case "9F896073-EE1F-400c-8B54-D9858B06AA01":
                    return new RateTableByTotalPrice();
                case "7068B66A-0336-4228-B1A8-03E034FECCDA":
                    return new RateTableByItemCount();
                case "5AAF9016-B03F-4e7c-8596-193F5EFFFDC3":
                    return new RatePerWeightFormula();
            }

            return null;
        }
    }
}