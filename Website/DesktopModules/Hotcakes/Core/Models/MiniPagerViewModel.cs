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
using System.ComponentModel.DataAnnotations;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Paging control needs to be shown on page
    /// </summary>
    [Serializable]
    public class MiniPagerViewModel
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
        public MiniPagerViewModel()
        {
            CurrentPage = 1;
            TotalPages = 0;
            PagerUrlFormat = string.Empty;
            PagerUrlFormatFirst = string.Empty;
        }

        /// <summary>
        ///     Total available pages based on page size and total records
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int TotalPages { get; set; }

        /// <summary>
        ///     Current page that end user needs to see
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int CurrentPage { get; set; }

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