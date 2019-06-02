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

using System;
using System.Collections.Generic;
using Hotcakes.Web.Search;

namespace Hotcakes.Commerce.Search
{
    public class Searcher
    {
        private readonly ISearchProvider provider;

        public Searcher(ISearchProvider p)
        {
            provider = p;
        }

        #region Search Object Word operations

        public bool ObjectWordIndexInsert(SearchObjectWord w)
        {
            return provider.ObjectWordIndexInsert(w);
        }

        #endregion

        #region Lexicon operations

        public long AddOrCreateWord(string stemmedWord, string culture)
        {
            var id = FindWordId(stemmedWord, culture);

            if (id > 0)
            {
                return id;
            }
            id = provider.InsertWord(stemmedWord, culture);
            return id;
        }

        public long FindWordId(string stemmedWord, string culture)
        {
            return provider.FindWordId(stemmedWord, culture);
        }

        public List<long> FindAllWordIds(List<string> stemmedWords, string culture)
        {
            return provider.FindAllWordIds(stemmedWords, culture);
        }

        #endregion

        #region Search Object operations

        public long ObjectIndexAddOrUpdate(SearchObject s)
        {
            var existing = provider.ObjectIndexFindByTypeAndId(s.SiteId, s.ObjectType, s.ObjectId);
            if (existing != null)
            {
                var existingId = existing.Id;
                ObjectIndexDelete(existingId);
                return provider.ObjectIndexInsert(s);
            }
            return provider.ObjectIndexInsert(s);
        }

        public SearchObject ObjectIndexFind(long id)
        {
            return provider.ObjectIndexFind(id);
        }

        public SearchObject ObjectIndexFindByTypeAndId(long siteId, int type, Guid objectId)
        {
            return provider.ObjectIndexFindByTypeAndId(siteId, type, objectId);
        }

        public bool ObjectIndexObjectExists(long siteId, int type, Guid objectId)
        {
            return provider.ObjectIndexObjectExists(siteId, type, objectId);
        }

        public bool ObjectIndexDelete(long id)
        {
            return provider.ObjectIndexDelete(id);
        }

        public void AddObjectIndex(long siteId, Guid objectId, int objectType, string title,
            Dictionary<string, int> wordScores, string culture)
        {
            provider.AddObjectIndex(siteId, objectId, objectType, title, wordScores, culture);
        }

        #endregion

        #region Search operations

        public List<SearchObject> DoSearch(string query, int pageNumber, int pageSize, string culture,
            ref int totalResults)
        {
            return DoSearch(-1, query, culture, pageNumber, pageSize, ref totalResults);
        }

        public List<SearchObject> DoSearch(long siteId, string query, string culture, int pageNumber, int pageSize,
            ref int totalResults)
        {
            // Parse Query into words
            var parts = TextParser.ParseText(query, culture);

            // Get wordIds for all words in query
            var wordIds = FindAllWordIds(parts, culture);

            if (siteId > 0)
            {
                return provider.DoSearchBySite(siteId, wordIds, pageNumber, pageSize, ref totalResults);
            }
            return provider.DoSearch(wordIds, pageNumber, pageSize, ref totalResults);
        }

        public ProductSearchResultAdv DoSearch(string query, string culture, ProductSearchQueryAdv queryAdv,
            int pageNumber, int pageSize)
        {
            return DoSearch(-1, query, culture, queryAdv, pageNumber, pageSize);
        }

        public ProductSearchResultAdv DoSearch(long siteId, string query, string culture, ProductSearchQueryAdv queryAdv,
            int pageNumber, int pageSize)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Parse Query into words
                var parts = TextParser.ParseText(query, culture);

                // Get wordIds for all words in query
                var wordIds = FindAllWordIds(parts, culture);

                if (siteId > 0)
                {
                    return provider.DoSearchBySite(siteId, wordIds, queryAdv, pageNumber, pageSize);
                }
                return provider.DoSearch(wordIds, queryAdv, pageNumber, pageSize);
            }
            return provider.DoSearchBySite(siteId, queryAdv, pageNumber, pageSize);
        }

        #endregion
    }
}