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

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class RateEstimatedCharges
    {
        private decimal _BilledWeight;
        private string _CurrencyCode = string.Empty;
        private decimal _DimensionalWeight;

        private bool _DimensionalWeightUsed;

        private RateCharge _DiscountedCharges = new RateCharge();

        private decimal _EffectiveNetDiscount;
        private RateCharge _ListCharges = new RateCharge();
        private decimal _MTWNetCharge;
        private string _RateScale = string.Empty;
        private string _RateZone = string.Empty;

        public RateEstimatedCharges()
        {
        }

        public RateEstimatedCharges(XmlNode n)
        {
            ParseNode(n);
        }

        public bool DimensionalWeightUsed
        {
            get { return _DimensionalWeightUsed; }
            set { _DimensionalWeightUsed = value; }
        }

        public string RateScale
        {
            get { return _RateScale; }
            set { _RateScale = value; }
        }

        public string RateZone
        {
            get { return _RateZone; }
            set { _RateZone = value; }
        }

        public string CurrencyCode
        {
            get { return _CurrencyCode; }
            set { _CurrencyCode = value; }
        }

        public decimal BilledWeight
        {
            get { return _BilledWeight; }
            set { _BilledWeight = value; }
        }

        public decimal DimensionalWeight
        {
            get { return _DimensionalWeight; }
            set { _DimensionalWeight = value; }
        }

        public RateCharge DiscountedCharges
        {
            get { return _DiscountedCharges; }
            set { _DiscountedCharges = value; }
        }

        public RateCharge ListCharges
        {
            get { return _ListCharges; }
            set { _ListCharges = value; }
        }

        public decimal EffectiveNetDiscount
        {
            get { return _EffectiveNetDiscount; }
            set { _EffectiveNetDiscount = value; }
        }

        public decimal MTWNetCharge
        {
            get { return _MTWNetCharge; }
            set { _MTWNetCharge = value; }
        }

        public void ParseNode(XmlNode n)
        {
            _DimensionalWeightUsed = XmlHelper.ParseBoolean(n, "DimWeightUsed");
            _RateScale = XmlHelper.ParseInnerText(n, "RateScale");
            _RateZone = XmlHelper.ParseInnerText(n, "RateZone");
            _CurrencyCode = XmlHelper.ParseInnerText(n, "CurrencyCode");
            _BilledWeight = XmlHelper.ParseDecimal(n, "BilledWeight");
            _DimensionalWeight = XmlHelper.ParseDecimal(n, "DimWeight");

            if (n != null)
            {
                _DiscountedCharges.ParseNode(n.SelectSingleNode("DiscountedCharges"));
                _ListCharges.ParseNode(n.SelectSingleNode("ListCharges"));
            }

            _EffectiveNetDiscount = XmlHelper.ParseDecimal(n, "EffectiveNetDiscount");
            _MTWNetCharge = XmlHelper.ParseDecimal(n, "MTWNetCharge");
        }
    }
}