#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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

using Hotcakes.Shipping.Ups.Models;
using Hotcakes.Shipping.Ups.Models.Responses;
using Hotcakes.Shipping.Ups.Services;
using Hotcakes.Web;
using Hotcakes.Web.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public class UPSService : IShippingService
    {
        public const string BaseUrlTesting = "https://wwwcie.ups.com";
        public const string BaseUrlProduction = "https://onlinetools.ups.com";
        public readonly string UPSLIVESERVER;
        private readonly List<IServiceCode> _Codes = new List<IServiceCode>();

        private readonly ILogger _Logger = new SupressLogger();

        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        private readonly TokenService _tokenService;

        public UPSService(UPSServiceGlobalSettings globalSettings, ILogger logger)
        {
            _Logger = logger;
            GlobalSettings = globalSettings;
            Settings = new UPSServiceSettings();
            UPSLIVESERVER = globalSettings.TestingMode ? BaseUrlTesting : BaseUrlProduction;
            _tokenService = new TokenService(globalSettings, UPSLIVESERVER);
            InitializeCodes();
        }

        public UPSServiceGlobalSettings GlobalSettings { get; set; }
        public UPSServiceSettings Settings { get; set; }

        public string Name
        {
            get { return "UPS"; }
        }

        public string Id
        {
            get { return "55E5A698-1111-4F78-958B-70B1BC5941B8"; }
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
            return GetUPSRatesForShipment(shipment);
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
            _Codes.Add(new ServiceCode {Code = "1", DisplayName = "UPS Next Day Air"});
            _Codes.Add(new ServiceCode {Code = "2", DisplayName = "UPS Second Day Air"});
            _Codes.Add(new ServiceCode {Code = "3", DisplayName = "UPS Ground"});
            _Codes.Add(new ServiceCode {Code = "7", DisplayName = "UPS Worldwide Express"});
            _Codes.Add(new ServiceCode {Code = "8", DisplayName = "UPS Worldwide Expedited"});
            _Codes.Add(new ServiceCode {Code = "11", DisplayName = "UPS Standard"});
            _Codes.Add(new ServiceCode {Code = "12", DisplayName = "UPS Three Day Select"});
            _Codes.Add(new ServiceCode {Code = "13", DisplayName = "UPS Next Day Air Saver"});
            _Codes.Add(new ServiceCode {Code = "14", DisplayName = "UPS Next Day Air Early AM"});
            _Codes.Add(new ServiceCode {Code = "54", DisplayName = "UPS Worldwide Express Plus"});
            _Codes.Add(new ServiceCode {Code = "59", DisplayName = "UPS Second Day Air AM"});
            _Codes.Add(new ServiceCode {Code = "65", DisplayName = "UPS Saver"});
        }

        // Gets all rates filtered by service settings
        private List<IShippingRate> GetUPSRatesForShipment(IShipment shipment)
        {
            var rates = new List<IShippingRate>();
            var allrates = GetAllShippingRatesForShipment(shipment);

            if (GlobalSettings.DiagnosticsMode)
            {
                var sb = new StringBuilder();

                foreach (var rate in allrates)
                {
                    sb.AppendFormat("Service Id: {0} Display Name: {1} Service Codes:{2} Estimated Cost:{3} || ",
                        rate.ServiceId, rate.DisplayName, rate.ServiceCodes, rate.EstimatedCost);
                }

                sb.Append("End of Rates || If this is all that you see, then there were no rates returned from UPS.");

                _Logger.LogMessage("UPS RATES FOUND", sb.ToString(), EventLogSeverity.Information);
            }

            // Filter all rates by just the ones we want
            var codefilters = Settings.ServiceCodeFilter;

            foreach (var rate in allrates)
            {
                if (Settings.GetAllRates || codefilters.Count < 1)
                {
                    rates.Add(rate);
                }
                else
                {
                    if (codefilters.Any(y => y.Code == rate.ServiceCodes.TrimStart('0')))
                    {
                        rates.Add(rate);
                    }
                }
            }

            if (GlobalSettings.DiagnosticsMode)
            {
                var sb = new StringBuilder();

                foreach (var rate in rates)
                {
                    sb.AppendFormat(
                        "Service Id: {0} Display Name: {1} Service Codes:{2} Estimated Cost:{3} || ", rate.ServiceId,
                        rate.DisplayName, rate.ServiceCodes, rate.EstimatedCost);
                }

                sb.Append(
                    "End of Rates || If this is all that you see, then you're not yet registered with UPS, there were no rates returned from UPS, the address was missing/invalid, or none of the returned rates match the shipping details of the line items in your order.");

                _Logger.LogMessage("VALID UPS RATES", sb.ToString(), EventLogSeverity.Information);
            }

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
                var settings = new UpsSettings
                {
                    ServerUrl = UPSLIVESERVER,
                    ClientId = GlobalSettings.ClientId,
                    ClientSecret = GlobalSettings.ClientSecret
                };

                try
                {
                    var ratesRequest = BuildUPSRateRequestForShipment(shipment, settings);
                    var jsonRequest = JsonConvert.SerializeObject(ratesRequest);

                    #region GetRates
                    using (var client = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Post, $"{UPSLIVESERVER}/api/rating/v2403/Shop?additionalinfo=");
                        request.Headers.Add("transID", "");
                        request.Headers.Add("transactionSrc", "testing");
                        request.Headers.Add("Authorization", $"Bearer {_tokenService.GetAccessTokenAsync()}");
                        var content = new StringContent(jsonRequest, null, "application/json");
                        request.Content = content;
                        var response = client.SendAsync(request).Result;
                        response.EnsureSuccessStatusCode();
                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = response.Content.ReadAsStringAsync().Result;
                            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);
                            foreach (var error in errorResponse.Response.Errors)
                            {
                                _Logger.LogMessage($"Error Code: {error.Code}, Message: {error.Message}");
                            }

                            return rates;
                        }
                        var stringContent = response.Content.ReadAsStringAsync().Result;
                        if (GlobalSettings.DiagnosticsMode)
                        {
                            _Logger.LogMessage(string.Format("UPS API Response: {0}", stringContent));
                        }

                        var ratesResponse = RatesResponse.FromJson(stringContent);

                        if (ratesResponse.RateResponse.Response.ResponseStatus.Code != "1")
                        {
                            return rates;
                        }

                        foreach (var ratedShipment in ratesResponse.RateResponse.RatedShipment)
                        {
                            var dRate = -1m;
                            if (ratedShipment.TotalCharges.MonetaryValue.Length > 0)
                            {
                                dRate = decimal.Parse(ratedShipment.TotalCharges.MonetaryValue, NumberStyles.Currency, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                var nop = new ShippingServiceMessage();
                                nop.SetInfo(string.Empty, "No UPS Postage Found");
                                _Messages.Add(nop);
                                hasErrors = true;
                            }

                            if (dRate >= 0)
                            {
                                var shippingRate = new ShippingRate
                                {
                                    DisplayName = DecodeUpsServiceCode(ratedShipment.Service.Code),
                                    EstimatedCost = dRate,
                                    ServiceCodes = ratedShipment.Service.Code,
                                    ServiceId = Id

                                };
                                rates.Add(shippingRate);
                            }

                            if (GlobalSettings.DiagnosticsMode)
                            {
                                var msg = new ShippingServiceMessage();

                                msg.SetDiagnostics("UPS Rates Found",
                                    string.Concat("StatusCode = ", ratesResponse.RateResponse.Response.ResponseStatus.Code, ", Postage = ", ratedShipment.TotalCharges.MonetaryValue,
                                        ", Errors = ", sErrorMessage, ", Rate = ", dRate));

                                _Messages.Add(msg);
                            }
                        }
                    }
                    #endregion

                }
                catch (Exception Exx)
                {
                    _Logger.LogException(Exx);

                    var mex = new ShippingServiceMessage();

                    mex.SetError("Exception", string.Concat(Exx.Message, " | ", Exx.Source));

                    _Messages.Add(mex);

                    return rates;
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

        private string DecodeUpsServiceCode(string sCode)
        {
            var temp = sCode;

            if (temp.StartsWith("0"))
            {
                temp = temp.Substring(1, temp.Length - 1);
            }

            foreach (var code in _Codes)
            {
                if (code.Code == temp)
                {
                    return code.DisplayName;
                }
            }

            return "UPS";
        }

      
        private RatesRequest BuildUPSRateRequestForShipment(IShipment shipment, UpsSettings settings)
        {
            var result = new RatesRequest();
            var request = new RateRequest();

            try
            {
                request.Request = new RequestBody();
                //--------------------------------------------
                // TransactionReference
                request.Request.TransactionReference = new Models.TransactionReference
                {
                    CustomerContext = "Rate Request",
                    TransactionIdentifier = new Guid().ToString()
                };
                // End TransactionReference
                //--------------------------------------------
                //--------------------------------------------
                // Pickup Type
                if (GlobalSettings.PickUpType != PickupType.Unknown)
                {
                    var pickupCode = ((int)GlobalSettings.PickUpType).ToString();

                    if (pickupCode.Trim().Length < 2)
                    {
                        pickupCode = "0" + pickupCode;
                    }

                    request.PickupType = new CodeInfo
                    {
                        Code = pickupCode
                    };
                }
                // End Pickup Type
                //--------------------------------------------

                //--------------------------------------------
                // Shipment
                request.Shipment = new RateShipment();

                // Shipper

                request.Shipment.Shipper = new Shipper
                {
                    Address = new Address(),
                    ShipperNumber = GlobalSettings.AccountNumber
                };

                //Use City name for countries that don't have postal codes
                if (shipment.SourceAddress.PostalCode.Trim().Length > 0)
                {
                   request.Shipment.Shipper.Address.PostalCode = XmlTools.TrimToLength(shipment.SourceAddress.PostalCode.Trim(), 9);
                }
                else
                {
                    request.Shipment.Shipper.Address.City = XmlTools.TrimToLength(shipment.SourceAddress.City.Trim(), 30);
                }

                if (shipment.SourceAddress.RegionData != null)
                {
                    request.Shipment.Shipper.Address.StateProvinceCode = shipment.SourceAddress.RegionData.Abbreviation;
                }

                request.Shipment.Shipper.Address.CountryCode = shipment.SourceAddress.CountryData.IsoCode;

                // Ship To
                request.Shipment.ShipTo = new ShipTo
                {
                    Address = new Address()
                };

                if (shipment.DestinationAddress.PostalCode.Length > 0)
                {
                    request.Shipment.ShipTo.Address.PostalCode = shipment.DestinationAddress.PostalCode;
                }
                else
                {
                    if (shipment.DestinationAddress.City.Length > 0)
                    {
                        request.Shipment.ShipTo.Address.City = shipment.DestinationAddress.City;
                    }
                }

                if (shipment.DestinationAddress.RegionData != null)
                {
                    request.Shipment.ShipTo.Address.StateProvinceCode = shipment.DestinationAddress.RegionData.Abbreviation;
                }

                if (shipment.DestinationAddress.CountryData.Bvin.Length > 0)
                {
                    request.Shipment.ShipTo.Address.CountryCode = shipment.DestinationAddress.CountryData.IsoCode;
                }

                var ignoreDimensions = GlobalSettings.IgnoreDimensions;

                // Optimize Packages for Weight
                var optimizedPackages = OptimizeSingleGroup(shipment);

                foreach (var p in optimizedPackages)
                {
                    WriteSingleUPSPackage(ref request, p, ignoreDimensions);
                }
                // End Shipment
                //--------------------------------------------
                // End Agreement Request
                //--------------------------------------------
            }
            catch (Exception ex)
            {
                _Logger.LogException(ex);
            }
            result.RateRequest = request;
            return result;
        }

        protected bool IsOversized(IShippable prod)
        {
            var IsOversize = false;
            var girth = (double) (prod.BoxLength + 2*prod.BoxHeight + 2*prod.BoxWidth);
            if (girth > 84)
            {
                //this is an oversize product
                IsOversize = true;
            }

            return IsOversize;
        }

        private void WriteSingleUPSPackage(ref RateRequest request, IShippable pak, bool ignoreDimensions)
        {
            decimal dGirth = 0;
            decimal dLength = 0;
            decimal dHeight = 0;
            decimal dwidth = 0;

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
            
            //--------------------------------------------
            // Package
            request.Shipment.Package = new RatePackage();

            var packageType = "02";

            if (GlobalSettings.DefaultPackaging != (int) PackagingType.Unknown)
            {
                packageType = ((int) GlobalSettings.DefaultPackaging).ToString();
                if (packageType.Trim().Length < 2)
                {
                    packageType = "0" + packageType;
                }
            }
            request.Shipment.Package.PackagingType = new Models.PackagingType
            {
                Code = packageType,
                Description = "Package"
            };

            //Dimensions can be skipped in latest UPS specs
            if (!ignoreDimensions)
            {
                if (dLength > 0 | dHeight > 0 | dwidth > 0)
                {
                    request.Shipment.Package.Dimensions = new Dimensions
                    {
                        Height = Math.Round(dHeight, 2).ToString(CultureInfo.InvariantCulture),
                        Length = Math.Round(dLength, 2).ToString(CultureInfo.InvariantCulture),
                        Width = Math.Round(dwidth, 2).ToString(CultureInfo.InvariantCulture),
                        UnitOfMeasurement = new CodeInfo 
                        {
                            Code = "IN"
                        }
                    };
                }
            }

            if (pak.BoxWeight > 0)
            {
                request.Shipment.Package.PackageWeight = new PackageWeight 
                {
                    UnitOfMeasurement = new CodeInfo 
                    {
                        Code = pak.BoxWeightType == WeightType.Pounds ? "LBS" : "KGS"
                    },
                    Weight = Math.Round(pak.BoxWeight, 1).ToString(CultureInfo.InvariantCulture)
                };
            }
            else
            {
                request.Shipment.Package.PackageWeight = new PackageWeight
                {
                    UnitOfMeasurement = new CodeInfo
                    {
                        Code = pak.BoxWeightType == WeightType.Pounds ? "LBS" : "KGS"
                    },
                    Weight = Math.Round(0.1, 1).ToString(CultureInfo.InvariantCulture)
                };
            }

            if (ignoreDimensions == false)
            {
                // Oversize Checks
                var oversizeCheck = dGirth + dLength;
                if (oversizeCheck > 84)
                {
                    if (oversizeCheck < 108 & pak.BoxWeight < 30)
                    {
                        request.Shipment.Package.OversizeIndicator = "1";
                    }
                    else
                    {
                        if (pak.BoxWeight < 70)
                        {
                            request.Shipment.Package.OversizeIndicator = "2";
                        }
                        else
                        {
                            request.Shipment.Package.OversizeIndicator = "0";
                        }
                    }
                }
            }
            // End Package
            //--------------------------------------------
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