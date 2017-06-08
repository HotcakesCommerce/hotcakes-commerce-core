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

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public class ShippedPackageInformation
    {
        private string _Base64Html = string.Empty;
        private string _Base64Image = string.Empty;
        private string _Base64Signature = string.Empty;
        private ShipLabelFormat _LabelFormat = ShipLabelFormat.Gif;
        private decimal _ServiceOptionsCharge;
        private CurrencyCode _ServiceOptionsChargeCurrency = CurrencyCode.UsDollar;

        private string _TrackingNumber = string.Empty;

        public string TrackingNumber
        {
            get { return _TrackingNumber; }
            set { _TrackingNumber = value; }
        }

        public decimal ServiceOptionsCharge
        {
            get { return _ServiceOptionsCharge; }
            set { _ServiceOptionsCharge = value; }
        }

        public CurrencyCode ServiceOptionsChargeCurrency
        {
            get { return _ServiceOptionsChargeCurrency; }
            set { _ServiceOptionsChargeCurrency = value; }
        }

        public ShipLabelFormat LabelFormat
        {
            get { return _LabelFormat; }
            set { _LabelFormat = value; }
        }

        public string Base64Image
        {
            get { return _Base64Image; }
            set { _Base64Image = value; }
        }

        public string Base64Html
        {
            get { return _Base64Html; }
            set { _Base64Html = value; }
        }

        public string Base64Signature
        {
            get { return _Base64Signature; }
            set { _Base64Signature = value; }
        }
    }
}