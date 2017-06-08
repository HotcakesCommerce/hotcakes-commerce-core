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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test
{
    /// <summary>
    ///     This is a test class for TextTest and is intended
    ///     to contain all TextTest Unit Tests
    /// </summary>
    [TestClass]
    public class TextTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for RemoveHtmlTags
        /// </summary>
        [TestMethod]
        public void RemoveHtmlTagsTest()
        {
            var input =
                "This<br/>Is<br />a test of the <b>html</b> stripper. <a href=\"something\" title\"test\">Link here</a>.";
            var expected = "This*Is*a test of the *html* stripper. *Link here*.";
            string actual;
            actual = Text.RemoveHtmlTags(input, "*");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemoveHtmlTagsTest2()
        {
            var input = "This</DIV>Is";
            var expected = "This*Is";
            string actual;
            actual = Text.RemoveHtmlTags(input, "*");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanSlugifyCorrectlyWithSlashes()
        {
            var input = @"shoes/nikes/awesome stuff/Shared Options.Tester_1";
            var expected = @"shoes/nikes/awesome-stuff/Shared-Options.Tester_1";
            var actual = Text.Slugify(input, true, true);
            Assert.AreEqual(actual, expected);
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