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

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    /// <summary>
    /// Defines the administrative class MenuItem.
    /// </summary>
    [Serializable]
    public class MenuItem
    {
        private const string nullUrl = "javascript:void(0);";
        private const string adminUrlBase = "~/DesktopModules/Hotcakes/Core/Admin/";

        public string Name { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public string Text { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public string Url { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public string PermissionToken { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public bool HiddenInDnnMenu { get; set; }
        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public List<MenuItem> ChildItems { get; set; }

        /// <summary>
        /// Performs the administrative operation represented by GetUrl.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public string GetUrl()
        {
            if (string.IsNullOrEmpty(Url))
            {
                return nullUrl;
            }

            if (Url != null && Url.StartsWith("~/"))
            {
                return Url;
            }

            return string.Concat(adminUrlBase, Url);
        }
    }
}
