using Hotcakes.Licensing.Common;
using Hotcakes.Licensing.Common.DataContracts;
using Hotcakes.Licensing.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests.Admin
{
    /// <summary>
    /// Summary description for SetupWizardTest5
    /// </summary>
    [TestClass]
    public class SetupWizardTest5 : BaseTest
    {
        //TODO:For test LicensingService ​ we need to set proper data for or need code so I can test with some fake data
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Logins user.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        public void Login()
        {
            //Arrange
            var logininfo = _irepo.GetLoginInfo();

            //Act
            var service = new LicensingService(logininfo);

            //Assert
            Assert.IsTrue(service.CanLogin());
        }

        /// <summary>
        /// Registers user.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        public void Register()
        {
            //Arrange
            var reginfo = _irepo.GetRegisterInfo();

            //Act
            var service = new LicensingService();

            //Assert
            Assert.IsTrue(service.Register(reginfo));
        }

        /// <summary>
        /// Loads the license orders.
        /// </summary>
        [TestMethod]
        [Priority(3)]
        public void LoadLicenseOrders()
        {
            //Arrange
            var count = _irepo.GetLicenseOrderCount();
            var logininfo = _irepo.GetLicenseLoginInfo();

            //Act
            var service = new LicensingService(logininfo);

            //Assert
            Assert.AreEqual(count, service.GetLicenseOrders().Count);
        }

        /// <summary>
        /// Activation Licenses.
        /// </summary>
        [TestMethod]
        [Priority(4)]
        public void LicenseActivation()
        {
            #region Arrange
            var licOrderId = _irepo.GetLicenseOrderId();
            var logininfo = _irepo.GetLicenseLoginInfo();
            var activationInfo = new ActivationInfoDTO
                {
                    LicenseOrderId = licOrderId,
                    MachineName = LicenseUtils.GetMachineName(),
                    MetaPath = LicenseUtils.GetInstanceMetaPath(),
                    Host = LicenseUtils.GetHostName(),
                };
            #endregion

            //Act
            var data = LicenseUtils.SerializeActivationInfo(activationInfo);
            var service = new LicensingService(logininfo);

            //Assert
            Assert.AreNotEqual(string.Empty, service.ActivateLicense(data));

        }

        /// <summary>
        /// Licenses the lookup.
        /// </summary>
        [TestMethod]
        [Priority(5)]
        public void LicenseLookup()
        {
            #region Arrange
            var orderlookupmsg = _irepo.GetOrderLookupMsg();
            var orderstore = _irepo.GetOrderStore();
            var orderquery = _irepo.GetOrderLookupQuery();
            var logininfo = _irepo.GetLicenseLoginInfo();
            OrderQueryDTO lookupQuery;
            if (orderstore != 1)
            {
                lookupQuery = new OrderQueryDTO
                    {
                        OrderSource = OrderSources.DnnMarketplace,
                        Email = orderquery.Email,
                        InvoiceId = orderquery.InvoiceId,
                    };
            }
            else
            {
                lookupQuery = new OrderQueryDTO
                    {
                        OrderSource = OrderSources.CompLicense,
                        Email = orderquery.Email,
                        LicenseKey = orderquery.LicenseKey
                    };
            }
            #endregion

            //Act
            var service = new LicensingService(logininfo);
            var result = service.LookupOrders(lookupQuery);

            //Assert
            Assert.AreEqual(OrderLookupResults.OrderNotFound, result);

        }

        /// <summary>
        /// Licenses the purchase.
        /// </summary>
        [TestMethod]
        [Priority(6)]
        public void LicensePurchase()
        {
            //TODO: Need to understand process 
            //TODO:Code not found
        }
    }
}
