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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// Provides the common repository boundary for mapping persistence entities to POCO models.
    /// </summary>
    /// <typeparam name="T">The persistence entity type used by the repository strategy.</typeparam>
    /// <typeparam name="V">The POCO model type exposed by the repository.</typeparam>
    public abstract class HccRepoBase<T, V> : IRepo
        where T : class, new()
        where V : class, new()
    {
        /// <summary>
        /// Initializes the repository with the commerce request context used for scoped operations.
        /// </summary>
        /// <param name="context">The request context that supplies commerce and culture information.</param>
        protected HccRepoBase(HccRequestContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the commerce request context associated with this repository.
        /// </summary>
        protected HccRequestContext Context { get; private set; }

        /// <summary>
        /// Creates a model and persists it.
        /// </summary>
        /// <param name="item">The model to map and persist.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        public abstract bool Create(V item);

        /// <summary>
        /// Updates the first persistence entity matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="m">The model whose values are copied to the entity.</param>
        /// <param name="predicate">The query predicate used to locate the entity.</param>
        /// <param name="mergeSubItems">Whether related model data should be merged after persistence.</param>
        /// <returns><see langword="true"/> when the update succeeds; otherwise, <see langword="false"/>.</returns>
        protected abstract bool Update(V m, Expression<Func<T, bool>> predicate, bool mergeSubItems = true);

        /// <summary>
        /// Deletes entities matching <paramref name="predicate"/> and returns the operation status.
        /// </summary>
        /// <param name="predicate">The query predicate identifying entities to delete.</param>
        /// <returns><see langword="true"/> when deletion succeeds; otherwise, <see langword="false"/>.</returns>
        protected virtual bool Delete(Expression<Func<T, bool>> predicate)
        {
            var result = DeleteAdv(predicate);
            return result.Success;
        }

        /// <summary>
        /// Deletes matching entities and returns the detailed batch result, including old values.
        /// </summary>
        /// <param name="predicate">The query predicate identifying entities to delete.</param>
        /// <returns>The detailed deletion result.</returns>
        protected abstract DalBatchOperationResult<V> DeleteAdv(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Loads related data for a collection of models after their main data is materialized.
        /// </summary>
        /// <param name="items">The models to enrich.</param>
        protected virtual void GetSubItems(List<V> items)
        {
        }

        /// <summary>
        /// Loads related data for a single model by routing through the collection hook.
        /// </summary>
        /// <param name="item">The model to enrich.</param>
        protected void GetSubItems(V item)
        {
            GetSubItems(new List<V> {item});
        }

        /// <summary>
        /// Persists or synchronizes related data after the main model operation completes.
        /// </summary>
        /// <param name="model">The model whose related data should be merged.</param>
        protected virtual void MergeSubItems(V model)
        {
        }

        /// <summary>
        /// Removes related data that must be deleted before the persistence entity is deleted.
        /// </summary>
        /// <param name="data">The persistence entity whose related data is being removed.</param>
        protected virtual void DeleteAllSubItems(T data)
        {
        }

        /// <summary>
        /// Counts all entities using a read strategy and a non-tracking query.
        /// </summary>
        /// <returns>The number of persisted entities visible to the repository.</returns>
        public virtual int CountOfAll()
        {
            using (var s = CreateReadStrategy())
            {
                return s.GetQuery().AsNoTracking().Count();
            }
        }

        /// <summary>
        /// Creates the repository's normal strategy for tracked persistence operations.
        /// </summary>
        /// <returns>A strategy created by the shared <see cref="Factory"/>.</returns>
        protected IRepoStrategy<T> CreateStrategy()
        {
            return Factory.Instance.CreateStrategy<T>();
        }

        /// <summary>
        /// Creates the repository's read strategy for query operations that do not require tracking.
        /// </summary>
        /// <returns>A read strategy created by the shared <see cref="Factory"/>.</returns>
        protected IRepoStrategy<T> CreateReadStrategy()
        {
            return Factory.Instance.CreateReadStrategy<T>();
        }
    }
}
