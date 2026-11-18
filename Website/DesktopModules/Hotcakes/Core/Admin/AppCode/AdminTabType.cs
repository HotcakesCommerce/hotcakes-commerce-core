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

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    /// <summary>
    /// Defines the administrative enum AdminTabType.
    /// </summary>
    public enum AdminTabType
    {
        /// <summary>
        /// Specifies the none value.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies the dashboard value.
        /// </summary>
        Dashboard = 1,
        /// <summary>
        /// Specifies the catalog value.
        /// </summary>
        Catalog = 2,
        /// <summary>
        /// Specifies the marketing value.
        /// </summary>
        Marketing = 8,
        /// <summary>
        /// Specifies the people value.
        /// </summary>
        People = 3,
        /// <summary>
        /// Specifies the content value.
        /// </summary>
        Content = 4, // excluded
        /// <summary>
        /// Specifies the orders value.
        /// </summary>
        Orders = 5,
        /// <summary>
        /// Specifies the reports value.
        /// </summary>
        Reports = 6,
        /// <summary>
        /// Specifies the admin value.
        /// </summary>
        Admin = 7,
        /// <summary>
        /// Specifies the configuration value.
        /// </summary>
        Configuration = 8,
        /// <summary>
        /// Specifies the plugins value.
        /// </summary>
        Plugins = 9, // excluded
        /// <summary>
        /// Specifies the host admin value.
        /// </summary>
        HostAdmin = 10,

        /// <summary>
        /// Specifies the setup wizard value.
        /// </summary>
        SetupWizard = 100
    }
}
