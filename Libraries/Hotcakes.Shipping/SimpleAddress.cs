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
using Hotcakes.Web.Geography;

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Provides a serializable address implementation for shipping operations.
    /// </summary>
    /// <seealso cref="IAddress" />
    [Serializable]
    public class SimpleAddress : IAddress
    {
        /// <summary>Gets or sets the street address.</summary>
        public string Street { get; set; }
        /// <summary>Gets or sets the second street-address line.</summary>
        public string Street2 { get; set; }

        /// <summary>Gets or sets the city.</summary>
        public string City { get; set; }

        /// <summary>Gets or sets the identifier of the region.</summary>
        public string RegionBvin { get; set; }

        /// <summary>Gets or sets the resolved region data.</summary>
        public IRegion RegionData { get; set; }

        /// <summary>Gets or sets the postal code.</summary>
        public string PostalCode { get; set; }

        /// <summary>Gets or sets the identifier of the country.</summary>
        public string CountryBvin { get; set; }

        /// <summary>Gets or sets the resolved country data.</summary>
        public ICountry CountryData { get; set; }

        /// <summary>
        /// Creates a copy of an address using the values exposed by <see cref="IAddress" />.
        /// </summary>
        /// <param name="source">The address to copy.</param>
        /// <returns>A new <see cref="SimpleAddress" /> containing the source values.</returns>
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
