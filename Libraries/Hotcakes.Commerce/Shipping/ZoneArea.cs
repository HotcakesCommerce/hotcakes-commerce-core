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

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Defines one country and optional region component of a <see cref="Zone"/>.
    /// Values are serialized as part of the zone's area data and are compared case-insensitively by
    /// <see cref="ZoneRepository.FindAllZonesForAddress"/>.
    /// </summary>
    public class ZoneArea
    {
        /// <summary>
        /// Gets or sets the ISO alpha-3 country code used for matching.
        /// </summary>
        public string CountryIsoAlpha3 { get; set; }

        /// <summary>
        /// Gets or sets the region abbreviation used for matching, or an empty value to match every
        /// region in <see cref="CountryIsoAlpha3"/>.
        /// </summary>
        public string RegionAbbreviation { get; set; }
    }
}
