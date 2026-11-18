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
    /// <summary>
    /// Discovers the shipping services available to a store and supplies each service with its stored settings.
    /// </summary>
    /// <remarks>
    /// Discovery starts with services returned by <see cref="Service.FindAll"/> and then adds the
    /// configured FedEx, United States Postal Service, UPS, and UPS Freight providers. Provider
    /// instances are created on each call; credentials and provider options are read from the supplied store.
    /// </remarks>
    public class AvailableServices
    {
        /// <summary>
        /// Finds all shipping services and configures the built-in providers for the store.
        /// </summary>
        /// <param name="currentStore">The store whose shipping settings configure the providers.</param>
        /// <returns>A new list containing discovered services and the built-in provider instances.</returns>
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
                ClientId = currentStore.Settings.ShippingUpsClientId,
                ClientSecret = currentStore.Settings.ShippingUpsClientSecret,
                DefaultPackaging = (PackagingType) currentStore.Settings.ShippingUpsDefaultPackaging,
                DiagnosticsMode = currentStore.Settings.ShippingUPSDiagnostics,
                TestingMode = currentStore.Settings.ShippingUPSTestingMode,
                ForceResidential = currentStore.Settings.ShippingUpsForceResidential,
                IgnoreDimensions = currentStore.Settings.ShippingUpsSkipDimensions,
                PickUpType = (PickupType) currentStore.Settings.ShippingUpsPickupType
            };
            result.Add(new UPSService(upsglobal, Factory.CreateEventLogger()));

            
            // Load UPS Freight
            var upsFreightGlobal = new UPSFreightServiceGlobalSettings
            {
                LicenseNumber = currentStore.Settings.ShippingUpsLicense,
                Username = currentStore.Settings.ShippingUpsUsername,
                Password = currentStore.Settings.ShippingUpsPassword,
                AccountNumber = currentStore.Settings.ShippingUpsAccountNumber,
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

        /// <summary>
        /// Finds the first discovered shipping service whose identifier exactly matches the requested identifier.
        /// </summary>
        /// <param name="id">The service identifier to compare with <see cref="IShippingService.Id"/>.</param>
        /// <param name="currentStore">The store whose provider settings are used during discovery.</param>
        /// <returns>The matching service, or <see langword="null"/> when no service has that identifier.</returns>
        /// <remarks>Discovery is performed again for this call, so the returned provider is a newly configured instance.</remarks>
        public static IShippingService FindById(string id, Store currentStore)
        {
            return FindAll(currentStore).FirstOrDefault(svc => svc.Id == id);
        }
    }
}
