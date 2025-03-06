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
using System.Collections.Generic;
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Social;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Category detail page which shows child category or products and other related information.
    /// </summary>
    [Serializable]
    public class CategoryPageViewModel
    {
        /// <summary>
        ///     Set Default values
        /// </summary>
        public CategoryPageViewModel()
        {
            LocalCategory = null;
            PagerData = new PagerViewModel();
            SortSelectList = new List<SelectListItem>();
            SubCategories = new List<SingleCategoryViewModel>();
            Products = new List<SingleProductViewModel>();
            Manufacturers = new List<VendorManufacturer>();
            Vendors = new List<VendorManufacturer>();
            Types = new List<ProductType>();

            LeftColumn = string.Empty;
            PreColumn = string.Empty;
            PostColumn = string.Empty;
        }

        /// <summary>
        /// If true, the current request (end-user) is authorized to edit the catalog. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public bool AuthorizedToEditCatalog { get; set; }

        /// <summary>
        /// The administration URL for viewing and editing all categories. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string CategoriesManagementUrl { get; set; }

        /// <summary>
        /// This URL can be used to directly navigate to the category performance view. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string CategoryAnalyticsUrl { get; set; }

        /// <summary>
        /// This URL can be used to directly navigate to the category editing view. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string CategoryEditUrl { get; set; }

        /// <summary>
        ///     Drill down sub categories or products JSON string for the current category
        ///     It contains all information like vendors, manufacturers, products related to the category.
        ///     More detail can be found from <see cref="DrillDownJsonModel" />
        /// </summary>
        public string DrillDownJsonModel { get; set; }

        /// <summary>
        /// This value is a legacy property that will always be an empty string and is never used by Hotcakes.
        /// </summary>
        public string LeftColumn { get; set; }

        /// <summary>
        ///     Catalog object which map to database entity in order to do different SQL operation
        /// </summary>
        public Category LocalCategory { get; set; }

        /// <summary>
        ///     A list of manufacturers for the different products found in the current category.
        /// </summary>
        public List<VendorManufacturer> Manufacturers { get; set; }

        /// <summary>
        ///     Pager model to render paging control on page
        /// </summary>
        public PagerViewModel PagerData { get; set; }

        /// <summary>
        /// This value is a legacy property that will always be an empty string and is never used by Hotcakes.
        /// </summary>
        public string PostColumn { get; set; }

        /// <summary>
        /// This value is a legacy property that will always be an empty string and is never used by Hotcakes.
        /// </summary>
        public string PreColumn { get; set; }

        /// <summary>
        ///     List of the products under current categories
        /// </summary>
        public List<SingleProductViewModel> Products { get; set; }

        /// <summary>
        /// The administration URL for viewing and editing all products. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string ProductsManagementUrl { get; set; }

        /// <summary>
        ///     List of the selected items by which needs to sorting on the page
        /// </summary>
        public List<SelectListItem> SortSelectList { get; set; }

        /// <summary>
        /// The store administration URL for the dashboard. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string StoreAdminUrl { get; set; }

        /// <summary>
        ///     If there are any sub-categories for the current category, this object will contain that list.
        /// </summary>
        public List<SingleCategoryViewModel> SubCategories { get; set; }

        /// <summary>
        ///     List the product types for the different products under current category
        /// </summary>
        public List<ProductType> Types { get; set; }

        /// <summary>
        ///     A list of vendors for the different products found in the current category.
        /// </summary>
        public List<VendorManufacturer> Vendors { get; set; }

        #region Obsolete

        /// <summary>
        ///     List of manufacturer for the different products under current category
        /// </summary>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<VendorManufacturer> Manufactures
        {
            get { return Manufacturers; }
            set { Manufacturers = value; }
        }

        /// <summary>
        ///     Social tagging controls html string for this category. It contains different
        ///     information from <see cref="ISocialItem" />
        /// </summary>
        [Obsolete("Removing in 03.04.00 or later. Previously was used for Evoq Social integration.")]
        public ISocialItem SocialItem { get; set; }

        #endregion
    }
}