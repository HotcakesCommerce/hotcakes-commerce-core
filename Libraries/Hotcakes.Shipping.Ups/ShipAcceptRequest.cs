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
    public class ShipAcceptRequest
    {
        private UpsSettings _settings = new UpsSettings();
        private string _ShipDigest = string.Empty;
        private string _XmlAcceptConfirmRequest = string.Empty;
        private string _XmlAcceptConfirmResponse = string.Empty;

        public UpsSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public string XmlAcceptRequest
        {
            get { return _XmlAcceptConfirmRequest; }
            set { _XmlAcceptConfirmRequest = value; }
        }

        public string XmlAcceptResponse
        {
            get { return _XmlAcceptConfirmResponse; }
            set { _XmlAcceptConfirmResponse = value; }
        }

        public string ShipDigest
        {
            get { return _ShipDigest; }
            set { _ShipDigest = value; }
        }
    }
}