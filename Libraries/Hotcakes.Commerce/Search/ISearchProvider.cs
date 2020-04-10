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

namespace Hotcakes.Commerce.Search
{
    public interface ISearchProvider
    {
        #region Lexicon operations

        long InsertWord(string stemmedWord, string culture);
        long FindWordId(string stemmedWord, string culture);
        List<long> FindAllWordIds(List<string> stemmedWords, string culture);

        #endregion

        #region Search Object operations

        long ObjectIndexInsert(SearchObject s);
        SearchObject ObjectIndexFind(long id);
        List<SearchObject> ObjectIndexFindAllInList(List<long> ids);
        SearchObject ObjectIndexFindByTypeAndId(long siteId, int type, Guid objectId);
        bool ObjectIndexObjectExists(long siteId, int type, Guid objectId);
        bool ObjectIndexDelete(long id);

        void AddObjectIndex(long siteId, Guid objectId, int objectType, string title, Dictionary<string, int> wordScores,
            string culture);

        #endregion

        #region Search Object Word operations

        bool ObjectWordIndexDelete(SearchObjectWord w);
        bool ObjectWordIndexInsert(SearchObjectWord w);

        #endregion

        #region Search operations

        List<SearchObject> DoSearch(List<long> wordIds, int pageNumber, int pageSize, ref int totalResults);

        List<SearchObject> DoSearchBySite(long siteId, List<long> wordIds, int pageNumber, int pageSize,
            ref int totalResults);

        ProductSearchResultAdv DoSearch(List<long> wordIds, ProductSearchQueryAdv query, int pageNumber, int pageSize);

        ProductSearchResultAdv DoSearchBySite(long siteId, List<long> wordIds, ProductSearchQueryAdv query,
            int pageNumber, int pageSize);

        ProductSearchResultAdv DoSearchBySite(long siteId, ProductSearchQueryAdv query, int pageNumber, int pageSize);

        #endregion
    }
}