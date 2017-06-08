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

using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Shipping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for OrderService and is intended
    ///     to contain all OrderService Unit Tests
    /// </summary>
    [TestClass]
    public class OrderServiceTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void OrderService_CanCreateShippingMethod()
        {
            var app = BaseTest.CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 230;

            var target = new ShippingMethod();
            target.Adjustment = 1.23m;
            target.AdjustmentType = ShippingMethodAdjustmentType.Percentage;
            target.Bvin = string.Empty;
            target.Name = "Test Name";
            target.Settings.AddOrUpdate("MySetting", "MySetVal");
            target.ShippingProviderId = "123456";
            target.ZoneId = -101;

            Assert.IsTrue(app.OrderServices.ShippingMethods.Create(target));
            Assert.AreNotEqual(string.Empty, target.Bvin, "Bvin should not be empty");
        }

        [TestMethod]
        public void OrderService_CanRetrieveShippingMethod()
        {
            var app = BaseTest.CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 230;

            var target = new ShippingMethod();
            target.Adjustment = 1.23m;
            target.AdjustmentType = ShippingMethodAdjustmentType.Percentage;
            target.Bvin = string.Empty;
            target.Name = "Test Name";
            target.Settings.AddOrUpdate("MySetting", "MySetVal");
            target.ShippingProviderId = "123456";
            target.ZoneId = -101;

            app.OrderServices.ShippingMethods.Create(target);
            Assert.AreNotEqual(string.Empty, target.Bvin, "Bvin should not be empty");

            var actual = app.OrderServices.ShippingMethods.Find(target.Bvin);
            Assert.IsNotNull(actual, "Actual should not be null");

            Assert.AreEqual(actual.Adjustment, target.Adjustment);
            Assert.AreEqual(actual.AdjustmentType, target.AdjustmentType);
            Assert.AreEqual(actual.Bvin, target.Bvin);
            Assert.AreEqual(actual.Name, target.Name);
            Assert.AreEqual(actual.Settings["MySetting"], target.Settings["MySetting"]);
            Assert.AreEqual(actual.ShippingProviderId, target.ShippingProviderId);
            Assert.AreEqual(actual.ZoneId, target.ZoneId);
        }

        [TestMethod]
        public void OrderService_CanUpdateShippingMethod()
        {
            var app = BaseTest.CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 230;

            var target = new ShippingMethod();
            target.Adjustment = 1.23m;
            target.AdjustmentType = ShippingMethodAdjustmentType.Percentage;
            target.Bvin = string.Empty;
            target.Name = "Test Name";
            target.Settings.AddOrUpdate("MySetting", "MySetVal");
            target.ShippingProviderId = "123456";
            target.ZoneId = -101;

            app.OrderServices.ShippingMethods.Create(target);
            Assert.AreNotEqual(string.Empty, target.Bvin, "Bvin should not be empty");

            target.Adjustment = 1.95m;
            target.AdjustmentType = ShippingMethodAdjustmentType.Amount;
            target.Name = "Test Name Updated";
            target.Settings.AddOrUpdate("MySetting", "MySetVal 2");
            target.ShippingProviderId = "1Update";
            target.ZoneId = -100;
            Assert.IsTrue(app.OrderServices.ShippingMethods.Update(target));

            var actual = app.OrderServices.ShippingMethods.Find(target.Bvin);
            Assert.IsNotNull(actual, "Actual should not be null");

            Assert.AreEqual(actual.Adjustment, target.Adjustment);
            Assert.AreEqual(actual.AdjustmentType, target.AdjustmentType);
            Assert.AreEqual(actual.Bvin, target.Bvin);
            Assert.AreEqual(actual.Name, target.Name);
            Assert.AreEqual(actual.Settings["MySetting"], target.Settings["MySetting"]);
            Assert.AreEqual(actual.ShippingProviderId, target.ShippingProviderId);
            Assert.AreEqual(actual.ZoneId, target.ZoneId);
        }

        [TestMethod]
        public void OrderService_CanDeleteShippingMethod()
        {
            var app = BaseTest.CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 230;

            var target = new ShippingMethod();
            target.Adjustment = 1.23m;
            target.AdjustmentType = ShippingMethodAdjustmentType.Percentage;
            target.Bvin = string.Empty;
            target.Name = "Test Name";
            target.Settings.AddOrUpdate("MySetting", "MySetVal");
            target.ShippingProviderId = "123456";
            target.ZoneId = -101;

            Assert.IsTrue(app.OrderServices.ShippingMethods.Create(target));
            Assert.AreNotEqual(string.Empty, target.Bvin, "Bvin should not be empty");

            Assert.IsTrue(app.OrderServices.ShippingMethods.Delete(target.Bvin), "Delete should be true");
            var actual = app.OrderServices.ShippingMethods.Find(target.Bvin);
            Assert.IsNull(actual, "Actual should be null after delete");
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion
    }
}