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
    ///     The various category views may list categories for a customer to explore.  When
    ///     they do, the CategoryMenuItemViewModel is used to render each individual category
    ///     in the category menu.
    /// </summary>
    [Serializable]
    public class CategoryMenuItemViewModel
    {
        private List<CategoryMenuItemViewModel> _items;

        /// <summary>
        ///     User friendly name of the category set in the administration area.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Description of the category which is shown when a specific category is chosen.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     URL related to this category based on the current culture selected by the customer.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Used to indicate if the customer is currently viewing this category or
        ///     another one.
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        ///     Number of products under this category even after applying different
        ///     filters. Shown when the module settings allow it to be displayed.
        /// </summary>
        public int ProductsCount { get; set; }

        /// <summary>
        ///     Unique identifier of the category.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     If there are list of sub/child categories then this list represent all
        ///     child category information.
        /// </summary>
        public List<CategoryMenuItemViewModel> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<CategoryMenuItemViewModel>();
                return _items;
            }
        }
    }
}