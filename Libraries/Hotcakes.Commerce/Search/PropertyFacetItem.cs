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

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Represents a product property facet and its available values.
    /// </summary>
    public class PropertyFacetItem
    {
        /// <summary>
        /// Initializes a property facet with an empty list of values.
        /// </summary>
        public PropertyFacetItem()
        {
            FacetItems = new List<FacetItem>();
        }

        /// <summary>
        /// Gets or sets the localized display name of the property.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the available values for this property facet.
        /// </summary>
        public List<FacetItem> FacetItems { get; set; }
        /// <summary>
        /// Gets or sets the property identifier.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the internal store name of the property.
        /// </summary>
        public string PropertyName { get; set; }
    }
}
