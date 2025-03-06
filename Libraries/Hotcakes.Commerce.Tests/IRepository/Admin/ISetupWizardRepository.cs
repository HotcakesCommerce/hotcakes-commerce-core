#region License

// Distributed under the MIT License
// ============================================================
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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Licensing.Common.DataContracts;
using Hotcakes.Shipping;
using Hotcakes.Shipping.FedEx;
using Hotcakes.Shipping.Services;
using Hotcakes.Shipping.USPostal;
using Hotcakes.Shipping.Ups;
using Address = Hotcakes.Commerce.Contacts.Address;

namespace Hotcakes.Commerce.Tests.IRepository.Admin
{
    /// <summary>
    /// IRepository
    /// </summary>
    public interface IXmlSetupWizardRepository : IDisposable
    {
        /// <summary>
        /// Get Store Data
        /// </summary>
        /// <returns></returns>
        Store GetStore();

        #region Step0
        /// <summary>
        /// Gets the store URL.
        /// </summary>
        /// <returns></returns>
        StoreSettingsUrls GetStoreUrl();

        /// <summary>
        /// Shippings the zones.
        /// </summary>
        /// <returns></returns>
        List<Zone> ShippingZones();
        #endregion

        #region Step1
        /// <summary>
        /// Gets the store information.
        /// </summary>
        /// <returns></returns>
        StoreSettings GetStoreInfo();

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <returns></returns>
        StoreSettingsUrls GetPages();

        /// <summary>
        /// Gets the page tab count.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, int> GetPageTabCount();

        /// <summary>
        /// Gets the country count.
        /// </summary>
        /// <returns></returns>
        int GetCountryCount();

        /// <summary>
        /// Gets the region count.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetRegionCount();

        /// <summary>
        /// Gets the culture count.
        /// </summary>
        /// <returns></returns>
        int GetCultureCount();

        /// <summary>
        /// Gets the contact address.
        /// </summary>
        /// <returns></returns>
        Address GetContactAddress();

        /// <summary>
        /// Gets the currency culture code.
        /// </summary>
        /// <returns></returns>
        string GetCurrencyCultureCode();

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        /// <returns></returns>
        string GetTimeZoneId();
        #endregion

        #region Step2
        /// <summary>
        /// Gets the payment method count.
        /// </summary>
        /// <returns></returns>
        int GetPaymentMethodCount();

        /// <summary>
        /// Gets the total payment method count.
        /// </summary>
        /// <returns></returns>
        int GetTotalPaymentMethodCount();

        /// <summary>
        /// Gets the payment methods.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetPaymentMethods();

        /// <summary>
        /// Gets the credit card information.
        /// </summary>
        /// <returns></returns>
        StoreSettings GetCreditCardInfo();

        /// <summary>
        /// Gets the total gateway count.
        /// </summary>
        /// <returns></returns>
        int GetTotalGatewayCount();

        /// <summary>
        /// Gets the pay pal information.
        /// </summary>
        /// <returns></returns>
        StoreSettingsPayPal GetPayPalInfo();
        #endregion

        #region Step3
        /// <summary>
        /// Gets the shipping method count.
        /// </summary>
        /// <returns></returns>
        int GetShippingMethodCount();

        /// <summary>
        /// Gets the total shipping method count.
        /// </summary>
        /// <returns></returns>
        int GetTotalShippingMethodCount();

        /// <summary>
        /// Gets the handling setting.
        /// </summary>
        /// <returns></returns>
        StoreSettings GetHandlingSetting();

        /// <summary>
        /// Gets the shipping zone count.
        /// </summary>
        /// <returns></returns>
        int GetShippingZoneCount();

        /// <summary>
        /// Gets the name of the add shipping zone.
        /// </summary>
        /// <returns></returns>
        string GetAddShippingZoneName();

        /// <summary>
        /// Gets the edit shipping zone.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetEditShippingZone();

        /// <summary>
        /// Gets the delete shipping zone.
        /// </summary>
        /// <returns></returns>
        string GetDeleteShippingZone();

        /// <summary>
        /// Gets the add zone area.
        /// </summary>
        /// <returns></returns>
        Zone GetAddZoneArea();

        /// <summary>
        /// Gets the delete zone area.
        /// </summary>
        /// <returns></returns>
        Zone GetDeleteZoneArea();

        /// <summary>
        /// Gets the delete shipping method.
        /// </summary>
        /// <returns></returns>
        string GetDeleteShippingMethod();

