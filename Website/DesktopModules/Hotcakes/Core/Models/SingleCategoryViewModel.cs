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
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Category image detail needs to be shown on carousel
    /// </summary>
    [Serializable]
    public class SingleCategoryViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public SingleCategoryViewModel()
        {
            AltText = string.Empty;
            IconUrl = string.Empty;
            LinkUrl = string.Empty;
            Name = string.Empty;
            OpenInNewWindow = false;
        }

        /// <summary>
        ///     Category information. More details about individual property of this object can be found at
        ///     <see cref="CategorySnapshot" />
        /// </summary>
        public CategorySnapshot LocalCategory { get; set; }

        /// <summary>
        ///     Alternate text to be shown when image not available or when mouse hover the image
        /// </summary>
        public string AltText { get; set; }

        /// <summary>
        ///     Image URL for the category
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        ///     Category Link url for redirection to category detail page
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        ///     Name of the category
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Category detail page needs to be opened on same page or on new tab
        /// </summary>
        public bool OpenInNewWindow { get; set; }
    }
}