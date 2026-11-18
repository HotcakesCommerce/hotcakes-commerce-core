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
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Represents one value in a product-search facet.
    /// </summary>
    /// <remarks>
    /// Identifier values are exposed as bvin strings, including identifiers converted from the
    /// database representation. Parent fields are populated for hierarchical category facets.
    /// </remarks>
    public class FacetItem
    {
        /// <summary>
        /// Gets or sets the number of matching products represented by this facet value.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Gets or sets the facet value identifier in bvin string form.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the localized display name of the facet value.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the parent facet identifier in bvin string form, when the value is hierarchical.
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// Gets or sets the localized display name of the parent facet, when one exists.
        /// </summary>
        public string ParentName { get; set; }
    }

    /// <summary>
    /// Represents a facet value together with its selected state for checkbox-based filters.
    /// </summary>
    public class CheckboxFacetItem : FacetItem
    {
        /// <summary>
        /// Initializes a checkbox facet by copying the supplied facet values.
        /// </summary>
        /// <param name="item">The facet value to copy.</param>
        /// <param name="selected">Whether the facet is selected.</param>
        /// <param name="baseCount">The count added to an unselected facet value.</param>
        public CheckboxFacetItem(FacetItem item, bool selected, int baseCount = 0)
        {
            Checked = selected;
            ParentId = item.ParentId;
            ParentName = item.ParentName;
            Id = item.Id;
            Name = item.Name;
            Count = item.Count;

            if (!selected)
            {
                Count += baseCount;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the facet is selected by the end user.
        /// </summary>
        public bool Checked { get; set; }
    }

    /// <summary>
    /// Represents a category facet value and its optional redirect URL.
    /// </summary>
    public class CategoryFacetItem : FacetItem
    {
        /// <summary>
        /// Initializes a category facet by copying the supplied facet values.
        /// </summary>
        /// <param name="item">The facet value to copy.</param>
        /// <param name="url">The URL associated with the category facet.</param>
        public CategoryFacetItem(FacetItem item, string url)
        {
            Url = url;
            ParentId = item.ParentId;
            ParentName = item.ParentName;
            Id = item.Id;
            Name = item.Name;
            Count = item.Count;
        }

        /// <summary>
        /// Gets or sets the category URL. The current implementation does not use this value for redirects.
        /// </summary>
        public string Url { get; set; }
    }

    internal class InternalFacetItem
    {
        public int Count { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }

        public FacetItem Convert()
        {
            return new FacetItem
            {
                ParentId = DataTypeHelper.GuidToBvin(ParentId),
                ParentName = ParentName,
                Id = DataTypeHelper.GuidToBvin(Id),
                Name = Name,
                Count = Count
            };
        }
    }
}
