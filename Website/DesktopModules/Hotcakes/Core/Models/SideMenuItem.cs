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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Side menu individual item
    /// </summary>
    [Serializable]
    public class SideMenuItem
    {
        /// <summary>
        ///     Set default value
        /// </summary>
        public SideMenuItem()
        {
            Url = string.Empty;
            Name = string.Empty;
            Title = string.Empty;
            OpenInNewWindow = false;
            CssClass = string.Empty;
        }

        /// <summary>
        ///     URL of the menu item
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     System name of the menu item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Title to be displayed on the screen
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Click on menu item needs to show on new window or same window
        /// </summary>
        public bool OpenInNewWindow { get; set; }

        /// <summary>
        ///     Class applied to each menu item
        /// </summary>
        public string CssClass { get; set; }
    }
}