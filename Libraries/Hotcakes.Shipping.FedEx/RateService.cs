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
using System.Linq;
using System.Net;
using System.Text;
using Hotcakes.Shipping.FedEx.FedExRateServices;
using Hotcakes.Web.Geography;
using Hotcakes.Web.Logging;

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class RateService
    {
        private const int DefaultTimeout = 100000;

        private RateService()
        {
        }

        public static string SendRequest(string serviceUrl, string postData)
        {
            return SendRequest(serviceUrl, postData, null);
        }

        public static string SendRequest(string serviceUrl, string postData, WebProxy proxy)
        {
            WebResponse objResp;
            WebRequest objReq;
            var strResp = string.Empty;
            byte[] byteReq;

            try
            {
                byteReq = Encoding.UTF8.GetBytes(postData);
                objReq = WebRequest.Create(serviceUrl);
                objReq.Method = "POST";
                objReq.ContentLength = byteReq.Length;
                objReq.ContentType = "application/x-www-form-urlencoded";
                objReq.Timeout = DefaultTimeout;
                if (proxy != null)
                {
                    objReq.Proxy = proxy;
                }
                var OutStream = objReq.GetRequestStream();
                OutStream.Write(byteReq, 0, byteReq.Length);
                OutStream.Close();
                objResp = objReq.GetResponse();
                var sr = new StreamReader(objResp.GetResponseStream(), Encoding.UTF8, true);
                strResp += sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error SendRequest: " + ex.Message + " " + ex.Source);
            }

            return strResp;
        }

        public static ShippingRate RatePackage(FedExGlobalServiceSettings globals, ILogger logger,
            FedExServiceSettings settings, IShipment package)
        {
            var result = new ShippingRate();

            // Get ServiceType
            var currentServiceType = (ServiceType) settings.ServiceCode;

            // Get PackageType
            var currentPackagingType = (PackageType) settings.Packaging;

            // Set max weight by service
            var carCode = GetCarrierCode(currentServiceType);

            // set the negotiated rates to true, or the setting in the local settings
            var useNegotiatedRates = !settings.ContainsKey("UseNegotiatedRates") ||
                                     bool.Parse(settings["UseNegotiatedRates"]);

            result.EstimatedCost = RateSinglePackage(globals,
                logger,
                package,
                currentServiceType,
                currentPackagingType,
                carCode,
                useNegotiatedRates);

            return result;
        }


        // Mappers between local enums and service enums
        private static CarrierCodeType GetCarrierCode(ServiceType service)
        {
            var result = CarrierCodeType.FDXG;

            switch (service)
            {
                case ServiceType.EUROPEFIRSTINTERNATIONALPRIORITY:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.FEDEX1DAYFREIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.FEDEX2DAY:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.FEDEX2DAYFREIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.FEDEX3DAYFREIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.FEDEXEXPRESSSAVER:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.FEDEXGROUND:
                    result = CarrierCodeType.FDXG;
                    break;
                case ServiceType.FIRSTOVERNIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.GROUNDHOMEDELIVERY:
                    result = CarrierCodeType.FDXG;
                    break;
                case ServiceType.INTERNATIONALECONOMY:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.INTERNATIONALECONOMYFREIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.INTERNATIONALFIRST:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.INTERNATIONALPRIORITY:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.INTERNATIONALPRIORITYFREIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.PRIORITYOVERNIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                case ServiceType.STANDARDOVERNIGHT:
                    result = CarrierCodeType.FDXE;
                    break;
                default:
                    result = CarrierCodeType.FDXE;
                    break;
            }

            return result;
        }

        private static DropoffType GetDropOffType(DropOffType dropType)
        {
            var result = DropoffType.BUSINESS_SERVICE_CENTER;
            switch (dropType)
            {
                case DropOffType.BUSINESSSERVICECENTER:
                    return DropoffType.BUSINESS_SERVICE_CENTER;
                case DropOffType.DROPBOX:
                    return DropoffType.DROP_BOX;
                case DropOffType.REGULARPICKUP:
                    return DropoffType.REGULAR_PICKUP;
                case DropOffType.REQUESTCOURIER:
                    return DropoffType.REQUEST_COURIER;
                case DropOffType.STATION:
                    return DropoffType.STATION;
            }
            return result;
        }

        private static FedExRateServices.ServiceType GetServiceType(ServiceType serviceType)
        {
            var result = FedExRateServices.ServiceType.FEDEX_2_DAY;

            switch (serviceType)
            {
                case ServiceType.EUROPEFIRSTINTERNATIONALPRIORITY:
                    return FedExRateServices.ServiceType.EUROPE_FIRST_INTERNATIONAL_PRIORITY;
                case ServiceType.FEDEX1DAYFREIGHT:
                    return FedExRateServices.ServiceType.FEDEX_1_DAY_FREIGHT;
                case ServiceType.FEDEX2DAY:
                    return FedExRateServices.ServiceType.FEDEX_2_DAY;
                case ServiceType.FEDEX2DAY_AM:
                    return FedExRateServices.ServiceType.FEDEX_2_DAY_AM;
                case ServiceType.FEDEX2DAYFREIGHT:
                    return FedExRateServices.ServiceType.FEDEX_2_DAY_FREIGHT;
                case ServiceType.FEDEX3DAYFREIGHT:
                    return FedExRateServices.ServiceType.FEDEX_3_DAY_FREIGHT;
                case ServiceType.FEDEXEXPRESSSAVER:
                    return FedExRateServices.ServiceType.FEDEX_EXPRESS_SAVER;
                case ServiceType.FIRSTOVERNIGHT:
                    return FedExRateServices.ServiceType.FIRST_OVERNIGHT;
                case ServiceType.FEDEXGROUND:
                    return FedExRateServices.ServiceType.FEDEX_GROUND;
                case ServiceType.GROUNDHOMEDELIVERY:
                    return FedExRateServices.ServiceType.GROUND_HOME_DELIVERY;
                case ServiceType.INTERNATIONALECONOMY:
                    return FedExRateServices.ServiceType.INTERNATIONAL_ECONOMY;
                case ServiceType.INTERNATIONALECONOMYFREIGHT:
                    return FedExRateServices.ServiceType.INTERNATIONAL_ECONOMY_FREIGHT;
                case ServiceType.INTERNATIONALFIRST:
                    return FedExRateServices.ServiceType.INTERNATIONAL_FIRST;
                case ServiceType.INTERNATIONALPRIORITY:
                    return FedExRateServices.ServiceType.INTERNATIONAL_PRIORITY;
                case ServiceType.INTERNATIONALPRIORITYFREIGHT:
                    return FedExRateServices.ServiceType.INTERNATIONAL_PRIORITY_FREIGHT;
                case ServiceType.PRIORITYOVERNIGHT:
                    return FedExRateServices.ServiceType.PRIORITY_OVERNIGHT;
                case ServiceType.STANDARDOVERNIGHT:
                    return FedExRateServices.ServiceType.STANDARD_OVERNIGHT;
                case ServiceType.SMARTPOST:
                    return FedExRateServices.ServiceType.SMART_POST;
            }

            return result;
        }

        private static PackagingType GetPackageType(PackageType packageType)
        {
            var result = PackagingType.YOUR_PACKAGING;

            switch (packageType)
            {
                case PackageType.FEDEX10KGBOX:
                    return PackagingType.FEDEX_10KG_BOX;
                case PackageType.FEDEX25KGBOX:
                    return PackagingType.FEDEX_25KG_BOX;
                case PackageType.FEDEXBOX:
                    return PackagingType.FEDEX_BOX;
                case PackageType.FEDEXENVELOPE:
                    return PackagingType.FEDEX_ENVELOPE;
                case PackageType.FEDEXPAK:
                    return PackagingType.FEDEX_PAK;
                case PackageType.FEDEXTUBE:
                    return PackagingType.FEDEX_TUBE;
                case PackageType.YOURPACKAGING:
                    return PackagingType.YOUR_PACKAGING;
            }

            return result;
        }

        private static string GetCountryCode(ICountry country)
        {
            var result = "US";
            if (country != null)
            {
                return country.IsoCode;
            }
            return result;
        }

        private static decimal RateSinglePackage(FedExGlobalServiceSettings globalSettings, ILogger logger,
            IShipment pak,
            ServiceType service, PackageType packaging, CarrierCodeType carCode,
            bool useNegotiatedRates)
        {
            var result = 0m;

            try
            {
                // Auth Header Data
                var req = new RateRequest
                {
                    WebAuthenticationDetail = new WebAuthenticationDetail
                    {
                        UserCredential = new WebAuthenticationCredential
                        {
                            Key = globalSettings.UserKey,
                            Password = globalSettings.UserPassword
                        }
                    }
                };
                req.ClientDetail = new ClientDetail
                {
                    AccountNumber = globalSettings.AccountNumber,
                    MeterNumber = globalSettings.MeterNumber,
                    IntegratorId = "Hotcakes"
                };
                req.Version = new VersionId();

                // Basic Transaction Data
                req.TransactionDetail = new TransactionDetail
                {
                    CustomerTransactionId = Guid.NewGuid().ToString()
                };
                req.ReturnTransitAndCommit = false;
                req.CarrierCodes = new CarrierCodeType[1] {carCode};

                // Shipment Details
                req.RequestedShipment = new RequestedShipment
                {
                    LabelSpecification = new LabelSpecification
                    {
                        ImageType = ShippingDocumentImageType.PDF,
                        LabelFormatType = LabelFormatType.COMMON2D,
                        CustomerSpecifiedDetail = new CustomerSpecifiedLabelDetail()
                    },
                    RateRequestTypes = new[] {RateRequestType.LIST},
                    DropoffType = GetDropOffType(globalSettings.DefaultDropOffType),
                    PackagingType = GetPackageType(packaging),
                    TotalWeight = new Weight
                    {
                        Value = Math.Round(pak.Items.Sum(y => y.BoxWeight), 1),
                        Units =
                            pak.Items[0].BoxWeightType == Shipping.WeightType.Kilograms
                                ? WeightUnits.KG
                                : WeightUnits.LB
                    },
                    PackageCount = pak.Items.Count.ToString(),
                    RequestedPackageLineItems = new RequestedPackageLineItem[pak.Items.Count]
                };
                
                /*
                 * Possible values for RateRequestType include: LIST and ACCOUNT.
                 * ACCOUNT will return negotiated rates, but LIST will return both regular and negotiated rates.
                 * http://www.fedex.com/us/developer/product/WebServices/MyWebHelp_March2010/Content/WS_Developer_Guide/Rate_Services.htm
                 */
                //req.RequestedShipment.RateRequestTypes = new[] { RateRequestType.ACCOUNT };
                
                // Uncomment these lines to get insured values passed in
                //
                //var totalValue = pak.Items.Sum(y => y.BoxValue);
                //req.RequestedShipment.TotalInsuredValue = new Money();
                //req.RequestedShipment.TotalInsuredValue.Amount = totalValue;
                //req.RequestedShipment.TotalInsuredValue.Currency = "USD";

                for (var i = 0; i < pak.Items.Count; i++)
                {
                    req.RequestedShipment.RequestedPackageLineItems[i] = new RequestedPackageLineItem
                    {
                        GroupNumber = "1",
                        GroupPackageCount = (i + 1).ToString(),
                        Weight = new Weight
                        {
                            Value = pak.Items[i].BoxWeight,
                            Units =
                                pak.Items[i].BoxWeightType == Shipping.WeightType.Kilograms
                                    ? WeightUnits.KG
                                    : WeightUnits.LB
                        }
                    };

                    req.RequestedShipment.RequestedPackageLineItems[i].Dimensions = new Dimensions
                    {
                        Height = pak.Items[i].BoxHeight.ToString(),
                        Length = pak.Items[i].BoxLength.ToString(),
                        Width = pak.Items[i].BoxWidth.ToString(),
                        Units = pak.Items[i].BoxLengthType == LengthType.Centimeters ? LinearUnits.CM : LinearUnits.IN
                    };
                }

                req.RequestedShipment.Recipient = new Party
                {
                    Address = new FedExRateServices.Address
                    {
                        City = pak.DestinationAddress.City,
                        CountryCode = GetCountryCode(pak.DestinationAddress.CountryData),
                        PostalCode = pak.DestinationAddress.PostalCode
                    }
                };


                if (pak.DestinationAddress.CountryData.IsoCode == "US" ||
                    pak.DestinationAddress.CountryData.IsoCode == "CA")
                {
                    req.RequestedShipment.Recipient.Address.StateOrProvinceCode = pak.DestinationAddress.RegionBvin;
                }
                else
                {
                    req.RequestedShipment.Recipient.Address.StateOrProvinceCode = string.Empty;
                }
                req.RequestedShipment.Recipient.Address.StreetLines = new string[2]
                {pak.DestinationAddress.Street, pak.DestinationAddress.Street2};

                switch (service)
                {
                    case ServiceType.GROUNDHOMEDELIVERY:
                        req.RequestedShipment.Recipient.Address.Residential = true;
                        req.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
                        break;
                    case ServiceType.FEDEXGROUND:
                        req.RequestedShipment.Recipient.Address.Residential = false;
                        req.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
                        break;
                    default:
                        req.RequestedShipment.Recipient.Address.ResidentialSpecified = false;
                        break;
                }

                req.RequestedShipment.Shipper = new Party
                {
                    AccountNumber = globalSettings.AccountNumber,
                    Address = new FedExRateServices.Address
                    {
                        City = pak.SourceAddress.City,
                        CountryCode = GetCountryCode(pak.SourceAddress.CountryData),
                        PostalCode = pak.SourceAddress.PostalCode,
                        Residential = false
                    }
                };

                if (pak.SourceAddress.CountryData.IsoCode == "US" || // US or Canada
                    pak.SourceAddress.CountryData.IsoCode == "CA")
                {
                    req.RequestedShipment.Shipper.Address.StateOrProvinceCode = pak.SourceAddress.RegionBvin;
                }
                else
                {
                    req.RequestedShipment.Shipper.Address.StateOrProvinceCode = string.Empty;
                }
                req.RequestedShipment.Shipper.Address.StreetLines = new string[2]
                {pak.SourceAddress.Street, pak.SourceAddress.Street2};

                var svc = new FedExRateServices.RateService
                {
                    Url =
                        globalSettings.UseDevelopmentServiceUrl
                            ? FedExConstants.DevRateServiceUrl
                            : FedExConstants.LiveRateServiceUrl
                };

                var res = svc.getRates(req);

                if (res.HighestSeverity == NotificationSeverityType.ERROR ||
                    res.HighestSeverity == NotificationSeverityType.FAILURE)
                {
                    if (globalSettings.DiagnosticsMode)
                    {
                        foreach (var err in res.Notifications)
                        {
                            logger.LogMessage("FEDEX", err.Message, EventLogSeverity.Debug);
                        }
                    }
                    result = 0m;
                }
                else
                {
                    result = 0m;

                    var lookingForService = GetServiceType(service);
                    var matchingResponse = res.RateReplyDetails.FirstOrDefault(y => y.ServiceType == lookingForService);
                    if (matchingResponse != null)
                    {
                        if (useNegotiatedRates)
                        {
                            var matchedRate =
                                matchingResponse.RatedShipmentDetails.FirstOrDefault(
                                    y => y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_PACKAGE ||
                                         y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT);

                            if (matchedRate != null)
                            {
                                result = matchedRate.ShipmentRateDetail.TotalNetCharge.Amount;

                                if (globalSettings.DiagnosticsMode)
                                {
                                    logger.LogMessage("FEDEX SHIPMENT",
                                        "Negotiated rates were found and are currently being used.",
                                        EventLogSeverity.Information);
                                }
                            }
                            else
                            {
                                matchedRate =
                                    matchingResponse.RatedShipmentDetails.FirstOrDefault(
                                        y => y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_PACKAGE ||
                                             y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_SHIPMENT);

                                result = matchedRate.ShipmentRateDetail.TotalNetCharge.Amount;

                                if (globalSettings.DiagnosticsMode)
                                {
                                    logger.LogMessage("FEDEX SHIPMENT",
                                        "No negotiated rates were found. Public rates are being shown. You should update your account information, or uncheck the ‘Use Negotiated Rates’ checkbox.",
                                        EventLogSeverity.Information);
                                }
                                else
                                {
                                    logger.LogMessage("FEDEX SHIPMENT", "No negotiated rates were found.",
                                        EventLogSeverity.Information);
                                }
                            }
                        }
                        else
                        {
                            var matchedRate =
                                matchingResponse.RatedShipmentDetails.FirstOrDefault(
                                    y => y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_PACKAGE ||
                                         y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_LIST_SHIPMENT);

                            if (matchedRate != null)
                            {
                                result = matchedRate.ShipmentRateDetail.TotalNetCharge.Amount;

                                if (
                                    matchingResponse.RatedShipmentDetails.Any(
                                        y => y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_PACKAGE ||
                                             y.ShipmentRateDetail.RateType == ReturnedRateType.PAYOR_ACCOUNT_SHIPMENT))
                                {
                                    if (globalSettings.DiagnosticsMode)
                                    {
                                        logger.LogMessage("FEDEX SHIPMENT",
                                            "We also found negotiated rates for your account. You should consider checking the ‘Use Negotiated Rates’ checkbox.",
                                            EventLogSeverity.Information);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0m;
                logger.LogException(ex);
            }

            return result;
        }
    }
}