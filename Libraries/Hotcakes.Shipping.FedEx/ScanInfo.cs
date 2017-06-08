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

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class ScanInfo
    {
        private string _City = string.Empty;
        private string _CountryCode = string.Empty;
        private DateTime _Date = DateTime.Now;

        private string _Description = string.Empty;
        private string _HoldAtLocation = string.Empty;
        private string _State = string.Empty;
        private string _Status = string.Empty;
        private string _StatusDescription = string.Empty;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string StatusDescription
        {
            get { return _StatusDescription; }
            set { _StatusDescription = value; }
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

        public string CountryCode
        {
            get { return _CountryCode; }
            set { _CountryCode = value; }
        }

        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        public string HoldAtLocation
        {
            get { return _HoldAtLocation; }
            set { _HoldAtLocation = value; }
        }
    }
}