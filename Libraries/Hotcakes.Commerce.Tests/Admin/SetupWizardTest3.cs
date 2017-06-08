using System;
using System.Collections.Generic;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Shipping;
using Hotcakes.Shipping.FedEx;
using Hotcakes.Shipping.Services;
using Hotcakes.Shipping.USPostal;
using Hotcakes.Shipping.Ups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Hotcakes.Commerce.Tests.Admin
{
    /// <summary>
    /// Summary description for SetupWizardTest3
    /// </summary>
    [TestClass]
    public class SetupWizardTest3 : BaseTest
    {
        /// <summary>
        /// Execute all test in order.
        /// </summary>
        [TestMethod]
        public void TestInOrder()
        {
            SaveHandlingSetting();
            LoadHandlingSetting();
            CreateShippingZone();
            AddZoneArea();
            DeleteZoneArea();
            EditShippingZone();
            DeleteShippingZone();
            AddSM_FlatRatePerItem();
            EditSM_FlatRatePerItem();
            DeleteShippingMethod();
            AddSM_FlatRatePerOrder();
            EditSM_FlatRatePerOrder();
            AddSM_RatePerWeightFormula();
            EditSM_RatePerWeightFormula();
            AddSM_RateTable();
            EditSM_RateTable();
            AddSM_FedEx();
            EditSM_FedEx();
            AddSM_UPS();
            EditSM_UPS();
            AddSM_UPSDomestic();
            EditSM_UPSDomestic();
            AddSM_UPSInternational();
            EditSM_UPSInternational();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Loads the total shipping methods.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        public void LoadTotalShippingMethods()
        {
            //Arrange
            var store = _application.CurrentStore;
            var storesmethod = AvailableServices.FindAll(store);

            //Act
            var resultcount = _irepo.GetTotalShippingMethodCount();

            //Assert
            Assert.AreEqual(storesmethod.Count, resultcount);
        }

        /// <summary>
        /// Loads the store shipping methods.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        public void LoadStoreShippingMethods()
        {
            //Arrange
            var store = _application.CurrentStore;
            var storesmethod = _applicationDB.OrderServices.ShippingMethods.FindAll(store.Id);

            //Act
            var resultcount = _irepo.GetShippingMethodCount();

            //Assert
            Assert.AreEqual(storesmethod.Count, resultcount);
        }

        /// <summary>
        /// Saves the handling setting.
        /// </summary>
        //[TestMethod]
        //[Priority(3)]
        public void SaveHandlingSetting()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var resultsettings = _irepo.GetHandlingSetting();

            store.Settings.HandlingAmount = resultsettings.HandlingAmount;
            store.Settings.HandlingType = resultsettings.HandlingType;
            store.Settings.HandlingNonShipping = resultsettings.HandlingNonShipping;
            #endregion

            //Act
            _application.AccountServices.Stores.Update(store);

            //Assert
            Assert.AreEqual(store.Settings.HandlingAmount, resultsettings.HandlingAmount);
            Assert.AreEqual(store.Settings.HandlingType, resultsettings.HandlingType);
            Assert.AreEqual(store.Settings.HandlingNonShipping, resultsettings.HandlingNonShipping);
        }

        /// <summary>
        /// Loads the handling setting.
        /// </summary>
        //[TestMethod]
        //[Priority(4)]
        public void LoadHandlingSetting()
        {
            //Arrange
            var store = _application.CurrentStore;

            //Act
            var resultsettings = _irepo.GetHandlingSetting();

            //Assert
            Assert.AreEqual(store.Settings.HandlingAmount, resultsettings.HandlingAmount);
            Assert.AreEqual(store.Settings.HandlingType, resultsettings.HandlingType);
            Assert.AreEqual(store.Settings.HandlingNonShipping, resultsettings.HandlingNonShipping);
        }

        /// <summary>
        /// Loads the service codes.
        /// </summary>
        [TestMethod]
        [Priority(5)]
        public void LoadServiceCodes()
        {
            //Arrange
            var count = _irepo.GetUpsServiceCodeCount();

            //Act
            var resultcount = AvailableServices.FindAll(_application.CurrentStore).FirstOrDefault(x => x.Name.Equals("UPS")).ListAllServiceCodes().Count;

            //Assert
            Assert.AreEqual(count, resultcount);
        }

        /// <summary>
        /// Loads the store shipping zones.
        /// </summary>
        [TestMethod]
        [Priority(6)]
        public void LoadStoreShippingZones()
        {
            //Arrange
            var store = _application.CurrentStore;
            var zonecount = _applicationDB.OrderServices.ShippingZones.FindForStore(store.Id);

            //Act
            var resultzonecount = _irepo.GetShippingZoneCount();

            //Assert
            Assert.AreEqual(zonecount.Count, resultzonecount);
        }

        /// <summary>
        /// Creates the shipping zone.
        /// </summary>
        //[TestMethod]
        //[Priority(7)]
        public void CreateShippingZone()
        {
            //Arrange
            var store = _application.CurrentStore;
            var zonename = _irepo.GetAddShippingZoneName();
            var z = new Zone { Name = zonename, StoreId = store.Id };

            //Act
            _application.OrderServices.ShippingZones.Create(z);
            var firstOrDefault = _application.OrderServices.ShippingZones.GetZones(store.Id).FirstOrDefault(x => x.Name.Equals(zonename));
            if (firstOrDefault == null) Assert.AreEqual(0, 1);
            var result = firstOrDefault.Name;

            //Assert
            Assert.AreEqual(zonename, result);
        }

        /// <summary>
        /// Adds the zone area.
        /// </summary>
        //[TestMethod]
        //[Priority(8)]
        public void AddZoneArea()
        {
            #region Arrange
            var zone0 = _irepo.GetAddZoneArea();
            var zone1 = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id).FirstOrDefault(x => x.Name.Equals(zone0.Name));
            if (zone1 == null) Assert.AreEqual(0, 1);

            var area = new ZoneArea
             {
                 CountryIsoAlpha3 = zone0.Areas.FirstOrDefault().CountryIsoAlpha3,
                 RegionAbbreviation = zone0.Areas.FirstOrDefault().RegionAbbreviation,
             };
            zone1.Areas.Add(area);
            #endregion

            #region Act
            _application.OrderServices.ShippingZones.Update(zone1);

            var resultzonearea = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id)
                                .FirstOrDefault(x => x.Name.Equals(zone0.Name))
                                .Areas.FirstOrDefault(x => x.CountryIsoAlpha3.Equals(zone0.Areas.FirstOrDefault().CountryIsoAlpha3));

            #endregion

            //Assert
            Assert.AreEqual(zone0.Areas.FirstOrDefault().CountryIsoAlpha3, resultzonearea.CountryIsoAlpha3);
            Assert.AreEqual(zone0.Areas.FirstOrDefault().RegionAbbreviation, resultzonearea.RegionAbbreviation);

        }

        /// <summary>
        /// Deletes the zone area.
        /// </summary>
        //[TestMethod]
        //[Priority(9)]
        public void DeleteZoneArea()
        {
            #region Arrange
            var zone0 = _irepo.GetDeleteZoneArea();
            var zone1 = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id).FirstOrDefault(x => x.Name.Equals(zone0.Name));
            if (zone1 == null) Assert.AreEqual(0, 1);

            var removezone = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id)
                             .FirstOrDefault(x => x.Name.Equals(zone0.Name));
            var removezonearea = removezone.Areas.FirstOrDefault(x => x.CountryIsoAlpha3.Equals(zone0.Areas.FirstOrDefault().CountryIsoAlpha3));

            #endregion
            #region Act
            _application.OrderServices.ShippingZoneRemoveArea(removezone.Id, removezonearea.CountryIsoAlpha3,
                                                                removezonearea.RegionAbbreviation);


            var resultarea = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id)
                             .FirstOrDefault(x => x.Name.Equals(zone0.Name)).Areas.FirstOrDefault(x => x.CountryIsoAlpha3.Equals(zone0.Areas.FirstOrDefault().CountryIsoAlpha3));

            #endregion

            //Assert
            Assert.IsTrue(resultarea == null);

        }

        /// <summary>
        /// Edits the shipping zone.
        /// </summary>
        //[TestMethod]
        //[Priority(10)]
        public void EditShippingZone()
        {
            //Arrange
            var zone0 = _irepo.GetEditShippingZone();
            var zone1 = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id).FirstOrDefault(x => x.Name.Equals(Convert.ToString(zone0["OldName"])));
            if (zone1 == null) Assert.AreEqual(0, 1);
            zone1.Name = Convert.ToString(zone0["NewName"]);

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingZones.Update(zone1));
        }

        /// <summary>
        /// Deletes the shipping zone.
        /// </summary>
        //[TestMethod]
        //[Priority(11)]
        public void DeleteShippingZone()
        {
            //Arrange
            var zone0 = _irepo.GetDeleteShippingZone();
            var zone1 = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id).FirstOrDefault(x => x.Name.Equals(zone0));
            if (zone1 == null) Assert.AreEqual(0, 1);

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingZones.Delete(zone1.Id));
        }

        /// <summary>
        /// Adds the flat rate per item shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(12)]
        public void AddSM_FlatRatePerItem()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetAddSMInfo_FlatRatePerItem(ref smethod0);

            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);

            var smethod1 = new ShippingMethod
                {
                    Name = smethod0.Name,
                    StoreId = _application.CurrentStore.Id,
                    ZoneId = smethod0.ZoneId,
                    AdjustmentType = ShippingMethodAdjustmentType.Amount,
                    Adjustment = 0,
                    ShippingProviderId = spproviderid,
                };

            var settings = new FlatRatePerItemSettings();
            settings.Merge(smethod1.Settings);
            settings.Amount = Money.RoundCurrency(setting.Amount);
            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));

        }

        /// <summary>
        /// Edits the flat rate per item shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(13)]
        public void EditSM_FlatRatePerItem()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetEditSMInfo_FlatRatePerItem(ref smethod0);

            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                            .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;
            var settings = new FlatRatePerItemSettings();
            settings.Merge(smethod1.Settings);
            settings.Amount = Money.RoundCurrency(setting.Amount);
            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));

        }

        /// <summary>
        /// Deletes the shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(14)]
        public void DeleteShippingMethod()
        {
            #region Arrange
            var method0 = _irepo.GetDeleteShippingMethod();
            var method1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                            .FirstOrDefault(x => x.Name.Equals(method0));
            if (method1 == null) Assert.AreEqual(0, 1);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Delete(method1.Bvin));
        }

        /// <summary>
        /// Adds the flat rate per order shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(15)]
        public void AddSM_FlatRatePerOrder()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetAddSMInfo_FlatRatePerOrder(ref smethod0);

            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);

            var smethod1 = new ShippingMethod
            {
                Name = smethod0.Name,
                StoreId = _application.CurrentStore.Id,
                ZoneId = smethod0.ZoneId,
                AdjustmentType = ShippingMethodAdjustmentType.Amount,
                Adjustment = 0,
                ShippingProviderId = spproviderid,
            };

            var settings = new FlatRatePerOrderSettings();
            settings.Merge(smethod1.Settings);
            settings.Amount = Money.RoundCurrency(setting.Amount);
            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));
        }

        /// <summary>
        /// Edits the flat rate per order shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(16)]
        public void EditSM_FlatRatePerOrder()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetEditSMInfo_FlatRatePerOrder(ref smethod0);


            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                            .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;
            var settings = new FlatRatePerOrderSettings();
            settings.Merge(smethod1.Settings);
            settings.Amount = Money.RoundCurrency(setting.Amount);
            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));
        }

        /// <summary>
        /// Adds the rate per weight formula shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(17)]
        public void AddSM_RatePerWeightFormula()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetAddSMInfo_RatePerWeightFormula(ref smethod0);
            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);

            var smethod1 = new ShippingMethod
              {
                  Name = smethod0.Name,
                  StoreId = _application.CurrentStore.Id,
                  ZoneId = smethod0.ZoneId,
                  AdjustmentType = ShippingMethodAdjustmentType.Amount,
                  Adjustment = 0,
                  ShippingProviderId = spproviderid,
              };

            var settings = new RatePerWeightFormulaSettings();
            settings.Merge(smethod1.Settings);

            settings.BaseAmount = setting.BaseAmount;
            settings.BaseWeight = setting.BaseWeight;
            settings.AdditionalWeightCharge = setting.AdditionalWeightCharge;
            settings.MaxWeight = setting.MaxWeight;
            settings.MinWeight = setting.MinWeight;

            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));
        }

        /// <summary>
        /// Edits the rate per weight formula shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(18)]
        public void EditSM_RatePerWeightFormula()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetEditSMInfo_RatePerWeightFormula(ref smethod0);

            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                            .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;
            var settings = new RatePerWeightFormulaSettings();
            settings.Merge(smethod1.Settings);
            settings.BaseAmount = setting.BaseAmount;
            settings.BaseWeight = setting.BaseWeight;
            settings.AdditionalWeightCharge = setting.AdditionalWeightCharge;
            settings.MaxWeight = setting.MaxWeight;
            settings.MinWeight = setting.MinWeight;
            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));
        }

        /// <summary>
        /// Adds the rate table shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(19)]
        public void AddSM_RateTable()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetAddSMInfo_RateTable(ref smethod0);
            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);
            var lstlevel = setting.GetLevels();

            var smethod1 = new ShippingMethod
            {
                Name = smethod0.Name,
                StoreId = _application.CurrentStore.Id,
                ZoneId = smethod0.ZoneId,
                AdjustmentType = ShippingMethodAdjustmentType.Amount,
                Adjustment = 0,
                ShippingProviderId = spproviderid,

            };

            var settings = new RateTableSettings();
            settings.Merge(smethod1.Settings);
            foreach (var rateTableLevel in lstlevel)
            {
                settings.AddLevel(rateTableLevel);
            }

            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));

        }

        /// <summary>
        /// Edits the rate table shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(20)]
        public void EditSM_RateTable()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var setting = _irepo.GetEditSMInfo_RateTable(ref smethod0);
            var lstlevel = setting.GetLevels();
            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                            .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;
            var settings = new RateTableSettings();
            settings.Merge(smethod1.Settings);

            settings.RemoveLevel(lstlevel.FirstOrDefault());

            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));

        }

        /// <summary>
        /// Adds the FedEx shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(21)]
        public void AddSM_FedEx()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetAddSMInfo_FedEx(ref store, ref smethod0);
            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);

            var smethod1 = new ShippingMethod
            {
                Name = smethod0.Name,
                StoreId = _application.CurrentStore.Id,
                ZoneId = smethod0.ZoneId,
                AdjustmentType = ShippingMethodAdjustmentType.Amount,
                Adjustment = 0,
                ShippingProviderId = spproviderid,
            };

            var settings = new FedExServiceSettings();
            settings.Merge(smethod1.Settings);

            settings.ServiceCode = setting.ServiceCode;
            settings.Packaging = setting.Packaging;

            smethod1.Settings.Merge(settings);

            _application.CurrentStore.Settings.ShippingFedExKey = store.Settings.ShippingFedExKey;
            _application.CurrentStore.Settings.ShippingFedExPassword = store.Settings.ShippingFedExPassword;
            _application.CurrentStore.Settings.ShippingFedExAccountNumber = store.Settings.ShippingFedExAccountNumber;
            _application.CurrentStore.Settings.ShippingFedExMeterNumber = store.Settings.ShippingFedExMeterNumber;
            _application.CurrentStore.Settings.ShippingFedExDefaultPackaging = store.Settings.ShippingFedExDefaultPackaging;
            _application.CurrentStore.Settings.ShippingFedExDropOffType = store.Settings.ShippingFedExDropOffType;
            _application.CurrentStore.Settings.ShippingFedExForceResidentialRates = store.Settings.ShippingFedExForceResidentialRates;
            _application.CurrentStore.Settings.ShippingFedExUseListRates = store.Settings.ShippingFedExUseListRates;
            _application.CurrentStore.Settings.ShippingFedExDiagnostics = store.Settings.ShippingFedExDiagnostics;
            _application.CurrentStore.Settings.ShippingFedExUseDevelopmentServiceUrl = store.Settings.ShippingFedExUseDevelopmentServiceUrl;

            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));

        }

        /// <summary>
        /// Edits the FedEx shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(22)]
        public void EditSM_FedEx()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetEditSMInfo_FedEx(ref store, ref smethod0);

            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                            .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;

            var settings = new FedExServiceSettings();
            settings.Merge(smethod1.Settings);

            settings.ServiceCode = setting.ServiceCode;
            settings.Packaging = setting.Packaging;

            smethod1.Settings.Merge(settings);

            _application.CurrentStore.Settings.ShippingFedExKey = store.Settings.ShippingFedExKey;
            _application.CurrentStore.Settings.ShippingFedExPassword = store.Settings.ShippingFedExPassword;
            _application.CurrentStore.Settings.ShippingFedExAccountNumber = store.Settings.ShippingFedExAccountNumber;
            _application.CurrentStore.Settings.ShippingFedExMeterNumber = store.Settings.ShippingFedExMeterNumber;
            _application.CurrentStore.Settings.ShippingFedExDefaultPackaging = store.Settings.ShippingFedExDefaultPackaging;
            _application.CurrentStore.Settings.ShippingFedExDropOffType = store.Settings.ShippingFedExDropOffType;
            _application.CurrentStore.Settings.ShippingFedExForceResidentialRates = store.Settings.ShippingFedExForceResidentialRates;
            _application.CurrentStore.Settings.ShippingFedExUseListRates = store.Settings.ShippingFedExUseListRates;
            _application.CurrentStore.Settings.ShippingFedExDiagnostics = store.Settings.ShippingFedExDiagnostics;
            _application.CurrentStore.Settings.ShippingFedExUseDevelopmentServiceUrl = store.Settings.ShippingFedExUseDevelopmentServiceUrl;

            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));

        }

        /// <summary>
        /// Adds the UPS shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(23)]
        public void AddSM_UPS()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetAddSMInfo_UPS(ref store, ref smethod0);
            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);
            var smethod1 = new ShippingMethod
            {
                Name = smethod0.Name,
                StoreId = _application.CurrentStore.Id,
                ZoneId = smethod0.ZoneId,
                AdjustmentType = ShippingMethodAdjustmentType.Amount,
                Adjustment = 0,
                ShippingProviderId = spproviderid,
            };

            var settings = new UPSServiceSettings();
            settings.Merge(smethod1.Settings);
            settings.GetAllRates = setting.GetAllRates;
            settings.ServiceCodeFilter = SetServicesFilterCode("UPS", setting.ServiceCodeFilter);

            smethod1.Settings.Merge(settings);

            _application.CurrentStore.Settings.ShippingFedExAccountNumber = store.Settings.ShippingUpsAccountNumber;
            _application.CurrentStore.Settings.ShippingUpsForceResidential = store.Settings.ShippingUpsForceResidential;
            _application.CurrentStore.Settings.ShippingUpsPickupType = store.Settings.ShippingUpsPickupType;
            _application.CurrentStore.Settings.ShippingUpsDefaultService = store.Settings.ShippingUpsDefaultService;
            _application.CurrentStore.Settings.ShippingUpsDefaultPackaging = store.Settings.ShippingUpsDefaultPackaging;
            _application.CurrentStore.Settings.ShippingUpsSkipDimensions = store.Settings.ShippingUpsSkipDimensions;
            _application.CurrentStore.Settings.ShippingUPSDiagnostics = store.Settings.ShippingUPSDiagnostics;
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));

        }

        /// <summary>
        /// Edits the UPS shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(24)]
        public void EditSM_UPS()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetEditSMInfo_UPS(ref store, ref smethod0);

            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                         .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;

            var settings = new UPSServiceSettings();
            settings.Merge(smethod1.Settings);
            settings.GetAllRates = setting.GetAllRates;
            settings.ServiceCodeFilter = SetServicesFilterCode("UPS", setting.ServiceCodeFilter);

            smethod1.Settings.Merge(settings);

            _application.CurrentStore.Settings.ShippingFedExAccountNumber = store.Settings.ShippingUpsAccountNumber;
            _application.CurrentStore.Settings.ShippingUpsForceResidential = store.Settings.ShippingUpsForceResidential;
            _application.CurrentStore.Settings.ShippingUpsPickupType = store.Settings.ShippingUpsPickupType;
            _application.CurrentStore.Settings.ShippingUpsDefaultService = store.Settings.ShippingUpsDefaultService;
            _application.CurrentStore.Settings.ShippingUpsDefaultPackaging = store.Settings.ShippingUpsDefaultPackaging;
            _application.CurrentStore.Settings.ShippingUpsSkipDimensions = store.Settings.ShippingUpsSkipDimensions;
            _application.CurrentStore.Settings.ShippingUPSDiagnostics = store.Settings.ShippingUPSDiagnostics;
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));

        }

        /// <summary>
        /// Adds the UPS domestic shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(25)]
        public void AddSM_UPSDomestic()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetAddSMInfo_UPS_Domestic(ref store, ref smethod0);
            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);
            var smethod1 = new ShippingMethod
            {
                Name = smethod0.Name,
                StoreId = _application.CurrentStore.Id,
                ZoneId = smethod0.ZoneId,
                AdjustmentType = ShippingMethodAdjustmentType.Amount,
                Adjustment = 0,
                ShippingProviderId = spproviderid,
            };

            var settings = new UPSServiceSettings();
            settings.Merge(smethod1.Settings);

            var servicecodes = AvailableServices.FindById(GetShippingProviderId(smethod0.ShippingProviderId), _application.CurrentStore).ListAllServiceCodes();

            foreach (var scode in setting.ServiceCodeFilter.Select(filter => servicecodes.FirstOrDefault(x => x.DisplayName.Equals(filter.DisplayName))))
            {
                settings.ServiceCodeFilter.Add(scode);
            }

            smethod1.Settings.Merge(settings);
            _application.CurrentStore.Settings.ShippingUSPostalDiagnostics = store.Settings.ShippingUSPostalDiagnostics;
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));

        }

        /// <summary>
        /// Edits the UPS domestic shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(26)]
        public void EditSM_UPSDomestic()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetEditSMInfo_UPS_Domestic(ref store, ref smethod0);

            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                         .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;

            var settings = new UPSServiceSettings();
            settings.Merge(smethod1.Settings);
            var servicecodes = AvailableServices.FindById(GetShippingProviderId(smethod0.ShippingProviderId), _application.CurrentStore).ListAllServiceCodes();

            foreach (var scode in setting.ServiceCodeFilter.Select(filter => servicecodes.FirstOrDefault(x => x.DisplayName.Equals(filter.DisplayName))))
            {
                settings.ServiceCodeFilter.Add(scode);
            }

            smethod1.Settings.Merge(settings);
            _application.CurrentStore.Settings.ShippingUSPostalDiagnostics = store.Settings.ShippingUSPostalDiagnostics;
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));

        }

        /// <summary>
        /// Adds the UPS international shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(27)]
        public void AddSM_UPSInternational()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetAddSMInfo_UPS_Internation(ref store, ref smethod0);
            var spproviderid = GetShippingProviderId(smethod0.ShippingProviderId);
            var smethod1 = new ShippingMethod
            {
                Name = smethod0.Name,
                StoreId = _application.CurrentStore.Id,
                ZoneId = smethod0.ZoneId,
                AdjustmentType = ShippingMethodAdjustmentType.Amount,
                Adjustment = 0,
                ShippingProviderId = spproviderid,
            };

            var settings = new UPSServiceSettings();
            settings.Merge(smethod1.Settings);
            var servicecodes = AvailableServices.FindById(GetShippingProviderId(smethod0.ShippingProviderId), _application.CurrentStore).ListAllServiceCodes();

            foreach (var scode in setting.ServiceCodeFilter.Select(filter => servicecodes.FirstOrDefault(x => x.DisplayName.Equals(filter.DisplayName))))
            {
                settings.ServiceCodeFilter.Add(scode);
            }

            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Create(smethod1));

        }

        /// <summary>
        /// Edits the UPS international shipping method.
        /// </summary>
        //[TestMethod]
        //[Priority(28)]
        public void EditSM_UPSInternational()
        {
            #region Arrange
            var smethod0 = new ShippingMethod();
            var store = _application.CurrentStore;
            var setting = _irepo.GetEditSMInfo_UPS_Internation(ref store, ref smethod0);

            var smethod1 = _application.OrderServices.ShippingMethods.FindAll(_application.CurrentStore.Id)
                         .FirstOrDefault(x => x.ShippingProviderId.Equals(GetShippingProviderId(smethod0.ShippingProviderId)));

            smethod1.Name = smethod0.Name;
            smethod1.ZoneId = smethod0.ZoneId;

            var settings = new UPSServiceSettings();
            settings.Merge(smethod1.Settings);
            var servicecodes = AvailableServices.FindById(GetShippingProviderId(smethod0.ShippingProviderId), _application.CurrentStore).ListAllServiceCodes();

            foreach (var scode in setting.ServiceCodeFilter.Select(filter => servicecodes.FirstOrDefault(x => x.DisplayName.Equals(filter.DisplayName))))
            {
                settings.ServiceCodeFilter.Add(scode);
            }

            smethod1.Settings.Merge(settings);
            #endregion

            //Act/Assert
            Assert.IsTrue(_application.OrderServices.ShippingMethods.Update(smethod1));

        }


        /// <summary>
        /// FedEx test rate.
        /// </summary>
        [TestMethod]
        [Priority(29)]
        public void FedExTestRate()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var testSettings = _irepo.GetTestRate_FedExSettings();
            var servicecode = _irepo.GetTestRate_FedExServiceCode();
            var testSvc = new FedExProvider(testSettings, new Web.Logging.TextLogger())
                {
                    Settings =
                        {
                            ServiceCode = servicecode,
                            Packaging = (int)testSettings.DefaultPackaging
                        }
                };

            var testShipment = _irepo.GetTestRate_FedExShipment(store);
            var testItem = _irepo.GetTestRate_FedExShippableInfo();
            testShipment.Items.Add(testItem);
            #endregion

            //Act
            var rates = testSvc.RateShipment(testShipment);


            //Assert
            if (rates == null)
                Assert.AreEqual(1, 0);
            else
                Assert.AreEqual(25, rates.FirstOrDefault().EstimatedCost);
        }

        /// <summary>
        /// UPS domestic shipping method test rate.
        /// </summary>
        [TestMethod]
        [Priority(30)]
        public void UPS_DomesticTestRate()
        {
            #region Arrange
            var shipment = _irepo.GetTestRate_UPS_DomesticShipment();
            var item = _irepo.GetTestRate_UPS_DomesticShippableInfo();
            shipment.Items.Add(item);
            var globalSettings = new USPostalServiceGlobalSettings {
				UserId = _application.CurrentStore.Settings.ShippingUSPostalUserId,
				DiagnosticsMode = true,
				IgnoreDimensions = false
			};
            var settings = _irepo.GetTestRate_UPS_DomesticServiceSetting();

            var servicecodes = AvailableServices.FindById(GetShippingProviderId("US Postal Service - Domestic"), _application.CurrentStore).ListAllServiceCodes();

            var displayname = settings.ServiceCodeFilter.FirstOrDefault().DisplayName;

            var code = servicecodes.FirstOrDefault(x => x.DisplayName.Equals(displayname));

            settings.ServiceCodeFilter = new List<IServiceCode> { code };

            var provider = new DomesticProvider(globalSettings, new Web.Logging.TextLogger())
                {
                    Settings = settings
                };
            #endregion

            //Act
            var rates = provider.RateShipment(shipment);

            //Assert
            if (rates == null)
                Assert.AreEqual(1, 0);
            else
                Assert.AreEqual(25, rates.FirstOrDefault().EstimatedCost);

        }

        /// <summary>
        /// UPS international shipping method test rate.
        /// </summary>
        [TestMethod]
        [Priority(31)]
        public void UPS_InternationalTestRate()
        {
            #region Arrange
            var shipment = _irepo.GetTestRate_UPS_InternationShipment();
            var item = _irepo.GetTestRate_UPS_InternationShippableInfo();
            var settings = _irepo.GetTestRate_UPS_InternationServiceSetting();
            shipment.Items.Add(item);
            var globalSettings = new USPostalServiceGlobalSettings { DiagnosticsMode = true, IgnoreDimensions = false };

            var servicecodes = AvailableServices.FindById(GetShippingProviderId("US Postal Service - International"), _application.CurrentStore).ListAllServiceCodes();
            var displayname = settings.ServiceCodeFilter.FirstOrDefault().DisplayName;
            var code = servicecodes.FirstOrDefault(x => x.DisplayName.Equals(displayname));
            settings.ServiceCodeFilter = new List<IServiceCode> { code };

            var provider = new InternationalProvider(globalSettings, new Web.Logging.TextLogger())
                {
                    Settings = settings
                };
            #endregion

            //Act
            var rates = provider.RateShipment(shipment);

            //Assert
            if (rates == null)
                Assert.AreEqual(1, 0);
            else
                Assert.AreEqual(25, rates.FirstOrDefault().EstimatedCost);

        }


        /// <summary>
        /// Sets the services filter code.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="serviceCodeFilter">The service code filter.</param>
        /// <returns></returns>
        private List<IServiceCode> SetServicesFilterCode(string name, IEnumerable<IServiceCode> serviceCodeFilter)
        {
            try
            {
                var servicecodes = AvailableServices.FindAll(_application.CurrentStore)
                                        .FirstOrDefault(x => x.Name.Equals(name))
                                        .ListAllServiceCodes();

                return serviceCodeFilter.Select(filter => servicecodes.FirstOrDefault(x => x.DisplayName.Equals(filter.DisplayName))).ToList();
            }
            catch (Exception)
            {
                return new List<IServiceCode>();
            }
        }

        /// <summary>
        /// Gets the shipping provider identifier.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetShippingProviderId(string name)
        {
            return AvailableServices.FindAll(_application.CurrentStore)
                               .FirstOrDefault(x => x.Name.Equals(name))
                               .Id;
        }
    }
}
