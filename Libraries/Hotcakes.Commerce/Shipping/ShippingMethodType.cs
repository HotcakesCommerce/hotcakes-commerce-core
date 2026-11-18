#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>Identifies built-in shipping calculation types and provider service codes.</summary>
    /// <remarks>Numeric values are persisted and exchanged with shipping providers; do not renumber or rename members.</remarks>
    public enum ShippingMethodType
    {
        /// <summary>No shipping method type.</summary>
        None = 0,
        /// <summary>Calculate shipping by weight.</summary>
        ByWeight = 1000,
        /// <summary>Calculate shipping by price.</summary>
        ByPrice = 1100,
        /// <summary>Calculate shipping by item count.</summary>
        ByItemCount = 1200,
        /// <summary>Calculate shipping per item.</summary>
        PerItem = 1300,

        /// <summary>FedEx Standard Overnight.</summary>
        FedExStandardOvernight = 2100,
        /// <summary>FedEx Priority Overnight.</summary>
        FedExPriorityOvernight = 2101,
        /// <summary>FedEx First Overnight.</summary>
        FedExFirstOvernight = 2102,
        /// <summary>FedEx 2Day.</summary>
        FedEx2Day = 2103,
        /// <summary>FedEx Express Saver.</summary>
        FedExExpressSaver = 2104,
        /// <summary>FedEx International Priority.</summary>
        FedExInternationalPriority = 2105,
        /// <summary>FedEx International Economy.</summary>
        FedExInternationalEconomy = 2106,
        /// <summary>FedEx International First.</summary>
        FedExInternationalFirst = 2107,
        /// <summary>FedEx 1Day Freight.</summary>
        FedEx1DayFreight = 2108,
        /// <summary>FedEx 2Day Freight.</summary>
        FedEx2DayFreight = 2109,
        /// <summary>FedEx 3Day Freight; the misspelled member name is retained for compatibility.</summary>
        FedEx3DayFrieght = 2110,
        /// <summary>FedEx Ground.</summary>
        FedExGround = 2111,
        /// <summary>FedEx Ground Home Delivery.</summary>
        FedExGroundHomeDelivery = 2112,
        /// <summary>FedEx International Priority Freight.</summary>
        FedExInternationalPriorityFreight = 2113,
        /// <summary>FedEx International Economy Freight.</summary>
        FedExInternationalEconomyFreight = 2114,

        /// <summary>UPS Standard.</summary>
        UPSStandard = 2200,
        /// <summary>UPS Next Day Air.</summary>
        UPSNextDayAir = 2201,
        /// <summary>UPS Express Canada.</summary>
        UPSExpressCanada = 2202,
        /// <summary>UPS Second Day Air.</summary>
        UPSSecondDayAir = 2203,
        /// <summary>UPS Expedited Canada.</summary>
        UPSExpeditedCanada = 2204,
        /// <summary>UPS Worldwide Express.</summary>
        UPSWorldwideExpress = 2205,
        /// <summary>UPS Express Europe.</summary>
        UPSExpressEurope = 2206,
        /// <summary>UPS Express Mexico.</summary>
        UPSExpressMexico = 2207,
        /// <summary>UPS Worldwide Expedited.</summary>
        UPSWorldwideExpedited = 2208,
        /// <summary>UPS Ground.</summary>
        UPSGround = 2209,
        /// <summary>UPS 3 Day Select.</summary>
        UPS3DaySelect = 2210,
        /// <summary>UPS Next Day Air Saver.</summary>
        UPSNextDayAirSaver = 2211,
        /// <summary>UPS Express Saver Canada.</summary>
        UPSExpressSaverCanada = 2212,
        /// <summary>UPS Next Day Air Early AM.</summary>
        UPSNextDayAirEarlyAM = 2213,
        /// <summary>UPS Worldwide Express Plus.</summary>
        UPSWorldwideExpressPlus = 2214,
        /// <summary>UPS Express Plus.</summary>
        UPSExpressPlus = 2215,
        /// <summary>UPS Second Day Air AM.</summary>
        UPSSecondDayAirAM = 2216,
        /// <summary>UPS Express Saver US.</summary>
        UPSExpressSaverUS = 2217,
        /// <summary>UPS Express Saver Europe.</summary>
        UPSExpressSaverEurope = 2218,

        /// <summary>US Postal Service Express.</summary>
        USPostalServiceExpress = 2302,
        /// <summary>US Postal Service Priority.</summary>
        USPostalServicePriority = 2301,
        /// <summary>US Postal Service Parcel.</summary>
        USPostalServiceParcel = 2300,
        /// <summary>US Postal Service Bound Printed Material.</summary>
        USPostalServiceBoundPrintedMaterial = 2303,
        /// <summary>US Postal Service Library.</summary>
        USPostalServiceLibrary = 2304,
        /// <summary>US Postal Service Media.</summary>
        USPostalServiceMedia = 2305,
        /// <summary>US Postal Service International.</summary>
        USPostalServiceInternational = 2306,

        //2004.6
        /// <summary>US Postal Service First Class.</summary>
        USPostalServiceFirstClass = 2400,
        /// <summary>US Postal Service Global Express.</summary>
        USPostalServiceGlobalExpress = 2401,
        /// <summary>US Postal Service Global Express Guaranteed.</summary>
        USPostalServiceGlobalExpressGuaranteed = 2402,
        /// <summary>US Postal Service Global Priority Mail.</summary>
        USPostalServiceGlobalPriorityMail = 2403,
        //USPostalServiceAllDomestic = 2404

        /// <summary>DHL service.</summary>
        DHL = 3000,
        /// <summary>Canada Post service.</summary>
        CanadaPost = 4000,
        /// <summary>Indicates that no shipping methods are available.</summary>
        NoMethodsAvailable = 99999
    }
}
