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

using System;
using System.Collections.Generic;

namespace Hotcakes.Shipping.UpsFreight
{
    [Serializable]
    public class UPSFreightServiceSettings : ServiceSettings
    {
        public bool NegotiatedRates
        {
            get { return GetBoolSetting("UpsNegotiatedRates"); }
            set { SetBoolSetting("UpsNegotiatedRates", value); }
        }

        public bool GetAllRates
        {
            get { return GetBoolSetting("GetAllRates"); }
            set { SetBoolSetting("GetAllRates", value); }
        }

        public string PayerName
        {
            get { return GetSettingOrEmpty("PayerName"); }
            set { SetSetting("PayerName", value); }
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
    }
}