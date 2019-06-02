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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class RateCharge
    {
        private decimal _BaseCharge;
        private decimal _NetCharge;
        private decimal _ShipmentNetCharge;
        private Collection<Surcharge> _Surcharges = new Collection<Surcharge>();
        private decimal _TotalDiscount;
        private decimal _TotalRebate;
        private decimal _TotalSurcharge;

        public RateCharge()
        {
        }

        public RateCharge(XmlNode n)
        {
            ParseNode(n);
        }

        public decimal BaseCharge
        {
            get { return _BaseCharge; }
            set { _BaseCharge = value; }
        }

        public Collection<Surcharge> Surcharges
        {
            get { return _Surcharges; }
            set { _Surcharges = value; }
        }

        public decimal TotalSurcharge
        {
            get { return _TotalSurcharge; }
            set { _TotalSurcharge = value; }
        }

        public decimal NetCharge
        {
            get { return _NetCharge; }
            set { _NetCharge = value; }
        }

        public decimal ShipmentNetCharge
        {
            get { return _ShipmentNetCharge; }
            set { _ShipmentNetCharge = value; }
        }

        public decimal TotalRebate
        {
            get { return _TotalRebate; }
            set { _TotalRebate = value; }
        }

        public decimal TotalDiscount
        {
            get { return _TotalDiscount; }
            set { _TotalDiscount = value; }
        }

        public void ParseNode(XmlNode n)
        {
            _BaseCharge = XmlHelper.ParseDecimal(n, "BaseCharge");
            if (n != null)
            {
                var sn = n.SelectSingleNode("Surcharges");
                if (sn != null)
                {
                    foreach (XmlNode snn in sn.ChildNodes)
                    {
                        var description = snn.Name;
                        var tempAmount = snn.InnerText;
                        var amount = 0m;
                        decimal.TryParse(tempAmount, NumberStyles.Float,
                            CultureInfo.InvariantCulture, out amount);
                        Surcharges.Add(new Surcharge(description, amount));
                    }
                }
            }
            _TotalSurcharge = XmlHelper.ParseDecimal(n, "TotalSurcharge");
            _NetCharge = XmlHelper.ParseDecimal(n, "NetCharge");
            _ShipmentNetCharge = XmlHelper.ParseDecimal(n, "ShipmentNetCharge");
            _TotalRebate = XmlHelper.ParseDecimal(n, "TotalRebate");
            _TotalDiscount = XmlHelper.ParseDecimal(n, "TotalDiscount");
        }
    }
}