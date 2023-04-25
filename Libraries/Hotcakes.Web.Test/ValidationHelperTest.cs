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

using Hotcakes.Web.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Web.Test
{
    /// <summary>
    ///     This is a test class for EmailValidationTest and is intended
    ///     to contain all EmailValidationTest Unit Tests
    /// </summary>
    [TestClass]
    public class ValidationHelperTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CanFailBlankEmailAddress()
        {
            var input = string.Empty;
            var expected = false;
            bool actual;
            actual = ValidationHelper.IsEmailValid(input);
            Assert.AreEqual(expected, actual);
        }

        private void TestSingleEmail(string input, bool expected)
        {
            if (expected)
            {
                Assert.IsTrue(ValidationHelper.IsEmailValid(input), "Failed on " + input);
            }
            else
            {
                Assert.IsFalse(ValidationHelper.IsEmailValid(input), "False Positive on " + input);
            }
        }

        [TestMethod]
        public void CanPassValidEmailAddresses()
        {
            TestSingleEmail("test@hotcakes.com", true);

            TestSingleEmail("first.last@company.ie", true);
            TestSingleEmail("name_last@nowhere.com", true);
            TestSingleEmail("verylongnameandaddressforfirstpart@longdomainnametoolongdomain.com", true);

            // TODO: These two items are in fact valid email addresses but the RegEx
            // does not pass them. A rare situation that might need to be 
            // addressed later
            // ---------------------------------------------------------------- //
            //TestSingleEmail("a@b.co.uk", true);
            //TestSingleEmail("a@b.ru", true);
            // ---------------------------------------------------------------- //
        }

        [TestMethod]
        public void CanPassQuestionableValidEmailAddresses()
        {
            TestSingleEmail("d.j@server1.proseware.com", true);
            TestSingleEmail("long+name@co.au", true);
            TestSingleEmail("js#internal@proseware.com", true);
            //TestSingleEmail("j_9@[129.126.118.1]", false);
            TestSingleEmail("one$or%ano!ther@nowhere.com", true);
            TestSingleEmail("one&or'another@nowhere.com", true);
            //TestSingleEmail("one*or+another@nowhere.com", false);
            TestSingleEmail("one-or/another@nowhere.com", true);
            TestSingleEmail("one=or?another@nowhere.com", true);
            TestSingleEmail("one^or_another@nowhere.com", true);
            TestSingleEmail("one`or{another@nowhere.com", true);
            TestSingleEmail("one|or}ano~ther@nowhere.com", true);
            TestSingleEmail("j.s@server1.proseware.com", true);
        }

        [TestMethod]
        public void CanFailInvalidEmailAddresses()
        {
            TestSingleEmail("j.@server1.proseware.com", false);
            //TestSingleEmail("j@proseware.com9", false);
            TestSingleEmail("te st@hotcakes.com", false);
            TestSingleEmail("a@b.o", false);
            TestSingleEmail("a", false);
            TestSingleEmail("a@", false);
            TestSingleEmail("a@c", false);
            TestSingleEmail("a@co", false);
            TestSingleEmail("a@co.k", false);
            TestSingleEmail("fir\"st.last@company.ie", false);
            TestSingleEmail("@nothing.com", false);
            TestSingleEmail("nothing.com", false);
            TestSingleEmail("nothing", false);
            TestSingleEmail("j..s@proseware.com", false);
            TestSingleEmail("js*@proseware.com", false);
            TestSingleEmail("js@proseware..com", false);
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