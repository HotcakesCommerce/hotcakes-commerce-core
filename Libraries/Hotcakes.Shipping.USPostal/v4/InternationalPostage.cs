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
using System.Xml;
using Hotcakes.Web;

namespace Hotcakes.Shipping.USPostal.v4
{
    [Serializable]
    public class InternationalPostage
    {
        public InternationalPostage()
        {
            ServiceDescription = string.Empty;
            ServiceId = string.Empty;
            Rate = 0m;
            ExtraServices = new List<InternationalExtraService>();
        }

        public InternationalPostage(XmlNode n)
        {
            ServiceDescription = string.Empty;
            ServiceId = string.Empty;
            Rate = 0m;
            ExtraServices = new List<InternationalExtraService>();

            ParseNode(n);
        }

        public string ServiceId { get; set; }
        public string ServiceDescription { get; set; }
        public decimal Rate { get; set; }
        public List<InternationalExtraService> ExtraServices { get; set; }

        public void ParseNode(XmlNode n)
        {
            if (n != null)
            {
                var classId = n.Attributes["ID"].Value;
                ServiceId = classId;

                ServiceDescription = Xml.ParseInnerText(n, "SvcDescription");
                Rate = Xml.ParseDecimal(n, "Postage");

                var ExtraServicesNode = n.SelectSingleNode("ExtraServices");

                ExtraServices.Clear();
                foreach (XmlNode n2 in ExtraServicesNode.SelectNodes("ExtraService"))
                {
                    var svc = new InternationalExtraService(n2);
                    ExtraServices.Add(svc);
                }
            }
        }
    }
}