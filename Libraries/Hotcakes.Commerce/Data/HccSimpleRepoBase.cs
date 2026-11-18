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
    /// Implements common CRUD, batch, merge, mapping, and materialization behavior for simple repositories.
    /// </summary>
    /// <typeparam name="T">The persistence entity type.</typeparam>
    /// <typeparam name="V">The POCO model type.</typeparam>
    public abstract class HccSimpleRepoBase<T, V> : HccRepoBase<T, V>
        where T : class, new()
        where V : class, new()
    {
        /// <summary>
        /// Initializes a simple repository with its commerce request context.
        /// </summary>
        /// <param name="context">The request context used by repository operations.</param>
        protected HccSimpleRepoBase(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Creates the in-memory match used to associate a model with a queried entity during batch operations.
        /// </summary>
        /// <param name="item">The model being matched.</param>
        /// <returns>A predicate over queried entities. The default predicate matches nothing.</returns>
        protected virtual Func<T, bool> MatchItems(V item)
        {
            return t => false;
        }

        /// <summary>
        /// Creates the in-memory predicate identifying queried entities absent from a merge input.
        /// </summary>
        /// <param name="items">The complete incoming model set.</param>
        /// <returns>A predicate over queried entities. The default predicate matches nothing.</returns>
        protected virtual Func<T, bool> NotMatchItems(List<V> items)
        {
            return t => false;
        }

        /// <summary>
        /// Maps, tracks, and persists one model, then copies persistence-generated values back to it.
        /// </summary>
        /// <param name="item">The model to create.</param>
        /// <returns><see langword="true"/> when the strategy submits successfully.</returns>
        public override bool Create(V item)
        {
            var result = false;

            var entity = new T();
            CopyModelToData(entity, item);

            using (var s = CreateStrategy())
            {
                s.Add(entity);
                result = s.SubmitChanges();
            }

            CopyDataToModel(entity, item);
            MergeSubItems(item);

            return result;
        }

        /// <summary>
        /// Creates a set of models in one tracked strategy operation.
        /// </summary>
        /// <param name="items">The models to create; an empty list returns <see langword="false"/>.</param>
        /// <param name="mergeSubItems">Whether each model's related data is merged after submission.</param>
        /// <returns><see langword="true"/> when the batch submission succeeds.</returns>
        public virtual bool BatchCreate(List<V> items, bool mergeSubItems = true)
        {
            var result = false;

            if (items.Count == 0)
                return result;

            var entities = new List<T>();
            using (var s = CreateStrategy())
            {
                s.AutoDetectChanges(false);

                foreach (var item in items)
                {
                    var entity = new T();
                    CopyModelToData(entity, item);
                    entities.Add(entity);
                    s.Add(entity);
                }
                result = s.SubmitChanges();
            }

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var entity = entities[i];
                CopyDataToModel(entity, item);
            }

            if (mergeSubItems)
            {
                foreach (var item in items)
                {
                    MergeSubItems(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the first matching entity through <see cref="UpdateAdv"/>.
        /// </summary>
        /// <param name="m">The replacement model values.</param>
        /// <param name="predicate">The predicate used to query the entity.</param>
        /// <param name="mergeSubItems">Whether related data is merged after submission.</param>
        /// <returns>The detailed operation success flag.</returns>
        protected override bool Update(V m, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = UpdateAdv(m, predicate, mergeSubItems);
            return result.Success;
        }

        /// <summary>
        /// Updates matched entities through <see cref="BatchUpdateAdv"/> and returns its success flag.
        /// </summary>
        /// <param name="items">The incoming models.</param>
        /// <param name="predicate">The predicate selecting candidate entities.</param>
        /// <param name="mergeSubItems">Whether related data is merged after submission.</param>
        /// <returns>The detailed operation success flag.</returns>
        protected virtual bool BatchUpdate(List<V> items, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = BatchUpdateAdv(items, predicate, mergeSubItems);
            return result.Success;
        }

        /// <summary>
        /// Updates the first tracked entity matching a predicate and captures its old mapped value before copying changes.
        /// </summary>
        /// <param name="m">The incoming model.</param>
        /// <param name="predicate">The predicate used to locate the entity.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns>A result containing success and the old-value snapshot.</returns>
        protected virtual DalSingleOperationResult<V> UpdateAdv(V m, Expression<Func<T, bool>> predicate,
            bool mergeSubItems = true)
        {
            var result = new DalSingleOperationResult<V>();

            using (var s = CreateStrategy())
            {
                var entity = s.GetQuery(predicate).FirstOrDefault();
                if (entity == null)
                    return result;

                CopyDataToModel(entity, result.OldValue);

                CopyModelToData(entity, m);
                result.Success = s.SubmitChanges();
            }

            if (mergeSubItems)
            {
                MergeSubItems(m);
            }

            return result;
        }

        /// <summary>
        /// Updates matched tracked entities, recording an old-value snapshot for each incoming item.
        /// </summary>
        /// <param name="items">The incoming models.</param>
        /// <param name="predicate">The predicate selecting candidate entities.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns>A batch result containing success and old-value snapshots.</returns>
        protected virtual DalBatchOperationResult<V> BatchUpdateAdv(List<V> items, Expression<Func<T, bool>> predicate,
            bool mergeSubItems = true)
        {
            var result = new DalBatchOperationResult<V>();

            if (items.Count == 0)
                return result;

            using (var s = CreateStrategy())
            {
                var entities = s.GetQuery(predicate).ToList();
                if (entities.Count == 0)
                    return result;

                foreach (var item in items)
                {
                    var entity = entities.Where(MatchItems(item)).FirstOrDefault();

                    var oldValue = new V();
                    CopyDataToModel(entity, oldValue);
                    result.OldValues.Add(oldValue);

                    CopyModelToData(entity, item);
                }
                result.Success = s.SubmitChanges();
            }

            if (mergeSubItems)
            {
                foreach (var item in items)
                {
                    MergeSubItems(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes all matching tracked entities, deletes their related data first, and records old values.
        /// </summary>
        /// <param name="predicate">The predicate selecting entities to delete.</param>
        /// <returns>A batch result containing success and deleted-value snapshots.</returns>
        protected override DalBatchOperationResult<V> DeleteAdv(Expression<Func<T, bool>> predicate)
        {
            var result = new DalBatchOperationResult<V>();
            using (var s = CreateStrategy())
            {
                var entities = s.GetQuery(predicate).ToList();
                if (entities.Count == 0)
                    return result;

                foreach (var entity in entities)
                {
                    DeleteAllSubItems(entity);

                    s.Delete(entity);
                }
                result.Success = s.SubmitChanges();
                result.OldValues = ListPoco(entities);
            }

            return result;
        }

        /// <summary>
        /// Merges incoming models with the queried set, creating missing entities and deleting entities absent from the input.
        /// </summary>
        /// <param name="items">The complete incoming model set.</param>
        /// <param name="predicate">The predicate selecting the existing set to reconcile.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns>A merge result containing success and old-value snapshots.</returns>
        protected virtual DalMergeOperationResult<V> MergeList(List<V> items, Expression<Func<T, bool>> predicate,
            bool mergeSubItems = true)
        {
            var result = new DalMergeOperationResult<V>();
            using (var s = CreateStrategy())
            {
                var entities = s.GetQuery(predicate).ToList();

                foreach (var entity in entities)
                {
                    var oldValue = new V();
                    CopyDataToModel(entity, oldValue);
                    result.OldValues.Add(oldValue);
                }

                var dictinary = new Dictionary<V, T>();
                foreach (var item in items)
                {
                    var entity = entities.Where(MatchItems(item)).FirstOrDefault();
                    if (entity == null)
                    {
                        entity = new T();
                        s.Add(entity);
                    }

                    dictinary[item] = entity;
                    CopyModelToData(entity, item);
                }

                var deletedEntities = entities.Where(NotMatchItems(items)).ToList();
                foreach (var entity in deletedEntities)
                {
                    DeleteAllSubItems(entity);

                    s.Delete(entity);
                }

                result.Success = s.SubmitChanges();

                foreach (var pair in dictinary)
                {
                    var item = pair.Key;
                    var entity = pair.Value;
                    CopyDataToModel(entity, item);
                }
            }

            if (mergeSubItems)
            {
                foreach (var item in items)
                {
                    MergeSubItems(item);
                }
            }

            return result;
        }

        /// <summary>
        /// Maps model values into a persistence entity before tracking or submission.
        /// </summary>
        /// <param name="data">The persistence entity to populate.</param>
        /// <param name="model">The source model.</param>
        protected abstract void CopyModelToData(T data, V model);

        /// <summary>
        /// Maps persistence values into a model after query materialization or submission.
        /// </summary>
        /// <param name="data">The persistence entity.</param>
        /// <param name="model">The destination model.</param>
        protected abstract void CopyDataToModel(T data, V model);

        /// <summary>
        /// Materializes the first matching entity with no-tracking semantics and maps its related data.
        /// </summary>
        /// <param name="predicate">The predicate used to select the entity.</param>
        /// <returns>The mapped model, or <see langword="null"/> when no entity matches.</returns>
        protected virtual V FindFirstPoco(Expression<Func<T, bool>> predicate)
        {
            using (var s = CreateReadStrategy())
            {
                var item = s.GetQuery(predicate).AsNoTracking().FirstOrDefault();
                return FirstPoco(item);
            }
        }

        /// <summary>
        /// Maps one entity to a model and invokes the single-model related-data hook.
        /// </summary>
        /// <param name="item">The entity to map.</param>
        /// <returns>The mapped model, or <see langword="null"/> for a <see langword="null"/> entity.</returns>
        protected virtual V FirstPoco(T item)
        {
            if (item == null)
                return null;

            var model = new V();
            CopyDataToModel(item, model);
            GetSubItems(model);
            return model;
        }
        
        /// <summary>
        /// Applies a query transformation to a non-tracking main-instance query and materializes the results.
        /// </summary>
        /// <param name="processQuery">The query transformation to apply before materialization.</param>
        /// <returns>Mapped models, including collection related-data processing.</returns>
        protected virtual List<V> FindListPocoMainInstance(Func<IQueryable<T>, IQueryable<T>> processQuery)
        {
            using (var s = CreateStrategy())
            {
                var items = processQuery(s.GetQuery().AsNoTracking()).ToList();
                return ListPoco(items);
            }
            ;
        }

        /// <summary>
        /// Applies a query transformation to a non-tracking read query and materializes the results.
        /// </summary>
        /// <param name="processQuery">The query transformation to apply before materialization.</param>
        /// <returns>Mapped models, including collection related-data processing.</returns>
        protected virtual List<V> FindListPoco(Func<IQueryable<T>, IQueryable<T>> processQuery)
        {
            using (var s = CreateReadStrategy())
            {
                var items = processQuery(s.GetQuery().AsNoTracking()).ToList();
                return ListPoco(items);
            }
            ;
        }

        /// <summary>
        /// Maps an entity sequence to models and invokes the collection related-data hook when non-empty.
        /// </summary>
        /// <param name="items">The entities to map.</param>
        /// <returns>The mapped models.</returns>
        protected virtual List<V> ListPoco(IEnumerable<T> items)
        {
            var result = new List<V>();

            foreach (var item in items)
            {
                var temp = new V();
                CopyDataToModel(item, temp);
                result.Add(temp);
            }

            if (result.Count > 0)
                GetSubItems(result);

            return result;
        }

        /// <summary>
        /// Applies the repository's paging calculation, normalizing page numbers below one to page one.
        /// </summary>
        /// <param name="query">The query to page.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The query with skip and take applied.</returns>
        protected virtual IQueryable<T> GetPagedItems(IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;
            return query.Skip(skip).Take(take);
        }

        /// <summary>
        /// Applies a query transformation and repository paging before materializing models.
        /// </summary>
        /// <param name="processQuery">The query transformation to apply.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The mapped page.</returns>
        protected virtual List<V> FindListPoco(Func<IQueryable<T>, IQueryable<T>> processQuery, int pageNumber,
            int pageSize)
        {
            return FindListPoco(q => GetPagedItems(processQuery(q), pageNumber, pageSize));
        }

        //protected T CreateDataEntity(V model)
        //{
        //	var data = new T();
        //	CopyModelToData(data, model);
        //	return data;
        //}

        /// <summary>
        /// Returns a page containing all models in the repository's default query order.
        /// </summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The requested page of models.</returns>
        public virtual List<V> FindAllPaged(int pageNumber, int pageSize)
        {
            return FindListPoco(q => q.OrderBy(i => true), pageNumber, pageSize);
        }
    }
}
