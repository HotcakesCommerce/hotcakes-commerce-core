using System;
using Hotcakes.Commerce.Contacts;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests.Admin
{
    /// <summary>
    /// Summary description for SetupWizardTest1
    /// </summary>
    [TestClass]
    public class SetupWizardTest1 : BaseTest
    {
        /// <summary>
        ///  Execute all test in order
        /// </summary>
        [TestMethod]
        public void TestInOrder()
        {
            SaveAddress();
            LoadAddress();
            SaveStore();
            StoreInfo();
        }


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        /// Populates the countries and regions.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        public void PopulateCountriesRegions()
        {
            //Arrange
            var cocount = _irepo.GetCountryCount();
            var recount = _irepo.GetRegionCount();

            //Act
            var resultcontries = _applicationDB.GlobalizationServices.Countries.FindActiveCountries();
            var bvnid = resultcontries.FirstOrDefault(x => x.DisplayName.Equals(Convert.ToString(recount["CId"]))).Bvin;
            var country = _applicationDB.GlobalizationServices.Countries.Find(bvnid);

            //Assert
            Assert.AreEqual(cocount, resultcontries.Count);
            Assert.AreEqual(Convert.ToInt32(recount["Count"]), country.Regions.Count);
        }

        /// <summary>
        /// Populates the cultures.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        public void PopulateCultures()
        {
            //Arrange
            var count = _irepo.GetCultureCount();

            //Act
            var resultcultures = _applicationDB.GlobalizationServices.Countries.FindAllForCurrency();

            //Assert
            Assert.AreEqual(count, resultcultures.Count);
        }

        /// <summary>
        /// Saves the address.
        /// </summary>
        //[TestMethod]
        //[Priority(3)]
        public void SaveAddress()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var address = _irepo.GetContactAddress();

            var toUpdate = new Address
                {
                    CountryBvin = "",
                    RegionBvin = "",
                    FirstName = address.FirstName,
                    LastName = address.LastName,
                    Company = address.Company,
                    Line1 = address.Line1,
                    Line2 = address.Line2,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    Phone = address.Phone,
                    Bvin = address.Bvin,
                    StoreId = store.Id,
                    AddressType = AddressTypes.StoreContact,
                };
            #endregion

            #region Act
            _application.ContactServices.Addresses.Create(toUpdate);
            var resultcontactaddress1 = _application.ContactServices.Addresses.FindStoreContactAddress();

            if (string.IsNullOrEmpty(resultcontactaddress1.Bvin)) return;

            toUpdate.FirstName = toUpdate.FirstName + "_Update";
            _application.ContactServices.Addresses.Update(toUpdate);
            var resultcontactaddress2 = _application.ContactServices.Addresses.FindStoreContactAddress();
            #endregion

            //Assert
            Assert.AreNotEqual(string.Empty, resultcontactaddress1.Bvin);
            Assert.AreEqual(toUpdate.FirstName + "_Update", resultcontactaddress2.FirstName);
        }

        /// <summary>
        /// Loads the address.
        /// </summary>
        //[TestMethod]
        //[Priority(4)]
        public void LoadAddress()
        {
            //Arrange
            var contactaddress = _irepo.GetContactAddress();

            //Act
            var resultcontactaddress = _applicationDB.ContactServices.Addresses.FindStoreContactAddress();
            

            #region Assert
            Assert.AreEqual(contactaddress.FirstName, resultcontactaddress.FirstName);
            Assert.AreEqual(contactaddress.LastName, resultcontactaddress.LastName);
            Assert.AreEqual(contactaddress.Company, resultcontactaddress.Company);
            Assert.AreEqual(contactaddress.Line1, resultcontactaddress.Line1);
            Assert.AreEqual(contactaddress.Line2, resultcontactaddress.Line2);
            Assert.AreEqual(contactaddress.City, resultcontactaddress.City);
            Assert.AreEqual(contactaddress.PostalCode, resultcontactaddress.PostalCode);
            Assert.AreEqual(contactaddress.Phone, resultcontactaddress.Phone);
            #endregion

        }

        /// <summary>
        /// Populates the urls.
        /// </summary>
        [TestMethod]
        [Priority(5)]
        public void PopulateUrls()
        {
            //Arrange
            var store = _application.CurrentStore;
            var pages = _irepo.GetPages();
            var pagetabcount = _irepo.GetPageTabCount();

            //Assert
            Assert.AreEqual(store.Settings.Urls.CategoryUrl, pages.CategoryUrl);
            Assert.AreEqual(store.Settings.Urls.ProductUrl, pages.ProductUrl);
            Assert.AreEqual(store.Settings.Urls.CheckoutUrl, pages.CheckoutUrl);

            //TODO: To check tab count need to change in tabcontroller
        }

        /// <summary>
        /// Saves the store.
        /// </summary>
        //[TestMethod]
        //[Priority(6)]
        public void SaveStore()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var storeinfo = _irepo.GetStoreInfo();
            var storeurls = _irepo.GetPages();
            var storeurlstab = _irepo.GetPageTabCount();

            store.Settings.FriendlyName = storeinfo.FriendlyName;
            store.Settings.UseLogoImage = true;
            store.Settings.LogoImage = storeinfo.LogoImage;
            store.Settings.ForceAdminSSL = storeinfo.ForceAdminSSL;

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_irepo.GetTimeZoneId());
            store.Settings.TimeZone = timeZone;
            store.Settings.CurrencyCultureCode = _irepo.GetCurrencyCultureCode();

            var sett = store.Settings.Urls;

            var showPageUrls = true;// Hotcakes.Commerce.Dnn.Utils.PageUtils.ShowPageUrls(); //TODO:Need to change this function for CI
            if (showPageUrls)
            {
                sett.CategoryUrl = storeurls.CategoryUrl;
                sett.ProductUrl = storeurls.ProductUrl;
                sett.CheckoutUrl = storeurls.CheckoutUrl;
                sett.AddressBookUrl = storeurls.AddressBookUrl;
                sett.CartUrl = storeurls.CartUrl;
                sett.OrderHistoryUrl = storeurls.OrderHistoryUrl;
                sett.ProductReviewUrl = storeurls.ProductReviewUrl;
                sett.SearchUrl = storeurls.SearchUrl;
                sett.WishListUrl = storeurls.WishListUrl;
            }

            sett.CategoryTabId = Convert.ToInt32(storeurlstab["CategoryTabCount"]);
            sett.ProductTabId = Convert.ToInt32(storeurlstab["ProductTabCount"]);
            sett.CheckoutTabId = Convert.ToInt32(storeurlstab["CheckoutTabCount"]);

            //PageUtils.EnsureTabsExist(sett);TODO:Need to change this function for CI 
            #endregion

            //Act
            _application.UpdateCurrentStore();

            #region Assert
            Assert.AreEqual(storeinfo.FriendlyName, store.Settings.FriendlyName);
            Assert.AreEqual(storeinfo.LogoImage, store.Settings.LogoImage);
            Assert.AreEqual(storeinfo.ForceAdminSSL, store.Settings.ForceAdminSSL);
            Assert.AreEqual(timeZone.Id, store.Settings.TimeZone.Id);
            Assert.AreEqual(_irepo.GetCurrencyCultureCode(), store.Settings.CurrencyCultureCode);
            Assert.AreEqual(sett.CategoryUrl, store.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.ProductUrl, store.Settings.Urls.ProductUrl);
            Assert.AreEqual(sett.CheckoutUrl, store.Settings.Urls.CheckoutUrl);
            Assert.AreEqual(sett.AddressBookUrl, store.Settings.Urls.AddressBookUrl);
            Assert.AreEqual(sett.CartUrl, store.Settings.Urls.CartUrl);
            Assert.AreEqual(sett.OrderHistoryUrl, store.Settings.Urls.OrderHistoryUrl);
            Assert.AreEqual(sett.ProductReviewUrl, store.Settings.Urls.ProductReviewUrl);
            Assert.AreEqual(sett.SearchUrl, store.Settings.Urls.SearchUrl);
            Assert.AreEqual(sett.WishListUrl, store.Settings.Urls.WishListUrl);
            #endregion

        }

        /// <summary>
        /// Stores information.
        /// </summary>
        //[TestMethod]
        //[Priority(7)]
        public void StoreInfo()
        {
            //Arrange
            var store = _application.CurrentStore;
            var storeinfo = _irepo.GetStoreInfo();

            #region Assert
            Assert.AreEqual(store.Settings.FriendlyName, storeinfo.FriendlyName);
            Assert.AreEqual(store.Settings.LogoImage, storeinfo.LogoImage);
            Assert.AreEqual(store.Settings.ForceAdminSSL, storeinfo.ForceAdminSSL);
            #endregion

        }

    }
}
