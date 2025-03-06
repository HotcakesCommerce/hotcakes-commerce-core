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

using System.Collections.Generic;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Common.Dnn;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    public class PerformanceUserSelections
    {
        private readonly Dictionary<string, string> _settings;

        public PerformanceUserSelections()
        {
            var context = HccRequestContext.Current;
            var userId = context.CurrentAccount.Bvin;
            var membershipServices = Factory.CreateService<MembershipServices>();
            _settings = membershipServices.GetUserSettings(userId, false);
        }

        public SalesPeriod ProductsPerformacePeriod
        {
            get { return (SalesPeriod) GetIntValue("HccProductsPerformacePeriod", (int) SalesPeriod.Month); }
            set
            {
                _settings["HccProductsPerformacePeriod"] = ((int) value).ToString();
                Update();
            }
        }

        public SalesPeriod CategoriesPerformacePeriod
        {
            get { return (SalesPeriod) GetIntValue("HccCategoriesPerformacePeriod", (int) SalesPeriod.Month); }
            set
            {
                _settings["HccCategoriesPerformacePeriod"] = ((int) value).ToString();
                Update();
            }
        }

        private int GetIntValue(string settingName, int defValue)
        {
            string val;
            var result = _settings.TryGetValue(settingName, out val);
            if (!string.IsNullOrEmpty(val))
            {
                return val.ConvertTo<int>();
            }
            _settings[settingName] = defValue.ToString();
            Update();
            return defValue;
        }

        private void Update()
        {
            var context = HccRequestContext.Current;
            var userId = context.CurrentAccount.Bvin;
            var membershipServices = Factory.CreateService<MembershipServices>();
            membershipServices.UpdateUserSettings(userId, _settings, false);
        }
    }
}