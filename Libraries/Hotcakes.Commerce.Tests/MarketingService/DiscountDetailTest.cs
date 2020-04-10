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
using System.Collections.Generic;
using Hotcakes.Commerce.Marketing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for DiscountDetailTest and is intended
    ///     to contain all DiscountDetailTest Unit Tests
    /// </summary>
    [TestClass]
    public class DiscountDetailTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for ListToXml
        /// </summary>
        [TestMethod]
        public void Discount_CanCreateXmlFromDiscountDetailList()
        {
            var details = new List<DiscountDetail>();
            var d1 = new DiscountDetail {Description = "Hello, World", Amount = -1.56m, PromotionId = 10, ActionId = 11};
            details.Add(d1);
            var d2 = new DiscountDetail
            {
                Description = "Cool Item Two",
                Amount = -1.10m,
                PromotionId = 20,
                ActionId = 22
            };
            details.Add(d2);

            var expected = "<DiscountDetails>" + Environment.NewLine;

            expected += "  <DiscountDetail>" + Environment.NewLine;
            expected += "    <Id>" + d1.Id + "</Id>" + Environment.NewLine;
            expected += "    <Description>Hello, World</Description>" + Environment.NewLine;
            expected += "    <Amount>-1.56</Amount>" + Environment.NewLine;
            expected += "    <DiscountType>0</DiscountType>" + Environment.NewLine;
            expected += "    <PromotionId>" + d1.PromotionId + "</PromotionId>" + Environment.NewLine;
            expected += "    <ActionId>" + d1.ActionId + "</ActionId>" + Environment.NewLine;
            expected += "  </DiscountDetail>" + Environment.NewLine;

            expected += "  <DiscountDetail>" + Environment.NewLine;
            expected += "    <Id>" + d2.Id + "</Id>" + Environment.NewLine;
            expected += "    <Description>Cool Item Two</Description>" + Environment.NewLine;
            expected += "    <Amount>-1.10</Amount>" + Environment.NewLine;
            expected += "    <DiscountType>0</DiscountType>" + Environment.NewLine;
            expected += "    <PromotionId>" + d2.PromotionId + "</PromotionId>" + Environment.NewLine;
            expected += "    <ActionId>" + d2.ActionId + "</ActionId>" + Environment.NewLine;
            expected += "  </DiscountDetail>" + Environment.NewLine;

            expected += "</DiscountDetails>";

            string actual;
            actual = DiscountDetail.ListToXml(details);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     A test for ListToXml
        /// </summary>
        [TestMethod]
        public void Discount_CanCreateXmlFromDiscountDetailListWhenEmpty()
        {
            var details = new List<DiscountDetail>();

            var expected = "<DiscountDetails />";

            string actual;
            actual = DiscountDetail.ListToXml(details);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     A test for ListFromXml
        /// </summary>
        [TestMethod]
        public void Discount_CanReadDiscountDetailListFromXml()
        {
            var details = new List<DiscountDetail>();
            var d1 = new DiscountDetail {Description = "Hello, World", Amount = -1.56m};
            details.Add(d1);
            var d2 = new DiscountDetail {Description = "Cool Item Two", Amount = -1.10m};
            details.Add(d2);
            var xml = DiscountDetail.ListToXml(details);

            List<DiscountDetail> actual;
            actual = DiscountDetail.ListFromXml(xml);

            Assert.AreEqual(details.Count, actual.Count, "Count of items didn't match");
            for (var i = 0; i < details.Count; i++)
            {
                Assert.AreEqual(details[i].Amount, actual[i].Amount, "Amount Didn't Match");
                Assert.AreEqual(details[i].Description, actual[i].Description, "Description Didn't Match");
                Assert.AreEqual(details[i].Id, actual[i].Id, "Id Didn't Match");
            }
        }

        /// <summary>
        ///     A test for ListFromXml
        /// </summary>
        [TestMethod]
        public void Discount_CanReadDiscountDetailListFromXmlWhenEmpty()
        {
            var xml = string.Empty;

            List<DiscountDetail> actual;
            actual = DiscountDetail.ListFromXml(xml);

            Assert.IsNotNull(actual, "List should not be null after reading.");
            Assert.AreEqual(actual.Count, 0, "Actual count should be zero");
        }

        /// <summary>
        ///     A test for ListFromXml
        /// </summary>
        [TestMethod]
        public void Discount_CanReadDiscountDetailListFromXmlWhenNoElementsInside()
        {
            var xml = "<DiscountDetails />";

            List<DiscountDetail> actual;
            actual = DiscountDetail.ListFromXml(xml);

            Assert.IsNotNull(actual, "List should not be null after reading.");
            Assert.AreEqual(actual.Count, 0, "Actual count should be zero");
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