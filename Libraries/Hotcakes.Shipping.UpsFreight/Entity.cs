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

namespace Hotcakes.Shipping.UpsFreight
{
    [Serializable]
    public class Entity
    {
        private string _AccountNumber = string.Empty;
        private string _AddressLine1 = string.Empty;
        private string _AddressLine2 = string.Empty;
        private string _AddressLine3 = string.Empty;
        private string _AttentionName = string.Empty;
        private string _City = string.Empty;

        private string _CompanyName = string.Empty;
        private string _CountryCode = string.Empty;
        private string _FaxNumber = string.Empty;
        private string _PhoneNumber = string.Empty;
        private string _PostalCode = string.Empty;
        private bool _ResidentialAddress;
        private string _StateProvinceCode = string.Empty;
        private string _TaxIDNumber = string.Empty;

        public string CompanyOrContactName
        {
            get { return _CompanyName; }
            set { _CompanyName = value; }
        }

        public string AttentionName
        {
            get { return _AttentionName; }
            set { _AttentionName = value; }
        }

        public string AccountNumber
        {
            get { return _AccountNumber; }
            set { _AccountNumber = value; }
        }

        public string TaxIDNumber
        {
            get { return _TaxIDNumber; }
            set { _TaxIDNumber = value; }
        }

        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = value; }
        }

        public string FaxNumber
        {
            get { return _FaxNumber; }
            set { _FaxNumber = value; }
        }

        public string AddressLine1
        {
            get { return _AddressLine1; }
            set { _AddressLine1 = value; }
        }

        public string AddressLine2
        {
            get { return _AddressLine2; }
            set { _AddressLine2 = value; }
        }

        public string AddressLine3
        {
            get { return _AddressLine3; }
            set { _AddressLine3 = value; }
        }

        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        public string StateProvinceCode
        {
            get { return _StateProvinceCode; }
            set { _StateProvinceCode = value; }
        }

        public string PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; }
        }

        public string CountryCode
        {
            get { return _CountryCode; }
            set { _CountryCode = value; }
        }

        public bool ResidentialAddress
        {
            get { return _ResidentialAddress; }
            set { _ResidentialAddress = value; }
        }
    }
}