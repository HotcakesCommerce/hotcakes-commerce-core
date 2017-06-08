using Hotcakes.Commerce.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests.Search
{
    /// <summary>
    /// Summary description for WordLibrary
    /// </summary>
    [TestClass]
    public class LexiconTest
    {
		public LexiconTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CanAddAWord()
        {
            string input = "thi";
			string culture = "en-US";

			Lexicon lex = new Lexicon(new StaticLexicon());

			long id = lex.AddOrCreateWord(input, culture);

            Assert.IsTrue(id > 0);
        }

        [TestMethod]
        public void CanFindWord()
        {
            string input = "thi";
			string culture = "en-US";

			Lexicon lex = new Lexicon(new StaticLexicon());
			long expected = lex.AddOrCreateWord(input, culture);
            Assert.IsTrue(expected > 0);

			long actual = lex.FindWordId(input, culture);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CanShowWordIsMissing()
        {
            string input = "thi";
			string culture = "en-US";

			Lexicon lex = new Lexicon(new StaticLexicon());

			long actual = lex.FindWordId(input, culture);
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void CanFindTwoWords()
        {
            string input = "thi";
            string input2 = "test";
			string culture = "en-US";

			Lexicon lex = new Lexicon(new StaticLexicon());
			long expected = lex.AddOrCreateWord(input, culture);
            Assert.IsTrue(expected > 0);
			long expected2 = lex.AddOrCreateWord(input2, culture);
            Assert.IsTrue(expected2 > 0);

			long actual2 = lex.FindWordId(input2, culture);
            Assert.AreEqual(expected2, actual2);

			long actual = lex.FindWordId(input, culture);
            Assert.AreEqual(expected, actual);
        }
    }
}
