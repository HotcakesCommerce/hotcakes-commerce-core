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

namespace Hotcakes.Commerce.Shipping
{
    public enum ShippingMethodType
    {
        None = 0,
        ByWeight = 1000,
        ByPrice = 1100,
        ByItemCount = 1200,
        PerItem = 1300,

        FedExStandardOvernight = 2100,
        FedExPriorityOvernight = 2101,
        FedExFirstOvernight = 2102,
        FedEx2Day = 2103,
        FedExExpressSaver = 2104,
        FedExInternationalPriority = 2105,
        FedExInternationalEconomy = 2106,
        FedExInternationalFirst = 2107,
        FedEx1DayFreight = 2108,
        FedEx2DayFreight = 2109,
        FedEx3DayFrieght = 2110,
        FedExGround = 2111,
        FedExGroundHomeDelivery = 2112,
        FedExInternationalPriorityFreight = 2113,
        FedExInternationalEconomyFreight = 2114,

        UPSStandard = 2200,
        UPSNextDayAir = 2201,
        UPSExpressCanada = 2202,
        UPSSecondDayAir = 2203,
        UPSExpeditedCanada = 2204,
        UPSWorldwideExpress = 2205,
        UPSExpressEurope = 2206,
        UPSExpressMexico = 2207,
        UPSWorldwideExpedited = 2208,
        UPSGround = 2209,
        UPS3DaySelect = 2210,
        UPSNextDayAirSaver = 2211,
        UPSExpressSaverCanada = 2212,
        UPSNextDayAirEarlyAM = 2213,
        UPSWorldwideExpressPlus = 2214,
        UPSExpressPlus = 2215,
        UPSSecondDayAirAM = 2216,
        UPSExpressSaverUS = 2217,
        UPSExpressSaverEurope = 2218,

        USPostalServiceExpress = 2302,
        USPostalServicePriority = 2301,
        USPostalServiceParcel = 2300,
        USPostalServiceBoundPrintedMaterial = 2303,
        USPostalServiceLibrary = 2304,
        USPostalServiceMedia = 2305,
        USPostalServiceInternational = 2306,

        //2004.6
        USPostalServiceFirstClass = 2400,
        USPostalServiceGlobalExpress = 2401,
        USPostalServiceGlobalExpressGuaranteed = 2402,
        USPostalServiceGlobalPriorityMail = 2403,
        //USPostalServiceAllDomestic = 2404

        DHL = 3000,
        CanadaPost = 4000,
        NoMethodsAvailable = 99999
    }
}