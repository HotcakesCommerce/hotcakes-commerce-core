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
