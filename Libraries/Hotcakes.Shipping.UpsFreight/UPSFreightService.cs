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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Hotcakes.Web;
using Hotcakes.Web.Logging;
using System.Net;
using System.Web.Services.Protocols;

namespace Hotcakes.Shipping.UpsFreight
{
    [Serializable]
    public class UPSFreightService : IShippingService
    {
        public const string UPSLIVESERVER = @"https://wwwcie.ups.com/webservices/";
        private readonly List<IServiceCode> _Codes = new List<IServiceCode>();

        private readonly ILogger _Logger = new SupressLogger();

        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        public UPSFreightService(UPSFreightServiceGlobalSettings globalSettings, ILogger logger)
        {
            _Logger = logger;
            GlobalSettings = globalSettings;
            Settings = new UPSFreightServiceSettings();
            InitializeCodes();
        }

        public UPSFreightServiceGlobalSettings GlobalSettings { get; set; }
        public UPSFreightServiceSettings Settings { get; set; }

        public string Name
        {
            get { return "UPS Freight"; }
        }

        public string Id
        {
            get { return "7DFEEB3E-4651-4ED1-92C8-D2554AA6F62D"; }
        }

        public bool IsSupportsTracking
        {
            get { return true; }
        }

        public ServiceSettings BaseSettings
        {
            get { return Settings; }
        }

        public List<ShippingServiceMessage> LatestMessages
        {
            get { return _Messages; }
            set { _Messages = value; }
        }

        public List<IServiceCode> ListAllServiceCodes()
        {
            return _Codes;
        }

        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            _Messages.Clear();
            return GetUPSFreightRatesForShipment(shipment);
        }

        public string GetTrackingUrl(string trackingCode)
        {
            if (!string.IsNullOrEmpty(trackingCode))
            {
                return string.Format("http://wwwapps.ups.com/WebTracking/track?track=yes&trackNums={0}", trackingCode);
            }
            return "http://wwwapps.ups.com/";
        }

        private void InitializeCodes()
        {
            _Codes.Add(new ServiceCode { Code = "308", DisplayName = "UPS Freight LTL" });
            _Codes.Add(new ServiceCode { Code = "309", DisplayName = "UPS Freight LTL - Guaranteed" });
            _Codes.Add(new ServiceCode { Code = "334", DisplayName = "UPS Freight LTL - Guaranteed A.M." });
            _Codes.Add(new ServiceCode { Code = "349", DisplayName = "UPS Standard LTL" });
        }

        // Gets all rates filtered by service settings
        private List<IShippingRate> GetUPSFreightRatesForShipment(IShipment shipment)
        {
            //var rates = new List<IShippingRate>();
            var rates = GetAllShippingRatesForShipment(shipment);

            return rates;
        }

        // Gets all available rates regardless of settings
        private List<IShippingRate> GetAllShippingRatesForShipment(IShipment shipment)
        {
            var rates = new List<IShippingRate>();
            var hasErrors = false;

            try
            {
                var sErrorMessage = string.Empty;
                var sErrorCode = string.Empty;

                var sURL = string.Concat(UPSLIVESERVER, "FreightRate");

                // Build XML
                var settings = new UPSFreightSettings
                {
                    UserID =  GlobalSettings.Username, 
                    Password = GlobalSettings.Password,
                    ServerUrl = UPSLIVESERVER,
                    License = GlobalSettings.LicenseNumber
                };


                var sXML = string.Empty;

                FreightRateRequest freightRateRequest = BuildUPSFreightRateRequestForShipment(shipment);
                FreightRateService rateService = new FreightRateService();

                //Set Web Service URL
                rateService.Url = sURL;

                //Set Security Settings For Web Service
                UPSSecurity upss = new UPSSecurity();
                UPSSecurityServiceAccessToken upsSvcToken = new UPSSecurityServiceAccessToken();
                upsSvcToken.AccessLicenseNumber = settings.License;
                upss.ServiceAccessToken = upsSvcToken;
                UPSSecurityUsernameToken upsSecUsrnameToken = new UPSSecurityUsernameToken();
                upsSecUsrnameToken.Username = settings.UserID;
                upsSecUsrnameToken.Password = settings.Password;
                upss.UsernameToken = upsSecUsrnameToken;
                rateService.UPSSecurityValue = upss;

                var sStatusCode = "-1";

                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12; //Set for SSL Webservice Call
                    FreightRateResponse freightRateResponse = rateService.ProcessFreightRate(freightRateRequest); //Send For Processing

                    if (freightRateResponse.Response.ResponseStatus.Code == "1") //Sucess
                    {
                        sStatusCode = "1";
                        var r = new ShippingRate
                        {
                            DisplayName = Settings.ServiceCodeFilter[0].DisplayName,
                            EstimatedCost = decimal.Parse(freightRateResponse.TotalShipmentCharge.MonetaryValue, NumberStyles.Currency, CultureInfo.InvariantCulture),
                            ServiceCodes = Settings.ServiceCodeFilter[0].Code,
                            ServiceId = Id
                        };
                        rates.Add(r);
                    }
                }
                catch (SoapException soapex) //Handle SOAP Exception
                {
                    _Logger.LogException(soapex);

                    var mex = new ShippingServiceMessage();

                    if(soapex.Detail != null)
                        mex.SetError("Exception", string.Concat(soapex.Detail.InnerText, " | ", soapex.Source));
                    else
                        mex.SetError("Exception", string.Concat(soapex.Message, " | ", soapex.Source));

                    _Messages.Add(mex);

                    return rates;
                }
                catch (Exception Exx)
                {
                    _Logger.LogException(Exx);

                    var mex = new ShippingServiceMessage();

                    mex.SetError("Exception", string.Concat(Exx.Message, " | ", Exx.Source));

                    _Messages.Add(mex);

                    return rates;
                }

                if (sStatusCode != "1")
                {
                    hasErrors = true;
                }
            }

