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
using System.Web;
using System.Web.Caching;

namespace Hotcakes.Commerce.Datalayer
{
    public class CacheManager
    {
        private static readonly Dictionary<string, byte> products = new Dictionary<string, byte>();
        private static readonly Dictionary<string, byte> strings = new Dictionary<string, byte>();

        public static string GetStringFromCache(string key)
        {
            if (HttpContext.Current == null)
            {
                return null;
            }
            return (string) HttpContext.Current.Cache.Get(key);
        }

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

        public static void UpsertStringInCache(string key, string value, int minutes)
        {
            RemoveStringFromCache(key);
            AddStringToCache(key, value, minutes);
        }

        public static void RemoveStringFromCache(string key)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Cache.Remove(key);
            }
        }

        public static void onProductRemovedCallBack(string key, object value, CacheItemRemovedReason reason)
        {
            lock (products)
            {
                products.Remove(key);
            }
        }

        public static void onStringRemovedCallBack(string key, object value, CacheItemRemovedReason reason)
        {
            lock (strings)
            {
                strings.Remove(key);
            }
        }
    }
}