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
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Data.EF.Fakes;
using Hotcakes.Commerce.Utilities;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for HotcakesApplicationTest and is intended
    ///     to contain all HotcakesApplicationTest Unit Tests
    /// </summary>
    [TestClass]
    public class HotcakesApplicationTest : BaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        public static void InitShims()
        {
            ShimHccDbContext.AllInstances.AddObjectIndexNullableOfInt64NullableOfGuidNullableOfInt32StringStringString =
                (HccDbContext a0, long? a1, Guid? a2, int? a3, string a4, string a5, string a6) => { return 0; };
        }


        [TestMethod]
        public void HccApp_CanAddSampleProductsToStore()
        {
            using (ShimsContext.Create())
            {
                InitShims();

                var target = CreateHccAppInMemory(false);
                SampleData.AddSampleProductsToStore();

                var totalCount = target.CatalogServices.Products.CountOfAll();
                Assert.AreEqual(6, totalCount, "Six Products should have been created.");
                var cats = target.CatalogServices.Categories.FindAll();
                Assert.IsNotNull(cats);
                Assert.AreEqual(3, cats.Count, "Four categories should have been created!");
            }
        }

        //{

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

        //public void HccApp_DB_OrdesIntegrationTest()

        //[TestMethod()]
        //	var hccApp = HccAppHelper.InitHccApp();

        //	var orders = hccApp.OrderServices.Orders.FindAllPaged(1, 10);
        //}
    }
}