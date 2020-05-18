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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public class UPSService : IShippingService
    {
        public const string UPSLIVESERVER = @"https://www.ups.com/ups.app/xml/";
        private readonly List<IServiceCode> _Codes = new List<IServiceCode>();

        private readonly ILogger _Logger = new SupressLogger();

        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        public UPSService(UPSServiceGlobalSettings globalSettings, ILogger logger)
        {
            _Logger = logger;
            GlobalSettings = globalSettings;
            Settings = new UPSServiceSettings();
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

                var sURL = string.Concat(UPSLIVESERVER, "Rate");

                // Build XML
                var settings = new UpsSettings
                {
                    UserID = GlobalSettings.Username,
                    Password = GlobalSettings.Password,
                    ServerUrl = UPSLIVESERVER,
                    License = GlobalSettings.LicenseNumber
                };

                var sXML = string.Empty;

                sXML = XmlTools.BuildAccessKey(settings);
                sXML += "\n";

                sXML += BuildUPSRateRequestForShipment(shipment, settings);

                var sResponse = string.Empty;
                sResponse = XmlTools.ReadHtmlPage_POST(sURL, sXML);

                if (GlobalSettings.DiagnosticsMode)
                {
                    _Logger.LogMessage(string.Format("UPS XML Response: {0}", sResponse));
                }

                XmlDocument xDoc;
                XmlNodeList NodeList;
                var sStatusCode = "-1";

                try
                {
                    xDoc = new XmlDocument();
                    xDoc.LoadXml(sResponse);

                    if (xDoc.DocumentElement.Name == "RatingServiceSelectionResponse")
                    {
                        XmlNode n;
                        var i = 0;
                        XmlNode nTag;

                        NodeList = xDoc.GetElementsByTagName("RatingServiceSelectionResponse");
                        n = NodeList.Item(0);
                        for (i = 0; i <= n.ChildNodes.Count - 1; i++)
                        {
                            nTag = n.ChildNodes.Item(i);
                            switch (nTag.Name)
                            {
                                case "Response":
                                    var iRes = 0;
                                    XmlNode nRes;
                                    for (iRes = 0; iRes <= nTag.ChildNodes.Count - 1; iRes++)
                                    {
                                        nRes = nTag.ChildNodes[iRes];
                                        switch (nRes.Name)
                                        {
                                            case "ResponseStatusCode":
                                                sStatusCode = nRes.FirstChild.Value;
                                                break;
                                            case "ResponseStatusDescription":
                                                // Not Used
                                                break;
                                            case "Error":
                                                var iErr = 0;
                                                XmlNode nErr;
                                                for (iErr = 0; iErr <= nRes.ChildNodes.Count - 1; iErr++)
                                                {
                                                    nErr = nRes.ChildNodes[iErr];
                                                    switch (nErr.Name)
                                                    {
                                                        case "ErrorCode":
                                                            sErrorCode = nErr.FirstChild.Value;
                                                            break;
                                                        case "ErrorDescription":
                                                            sErrorMessage = nErr.FirstChild.Value;
                                                            break;
                                                        case "ErrorSeverity":
                                                            // Not Used
                                                            break;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case "RatedShipment":

                                    var iRated = 0;
                                    XmlNode nRated;

                                    var sPostage = string.Empty;
                                    var sCurrencyCode = string.Empty;
                                    var sCode = string.Empty;
                                    var sDescription = string.Empty;

                                    for (iRated = 0; iRated <= nTag.ChildNodes.Count - 1; iRated++)
                                    {
                                        nRated = nTag.ChildNodes[iRated];
                                        switch (nRated.Name)
                                        {
                                            case "Service":
                                                var iServices = 0;
                                                XmlNode nServices;
                                                for (iServices = 0;
                                                    iServices <= nRated.ChildNodes.Count - 1;
                                                    iServices++)
                                                {
                                                    nServices = nRated.ChildNodes[iServices];
                                                    switch (nServices.Name)
                                                    {
                                                        case "Code":
                                                            sCode = nServices.FirstChild.Value;
                                                            sDescription = DecodeUpsServiceCode(sCode);
                                                            break;
                                                        case "Description":
                                                            sDescription = nServices.FirstChild.Value;
                                                            break;
                                                    }
                                                }
                                                break;
                                            case "TotalCharges":
                                                var iCharges = 0;
                                                XmlNode nCharges;
                                                for (iCharges = 0; iCharges <= nRated.ChildNodes.Count - 1; iCharges++)
                                                {
                                                    nCharges = nRated.ChildNodes[iCharges];
                                                    switch (nCharges.Name)
                                                    {
                                                        case "MonetaryValue":
                                                            sPostage = nCharges.FirstChild.Value;
                                                            break;
                                                        case "CurrencyCode":
                                                            sCurrencyCode = nCharges.FirstChild.Value;
                                                            break;
                                                    }
                                                }
                                                break;
                                        }
                                    }

                                    var dRate = -1m;

                                    if (sPostage.Length > 0)
                                    {
                                        dRate = decimal.Parse(sPostage, NumberStyles.Currency,
                                            CultureInfo.InvariantCulture);
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
                                        var r = new ShippingRate
                                        {
                                            DisplayName = sDescription,
                                            EstimatedCost = dRate,
                                            ServiceCodes = sCode,
                                            ServiceId = Id
                                        };
                                        rates.Add(r);
                                    }

                                    if (GlobalSettings.DiagnosticsMode)
                                    {
                                        var msg = new ShippingServiceMessage();

                                        msg.SetDiagnostics("UPS Rates Found",
                                            string.Concat("StatusCode = ", sStatusCode, ", Postage = ", sPostage,
                                                ", Errors = ", sErrorMessage, ", Rate = ", dRate));

                                        _Messages.Add(msg);
                                    }

                                    break;
                            }
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        sErrorMessage = "Couldn't find valid XML response from server.";
                    }
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

        private string BuildUPSRateRequestForShipment(IShipment shipment, UpsSettings settings)
        {
            var sXML = string.Empty;
            var strWriter = new StringWriter();
            var xw = new XmlTextWriter(strWriter);

            try
            {
                xw.Formatting = Formatting.Indented;
                xw.Indentation = 3;

                //--------------------------------------------            
                // Agreement Request
                xw.WriteStartElement("RatingServiceSelectionRequest");

                //--------------------------------------------
                // Request
                xw.WriteStartElement("Request");
                //--------------------------------------------
                // TransactionReference
                xw.WriteStartElement("TransactionReference");
                xw.WriteElementString("CustomerContext", "Rate Request");
                xw.WriteElementString("XpciVersion", "1.0001");
                xw.WriteEndElement();
                // End TransactionReference
                //--------------------------------------------
                xw.WriteElementString("RequestOption", "Shop");
                xw.WriteEndElement();
                // End Request
                //--------------------------------------------

                //--------------------------------------------
                // Pickup Type
                if (GlobalSettings.PickUpType != PickupType.Unknown)
                {
                    var pickupCode = ((int) GlobalSettings.PickUpType).ToString();

                    if (pickupCode.Trim().Length < 2)
                    {
                        pickupCode = "0" + pickupCode;
                    }

                    xw.WriteStartElement("PickupType");
                    xw.WriteElementString("Code", pickupCode);
                    xw.WriteEndElement();
                }
                // End Pickup Type
                //--------------------------------------------

                //--------------------------------------------
                // Shipment
                xw.WriteStartElement("Shipment");

                // Shipper
                xw.WriteStartElement("Shipper");
                xw.WriteStartElement("Address");

                //Use City name for countries that don't have postal codes
                if (shipment.SourceAddress.PostalCode.Trim().Length > 0)
                {
                    xw.WriteElementString("PostalCode",
                        XmlTools.TrimToLength(shipment.SourceAddress.PostalCode.Trim(), 9));
                }
                else
                {
                    xw.WriteElementString("City", XmlTools.TrimToLength(shipment.SourceAddress.City.Trim(), 30));
                }

                if (shipment.SourceAddress.RegionData != null)
                {
                    xw.WriteElementString("StateProvinceCode", shipment.SourceAddress.RegionData.Abbreviation);
                }

                xw.WriteElementString("CountryCode", shipment.SourceAddress.CountryData.IsoCode);

                xw.WriteElementString("ShipperNumber", settings.License);

                xw.WriteEndElement();
                xw.WriteEndElement();

                // Ship To
                xw.WriteStartElement("ShipTo");
                xw.WriteStartElement("Address");

                if (shipment.DestinationAddress.PostalCode.Length > 0)
                {
                    xw.WriteElementString("PostalCode", shipment.DestinationAddress.PostalCode);
                }
                else
                {
                    if (shipment.DestinationAddress.City.Length > 0)
                    {
                        xw.WriteElementString("City", shipment.DestinationAddress.City);
                    }
                }

                if (shipment.DestinationAddress.RegionData != null)
                {
                    xw.WriteElementString("StateProvinceCode", shipment.DestinationAddress.RegionData.Abbreviation);
                }

                if (shipment.DestinationAddress.CountryData.Bvin.Length > 0)
                {
                    xw.WriteElementString("CountryCode", shipment.DestinationAddress.CountryData.IsoCode);
                }

                if (GlobalSettings.ForceResidential)
                {
                    xw.WriteElementString("ResidentialAddress", string.Empty);
                }

                xw.WriteEndElement();
                xw.WriteEndElement();

                var ignoreDimensions = GlobalSettings.IgnoreDimensions;

                // Optimize Packages for Weight
                var optimizedPackages = OptimizeSingleGroup(shipment);

                foreach (var p in optimizedPackages)
                {
                    WriteSingleUPSPackage(ref xw, p, ignoreDimensions);
                }

                if (Settings.NegotiatedRates)
                {
                    xw.WriteStartElement("RateInformation");
                    xw.WriteElementString("NegotiatedRatesIndicator", string.Empty);
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                // End Shipment
                //--------------------------------------------

                xw.WriteEndElement();
                // End Agreement Request
                //--------------------------------------------

                xw.Flush();
                xw.Close();
            }
            catch (Exception ex)
            {
                _Logger.LogException(ex);
            }

            sXML = strWriter.GetStringBuilder().ToString();

            return sXML;
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

        private void WriteSingleUPSPackage(ref XmlTextWriter xw, IShippable pak, bool ignoreDimensions)
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
            xw.WriteStartElement("Package");

            xw.WriteStartElement("PackagingType");

            var packageType = "02";

            if (GlobalSettings.DefaultPackaging != (int) PackagingType.Unknown)
            {
                packageType = ((int) GlobalSettings.DefaultPackaging).ToString();
                if (packageType.Trim().Length < 2)
                {
                    packageType = "0" + packageType;
                }
            }

            xw.WriteElementString("Code", packageType);
            xw.WriteElementString("Description", "Package");
            xw.WriteEndElement();

            //Dimensions can be skipped in latest UPS specs
            if (!ignoreDimensions)
            {
                if (dLength > 0 | dHeight > 0 | dwidth > 0)
                {
                    xw.WriteStartElement("Dimensions");
                    xw.WriteStartElement("UnitOfMeasure");
                    xw.WriteElementString("Code", "IN");
                    xw.WriteEndElement();
                    xw.WriteElementString("Length", Math.Round(dLength, 2).ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("Width", Math.Round(dwidth, 2).ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("Height", Math.Round(dHeight, 2).ToString(CultureInfo.InvariantCulture));
                    xw.WriteEndElement();
                }
            }

            if (pak.BoxWeight > 0)
            {
                xw.WriteStartElement("PackageWeight");
                xw.WriteStartElement("UnitOfMeasure");

                xw.WriteElementString("Code", pak.BoxWeightType == WeightType.Pounds ? "LBS" : "KGS");

                xw.WriteEndElement();
                xw.WriteElementString("Weight", Math.Round(pak.BoxWeight, 1).ToString(CultureInfo.InvariantCulture));
                xw.WriteEndElement();
            }
            else
            {
                xw.WriteStartElement("PackageWeight");
                xw.WriteStartElement("UnitOfMeasure");

                if (pak.BoxWeightType == WeightType.Pounds)
                {
                    xw.WriteElementString("Code", "LBS");
                }
                else
                {
                    xw.WriteElementString("Code", "KGS");
                }

                xw.WriteEndElement();
                xw.WriteElementString("Weight", Math.Round(0.1, 1).ToString(CultureInfo.InvariantCulture));
                xw.WriteEndElement();
            }

            if (ignoreDimensions == false)
            {
                // Oversize Checks
                var oversizeCheck = dGirth + dLength;
                if (oversizeCheck > 84)
                {
                    if (oversizeCheck < 108 & pak.BoxWeight < 30)
                    {
                        xw.WriteElementString("OversizePackage", "1");
                    }
                    else
                    {
                        if (pak.BoxWeight < 70)
                        {
                            xw.WriteElementString("OversizePackage", "2");
                        }
                        else
                        {
                            xw.WriteElementString("OversizePackage", "0");
                        }
                    }
                }
            }

            xw.WriteEndElement();
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