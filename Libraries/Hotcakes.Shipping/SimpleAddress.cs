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
using Hotcakes.Web.Geography;

namespace Hotcakes.Shipping
{
    [Serializable]
    public class SimpleAddress : IAddress
    {
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string RegionBvin { get; set; }
        public IRegion RegionData { get; set; }
        public string PostalCode { get; set; }
        public string CountryBvin { get; set; }
        public ICountry CountryData { get; set; }

        public static SimpleAddress CloneAddress(IAddress source)
        {
            var result = new SimpleAddress
            {
                City = source.City,
                CountryBvin = source.CountryBvin,
                CountryData = source.CountryData,
                PostalCode = source.PostalCode,
                RegionBvin = source.RegionBvin,
                RegionData = source.RegionData,
                Street = source.Street,
                Street2 = source.Street2
            };

            return result;
        }
    }
}