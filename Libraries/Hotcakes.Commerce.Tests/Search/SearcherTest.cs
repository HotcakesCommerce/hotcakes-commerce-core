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
    /// Summary description for SearchObjectIndex
    /// </summary>
    [TestClass]
    public class SearcherTest
    {
		public SearcherTest()
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
        public void CanAddObject()
        {
			SearchObject s = new SearchObject();
            s.ObjectType = 0;
            s.ObjectId = "1234";

			Lexicon lexicon = new Lexicon(new StaticLexicon());
			Searcher searcher = new Searcher(lexicon, new StaticSearcher());

            long id = searcher.ObjectIndexAddOrUpdate(s);

            Assert.IsTrue(id > 0);

        }

        [TestMethod]
        public void CanFindObjectById()
        {
			SearchObject s = new SearchObject();
            s.ObjectType = 0;
            s.ObjectId = "1234";

			Lexicon lexicon = new Lexicon(new StaticLexicon());
			Searcher searcher = new Searcher(lexicon, new StaticSearcher());

            long id = searcher.ObjectIndexAddOrUpdate(s);

            Assert.IsTrue(id > 0);

			SearchObject actual = searcher.ObjectIndexFind(id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(id, actual.Id);
            Assert.AreEqual(s.ObjectType, actual.ObjectType);
            Assert.AreEqual(s.ObjectId, actual.ObjectId);
        }

        [TestMethod]
        public void ReturnsNullWhenCantFindObject()
        {
			Lexicon lexicon = new Lexicon(new StaticLexicon());
			Searcher searcher = new Searcher(lexicon, new StaticSearcher());
			SearchObject actual = searcher.ObjectIndexFind(99);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void DoesntAddObjectsToIndexTwice()
        {
			SearchObject s = new SearchObject();
            s.ObjectType = 0;
            s.ObjectId = "1234";

			Lexicon lexicon = new Lexicon(new StaticLexicon());
			Searcher searcher = new Searcher(lexicon, new StaticSearcher());

            long id = searcher.ObjectIndexAddOrUpdate(s);

            Assert.IsTrue(id > 0);

            long secondId = searcher.ObjectIndexAddOrUpdate(s);

            Assert.AreEqual(id, secondId);
        }

        [TestMethod]
        public void CanDeleteFromIndex()
        {
			SearchObject s = new SearchObject();
            s.ObjectType = 0;
            s.ObjectId = "1234";

			Lexicon lexicon = new Lexicon(new StaticLexicon());
			Searcher searcher = new Searcher(lexicon, new StaticSearcher());

            long id = searcher.ObjectIndexAddOrUpdate(s);

            Assert.IsTrue(id > 0);

			SearchObject actual = searcher.ObjectIndexFind(id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(id, actual.Id);
            Assert.AreEqual(s.ObjectType, actual.ObjectType);
            Assert.AreEqual(s.ObjectId, actual.ObjectId);

            bool actual2 = searcher.ObjectIndexDelete(id);
            SearchObject existingAfterDelete = searcher.ObjectIndexFind(id);
            Assert.IsNull(existingAfterDelete);
        }

        [TestMethod]
        public void CanRecordObjectWord()
        {
			string culture = "en-US";

            // Setup 
			SearchObject s = new SearchObject();
            s.ObjectType = 0;
            s.ObjectId = "1234";

			Lexicon lexicon = new Lexicon(new StaticLexicon());
			long wordId = lexicon.AddOrCreateWord("thi", culture);
            Assert.IsTrue(wordId > 0);

			Searcher searcher = new Searcher(lexicon, new StaticSearcher());
            long id = searcher.ObjectIndexAddOrUpdate(s);
            Assert.IsTrue(id > 0);

            // Test
			SearchObjectWord w = new SearchObjectWord();
            w.SearchObjectId = id;
            w.WordId = wordId;
            w.Score = 1;

            bool actual = searcher.ObjectWordIndexUpdate(w);
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CanFindObjectsForWord()
        {
            long siteId = 0;
			string culture = "en-US";

            // Setup Words
			Lexicon lexicon = new Lexicon(new StaticLexicon());
			long thi_id = lexicon.AddOrCreateWord("thi", culture);
			long test_id = lexicon.AddOrCreateWord("test", culture);
			long sampl_id = lexicon.AddOrCreateWord("sampl", culture);

            // Setup Search Objects
			SearchObject s1 = new SearchObject();
            s1.ObjectType = 0;
            s1.ObjectId = "1";
			SearchObject s2 = new SearchObject();
            s2.ObjectType = 0;
            s2.ObjectId = "2";

            // Setup Searcher
			Searcher searcher = new Searcher(lexicon, new StaticSearcher());

            // Record Objects
            long sid1 = searcher.ObjectIndexAddOrUpdate(s1);
            long sid2 = searcher.ObjectIndexAddOrUpdate(s2);

            // Index Words for Objects
			SearchObjectWord w1_1 = new SearchObjectWord() { SearchObjectId = s1.Id, WordId = thi_id, Score = 1, SiteId = siteId };
			SearchObjectWord w1_2 = new SearchObjectWord() { SearchObjectId = s1.Id, WordId = test_id, Score = 1, SiteId = siteId };
			SearchObjectWord w2_1 = new SearchObjectWord() { SearchObjectId = s2.Id, WordId = thi_id, Score = 1, SiteId = siteId };
			SearchObjectWord w2_2 = new SearchObjectWord() { SearchObjectId = s2.Id, WordId = sampl_id, Score = 1, SiteId = siteId };
            searcher.ObjectWordIndexUpdate(w1_1);
            searcher.ObjectWordIndexUpdate(w1_2);
            searcher.ObjectWordIndexUpdate(w2_1);
            searcher.ObjectWordIndexUpdate(w2_2);

            // Test
            List<SearchObjectWord> actual = searcher.ObjectWordIndexFindByWordId(siteId, thi_id);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);

            List<SearchObjectWord> actual2 = searcher.ObjectWordIndexFindByWordId(siteId, test_id);
            Assert.IsNotNull(actual2);
            Assert.AreEqual(1, actual2.Count);

            List<SearchObjectWord> actual3 = searcher.ObjectWordIndexFindByWordId(siteId, 99);
            Assert.IsNotNull(actual3);
            Assert.AreEqual(0, actual3.Count);
        }

        [TestMethod]
        public void CanDoOneWordSearch()
        {
            long siteId = 0;
			string culture = "en-US";

            // Setup Objects
			Lexicon l = new Lexicon(new StaticLexicon());
			Searcher s = new Searcher(l, new StaticSearcher());
			SimpleTextIndexer indexer = new SimpleTextIndexer(s);

			indexer.Index(siteId, "1", 0, "Document Red", "Red is the first document. Red is a test.", culture);
			indexer.Index(siteId, "2", 0, "Document Blue", "Blue is the second document not Red like the first. Blue document is a sample", culture);
			indexer.Index(siteId, "3", 0, "Shakespeare: Julius Ceasar", "Et tu brute?", culture);
			indexer.Index(siteId, "4", 0, "Shakespeare: To Be or Not To Be", "To be or not to be", culture);
			indexer.Index(siteId, "5", 0, "Doc Brown", "The Flux Capacitor is what enables time travel. I fell off my toilet and hit my head. When I woke up, I drew this...", culture);

            string query = "document";
            
            int total = 0;
			List<SearchObject> results = s.DoSearch(query, 1, 10, culture, ref total);
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void CanDoTwoWordSearch()
        {
            long siteId = 0;
			string culture = "en-US";

            // Setup Objects
			Lexicon l = new Lexicon(new StaticLexicon());
			Searcher s = new Searcher(l, new StaticSearcher());
			SimpleTextIndexer indexer = new SimpleTextIndexer(s);

			indexer.Index(siteId, "1", 0, "Document Red", "Red is the first document. Red is a test.", culture);
			indexer.Index(siteId, "2", 0, "Document Blue", "Blue is the second document not Red like the first. Blue Blue document is a sample", culture);
			indexer.Index(siteId, "3", 0, "Shakespeare: Julius Ceasar", "Et tu brute?", culture);
			indexer.Index(siteId, "4", 0, "Shakespeare: To Be or Not To Be", "To be or not to be Red Red Red Red Red Red Red", culture);
			indexer.Index(siteId, "5", 0, "Doc Brown", "The Flux Capacitor is what enables time travel. I fell off my toilet and hit my head. When I woke up, I drew this...", culture);

            string query = "document blue";
            int total = 0;
			List<SearchObject> results = s.DoSearch(query, 1, 10, culture, ref total);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public void CanScoreDocumentsSearch()
        {
            long siteId = 0;
			string culture = "en-US";

            // Setup Objects
			Lexicon l = new Lexicon(new StaticLexicon());
			Searcher s = new Searcher(l, new StaticSearcher());
			SimpleTextIndexer indexer = new SimpleTextIndexer(s);

			indexer.Index(siteId, "1", 0, "Document Red", "Red is the first document. Red is a test.", culture);
			indexer.Index(siteId, "2", 0, "Document Blue", "Blue is the second document not Red like the first. Blue Blue document is a sample", culture);
			indexer.Index(siteId, "3", 0, "Shakespeare: Julius Ceasar", "Et tu brute?", culture);
			indexer.Index(siteId, "4", 0, "Shakespeare: To Be or Not To Be", "To be or not to be document Red Red Red Red Red Red Red", culture);
			indexer.Index(siteId, "5", 0, "Doc Brown", "The Flux Capacitor is what enables time travel. I fell off my toilet and hit my head. When I woke up, I drew this...", culture);

            string query = "document red";
            int total = 0;
            List<SearchObject> results = s.DoSearch(query, 1, 10, culture, ref total);
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(4, results[0].Id, "First Document should be #4");
            Assert.AreEqual(1, results[1].Id, "Second Document should be #1");
            Assert.AreEqual(2, results[2].Id, "Last Document should be #2");
        }

        [TestMethod]
        public void CanSearchSeparateSites()
        {
            long siteId = 99;
            long siteId2 = 1;
			string culture = "en-US";

            // Setup Objects
			Lexicon l = new Lexicon(new StaticLexicon());
			Searcher s = new Searcher(l, new StaticSearcher());
			SimpleTextIndexer indexer = new SimpleTextIndexer(s);

			indexer.Index(siteId, "1", 0, "Document Red", "Red is the first document. Red is a test.", culture);
			indexer.Index(siteId, "2", 0, "Document Blue", "Blue is the second document not Red like the first. Blue document is a sample", culture);
			indexer.Index(siteId, "3", 0, "Shakespeare: Julius Ceasar", "Et tu brute?", culture);
			indexer.Index(siteId2, "4", 0, "Shakespeare: To Be or Not To Be", "To be or not to be", culture);
			indexer.Index(siteId2, "5", 0, "Doc Brown", "The Flux Capacitor is what enables time travel. I fell off my toilet and hit my head. When I woke up, I drew this...", culture);

            string query = "Shakespeare";
            int total = 0;
            List<SearchObject> results = s.DoSearch(siteId, query, culture, 1, 10, ref total);
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("3", results[0].ObjectId);

            List<SearchObject> results2 = s.DoSearch(siteId2, query, culture, 1, 10, ref total);
            Assert.IsNotNull(results2);
            Assert.AreEqual(1, results2.Count);
            Assert.AreEqual("4", results2[0].ObjectId);
        }
    }
}
