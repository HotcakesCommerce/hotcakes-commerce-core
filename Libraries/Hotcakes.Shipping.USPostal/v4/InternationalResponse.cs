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

namespace Hotcakes.Shipping.USPostal.v4
{
    [Serializable]
    public class InternationalResponse
    {
        public InternationalResponse()
        {
            Packages = new List<InternationalPackage>();
            Errors = new List<USPSError>();
        }

        public InternationalResponse(string xmlData)
        {
            Packages = new List<InternationalPackage>();
            Errors = new List<USPSError>();
            Parse(xmlData);
        }

        public List<USPSError> Errors { get; set; }
        public List<InternationalPackage> Packages { get; set; }

        public void Parse(string xmlData)
        {
            try
            {
                if (xmlData.Trim().Length > 0)
                {
                    var xdoc = new XmlDocument();
                    xdoc.LoadXml(xmlData);

                    Errors.Clear();
                    XmlNodeList errorNodes;
                    errorNodes = xdoc.SelectNodes("/IntlRateV2Response/Error");
                    if (errorNodes != null)
                    {
                        foreach (XmlNode en in errorNodes)
                        {
                            var e = new USPSError(en);
                            Errors.Add(e);
                        }
                    }

                    Packages.Clear();
                    XmlNodeList packageNodes;
                    packageNodes = xdoc.SelectNodes("/IntlRateV2Response/Package");
                    if (packageNodes != null)
                    {
                        foreach (XmlNode pn in packageNodes)
                        {
                            var p = new InternationalPackage(pn);
                            Packages.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var e = new USPSError
                {
                    Source = ex.StackTrace,
                    Description = "Hotcakes Parsing Error: " + ex.Message
                };
                Errors.Add(e);
            }
        }
    }
}