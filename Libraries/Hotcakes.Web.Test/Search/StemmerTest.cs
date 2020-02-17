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

using Hotcakes.Web.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test.Search
{
    /// <summary>
    ///     This is a test class for StemmerTest and is intended
    ///     to contain all StemmerTest Unit Tests
    /// </summary>
    [TestClass]
    public class StemmerTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///     A test for stem
        /// </summary>
        [TestMethod]
        public void stemTest()
        {
            TestWord("samples", "sampl");
            TestWord("sampl", "sampl");
            TestWord("sample", "sampl");
            TestWord("sampling", "sampl");
            TestWord("Façade", "façad");
        }

        private void TestWord(string raw, string expected)
        {
            var cultue = "en-US";
            var actual = WordProcessor.StemSingleWord(raw, cultue);
            Assert.AreEqual(expected, actual);
        }

        private void StopWordTest(string raw, bool expected)
        {
            var cultue = "en-US";
            var stemmed = WordProcessor.StemSingleWord(raw, cultue);
            var actual = WordProcessor.IsStopWord(stemmed, cultue);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     A test for IsStopWord
        /// </summary>
        [TestMethod]
        public void IsStopWordTest()
        {
            StopWordTest("it", true);
            StopWordTest("monkey", false);
            StopWordTest("this", true);
            StopWordTest("one", true);
            StopWordTest("two", false);
            StopWordTest("and", true);
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