#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    [Serializable]
    public class SearchModuleSettings
    {
        private readonly IModuleSettingsProvider _provider;

        public SearchModuleSettings(int moduleId)
        {
            _provider = Factory.Instance.CreateModuleSettingsProvider(moduleId);
        }

        public string View
        {
            get { return _provider.GetSettingValue("View", string.Empty); }
            set { _provider.SetSettingValue("View", value); }
        }

        public int PageSize
        {
            get { return _provider.GetSettingValue("PageSize", 9); }
            set { _provider.SetSettingValue("PageSize", value); }
        }

        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public bool ShowManufactures
        {
            get { return ShowManufacturers; }
            set { ShowManufacturers = value; }
        }

        public bool ShowManufacturers
        {
            get { return _provider.GetSettingValue("ShowManufactures", true); }
            set { _provider.SetSettingValue("ShowManufactures", value); }
        }

        public bool ShowVendors
        {
            get { return _provider.GetSettingValue("ShowVendors", true); }
            set { _provider.SetSettingValue("ShowVendors", value); }
        }
    }
}