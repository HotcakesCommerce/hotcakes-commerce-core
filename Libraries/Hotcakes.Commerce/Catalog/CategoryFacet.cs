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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Category additional information stored in as Category property
    /// </summary>
    public class CategoryFacet
    {
        private string _CategoryId = string.Empty;
        private CategoryFacetDisplayMode _DisplayMode = CategoryFacetDisplayMode.Single;
        private string _FilterName = string.Empty;

        public CategoryFacet()
        {
            StoreId = 0;
            SortOrder = 0;
            ParentPropertyId = 0;
            PropertyId = 0;
            Id = 0;
        }

        /// <summary>
        ///     Unique identifier of the facet
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Unique identifier of the category for which this facet is created
        /// </summary>
        public string CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }

        /// <summary>
        ///     Property id of the facet
        /// </summary>
        public long PropertyId { get; set; }

        /// <summary>
        ///     Parent property id of the facet
        /// </summary>
        public long ParentPropertyId { get; set; }

        /// <summary>
        ///     Display order to show the facet information of the category on category viewer page
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        ///     Name of the filter chosen from the facet option
        /// </summary>
        public string FilterName
        {
            get { return _FilterName; }
            set { _FilterName = value; }
        }

        /// <summary>
        ///     Facet can be displayed in single or multiple mode and laid out accordingly on the page.
        /// </summary>
        public CategoryFacetDisplayMode DisplayMode
        {
            get { return _DisplayMode; }
            set { _DisplayMode = value; }
        }

        /// <summary>
        ///     Unique store identifier
        /// </summary>
        public long StoreId { get; set; }
    }
}