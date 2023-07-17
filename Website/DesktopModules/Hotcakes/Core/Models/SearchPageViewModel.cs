#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Search;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Search page view details
    /// </summary>
    [Serializable]
    public class SearchPageViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public SearchPageViewModel()
        {
            LocalCategory = null;
            PagerData = new PagerViewModel();
            SortSelectList = new List<SelectListItem>();
            SubCategories = new List<SingleCategoryViewModel>();
            Products = new List<SingleProductViewModel>();

            Categories = new List<FacetItem>();
            Manufacturers = new List<FacetItem>();
            Vendors = new List<FacetItem>();
            Types = new List<FacetItem>();
            Properties = new List<PropertyFacetItem>();

            SelectedCategories = new List<SelectedFacetItem>();
            SelectedManufacturers = new List<SelectedFacetItem>();
            SelectedVendors = new List<SelectedFacetItem>();
            SelectedTypes = new List<SelectedFacetItem>();
            SelectedProperties = new List<SelectedPropertyFacetItem>();

            CurrentRouteValues = new RouteValueDictionary();
        }

        /// <summary>
        ///     Current chosen category.
        /// </summary>
        public Category LocalCategory { get; set; }

        /// <summary>
        ///     Pager control for the list of searched product list.
        /// </summary>
        public PagerViewModel PagerData { get; set; }

        /// <summary>
        ///     Sorted list of category and urls
        /// </summary>
        public List<SelectListItem> SortSelectList { get; set; }

        /// <summary>
        ///     List of subcategories to be shown for chosen category.
        /// </summary>
        public List<SingleCategoryViewModel> SubCategories { get; set; }

        /// <summary>
        ///     List of products available for the search.
        /// </summary>
        public List<SingleProductViewModel> Products { get; set; }

        /// <summary>
        ///     Flag to indicate whether Manufacturers needs to be shown on filter panel.
        /// </summary>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the ShowManufacturers property instead. Removing in version 03.04.00 or later.")]
        public bool ShowManufactures
        {
            get { return ShowManufacturers;}
            set { ShowManufacturers = value; }
        } 

        /// <summary>
        ///     Flag to indicate whether Manufacturers needs to be shown on filter panel.
        /// </summary>
        public bool ShowManufacturers { get; set; }

        /// <summary>
        ///     Flag to indicate whether Vendors needs to be shown on filter panel.
        /// </summary>
        public bool ShowVendors { get; set; }

        /// <summary>
        ///     List of available categories. More detail for category on filter panel can be found at <see cref="FacetItem" />.
        /// </summary>
        public List<FacetItem> Categories { get; set; }

        /// <summary>
        ///     List of available manufacturers. More detail for manufacturer on filter panel can be found at
        ///     <see cref="FacetItem" />.
        /// </summary>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<FacetItem> Manufactures {
            get { return Manufacturers; }
            set { Manufacturers = value; }
        }

        /// <summary>
        ///     List of available manufacturers. More detail for manufacturer on filter panel can be found at
        ///     <see cref="FacetItem" />.
        /// </summary>
        public List<FacetItem> Manufacturers { get; set; }

        /// <summary>
        ///     List of available vendors. More detail for Vendor on filter panel can be found at <see cref="FacetItem" />.
        /// </summary>
        public List<FacetItem> Vendors { get; set; }

        /// <summary>
        ///     List of available product types.  More detail for Types on filter panel can be found at <see cref="FacetItem" />.
        /// </summary>
        public List<FacetItem> Types { get; set; }

        /// <summary>
        ///     List of properties information for the product. Each product has product type and product properties bind to type.
        ///     Based on filter record
        ///     left side shows the available product properties information. More information about individual can be found at
        ///     <see cref="PropertyFacetItem" />.
        /// </summary>
        public List<PropertyFacetItem> Properties { get; set; }

        /// <summary>
        ///     Minimum price filter.
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        ///     Max price filter.
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        ///     Chosen categories from filter panel.
        /// </summary>
        public List<SelectedFacetItem> SelectedCategories { get; set; }

        /// <summary>
        ///     Chosen manufacturers from filter panel.
        /// </summary>
        [Obsolete(
            "Deprecated in Hotcakes Commerce 03.03.00. Please use the SelectedManufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<SelectedFacetItem> SelectedManufactures
        {
            get
            { return SelectedManufacturers; }
            set { SelectedManufacturers = value; }
        }

        /// <summary>
        ///     Chosen manufacturers from filter panel.
        /// </summary>
        public List<SelectedFacetItem> SelectedManufacturers { get; set; }

        /// <summary>
        ///     Chosen vendors from the filter panel.
        /// </summary>
        public List<SelectedFacetItem> SelectedVendors { get; set; }

        /// <summary>
        ///     chosen types from the filter panel.
        /// </summary>
        public List<SelectedFacetItem> SelectedTypes { get; set; }

        /// <summary>
        ///     Chosen properties from the filter panel.
        /// </summary>
        public List<SelectedPropertyFacetItem> SelectedProperties { get; set; }

        /// <summary>
        ///     Chosen min price for filter.
        /// </summary>
        public decimal SelectedMinPrice { get; set; }

        /// <summary>
        ///     Chosen max price for filter.
        /// </summary>
        public decimal SelectedMaxPrice { get; set; }

        /// <summary>
        ///     URL for the different "search", "categories", "types", "manufacturers", "vendors", "minprice", "maxprice"  search
        ///     option.
        /// </summary>
        public RouteValueDictionary CurrentRouteValues { get; set; }
    }
}