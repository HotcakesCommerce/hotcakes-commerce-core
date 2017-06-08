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

using System.Collections.Generic;
using System.Reflection;
using Hotcakes.Web.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test.Search
{
    /// <summary>
    ///     This is a test class for QueryParserTest and is intended
    ///     to contain all QueryParserTest Unit Tests
    /// </summary>
    [TestClass]
    public class QueryParserTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        private void TestPhrase(string raw, string expected)
        {
            var textParserAccessor = new PrivateObject(typeof (TextParser));
            var actual =
                (string)
                    textParserAccessor.Invoke("ReplaceNonAlphaNumeric", BindingFlags.Static | BindingFlags.NonPublic,
                        raw);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReplaceNonAlphaNumericTest()
        {
            TestPhrase("\"This is a test.\"",
                " this is a test  ");
            TestPhrase("This is a test.",
                "this is a test ");
            TestPhrase("This is a Façade.",
                "this is a façade ");
            TestPhrase("This \"is a\" TeSt_oNe.",
                "this  is a  test one ");
        }

        private void AssertListsAreEqual(List<string> expected, List<string> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i], "Word didn't match: " + actual[i] + " expected " + expected[i]);
            }
        }

        [TestMethod]
        public void CanParseQueries()
        {
            var input = "\"Red is a test.\"";
            var culture = "en-US";

            var expected = new List<string> {"red", "test"};

            var actual = TextParser.ParseText(input, culture);

            AssertListsAreEqual(expected, actual);
        }

        [TestMethod]
        public void CanParseQueriesWithoutStopWords()
        {
            var input = "\"Red  test.\"";
            var culture = "en-US";

            var expected = new List<string> {"red", "test"};

            var actual = TextParser.ParseText(input, culture);

            AssertListsAreEqual(expected, actual);
        }

        [TestMethod]
        public void CanParseQueriesWithUnderscores()
        {
            var input = "Red \"is a\" TeSt_oNe.";
            var culture = "en-US";

            var expected = new List<string> {"red", "test"};

            var actual = TextParser.ParseText(input, culture);

            AssertListsAreEqual(expected, actual);
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