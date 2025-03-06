#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.IO;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Hotcakes.Web.Geography;

namespace Hotcakes.Shipping
{
    public class USPSAddressProvider : IAddressProvider
    {
        private const string _url = "https://production.shippingapis.com/ShippingAPITest.dll?API=Verify&XML=";
        private readonly string _userID;

        public USPSAddressProvider(string userID)
        {
            _userID = userID;
        }

        public bool ValidateAddress(IAddress address, out string message)
        {
            var xmlRes = RequestValidation(address);
            var xmlErr = xmlRes.Element("Error");

            if (xmlErr == null)
            {
                message = GetElementValue(xmlRes, "ReturnText");
                return true;
            }
            message = GetElementValue(xmlErr, "Description");
            return false;
        }

        public bool NormalizeAddress(IAddress address, out string message)
        {
            var xmlRes = RequestValidation(address);
            var xmlErr = xmlRes.Element("Error");
            var normalized = false;

            if (xmlErr == null)
            {
                message = GetElementValue(xmlRes, "ReturnText");
                address.Street = GetElementValue(xmlRes, "Address1");
                address.Street2 = GetElementValue(xmlRes, "Address2");
                address.City = GetElementValue(xmlRes, "City");
                address.RegionBvin = GetElementValue(xmlRes, "State");
                address.PostalCode = GetElementValue(xmlRes, "Zip5");

                // do not allow first empty line 
                if (string.IsNullOrEmpty(address.Street) && !string.IsNullOrEmpty(address.Street2))
                {
                    address.Street = address.Street2;
                    address.Street2 = string.Empty;
                }

                normalized = true;
            }
            else
            {
                message = GetElementValue(xmlErr, "Description");
            }

            return normalized;
        }

        private XElement RequestValidation(IAddress address)
        {
            var xdoc = new XElement("AddressValidateRequest",
                new XAttribute("USERID", _userID),
                new XElement("Address",
                    new XElement("Address1", address.Street),
                    new XElement("Address2", address.Street2),
                    new XElement("City", address.City),
                    new XElement("State", address.RegionBvin),
                    new XElement("Zip5", address.PostalCode),
                    new XElement("Zip4", "")
                    ));

            var xml = HttpUtility.UrlEncode(xdoc.ToString());
            var request = WebRequest.Create(_url + xml);
            var response = request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var xmlRes = XElement.Parse(reader.ReadToEnd());
                var addr = xmlRes.Element("Address");

                if (addr != null)
                {
                    return addr;
                }
                if (xmlRes.Name == "Error")
                {
                    var exMessage = GetElementValue(xmlRes, "Description");
                    throw new USPSException(exMessage);
                }

                var err = xmlRes.Element("Error");
                if (err != null)
                {
                    return new XElement("Address", err);
                }
                return new XElement("Address");
            }
        }

        private string GetElementValue(XElement el, string subElName)
        {
            var subEl = el.Element(subElName);

            if (subEl == null) return string.Empty;
            return subEl.Value;
        }

        public class USPSException : ApplicationException
        {
            public USPSException(string message)
                : base(message)
            {
            }
        }
    }
}