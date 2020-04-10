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

using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Orders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for LineItemTest and is intended
    ///     to contain all LineItemTest Unit Tests
    /// </summary>
    [TestClass]
    public class LineItemTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for LineTotal
        /// </summary>
        /// <summary>
        ///     A test for LineTotal
        /// </summary>
        [TestMethod]
        public void LineItem_CanGetLineTotalWithoutDiscounts()
        {
            var target = new LineItem();
            target.BasePricePerItem = 39.99m;
            target.DiscountDetails.Add(new DiscountDetail {Amount = -20.01m});
            target.DiscountDetails.Add(new DiscountDetail {Amount = -5m});

            var actual = target.LineTotalWithoutDiscounts;

            Assert.AreEqual(39.99m, actual, "Total was not correct.");
        }

        ///A test for LineTotal

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

        /// <summary>
        ///</summary>
        //[TestMethod()]
        //public void LineItem_CanGetLineTotalWhenNoDiscounts()
        //{
        //	LineItem target = new LineItem();
        //	target.BasePricePerItem = 39.99m;
        //	Decimal actual = target.LineTotal;

        //	Assert.AreEqual(39.99m, actual, "Total was not correct.");
        //}

        /// <summary>
        ///A test for LineTotal
        ///</summary>
        //[TestMethod()]
        //public void LineItem_CanGetLineTotalWithDiscountsAndMultipleQuantity()
        //{
        //	LineItem target = new LineItem();
        //	target.BasePricePerItem = 39.99m;
        //	target.Quantity = 3;
        //	target.DiscountDetails.Add(new DiscountDetail() { Amount = -18m });
        //	target.DiscountDetails.Add(new DiscountDetail() { Amount = -6m });

        //	Decimal actual = target.LineTotal;

        //	decimal expected = 39.99m * 3;
        //	expected -= 18m;
        //	expected -= 6m;

        //	Assert.AreEqual(expected, actual, "Total was not correct.");

        //}

        /// <summary>
        ///A test for LineTotal
        ///</summary>
        //[TestMethod()]
        //public void LineItem_CanGetAdjustedPerItemPrice()
        //{
        //	LineItem target = new LineItem();
        //	target.BasePricePerItem = 39.99m;
        //	target.Quantity = 3;
        //	target.DiscountDetails.Add(new DiscountDetail() { Amount = -18m });
        //	target.DiscountDetails.Add(new DiscountDetail() { Amount = -6m });

        //	Decimal actual = target.AdjustedPricePerItem;

        //	// 39.99 * 3 = 119.97
        //	// -18       = 101.97
        //	// -6        =  95.97
        //	// divided by three = 31.99;
        //	decimal expected = 31.99m;

        //	Assert.AreEqual(expected, actual, "Total was not correct.");

        //}
    }
}