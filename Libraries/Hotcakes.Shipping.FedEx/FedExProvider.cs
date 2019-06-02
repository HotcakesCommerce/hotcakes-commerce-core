#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Web.Logging;

namespace Hotcakes.Shipping.FedEx
{
    [Serializable]
    public class FedExProvider : IShippingService
    {
        private readonly ILogger _Logger = new SupressLogger();
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        public FedExProvider(FedExGlobalServiceSettings globalSettings, ILogger logger)
        {
            _Logger = logger;
            GlobalSettings = globalSettings;
            Settings = new FedExServiceSettings();
        }

        public FedExGlobalServiceSettings GlobalSettings { get; set; }
        public FedExServiceSettings Settings { get; set; }

        public string Name
        {
            get { return "FedEx"; }
        }

        public string Id
        {
            get { return "43CF0D39-4E2D-4f9d-AF65-87EDF5FF84EA"; }
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

        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            _Messages.Clear();

            var result = new List<IShippingRate>();
            var hasErrors = false;

            try
            {
                var optimizedPackages = OptimizeSingleGroup(shipment);

                if (optimizedPackages.Count > 0)
                {
                    result = RatePackages(optimizedPackages);

                    if (GlobalSettings.DiagnosticsMode)
                    {
                        foreach (var package in optimizedPackages)
                        {
                            _Logger.LogMessage("FEDEX SHIPMENT",
                                string.Format("Source Address: {0}, {1}, {2}, {3}, {4} {5}",
                                    package.SourceAddress.Street, package.SourceAddress.Street2,
                                    package.SourceAddress.City, package.SourceAddress.RegionData.DisplayName,
                                    package.SourceAddress.CountryData.DisplayName, package.SourceAddress.PostalCode),
                                EventLogSeverity.Information);

                            _Logger.LogMessage("FEDEX SHIPMENT",
                                string.Format("Destination Address: {0}, {1}, {2}, {3}, {4} {5}",
                                    package.DestinationAddress.Street, package.DestinationAddress.Street2,
                                    package.DestinationAddress.City, package.DestinationAddress.RegionData.DisplayName,
                                    package.DestinationAddress.CountryData.DisplayName,
                                    package.DestinationAddress.PostalCode),
                                EventLogSeverity.Information);
                        }
                    }
                }
                else
                {
                    if (GlobalSettings.DiagnosticsMode)
                    {
                        _Logger.LogMessage("No packages found to rate for FedEx");
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

            if (GlobalSettings.DiagnosticsMode)
            {
                foreach (var rate in result)
                {
                    _Logger.LogMessage("OPTIMIZED FEDEX RATE",
                        string.Format(
                            "Service Id: {0}<br />Display Name: {1}<br />Service Codes:{2}<br />Estimated Cost:{3}",
                            rate.ServiceId, rate.DisplayName, rate.ServiceCodes, rate.EstimatedCost),
                        EventLogSeverity.Information);
                }
            }

            if (hasErrors)
            {
                result = new List<IShippingRate>();
            }
            return result;
        }

        public string GetTrackingUrl(string trackingCode)
        {
            return "http://www.fedex.com/Tracking?language=english&cntry_code=us&tracknumbers=" + trackingCode;
        }

        public List<IServiceCode> ListAllServiceCodes()
        {
            var result = new List<IServiceCode>
            {
                new ServiceCode("Priority Overnight", "1"),
                new ServiceCode("Standard Overnight", "2"),
                new ServiceCode("First Overnight", "3"),
                new ServiceCode("FedEx 2 Day", "4"),
                new ServiceCode("FedEx Express Saver", "5"),
                new ServiceCode("International Priority", "6"),
                new ServiceCode("International Economy", "7"),
                new ServiceCode("International First", "8"),
                new ServiceCode("FedEx 1 Day Freight", "9"),
                new ServiceCode("FedEx 2 Day Freight", "10"),
                new ServiceCode("FedEx 3 Day Freight", "11"),
                new ServiceCode("FedEx Ground", "12"),
                new ServiceCode("Ground Home Delivery", "13"),
                new ServiceCode("International Priority Freight", "14"),
                new ServiceCode("International Economy Freight", "15"),
                new ServiceCode("Europe First International Priority", "16")
            };
            
            return result;
        }

        private List<IShippingRate> RatePackages(List<Shipment> packages)
        {
            var result = new List<IShippingRate>();

            // Loop through packages, getting rates for each package 
            var allPackagesRated = true;

            var individualRates = new List<ShippingRate>();

            foreach (IShipment s in packages)
            {
                var singlePackageRate = new ShippingRate();
                singlePackageRate = RateService.RatePackage(GlobalSettings, _Logger, Settings, s);

                if (singlePackageRate == null)
                {
                    allPackagesRated = false;
                    break;
                }
                if (singlePackageRate.EstimatedCost < 0)
                {
                    allPackagesRated = false;
                    break;
                }
                individualRates.Add(singlePackageRate);
            }

            // we are done with all packages for this shipping type
            if (allPackagesRated)
            {
                var total = individualRates.Sum(rate => rate.EstimatedCost);
                if (total <= 0m) return result;
                if (individualRates.Count > 0)
                {
                    result.Add(
                        new ShippingRate
                        {
                            EstimatedCost = total,
                            DisplayName = "FedEx:" + FindNameByServiceCode(Settings.ServiceCode),
                            ServiceCodes = Settings.ServiceCode.ToString(),
                            ServiceId = Id
                        });
                }
            }

            return result;
        }

        protected bool IsOversized(IShippable prod)
        {
            var IsOversize = false;
            var girth = (double) (prod.BoxLength + 2*prod.BoxHeight + 2*prod.BoxWidth);
            if (girth > 130)
            {
                //this is an oversize product
                IsOversize = true;
            }
            else
            {
                if (prod.BoxHeight > 108)
                {
                    IsOversize = true;
                }
                else if (prod.BoxLength > 108)
                {
                    IsOversize = true;
                }
                else if (prod.BoxWidth > 108)
                {
                    IsOversize = true;
                }
            }

            return IsOversize;
        }

        public string FindNameByServiceCode(int serviceCode)
        {
            var result = "FedEx";
            var codes = ListAllServiceCodes();
            var temp = codes.FirstOrDefault(y => y.Code == serviceCode.ToString());
            if (temp != null)
            {
                result = temp.DisplayName;
            }
            return result;
        }

        public List<Shipment> OptimizeSingleGroup(IShipment shipment)
        {
            decimal MAXWEIGHT = 70;

            // Set Max Weight for Ground Services
            if (Settings.ServiceCode == (int) ServiceType.FEDEXGROUND ||
                Settings.ServiceCode == (int) ServiceType.GROUNDHOMEDELIVERY)
            {
                MAXWEIGHT = 150;
            }
            
            var result = new List<Shipment>();

            var itemsToSplit = new List<IShippable>();

            foreach (var item in shipment.Items)
            {
                if (IsOversized(item))
                {
                    var s1 = Shipment.CloneAddressesFromInterface(shipment);
                    s1.Items.Add(item.CloneShippable());
                    result.Add(s1);
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
                        var s2 = Shipment.CloneAddressesFromInterface(shipment);
                        s2.Items.Add(tempPackage.CloneShippable());
                        result.Add(s2);

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

                                var s3 = Shipment.CloneAddressesFromInterface(shipment);
                                s3.Items.Add(newP.CloneShippable());
                                result.Add(s3);
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
                                var s4 = Shipment.CloneAddressesFromInterface(shipment);
                                s4.Items.Add(newP.CloneShippable());
                                result.Add(s4);
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
                var s5 = Shipment.CloneAddressesFromInterface(shipment);
                s5.Items.Add(tempPackage.CloneShippable());
                result.Add(s5);

                tempPackage = new Shippable();
            }

            return result;
        }
    }
}