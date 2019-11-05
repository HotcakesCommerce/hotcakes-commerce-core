#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;
using Hotcakes.Web.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for ReportingService and is intended
    ///     to contain all ReportingService Unit Tests
    /// </summary>
    [TestClass]
    public class ReportingServiceTest : BaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ReportingService_TestOrderSummary()
        {
        }

        [TestMethod]
        public void ReportingService_TestSalesReport()
        {
            var app = CreateHccAppForDb();

            SamplesBuilder.AddFiveSimpleProducts(app); // SKU: 001, 002, 003, 004, 005

            var now = DateTime.UtcNow;

            CreateOrder(app, now.AddMonths(-2), new[] {"001", "002"}); // 100+200
            CreateOrder(app, now.AddMonths(-1), new[] {"002", "003"}); // 200+300
            CreateOrder(app, now, new[] {"004", "005"}); // 400 + 499.99

            Assert.AreEqual(3, app.OrderServices.Orders.CountOfAll());

            var reporting = Factory.CreateService<ReportingService>();
            var salesInfo = reporting.GetSalesInfo(SalesPeriod.Year);

            Assert.AreEqual(12, salesInfo.ChartLabels.Count);
            Assert.AreEqual(12, salesInfo.ChartData.Count);

            Assert.AreEqual(300m, salesInfo.ChartData[9]);
            Assert.AreEqual(500m, salesInfo.ChartData[10]);
            Assert.AreEqual(900m, salesInfo.ChartData[11]); // report function rounds value from 899.99 to 900

            Assert.AreEqual(3, salesInfo.SalesByDesktopCount);
            Assert.AreEqual(100, salesInfo.SalesByDesktopPercent);
            Assert.AreEqual(0, salesInfo.SalesByDesktopPercentPrev);
        }

        [TestMethod]
        public void ReportingService_ProductPerformance()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            Assert.AreEqual(8, app.OrderServices.Orders.CountOfAll());

            var reportingService = Factory.CreateService<ReportingService>();

            var performanceInfo = reportingService.GetProductPerformance(product1.Bvin, SalesPeriod.Year);

            Assert.AreEqual(2, performanceInfo.Purchases);
            Assert.AreEqual(1, performanceInfo.PurchasesPrev);
            Assert.AreEqual(3, performanceInfo.AddsToCart);
            Assert.AreEqual(2, performanceInfo.AddsToCartPrev);
            Assert.AreEqual(3, performanceInfo.Views);
            Assert.AreEqual(1, performanceInfo.ViewsPrev);

            Assert.AreEqual(5, performanceInfo.PurchasedData[335]);
            Assert.AreEqual(1, performanceInfo.BouncedData[335]);
            Assert.AreEqual(15, performanceInfo.AbandonedData[335]);

            performanceInfo = reportingService.GetProductPerformance(product1.Bvin, SalesPeriod.Week);

            // Array data is not checked for week period since position can change depending on time tests are run
            Assert.AreEqual(0, performanceInfo.Purchases);
            Assert.AreEqual(0, performanceInfo.PurchasesPrev);
            Assert.AreEqual(0, performanceInfo.AddsToCart);
            Assert.AreEqual(0, performanceInfo.AddsToCartPrev);
            Assert.AreEqual(0, performanceInfo.Views);
            Assert.AreEqual(1, performanceInfo.ViewsPrev);
        }

        [TestMethod]
        public void ReportingService_AllProductPerformance()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            Assert.AreEqual(8, app.OrderServices.Orders.CountOfAll());

            var reportingService = Factory.CreateService<ReportingService>();

            var performanceInfo = reportingService.GetProductPerformance(SalesPeriod.Year);

            Assert.AreEqual(10, performanceInfo.Purchases);
            Assert.AreEqual(2, performanceInfo.PurchasesPrev);
            Assert.AreEqual(12, performanceInfo.AddsToCart);
            Assert.AreEqual(4, performanceInfo.AddsToCartPrev);
            Assert.AreEqual(4, performanceInfo.Views);
            Assert.AreEqual(2, performanceInfo.ViewsPrev);

            Assert.AreEqual(13, performanceInfo.PurchasedData[335]);
            Assert.AreEqual(1, performanceInfo.BouncedData[335]);
            Assert.AreEqual(23, performanceInfo.AbandonedData[335]);

            performanceInfo = reportingService.GetProductPerformance(SalesPeriod.Week);

            // Array data is not checked for week period since position can change depending on time tests are run
            Assert.AreEqual(4, performanceInfo.Purchases);
            Assert.AreEqual(2, performanceInfo.PurchasesPrev);
            Assert.AreEqual(4, performanceInfo.AddsToCart);
            Assert.AreEqual(2, performanceInfo.AddsToCartPrev);
            Assert.AreEqual(0, performanceInfo.Views);
            Assert.AreEqual(1, performanceInfo.ViewsPrev);
        }

        [TestMethod]
        public void ReportingService_CategoryPerformance()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            var reportingService = Factory.CreateService<ReportingService>();

            var performanceInfo = reportingService.GetCategoryPerformance(category1.Bvin, SalesPeriod.Year);

            Assert.AreEqual(4, performanceInfo.Purchases);
            Assert.AreEqual(2, performanceInfo.PurchasesPrev);
            Assert.AreEqual(6, performanceInfo.AddsToCart);
            Assert.AreEqual(4, performanceInfo.AddsToCartPrev);
            Assert.AreEqual(4, performanceInfo.Views);
            Assert.AreEqual(2, performanceInfo.ViewsPrev);

            Assert.AreEqual(13, performanceInfo.PurchasedData[335]);
            Assert.AreEqual(1, performanceInfo.BouncedData[335]);
            Assert.AreEqual(23, performanceInfo.AbandonedData[335]);

            performanceInfo = reportingService.GetCategoryPerformance(category1.Bvin, SalesPeriod.Week);

            // Array data is not checked for week period since position can change depending on time tests are run
            Assert.AreEqual(0, performanceInfo.Purchases);
            Assert.AreEqual(1, performanceInfo.PurchasesPrev);
            Assert.AreEqual(0, performanceInfo.AddsToCart);
            Assert.AreEqual(1, performanceInfo.AddsToCartPrev);
            Assert.AreEqual(0, performanceInfo.Views);
            Assert.AreEqual(1, performanceInfo.ViewsPrev);
        }

        [TestMethod]
        public void ReportingService_TopChangeByBounces()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            var reportingService = Factory.CreateService<ReportingService>();

            var topBounced = reportingService.GetTopChangeByBounces(SalesPeriod.Year, SortDirection.Descending, 1, 5);
            Assert.AreEqual(9, topBounced.TotalCount);
            Assert.AreEqual("Product 1", topBounced.Items[0].ProductName);
            Assert.AreEqual(1, topBounced.Items[0].Change);
        }

        [TestMethod]
        public void ReportingService_TopChangeByAbandoments()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            var reportingService = Factory.CreateService<ReportingService>();

            var topAbandoned = reportingService.GetTopChangeByAbandoments(SalesPeriod.Year, SortDirection.Descending, 1,
                5);
            Assert.AreEqual(9, topAbandoned.TotalCount);
            Assert.AreEqual("Product 1", topAbandoned.Items[0].ProductName);
            Assert.AreEqual(-0.72, (double) topAbandoned.Items[0].Change, 0.01);
        }

        [TestMethod]
        public void ReportingService_TopChangeByPurchases()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            var reportingService = Factory.CreateService<ReportingService>();

            var topPurchased = reportingService.GetTopChangeByPurchases(SalesPeriod.Year, SortDirection.Descending, 1, 5);

            Assert.AreEqual(9, topPurchased.TotalCount);
            Assert.AreEqual("Product 2", topPurchased.Items[0].ProductName);
            Assert.AreEqual(10, topPurchased.Items[0].Change);
            Assert.AreEqual("Product 1", topPurchased.Items[1].ProductName);
            Assert.AreEqual(2, topPurchased.Items[1].Change);
            Assert.AreEqual("Product 3", topPurchased.Items[2].ProductName);
            Assert.AreEqual(1, topPurchased.Items[2].Change);

            topPurchased = reportingService.GetTopChangeByPurchases(SalesPeriod.Week, SortDirection.Descending, 1, 5);
            Assert.AreEqual(9, topPurchased.TotalCount);
            Assert.AreEqual("Product 3", topPurchased.Items[3].ProductName);
            Assert.AreEqual(-0.57, (double) topPurchased.Items[3].Change, 0.01);
            Assert.AreEqual("Brown Fedora", topPurchased.Items[4].ProductName);
            Assert.AreEqual(0, topPurchased.Items[4].Change);
        }

        [TestMethod]
        public void ReportingService_TopAffectedProducts()
        {
            var app = CreateHccAppForDb();

            Product product1;
            Category category1;
            PopulateTestData(app, out product1, out category1);

            var reportingService = Factory.CreateService<ReportingService>();

            var topAffected = reportingService.GetTopAffectedProducts(SalesPeriod.Year, TopAffectedSort.ByChange,
                SortDirection.Descending, 1, 5);
            Assert.AreEqual(9, topAffected.TotalCount);
            Assert.AreEqual("Product 2", topAffected.Items[0].ProductName);
            Assert.AreEqual(10, topAffected.Items[0].Change);
            Assert.AreEqual(0, topAffected.Items[0].BouncesChange);
            Assert.AreEqual(0, topAffected.Items[0].AbandomentsChange);
            Assert.AreEqual(10, topAffected.Items[0].PurchasesChange);
            Assert.AreEqual("Product 1", topAffected.Items[1].ProductName);
            Assert.AreEqual(1.72, (double) topAffected.Items[1].Change, 0.01);
            Assert.AreEqual(1, topAffected.Items[1].BouncesChange);
            Assert.AreEqual(-0.72, (double) topAffected.Items[1].AbandomentsChange, 0.01);
            Assert.AreEqual(2, topAffected.Items[1].PurchasesChange);

            topAffected = reportingService.GetTopAffectedProducts(SalesPeriod.Week, TopAffectedSort.ByPurchasesChange,
                SortDirection.Descending, 1, 5);
            Assert.AreEqual(9, topAffected.TotalCount);
            Assert.AreEqual("Product 3", topAffected.Items[3].ProductName);
            Assert.AreEqual(-0.57, (double) topAffected.Items[3].Change, 0.01);
            Assert.AreEqual(0, topAffected.Items[3].BouncesChange);
            Assert.AreEqual(0, topAffected.Items[3].AbandomentsChange);
            Assert.AreEqual(-0.57, (double) topAffected.Items[3].PurchasesChange, 0.01);
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
        [TestInitialize]
        public void TestInitialize()
        {
            var sqlQueryTemplate = @"RESTORE DATABASE HotcakesDevTest
							FROM DISK = '{0}'
							WITH REPLACE,
							MOVE 'HotcakesDevTest' TO '{1}',
							MOVE 'HotcakesDevTest_log' TO '{2}'";

            var sqlQuery = string.Format(sqlQueryTemplate,
                Path.Combine(Environment.CurrentDirectory, @"DbBackup\HotcakesDevTest.bak"),
                Path.Combine(Environment.CurrentDirectory, @"DbBackup\HotcakesDevTest.mdf"),
                Path.Combine(Environment.CurrentDirectory, @"DbBackup\HotcakesDevTest.ldf"));
            ExecuteSql(sqlQuery);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            var sqlQuery = @"IF EXISTS (SELECT name FROM sys.databases WHERE name = N'HotcakesDevTest')
								BEGIN
									ALTER DATABASE [HotcakesDevTest]
									SET SINGLE_USER WITH ROLLBACK IMMEDIATE
									DROP DATABASE [HotcakesDevTest]
								END";

            ExecuteSql(sqlQuery);
        }

        private void ExecuteSql(string sql)
        {
            var conString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var conStringBuilder = new SqlConnectionStringBuilder(conString);
            conStringBuilder.InitialCatalog = string.Empty;
            using (var connection = new SqlConnection(conStringBuilder.ConnectionString))
            {
                connection.Open();

                var sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = sql;
                sqlCommand.ExecuteNonQuery();
            }
        }

        #endregion

        #region Private

        private void PopulateTestData(HotcakesApplication app, out Product product1, out Category category1)
        {
            product1 = new Product
            {
                Sku = "Product1",
                ProductName = "Product 1",
                SitePrice = 12
            };
            var product2 = new Product
            {
                Sku = "Product2",
                ProductName = "Product 2",
                SitePrice = 18
            };
            var product3 = new Product
            {
                Sku = "Product3",
                ProductName = "Product 3",
                SitePrice = 24
            };

            category1 = new Category
            {
                Name = "Category 1"
            };
            var category2 = new Category
            {
                Name = "Category 2"
            };

            app.CatalogServices.Products.Create(product1);
            app.CatalogServices.Products.Create(product2);
            app.CatalogServices.Products.Create(product3);

            app.CatalogServices.Categories.Create(category1);
            app.CatalogServices.Categories.Create(category2);

            app.CatalogServices.CategoriesXProducts.AddProductToCategory(product1.Bvin, category1.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(product2.Bvin, category1.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(product1.Bvin, category2.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(product3.Bvin, category2.Bvin);

            var now = DateTime.UtcNow.Date;

            // Months offsets were converted to days offsets due to test crashes because of different month length
            CreateOrder(app, now.AddDays(-395), new[] {"Product1", "Product2"}, new[] {2, 1});
            CreateOrder(app, now.AddDays(-61), new[] {"Product3", "Product1"}, new[] {2, 1});
            CreateOrder(app, now.AddDays(-30), new[] {"Product1", "Product2"}, new[] {5, 8});
            CreateOrder(app, now.AddDays(-30), new[] {"Product1", "Product2"}, new[] {15, 8}, false);
            CreateOrder(app, now.AddDays(-487), new[] {"Product1", "Product2"}, new[] {55, 8}, false);
            CreateOrder(app, now.AddDays(-9), new[] {"Product2", "Product3"}, new[] {3, 7});
            CreateOrder(app, now.AddDays(-2), new[] {"SAMPLE001", "Product3"}, new[] {7, 3});
            CreateOrder(app, now, new[] {"SAMPLE001", "SAMPLE002"}, new[] {2, 3});

            var shoppingSession1 = Guid.NewGuid();
            var shoppingSession2 = Guid.NewGuid();
            RegisterAnalyticsEvent(app, now.AddDays(-13), ActionTypes.ProductViewed, product1.Bvin, shoppingSession2);
            RegisterAnalyticsEvent(app, now.AddDays(-30), ActionTypes.ProductViewed, product1.Bvin, shoppingSession2);
            RegisterAnalyticsEvent(app, now.AddDays(-61), ActionTypes.ProductViewed, product1.Bvin, shoppingSession1);
            RegisterAnalyticsEvent(app, now.AddDays(-395), ActionTypes.ProductViewed, product1.Bvin, shoppingSession1);
            RegisterAnalyticsEvent(app, now.AddDays(-61), ActionTypes.ProductViewed, product2.Bvin, shoppingSession1);
            RegisterAnalyticsEvent(app, now.AddDays(-426), ActionTypes.ProductViewed, product2.Bvin, shoppingSession1);
            RegisterAnalyticsEvent(app, now.AddDays(-30), ActionTypes.ProductAddedToCart, product2.Bvin,
                shoppingSession1);
            RegisterAnalyticsEvent(app, now.AddDays(-26), ActionTypes.ProductAddedToCart, product2.Bvin,
                shoppingSession2);
        }

        private void CreateOrder(HotcakesApplication app, DateTime timeOfOrder, string[] skus, int[] quantities = null,
            bool isPlaced = true)
        {
            var o = new Order {StoreId = app.CurrentStore.Id, TimeOfOrderUtc = timeOfOrder, IsPlaced = isPlaced};
            for (var i = 0; i < skus.Length; i++)
            {
                var qty = 1;
                if (quantities != null && i < quantities.Length)
                    qty = quantities[i];
                AddProductToCart(app, o, skus[i], qty);
            }
            app.CalculateOrderAndSave(o);

            // Place order
            o.IsPlaced = isPlaced;
            app.OrderServices.Orders.Update(o);

            if (isPlaced)
            {
                // Pay for order
                var pm = new OrderPaymentManager(o, app);
                pm.OfflinePaymentAddInfo(o.TotalGrand, "Cash");

                CashReceive(app, pm, o.TotalGrand, o);
            }
        }

        private void RegisterAnalyticsEvent(HotcakesApplication app, DateTime dateTime, ActionTypes actionType,
            string objectId, Guid shoppingSessionGuid)
        {
            var analyticsEvent = new AnalyticsEvent();
            analyticsEvent.UserId = null;
            analyticsEvent.SessionGuid = null;
            analyticsEvent.ShoppingSessionGuid = shoppingSessionGuid;
            analyticsEvent.StoreId = app.CurrentStore.Id;
            analyticsEvent.Action = actionType;
            analyticsEvent.ObjectId = DataTypeHelper.BvinToNullableGuid(objectId);
            analyticsEvent.DateTime = dateTime;

            app.AnalyticsService.AnalyticsEvents.Create(analyticsEvent);
        }

        private void CashReceive(HotcakesApplication app, OrderPaymentManager pm, decimal amount, Order o)
        {
            var t = pm.CreateEmptyTransaction();
            t.Amount = amount;
            t.Action = ActionType.CashReceived;
            var ot = new OrderTransaction(t) {Success = true, TimeStampUtc = o.TimeOfOrderUtc};
            app.OrderServices.AddPaymentTransactionToOrder(o, ot);
        }

        private void AddProductToCart(HotcakesApplication app, Order o, string sku, int quantity)
        {
            var p = app.CatalogServices.Products.FindBySku(sku);
            Assert.IsNotNull(p);
            var li = p.ConvertToLineItem(app, quantity);
            o.Items.Add(li);
        }

        #endregion
    }
}