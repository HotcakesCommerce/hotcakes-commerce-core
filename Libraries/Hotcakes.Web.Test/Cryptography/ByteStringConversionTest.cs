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
    ///     This is a test class for ByteStringConversionTest and is intended
    ///     to contain all ByteStringConversionTest Unit Tests
    /// </summary>
    [TestClass]
    public class ByteStringConversionTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        private void ConvertAndBack(string input)
        {
            var converted = Conversion.StringToBytes(input);
            var decoded = Conversion.BytesToString(converted);
            Assert.AreEqual(input, decoded);
        }

        [TestMethod]
        public void ConvertToByteArrayTest()
        {
            var input = "Ǝ"; //&#398
            ConvertAndBack(input);
        }

        [TestMethod]
        public void ConvertToByteArrayTest2()
        {
            ConvertAndBack("AB 1");
        }

        [TestMethod]
        public void BunchOfInput()
        {
            ConvertAndBack("");
            ConvertAndBack("!@#$%^&*()_+=-<>.,/?';:\"{[]}\\|");
            ConvertAndBack(" ");
            ConvertAndBack("My name is test guy!");
            ConvertAndBack("The quick brown fox jumped over the lazy yellow dog.");
            ConvertAndBack("1234567890`~");
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