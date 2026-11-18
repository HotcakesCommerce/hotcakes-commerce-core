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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Describes a store's shipping region as a named collection of <see cref="ZoneArea"/> entries.
    /// A zone matches an address when one of its areas matches the address country and, when specified,
    /// its region abbreviation; see <see cref="ZoneRepository.FindAllZonesForAddress"/>.
    /// </summary>
    public class Zone
    {
        /// <summary>
        /// Initializes an empty zone with a new area list and default identifiers and name.
        /// </summary>
        public Zone()
        {
            Id = 0;
            StoreId = 0;
            Name = string.Empty;
            Areas = new List<ZoneArea>();
        }

        /// <summary>
        /// Gets or sets the zone identifier. Negative identifiers identify built-in zones.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the store identifier that owns this zone.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the zone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the country and region entries that define this zone's address contract.
        /// An empty region in an area means every region in that area’s country.
        /// </summary>
        public List<ZoneArea> Areas { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a built-in zone represented by a negative identifier.
        /// </summary>
        public bool IsBuiltInZone
        {
            get { return Id < 0; }
        }
    }
}
