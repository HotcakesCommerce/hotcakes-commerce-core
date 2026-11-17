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

using System.Diagnostics.CodeAnalysis;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Describes an image's dimensions, file size, and optional path information.
    /// </summary>
    public class ImageInfo
    {
        private string _FormattedDimensions = string.Empty;
        private string _FormattedSize = string.Empty;
        private string _PhysicalPath = string.Empty;
        private string _Url = string.Empty;

        /// <summary>
        /// Initializes an empty image-information object with a zero byte size.
        /// </summary>
        public ImageInfo()
        {
            SizeInBytes = 0m;
        }

        /// <summary>
        /// Gets or sets the image width in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the image height in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the image file size in bytes.
        /// </summary>
        public decimal SizeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the display-formatted file size, such as <c>12 KB</c>.
        /// </summary>
        /// <value>A display string; the class does not calculate or normalize assigned values.</value>
        public string FormattedSize
        {
            get { return _FormattedSize; }
            set { _FormattedSize = value; }
        }

        /// <summary>
        /// Gets or sets the display-formatted dimensions, such as <c>640 x 480</c>.
        /// </summary>
        public string FormattedDimensions
        {
            get { return _FormattedDimensions; }
            set { _FormattedDimensions = value; }
        }

        /// <summary>
        /// Gets or sets the physical file-system path for the image.
        /// </summary>
        public string PhysicalPath
        {
            get { return _PhysicalPath; }
            set { _PhysicalPath = value; }
        }

        /// <summary>
        /// Gets or sets the URL used to access the image.
        /// </summary>
        /// <value>A URL string; this property remains a string for compatibility with the existing API.</value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
    }
}
