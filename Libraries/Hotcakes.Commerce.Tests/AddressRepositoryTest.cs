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

using Hotcakes.Commerce.Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for AddressRepositoryTest and is intended
    ///     to contain all AddressRepositoryTest Unit Tests
    /// </summary>
    [TestClass]
    public class AddressRepositoryTest : BaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for Create
        /// </summary>
        [TestMethod]
        public void Address_CanCreateAnAddressAndAssignBvin()
        {
            HccRequestContext.Current = new HccRequestContext();
            HccRequestContext.Current.CurrentStore.Id = 12;
            var addressRepo = Factory.CreateRepo<AddressRepository>();

            var item = new Address();
            item.City = "Richmond";

            var expected = true;
            var actual = addressRepo.Create(item);

            Assert.AreEqual(expected, actual);
            Assert.AreNotEqual(string.Empty, item.Bvin, "Bvin should not be empty after create.");
        }

        /// <summary>
        ///     A test for FindStoreContactAddress
        /// </summary>
        [TestMethod]
        public void Address_CanFindStoreAddressIfOneDoesntExist()
        {
            var app = CreateHccAppInMemory(false);
            var target = app.ContactServices.Addresses;

            var countBefore = target.CountOfAll();
            Assert.AreEqual(0, countBefore, "No Addresses should exist before the request for store address");

            // Target Store Address Should Not Exist
            Address actual;
            actual = target.FindStoreContactAddress();

            // Make sure we received an address
            Assert.IsNotNull(actual, "Actual address should not be null");
            Assert.AreNotEqual(string.Empty, actual.Bvin, "Bvin should be a valid value");

            // Make sure it was saved in the database
            var countAfter = target.CountOfAll();
            Assert.AreEqual(1, countAfter, "Request for store address should have created one if missing.");

            // Make sure we can update it
            actual.City = "My New City";
            Assert.IsTrue(target.Update(actual), "Update should be true");

            var actual2 = target.FindStoreContactAddress();
            Assert.IsNotNull(actual2, "Second request should return an address");
            Assert.AreEqual(actual.Bvin, actual2.Bvin, "Both request should return the same address");
            Assert.AreEqual("My New City", actual2.City, "City should have been updated.");
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