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
    /// <summary>
    /// Provides access to the ASP.NET application cache used by Hotcakes Commerce.
    /// Cache entries are scoped by the dimensions included in their keys, and cached values are normally retained for 60 minutes.
    /// A missing cache, cache miss, or null factory result is represented by the applicable default value; null values are not inserted.
    /// </summary>
    public static class CacheManager
    {
        /// <summary>Gets the current ASP.NET application cache, or <c>null</c> when no cache is available.</summary>
        public static Cache Current()
        {
            return HttpRuntime.Cache;
        }

        // Store
        /// <summary>Gets a cached store for the specified store and culture.</summary>
        /// <param name="storeId">The store identifier, which is part of the cache key.</param>
        /// <param name="culture">The culture, which is part of the cache key.</param>
        /// <returns>The cached store, or <c>null</c> when it is not cached.</returns>
        public static Store GetStore(long storeId, string culture)
        {
            var cacheKey = string.Format("storeid-{0}-culture-{1}", storeId, culture);
            return GetItem<Store>(cacheKey);
        }

        /// <summary>Adds a store to the cache for its identifier and culture for 60 minutes.</summary>
        /// <param name="store">The store to cache.</param>
        /// <param name="culture">The culture dimension of the cache key.</param>
        public static void AddStore(Store store, string culture)
        {
            var cacheKey = string.Format("storeid-{0}-culture-{1}", store.Id, culture);
            StoreItem(cacheKey, store, 60);
        }

        /// <summary>Gets a cached store, loading and caching it for 60 minutes on a miss when the cache is available.</summary>
        /// <param name="storeId">The store identifier, which is part of the cache key.</param>
        /// <param name="culture">The culture, which is part of the cache key.</param>
        /// <param name="getData">The loader used only on a cache miss.</param>
        /// <returns>The cached or loaded store, or <c>null</c> when unavailable or when the loader returns <c>null</c>.</returns>
        public static Store GetStore(long storeId, string culture, Func<Store> getData)
        {
            var cacheKey = string.Format("storeid-{0}-culture-{1}", storeId, culture);

            return GetItem(cacheKey, getData);
        }

        /// <summary>Clears the store cache entry for an identifier and optional culture; without a culture, clears all matching culture entries.</summary>
        /// <param name="id">The store identifier.</param>
        /// <param name="culture">The optional culture dimension to clear.</param>
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
        /// <summary>Gets a store identifier by host name, loading and caching a non-null result for 60 minutes.</summary>
        /// <param name="hostName">The host name cache-key dimension.</param>
        /// <param name="getData">The loader used on a cache miss.</param>
        /// <returns>The store identifier, or <c>null</c> when unavailable.</returns>
        public static long? GetStoreIdByHostName(string hostName, Func<long?> getData)
        {
            var cacheKey = string.Format("storeguid-{0}", hostName);

            return GetItem(cacheKey, getData);
        }

        // Store Id by Guid
        /// <summary>Gets a store identifier by GUID, loading and caching a non-null result for 60 minutes.</summary>
        /// <param name="guid">The store GUID cache-key dimension.</param>
        /// <param name="getData">The loader used on a cache miss.</param>
        /// <returns>The store identifier, or <c>null</c> when unavailable.</returns>
        public static long? GetStoreIdByGuid(Guid guid, Func<long?> getData)
        {
            var cacheKey = string.Format("storeguid-{0}", guid);

            return GetItem(cacheKey, getData);
        }

        /// <summary>Clears the store-identifier cache entry for a store GUID.</summary>
        /// <param name="guid">The store GUID cache-key dimension.</param>
        public static void ClearStoreIdByGuid(Guid guid)
        {
            var cachekey = string.Format("storeguid-{0}", guid);
            ClearItem(cachekey);
        }

        // Products
        /// <summary>Adds a product to the cache for its store, product identifier, and content culture for 60 minutes.</summary>
        /// <param name="p">The product to cache.</param>
        public static void AddProduct(Product p)
        {
            StoreItem(string.Format("storeid-{0}-product-{1}-culture-{2}", p.StoreId, p.Bvin, p.ContentCulture), p, 60);
        }

        /// <summary>Gets a cached product using store, product identifier, and culture dimensions.</summary>
        /// <param name="bvin">The product unique identifier.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="culture">The content culture.</param>
        /// <returns>The cached product, or <c>null</c> when it is not cached.</returns>
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

        /// <summary>Clears a product cache entry, or all culture entries for the product when culture is omitted.</summary>
        /// <param name="bvin">The product unique identifier.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="culture">The optional content culture to clear.</param>
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
        /// <summary>Gets a cached product search result, loading it on a miss and keying it by store, query, culture, query options hash, page, page size, and account.</summary>
        /// <param name="storeId">The optional store identifier.</param><param name="query">The search text.</param><param name="culture">The culture.</param>
        /// <param name="queryAdv">The advanced query options, represented in the key by an uppercase MD5 hash.</param><param name="pageNumber">The page number.</param><param name="pageSize">The page size.</param><param name="accountId">The account identifier.</param><param name="getData">The loader used on a miss.</param>
        /// <returns>The cached or loaded result, or the default value when no cache is available or the loader returns <c>null</c>.</returns>
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
        /// <summary>Caches countries for a culture for 60 minutes.</summary>
        /// <param name="culture">The culture cache-key dimension.</param><param name="countries">The countries to cache.</param>
        public static void AddCountries(string culture, List<Country> countries)
        {
            StoreItem(string.Format("countries-culture-{0}", culture), countries, 60);
        }

        /// <summary>Gets countries cached for a culture.</summary>
        /// <param name="culture">The culture cache-key dimension.</param><returns>The cached countries, or <c>null</c> when absent.</returns>
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

        /// <summary>Clears countries for one culture, or all country entries when culture is omitted.</summary>
        /// <param name="culture">The optional culture dimension to clear.</param>
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
        /// <summary>Clears every cache entry whose key is scoped to the specified store.</summary>
        /// <param name="storeId">The store identifier.</param>
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

        /// <summary>Removes every entry from the current application cache.</summary>
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

        /// <summary>Gets a generic cached value, loading and caching a non-null result for 60 minutes on a miss.</summary>
        /// <typeparam name="T">The cached value type.</typeparam>
        /// <param name="key">The complete cache key, including all required scope dimensions.</param>
        /// <param name="getData">The loader used on a cache miss.</param>
        /// <returns>The cached or loaded value, or <c>default(T)</c> when no cache is available or the loader returns <c>null</c>.</returns>
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
