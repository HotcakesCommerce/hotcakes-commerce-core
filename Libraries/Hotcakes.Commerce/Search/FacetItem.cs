#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
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

        public string Url { get; set; }
    }

    internal class InternalFacetItem
    {
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }

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