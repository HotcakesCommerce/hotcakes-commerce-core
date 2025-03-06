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
using AuthorizeNet;
using Hotcakes.Payment.Gateways;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Payment.Tests
{
    /// <summary>
    ///     Summary description for AuthorizeNetRecurringTest
    /// </summary>
    [TestClass]
    public class AuthorizeNetRecurringTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void AuthorizeNet_ARBCallTest()
        {
            var target = new SubscriptionGateway("77pmw32Vh7LS", "73629XQgg28GW2tp", ServiceMode.Test);

            ISubscriptionRequest subscription = SubscriptionRequest
                .CreateMonthly("john2@doe.com", "ARB Test" + Guid.NewGuid(), (decimal) 5.60, 12);

            subscription.CardNumber = "4111111111111111";
            subscription.CardExpirationMonth = 1;
            subscription.CardExpirationYear = 20;
            
            var billToAddress = new Address();
            billToAddress.First = "John";
            billToAddress.Last = "Doe";
            subscription.BillingAddress = billToAddress;

            ISubscriptionRequest actual = null;

            try
            {
                actual = target.CreateSubscription(subscription);
            }
            catch (Exception e)
            {
                var s = e.Message;
                Console.WriteLine("Failed to create SUB: " + e);
            }
        }

        [TestMethod]
        public void AuthorizeNet_BasicTest()
        {
            var gw = new RecurringGateways.AuthorizeNet
            {
                Settings = new AuthorizeNetSettings
                {
                    MerchantLoginId = "77pmw32Vh7LS",
                    TransactionKey = "73629XQgg28GW2tp",
                    SendEmailToCustomer = true,
                    DeveloperMode = true,
                    DebugMode = true
                    //TestMode = true,
                }
            };
            var t = new Transaction
            {
                Action = ActionType.RecurringSubscriptionCreate,
                Card = new CardData
                {
                    CardNumber = "4111111111111111",
                    ExpirationMonth = 1,
                    ExpirationYear = 2020,
                    SecurityCode = "999",
                    CardHolderName = "TestCard"
                },
                Amount = 5.5m,
                Customer =
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@doe.com"
                }
            };
            t.AdditionalSettings["SubscriptionName"] = "ARB Subscrition Test 11";
            gw.ProcessTransaction(t);
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