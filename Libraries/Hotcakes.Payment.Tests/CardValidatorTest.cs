#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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

namespace Hotcakes.Payment.Tests
{
    /// <summary>
    ///     This is a test class for CardValidatorTest and is intended
    ///     to contain all CardValidatorTest Unit Tests
    /// </summary>
    [TestClass]
    public class CardValidatorTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }
        
        /// <summary>
        ///     A test for GetCardTypeFromNumber
        /// </summary>
        [TestMethod]
        public void GetCardTypeFromNumberTest()
        {
            // Visa
            TestCard(CardType.Visa, "4111-1111-1111-1111");
            TestCard(CardType.Visa, "4222222222222");
            TestCard(CardType.Visa, "4012888888881881");
            TestCard(CardType.Visa, "4444 3333 2222 1111");
            TestCard(CardType.Visa, "4539442826535870");
            TestCard(CardType.Visa, "4716162844265541");
            TestCard(CardType.Visa, "4556888053402127");
            TestCard(CardType.Visa, "4716179512278630");
            TestCard(CardType.Visa, "4 485073192596545");
            TestCard(CardType.Visa, "4539668529847614");
            TestCard(CardType.Visa, "4539462228326744");
            TestCard(CardType.Visa, "4916555961657846");
            TestCard(CardType.Visa, "4532426 048232405");
            TestCard(CardType.Visa, "4024007144115762");

            // American Express
            TestCard(CardType.Amex, "378282246310005");
            TestCard(CardType.Amex, "371449635398431");
            TestCard(CardType.Amex, "378734493671000");
            TestCard(CardType.Amex, "341-1111-1111-1111");
            TestCard(CardType.Amex, "37 000000000000 2");

            // MasterCard
            TestCard(CardType.MasterCard, "5105105105105100");
            TestCard(CardType.MasterCard, "5555555555554444");
            TestCard(CardType.MasterCard, "5431-1111-1111-1111");
            TestCard(CardType.MasterCard, "5424 0000000000 15");
            TestCard(CardType.MasterCard, "5434530338525984");
            TestCard(CardType.MasterCard, "5499664153299995");
            TestCard(CardType.MasterCard, "5134664093528504");
            TestCard(CardType.MasterCard, "548674080410 3102");
            TestCard(CardType.MasterCard, "5408292810328294");
            TestCard(CardType.MasterCard, "5256555606637687");
            TestCard(CardType.MasterCard, "5110203121335612");
            TestCard(CardType.MasterCard, "5380846306970745");
            TestCard(CardType.MasterCard, "55 52383618336847");
            TestCard(CardType.MasterCard, "5395601672420384");

            // Diner's Club
            TestCard(CardType.DinersClub, "38520000023237");
            TestCard(CardType.DinersClub, "38520000023237");

            // Discover
            TestCard(CardType.Discover, "6011111111111117");
            TestCard(CardType.Discover, "6011000990139424");
            TestCard(CardType.Discover, "6011-6011-6011-6611");
            TestCard(CardType.Discover, "6011 0000000000 12");

            // JCB
            TestCard(CardType.JCB, "3530111333300000");
            TestCard(CardType.JCB, "3566002020360505");

            // Switch
            TestCard(CardType.Switch, "6331101999990016");
        }

        private void TestCard(CardType expected, string cardNumber)
        {
            var actual = CardValidator.GetCardTypeFromNumber(cardNumber);
            Assert.AreEqual(expected, actual, "Test failed for card " + cardNumber);
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