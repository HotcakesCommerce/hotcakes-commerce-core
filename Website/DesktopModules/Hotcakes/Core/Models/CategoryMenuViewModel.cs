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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Whenever a menu of the enabled categories are displayed, the CategoryMenuViewModel
    ///     is used to allow for that rendering to occur.
    /// </summary>
    [Serializable]
    public class CategoryMenuViewModel
    {
        /// <summary>
        ///     Current category menu item details <see cref="CategoryMenuItemViewModel" />
        /// </summary>
        public CategoryMenuItemViewModel MenuItem;

        /// <summary>
        ///     Set default values
        /// </summary>
        public CategoryMenuViewModel()
        {
            ShowProductCounts = false;
            ShowCategoryCounts = false;
            ShowHomeLink = false;
            CurrentId = string.Empty;
            CategoryMenuMode = "0";
            MaximumDepth = 0;
            Title = string.Empty;
            SelectedCategories = new HashSet<string>();
        }

        /// <summary>
        ///     Flag to control whether it's required to show product count to the customer
        ///     or not.
        /// </summary>
        public bool ShowProductCounts { get; set; }

        /// <summary>
        ///     Flag to control whether it's required to show the category count to the
        ///     customer or not.
        /// </summary>
        public bool ShowCategoryCounts { get; set; }

        /// <summary>
        ///     Show home link on the list of the categories or not when traversing through
        ///     nested categories.
        /// </summary>
        public bool ShowHomeLink { get; set; }

        /// <summary>
        ///     Current Category ID which the customer is viewing.
        /// </summary>
        public string CurrentId { get; set; }

        /// <summary>
        ///     User friendly name of the currently selected category set in the
        ///     administration area.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Defines the maximum number of child categories that should be rendered
        ///     on the view.
        /// </summary>
        public int MaximumDepth { get; set; }

        /// <summary>
        ///     There are different mode of display for the categories including:
        ///     1 - All Categories
        ///     2 - Peers or Children and Parents
        ///     3 - Root and expanded children
        ///     4 - Load selected categories
        ///     5 - Load children of selected
        /// </summary>
        public string CategoryMenuMode { get; set; }

        /// <summary>
        ///     List of selected categories on the view.
        /// </summary>
        public HashSet<string> SelectedCategories { get; set; }

        /// <summary>
        ///     This contains the value from the module settings that sets what the current
        ///     parent category should be for the view, which overrides the default behavior.
        /// </summary>
        public string ChildrenOfCategory { get; set; }
    }
}