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
    public class Package
    {
        private bool _AdditionalHandlingIsRequired;
        private bool _COD;
        private decimal _CODAmount;
        private CurrencyCode _CODCurrencyCode = CurrencyCode.UsDollar;
        private CODFundsCode _CODPaymentType = CODFundsCode.AllTypesAccepted;
        private bool _DeliveryConfirmation;
        private string _DeliveryConfirmationControlNumber = string.Empty;
        private ConfirmationType _DeliveryConfirmationType = ConfirmationType.NoSignatureRequired;
        private string _Description = string.Empty;
        private UnitsType _DimensionalUnits = UnitsType.Imperial;
        private decimal _Height;
        private decimal _InsuredValue;
        private CurrencyCode _InsuredValueCurrency = CurrencyCode.UsDollar;
        private decimal _Length;

        private PackagingType _Packaging = PackagingType.CustomerSupplied;
        private string _ReferenceNumber = string.Empty;
        private string _ReferenceNumber2 = string.Empty;
        private ReferenceNumberCode _ReferenceNumber2Type = ReferenceNumberCode.TransactionReferenceNumber;
        private ReferenceNumberCode _ReferenceNumberType = ReferenceNumberCode.TransactionReferenceNumber;
        private bool _VerbalConfirmation;
        private string _VerbalConfirmationName = string.Empty;
        private string _VerbalConfirmationPhoneNumber = string.Empty;
        private decimal _Weight;
        private UnitsType _WeightUnits = UnitsType.Imperial;
        private decimal _Width;

        public PackagingType Packaging
        {
            get { return _Packaging; }
            set { _Packaging = value; }
        }

        public decimal Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        public decimal Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        public decimal Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public decimal Weight
        {
            get { return _Weight; }
            set { _Weight = value; }
        }

        public UnitsType DimensionalUnits
        {
            get { return _DimensionalUnits; }
            set { _DimensionalUnits = value; }
        }

        public UnitsType WeightUnits
        {
            get { return _WeightUnits; }
            set { _WeightUnits = value; }
        }

        public string ReferenceNumber
        {
            get { return _ReferenceNumber; }
            set { _ReferenceNumber = value; }
        }

        public ReferenceNumberCode ReferenceNumberType
        {
            get { return _ReferenceNumberType; }
            set { _ReferenceNumberType = value; }
        }

        public string ReferenceNumber2
        {
            get { return _ReferenceNumber2; }
            set { _ReferenceNumber2 = value; }
        }

        public ReferenceNumberCode ReferenceNumber2Type
        {
            get { return _ReferenceNumber2Type; }
            set { _ReferenceNumber2Type = value; }
        }

        public bool AdditionalHandlingIsRequired
        {
            get { return _AdditionalHandlingIsRequired; }
            set { _AdditionalHandlingIsRequired = value; }
        }

        public bool DeliveryConfirmation
        {
            get { return _DeliveryConfirmation; }
            set { _DeliveryConfirmation = value; }
        }

        public ConfirmationType DeliveryConfirmationType
        {
            get { return _DeliveryConfirmationType; }
            set { _DeliveryConfirmationType = value; }
        }

        public string DeliveryConfirmationControlNumber
        {
            get { return _DeliveryConfirmationControlNumber; }
            set { _DeliveryConfirmationControlNumber = value; }
        }

        public decimal InsuredValue
        {
            get { return _InsuredValue; }
            set { _InsuredValue = value; }
        }

        public CurrencyCode InsuredValueCurrency
        {
            get { return _InsuredValueCurrency; }
            set { _InsuredValueCurrency = value; }
        }

        public bool COD
        {
            get { return _COD; }
            set { _COD = value; }
        }

        public CODFundsCode CODPaymentType
        {
            get { return _CODPaymentType; }
            set { _CODPaymentType = value; }
        }

        public CurrencyCode CODCurrencyCode
        {
            get { return _CODCurrencyCode; }
            set { _CODCurrencyCode = value; }
        }

        public decimal CODAmount
        {
            get { return _CODAmount; }
            set { _CODAmount = value; }
        }

        public bool VerbalConfirmation
        {
            get { return _VerbalConfirmation; }
            set { _VerbalConfirmation = value; }
        }

        public string VerbalConfirmationName
        {
            get { return _VerbalConfirmationName; }
            set { _VerbalConfirmationName = value; }
        }

        public string VerbalConfirmationPhoneNumber
        {
            get { return _VerbalConfirmationPhoneNumber; }
            set { _VerbalConfirmationPhoneNumber = XmlTools.CleanPhoneNumber(value); }
        }
    }
}