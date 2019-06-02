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
using System.ComponentModel.DataAnnotations;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Pager information on different screens
    /// </summary>
    [Serializable]
    public class PagerViewModel
    {
        /// <summary>
        ///     If the page is very first page then to get the data for the first page it's not required to pass the
        ///     page number and so the format of the URL is different.
        ///     User can configure separately this first page URL with this.
        ///     If this is not available then by default PageURLForamt will be used by system
        /// </summary>
        private string _PagerUrlFormatFirst = string.Empty;

        /// <summary>
        ///     Set default values
        /// </summary>
        public PagerViewModel()
        {
            PageRange = 15;
            PageSize = 9;
            CurrentPage = 1;
            TotalItems = 0;
            PagerUrlFormat = string.Empty;
            PagerUrlFormatFirst = string.Empty;
        }

        /// <summary>
        ///     Number of records per page
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int PageSize { get; set; }

        /// <summary>
        ///     The page number which currently user want to looking for
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int CurrentPage { get; set; }

        /// <summary>
        ///     Total number of items available
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int TotalItems { get; set; }

        /// <summary>
        ///     used to identify how many
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int PageRange { get; set; }

        /// <summary>
        ///     Total number of available pages based on the size of page and number of available records
        /// </summary>
        public int TotalPages
        {
            get { return Paging.TotalPages(TotalItems, PageSize); }
        }

        /// <summary>
        ///     URL format for the pager. All URL in system are the multilingual supported
        ///     so needs to format the URL format with tokens in order to get replaced with proper values
        ///     at run time.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PagerUrlFormat { get; set; }

        // Url for first page 
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PagerUrlFormatFirst
        {
            get
            {
                if (_PagerUrlFormatFirst.Trim().Length < 1) return PagerUrlFormat;
                return _PagerUrlFormatFirst;
            }
            set { _PagerUrlFormatFirst = value; }
        }
    }
}