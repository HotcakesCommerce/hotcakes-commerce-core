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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    [Serializable]
    public class MemoryStrategyFactory
    {
        public static Dictionary<Type, IList> _storage = new Dictionary<Type, IList>();

        public static List<TF> CreateItems<TF>()
        {
            IList list = null;
            if (!_storage.TryGetValue(typeof (TF), out list))
            {
                list = new List<TF>();
                _storage.Add(typeof (TF), list);
            }

            return list as List<TF>;
        }

        public static void Clear()
        {
            foreach (var item in _storage.Values)
            {
                item.Clear();
            }
        }
    }

    [Serializable]
    public class MemoryStrategy<T> : IRepositoryStrategy<T> where T : class, new()
    {
        private long _IdentityCounter;
        private readonly PrimaryKeyType _keyType = PrimaryKeyType.Bvin;
        private readonly List<T> items;

        public MemoryStrategy(PrimaryKeyType keyType)
        {
            items = MemoryStrategyFactory.CreateItems<T>();
            _keyType = keyType;
            Logger = new NullLogger();
        }

        public MemoryStrategy(PrimaryKeyType keyType, ILogger logger)
        {
            items = MemoryStrategyFactory.CreateItems<T>();
            _keyType = keyType;
            Logger = logger;
        }

        public ILogger Logger { get; set; }


        public bool AutoSubmit
        {
            get { return true; }
            set { } // do nothing
        }

        public T FindByPrimaryKey(PrimaryKey key)
        {
            return items.SingleOrDefault(delegate(T t)
            {
                var keyValue = t.GetType().GetProperty(key.KeyName).GetValue(t, null);
                switch (key.KeyType)
                {
                    case PrimaryKeyType.Bvin:
                        var memberId = keyValue.ToString();
                        return memberId.Trim().ToLowerInvariant() == key.BvinValue.Trim().ToLowerInvariant();
                    case PrimaryKeyType.Guid:
                        var guidmemberId = (Guid) keyValue;
                        return guidmemberId == key.GuidValue;
                    case PrimaryKeyType.Integer:
                        var intmemberId = (int) keyValue;
                        return intmemberId == key.IntValue;
                    case PrimaryKeyType.Long:
                        var longmemberId = (long) keyValue;
                        return longmemberId == key.LongValue;
                }
                return false;
            });
        }

        public IQueryable<T> Find()
        {
            try
            {
                return items.AsQueryable();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, EventLogSeverity.Debug);
            }

            return null;
        }

        public bool Create(T item)
        {
            var result = false;

            try
            {
                SetPrimaryKey(item);
                items.Add(item);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.LogException(ex, EventLogSeverity.Debug);
            }

            return result;
        }

        public bool SubmitChanges()
        {
            return true;
        }

        public bool Delete(PrimaryKey key)
        {
            var result = false;

            try
            {
                var existing = FindByPrimaryKey(key);

                if (existing != null)
                {
                    items.Remove(existing);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.LogException(ex, EventLogSeverity.Debug);
            }

            return result;
        }

        public void Detach(T item)
        {
        }

        public int CountOfAll()
        {
            var result = 0;

            try
            {
                result = items.Count();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, EventLogSeverity.Debug);
            }

            return result;
        }

        private void SetPrimaryKey(T item)
        {
            _IdentityCounter += 1;
            var prop = item.GetType().GetProperty("bvin", BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                prop = item.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            }

            if (prop != null && prop.CanWrite)
            {
                switch (_keyType)
                {
                    case PrimaryKeyType.Bvin:
                        // do nothing, handled by external code
                        break;
                    case PrimaryKeyType.Guid:
                        // Assign Guid
                        prop.SetValue(item, Guid.NewGuid(), null);
                        break;
                    case PrimaryKeyType.Integer:
                        // Assign Integer
                        prop.SetValue(item, (int) _IdentityCounter, null);
                        break;
                    case PrimaryKeyType.Long:
                        // Assign Long
                        prop.SetValue(item, _IdentityCounter, null);
                        break;
                }
            }
        }
    }
}