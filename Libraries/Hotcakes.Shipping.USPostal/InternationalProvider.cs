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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hotcakes.Shipping.USPostal.v4;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Shipping.USPostal
{
    [Serializable]
    public class InternationalProvider : IShippingService
    {
        public const string ServiceId = "BD2CB7D9-CEF3-41D7-84A1-44FD420A1CF3";
        private List<IServiceCode> _Codes = new List<IServiceCode>();
        private readonly ILogger _Logger = new SupressLogger();
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        public InternationalProvider(USPostalServiceGlobalSettings globalSettings, ILogger logger)
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
            get { return "US Postal Service - International"; }
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
                new ServiceCode {Code = "13", DisplayName = "First-Class International Letter"},
                new ServiceCode {Code = "14", DisplayName = "First-Class International Flats"},
                new ServiceCode {Code = "15", DisplayName = "First-Class International Parcel"},
                new ServiceCode {Code = "1", DisplayName = "Express Mail International"},
                new ServiceCode {Code = "10", DisplayName = "Express Mail International Flat Rate Envelope"},
                new ServiceCode
                {
                    Code = "17",
                    DisplayName = "Express Mail International Legal Flat Rate Envelope"
                },
                new ServiceCode {Code = "2", DisplayName = "Priority Mail International"},
                new ServiceCode {Code = "8", DisplayName = "Priority Mail International Flat Rate Envelope"},
                new ServiceCode {Code = "9", DisplayName = "Priority Mail International Medium Flat Rate Box"},
                new ServiceCode {Code = "11", DisplayName = "Priority Mail International Large Flat Rate Box"},
                new ServiceCode {Code = "18", DisplayName = "Priority Mail International Gift Card Flat Rate"},
                new ServiceCode
                {
                    Code = "19",
                    DisplayName = "Priority Mail International Window Flat Rate Envelope"
                },
                new ServiceCode
                {
                    Code = "20",
                    DisplayName = "Priority Mail International Small Flat Rate Envelope"
                },
                new ServiceCode
                {
                    Code = "22",
                    DisplayName = "Priority Mail International Legal Flat Rate Envelope"
                },
                new ServiceCode
                {
                    Code = "23",
                    DisplayName = "Priority Mail International Padded Flat Rate Envelope"
                },
                new ServiceCode {Code = "4", DisplayName = "Global Express Guaranteed"},
                new ServiceCode {Code = "6", DisplayName = "Global Express Guaranteed Rectangular"},
                new ServiceCode {Code = "7", DisplayName = "Global Express Guaranteed Non-Rectangular"},
                new ServiceCode {Code = "12", DisplayName = "Global Express Guaranteed Envelopes"},
                new ServiceCode {Code = "8888", DisplayName = "Airmail Parcel Post"},
                new ServiceCode {Code = "9999", DisplayName = "Airmail Letter"}
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
                if (codefilters.Any(y => y.Code == rate.ServiceCodes))
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
                        _Logger.LogMessage("No Packaged to Rate for US Postal Service: Code 797");
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

        private List<InternationalPackage> OptimizePackages(IShipment shipment)
        {
            var optimizedPackages = shipment.Items;

            var counter = 0;
            var result = new List<InternationalPackage>();
            foreach (var s in optimizedPackages)
            {
                var pak = new InternationalPackage
                {
                    Id = counter.ToString(),
                    DestinationCountry = shipment.DestinationAddress.CountryData.SystemName,
                    ZipOrigination = Text.TrimToLength(shipment.SourceAddress.PostalCode, 5)
                };

                var weightInPounds = s.BoxWeight;
                if (s.BoxWeightType == WeightType.Kilograms)
                    weightInPounds = Conversions.KilogramsToPounds(s.BoxWeight);
                pak.Pounds = (int) Math.Floor(weightInPounds);
                pak.Ounces = Conversions.DecimalPoundsToOunces(weightInPounds);

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

                pak.Container = InternationalContainerType.Rectangular;
                pak.CommercialRates = false;

                counter++;
                result.Add(pak);
            }

            return result;
        }

        private List<IShippingRate> RatePackages(List<InternationalPackage> packages)
        {
            var rates = new List<IShippingRate>();

            var req = new InternationalRequest();
            if (!string.IsNullOrWhiteSpace(GlobalSettings.UserId))
                req.UserId = GlobalSettings.UserId;
            req.Packages = packages;

            var svc = new InternationalService();
            var res = svc.ProcessRequest(req);


            if (GlobalSettings.DiagnosticsMode)
            {
                _Logger.LogMessage("US Postal Intl. Request: " + svc.LastRequest);
                _Logger.LogMessage("US Postal Intl. Response: " + svc.LastResponse);
            }

            var hasErrors = res.Errors.Count > 0;

            foreach (InternationalServiceType possibleResponse in Enum.GetValues(typeof (InternationalServiceType)))
            {
                var AllPackagesRated = true;
                var totalRate = 0m;
                var serviceDesciption = string.Empty;

                foreach (var p in res.Packages)
                {
                    var found =
                        p.Postages.FirstOrDefault(y => y.ServiceId == ((int) possibleResponse).ToString());
                    if (found == null)
                    {
                        AllPackagesRated = false;
                        break;
                    }

                    totalRate += found.Rate;
                    serviceDesciption = HttpUtility.HtmlDecode(found.ServiceDescription);
                }

                if (AllPackagesRated && totalRate > 0)
                {
                    // Rate is good to go for all packages
                    rates.Add(new ShippingRate
                    {
                        EstimatedCost = totalRate,
                        ServiceId = Id,
                        ServiceCodes = ((int) possibleResponse).ToString(),
                        DisplayName = "USPS:" + serviceDesciption
                    });
                }
            }

            return rates;
        }
    }
}