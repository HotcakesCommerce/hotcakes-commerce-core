#region License

// Distributed under the MIT License
// ============================================================
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

namespace Hotcakes.Shipping.Ups.Models
{
    public class ShipmentRequest
    {
        public Request Request { get; set; }
        public Shipment Shipment { get; set; }
    }

    public class Request
    {
        public string RequestOption { get; set; }
    }

    public class Shipment
    {
        public Shipper Shipper { get; set; }
        public ShipTo ShipTo { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
        public CodeInfo Service { get; set; }
        public Package Package { get; set; }
    }

    public class Shipper
    {
        public string ShipperNumber { get; set; } //Ups Account Number
        public Address Address { get; set; }
    }

    public class ShipTo
    {
        public Address Address { get; set; }
    }

    public class PaymentInformation
    {
        public ShipmentCharge ShipmentCharge { get; set; }
    }

    public class ShipmentCharge
    {
        public string Type { get; set; }
        public BillShipper BillShipper { get; set; }
    }

    public class BillShipper
    {
        public string AccountNumber { get; set; } //Ups Account Number
    }

    public class CodeInfo
    {
        public string Code { get; set; }
    }

    public class Package
    {
        public CodeInfo Packaging { get; set; }
        public PackageWeight PackageWeight { get; set; }
    }

    public class PackageWeight
    {
        public CodeInfo UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }
    public class Address
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
}
