#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Search;

namespace Hotcakes.Modules.Core.Models.Json
{
    [Serializable]
    public class DrillDownJsonModel
    {
        private List<SelectedFacetItem> _SelectedManufacturers;

        public DrillDownJsonModel()
        {
            PagerData = new PagerViewModel();
        }

        /// <summary>
        /// A list of manufacturers for the different products found in the current category.
        /// </summary>
        public List<CheckboxFacetItem> Manufacturers { get; set; }

        /// <summary>
        /// This property will contain the highest price available for all of the products that match the current selections in the drill-down view.
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// This property will contain the lowest price available for all of the products that match the current selections in the drill-down view.
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// A model used to render the paging control in the view.
        /// </summary>
        public PagerViewModel PagerData { get; set; }

        /// <summary>
        /// A list of the products that are associated with the current category.
        /// </summary>
        public List<SingleProductJsonModel> Products { get; set; }

        /// <summary>
        /// A model used to render any product type properties to the view.
        /// </summary>
        public List<PropertyFacetItem> Properties { get; set; }

        /// <summary>
        /// If any manufacturers have been selected by the customer, they will be in this list.
        /// </summary>
        public List<SelectedFacetItem> SelectedManufacturers
        {
            get { return _SelectedManufacturers; }
            set { _SelectedManufacturers = value; }
        }

        /// <summary>
        /// If the customer uses the price slider to filter the drill-down results, this value will reflect the highest price of the products in the results.
        /// </summary>
        public decimal SelectedMaxPrice { get; set; }

        /// <summary>
        /// If the customer uses the price slider to filter the drill-down results, this value will reflect the lowest price of the products in the results.
        /// </summary>
        public decimal SelectedMinPrice { get; set; }

        /// <summary>
        /// If the customer filters the drill-down results using product type properties, this list will contain each of those properties.
        /// </summary>
        public List<SelectedPropertyFacetItem> SelectedProperties { get; set; }

        /// <summary>
        /// If the customer filters the drill-down results using product types, this list will contain each of those types.
        /// </summary>
        public List<SelectedFacetItem> SelectedTypes { get; set; }

        /// <summary>
        /// If the customer filters the drill-down results by selecting one or more vendors, this list will contain each of those vendors.
        /// </summary>
        public List<SelectedFacetItem> SelectedVendors { get; set; }

        /// <summary>
        /// This property contains the default sort for the view, as set in the category in the store administration. If the customer chooses a different sort order, that value will be in this property instead.
        /// </summary>
        public CategorySortOrder SortOrder { get; set; }

        /// <summary>
        /// If there are any sub-categories for the current category, this object will contain that list.
        /// </summary>
        public List<CategoryMenuItemViewModel> SubCategories { get; set; }

        /// <summary>
        /// This property will reflect the total number of products that match the current selections by the customer, or the total number of products before selections have been made.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// If there are any product types found matching any of the products returned, they will be included in this property.
        /// </summary>
        public List<CheckboxFacetItem> Types { get; set; }

        /// <summary>
        /// If there are any vendors found matching any of the products returned, they will be included in this property.
        /// </summary>
        public List<CheckboxFacetItem> Vendors { get; set; }

        #region Obsolete

        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<CheckboxFacetItem> Manufactures
        {
            get { return Manufacturers; }
            set { Manufacturers = value; }
        }

        #endregion
    }
}