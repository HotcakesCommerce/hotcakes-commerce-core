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
    public class RatesRequest 
    {
        public RateRequest RateRequest { get; set; }
    }
    public class RateRequest
    {
        public RequestBody Request { get; set; }
        public CodeInfo PickupType { get; set; }
        public RateShipment Shipment { get; set; }
    }

    public class RequestBody
    {
        public TransactionReference TransactionReference { get; set; }
    }

    public class RateShipment
    {
        public Shipper Shipper { get; set; }
        public ShipTo ShipTo { get; set; }
        public RatePackage Package { get; set; }
    }


    public class RatePackage
    {
        public string OversizeIndicator { get; set; }
        public PackagingType PackagingType { get; set; }
        public Dimensions Dimensions { get; set; }
        public PackageWeight PackageWeight { get; set; }
    }

    public class PackagingType
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Dimensions
    {
        public CodeInfo UnitOfMeasurement { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

}