        #region Shipping Methods Add/Edit Functions
        /// <summary>
        /// Gets the add flat rate per item.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        FlatRatePerItemSettings GetAddSMInfo_FlatRatePerItem(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit flat rate per item.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        FlatRatePerItemSettings GetEditSMInfo_FlatRatePerItem(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add flat rate per order.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        FlatRatePerOrderSettings GetAddSMInfo_FlatRatePerOrder(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit flat rate per order.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        FlatRatePerOrderSettings GetEditSMInfo_FlatRatePerOrder(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add rate per weight formula.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        RatePerWeightFormulaSettings GetAddSMInfo_RatePerWeightFormula(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit rate per weight formula.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        RatePerWeightFormulaSettings GetEditSMInfo_RatePerWeightFormula(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add rate table.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        RateTableSettings GetAddSMInfo_RateTable(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit rate table.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        RateTableSettings GetEditSMInfo_RateTable(ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add ups.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        UPSServiceSettings GetAddSMInfo_UPS(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit ups.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        UPSServiceSettings GetEditSMInfo_UPS(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add ups internation.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        USPostalServiceSettings GetAddSMInfo_UPS_Internation(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit ups internation.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        USPostalServiceSettings GetEditSMInfo_UPS_Internation(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add ups domestic.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        USPostalServiceSettings GetAddSMInfo_UPS_Domestic(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit ups domestic.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        USPostalServiceSettings GetEditSMInfo_UPS_Domestic(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the add  fedex.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        FedExServiceSettings GetAddSMInfo_FedEx(ref Store store, ref ShippingMethod spMethod);

        /// <summary>
        /// Gets the edit fedex.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        FedExServiceSettings GetEditSMInfo_FedEx(ref Store store, ref ShippingMethod spMethod);
        #endregion

        #region Shipping Methods Test Rates Functions
        /// <summary>
        /// Gets the test rate fedex shipment.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns></returns>
        Shipment GetTestRate_FedExShipment(Store store);

        /// <summary>
        /// Gets the test rate fedex shippable information.
        /// </summary>
        /// <returns></returns>
        Shippable GetTestRate_FedExShippableInfo();

        /// <summary>
        /// Gets the test rate fedex settings.
        /// </summary>
        /// <returns></returns>
        FedExGlobalServiceSettings GetTestRate_FedExSettings();

        /// <summary>
        /// Gets the test rate fedex service code.
        /// </summary>
        /// <returns></returns>
        int GetTestRate_FedExServiceCode();

        /// <summary>
        /// Gets the test rate ups domestic shipment.
        /// </summary>
        /// <returns></returns>
        Shipment GetTestRate_UPS_DomesticShipment();

        /// <summary>
        /// Gets the test rate ups domestic shippable information.
        /// </summary>
        /// <returns></returns>
        Shippable GetTestRate_UPS_DomesticShippableInfo();

        /// <summary>
        /// Gets the test rate ups domestic service setting.
        /// </summary>
        /// <returns></returns>
        USPostalServiceSettings GetTestRate_UPS_DomesticServiceSetting();

        /// <summary>
        /// Gets the test rate ups internation shipment.
        /// </summary>
        /// <returns></returns>
        Shipment GetTestRate_UPS_InternationShipment();

        /// <summary>
        /// Gets the test rate ups internation shippable information.
        /// </summary>
        /// <returns></returns>
        Shippable GetTestRate_UPS_InternationShippableInfo();

        /// <summary>
        /// Gets the test rate ups internation service setting.
        /// </summary>
        /// <returns></returns>
        USPostalServiceSettings GetTestRate_UPS_InternationServiceSetting();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetUpsServiceCodeCount();
        #endregion

        #region Step4
        /// <summary>
        /// Gets the store tax schedule count.
        /// </summary>
        /// <returns></returns>
        int GetStoreTaxScheduleCount();

        /// <summary>
        /// Gets the delete tax schedule.
        /// </summary>
        /// <returns></returns>
        string GetDeleteTaxSchedule();

        /// <summary>
        /// Ges the tax schedule apply vat rules.
        /// </summary>
        /// <returns></returns>
        bool GeTaxScheduleApplyVatRules();

        /// <summary>
        /// Gets the edit tax schedule.
        /// </summary>
        /// <returns></returns>
        TaxSchedule GetEditTaxSchedule();

        /// <summary>
        /// Gets the add tax schedule.
        /// </summary>
        /// <returns></returns>
        TaxSchedule GetAddTaxSchedule();

        /// <summary>
        /// Gets the add tax.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, Tax> GetAddTax();

        /// <summary>
        /// Gets the delete tax.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, Tax> GetDeleteTax();
        #endregion

        #region Step5
        /// <summary>
        /// Gets the login information.
        /// </summary>
        /// <returns></returns>
        CredentialsDTO GetLoginInfo();

        /// <summary>
        /// Gets the register information.
        /// </summary>
        /// <returns></returns>
        UserDTO GetRegisterInfo();

        /// <summary>
        /// Gets the license order count.
        /// </summary>
        /// <returns></returns>
        int GetLicenseOrderCount();

        /// <summary>
        /// Gets the license login information.
        /// </summary>
        /// <returns></returns>
        CredentialsDTO GetLicenseLoginInfo();

        /// <summary>
        /// Gets the license order identifier.
        /// </summary>
        /// <returns></returns>
        int GetLicenseOrderId();

        /// <summary>
        /// Gets the order lookup query.
        /// </summary>
        /// <returns></returns>
        OrderQueryDTO GetOrderLookupQuery();

        /// <summary>
        /// Gets the order lookup MSG.
        /// </summary>
        /// <returns></returns>
        string GetOrderLookupMsg();

        /// <summary>
        /// Gets the order store.
        /// </summary>
        /// <returns></returns>
        int GetOrderStore();
        #endregion
    }
}
