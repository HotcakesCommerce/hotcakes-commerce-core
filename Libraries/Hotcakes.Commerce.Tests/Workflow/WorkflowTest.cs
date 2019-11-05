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
using System.Linq;
using AuthorizeNet;
using AuthorizeNet.APICore;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.BusinessRules.OrderTasks;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Payment;
using Hotcakes.Payment.Gateways;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Order = Hotcakes.Commerce.Orders.Order;

namespace Hotcakes.Commerce.Tests.Workflow
{
    /// <summary>
    ///     Summary description for WorkflowTest
    /// </summary>
    [TestClass]
    public class WorkflowTest : BaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Workflow_CreateRecurringSubscriptions()
        {
            var app = CreateHccAppInMemory();

            // Configure Authorize ARB
            var gateId = new Hotcakes.Payment.RecurringGateways.AuthorizeNet().Id;
            app.CurrentStore.Settings.PaymentReccuringGateway = gateId;
            app.CurrentStore.Settings.PaymentSettingsSet(gateId, new AuthorizeNetSettings
            {
                MerchantLoginId = "77pmw32Vh7LS",
                TransactionKey = "73629XQgg28GW2tp",
                DeveloperMode = true,
                DebugMode = true,
                TestMode = true
            });
            app.UpdateCurrentStore();

            // Create sample subscription
            var prod1 = new Product
            {
                Sku = "001",
                ProductName = "Subscription1",
                SitePrice = 10m,
                IsRecurring = true,
                RecurringInterval = 10,
                RecurringIntervalType = RecurringIntervalType.Days
            };
            app.CatalogServices.Products.Create(prod1);

            // Add subscription to cart
            var o = new Order {StoreId = app.CurrentStore.Id, OrderNumber = DateTime.Now.ToString("yyMMddHHmmss")};
            o.Items.Add(prod1.ConvertToLineItem(app));
            app.CalculateOrderAndSave(o);

            // Add card info
            var payManager = new OrderPaymentManager(o, app);
            payManager.RecurringSubscriptionAddCardInfo(new CardData
            {
                CardHolderName = "Test Card",
                CardNumber = "4111111111111111",
                ExpirationMonth = 1,
                ExpirationYear = 2020
            });

            o.BillingAddress.FirstName = "John";
            o.BillingAddress.LastName = "Joe";
            o.UserEmail = "john.joe@test.com";

            // Run workflow task
            var c = new OrderTaskContext(app.CurrentRequestContext);
            c.UserId = app.CurrentCustomerId;
            c.Order = o;
            c.Inputs.SetProperty("hcc", "CardSecurityCode", "999");

            var task = new CreateRecurringSubscriptions();
            var taskResult = task.Execute(c);

            // Check result ---------------------
            Assert.IsTrue(taskResult);
            var trans = app.OrderServices.Transactions.FindAllPaged(1, 100);
            Assert.AreEqual(2, trans.Count);
            var tInfo = trans.FirstOrDefault(t => t.Action == ActionType.RecurringSubscriptionInfo);
            var tCreateSub = trans.FirstOrDefault(t => t.Action == ActionType.RecurringSubscriptionCreate);

            Assert.IsNotNull(tInfo);
            Assert.AreEqual("4111111111111111", tInfo.CreditCard.CardNumber);
            Assert.AreEqual(2020, tInfo.CreditCard.ExpirationYear);
            Assert.IsNotNull(tCreateSub);
            Assert.AreEqual(10m, tCreateSub.Amount);
            Assert.AreNotEqual("", tCreateSub.RefNum1);
            Assert.AreEqual(tInfo.IdAsString, tCreateSub.LinkedToTransaction);

            var subGatewey = new SubscriptionGateway2("77pmw32Vh7LS", "73629XQgg28GW2tp", ServiceMode.Test);

            var resSub = subGatewey.GetSubscriptionById(Convert.ToInt32(tCreateSub.RefNum1));

            Assert.IsNotNull(resSub);
            Assert.AreEqual("John", resSub.firstName);
            Assert.AreEqual("Joe", resSub.lastName);
            Assert.AreEqual(10, resSub.amount);
            Assert.AreEqual(ARBSubscriptionStatusEnum.active, resSub.status);
            Assert.IsTrue(resSub.invoice.Contains(tCreateSub.OrderNumber));
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion
    }
}