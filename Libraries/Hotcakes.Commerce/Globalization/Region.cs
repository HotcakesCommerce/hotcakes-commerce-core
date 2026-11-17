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

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     Represents the mapped identity and localized display data for a region within a country.
    /// </summary>
    /// <remarks>
    ///     <see cref="RegionId"/> and <see cref="CountryId"/> identify the persisted records. <see cref="DisplayName"/>
    ///     exposes the localized name when available and otherwise falls back to <see cref="SystemName"/>.
    ///     The object is serializable for compatibility with existing commerce and API payloads.
    /// </remarks>
    [Serializable]
    public class Region : IRegion
    {
        /// <summary>
        ///     Gets or sets the persisted identifier of the region.
        /// </summary>
        /// <value>The region identifier used by the region repository.</value>
        public Guid RegionId { get; set; }

        /// <summary>
        ///     Gets or sets the identifier of the country that owns the region.
        /// </summary>
        /// <value>The parent country identifier used for region filtering and mapping.</value>
        public Guid CountryId { get; set; }

        private string _displayName { get; set; }

        /// <summary>
        ///     Gets or sets the stable, non-localized region name.
        /// </summary>
        /// <value>The system name used as the localization fallback and global identity label.</value>
        public string SystemName { get; set; }

        /// <summary>
        ///     Gets or sets the standardized region abbreviation.
        /// </summary>
        /// <value>The abbreviation used when a caller looks up a region through <see cref="Country.FindRegion(string)"/>.</value>
        public string Abbreviation { get; set; }

        /// <summary>
        ///     Gets or sets the localized region name displayed to customers.
        /// </summary>
        /// <remarks>
        ///     When the localized value is null or empty, the getter returns <see cref="SystemName"/>. The setter stores
        ///     the localized value without changing the system name.
        /// </remarks>
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