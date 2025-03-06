#region License

// Distributed under the MIT License
// ============================================================
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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hotcakes.Web.Search;
using Hotcakes.Commerce.Search;

namespace Hotcakes.Commerce.Tests.Search
{
    /// <summary>
    /// Summary description for SimpleIndexer
    /// </summary>
    [TestClass]
    public class SimpleTextIndexer
    {
        public SimpleTextIndexer()
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
        public void CanIndexAnObject()
        {
			string culture = "en-US";
            string message = "Red is a sample or red is a test.";

			Lexicon l = new Lexicon(new StaticLexicon());
			Searcher s = new Searcher(l, new StaticSearcher());

			SimpleTextIndexer indexer = new SimpleTextIndexer(s);

			indexer.Index(0, "1234", 0, "", message, culture);

            SearchObject actual = s.ObjectIndexFindByTypeAndId(0, 0, "1234");
            Assert.IsNotNull(actual);

            List<SearchObjectWord> words = s.ObjectWordIndexFindAll();
            Assert.IsNotNull(words);

            List<SearchObjectWord> expected = new List<SearchObjectWord>();
			expected.Add(new SearchObjectWord() { SearchObjectId = actual.Id, WordId = l.FindWordId("red", culture), Score = 2 });
			expected.Add(new SearchObjectWord() { SearchObjectId = actual.Id, WordId = l.FindWordId("sampl", culture), Score = 1 });
			expected.Add(new SearchObjectWord() { SearchObjectId = actual.Id, WordId = l.FindWordId("test", culture), Score = 1 });

            Assert.AreEqual(expected.Count, words.Count);
            Assert.AreEqual(expected[0].WordId, words[0].WordId);
            Assert.AreEqual(expected[1].WordId, words[1].WordId);
            Assert.AreEqual(expected[2].WordId, words[2].WordId);
            Assert.AreEqual(expected[0].Score, words[0].Score);
            Assert.AreEqual(expected[1].Score, words[1].Score);
            Assert.AreEqual(expected[2].Score, words[2].Score);

        }
    }
}
