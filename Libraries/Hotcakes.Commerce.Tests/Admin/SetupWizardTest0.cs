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

using System.Collections.Generic;
using DotNetNuke.Entities.Users;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Web.Data;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests.Admin
{
    /// <summary>
    /// Summary description for SetupWizardTest
    /// </summary>
    [TestClass]
    public class SetupWizardTest0 : BaseTest
    {
        /// <summary>
        /// Execute all test in order
        /// </summary>
        [TestMethod]
        public void TestInOrder()
        {
			using (ShimsContext.Create())
			{
				DnnGlobal.SetTestableInstance(GetStubIDnnGlobal());
				DnnUserController.SetTestableInstance(GetStubIUserController(new List<UserInfo>()));

				CreateDefaultStore();
				StoreUrls();
				EnsureStore();
			}
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Creates the default store.
        /// </summary>
        //[TestMethod]
        //[Priority(1)]
        public void CreateDefaultStore()
        {
            //TODO: Need to change CreateAndSetupStore function for CI

            //Arrange
            var store = _irepo.GetStore();

            //Act
            var resultstore = _application.AccountServices.CreateAndSetupStore();

            //Assert
            if (resultstore == null)
                Assert.AreEqual(1, 0);
            else
            {
                _application.CurrentStore = resultstore;

                #region Assert

                Assert.AreEqual(store.StoreName, resultstore.StoreName);
                //Assert.AreEqual(store.CustomUrl, resultstore.CustomUrl);
                Assert.AreEqual(store.Settings.MaxItemsPerOrder, resultstore.Settings.MaxItemsPerOrder);
                Assert.AreEqual(store.Settings.MaxWeightPerOrder, resultstore.Settings.MaxWeightPerOrder);
                Assert.AreEqual(store.Settings.AllowProductReviews, resultstore.Settings.AllowProductReviews);
                Assert.AreEqual(store.Settings.ProductReviewModerate, resultstore.Settings.ProductReviewModerate);
                Assert.AreEqual(store.Settings.ProductReviewCount, resultstore.Settings.ProductReviewCount);
                Assert.AreEqual(store.Settings.MinumumOrderAmount, resultstore.Settings.MinumumOrderAmount);
                Assert.AreEqual(store.Settings.LogoText, resultstore.Settings.LogoText);
                Assert.AreEqual(store.Settings.UseLogoImage, resultstore.Settings.UseLogoImage);
                Assert.AreEqual(store.Settings.LogoRevision, resultstore.Settings.LogoRevision);
                Assert.AreEqual(store.Settings.FriendlyName, resultstore.Settings.FriendlyName);

                Assert.AreEqual(store.Settings.MailServer.FromEmail, resultstore.Settings.MailServer.FromEmail);
                Assert.AreEqual(store.Settings.MailServer.EmailForGeneral, resultstore.Settings.MailServer.EmailForGeneral);
                Assert.AreEqual(store.Settings.MailServer.EmailForNewOrder, resultstore.Settings.MailServer.EmailForNewOrder);
                Assert.AreEqual(store.Settings.MailServer.UseCustomMailServer, resultstore.Settings.MailServer.UseCustomMailServer);

                Assert.AreEqual(store.Settings.PayPal.FastSignupEmail, resultstore.Settings.PayPal.FastSignupEmail);
                Assert.AreEqual(store.Settings.PayPal.Currency, resultstore.Settings.PayPal.Currency);

                #endregion
            }
        }

        /// <summary>
        /// Stores urls.
        /// </summary>
        //[TestMethod]
        //[Priority(2)]
        public void StoreUrls()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var storeurls = _irepo.GetStoreUrl();
            var sett = _application.CurrentStore.Settings.Urls;

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
            #endregion

            #region Act
            //PageUtils.EnsureTabsExist(sett); TODO:Need to change this function for CI
            _application.AccountServices.Stores.Update(store);//OR _application.UpdateCurrentStore();
            _application.UpdateCurrentStore();
            var resultstore = _application.AccountServices.Stores.FindById(new PrimaryKey(1).IntValue);
            #endregion

            #region Assert
            Assert.AreEqual(sett.CategoryUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.ProductUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.CheckoutUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.AddressBookUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.CartUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.OrderHistoryUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.ProductReviewUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.SearchUrl, resultstore.Settings.Urls.CategoryUrl);
            Assert.AreEqual(sett.WishListUrl, resultstore.Settings.Urls.CategoryUrl);
            #endregion
        }

        /// <summary>
        /// Ensures the store.
        /// </summary>
        //[TestMethod]
        //[Priority(3)]
        public void EnsureStore()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var zone = _irepo.ShippingZones();
            var systemColumnsFilePath = System.Configuration.ConfigurationManager.AppSettings["SystemColumnsData"];
            #endregion

            #region Act
            //_application.ContentServices.Columns.CreateFromTemplateFile(systemColumnsFilePath);
            //TODO: Need to change CreateFromTemplateFile functiona for set path
            _application.OrderServices.ShippingZones.EnsureDefaultZones(store.Id, _application);
            //  Hotcakes.Modules.Core.HotcakesController.AddAdminPage(PortalSettings.Current.PortalId); TODO: Need to change this functiona and get data from memory for CI

            var resultcolumns = _application.ContentServices.Columns.FindAll();
            var resultdefaulttimezone = _application.OrderServices.ShippingZones.FindForStore(_application.CurrentStore.Id);

            #endregion
          
            #region Assert
            foreach (var zone1 in zone)
            {
                Assert.AreEqual(zone1.Name, resultdefaulttimezone.Find(x => x.Name.Equals(zone1.Name)).Name);
            }

            foreach (var column in new List<string> { "Default Category Viewer Header", "Default Category Viewer Footer" })
            {
                Assert.AreEqual(column, resultcolumns.Find(x => x.DisplayName.Equals(column)).DisplayName);
            }

            #endregion
        }
    }
}
