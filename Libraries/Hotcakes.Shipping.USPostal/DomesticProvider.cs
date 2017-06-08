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
using System.Linq;
using Hotcakes.Shipping.USPostal.v4;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Shipping.USPostal
{
    [Serializable]
    public class DomesticProvider : IShippingService
    {
        public const string ServiceId = "B28F245B-8FE5-404E-A857-A6D01904A29A";
        private List<IServiceCode> _Codes = new List<IServiceCode>();
        private readonly ILogger _Logger = new SupressLogger();
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        public DomesticProvider(USPostalServiceGlobalSettings globalSettings, ILogger logger)
        {
            _Logger = logger;
            GlobalSettings = globalSettings;
            Settings = new USPostalServiceSettings();
            InitializeCodes();
        }

        public USPostalServiceGlobalSettings GlobalSettings { get; set; }
        public USPostalServiceSettings Settings { get; set; }

        public string Id
        {
            get { return ServiceId; }
        }

        public string Name
        {
            get { return "US Postal Service - Domestic"; }
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

        public string GetTrackingUrl(string trackingCode)
        {
            if (trackingCode != string.Empty)
            {
                return "http://trkcnfrm1.smi.usps.com/PTSInternetWeb/InterLabelInquiry.do?origTrackNum=" + trackingCode;
            }
            return "http://www.usps.com";
        }

        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            _Messages.Clear();
            return GetUsPostalRatesForShipment(shipment);
        }

        private void InitializeCodes()
        {
            var result = new List<IServiceCode>
            {
                new ServiceCode {Code = "-1", DisplayName = "All Available Services"},
                new ServiceCode {Code = "0", DisplayName = "First-Class"},
                new ServiceCode {Code = "1", DisplayName = "Priority Mail"},
                new ServiceCode {Code = "2", DisplayName = "Express Mail"},
                new ServiceCode {Code = "3", DisplayName = "Express Mail Sunday/Holiday"},
                new ServiceCode {Code = "4", DisplayName = "Express Mail Hold for Pickup"},
                new ServiceCode {Code = "6", DisplayName = "Parcel Post"},
                new ServiceCode {Code = "7", DisplayName = "Media Mail"},
                new ServiceCode {Code = "8", DisplayName = "Library Material"}
            };

            _Codes = result;
        }

        public bool ShipmentHasAddresses(IShipment shipment)
        {
            if (shipment.SourceAddress == null)
            {
                return false;
            }
            if (shipment.DestinationAddress == null)
            {
                return false;
            }
            return true;
        }

        // Gets all rates filtered by service settings
        public List<IShippingRate> GetUsPostalRatesForShipment(IShipment shipment)
        {
            var rates = new List<IShippingRate>();

            var allrates = GetAllShippingRatesForShipment(shipment);

            // Filter all rates by just the ones we want
            var codefilters = Settings.ServiceCodeFilter;

            if (codefilters == null) return allrates;
            if (codefilters.Count < 1) return allrates;
            if (Settings.ReturnAllServices()) return allrates;

            foreach (var rate in allrates)
            {
                if (codefilters.Any(y => rate.ServiceCodes.StartsWith(y.Code + "-")))
                {
                    rates.Add(rate);
                }
            }

            return rates;
        }

        private List<IShippingRate> GetAllShippingRatesForShipment(IShipment shipment)
        {
            var rates = new List<IShippingRate>();

            var hasErrors = false;

            try
            {
                var packagesToRate = OptimizePackages(shipment);

                if (packagesToRate.Count > 0)
                {
                    rates = RatePackages(packagesToRate);
                }
                else
                {
                    if (GlobalSettings.DiagnosticsMode)
                    {
                        _Logger.LogMessage("No Packaged to Rate for US Postal Service: Code 795");
                    }
                }
            }

            catch (Exception ex)
            {
                _Logger.LogException(ex);
                var m = new ShippingServiceMessage();
                m.SetError("Exception", ex.Message + " | " + ex.StackTrace);
                _Messages.Add(m);
            }

            if (hasErrors)
            {
                rates = new List<IShippingRate>();
            }
            return rates;
        }

        private List<DomesticPackage> OptimizePackages(IShipment shipment)
        {
            // Determine what service to use when processing
            var service = Settings.GetServiceForProcessing();

            var optimizedPackages = shipment.Items;

            var counter = 0;
            var result = new List<DomesticPackage>();
            foreach (var s in optimizedPackages)
            {
                var pak = new DomesticPackage
                {
                    Id = counter.ToString(),
                    ZipDestination = Text.TrimToLength(shipment.DestinationAddress.PostalCode, 5),
                    ZipOrigination = Text.TrimToLength(shipment.SourceAddress.PostalCode, 5)
                };

                var weightInPounds = s.BoxWeight;
                if (s.BoxWeightType == WeightType.Kilograms)
                    weightInPounds = Conversions.KilogramsToPounds(s.BoxWeight);
                pak.Pounds = (int) Math.Floor(weightInPounds);
                pak.Ounces = Conversions.DecimalPoundsToOunces(weightInPounds);

                pak.Service = service;
                if (s.BoxLengthType == LengthType.Centimeters)
                {
                    pak.Width = Conversions.CentimetersToInches(s.BoxWidth);
                    pak.Height = Conversions.CentimetersToInches(s.BoxHeight);
                    pak.Length = Conversions.CentimetersToInches(s.BoxLength);
                }
                else
                {
                    pak.Width = s.BoxWidth;
                    pak.Height = s.BoxHeight;
                    pak.Length = s.BoxLength;
                }

                pak.Container = Settings.PackageType;
                
                // If we're using first class service, make sure we have a valid package type
                if (service == DomesticServiceType.FirstClass)
                {
                    if ((int) pak.Container < 100)
                    {
                        if (pak.Ounces < 3.5m)
                        {
                            pak.Container = DomesticPackageType.FirstClassLetter;
                        }
                        else
                        {
                            pak.Container = DomesticPackageType.FirstClassParcel;
                        }
                    }
                }

                counter++;
                result.Add(pak);
            }

            return result;
        }

        private decimal CalculateMaxWeightPerPackage(DomesticServiceType s, DomesticPackageType packageType)
        {
            if (s == DomesticServiceType.FirstClass)
            {
                if (packageType == DomesticPackageType.FirstClassLetter)
                    return USPostalConstants.MaxFirstClassLetterWeightInPounds;
                return USPostalConstants.MaxFirstClassWeightInPounds;
            }

            return USPostalConstants.MaxWeightInPounds;
        }

        private List<IShippingRate> RatePackages(List<DomesticPackage> packages)
        {
            var rates = new List<IShippingRate>();

            var req = new DomesticRequest();
            if (!string.IsNullOrWhiteSpace(GlobalSettings.UserId))
                req.UserId = GlobalSettings.UserId;
            req.Packages = packages;

            var svc = new DomesticService();
            var res = svc.ProcessRequest(req);

            if (GlobalSettings.DiagnosticsMode)
            {
                _Logger.LogMessage("US Postal Request: " + svc.LastRequest);
                _Logger.LogMessage("US Postal Response: " + svc.LastResponse);
            }

            var hasErrors = res.Errors.Count > 0;

            foreach (var possibleResponse in DomesticPackageServiceResponse.FindAll())
            {
                var AllPackagesRated = true;
                var totalRate = 0m;

                foreach (var p in res.Packages)
                {
                    var found =
                        p.Postages.FirstOrDefault(y => y.MailServiceClassId == possibleResponse.XmlClassId);
                    if (found == null)
                    {
                        AllPackagesRated = false;
                        break;
                    }

                    totalRate += found.Rate;
                }

                if (AllPackagesRated && totalRate > 0)
                {
                    // Rate is good to go for all packages
                    rates.Add(new ShippingRate
                    {
                        EstimatedCost = totalRate,
                        ServiceId = Id,
                        ServiceCodes = (int) possibleResponse.ServiceType + "-" + possibleResponse.XmlClassId,
                        DisplayName = "USPS:" + possibleResponse.XmlName
                    });
                }
            }

            return rates;
        }
    }
}