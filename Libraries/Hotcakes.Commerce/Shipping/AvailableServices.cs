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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Shipping;
using Hotcakes.Shipping.FedEx;
using Hotcakes.Shipping.Ups;
using Hotcakes.Shipping.USPostal;
using Hotcakes.Shipping.UpsFreight;
using System;

namespace Hotcakes.Commerce.Shipping
{
    public class AvailableServices
    {
        public static List<IShippingService> FindAll(Store currentStore)
        {
            var result = Service.FindAll();

            // FedEx
            var fedexGlobal = new FedExGlobalServiceSettings
            {
                UserKey = currentStore.Settings.ShippingFedExKey,
                UserPassword = currentStore.Settings.ShippingFedExPassword,
                AccountNumber = currentStore.Settings.ShippingFedExAccountNumber,
                MeterNumber = currentStore.Settings.ShippingFedExMeterNumber,
                DefaultDropOffType = (DropOffType) currentStore.Settings.ShippingFedExDropOffType,
                DefaultPackaging = (PackageType) currentStore.Settings.ShippingFedExDefaultPackaging,
                DiagnosticsMode = currentStore.Settings.ShippingFedExDiagnostics,
                ForceResidentialRates = currentStore.Settings.ShippingFedExForceResidentialRates,
                UseDevelopmentServiceUrl = currentStore.Settings.ShippingFedExUseDevelopmentServiceUrl
            };
            result.Add(new FedExProvider(fedexGlobal, Factory.CreateEventLogger()));

            // Load US Postal
            var uspostalGlobal = new USPostalServiceGlobalSettings
            {
                UserId = currentStore.Settings.ShippingUSPostalUserId,
                DiagnosticsMode = currentStore.Settings.ShippingUSPostalDiagnostics
            };
            result.Add(new DomesticProvider(uspostalGlobal, Factory.CreateEventLogger()));
            result.Add(new InternationalProvider(uspostalGlobal, Factory.CreateEventLogger()));

            // Load UPS
            var upsglobal = new UPSServiceGlobalSettings
            {
                AccountNumber = currentStore.Settings.ShippingUpsAccountNumber,
                LicenseNumber = currentStore.Settings.ShippingUpsLicense,
                Username = currentStore.Settings.ShippingUpsUsername,
                Password = currentStore.Settings.ShippingUpsPassword,
                DefaultPackaging = (PackagingType) currentStore.Settings.ShippingUpsDefaultPackaging,
                DiagnosticsMode = currentStore.Settings.ShippingUPSDiagnostics,
                ForceResidential = currentStore.Settings.ShippingUpsForceResidential,
                IgnoreDimensions = currentStore.Settings.ShippingUpsSkipDimensions,
                PickUpType = (PickupType) currentStore.Settings.ShippingUpsPickupType
            };
            result.Add(new UPSService(upsglobal, Factory.CreateEventLogger()));

            
            // Load UPS Freight
            var upsFreightGlobal = new UPSFreightServiceGlobalSettings
            {
                AccountNumber = currentStore.Settings.ShippingUpsAccountNumber,
                LicenseNumber = currentStore.Settings.ShippingUpsLicense,
                Username = currentStore.Settings.ShippingUpsUsername,
                Password = currentStore.Settings.ShippingUpsPassword,
                DefaultPackaging = (Hotcakes.Shipping.UpsFreight.PackingTypes)currentStore.Settings.ShippingUpsFreightDefaultPackaging,
                DiagnosticsMode = currentStore.Settings.ShippingUPSFreightDiagnostics,
                ForceResidential = currentStore.Settings.ShippingUpsFreightForceResidential,
                IgnoreDimensions = currentStore.Settings.ShippingUpsFreightSkipDimensions,
                BillingOption = (Hotcakes.Shipping.UpsFreight.BillingOption)currentStore.Settings.ShippingUpsFreightBillingOption,
                HandleOneUnitType = (currentStore.Settings.ShippingUpsFreightHandleOneUnitType != string.Empty ? (HandlineOneUnitType)Enum.Parse(typeof(HandlineOneUnitType),currentStore.Settings.ShippingUpsFreightHandleOneUnitType) : HandlineOneUnitType.CBY),
                FreightClass = currentStore.Settings.ShippingUpsFreightFreightClass
            };
            result.Add(new UPSFreightService(upsFreightGlobal, Factory.CreateEventLogger()));
            return result;
        }

        public static IShippingService FindById(string id, Store currentStore)
        {
            return FindAll(currentStore).FirstOrDefault(svc => svc.Id == id);
        }
    }
}