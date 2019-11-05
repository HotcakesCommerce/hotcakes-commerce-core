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

using System.Collections.Generic;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Security
{
    public class FraudCheckData
    {
        private string _CreditCard = string.Empty;
        private string _DomainName = string.Empty;
        private string _EmailAddress = string.Empty;

        private string _IpAddress = string.Empty;
        private string _PhoneNumber = string.Empty;

        public FraudCheckData()
        {
            Messages = new List<string>();
        }

        public string IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value.Trim(); }
        }

        public string DomainName
        {
            get { return _DomainName; }
            set { _DomainName = value.Trim().ToLower(); }
        }

        public string EmailAddress
        {
            get { return _EmailAddress; }
            set { _EmailAddress = value.Trim().ToLower(); }
        }

        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = CreditCardValidator.CleanCardNumber(value.Trim().ToLower()); }
        }

        public string CreditCard
        {
            get { return _CreditCard; }
            set { _CreditCard = CreditCardValidator.CleanCardNumber(value.Trim().ToLower()); }
        }

        public List<string> Messages { get; set; }
    }
}