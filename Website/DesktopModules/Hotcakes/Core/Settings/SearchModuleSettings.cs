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
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.Settings
{
    /// <summary>
    /// Provides persisted settings for a search module instance.
    /// </summary>
    [Serializable]
    public class SearchModuleSettings
    {
        private readonly IModuleSettingsProvider _provider;
        /// <summary>
        /// Initializes settings for the specified module instance.
        /// </summary>
        /// <param name="moduleId">The DNN module instance identifier.</param>

        public SearchModuleSettings(int moduleId)
        {
            _provider = Factory.Instance.CreateModuleSettingsProvider(moduleId);
        }

        /// <summary>
        /// Gets or sets the selected search view.
        /// </summary>
        public string View
        {
            get { return _provider.GetSettingValue("View", string.Empty); }
            set { _provider.SetSettingValue("View", value); }
        }

        /// <summary>
        /// Gets or sets the number of search results displayed on each page.
        /// </summary>
        public int PageSize
        {
            get { return _provider.GetSettingValue("PageSize", 9); }
            set { _provider.SetSettingValue("PageSize", value); }
        }

        /// <summary>
        /// Gets or sets whether manufacturers are displayed; use <see cref="ShowManufacturers"/> instead.
        /// </summary>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public bool ShowManufactures
        {
            get { return ShowManufacturers; }
            set { ShowManufacturers = value; }
        }

        /// <summary>
        /// Gets or sets whether manufacturers are displayed.
        /// </summary>
        public bool ShowManufacturers
        {
            get { return _provider.GetSettingValue("ShowManufactures", true); }
            set { _provider.SetSettingValue("ShowManufactures", value); }
        }

        /// <summary>
        /// Gets or sets whether vendors are displayed.
        /// </summary>
        public bool ShowVendors
        {
            get { return _provider.GetSettingValue("ShowVendors", true); }
            set { _provider.SetSettingValue("ShowVendors", value); }
        }
    }
}
