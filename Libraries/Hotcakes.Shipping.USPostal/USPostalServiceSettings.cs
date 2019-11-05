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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Shipping.USPostal.v4;

namespace Hotcakes.Shipping.USPostal
{
    [Serializable]
    public class USPostalServiceSettings : ServiceSettings
    {
        public USPostalServiceSettings()
        {
            PackageType = DomesticPackageType.Ignore;
        }

        public DomesticPackageType PackageType
        {
            get { return (DomesticPackageType) GetIntegerSetting("PackageType"); }
            set { SetIntegerSetting("PackageType", (int) value); }
        }

        public List<IServiceCode> ServiceCodeFilter
        {
            get
            {
                var result = new List<IServiceCode>();
                var serialized = GetSettingOrEmpty("ServiceCodeFilter");
                var codes = serialized.Split(',');
                foreach (var code in codes)
                {
                    var parts = code.Split('|');

                    if (parts.Length > 0)
                    {
                        var c = new ServiceCode {Code = parts[0]};
                        if (parts.Length > 1)
                        {
                            c.DisplayName = parts[1];
                        }
                        result.Add(c);
                    }
                }

                if (result.Count < 1) result.Add(new ServiceCode {Code = "-1", DisplayName = "All Available Services"});
                return result;
            }
            set
            {
                var s = string.Empty;
                foreach (var code in value)
                {
                    s += code.Code + "|" + code.DisplayName + ",";
                }
                s = s.TrimEnd(',');
                AddOrUpdate("ServiceCodeFilter", s);
            }
        }

        public bool ReturnAllServices()
        {
            var x = (from svc in ServiceCodeFilter
                where svc.Code == "-1"
                select svc).Count();

            if (x > 0) return true;

            return false;
        }

        public DomesticServiceType GetServiceForProcessing()
        {
            // if a single service is selected, return that strategy code
            if (ServiceCodeFilter.Count == 1)
            {
                var _code = -1;
                if (int.TryParse(ServiceCodeFilter[0].Code, out _code))
                {
                    return (DomesticServiceType) _code;
                }
            }

            // otherwise return multi-code
            return DomesticServiceType.All;
        }
    }
}