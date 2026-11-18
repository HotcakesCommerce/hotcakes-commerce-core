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

namespace Hotcakes.Web
{
    /// <summary>
    /// Specifies the priority value written to a sitemap document.
    /// </summary>
    public enum SiteMapPriority
    {
        /// <summary>No priority is written.</summary>
        NotSet,
        /// <summary>Priority 1.0.</summary>
        p10,
        /// <summary>Priority 0.9.</summary>
        p9,
        /// <summary>Priority 0.8.</summary>
        p8,
        /// <summary>Priority 0.7.</summary>
        p7,
        /// <summary>Priority 0.6.</summary>
        p6,
        /// <summary>Priority 0.5.</summary>
        p5,
        /// <summary>Priority 0.4.</summary>
        p4,
        /// <summary>Priority 0.3.</summary>
        p3,
        /// <summary>Priority 0.2.</summary>
        p2,
        /// <summary>Priority 0.1.</summary>
        p1
    }
}
