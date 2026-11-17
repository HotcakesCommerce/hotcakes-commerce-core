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
using Hotcakes.Web.Search;

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Provides the public search facade and delegates storage and execution to an <see cref="ISearchProvider"/>.
    /// </summary>
    /// <remarks>
    /// This class preserves the provider contract while handling query parsing and the all-site sentinel.
    /// For object searches, a positive site identifier selects site-scoped execution; zero or a negative site
    /// identifier selects the provider's all-site overload. Product searches with blank or whitespace-only text
    /// deliberately use the provider overload without word IDs and pass the site value through unchanged.
    /// </remarks>
    public class Searcher
    {
        private readonly ISearchProvider provider;

        /// <summary>
        /// Initializes a search facade with the provider used for all search operations.
        /// </summary>
        /// <param name="p">The provider that persists the lexicon and index and executes searches.</param>
        public Searcher(ISearchProvider p)
        {
            provider = p;
        }

        #region Search Object Word operations

        /// <summary>
        /// Delegates insertion of an indexed-object/lexicon-word association to the provider.
        /// </summary>
        /// <param name="w">The association, including object, site, word, and score.</param>
        /// <returns>The provider's success value.</returns>
        public bool ObjectWordIndexInsert(SearchObjectWord w)
        {
            return provider.ObjectWordIndexInsert(w);
        }

        #endregion

        #region Lexicon operations

        /// <summary>
        /// Returns an existing lexicon entry or creates one when the word is not present.
        /// </summary>
        /// <param name="stemmedWord">The normalized, stemmed word.</param>
        /// <param name="culture">The culture-specific lexicon to use.</param>
        /// <returns>The existing or newly inserted lexicon identifier.</returns>
        /// <remarks>
        /// The provider is queried first. Any identifier greater than zero is reused; otherwise the provider's
        /// insert operation is called. The operation does not add a separate synchronization or duplicate check.
        /// </remarks>
        public long AddOrCreateWord(string stemmedWord, string culture)
        {
            var id = provider.FindWordId(stemmedWord, culture);
            if (id > 0)
            {
                return id;
            }
            return provider.InsertWord(stemmedWord, culture);
        }

        /// <summary>
        /// Delegates lookup of one stemmed word in a culture-specific lexicon.
        /// </summary>
        /// <param name="stemmedWord">The normalized, stemmed word.</param>
        /// <param name="culture">The culture-specific lexicon to use.</param>
        /// <returns>The provider's lexicon identifier or not-found value.</returns>
        public long FindWordId(string stemmedWord, string culture)
        {
            return provider.FindWordId(stemmedWord, culture);
        }

        /// <summary>
        /// Delegates lookup of multiple stemmed words in a culture-specific lexicon.
        /// </summary>
        /// <param name="stemmedWords">The normalized, stemmed words.</param>
        /// <param name="culture">The culture-specific lexicon to use.</param>
        /// <returns>The provider's matching lexicon identifiers; a provider <see langword="null"/> result is not changed here.</returns>
        public List<long> FindAllWordIds(List<string> stemmedWords, string culture)
        {
            return provider.FindAllWordIds(stemmedWords, culture);
        }

        #endregion

        #region Search Object operations

        /// <summary>
        /// Replaces the indexed object for the same site, type, and source identifier, then inserts the supplied object.
        /// </summary>
        /// <param name="s">The object record to add or use as the replacement.</param>
        /// <returns>The identifier returned by the provider for the inserted object.</returns>
        /// <remarks>
        /// When a matching object exists, it is deleted before insertion. This preserves the existing replacement
        /// sequence and does not make the two provider calls transactional at this facade level.
        /// </remarks>
        public long ObjectIndexAddOrUpdate(SearchObject s)
        {
            var existing = provider.ObjectIndexFindByTypeAndId(s.SiteId, s.ObjectType, s.ObjectId);
            if (existing != null)
            {
                ObjectIndexDelete(existing.Id);
            }
            return provider.ObjectIndexInsert(s);
        }

        /// <summary>
        /// Delegates lookup of an indexed object by its search-object identifier.
        /// </summary>
        /// <param name="id">The search-object identifier.</param>
        /// <returns>The matching object, or the provider's not-found value.</returns>
        public SearchObject ObjectIndexFind(long id)
        {
            return provider.ObjectIndexFind(id);
        }

        /// <summary>
        /// Delegates lookup of an indexed object by site, type, and source identifier.
        /// </summary>
        /// <param name="siteId">The site that owns the object.</param>
        /// <param name="type">The provider-defined object type.</param>
        /// <param name="objectId">The source object's identifier.</param>
        /// <returns>The matching object, or the provider's not-found value.</returns>
        public SearchObject ObjectIndexFindByTypeAndId(long siteId, int type, Guid objectId)
        {
            return provider.ObjectIndexFindByTypeAndId(siteId, type, objectId);
        }

        /// <summary>
        /// Delegates the existence check for an indexed object in a site and type.
        /// </summary>
        /// <param name="siteId">The site that owns the object.</param>
        /// <param name="type">The provider-defined object type.</param>
        /// <param name="objectId">The source object's identifier.</param>
        /// <returns><see langword="true"/> when the provider finds a match; otherwise, <see langword="false"/>.</returns>
        public bool ObjectIndexObjectExists(long siteId, int type, Guid objectId)
        {
            return provider.ObjectIndexObjectExists(siteId, type, objectId);
        }

        /// <summary>
        /// Delegates deletion of an indexed object by its search-object identifier.
        /// </summary>
        /// <param name="id">The search-object identifier to delete.</param>
        /// <returns>The provider's deletion result.</returns>
        public bool ObjectIndexDelete(long id)
        {
            return provider.ObjectIndexDelete(id);
        }

        /// <summary>
        /// Delegates aggregate indexing of an object title and its scored stemmed words.
        /// </summary>
        /// <param name="siteId">The site that owns the object.</param>
        /// <param name="objectId">The source object's identifier.</param>
        /// <param name="objectType">The provider-defined object type.</param>
        /// <param name="title">The title stored with the indexed object.</param>
        /// <param name="wordScores">The stemmed-word scores to associate with the object.</param>
        /// <param name="culture">The culture used for the word entries.</param>
        public void AddObjectIndex(long siteId, Guid objectId, int objectType, string title,
            Dictionary<string, int> wordScores, string culture)
        {
            provider.AddObjectIndex(siteId, objectId, objectType, title, wordScores, culture);
        }

        #endregion

        #region Search operations

        /// <summary>
        /// Searches indexed objects across all sites after parsing the supplied text.
        /// </summary>
        /// <param name="query">The user query text.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of results on the page.</param>
        /// <param name="culture">The culture used by <see cref="TextParser"/> and the lexicon.</param>
        /// <param name="totalResults">Receives the total number of matching results.</param>
        /// <returns>The matching indexed objects for the requested page.</returns>
        public List<SearchObject> DoSearch(string query, int pageNumber, int pageSize, string culture,
            ref int totalResults)
        {
            return DoSearch(-1, query, culture, pageNumber, pageSize, ref totalResults);
        }

        /// <summary>
        /// Parses text into lexicon identifiers and searches either one site or all sites.
        /// </summary>
        /// <param name="siteId">The site to search; values greater than zero select site scope, while zero or negative values select all sites.</param>
        /// <param name="query">The user query text.</param>
        /// <param name="culture">The culture used by <see cref="TextParser"/> and the lexicon.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of results on the page.</param>
        /// <param name="totalResults">Receives the total number of matching results.</param>
        /// <returns>The matching indexed objects for the requested page.</returns>
        /// <remarks>
        /// Empty or whitespace-only text produces an empty word-ID list. The provider is still called, so provider
        /// behavior for an empty word list remains part of the existing search contract.
        /// </remarks>
        public List<SearchObject> DoSearch(long siteId, string query, string culture, int pageNumber, int pageSize,
            ref int totalResults)
        {
            // Parse Query into words and obtain word ids
            var wordIds = GetWordIdsForQuery(query, culture);

            if (siteId > 0)
            {
                return provider.DoSearchBySite(siteId, wordIds, pageNumber, pageSize, ref totalResults);
            }
            return provider.DoSearch(wordIds, pageNumber, pageSize, ref totalResults);
        }

        /// <summary>
        /// Searches products across all sites using advanced filters and parsed query text.
        /// </summary>
        /// <param name="query">The user query text.</param>
        /// <param name="culture">The culture used by <see cref="TextParser"/> and the lexicon.</param>
        /// <param name="queryAdv">The advanced product-search filters and facets.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products on the page.</param>
        /// <returns>The advanced product-search result.</returns>
        public ProductSearchResultAdv DoSearch(string query, string culture, ProductSearchQueryAdv queryAdv,
            int pageNumber, int pageSize)
        {
            return DoSearch(-1, query, culture, queryAdv, pageNumber, pageSize);
        }

        /// <summary>
        /// Searches products with advanced filters, using site scope when requested and a dedicated provider path for blank text.
        /// </summary>
        /// <param name="siteId">The site to search; a positive value selects site scope, while zero or negative values select the all-site path for nonblank text.</param>
        /// <param name="query">The user query text.</param>
        /// <param name="culture">The culture used by <see cref="TextParser"/> and the lexicon.</param>
        /// <param name="queryAdv">The advanced product-search filters and facets.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products on the page.</param>
        /// <returns>The advanced product-search result.</returns>
        /// <remarks>
        /// Nonblank text is parsed into lexicon identifiers and delegated to the matching site or all-site overload.
        /// Blank or whitespace-only text bypasses lexicon parsing and delegates to
        /// <see cref="ISearchProvider.DoSearchBySite(long, ProductSearchQueryAdv, int, int)"/>. In that case,
        /// the supplied site identifier, including the all-site sentinel, is passed through unchanged.
        /// </remarks>
        public ProductSearchResultAdv DoSearch(long siteId, string query, string culture, ProductSearchQueryAdv queryAdv,
            int pageNumber, int pageSize)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Parse Query into words and obtain word ids
                var wordIds = GetWordIdsForQuery(query, culture);

                if (siteId > 0)
                {
                    return provider.DoSearchBySite(siteId, wordIds, queryAdv, pageNumber, pageSize);
                }
                return provider.DoSearch(wordIds, queryAdv, pageNumber, pageSize);
            }
            return provider.DoSearchBySite(siteId, queryAdv, pageNumber, pageSize);
        }

        #endregion

        #region Helpers

        private List<long> GetWordIdsForQuery(string query, string culture)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<long>();
            }

            var parts = TextParser.ParseText(query, culture);
            var wordIds = FindAllWordIds(parts, culture);
            return wordIds ?? new List<long>();
        }

        #endregion
    }
}
