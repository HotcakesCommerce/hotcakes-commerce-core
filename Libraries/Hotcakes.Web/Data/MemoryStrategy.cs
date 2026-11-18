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
using System.Linq;
using System.Reflection;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    /// <summary>
    /// Provides shared in-memory collections for memory repository strategies.
    /// </summary>
    [Serializable]
    public class MemoryStrategyFactory
    {
        /// <summary>
        /// Gets the type-indexed in-memory storage collections.
        /// </summary>
        public static Dictionary<Type, IList> _storage = new Dictionary<Type, IList>();

        /// <summary>
        /// Gets or creates the collection used for a model type.
        /// </summary>
        /// <typeparam name="TF">The model type whose collection is requested.</typeparam>
        /// <returns>The shared collection for <typeparamref name="TF"/>.</returns>
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

        /// <summary>
        /// Removes all models from every shared in-memory collection.
        /// </summary>
        public static void Clear()
        {
            foreach (var item in _storage.Values)
            {
                item.Clear();
            }
        }
    }

    /// <summary>
    /// Implements repository persistence using shared in-memory collections.
    /// </summary>
    /// <typeparam name="T">The model type managed by the strategy.</typeparam>
    [Serializable]
    public class MemoryStrategy<T> : IRepositoryStrategy<T> where T : class, new()
    {
        private long _IdentityCounter;
        private readonly PrimaryKeyType _keyType = PrimaryKeyType.Bvin;
        private readonly List<T> items;

        /// <summary>
        /// Initializes a memory strategy using the specified key type.
        /// </summary>
        /// <param name="keyType">The key type used when assigning generated keys.</param>
        public MemoryStrategy(PrimaryKeyType keyType)
        {
            items = MemoryStrategyFactory.CreateItems<T>();
            _keyType = keyType;
            Logger = new NullLogger();
        }

        /// <summary>
        /// Initializes a memory strategy using the specified key type and logger.
        /// </summary>
        /// <param name="keyType">The key type used when assigning generated keys.</param>
        /// <param name="logger">The logger used for persistence diagnostics.</param>
        public MemoryStrategy(PrimaryKeyType keyType, ILogger logger)
        {
            items = MemoryStrategyFactory.CreateItems<T>();
            _keyType = keyType;
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the logger used for persistence diagnostics.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the submission behavior. In-memory changes are always immediate.
        /// </summary>
        public bool AutoSubmit
        {
            get { return true; }
            set { } // do nothing
        }

        /// <summary>
        /// Finds an item by its <see cref="PrimaryKey"/>.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <returns>The matching item, or <see langword="null"/> when it is not found.</returns>
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

        /// <summary>
        /// Gets the in-memory items as a queryable sequence.
        /// </summary>
        /// <returns>The queryable item sequence, or <see langword="null"/> when querying fails.</returns>
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

        /// <summary>
        /// Adds an item to the shared in-memory collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns><see langword="true"/> when the item is added; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Completes pending changes. In-memory changes are already applied.
        /// </summary>
        /// <returns>Always <see langword="true"/>.</returns>
        public bool SubmitChanges()
        {
            return true;
        }

        /// <summary>
        /// Deletes the item identified by a <see cref="PrimaryKey"/>.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <returns><see langword="true"/> when an item is deleted; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Detaches an item. This strategy does not track items, so the operation has no effect.
        /// </summary>
        /// <param name="item">The item to detach.</param>
        public void Detach(T item)
        {
        }

        /// <summary>
        /// Counts the items in the shared in-memory collection.
        /// </summary>
        /// <returns>The item count.</returns>
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
