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

using System;
using Hotcakes.Web.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test.Cryptography
{
    /// <summary>
    ///     This is a test class for AesEncryptionTest and is intended
    ///     to contain all AesEncryptionTest Unit Tests
    /// </summary>
    [TestClass]
    public class AesEncryptionTest
    {
        public const string StaticKey =
            "5C-0D-67-8A-47-9E-B4-A3-AD-12-4B-EB-B7-37-73-B3-98-DC-72-AF-25-93-04-1B-3D-85-2E-29-23-92-8D-E1";

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        private void EncodeDecode(string message)
        {
            EncodeDecode(message, KeyGenerator.Generate256bitKey());
        }

        private void EncodeDecode(string message, string key)
        {
            var encoded = AesEncryption.Encode(message, key);
            Assert.IsNotNull(encoded);
            Assert.AreNotEqual("", encoded);
            var decoded = AesEncryption.Decode(encoded, key);
            Assert.AreEqual(message, decoded);
        }

        [TestMethod]
        public void EncodeDecode1()
        {
            EncodeDecode("This is my message.", StaticKey);
        }

        [TestMethod]
        public void EncodeDecodeBlank()
        {
            EncodeDecode(string.Empty, StaticKey);
        }

        [TestMethod]
        public void EncodeDecodeSingleCharacter()
        {
            EncodeDecode("A", StaticKey);
        }

        [TestMethod]
        public void EncodeDecodeHighAscii()
        {
            EncodeDecode("הּ", StaticKey);
        }

        [TestMethod]
        public void EncodeDecodeWithRandomKey()
        {
            EncodeDecode("This is my message for a random key");
        }

        [TestMethod]
        public void EncodeDecodeWithBlankKey()
        {
            try
            {
                EncodeDecode("4111111111111111", string.Empty);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex, "Should have thrown an exception!");
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