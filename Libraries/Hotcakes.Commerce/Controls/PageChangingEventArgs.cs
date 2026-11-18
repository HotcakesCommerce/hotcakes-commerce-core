#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

namespace Hotcakes.Commerce.Controls
{
    /// <summary>
    /// Carries the paging values associated with a requested page change.
    /// </summary>
    /// <remarks>
    /// This is a mutable data-transfer object. The properties are not range-checked;
    /// their interpretation, including whether <see cref="CurrentRow"/> is zero- or
    /// one-based, is determined by the control that raises or consumes the event.
    /// </remarks>
    public class PageChangingEventArgs
    {
        /// <summary>
        /// Gets or sets the row at which the requested page begins or is positioned.
        /// </summary>
        /// <value>The current row value supplied by the paging control.</value>
        public int CurrentRow { get; set; }

        /// <summary>
        /// Gets or sets the total number of rows available to page through.
        /// </summary>
        /// <value>The row count supplied by the paging control.</value>
        public int RowCount { get; set; }

        /// <summary>
        /// Gets or sets the number of items displayed on each page.
        /// </summary>
        /// <value>The page-size value supplied by the paging control.</value>
        public int ItemsPerPage { get; set; }
    }
}
