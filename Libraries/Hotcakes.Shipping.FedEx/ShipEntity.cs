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

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class ShipEntity
    {
        private string _City = string.Empty;
        private string _Company = string.Empty;
        private string _Country = "US";
        private string _Department = string.Empty;
        private string _DialNumber = string.Empty;
        private string _Email = string.Empty;
        private string _Extenstion = string.Empty;
        private string _Fax = string.Empty;
        private string _Line1 = string.Empty;
        private string _Line2 = string.Empty;
        private string _Line3 = string.Empty;
        private string _LineNumber = string.Empty;
        private string _Pager = string.Empty;

        private string _Person = string.Empty;
        private string _Phone = string.Empty;
        private string _PhoneCountryCode = string.Empty;
        private string _PostalCode = string.Empty;
        private string _State = string.Empty;
        private string _TaxID = string.Empty;

        public string Person
        {
            get { return _Person; }
            set { _Person = value; }
        }

        public string Company
        {
            get { return _Company; }
            set { _Company = value; }
        }

        public string Department
        {
            get { return _Department; }
            set { _Department = value; }
        }

        public string TaxID
        {
            get { return _TaxID; }
            set { _TaxID = value; }
        }

        public string Phone
        {
            get { return _Phone; }
            set { _Phone = value; }
        }

        public string Pager
        {
            get { return _Pager; }
            set { _Pager = value; }
        }

        public string Fax
        {
            get { return _Fax; }
            set { _Fax = value; }
        }

        public string PhoneCountryCode
        {
            get { return _PhoneCountryCode; }
            set { _PhoneCountryCode = value; }
        }

        public string DialNumber
        {
            get { return _DialNumber; }
            set { _DialNumber = value; }
        }

        public string LineNumber
        {
            get { return _LineNumber; }
            set { _LineNumber = value; }
        }

        public string Extenstion
        {
            get { return _Extenstion; }
            set { _Extenstion = value; }
        }

        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        public string Line1
        {
            get { return _Line1; }
            set { _Line1 = value; }
        }

        public string Line2
        {
            get { return _Line2; }
            set { _Line2 = value; }
        }

        public string Line3
        {
            get { return _Line3; }
            set { _Line3 = value; }
        }

        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        public string State
        {
            get { return _State; }
            set { _State = value; }
        }

        public string PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; }
        }

        public string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }
    }
}