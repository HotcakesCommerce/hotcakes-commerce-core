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

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class RateSpecialServices
    {
        private bool _InsideDelivery;
        private bool _NonstandardContainer;

        private bool _ResidentialDelivery;
        private bool _SaturdayDelivery;

        public bool ResidentialDelivery
        {
            get { return _ResidentialDelivery; }
            set { _ResidentialDelivery = value; }
        }

        public bool InsideDelivery
        {
            get { return _InsideDelivery; }
            set { _InsideDelivery = value; }
        }

        public bool SaturdayDelivery
        {
            get { return _SaturdayDelivery; }
            set { _SaturdayDelivery = value; }
        }

        public bool NonstandardContainer
        {
            get { return _NonstandardContainer; }
            set { _NonstandardContainer = value; }
        }

        public void WriteToXml(XmlTextWriter xw, string elementName)
        {
            xw.WriteStartElement(elementName);
            WriteToXml(xw);
            xw.WriteEndElement();
        }

        public void WriteToXml(XmlTextWriter xw)
        {
            if (_ResidentialDelivery)
            {
                xw.WriteElementString("ResidentialDelivery", "1");
            }
            if (_InsideDelivery)
            {
                xw.WriteElementString("InsideDelivery", "1");
            }
            if (_SaturdayDelivery)
            {
                xw.WriteElementString("SaturdayDelivery", "1");
            }
            if (_NonstandardContainer)
            {
                xw.WriteElementString("NonstandardContainer", "1");
            }
        }
    }
}