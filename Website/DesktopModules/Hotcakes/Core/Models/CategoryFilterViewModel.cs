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
using System.Collections.Generic;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.People;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Provide way to filter the category on the view.
    /// </summary>
    [Serializable]
    public class CategoryFilterViewModel
    {
        /// <summary>
        ///     There is one flag to determine whether specific category is searchable or
        ///     not. We can set this flag to overwrite or ignore IsSearchable = false.
        /// </summary>
        public bool IsConsiderSearchable = true;

        /// <summary>
        ///     Category is searchable by hotcakes search provider or not.
        /// </summary>
        public bool IsSearchable = true;

        /// <summary>
        ///     The unique ID of the module where the current drill down filter is being used.
        /// </summary>
        public int ModuleId { get; set; }

        /// <summary>
        ///     Selected category id
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        ///     List of the manufacturers which needs to be shown in the drill down filter.
        /// </summary>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<string> Manufactures {
            get { return Manufacturers; }
            set { Manufacturers = value; }
        }

        /// <summary>
        ///     List of the manufacturers which needs to be shown in the drill down filter.
        /// </summary>
        public List<string> Manufacturers { get; set; }

        /// <summary>
        ///     List of the vendors which needs to be shown in the drill down filter.
        /// </summary>
        public List<string> Vendors { get; set; }

        /// <summary>
        ///     List of category types for the filter.
        /// </summary>
        public List<string> Types { get; set; }

        /// <summary>
        ///     Properties of the category(ies) in JSON format to use inside the drill down view filter.
        /// </summary>
        public string PropertiesJson { get; set; }

        /// <summary>
        ///     Minimum price by which the drill down view needs to filter the list of products.
        /// </summary>
        public decimal? MinPrice { get; set; }

        /// <summary>
        ///     Maximum price by which needs to filter the list of products.
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        ///     Represents which page is currently being displayed. Default is 1.
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        ///     Used to sort the products displayed in the category drill down view <see cref="SortOrder" />
        /// </summary>
        public CategorySortOrder SortOrder { get; set; }
    }
}