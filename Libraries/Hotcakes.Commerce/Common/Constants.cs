#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020 UpendoVentures, LLC
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

namespace Hotcakes.Commerce.Common
{
    public class Constants
    {
        public const string TAG_CANONICAL = "<link rel=\"canonical\" href=\"{0}\" />";
        public const string TAG_OGTITLE = "<meta property=\"og:title\" content=\"{0}\"/>";
        public const string TAG_OGTYPE = "<meta property=\"og:type\" content=\"product\"/>";
        public const string TAG_OGURL = "<meta property=\"og:url\" content=\"{0}\"/>";
        public const string TAG_OGIMAGE = "<meta property=\"og:image\" content=\"{0}\"/>";
        public const string TAG_OGSITENAME = "<meta property=\"og:site_name\" content=\"{0}\" />";
        public const string TAG_OGFBADMIN = "<meta property=\"fb:admins\" content=\"{0}\" />";
        public const string TAG_OGFBAPPID = "<meta property=\"fb:app_id\" content=\"{0}\" />";
        public const string TAG_IOGPRICEAMOUNT = "<meta property=\"og:price:amount\" content=\"{0}\" />";
        public const string TAG_IOGPRICECURRENCY = "<meta property=\"og:price:currency\" content=\"{0}\" />";
    }
}
