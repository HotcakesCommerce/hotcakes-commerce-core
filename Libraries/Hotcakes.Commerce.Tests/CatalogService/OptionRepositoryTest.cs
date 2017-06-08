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
using Hotcakes.Commerce.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for OptionRepositoryTest and is intended
    ///     to contain all OptionRepositoryTest Unit Tests
    /// </summary>
    [TestClass]
    public class OptionRepositoryTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void Option_CanAddOptionsToProductInCorrectOrder()
        {
            var app = BaseTest.CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 342;

            var p = new Product();
            p.Sku = "TESTABC";
            p.ProductName = "Test Product ABC";


            var opt = new Option();
            opt.SetProcessor(OptionTypes.CheckBoxes);
            opt.Name = "Test Option A";
            opt.Items.Add(new OptionItem {Name = "Item A"});
            opt.Items.Add(new OptionItem {Name = "Item B"});

            var opt2 = new Option();
            opt.SetProcessor(OptionTypes.DropDownList);
            opt.Items.Add(new OptionItem {Name = "Choice One"});
            opt.Items.Add(new OptionItem {Name = "Choice Two"});

            var opt3 = Option.Factory(OptionTypes.Html);
            opt3.Name = "Option 3";

            var opt4 = Option.Factory(OptionTypes.Html);
            opt3.Name = "Option 4";

            var opt5 = Option.Factory(OptionTypes.Html);
            opt3.Name = "Option 5";

            var opt6 = Option.Factory(OptionTypes.Html);
            opt3.Name = "Option 6";

            var opt7 = Option.Factory(OptionTypes.Html);
            opt3.Name = "Option 7";


            // Add the option
            p.Options.Add(opt);
            p.Options.Add(opt2);
            p.Options.Add(opt3);
            p.Options.Add(opt4);
            p.Options.Add(opt5);
            p.Options.Add(opt6);
            p.Options.Add(opt7);

            Assert.IsTrue(app.CatalogServices.Products.Create(p));

            var actual = app.CatalogServices.Products.Find(p.Bvin);
            Assert.IsNotNull(actual, "Actual product should not be null");

            Assert.AreEqual(7, actual.Options.Count, "There should be one option on the product");

            Assert.AreEqual(opt.Name, actual.Options[0].Name, "Option name didn't match");
            Assert.AreEqual(opt2.Name, actual.Options[1].Name, "Option2 name didn't match");
            Assert.AreEqual(opt3.Name, actual.Options[2].Name, "Option3 name didn't match");
            Assert.AreEqual(opt4.Name, actual.Options[3].Name, "Option4 name didn't match");
            Assert.AreEqual(opt5.Name, actual.Options[4].Name, "Option5 name didn't match");
            Assert.AreEqual(opt6.Name, actual.Options[5].Name, "Option6 name didn't match");
            Assert.AreEqual(opt7.Name, actual.Options[6].Name, "Option7 name didn't match");
        }

        /// <summary>
        ///     A test for Create
        /// </summary>
        [TestMethod]
        public void Option_CanAddOptionsToProductWithItemsInCorrectOrder()
        {
            var app = BaseTest.CreateHccAppInMemory();
            app.CurrentRequestContext.CurrentStore = new Store();
            app.CurrentRequestContext.CurrentStore.Id = 342;

            var p = new Product();
            p.Sku = "TESTABC";
            p.ProductName = "Test Product ABC";

            var opt = new Option();
            opt.SetProcessor(OptionTypes.CheckBoxes);
            opt.Name = "Test Option";
            opt.Items.Add(new OptionItem {Name = "Item A"});
            opt.Items.Add(new OptionItem {Name = "Item B"});
            opt.Items.Add(new OptionItem {Name = "Alphabet City"});

            // Add the option
            p.Options.Add(opt);

            Assert.IsTrue(app.CatalogServices.Products.Create(p));

            var actual = app.CatalogServices.Products.Find(p.Bvin);
            Assert.IsNotNull(actual, "Actual product should not be null");

            Assert.AreEqual(1, actual.Options.Count, "There should be one option on the product");
            Assert.AreEqual(opt.Name, actual.Options[0].Name, "Option name didn't match");
            Assert.AreEqual(opt.OptionType, actual.Options[0].OptionType, "Option type was incorrect");

            Assert.AreEqual(3, actual.Options[0].Items.Count, "Item count on option should be 3");

            Assert.AreEqual(1, actual.Options[0].Items[0].SortOrder, "First sort order should be zero");
            Assert.AreEqual("Item A", actual.Options[0].Items[0].Name, "First Name Didn't Match");

            Assert.AreEqual(2, actual.Options[0].Items[1].SortOrder, "Second sort order should be zero");
            Assert.AreEqual("Item B", actual.Options[0].Items[1].Name, "Second Name Didn't Match");

            Assert.AreEqual(3, actual.Options[0].Items[2].SortOrder, "Third sort order should be zero");
            Assert.AreEqual("Alphabet City", actual.Options[0].Items[2].Name, "Third Name Didn't Match");
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