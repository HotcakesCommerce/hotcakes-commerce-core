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
using Hotcakes.Commerce.Metrics;
using Hotcakes.Commerce.Search;
using Hotcakes.Web;
using Hotcakes.Web.Data;
using Hotcakes.Web.Search;
using StackExchange.Profiling;

namespace Hotcakes.Commerce.Catalog
{
    public class SearchManager
    {
        private const int INDEXREBUILDPAGESIZE = 1000;

        private readonly Searcher searcher;

        public SearchManager(HccRequestContext context)
        {
            Context = context;

            var searchProvider = Factory.Instance.CreateSearchProvider(Context);
            searcher = new Searcher(searchProvider);

            QueryRepository = Factory.CreateRepo<SearchQueryRepository>(Context);
        }

        protected HccRequestContext Context { get; set; }
        private SearchQueryRepository QueryRepository { get; set; }

        public List<SearchObject> DoSearchForAllStores(string query, int pageNumber, int pageSize, ref int totalResults)
        {
            return searcher.DoSearch(query, pageNumber, pageSize, Context.MainContentCulture, ref totalResults);
        }

        public List<SearchObject> DoSearch(long siteId, string query, int pageNumber, int pageSize, ref int totalResults)
        {
            return searcher.DoSearch(siteId, query, Context.MainContentCulture, pageNumber, pageSize, ref totalResults);
        }

        public ProductSearchResultAdv DoProductSearchForAllStores(string query, ProductSearchQueryAdv queryAdv,
            int pageNumber, int pageSize)
        {
            var culture = Context.MainContentCulture;
            var accountId = Context.CurrentAccount != null ? Context.CurrentAccount.Bvin : string.Empty;

            return CacheManager.GetSearchResult(null, query, culture, queryAdv, pageNumber, pageSize, accountId,
                () => { return searcher.DoSearch(query, culture, queryAdv, pageNumber, pageSize); });
        }

        public ProductSearchResultAdv DoProductSearch(long siteId, string query, ProductSearchQueryAdv queryAdv,
            int pageNumber, int pageSize, bool logSearchQuery = false)
        {
            var culture = Context.MainContentCulture;
            var accountId = Context.CurrentAccount != null ? Context.CurrentAccount.Bvin : string.Empty;

            if (logSearchQuery)
            {
                var searchQuery = new SearchQuery
                {
                    QueryPhrase = query,
                    ShopperID = accountId
                };
                QueryRepository.Create(searchQuery);
            }

            return CacheManager.GetSearchResult(siteId, query, culture, queryAdv, pageNumber, pageSize, accountId,
                () => { return searcher.DoSearch(siteId, query, culture, queryAdv, pageNumber, pageSize); });
        }

        public void RebuildProductSearchIndex(HotcakesApplication app)
        {
            var totalProducts = app.CatalogServices.Products.FindAllForAllStoresCount();
            var totalPages = Paging.TotalPages(totalProducts, INDEXREBUILDPAGESIZE);

            for (var i = 1; i <= totalPages; i++)
            {
                IndexAPage(i, app);
            }
        }

        private void IndexAPage(int pageNumber, HotcakesApplication app)
        {
            var startIndex = Paging.StartRowIndex(pageNumber, INDEXREBUILDPAGESIZE);
            var page = app.CatalogServices.Products.FindAllPagedForAllStores(startIndex, INDEXREBUILDPAGESIZE);
            if (page != null)
            {
                foreach (var p in page)
                {
                    IndexProduct(p);
                }
            }
        }

        // Products
        public void IndexSingleProduct(Product p)
        {
            IndexProduct(p);
        }

        public void RemoveSingleProduct(long storeId, string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            var o = searcher.ObjectIndexFindByTypeAndId(storeId, (int) SearchManagerObjectType.Product, guid);
            if (o == null || o.Id < 1)
                return;
            searcher.ObjectIndexDelete(o.Id);
        }

        private void IndexProduct(Product p)
        {
            if (p == null)
                throw new ArgumentNullException("p");
            var languages = Factory.Instance.CreateStoreSettingsProvider().GetLocales();
            if (languages != null && languages.Any())
            {
                foreach (var language in languages)
                {
                    IndexProduct(p, language.Code);
                }
            }
            else
            {
                IndexProduct(p, Context.MainContentCulture);
            }
        }

        private void IndexProduct(Product p, string culture)
        {
            var storeId = p.StoreId;
            var documentId = p.Bvin;
            var documentType = (int) SearchManagerObjectType.Product;
            var title = p.ProductName + " | " + p.Sku;
            var scoredparts = new Dictionary<string, int>();

            ParseAndValue(p.Sku, culture, SearchManagerImportance.Highest, scoredparts, 10);
            ParseAndValue(p.ProductName, culture, SearchManagerImportance.Highest, scoredparts, 20);
            ParseAndValue(p.MetaTitle, culture, SearchManagerImportance.High, scoredparts, 20);
            ParseAndValue(p.MetaKeywords, culture, SearchManagerImportance.High, scoredparts, 20);

            ParseAndValue(p.MetaDescription, culture, SearchManagerImportance.Normal, scoredparts, 20);
            ParseAndValue(p.LongDescription, culture, SearchManagerImportance.Normal, scoredparts, 100);
            ParseAndValue(p.Keywords, culture, SearchManagerImportance.Normal, scoredparts, 20);

            if (p.HasVariants())
            {
                foreach (var v in p.Variants)
                {
                    if (v.Sku != p.Sku)
                    {
                        ParseAndValue(v.Sku, culture, SearchManagerImportance.Highest, scoredparts, 10);
                    }
                }
            }

            var optiontext = string.Empty;
            if (p.HasOptions())
            {
                foreach (var opt in p.Options)
                {
                    optiontext += opt.Name + " ";
                    foreach (var item in opt.Items)
                    {
                        optiontext += item.Name + " ";
                    }
                }
            }
            ParseAndValue(optiontext, culture, SearchManagerImportance.NormalHigh, scoredparts, 10);

            using (MiniProfiler.Current.Step("Search Index Rebuilt For Product"))
            {
                var objectId = DataTypeHelper.BvinToGuid(documentId);
                searcher.AddObjectIndex(storeId, objectId, documentType, title, scoredparts, culture);
            }
        }

        private void ParseAndValue(string text, string culture, SearchManagerImportance importance,
            Dictionary<string, int> scoredParts, int maxWordsToParse)
        {
            var parts = TextParser.ParseText(text, culture, true, maxWordsToParse);
            if (parts == null)
                return;

            foreach (var s in parts)
            {
                if (scoredParts.ContainsKey(s))
                {
                    scoredParts[s] += (int) importance;
                }
                else
                {
                    scoredParts.Add(s, (int) importance);
                }
            }
        }
    }
}