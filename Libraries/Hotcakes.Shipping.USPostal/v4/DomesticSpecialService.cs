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
using System.Xml;
using Hotcakes.Web;

namespace Hotcakes.Shipping.USPostal.v4
{
    [Serializable]
    public class DomesticSpecialService
    {
        private string _ServiceId = string.Empty;

        public DomesticSpecialService()
        {
            ServiceName = string.Empty;
            ServiceType = DomesticSpecialServiceType.NotSet;
            Price = 0m;
            Available = false;
        }

        public DomesticSpecialService(XmlNode node)
        {
            ServiceName = string.Empty;
            ServiceType = DomesticSpecialServiceType.NotSet;
            Price = 0m;
            Available = false;

            ParseNode(node);
        }

        public DomesticSpecialServiceType ServiceType { get; set; }
        public decimal Price { get; set; }
        public string ServiceName { get; set; }
        public bool Available { get; set; }

        public void ParseNode(XmlNode n)
        {
            if (n != null)
            {
                _ServiceId = Xml.ParseInnerText(n, "ServiceID");
                ServiceName = Xml.ParseInnerText(n, "ServiceName");
                Price = Xml.ParseDecimal(n, "Price");
                Available = Xml.ParseBoolean(n, "Available");

                try
                {
                    var temp = -1;
                    if (int.TryParse(_ServiceId, out temp))
                    {
                        ServiceType = (DomesticSpecialServiceType) temp;
                    }
                }
                catch
                {
                    ServiceType = DomesticSpecialServiceType.NotSet;
                }
            }
        }
    }
}