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

using System;
using System.Text;

namespace Hotcakes.Web
{
    /// <summary>
    /// Provides helpers for calculating page offsets and rendering HTML pagers.
    /// </summary>
    [Serializable]
    public class Paging
    {
        /// <summary>
        /// Calculates the number of pages needed for a record count and page size.
        /// </summary>
        /// <param name="totalRecords">The total number of records.</param>
        /// <param name="pageSize">The number of records on each page.</param>
        /// <returns>The calculated page count, with at least one page.</returns>
        public static int TotalPages(int totalRecords, int pageSize)
        {
            var result = 1;

            if (totalRecords < pageSize)
            {
                return result;
            }
            var wholePages = (int) Math.Floor(totalRecords/(double) pageSize);
            var partialPages = (int) (totalRecords%(double) pageSize);
            if (partialPages > 0)
            {
                result = wholePages + 1;
            }
            else
            {
                result = wholePages;
            }

            return result;
        }

        /// <summary>
        /// Calculates the zero-based starting row index for a page.
        /// </summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The number of records on each page.</param>
        /// <returns>The zero-based starting row index.</returns>
        public static int StartRowIndex(int pageNumber, int pageSize)
        {
            var pageIndex = pageNumber - 1;
            if (pageIndex < 0) pageIndex = 0;
            return pageIndex*pageSize;
        }

        /// <summary>
        /// Renders an HTML pager based on a record count and page size.
        /// </summary>
        /// <param name="link">A composite-format link containing the page number placeholder.</param>
        /// <param name="currentPage">The current one-based page number.</param>
        /// <param name="totalRecords">The total number of records.</param>
        /// <param name="pageSize">The number of records on each page.</param>
        /// <returns>The rendered pager markup, or an empty string when paging is unnecessary.</returns>
        /// <see cref="RenderPager(string, int, int)" />
        public static string RenderPager(string link, int currentPage, int totalRecords, int pageSize)
        {
            return RenderPager(link, currentPage, TotalPages(totalRecords, pageSize));
        }

        /// <summary>
        /// Renders an HTML pager for a known page count using the default display limit.
        /// </summary>
        /// <param name="link">A composite-format link containing the page number placeholder.</param>
        /// <param name="currentPage">The current one-based page number.</param>
        /// <param name="pages">The total number of pages.</param>
        /// <returns>The rendered pager markup, or an empty string when paging is unnecessary.</returns>
        /// <see cref="RenderPagerWithLimits(string, int, int, int)" />
        public static string RenderPager(string link, int currentPage, int pages)
        {
            return RenderPagerWithLimits(link, currentPage, pages, 10);
        }

        /// <summary>
        /// Renders an HTML pager from a record count with a maximum number of visible pages.
        /// </summary>
        /// <param name="link">A composite-format link containing the page number placeholder.</param>
        /// <param name="currentPage">The current one-based page number.</param>
        /// <param name="totalRecords">The total number of records.</param>
        /// <param name="pageSize">The number of records on each page.</param>
        /// <param name="maxPagesToShow">The maximum number of page links in one group.</param>
        /// <returns>The rendered pager markup, or an empty string when paging is unnecessary.</returns>
        /// <see cref="TotalPages(int, int)" />
        /// <see cref="RenderPagerWithLimits(string, int, int, int)" />
        public static string RenderPagerWithLimits(string link, int currentPage, int totalRecords, int pageSize,
            int maxPagesToShow)
        {
            return RenderPagerWithLimits(link, currentPage, TotalPages(totalRecords, pageSize), maxPagesToShow);
        }

        /// <summary>
        /// Renders an HTML pager for a known page count with a maximum number of visible pages.
        /// </summary>
        /// <param name="link">A composite-format link containing the page number placeholder.</param>
        /// <param name="currentPage">The current one-based page number.</param>
        /// <param name="pages">The total number of pages.</param>
        /// <param name="maxPagesToShow">The maximum number of page links in one group.</param>
        /// <returns>The rendered pager markup, or an empty string when paging is unnecessary.</returns>
        public static string RenderPagerWithLimits(string link, int currentPage, int pages, int maxPagesToShow)
        {
            // don't render a pager if we don't need one
            if (pages < 2) return string.Empty;

            // Make sure current page is within limits
            var current = currentPage;
            if (current < 1) current = 1;
            if (current > pages) current = pages;

            // Truncate to 10 pages at a time
            decimal limitToShow = maxPagesToShow;
            var pageOfPages = Math.Floor((currentPage - 1)/limitToShow);
            var startPage = (int) (limitToShow*pageOfPages + 1);
            var endingPage = startPage + ((int) limitToShow - 1);
            if (endingPage > pages) endingPage = pages;


            var sb = new StringBuilder();
            sb.Append("<div class=\"hcPager\"><ul>");

            // Render previous page groups
            if (startPage > limitToShow)
            {
                sb.Append("<li><a href=\"" + string.Format(link, startPage - 1) + "\">...</a></li>");
            }

            for (var i = startPage; i <= endingPage; i++)
            {
                sb.Append("<li");
                if (current == i)
                {
                    sb.Append(" class=\"hcCurrent\"");
                }
                sb.Append(">");
                sb.Append("<a href=\"" + string.Format(link, i) + "\">" + i + "</a></li>");
            }

            // Render more pages if available
            if (pages > endingPage)
            {
                sb.Append("<li><a href=\"" + string.Format(link, endingPage + 1) + "\">...</a></li>");
            }

            sb.Append("</ul></div>");

            return sb.ToString();
        }
    }
}
