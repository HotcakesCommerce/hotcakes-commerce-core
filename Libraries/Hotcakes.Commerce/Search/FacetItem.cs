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
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Search
{
    public class FacetItem
    {
        /// <summary>
        /// This property reflects the total number of child facet items for the current facet item.
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// This is the unique identifier for the current facet item.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// This is the localized name of the current facet item.  
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// If this facet item has a parent, it's parent ID will be in this property. 
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// If this facet item has a parent, this property will contain the localized name of the parent facet item.  
        /// </summary>
        public string ParentName { get; set; }
    }

    public class CheckboxFacetItem : FacetItem
    {
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
        /// If true, the current facet has been selected by the end-user.
        /// </summary>
        public bool Checked { get; set; }
    }

    public class CategoryFacetItem : FacetItem
    {
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
        /// This property is not currently used. It is intended to be used for categories that use a redirect URL.
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