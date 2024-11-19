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
