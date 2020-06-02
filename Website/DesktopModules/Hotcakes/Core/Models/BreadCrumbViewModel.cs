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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The BreadCrumbItem object is used to populate a collection of navigation items used in the BreadCrumbViewModel.
    /// </summary>
    [Serializable]
    public class BreadCrumbItem
    {
        /// <summary>
        ///     Set default value to all properties
        /// </summary>
        public BreadCrumbItem()
        {
            Name = string.Empty;
            Link = string.Empty;
            Title = string.Empty;
        }

        /// <summary>
        ///     This can be the name of the any entity shown as a breadcrumb item such as the product name, category name etc.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Link to a specific page for the current breadcrumb item
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        ///     Shown to the end user as breadcrumb text
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    ///     This class used to represent the items needs to be shown as navigation links on the
    ///     page when user visit nested pages.
    /// </summary>
    [Serializable]
    public class BreadCrumbViewModel
    {
        /// <summary>
        ///     Default values
        /// </summary>
        public BreadCrumbViewModel()
        {
            HideHomeLink = false;
            Items = new Queue<BreadCrumbItem>();
        }

        /// <summary>
        ///     Show Home link or not. In some pages and scenarios its required to hide the
        ///     home link. That can be controlled from this variable
        /// </summary>
        public bool HideHomeLink { get; set; }

        /// <summary>
        ///     List of the items which are shown in navigation. As a user drills down to nested
        ///     view, this list get updated each time and enter the new value to list. Remove the
        ///     item if user move back to the upper level again.
        /// </summary>
        public Queue<BreadCrumbItem> Items { get; private set; }
    }
}