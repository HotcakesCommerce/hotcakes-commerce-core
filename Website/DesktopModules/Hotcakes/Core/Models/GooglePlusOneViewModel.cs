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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    /// Defines the supported values for GooglePlusOneSize.
    /// </summary>
    public enum GooglePlusOneSize
    {
        /// <summary>
        /// Represents the Small option.
        /// </summary>
        Small = 0,
        /// <summary>
        /// Represents the Medium option.
        /// </summary>
        Medium = 1,
        /// <summary>
        /// Represents the Standard option.
        /// </summary>
        Standard = 2,
        /// <summary>
        /// Represents the Large option.
        /// </summary>
        Large = 3
    }

    /// <summary>
    /// Defines the supported values for GooglePlusOneCountPosition.
    /// </summary>
    public enum GooglePlusOneCountPosition
    {
        /// <summary>
        /// Represents the None option.
        /// </summary>
        None = 0,
        /// <summary>
        /// Represents the Bubble option.
        /// </summary>
        Bubble = 1,
        /// <summary>
        /// Represents the Inline option.
        /// </summary>
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
