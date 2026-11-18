#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Stores search data in process-managed collections rather than in the SQL search tables.
    /// </summary>
    /// <remarks>
    /// This provider is primarily a lightweight or test implementation. Lexicon and object identifiers
    /// are generated in memory starting at one, and the collections are not durable or shared with SQL.
    /// Basic object searches differ from <see cref="SqlSearchProvider"/> in their site sentinel, required
    /// word matching, and score ordering. Advanced product-search operations and composite object indexing
    /// are intentionally unsupported and throw <see cref="NotImplementedException"/>.
    /// </remarks>
    [Serializable]
    public class StaticSearchProvider : ISearchProvider
    {
        private readonly List<SearchObject> objects = new List<SearchObject>();
        private readonly List<SearchObjectWord> objectWords = new List<SearchObjectWord>();
        private readonly List<LexiconWord> words = new List<LexiconWord>();

        #region Lexicon operations

        /// <summary>
        /// Finds an in-memory lexicon identifier for a stemmed word and culture.
        /// </summary>
        /// <param name="stemmedWord">The stemmed word to find.</param>
        /// <param name="culture">The culture to match.</param>
        /// <returns>The matching identifier, or the sentinel value <c>0</c> when not found.</returns>
        public long FindWordId(string stemmedWord, string culture)
        {
            var word = words
                .Where(w => w.Culture == culture && w.Word == stemmedWord)
                .Select(w => w.Id)
                .FirstOrDefault();

            if (word < 1)
            {
                return 0;
            }
            return word;
        }

        /// <summary>
        /// Adds a stemmed word to the in-memory lexicon and assigns the next identifier.
        /// </summary>
        /// <param name="stemmedWord">The stemmed word to store.</param>
        /// <param name="culture">The culture associated with the word.</param>
        /// <returns>The generated in-memory identifier.</returns>
        /// <remarks>Identifiers are derived from the current maximum and are not database identifiers.</remarks>
        public long InsertWord(string stemmedWord, string culture)
        {
            long max = 0;
            if (words.Count > 0)
            {
                var max2 = words.Max(w => w.Id);
                max = max2;
                max += 1;
            }
            else
            {
                max = 1;
            }

            words.Add(new LexiconWord {Id = max, Word = stemmedWord, Culture = culture});

            return max;
        }

        /// <summary>
        /// Finds in-memory lexicon identifiers for the supplied stemmed words and culture.
        /// </summary>
        /// <param name="stemmedWords">The stemmed words to match.</param>
        /// <param name="culture">The culture to match.</param>
        /// <returns>The matching identifiers, in the order produced by the in-memory query.</returns>
        public List<long> FindAllWordIds(List<string> stemmedWords, string culture)
        {
            var result = new List<long>();

            var selectedWords = words
                .Where(w => w.Culture == culture && stemmedWords.Contains(w.Word))
                .Select(w => w.Id)
                .ToList();

            if (selectedWords != null)
            {
                foreach (var word in selectedWords)
                {
                    result.Add(word);
                }
            }

            return result;
        }

        #endregion

        #region Search Object operations

        /// <summary>
        /// Adds a search object to the in-memory object index and assigns the next identifier to it.
        /// </summary>
        /// <param name="s">The object to add.</param>
        /// <returns>The generated in-memory identifier.</returns>
        /// <remarks>The supplied object instance is retained in the in-memory collection.</remarks>
        public long ObjectIndexInsert(SearchObject s)
        {
            long max = 0;
            if (objects.Count > 0)
            {
                var max2 = (from o in objects
                    select o.Id).Max();
                max = max2;
                max += 1;
            }
            else
            {
                max = 1;
            }

            s.Id = max;
            objects.Add(s);

            return max;
        }

        /// <summary>
        /// Finds an in-memory search object by identifier.
        /// </summary>
        /// <param name="id">The in-memory object identifier.</param>
        /// <returns>The retained object, or <c>null</c> when no object matches.</returns>
        public SearchObject ObjectIndexFind(long id)
        {
            SearchObject result = null;

            var o = (from ob in objects
                where ob.Id == id
                select ob).FirstOrDefault();

            if (o != null)
            {
                return o;
            }

            return result;
        }

        /// <summary>
        /// Finds in-memory objects whose identifiers occur in the supplied list.
        /// </summary>
        /// <param name="ids">The object identifiers to find.</param>
        /// <returns>Copied search objects, or <c>null</c> if the method's existing broad exception handler catches an error.</returns>
        /// <remarks>The returned copies do not copy every field retained by the original object.</remarks>
        public List<SearchObject> ObjectIndexFindAllInList(List<long> ids)
        {
            var result = new List<SearchObject>();

            try
            {
                var o = (from ob in objects
                    where ids.Contains(ob.Id)
                    select ob).ToList();
                if (o != null)
                {
                    foreach (var ob in o)
                    {
                        var so = new SearchObject();
                        so.Id = ob.Id;
                        so.ObjectId = ob.ObjectId;
                        so.ObjectType = ob.ObjectType;
                        so.Title = ob.Title;
                        so.SiteId = ob.SiteId;
                        result.Add(so);
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Finds an in-memory object by site, type, and application object identifier.
        /// </summary>
        /// <param name="siteId">The site identifier.</param>
        /// <param name="type">The search object type.</param>
        /// <param name="objectId">The application object identifier.</param>
        /// <returns>The matching retained object, or <c>null</c> when none exists.</returns>
        public SearchObject ObjectIndexFindByTypeAndId(long siteId, int type, Guid objectId)
        {
            var o = (from ob in objects
                where ob.ObjectType == type &&
                      ob.ObjectId == objectId &&
                      ob.SiteId == siteId
                select ob).FirstOrDefault();

            if (o != null)
            {
                return o;
            }

            return null;
        }

        /// <summary>
        /// Determines whether an in-memory object exists for the supplied site, type, and identifier.
        /// </summary>
        /// <param name="siteId">The site identifier.</param>
        /// <param name="type">The search object type.</param>
        /// <param name="objectId">The application object identifier.</param>
        /// <returns><c>true</c> when a matching object exists; otherwise, <c>false</c>.</returns>
        public bool ObjectIndexObjectExists(long siteId, int type, Guid objectId)
        {
            var o = ObjectIndexFindByTypeAndId(siteId, type, objectId);
            if (o != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an in-memory search object by identifier.
        /// </summary>
        /// <param name="id">The object identifier.</param>
        /// <returns><c>true</c> when an object was removed; otherwise, <c>false</c>.</returns>
        public bool ObjectIndexDelete(long id)
        {
            var s = ObjectIndexFind(id);
            if (s != null)
            {
                objects.Remove(s);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Represents the composite object-and-word indexing operation, which this provider does not implement.
        /// </summary>
        /// <param name="siteId">The site identifier.</param>
        /// <param name="objectId">The application object identifier.</param>
        /// <param name="objectType">The search object type.</param>
        /// <param name="title">The object title.</param>
        /// <param name="wordScores">The word-to-score map used by SQL providers.</param>
        /// <param name="culture">The indexed content culture.</param>
        /// <exception cref="NotImplementedException">Always thrown by this provider.</exception>
        public void AddObjectIndex(long siteId, Guid objectId, int objectType, string title,
            Dictionary<string, int> wordScores, string culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Search Object Word operations

        /// <summary>
        /// Removes the first matching in-memory object-word association.
        /// </summary>
        /// <param name="w">The association identified by search-object and word identifiers.</param>
        /// <returns>Always <c>true</c>, including when no matching association exists.</returns>
        /// <remarks>This sentinel behavior is retained for compatibility with the static provider.</remarks>
        public bool ObjectWordIndexDelete(SearchObjectWord w)
        {
            var d = (from wd in objectWords
                where wd.SearchObjectId == w.SearchObjectId &&
                      wd.WordId == w.WordId
                select wd).FirstOrDefault();
            if (d != null)
            {
                objectWords.Remove(d);
            }

            return true;
        }

        /// <summary>
        /// Adds an object-word association to the in-memory collection.
        /// </summary>
        /// <param name="w">The association, including its score and site identifier.</param>
        /// <returns>Always <c>true</c> after adding the association.</returns>
        public bool ObjectWordIndexInsert(SearchObjectWord w)
        {
            objectWords.Add(w);
            return true;
        }

        #endregion

        #region Search operations

        /// <summary>
        /// Searches the in-memory object-word index without site restriction.
        /// </summary>
        /// <param name="wordIds">The word identifiers that must match.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of objects to return.</param>
        /// <param name="totalResults">Receives the total number of objects matching all requested words.</param>
        /// <returns>The paged matching objects, ordered by descending summed score.</returns>
        /// <remarks>This overload uses the sentinel site value <c>0</c>; SQL uses <c>-1</c> for its equivalent overload.</remarks>
        public List<SearchObject> DoSearch(List<long> wordIds, int pageNumber, int pageSize, ref int totalResults)
        {
            return DoSearchBySite(0, wordIds, pageNumber, pageSize, ref totalResults);
        }

        /// <summary>
        /// Searches in-memory object-word associations, optionally restricting matches to a site.
        /// </summary>
        /// <param name="siteId">The site identifier; values greater than zero restrict associations to that site.</param>
        /// <param name="wordIds">The word identifiers that must all be represented for an object to match.</param>
        /// <param name="pageNumber">The one-based page number; negative offsets are clamped to zero.</param>
        /// <param name="pageSize">The maximum number of objects to return.</param>
        /// <param name="totalResults">Receives the total number of objects matching all requested words.</param>
        /// <returns>Matching objects ordered by descending summed score.</returns>
        /// <remarks>Unlike the SQL basic search, the in-memory query filters groups whose count is less than the number of requested words.</remarks>
        public List<SearchObject> DoSearchBySite(long siteId, List<long> wordIds, int pageNumber, int pageSize,
            ref int totalResults)
        {
            var results = new List<SearchObject>();

            var skip = (pageNumber - 1)*pageSize;
            if (skip < 0) skip = 0;

            if (siteId > 0)
            {
                var step1a = (from w in objectWords
                    where wordIds.Contains(w.WordId) &&
                          w.SiteId == siteId
                    group w by w.SearchObjectId
                    into g
                    select new {ObjectId = g.Key, Score = g.Sum(y => y.Score), Count = g.Sum(y => 1)})
                    .Where(y => y.Count >= wordIds.Count).OrderByDescending(y => y.Score);
                totalResults = step1a.Count();

                var step1 = step1a.Skip(skip).Take(pageSize).ToList();
                foreach (var s in step1)
                {
                    results.Add(ObjectIndexFind(s.ObjectId));
                }
            }
            else
            {
                var step1a = (from w in objectWords
                    where wordIds.Contains(w.WordId)
                    group w by w.SearchObjectId
                    into g
                    select new {ObjectId = g.Key, Score = g.Sum(y => y.Score), Count = g.Sum(y => 1)})
                    .Where(y => y.Count >= wordIds.Count).OrderByDescending(y => y.Score);
                totalResults = step1a.Count();
                var step1 = step1a.Skip(skip).Take(pageSize).ToList();

                foreach (var s in step1)
                {
                    results.Add(ObjectIndexFind(s.ObjectId));
                }
            }

            return results;
        }

        /// <summary>
        /// Represents an unsupported advanced product search in the static provider.
        /// </summary>
        /// <param name="wordIds">The word identifiers that would be matched.</param>
        /// <param name="query">The product search query.</param>
        /// <param name="pageNumber">The requested one-based page number.</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <exception cref="NotImplementedException">Always thrown by this provider.</exception>
        public ProductSearchResultAdv DoSearch(List<long> wordIds, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Represents an unsupported site-aware advanced product search in the static provider.
        /// </summary>
        /// <param name="siteId">The site identifier that would restrict the search.</param>
        /// <param name="wordIds">The word identifiers that would be matched.</param>
        /// <param name="query">The product search query.</param>
        /// <param name="pageNumber">The requested one-based page number.</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <exception cref="NotImplementedException">Always thrown by this provider.</exception>
        public ProductSearchResultAdv DoSearchBySite(long siteId, List<long> wordIds, ProductSearchQueryAdv query,
            int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Represents an unsupported advanced product search without explicit word identifiers.
        /// </summary>
        /// <param name="siteId">The site identifier that would restrict the search.</param>
        /// <param name="query">The product search query.</param>
        /// <param name="pageNumber">The requested one-based page number.</param>
        /// <param name="pageSize">The requested page size.</param>
        /// <exception cref="NotImplementedException">Always thrown by this provider.</exception>
        public ProductSearchResultAdv DoSearchBySite(long siteId, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
