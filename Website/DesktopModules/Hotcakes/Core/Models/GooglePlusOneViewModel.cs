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

namespace Hotcakes.Modules.Core.Models
{
    public enum GooglePlusOneSize
    {
        Small = 0,
        Medium = 1,
        Standard = 2,
        Large = 3
    }

    public enum GooglePlusOneCountPosition
    {
        None = 0,
        Bubble = 1,
        Inline = 2
    }

    /// <summary>
    ///     This class used to show the google plus one widget on the screen
    /// </summary>
    [Serializable]
    public class GooglePlusOneViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public GooglePlusOneViewModel()
        {
            PageUrl = string.Empty;
            Size = GooglePlusOneSize.Standard;
            CountPosition = GooglePlusOneCountPosition.Inline;
        }

        /// <summary>
        ///     current page url
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        ///     Size of the widget needs to be displayed on the page.
        /// </summary>
        public GooglePlusOneSize Size { get; set; }

        /// <summary>
        ///     Specify where needs to show the counts for the google plus
        /// </summary>
        public GooglePlusOneCountPosition CountPosition { get; set; }
    }
}