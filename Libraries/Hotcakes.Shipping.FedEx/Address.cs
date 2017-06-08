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

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class Address
    {
        private string _City = string.Empty;
        private string _CountryCode = "US";

        private string _Line1 = string.Empty;
        private string _Line2 = string.Empty;
        private string _PostalCode = string.Empty;
        private string _StateOrProvinceCode = string.Empty;

        public string Line1
        {
            get { return _Line1; }
            set { _Line1 = Text.TrimToLength(value, 35); }
        }

        public string Line2
        {
            get { return _Line2; }
            set { _Line2 = Text.TrimToLength(value, 35); }
        }

        public string City
        {
            get { return _City; }
            set { _City = Text.TrimToLength(value, 35); }
        }

        public string StateOrProvinceCode
        {
            get { return _StateOrProvinceCode; }
            set { _StateOrProvinceCode = Text.TrimToLength(value, 2); }
        }

        public string PostalCode
        {
            get { return _PostalCode; }
            set
            {
                var temp = value;
                temp = temp.Replace(" ", string.Empty);
                _PostalCode = Text.TrimToLength(temp, 16);
            }
        }

        public string CountryCode
        {
            get { return _CountryCode; }
            set { _CountryCode = Text.TrimToLength(value, 2); }
        }

        public void WriteToXml(XmlTextWriter xw, string elementName)
        {
            xw.WriteStartElement(elementName);
            WriteToXml(xw);
            xw.WriteEndElement();
        }

        public void WriteToXml(XmlTextWriter xw)
        {
            XmlHelper.WriteIfNotEmpty(xw, "Line1", _Line1);
            XmlHelper.WriteIfNotEmpty(xw, "Line2", _Line2);
            XmlHelper.WriteIfNotEmpty(xw, "City", _City);
            XmlHelper.WriteIfNotEmpty(xw, "StateOrProvinceCode", _StateOrProvinceCode);
            XmlHelper.WriteIfNotEmpty(xw, "PostalCode", _PostalCode);
            XmlHelper.WriteIfNotEmpty(xw, "CountryCode", _CountryCode);
        }
    }
}