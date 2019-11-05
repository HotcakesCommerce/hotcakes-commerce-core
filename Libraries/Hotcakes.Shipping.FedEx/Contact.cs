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
using System.Xml;
using Hotcakes.Web;

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class Contact
    {
        private string _CompanyName = string.Empty;
        private string _EmailAddress = string.Empty;
        private string _FaxNumber = string.Empty;
        private string _PagerNumber = string.Empty;

        private string _PersonName = string.Empty;
        private string _PhoneNumber = string.Empty;

        public string PersonName
        {
            get { return _PersonName; }
            set { _PersonName = Text.TrimToLength(value, 35); }
        }

        public string CompanyName
        {
            get { return _CompanyName; }
            set { _CompanyName = Text.TrimToLength(value, 35); }
        }

        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = Text.TrimToLength(value, 16); }
        }

        public string PagerNumber
        {
            get { return _PagerNumber; }
            set { _PagerNumber = Text.TrimToLength(value, 16); }
        }

        public string FaxNumber
        {
            get { return _FaxNumber; }
            set { _FaxNumber = Text.TrimToLength(value, 16); }
        }

        public string EmailAddress
        {
            get { return _EmailAddress; }
            set { _EmailAddress = Text.TrimToLength(value, 120); }
        }

        public void WriteToXml(XmlTextWriter xw, string elementName)
        {
            xw.WriteStartElement(elementName);
            WriteToXml(xw);
            xw.WriteEndElement();
        }

        public void WriteToXml(XmlTextWriter xw)
        {
            if (_PersonName.Trim().Length > 0)
            {
                xw.WriteElementString("PersonName", _PersonName);
            }
            if (_CompanyName.Trim().Length > 0)
            {
                xw.WriteElementString("CompanyName", _CompanyName);
            }

            xw.WriteElementString("PhoneNumber", _PhoneNumber);

            if (_PagerNumber.Trim().Length > 0)
            {
                xw.WriteElementString("PagerNumber", _PagerNumber);
            }
            if (_FaxNumber.Trim().Length > 0)
            {
                xw.WriteElementString("FaxNumber", _FaxNumber);
            }
            if (_EmailAddress.Trim().Length > 0)
            {
                xw.WriteElementString("EmailAddress", _EmailAddress);
            }
        }
    }
}