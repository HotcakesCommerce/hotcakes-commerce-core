#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    [Serializable]
    public class StaticSearchProvider : ISearchProvider
    {
        private readonly List<SearchObject> objects = new List<SearchObject>();
        private readonly List<SearchObjectWord> objectWords = new List<SearchObjectWord>();
        private readonly List<LexiconWord> words = new List<LexiconWord>();

        #region Lexicon operations

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

        public bool ObjectIndexObjectExists(long siteId, int type, Guid objectId)
        {
            var o = ObjectIndexFindByTypeAndId(siteId, type, objectId);
            if (o != null)
            {
                return true;
            }
            return false;
        }

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

        public void AddObjectIndex(long siteId, Guid objectId, int objectType, string title,
            Dictionary<string, int> wordScores, string culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Search Object Word operations

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

        public bool ObjectWordIndexInsert(SearchObjectWord w)
        {
            objectWords.Add(w);
            return true;
        }

        #endregion

        #region Search operations

        public List<SearchObject> DoSearch(List<long> wordIds, int pageNumber, int pageSize, ref int totalResults)
        {
            return DoSearchBySite(0, wordIds, pageNumber, pageSize, ref totalResults);
        }

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

        public ProductSearchResultAdv DoSearch(List<long> wordIds, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            throw new NotImplementedException();
        }

        public ProductSearchResultAdv DoSearchBySite(long siteId, List<long> wordIds, ProductSearchQueryAdv query,
            int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public ProductSearchResultAdv DoSearchBySite(long siteId, ProductSearchQueryAdv query, int pageNumber,
            int pageSize)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}