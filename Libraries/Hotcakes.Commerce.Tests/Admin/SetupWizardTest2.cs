using System.Linq;
using Hotcakes.Commerce.Payment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests.Admin
{
    /// <summary>
    /// Summary description for SetupWizardTest2
    /// </summary>
    [TestClass]
    public class SetupWizardTest2 : BaseTest
    {
        /// <summary>
        /// Execute all test in order
        /// </summary>
        [TestMethod]
        public void TestInOrder()
        {
            SavePaymentOptions();
            LoadStorePaymentMethods();
            SavePaypalPaymentOptions();
            LoadPaypalPaymentOptions();
            SaveCreditCardPaymentOptions();
            LoadCreditCardPaymentOptions();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Populates the gateways.
        /// </summary>
        [TestMethod]
        [Priority(1)]
        public void PopulateGateways()
        {
            //Arrange
            var pgcount = _irepo.GetTotalGatewayCount();
            var resultpgcount = PaymentGateways.FindAll().Count;

            //Assert
            Assert.AreEqual(pgcount, resultpgcount);
        }

        /// <summary>
        /// Load the payment methods.
        /// </summary>
        [TestMethod]
        [Priority(2)]
        public void LoadPaymentMethods()
        {
            //Arrange
            var count = _irepo.GetTotalPaymentMethodCount();
            var resultcount = PaymentMethods.AvailableMethods().Count;

            //Assert
            Assert.AreEqual(count, resultcount);
        }

        /// <summary>
        /// Saves the payment options.
        /// </summary>
        //[TestMethod]
        //[Priority(3)]
        public void SavePaymentOptions()
        {
            #region Arrange
            var store = _application.CurrentStore;

            var paymentmethods = _irepo.GetPaymentMethods();
            var newlst = paymentmethods.Select(paymentmethod => paymentmethod.Key).ToList();
            _application.CurrentStore.Settings.PaymentMethodsEnabled = newlst;
            #endregion

            //Act
            _application.UpdateCurrentStore();
            // _application.AccountServices.Stores.Update(store);


            //Assert
            Assert.AreEqual(newlst.Count, _application.CurrentStore.Settings.PaymentMethodsEnabled.Count);
        }

        /// <summary>
        /// Loads the store payment methods.
        /// </summary>
        //[TestMethod]
        //[Priority(4)]
        public void LoadStorePaymentMethods()
        {
            //Arrange
            var store = _application.CurrentStore;
            var count = _irepo.GetPaymentMethodCount();

            //Assert
            Assert.AreEqual(store.Settings.PaymentMethodsEnabled.Count, count);
        }

        /// <summary>
        /// Saves the paypal payment options.
        /// </summary>
        //[TestMethod]
        //[Priority(5)]
        public void SavePaypalPaymentOptions()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var paypalinfo = _irepo.GetPayPalInfo();

            store.Settings.PayPal.Mode = paypalinfo.Mode;
            store.Settings.PayPal.UserName = paypalinfo.UserName;
            store.Settings.PayPal.Password = paypalinfo.Password;
            store.Settings.PayPal.Signature = paypalinfo.Signature;
            store.Settings.PayPal.FastSignupEmail = paypalinfo.FastSignupEmail;
            store.Settings.PayPal.ExpressAuthorizeOnly = paypalinfo.ExpressAuthorizeOnly;
            store.Settings.PayPal.AllowUnconfirmedAddresses = paypalinfo.AllowUnconfirmedAddresses;
            store.Settings.PayPal.Currency = paypalinfo.Currency;
            #endregion

            //Act
            _application.UpdateCurrentStore();

            #region Assert
            Assert.AreEqual(store.Settings.PayPal.Mode, paypalinfo.Mode);
            Assert.AreEqual(store.Settings.PayPal.UserName, paypalinfo.UserName);
            Assert.AreEqual(store.Settings.PayPal.Password, paypalinfo.Password);
            Assert.AreEqual(store.Settings.PayPal.Signature, paypalinfo.Signature);
            Assert.AreEqual(store.Settings.PayPal.FastSignupEmail, paypalinfo.FastSignupEmail);
            Assert.AreEqual(store.Settings.PayPal.ExpressAuthorizeOnly, paypalinfo.ExpressAuthorizeOnly);
            Assert.AreEqual(store.Settings.PayPal.AllowUnconfirmedAddresses, paypalinfo.AllowUnconfirmedAddresses);
            Assert.AreEqual(store.Settings.PayPal.Currency, paypalinfo.Currency);
            #endregion

        }

        /// <summary>
        /// Loads the paypal payment options.
        /// </summary>
        //[TestMethod]
        //[Priority(6)]
        public void LoadPaypalPaymentOptions()
        {
            //Arrange
            var store = _application.CurrentStore;
            var paypalinfo = _irepo.GetPayPalInfo();

            //Assert
            Assert.AreEqual(store.Settings.PayPal.Mode, paypalinfo.Mode);
            Assert.AreEqual(store.Settings.PayPal.UserName, paypalinfo.UserName);
            Assert.AreEqual(store.Settings.PayPal.Password, paypalinfo.Password);
            Assert.AreEqual(store.Settings.PayPal.Signature, paypalinfo.Signature);
            Assert.AreEqual(store.Settings.PayPal.FastSignupEmail, paypalinfo.FastSignupEmail);
            Assert.AreEqual(store.Settings.PayPal.ExpressAuthorizeOnly, paypalinfo.ExpressAuthorizeOnly);
            Assert.AreEqual(store.Settings.PayPal.AllowUnconfirmedAddresses, paypalinfo.AllowUnconfirmedAddresses);
            Assert.AreEqual(store.Settings.PayPal.Currency, paypalinfo.Currency);

        }

        /// <summary>
        /// Saves the credit card payment options.
        /// </summary>
        //[TestMethod]
        //[Priority(7)]
        public void SaveCreditCardPaymentOptions()
        {
            #region Arrange
            var store = _application.CurrentStore;
            var cardinfo = _irepo.GetCreditCardInfo();
            store.Settings.PaymentCreditCardAuthorizeOnly = cardinfo.PaymentCreditCardAuthorizeOnly;
            store.Settings.PaymentCreditCardRequireCVV = cardinfo.PaymentCreditCardRequireCVV;
            store.Settings.PaymentCreditCardGateway = cardinfo.PaymentCreditCardGateway;
            store.Settings.DisplayFullCreditCardNumbers = cardinfo.DisplayFullCreditCardNumbers;
            store.Settings.PaymentAcceptedCards = cardinfo.PaymentAcceptedCards;
            #endregion

            //Act
            _application.UpdateCurrentStore();

            //Assert
            Assert.AreEqual(store.Settings.PaymentCreditCardAuthorizeOnly, cardinfo.PaymentCreditCardAuthorizeOnly);
            Assert.AreEqual(store.Settings.PaymentCreditCardRequireCVV, cardinfo.PaymentCreditCardRequireCVV);
            Assert.AreEqual(store.Settings.DisplayFullCreditCardNumbers, cardinfo.DisplayFullCreditCardNumbers);
            Assert.AreEqual(store.Settings.PaymentCreditCardGateway, cardinfo.PaymentCreditCardGateway);
            Assert.AreEqual(store.Settings.PaymentAcceptedCards.Count, cardinfo.PaymentAcceptedCards.Count);
        }

        /// <summary>
        /// Loads the credit card payment options.
        /// </summary>
        //[TestMethod]
        //[Priority(8)]
        public void LoadCreditCardPaymentOptions()
        {
            //Arrange
            var store = _application.CurrentStore;
            var cardinfo = _irepo.GetCreditCardInfo();

            //Assert
            Assert.AreEqual(store.Settings.PaymentCreditCardAuthorizeOnly, cardinfo.PaymentCreditCardAuthorizeOnly);
            Assert.AreEqual(store.Settings.PaymentCreditCardRequireCVV, cardinfo.PaymentCreditCardRequireCVV);
            Assert.AreEqual(store.Settings.DisplayFullCreditCardNumbers, cardinfo.DisplayFullCreditCardNumbers);
            Assert.AreEqual(store.Settings.PaymentCreditCardGateway, cardinfo.PaymentCreditCardGateway);
            Assert.AreEqual(store.Settings.PaymentAcceptedCards.Count, cardinfo.PaymentAcceptedCards.Count);
        }


    }
}
