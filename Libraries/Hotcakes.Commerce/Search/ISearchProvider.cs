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

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Defines the persistence and execution contract used by <see cref="Searcher"/> for the search lexicon,
    /// indexed objects, object-word associations, and search operations.
    /// </summary>
    /// <remarks>
    /// Implementations may use different storage mechanisms, but must preserve the distinction between the
    /// site-scoped overloads and the all-site overloads. A site identifier greater than zero identifies a
    /// specific site; zero or a negative value is treated by the search facade as the all-site path. The
    /// overload accepting only a <see cref="ProductSearchQueryAdv"/> is also used for product searches with
    /// blank text, so providers must retain that overload even when no lexicon word IDs are available.
    /// </remarks>
    public interface ISearchProvider
    {
        #region Lexicon operations

        /// <summary>
        /// Inserts a stemmed word into the lexicon for the specified culture.
        /// </summary>
        /// <param name="stemmedWord">The normalized, stemmed word to insert.</param>
        /// <param name="culture">The culture associated with the lexicon entry.</param>
        /// <returns>The provider-assigned lexicon identifier.</returns>
        long InsertWord(string stemmedWord, string culture);

        /// <summary>
        /// Finds the identifier of a stemmed word in a culture-specific lexicon.
        /// </summary>
        /// <param name="stemmedWord">The normalized, stemmed word to find.</param>
        /// <param name="culture">The culture whose lexicon is searched.</param>
        /// <returns>The lexicon identifier, or the provider's not-found value.</returns>
        long FindWordId(string stemmedWord, string culture);

        /// <summary>
        /// Finds the identifiers for the supplied stemmed words in a culture-specific lexicon.
        /// </summary>
        /// <param name="stemmedWords">The normalized, stemmed words to find.</param>
        /// <param name="culture">The culture whose lexicon is searched.</param>
        /// <returns>The matching lexicon identifiers. Providers may return <see langword="null"/> when no result is available; <see cref="Searcher"/> normalizes that result for query parsing.</returns>
        List<long> FindAllWordIds(List<string> stemmedWords, string culture);

        #endregion

        #region Search Object operations

        /// <summary>
        /// Inserts an indexed <see cref="SearchObject"/> and returns its assigned identifier.
        /// </summary>
        /// <param name="s">The object record to insert.</param>
        /// <returns>The assigned search-object identifier.</returns>
        long ObjectIndexInsert(SearchObject s);

        /// <summary>
        /// Finds an indexed object by its search-object identifier.
        /// </summary>
        /// <param name="id">The search-object identifier.</param>
        /// <returns>The matching object, or <see langword="null"/> when it is not found.</returns>
        SearchObject ObjectIndexFind(long id);

        /// <summary>
        /// Finds all indexed objects whose identifiers occur in the supplied list.
        /// </summary>
        /// <param name="ids">The search-object identifiers to resolve.</param>
        /// <returns>The matching indexed objects.</returns>
        List<SearchObject> ObjectIndexFindAllInList(List<long> ids);

        /// <summary>
        /// Finds an indexed object by site, object type, and source object identifier.
        /// </summary>
        /// <param name="siteId">The site that owns the indexed object.</param>
        /// <param name="type">The provider-defined object type.</param>
        /// <param name="objectId">The source object's identifier.</param>
        /// <returns>The matching object, or <see langword="null"/> when it is not found.</returns>
        SearchObject ObjectIndexFindByTypeAndId(long siteId, int type, Guid objectId);

        /// <summary>
        /// Determines whether an indexed object exists for the specified site, type, and source identifier.
        /// </summary>
        /// <param name="siteId">The site that owns the indexed object.</param>
        /// <param name="type">The provider-defined object type.</param>
        /// <param name="objectId">The source object's identifier.</param>
        /// <returns><see langword="true"/> when a matching indexed object exists; otherwise, <see langword="false"/>.</returns>
        bool ObjectIndexObjectExists(long siteId, int type, Guid objectId);

        /// <summary>
        /// Deletes an indexed object by its search-object identifier.
        /// </summary>
        /// <param name="id">The search-object identifier to delete.</param>
        /// <returns><see langword="true"/> when an object was deleted; otherwise, <see langword="false"/>.</returns>
        bool ObjectIndexDelete(long id);

        /// <summary>
        /// Adds an indexed object and its word scores using the provider's aggregate indexing operation.
        /// </summary>
        /// <param name="siteId">The site that owns the indexed object.</param>
        /// <param name="objectId">The source object's identifier.</param>
        /// <param name="objectType">The provider-defined object type.</param>
        /// <param name="title">The title stored with the indexed object.</param>
        /// <param name="wordScores">A mapping of stemmed words to their scores.</param>
        /// <param name="culture">The culture used for the word entries.</param>
        void AddObjectIndex(long siteId, Guid objectId, int objectType, string title, Dictionary<string, int> wordScores,
            string culture);

        #endregion

        #region Search Object Word operations

        /// <summary>
        /// Deletes the association between an indexed object and a lexicon word.
        /// </summary>
        /// <param name="w">The object-word association to delete.</param>
        /// <returns><see langword="true"/> when the provider reports success; otherwise, <see langword="false"/>.</returns>
        bool ObjectWordIndexDelete(SearchObjectWord w);

        /// <summary>
        /// Inserts an association between an indexed object and a lexicon word.
        /// </summary>
        /// <param name="w">The object-word association, including its score and site.</param>
        /// <returns><see langword="true"/> when the provider reports success; otherwise, <see langword="false"/>.</returns>
        bool ObjectWordIndexInsert(SearchObjectWord w);

        #endregion

        #region Search operations

        /// <summary>
        /// Searches indexed objects containing the supplied lexicon words across all sites.
        /// </summary>
        /// <param name="wordIds">The lexicon identifiers parsed from the query.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of results on the page.</param>
        /// <param name="totalResults">Receives the total number of matching results.</param>
        /// <returns>The matching indexed objects for the requested page.</returns>
        List<SearchObject> DoSearch(List<long> wordIds, int pageNumber, int pageSize, ref int totalResults);

        /// <summary>
        /// Searches indexed objects containing the supplied lexicon words for one site, or across all sites when the site value is not positive.
        /// </summary>
        /// <param name="siteId">The site to search; values less than or equal to zero select the all-site behavior.</param>
        /// <param name="wordIds">The lexicon identifiers parsed from the query.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of results on the page.</param>
        /// <param name="totalResults">Receives the total number of matching results.</param>
        /// <returns>The matching indexed objects for the requested page.</returns>
        List<SearchObject> DoSearchBySite(long siteId, List<long> wordIds, int pageNumber, int pageSize,
            ref int totalResults);

        /// <summary>
        /// Searches products across all sites using lexicon word identifiers and advanced product filters.
        /// </summary>
        /// <param name="wordIds">The lexicon identifiers parsed from the query.</param>
        /// <param name="query">The advanced product-search filters and facets.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products on the page.</param>
        /// <returns>The advanced product-search result.</returns>
        ProductSearchResultAdv DoSearch(List<long> wordIds, ProductSearchQueryAdv query, int pageNumber, int pageSize);

        /// <summary>
        /// Searches products for one site using lexicon word identifiers and advanced product filters.
        /// </summary>
        /// <param name="siteId">The site to search.</param>
        /// <param name="wordIds">The lexicon identifiers parsed from the query.</param>
        /// <param name="query">The advanced product-search filters and facets.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products on the page.</param>
        /// <returns>The advanced product-search result.</returns>
        ProductSearchResultAdv DoSearchBySite(long siteId, List<long> wordIds, ProductSearchQueryAdv query,
            int pageNumber, int pageSize);

        /// <summary>
        /// Searches products using advanced filters without lexicon word identifiers, as used for blank product-search text.
        /// </summary>
        /// <param name="siteId">The site to search; the facade passes the sentinel value through unchanged for an all-site blank-text search.</param>
        /// <param name="query">The advanced product-search filters and facets.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of products on the page.</param>
        /// <returns>The advanced product-search result.</returns>
        ProductSearchResultAdv DoSearchBySite(long siteId, ProductSearchQueryAdv query, int pageNumber, int pageSize);

        #endregion
    }
}
