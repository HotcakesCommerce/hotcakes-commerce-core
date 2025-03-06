#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Shipping.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for OrderTest and is intended
    ///     to contain all OrderTest Unit Tests
    /// </summary>
    [TestClass]
    public class OrderTest : BaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void Order_CanAddItemToOrderAndCalculate()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            var target = new Order();
            var li = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product",
                ProductSku = "ABC123",
                Quantity = 2
            };
            target.Items.Add(li);
            app.CalculateOrder(target);
            Assert.AreEqual(39.98m, target.TotalOrderBeforeDiscounts, "SubTotal was Incorrect");
            Assert.AreEqual(39.98m, target.TotalGrand, "Grand Total was incorrect");

            var upsertResult = app.OrderServices.Orders.Upsert(target);
            Assert.IsTrue(upsertResult, "Order Upsert Failed");

            Assert.AreEqual(app.CurrentRequestContext.CurrentStore.Id, target.StoreId,
                "Order store ID was not set correctly");
            Assert.AreNotEqual(string.Empty, target.bvin, "Order failed to get a bvin");
            Assert.AreEqual(1, target.Items.Count, "Item count should be one");
            Assert.AreEqual(target.bvin, target.Items[0].OrderBvin, "Line item didn't receive order bvin");
            Assert.AreEqual(target.StoreId, target.Items[0].StoreId, "Line item didn't recieve storeid");
        }

        [TestMethod]
        public void Order_CanCalculateShippingWithNonShippingItems()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            // Create Shipping Method            
            var m = new ShippingMethod();
            m.ShippingProviderId = new FlatRatePerItem().Id;
            m.Settings = new FlatRatePerItemSettings {Amount = 1.50m};
            m.Adjustment = 0m;
            m.Bvin = Guid.NewGuid().ToString();
            m.Name = "Sample Order";
            m.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(m);
            app.OrderServices.EnsureDefaultZones(app.CurrentStore.Id);

            var target = new Order {StoreId = app.CurrentStore.Id};
            target.BillingAddress.City = "Richmond";
            target.BillingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            target.BillingAddress.Line1 = "124 Anywhere St.";
            target.BillingAddress.PostalCode = "23233";
            target.BillingAddress.RegionBvin = "VA";

            target.ShippingAddress.City = "Richmond";
            target.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            target.ShippingAddress.Line1 = "124 Anywhere St.";
            target.ShippingAddress.PostalCode = "23233";
            target.ShippingAddress.RegionBvin = "VA";

            target.ShippingMethodId = m.Bvin;
            target.ShippingProviderId = m.ShippingProviderId;

            var li = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product",
                ProductSku = "ABC123",
                Quantity = 1,
                IsNonShipping = true
            };
            target.Items.Add(li);

            var li2 = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product 2",
                ProductSku = "ABC1232",
                Quantity = 1,
                IsNonShipping = false
            };
            target.Items.Add(li2);

            app.CalculateOrder(target);
            Assert.AreEqual(39.98m, target.TotalOrderBeforeDiscounts, "SubTotal was Incorrect");
            Assert.AreEqual(1.50m, target.TotalShippingBeforeDiscounts, "Shipping Total was Incorrect");
            Assert.AreEqual(41.48m, target.TotalGrand, "Grand Total was incorrect");
        }

        [TestMethod]
        public void Order_CanSkipShippingWhenOnlyNonShippingItems()
        {
            var app = CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 1;

            // Create Shipping Method            
            var m = new ShippingMethod();
            m.ShippingProviderId = "3D6623E7-1E2C-444d-B860-A8F542133093"; // Flat Rate Per Item
            m.Settings = new FlatRatePerItemSettings {Amount = 1.50m};
            m.Adjustment = 0m;
            m.Bvin = Guid.NewGuid().ToString();
            m.Name = "Sample Order";
            m.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(m);

            var target = new Order();
            target.BillingAddress.City = "Richmond";
            target.BillingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            target.BillingAddress.Line1 = "124 Anywhere St.";
            target.BillingAddress.PostalCode = "23233";
            target.BillingAddress.RegionBvin = "VA";

            target.ShippingAddress.City = "Richmond";
            target.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            target.ShippingAddress.Line1 = "124 Anywhere St.";
            target.ShippingAddress.PostalCode = "23233";
            target.ShippingAddress.RegionBvin = "VA";

            target.ShippingMethodId = m.Bvin;
            target.ShippingProviderId = m.ShippingProviderId;

            var li = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product",
                ProductSku = "ABC123",
                Quantity = 1,
                IsNonShipping = true
            };
            target.Items.Add(li);

            var li2 = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product 2",
                ProductSku = "ABC1232",
                Quantity = 1,
                IsNonShipping = true
            };
            target.Items.Add(li2);

            app.CalculateOrder(target);
            Assert.AreEqual(39.98m, target.TotalOrderBeforeDiscounts, "SubTotal was Incorrect");
            Assert.AreEqual(0m, target.TotalShippingBeforeDiscounts, "Shipping Total was Incorrect");
            Assert.AreEqual(39.98m, target.TotalGrand, "Grand Total was incorrect");
        }

        [TestMethod]
        public void Order_CanUseShippingOverrideCorrectly()
        {
            var app = CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 1;

            // Create Shipping Method            
            var m = new ShippingMethod();
            m.ShippingProviderId = "3D6623E7-1E2C-444d-B860-A8F542133093"; // Flat Rate Per Item
            m.Settings = new FlatRatePerItemSettings {Amount = 1.50m};
            m.Adjustment = 0m;
            m.Bvin = Guid.NewGuid().ToString();
            m.Name = "Sample Order";
            m.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(m);

            var target = new Order();
            target.BillingAddress.City = "Richmond";
            target.BillingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            target.BillingAddress.Line1 = "124 Anywhere St.";
            target.BillingAddress.PostalCode = "23233";
            target.BillingAddress.RegionBvin = "VA";

            target.ShippingAddress.City = "Richmond";
            target.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            target.ShippingAddress.Line1 = "124 Anywhere St.";
            target.ShippingAddress.PostalCode = "23233";
            target.ShippingAddress.RegionBvin = "VA";

            target.ShippingMethodId = m.Bvin;
            target.ShippingProviderId = m.ShippingProviderId;

            var li = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product",
                ProductSku = "ABC123",
                Quantity = 1,
                IsNonShipping = true
            };
            target.Items.Add(li);

            var li2 = new LineItem
            {
                BasePricePerItem = 19.99m,
                ProductName = "Sample Product 2",
                ProductSku = "ABC1232",
                Quantity = 1,
                IsNonShipping = false
            };
            target.Items.Add(li2);

            target.TotalShippingBeforeDiscountsOverride = 5.00m;

            app.CalculateOrder(target);
            Assert.AreEqual(39.98m, target.TotalOrderBeforeDiscounts, "SubTotal was Incorrect");
            Assert.AreEqual(5.00m, target.TotalShippingBeforeDiscounts, "Shipping Total was not overridden");
            Assert.AreEqual(44.98m, target.TotalGrand, "Grand Total was incorrect");
        }

        [TestMethod]
        public void Order_CanAddCouponToOrder()
        {
            var target = new Order();

            Assert.IsTrue(target.AddCouponCode("coupon"), "Add failed");
            Assert.IsTrue(target.CouponCodeExists("coupon"), "Validate Check Failed");
            Assert.AreEqual(1, target.Coupons.Count, "Coupon count should be one");
            Assert.AreEqual("COUPON", target.Coupons[0].CouponCode, "Code didn't match");
        }

        [TestMethod]
        public void Order_CanSaveAndRetrieveCouponsInRepository()
        {
            var target = new Order();
            var c = new HccRequestContext();
            c.CurrentStore = new Store {Id = 1};
            var repository = Factory.CreateRepo<OrderRepository>(c);
            repository.Create(target);
            target.AddCouponCode("one");
            target.AddCouponCode("two");
            target.AddCouponCode("three");
            repository.Update(target);

            var actual = repository.FindForCurrentStore(target.bvin);

            Assert.AreEqual(target.Coupons.Count, actual.Coupons.Count, "Coupon count didn't match");
            for (var i = 0; i < target.Coupons.Count; i++)
            {
                Assert.AreEqual(target.Coupons[i].CouponCode, actual.Coupons[i].CouponCode,
                    "Code didn't match for index " + i);
            }
        }

        [TestMethod]
        public void Order_CanAddCouponAndGetIdNumber()
        {
            var target = new Order();
            target.AddCouponCode("one");
            target.AddCouponCode("two");
            var c = new HccRequestContext();
            c.CurrentStore = new Store {Id = 1};
            var repository = Factory.CreateRepo<OrderRepository>(c);
            repository.Create(target);

            Assert.AreEqual(2, target.Coupons.Count, "Coupon count should be one");
            Assert.AreNotEqual(0, target.Coupons[0].Id, "Coupon id should never be zero");
            Assert.AreNotEqual(target.Coupons[0].Id, target.Coupons[1].Id, "Coupon ids should be unique");
        }

        [TestMethod]
        public void Order_RemoveCouponCodeTest()
        {
            var target = new Order();
            var c = new HccRequestContext();
            c.CurrentStore = new Store {Id = 1};
            var repository = Factory.CreateRepo<OrderRepository>(c);
            repository.Create(target);
            target.AddCouponCode("one");
            target.AddCouponCode("two");
            target.AddCouponCode("three");
            repository.Update(target);

            Assert.IsTrue(target.RemoveCouponCode(target.Coupons[1].Id), "Call to remove failed");
            Assert.AreEqual(2, target.Coupons.Count, "Target count should be two!");
            Assert.IsTrue(repository.Update(target), "Call to updated failed");

            var actual = repository.FindForCurrentStore(target.bvin);

            Assert.AreEqual(2, actual.Coupons.Count, "Coupon count didn't match");
            for (var i = 0; i < target.Coupons.Count; i++)
            {
                Assert.AreEqual(target.Coupons[i].CouponCode, actual.Coupons[i].CouponCode,
                    "Code didn't match for index " + i);
            }
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