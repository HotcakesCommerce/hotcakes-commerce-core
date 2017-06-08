#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Reporting;
using Hotcakes.Common.Dnn;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    public class DashboardUserSelections
    {
        private readonly HotcakesApplication _app;
        private readonly Dictionary<string, string> _settings;

        public DashboardUserSelections(HotcakesApplication app)
        {
            var userId = app.CurrentCustomerId;
            _settings = app.MembershipServices.GetUserSettings(userId, false);
            _app = app;
        }

        public SalesPeriod Period1
        {
            get { return (SalesPeriod) GetIntValue("HccDashboardPeriod1", (int) SalesPeriod.Month); }
            set
            {
                _settings["HccDashboardPeriod1"] = ((int) value).ToString();
                Update();
            }
        }

        public SalesPeriod Period2
        {
            get { return (SalesPeriod) GetIntValue("HccDashboardPeriod2", (int) SalesPeriod.Month); }
            set
            {
                _settings["HccDashboardPeriod2"] = ((int) value).ToString();
                Update();
            }
        }

        public Top5ProductMode ProductMode
        {
            get { return (Top5ProductMode) GetIntValue("HccDashboardProductMode", (int) Top5ProductMode.Amount); }
            set
            {
                _settings["HccDashboardProductMode"] = ((int) value).ToString();
                Update();
            }
        }

        public Top5CustomerMode CustomerMode
        {
            get { return (Top5CustomerMode) GetIntValue("HccDashboardCustomerMode", (int) Top5CustomerMode.Amount); }
            set
            {
                _settings["HccDashboardCustomerMode"] = ((int) value).ToString();
                Update();
            }
        }

        public Top5VendorType VendorType
        {
            get { return (Top5VendorType) GetIntValue("HccDashboardVendorType", (int) Top5VendorType.Vendors); }
            set
            {
                _settings["HccDashboardVendorType"] = ((int) value).ToString();
                Update();
            }
        }

        public Top5AffiliateMode AffiliateMode
        {
            get
            {
                return (Top5AffiliateMode) GetIntValue("HccDashboardAffiliateMode", (int) Top5AffiliateMode.Referral);
            }
            set
            {
                _settings["HccDashboardAffiliateMode"] = ((int) value).ToString();
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
            Update();
            return defValue;
        }

        private void Update()
        {
            var userId = _app.CurrentCustomerId;
            _app.MembershipServices.UpdateUserSettings(userId, _settings, false);
        }
    }
}