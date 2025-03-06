#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Search;

namespace Hotcakes.Commerce
{
    public static class CacheManager
    {
        public static Cache Current()
        {
            return HttpRuntime.Cache;
        }

        // Store
        public static Store GetStore(long storeId, string culture)
        {
            var cacheKey = string.Format("storeid-{0}-culture-{1}", storeId, culture);
            return GetItem<Store>(cacheKey);
        }

        public static void AddStore(Store store, string culture)
        {
            var cacheKey = string.Format("storeid-{0}-culture-{1}", store.Id, culture);
            StoreItem(cacheKey, store, 60);
        }

        public static Store GetStore(long storeId, string culture, Func<Store> getData)
        {
            var cacheKey = string.Format("storeid-{0}-culture-{1}", storeId, culture);

            return GetItem(cacheKey, getData);
        }

        public static void ClearStoreById(long id, string culture = null)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                var cachekey = string.Format("storeid-{0}-culture-{1}", id, culture);
                ClearItem(cachekey);
            }
            else
            {
                var cache = Current();
                if (cache != null)
                {
                    var cachekey = string.Format("storeid-{0}", id);
                    foreach (DictionaryEntry item in cache)
                    {
                        var key = (string) item.Key;
                        if (key.StartsWith(cachekey))
                            cache.Remove(key);
                    }
                }
            }
        }

        // Store Id by HostUrl
        public static long? GetStoreIdByHostName(string hostName, Func<long?> getData)
        {
            var cacheKey = string.Format("storeguid-{0}", hostName);

            return GetItem(cacheKey, getData);
        }

        // Store Id by Guid
        public static long? GetStoreIdByGuid(Guid guid, Func<long?> getData)
        {
            var cacheKey = string.Format("storeguid-{0}", guid);

            return GetItem(cacheKey, getData);
        }

        public static void ClearStoreIdByGuid(Guid guid)
        {
            var cachekey = string.Format("storeguid-{0}", guid);
            ClearItem(cachekey);
        }

        // Products
        public static void AddProduct(Product p)
        {
            StoreItem(string.Format("storeid-{0}-product-{1}-culture-{2}", p.StoreId, p.Bvin, p.ContentCulture), p, 60);
        }

        public static Product GetProduct(string bvin, long storeId, string culture)
        {
            var cache = Current();
            if (cache != null)
            {
                var i = cache[string.Format("storeid-{0}-product-{1}-culture-{2}", storeId, bvin, culture)];
                if (i != null)
                {
                    return (Product) i;
                }
            }
            return null;
        }

        public static void ClearProduct(string bvin, long storeId, string culture = null)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                ClearItem(string.Format("storeid-{0}-product-{1}-culture-{2}", storeId, bvin, culture));
            }
            else
            {
                var cache = Current();
                if (cache != null)
                {
                    foreach (DictionaryEntry item in cache)
                    {
                        var key = (string) item.Key;
                        if (key.StartsWith(string.Format("storeid-{0}-product-{1}", storeId, bvin)))
                            cache.Remove(key);
                    }
                }
            }
        }

        // Search Results
        public static ProductSearchResultAdv GetSearchResult(long? storeId, string query, string culture,
            ProductSearchQueryAdv queryAdv, int pageNumber, int pageSize, string accountId,
            Func<ProductSearchResultAdv> getData)
        {
            var cachekeyTemplate = "storeid-{0}-searchresult-{1}-{2}-{3}-{4}-{5}-{6}";
            var queryHash = MD5HashGenerator.GenerateKey(queryAdv);
            var cacheKey = string.Format(cachekeyTemplate, storeId, query, culture, queryHash, pageNumber, pageSize,
                accountId);

            return GetItem(cacheKey, getData);
        }

        //Countries
        public static void AddCountries(string culture, List<Country> countries)
        {
            StoreItem(string.Format("countries-culture-{0}", culture), countries, 60);
        }

        public static List<Country> GetCountries(string culture)
        {
            var cache = Current();
            if (cache != null)
            {
                var i = cache[string.Format("countries-culture-{0}", culture)];
                if (i != null)
                {
                    return (List<Country>) i;
                }
            }
            return null;
        }

        public static void ClearCountries(string culture = null)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                ClearItem(string.Format("countries-culture-{0}", culture));
            }
            else
            {
                var cache = Current();
                if (cache != null)
                {
                    foreach (DictionaryEntry item in cache)
                    {
                        var key = (string) item.Key;
                        if (key.StartsWith("countries"))
                            cache.Remove(key);
                    }
                }
            }
        }

        // Clear
        public static void ClearForStore(long storeId)
        {
            var cache = Current();
            if (cache != null)
            {
                foreach (DictionaryEntry item in cache)
                {
                    var key = (string) item.Key;
                    if (key.StartsWith("storeid-" + storeId))
                        cache.Remove(key);
                }
            }
        }

        public static void ClearAll()
        {
            var cache = Current();
            if (cache != null)
            {
                foreach (DictionaryEntry item in cache)
                {
                    cache.Remove((string) item.Key);
                }
            }
        }

        // Generic
        private static void StoreItem<T>(string key, T item, int minutes)
        {
            var cache = Current();
            if (cache != null)
            {
                cache.Insert(key, item, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, minutes, 0));
            }
        }

        private static T GetItem<T>(string key)
        {
            var cache = Current();
            if (cache != null)
            {
                var item = cache[key];
                if (item != null)
                {
                    return (T) item;
                }
            }
            return default(T);
        }

        public static T GetItem<T>(string key, Func<T> getData)
        {
            var cache = Current();
            if (cache != null)
            {
                var item = cache[key];
                if (item != null)
                {
                    return (T) item;
                }
                item = getData();
                if (item != null)
                    StoreItem(key, item, 60);
                return (T) item;
            }
            return default(T);
        }

        private static void ClearItem(string key)
        {
            var cache = Current();
            if (cache != null)
                cache.Remove(key);
        }
    }
}