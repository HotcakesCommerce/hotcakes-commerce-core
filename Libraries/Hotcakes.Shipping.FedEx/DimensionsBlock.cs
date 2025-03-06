#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    public class DimensionsBlock
    {
        private decimal _Height;

        private decimal _Length;
        private DimensionType _Units = DimensionType.IN;
        private decimal _Width;

        public decimal Length
        {
            get { return _Length; }
            set { _Length = Math.Round(value, 1); }
        }

        public decimal Width
        {
            get { return _Width; }
            set { _Width = Math.Round(value, 1); }
        }

        public decimal Height
        {
            get { return _Height; }
            set { _Height = Math.Round(value, 1); }
        }

        public DimensionType Units
        {
            get { return _Units; }
            set { _Units = value; }
        }

        public void WriteToXml(XmlTextWriter xw, string elementName)
        {
            if ((_Length > 0m) | (_Width > 0m) | (_Height > 0m))
            {
                xw.WriteStartElement(elementName);
                WriteToXml(xw);
                xw.WriteEndElement();
            }
        }

        public void WriteToXml(XmlTextWriter xw)
        {
            XmlHelper.WriteIfNotEmpty(xw, "Length", Math.Round(_Length, 0).ToString());
            XmlHelper.WriteIfNotEmpty(xw, "Width", Math.Round(_Width, 0).ToString());
            XmlHelper.WriteIfNotEmpty(xw, "Height", Math.Round(_Height, 0).ToString());
            XmlHelper.WriteIfNotEmpty(xw, "Units", _Units.ToString());
        }
    }
}