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

namespace Hotcakes.Commerce.Content
{
    /// <summary>Represents a store-scoped custom URL redirect.</summary>
    public class CustomUrl
    {
        private string _RedirectToUrl = string.Empty;

        private string _RequestedUrl = string.Empty;

        /// <summary>Initializes a custom URL with a UTC timestamp, permanent redirect semantics, and general URL type.</summary>
        public CustomUrl()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            IsPermanentRedirect = true;
            SystemDataType = CustomUrlType.General;
            SystemData = string.Empty;
            StoreId = 0;
        }

        /// <summary>Gets or sets the unique identifier for this URL record.</summary>
        public string Bvin { get; set; }
        /// <summary>Gets or sets the UTC time when this record was last updated.</summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>Gets or sets whether the redirect is permanent.</summary>
        public bool IsPermanentRedirect { get; set; }
        /// <summary>Gets or sets the type of commerce data associated with the URL.</summary>
        public CustomUrlType SystemDataType { get; set; }
        /// <summary>Gets or sets the associated system object's identifier.</summary>
        public string SystemData { get; set; }
        /// <summary>Gets or sets the store that owns this URL.</summary>
        public long StoreId { get; set; }

        /// <summary>Gets or sets the requested path, trimming trailing slashes and normalizing it to lowercase invariant text.</summary>
        /// <remarks>Whitespace-only values become an empty string. Leading slashes and other characters are preserved.</remarks>
        public string RequestedUrl
        {
            get { return _RequestedUrl; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var trimValue = "/";
                    _RequestedUrl = value.TrimEnd(trimValue.ToCharArray());
                }
                else
                {
                    _RequestedUrl = string.Empty;
                }
                _RequestedUrl = _RequestedUrl.ToLowerInvariant();
            }
        }

        /// <summary>Gets or sets the redirect destination, trimming trailing slashes and normalizing it to lowercase invariant text.</summary>
        /// <remarks>Whitespace-only values become an empty string. Leading slashes and other characters are preserved.</remarks>
        public string RedirectToUrl
        {
            get { return _RedirectToUrl; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var trimValue = "/";
                    _RedirectToUrl = value.TrimEnd(trimValue.ToCharArray());
                }
                else
                {
                    _RedirectToUrl = string.Empty;
                }
                _RedirectToUrl = _RedirectToUrl.ToLowerInvariant();
            }
        }
    }
}
