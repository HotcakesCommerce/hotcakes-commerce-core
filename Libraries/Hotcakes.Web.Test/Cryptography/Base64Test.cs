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

using Hotcakes.Web.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test.Cryptography
{
    /// <summary>
    ///     This is a test class for Base64Test and is intended
    ///     to contain all Base64Test Unit Tests
    /// </summary>
    [TestClass]
    public class Base64Test
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }


        /// <summary>
        ///     A test for ConvertFromBase64
        /// </summary>
        [TestMethod]
        public void AutoTests()
        {
            TestBase64("This is my test string to encode !@#$%^", "VGhpcyBpcyBteSB0ZXN0IHN0cmluZyB0byBlbmNvZGUgIUAjJCVe");
            TestBase64(string.Empty, string.Empty);
            TestBase64("test", "dGVzdA==");
            TestBase64("!", "IQ==");
            TestBase64(" ", "IA==");
            TestBase64("E", "RQ==");
            TestBase64("e", "ZQ==");
        }

        [TestMethod]
        public void HighAsciiTest()
        {
            TestBase64("הּ", "76y0");
        }

        [TestMethod]
        public void SimpleTest()
        {
            TestBase64("AB 1", "QUIgMQ==");
        }

        private void TestBase64(string plaintext, string expectedEncoding)
        {
            var encoded = Base64.ConvertToBase64(Conversion.StringToBytes(plaintext));
            Assert.AreEqual(expectedEncoding, encoded, "Encoding did match expected value");

            var decodedBytes = Base64.ConvertFromBase64(expectedEncoding);
            var expectedDecodedBytes = Conversion.StringToBytes(plaintext);
            Assert.IsNotNull(decodedBytes);
            for (var i = 0; i < decodedBytes.Length; i++)
            {
                Assert.AreEqual(expectedDecodedBytes[i], decodedBytes[i], "a decoded byte didn't match");
            }
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