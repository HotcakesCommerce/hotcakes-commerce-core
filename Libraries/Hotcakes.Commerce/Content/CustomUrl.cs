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

namespace Hotcakes.Commerce.Content
{
    public class CustomUrl
    {
        private string _RedirectToUrl = string.Empty;

        private string _RequestedUrl = string.Empty;

        public CustomUrl()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            IsPermanentRedirect = true;
            SystemDataType = CustomUrlType.General;
            SystemData = string.Empty;
            StoreId = 0;
        }

        public string Bvin { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsPermanentRedirect { get; set; }
        public CustomUrlType SystemDataType { get; set; }
        public string SystemData { get; set; }
        public long StoreId { get; set; }

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