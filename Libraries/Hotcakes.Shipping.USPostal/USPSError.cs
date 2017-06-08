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

namespace Hotcakes.Shipping.USPostal
{
    [Serializable]
    public class USPSError
    {
        private string _Description = string.Empty;
        private string _HelpContext = string.Empty;
        private string _HelpFile = string.Empty;

        private string _Number = string.Empty;
        private string _Source = string.Empty;

        public USPSError()
        {
        }

        public USPSError(XmlNode n)
        {
            ParseNode(n);
        }

        public string Number
        {
            get { return _Number; }
            set { _Number = value; }
        }

        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string HelpFile
        {
            get { return _HelpFile; }
            set { _HelpFile = value; }
        }

        public string HelpContext
        {
            get { return _HelpContext; }
            set { _HelpContext = value; }
        }

        public void ParseNode(XmlNode n)
        {
            if (n == null) return;

            _Number = Xml.ParseInnerText(n, "Number");
            _Source = Xml.ParseInnerText(n, "Source");
            _Description = Xml.ParseInnerText(n, "Description");
            _HelpFile = Xml.ParseInnerText(n, "HelpFile");
            _HelpContext = Xml.ParseInnerText(n, "HelpContext");
        }
    }
}