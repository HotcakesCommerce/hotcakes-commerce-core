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
            Manufactures = new List<VendorManufacturer>();
            Vendors = new List<VendorManufacturer>();
            Types = new List<ProductType>();

            LeftColumn = string.Empty;
            PreColumn = string.Empty;
            PostColumn = string.Empty;
        }

        /// <summary>
        ///     Catalog object which map to database entity in order to do different
        ///     SQL operation
        /// </summary>
        public Category LocalCategory { get; set; }

        /// <summary>
        ///     Pager model to render paging control on page
        /// </summary>
        public PagerViewModel PagerData { get; set; }

        /// <summary>
        ///     List of the selected items by which needs to sorting on the page
        /// </summary>
        public List<SelectListItem> SortSelectList { get; set; }

        /// <summary>
        ///     List of the subcategories under the current category
        /// </summary>
        public List<SingleCategoryViewModel> SubCategories { get; set; }

        /// <summary>
        ///     List of the products under current categories
        /// </summary>
        public List<SingleProductViewModel> Products { get; set; }

        /// <summary>
        ///     List of manufacturer for the different products under current category
        /// </summary>
        public List<VendorManufacturer> Manufactures { get; set; }

        /// <summary>
        ///     List of vendors for the different products under current category
        /// </summary>
        public List<VendorManufacturer> Vendors { get; set; }

        /// <summary>
        ///     List the product types for the different products under current category
        /// </summary>
        public List<ProductType> Types { get; set; }

        /// <summary>
        ///     Social tagging controls html string for this category. It contains different
        ///     information from <see cref="ISocialItem" />
        /// </summary>
        public ISocialItem SocialItem { get; set; }

        public string LeftColumn { get; set; }
        public string PreColumn { get; set; }
        public string PostColumn { get; set; }

        /// <summary>
        ///     Drill down sub categories or products JSON string for the current category
        ///     It contains all information like vendors, manufacturers, products related to the catgory.
        ///     More detail can be found from <see cref="DrillDownJsonModel" />
        /// </summary>
        public string DrillDownJsonModel { get; set; }
    }
}