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

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public class ShipResponse
    {
        private decimal _BillingWeight;
        private UnitsType _BillingWeightUnits = UnitsType.Imperial;
        private string _ErrorCode = string.Empty;
        private string _ErrorMessage = string.Empty;
        private decimal _ServiceOptionsCharge;
        private CurrencyCode _ServiceOptionsChargeCurrency = CurrencyCode.UsDollar;
        private string _ShipmentDigest = string.Empty;
        private bool _Success;
        private decimal _TotalCharge;
        private CurrencyCode _TotalChargeCurrency = CurrencyCode.UsDollar;
        private string _TrackingNumber = string.Empty;
        private decimal _TransportationCharge;
        private CurrencyCode _TransportationChargeCurrency = CurrencyCode.UsDollar;

        public string ErrorCode
        {
            get { return _ErrorCode; }
            set { _ErrorCode = value; }
        }

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        public bool Success
        {
            get { return _Success; }
            set { _Success = value; }
        }

        public decimal TransportationCharge
        {
            get { return _TransportationCharge; }
            set { _TransportationCharge = value; }
        }

        public CurrencyCode TransportationChargeCurrency
        {
            get { return _TransportationChargeCurrency; }
            set { _TransportationChargeCurrency = value; }
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

        public decimal TotalCharge
        {
            get { return _TotalCharge; }
            set { _TotalCharge = value; }
        }

        public CurrencyCode TotalChargeCurrency
        {
            get { return _TotalChargeCurrency; }
            set { _TotalChargeCurrency = value; }
        }

        public decimal BillingWeight
        {
            get { return _BillingWeight; }
            set { _BillingWeight = value; }
        }

        public UnitsType BillingWeightUnits
        {
            get { return _BillingWeightUnits; }
            set { _BillingWeightUnits = value; }
        }

        public string TrackingNumber
        {
            get { return _TrackingNumber; }
            set { _TrackingNumber = value; }
        }

        public string ShipmentDigest
        {
            get { return _ShipmentDigest; }
            set { _ShipmentDigest = value; }
        }
    }
}