            catch (Exception ex)
            {
                _Logger.LogException(ex);

                var m = new ShippingServiceMessage();

                m.SetError("Exception", string.Concat(ex.Message, " | ", ex.StackTrace));

                _Messages.Add(m);
            }

            if (hasErrors)
            {
                rates = new List<IShippingRate>();
            }

            return rates;
        }

        private FreightRateRequest BuildUPSFreightRateRequestForShipment(IShipment shipment)
        {

            FreightRateRequest freightRateRequest = new FreightRateRequest();
            RequestType request = new RequestType();
            String[] requestOption = { "1" }; //Ground
            request.RequestOption = requestOption;
            freightRateRequest.Request = request;

            /** ****************ShipFrom******************************* */
            ShipFromType shipFrom = new ShipFromType();
            AddressType shipFromAddress = new AddressType();
            String[] shipFromAddressLines = { shipment.SourceAddress.Street, shipment.SourceAddress.Street2 };
            shipFromAddress.AddressLine = shipFromAddressLines;
            shipFromAddress.City = shipment.SourceAddress.City;
            shipFromAddress.StateProvinceCode = shipment.SourceAddress.RegionBvin;
            shipFromAddress.PostalCode = shipment.SourceAddress.PostalCode;
            shipFromAddress.CountryCode = shipment.SourceAddress.CountryData.IsoCode;
            shipFrom.Address = shipFromAddress;

            freightRateRequest.ShipFrom = shipFrom;
            /** ****************ShipFrom******************************* */

            /** ****************ShipTo*************************************** */
            ShipToType shipTo = new ShipToType();
            AddressType shipToAddress = new AddressType();
            String[] shipToAddressLines = { shipment.DestinationAddress.Street, shipment.DestinationAddress.Street2 };
            shipToAddress.AddressLine = shipToAddressLines;
            shipToAddress.City = shipment.DestinationAddress.City;
            shipToAddress.StateProvinceCode = shipment.DestinationAddress.RegionBvin;
            shipToAddress.PostalCode = shipment.DestinationAddress.PostalCode;
            shipToAddress.CountryCode = shipment.DestinationAddress.CountryData.IsoCode;
            shipTo.Address = shipToAddress;

           
            freightRateRequest.ShipTo = shipTo;
            /** ****************ShipTo*************************************** */

            /** ***************PaymentInformationType************************* */

            PaymentInformationType paymentInfo = new PaymentInformationType();
            PayerType payer = new PayerType();
            payer.AttentionName = Settings.PayerName; //Required PayerName
            payer.Name = Settings.PayerName;


            AddressType payerAddress = new AddressType();
            String[] payerAddressLines = { shipment.DestinationAddress.Street };
            payerAddress.AddressLine = payerAddressLines;
            payerAddress.City = shipment.DestinationAddress.City;
            payerAddress.StateProvinceCode = shipment.DestinationAddress.RegionBvin;
            payerAddress.PostalCode = shipment.DestinationAddress.PostalCode;
            payerAddress.CountryCode = shipment.DestinationAddress.CountryData.IsoCode;
            payer.Address = payerAddress;
            paymentInfo.Payer = payer;

            RateCodeDescriptionType shipBillOption = new RateCodeDescriptionType();
            shipBillOption.Code =((int)GlobalSettings.BillingOption).ToString();
            shipBillOption.Description = GlobalSettings.BillingOption.ToString();
            paymentInfo.ShipmentBillingOption = shipBillOption;
            freightRateRequest.PaymentInformation = paymentInfo;

            /** ***************PaymentInformationType************************* */

            /** ***************Service************************************** */
            RateCodeDescriptionType service = new RateCodeDescriptionType();
            service.Code = Settings.ServiceCodeFilter[0].Code;
            service.Description = Settings.ServiceCodeFilter[0].DisplayName;
            freightRateRequest.Service = service;
            /** ***************Service************************************** */

            /** **************HandlingUnitOne************************************* */
            HandlingUnitType handUnitType = new HandlingUnitType();
            handUnitType.Quantity = shipment.Items.Count.ToString();
            RateCodeDescriptionType rateCodeDescType = new RateCodeDescriptionType();
            rateCodeDescType.Code = GlobalSettings.HandleOneUnitType.ToString();
            rateCodeDescType.Description = GlobalSettings.HandleOneUnitType.ToString();
            handUnitType.Type = rateCodeDescType;
            freightRateRequest.HandlingUnitOne = handUnitType;
            /** **************HandlingUnitOne************************************* */

            /** **************Commodity************************************* */
            // Optimize Packages for Weight
            var optimizedPackages = OptimizeSingleGroup(shipment);
            var ignoreDimensions = GlobalSettings.IgnoreDimensions;

            List<CommodityType> commodityArray = new List<CommodityType>();
            foreach (var pak in optimizedPackages)
            {
                //Prepare Commodity Item
                CommodityType commodity = WriteSingleCommidityPackage(pak, ignoreDimensions);
                commodityArray.Add(commodity);
            }
            freightRateRequest.Commodity = commodityArray.ToArray();

            /** **************Commodity************************************* */

            return freightRateRequest;
        }

        protected bool IsOversized(IShippable prod)
        {
            var IsOversize = false;
            var girth = (double)(prod.BoxLength + 2 * prod.BoxHeight + 2 * prod.BoxWidth);
            if (girth > 84)
            {
                //this is an oversize product
                IsOversize = true;
            }

            return IsOversize;
        }

        private List<IShippable> OptimizeSingleGroup(IShipment shipment)
        {
            const decimal MAXWEIGHT = 70;

            var result = new List<IShippable>();
            var itemsToSplit = new List<IShippable>();

            foreach (var item in shipment.Items)
            {
                if (IsOversized(item))
                {
                    result.Add(item.CloneShippable());
                }
                else
                {
                    itemsToSplit.Add(item);
                }
            }

            IShippable tempPackage = new Shippable();

            foreach (var pak in itemsToSplit)
            {
                if (MAXWEIGHT - tempPackage.BoxWeight >= pak.BoxWeight)
                {
                    // add to current box
                    tempPackage.BoxWeight += pak.BoxWeight;
                    tempPackage.QuantityOfItemsInBox += pak.QuantityOfItemsInBox;
                    tempPackage.BoxValue += pak.BoxValue;
                }
                else
                {
                    // Save the temp package if it has items
                    if (tempPackage.BoxWeight > 0 || tempPackage.QuantityOfItemsInBox > 0)
                    {
                        result.Add(tempPackage.CloneShippable());
                        tempPackage = new Shippable();
                    }

                    // create new box
                    if (pak.BoxWeight > MAXWEIGHT)
                    {
                        //Loop to break up > maxWeight Packages
                        var currentItemsInBox = pak.QuantityOfItemsInBox;
                        var currentWeight = pak.BoxWeight;

                        while (currentWeight > 0)
                        {
                            if (currentWeight > MAXWEIGHT)
                            {
                                var newP = pak.CloneShippable();
                                newP.BoxWeight = MAXWEIGHT;

                                if (currentItemsInBox > 0)
                                {
                                    currentItemsInBox -= 1;
                                    newP.QuantityOfItemsInBox = 1;
                                }

                                result.Add(newP);
                                currentWeight = currentWeight - MAXWEIGHT;

                                if (currentWeight < 0)
                                {
                                    currentWeight = 0;
                                }
                            }
                            else
                            {
                                // Create a new shippable box 
                                var newP = pak.CloneShippable();
                                newP.BoxWeight = currentWeight;

                                if (currentItemsInBox > 0)
                                {
                                    newP.QuantityOfItemsInBox = currentItemsInBox;
                                }

                                result.Add(newP);
                                currentWeight = 0;
                            }
                        }
                    }
                    else
                    {
                        tempPackage = pak.CloneShippable();
                    }
                }
            }

            // Save the temp package if it has items
            if (tempPackage.BoxWeight > 0 || tempPackage.QuantityOfItemsInBox > 0)
            {
                result.Add(tempPackage.CloneShippable());
                tempPackage = new Shippable();
            }

            return result;
        }

        private CommodityType WriteSingleCommidityPackage(IShippable pak, bool ignoreDimensions)
        {
            decimal dGirth = 0;
            decimal dLength = 0;
            decimal dHeight = 0;
            decimal dwidth = 0;

            CommodityType commodity = new CommodityType();
            CommodityValueType commValue = new CommodityValueType();
            commodity.NumberOfPieces = pak.QuantityOfItemsInBox.ToString();

            RateCodeDescriptionType packagingType = new RateCodeDescriptionType();
            packagingType.Code = ((int)GlobalSettings.DefaultPackaging).ToString();
            packagingType.Description = GlobalSettings.DefaultPackaging.ToString();
            commodity.PackagingType = packagingType;

            WeightType weight = new WeightType();
            UnitOfMeasurementType unitOfMeasurement = new UnitOfMeasurementType();

            unitOfMeasurement.Code = (pak.BoxWeightType == Hotcakes.Shipping.WeightType.Pounds ? "LBS" : "KGS");
            unitOfMeasurement.Description = (pak.BoxWeightType == Hotcakes.Shipping.WeightType.Pounds ? "LBS" : "KGS");

            weight.UnitOfMeasurement = unitOfMeasurement;
            if (pak.BoxWeight > 0)
                weight.Value = Math.Round(pak.BoxWeight, 1).ToString();
            else
                weight.Value = "1";

            commodity.Weight = weight;
            commodity.Description = "Weight";
            commodity.FreightClass = GlobalSettings.FreightClass;


            var dimensions = new List<DimensionAmount>();

            if (pak.BoxLengthType == LengthType.Centimeters)
            {
                dimensions.Add(new DimensionAmount(Conversions.CentimetersToInches(pak.BoxLength)));
                dimensions.Add(new DimensionAmount(Conversions.CentimetersToInches(pak.BoxWidth)));
                dimensions.Add(new DimensionAmount(Conversions.CentimetersToInches(pak.BoxHeight)));
            }
            else
            {
                dimensions.Add(new DimensionAmount(pak.BoxLength));
                dimensions.Add(new DimensionAmount(pak.BoxWidth));
                dimensions.Add(new DimensionAmount(pak.BoxWidth));
            }

            var sorted = (from d in dimensions
                          orderby d.Amount descending
                          select d.Amount).ToList();
            dLength = sorted[0];
            dwidth = sorted[1];
            dHeight = sorted[2];

            dGirth = dwidth + dwidth + dHeight + dHeight;
            //Dimensions can be skipped in latest UPS specs
            if (!ignoreDimensions)
            {
                if (dLength > 0 | dHeight > 0 | dwidth > 0)
                {
                    DimensionsType commondityDimensions = new DimensionsType();

                    commondityDimensions.Height = Math.Round(dHeight, 2).ToString(CultureInfo.InvariantCulture);
                    commondityDimensions.Width = Math.Round(dwidth, 2).ToString(CultureInfo.InvariantCulture);
                    commondityDimensions.Length = Math.Round(dLength, 2).ToString(CultureInfo.InvariantCulture);

                    UnitOfMeasurementType unitOfMeasurementDimention = new UnitOfMeasurementType();
                    unitOfMeasurementDimention.Code = "IN";
                    unitOfMeasurementDimention.Description = "Inch";
                    commondityDimensions.UnitOfMeasurement = unitOfMeasurementDimention;

                    commodity.Dimensions = commondityDimensions;
                }
            }
            return commodity;
        }

        private class DimensionAmount
        {
            public DimensionAmount(decimal amount)
            {
                Amount = amount;
            }

            public decimal Amount { get; set; }
        }




    }
}