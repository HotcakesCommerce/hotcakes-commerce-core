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
using System.Collections.Generic;

namespace Hotcakes.Modules.Core.Areas.ContentBlocks.Models
{
    /// <summary>
    /// Represents one image and its link in an image-rotator content block.
    /// </summary>
    [Serializable]
    public class ImageRotatorImageViewModel
    {
        /// <summary>
        /// Represents one image and its link in an image-rotator content block.
        /// </summary>
        public ImageRotatorImageViewModel()
        {
            ImageUrl = string.Empty; // 1            
            Url = string.Empty; // 2
            NewWindow = false; // 3
            Caption = string.Empty; // 4
        }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Gets or sets the image caption.
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// Gets or sets the destination URL for the image.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the destination opens in a new window.
        /// </summary>
        public bool NewWindow { get; set; }
    }
    /// <summary>
    /// Represents the data displayed by an image-rotator content block.
    /// </summary>

    [Serializable]
    public class ImageRotatorViewModel
    {
        /// <summary>
        /// Represents the data displayed by an image-rotator content block.
        /// </summary>
        public ImageRotatorViewModel()
        {
            Pause = 3;
            Images = new List<ImageRotatorImageViewModel>();
            CssClass = string.Empty;
            CssId = string.Empty;
            Height = 0;
            Width = 0;
        }

        /// <summary>
        /// Gets or sets the images displayed by the content block.
        /// </summary>
        public List<ImageRotatorImageViewModel> Images { get; set; }
        /// <summary>
        /// Gets or sets the HTML element identifier for the rotator.
        /// </summary>
        public string CssId { get; set; }
        /// <summary>
        /// Gets or sets the CSS class applied to the rotator.
        /// </summary>
        public string CssClass { get; set; }
        /// <summary>
        /// Gets or sets the rotator height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Gets or sets the rotator width.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the rotator pause interval.
        /// </summary>
        public int Pause { get; set; }
    }
}
