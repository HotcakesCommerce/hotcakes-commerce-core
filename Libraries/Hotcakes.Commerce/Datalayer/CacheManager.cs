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
using System.Web;
using System.Web.Caching;

namespace Hotcakes.Commerce.Datalayer
{
    /// <summary>
    /// Provides the string-cache operations used by Hotcakes Commerce.
    /// </summary>
    /// <remarks>
    /// These operations depend on <see cref="HttpContext.Current"/> and therefore
    /// require an active ASP.NET request host. Read and write operations that are
    /// invoked without a current HTTP context do not access an ASP.NET cache.
    /// String entries are tracked until the ASP.NET cache invokes
    /// <see cref="onStringRemovedCallBack(string, object, CacheItemRemovedReason)"/>.
    /// </remarks>
    public class CacheManager
    {
        private static readonly Dictionary<string, byte> products = new Dictionary<string, byte>();
        private static readonly Dictionary<string, byte> strings = new Dictionary<string, byte>();

        /// <summary>
        /// Gets a string value from the current ASP.NET request cache.
        /// </summary>
        /// <param name="key">The cache key to retrieve.</param>
        /// <returns>
        /// The cached string value, or <see langword="null"/> when there is no
        /// current request host or no value stored for <paramref name="key"/>.
        /// </returns>
        /// <remarks>
        /// This method reads <c>HttpContext.Current.Cache</c> directly and
        /// does not create a request context or otherwise alter cache state.
        /// </remarks>
        public static string GetStringFromCache(string key)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            return (string) HttpContext.Current.Cache.Get(key);
        }

        /// <summary>
        /// Adds or replaces a string value in the current ASP.NET request cache.
        /// </summary>
        /// <param name="key">The cache key under which to store the value.</param>
        /// <param name="value">The string value to cache.</param>
        /// <param name="minutes">
        /// The number of minutes from the current local time at which the entry
        /// expires.
        /// </param>
        /// <remarks>
        /// The entry uses absolute expiration based on
        /// <see cref="DateTime.Now"/> plus <paramref name="minutes"/> and does
        /// not use sliding expiration. When a request host is unavailable, this
        /// method performs no cache insertion and does not track the key. When a
        /// host is available, the key is tracked once in the internal string-key
        /// set and is removed when ASP.NET invokes
        /// <see cref="onStringRemovedCallBack(string, object, CacheItemRemovedReason)"/>.
        /// </remarks>
        public static void AddStringToCache(string key, string value, int minutes)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Insert(key, value, null,
                    DateTime.Now.AddMinutes(minutes),
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.Normal,
                    onStringRemovedCallBack);

                lock (strings)
                {
                    if (!strings.ContainsKey(key))
                    {
                        strings.Add(key, 0);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the existing string entry and then adds the supplied value.
        /// </summary>
        /// <param name="key">The cache key to replace.</param>
        /// <param name="value">The string value to cache.</param>
        /// <param name="minutes">
        /// The number of minutes from the current local time at which the new
        /// entry expires.
        /// </param>
        /// <remarks>
        /// This operation composes <see cref="RemoveStringFromCache(string)"/>
        /// and <see cref="AddStringToCache(string, string, int)"/> in that order.
        /// Its effective behavior therefore depends on the current ASP.NET
        /// request host, and the replacement uses absolute rather than sliding
        /// expiration.
        /// </remarks>
        public static void UpsertStringInCache(string key, string value, int minutes)
        {
            RemoveStringFromCache(key);
            AddStringToCache(key, value, minutes);
        }

        /// <summary>
        /// Removes a string entry from the current ASP.NET request cache.
        /// </summary>
        /// <param name="key">The cache key to remove.</param>
        /// <remarks>
        /// When no request host is available, this method performs no operation.
        /// When a host is available, it delegates removal to the ASP.NET cache
        /// exposed by <see cref="HttpContext.Cache"/>; the associated tracking entry is
        /// removed when the cache invokes
        /// <see cref="onStringRemovedCallBack(string, object, CacheItemRemovedReason)"/>.
        /// </remarks>
        public static void RemoveStringFromCache(string key)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        /// <summary>
        /// Removes a product key from the internal product tracking set after a
        /// cached product is removed.
        /// </summary>
        /// <param name="key">The removed product cache key.</param>
        /// <param name="value">The removed cached value supplied by ASP.NET.</param>
        /// <param name="reason">The ASP.NET cache removal reason.</param>
        /// <remarks>
        /// The callback spelling is intentionally preserved for compatibility.
        /// The supplied <paramref name="value"/> and <paramref name="reason"/>
        /// are not otherwise inspected.
        /// </remarks>
        public static void onProductRemovedCallBack(string key, object value, CacheItemRemovedReason reason)
        {
            lock (products)
            {
                products.Remove(key);
            }
        }

        /// <summary>
        /// Removes a string key from the internal string tracking set after a
        /// cached string is removed.
        /// </summary>
        /// <param name="key">The removed string cache key.</param>
        /// <param name="value">The removed cached value supplied by ASP.NET.</param>
        /// <param name="reason">The ASP.NET cache removal reason.</param>
        /// <remarks>
        /// ASP.NET supplies this callback when the cache entry expires or is
        /// otherwise removed. The callback spelling is intentionally preserved
        /// for compatibility, and the supplied <paramref name="value"/> and
        /// <paramref name="reason"/> are not otherwise inspected.
        /// </remarks>
        public static void onStringRemovedCallBack(string key, object value, CacheItemRemovedReason reason)
        {
            lock (strings)
            {
                strings.Remove(key);
            }
        }
    }
}
