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
using Hotcakes.Web.Geography;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     This object is used to hold a minimal amount of mapped information about a Region and Country combination in other
    ///     objects.
    /// </summary>
    [Serializable]
    public class Region : IRegion
    {
        /// <summary>
        ///     The ID or bvin of the region.
        /// </summary>
        public Guid RegionId { get; set; }

        /// <summary>
        ///     The ID or bvin of the country.
        /// </summary>
        public Guid CountryId { get; set; }

        private string _displayName { get; set; }

        /// <summary>
        ///     This value is used to globally identify this field on localized sites.
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        ///     The standardized abbreviation of the region.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        ///     This is the display name specified in a localized site, otherwise the SystemName is used.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                    return SystemName;
                return _displayName;
            }
            set { _displayName = value; }
        }
    }
}