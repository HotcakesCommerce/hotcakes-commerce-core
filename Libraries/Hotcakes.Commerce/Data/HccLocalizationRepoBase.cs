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
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// Provides repository operations for a primary entity and its culture-specific translation entity.
    /// </summary>
    /// <typeparam name="T">The primary persistence entity type.</typeparam>
    /// <typeparam name="TT">The translation entity type.</typeparam>
    /// <typeparam name="V">The localized POCO model type.</typeparam>
    /// <typeparam name="TKey">The key type shared by the primary and translation entities.</typeparam>
    public abstract class HccLocalizationRepoBase<T, TT, V, TKey> : HccRepoBase<T, V>
        where T : class, new()
        where TT : class, ITranslationEntity, new()
        where V : class, new()
        where TKey : struct
    {
        /// <summary>
        /// Initializes a localization repository with its commerce request context.
        /// </summary>
        /// <param name="context">The request context providing main and fallback cultures.</param>
        protected HccLocalizationRepoBase(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets the expression selecting the primary entity key used for the translation join.
        /// </summary>
        protected abstract Expression<Func<T, TKey>> ItemKeyExp { get; }

        /// <summary>
        /// Gets the expression selecting the translation entity key used for the primary join.
        /// </summary>
        protected abstract Expression<Func<TT, TKey>> ItemTranslationKeyExp { get; }

        /// <summary>
        /// Creates the in-memory match used to associate a localized model with a joined item.
        /// </summary>
        /// <param name="item">The localized model being matched.</param>
        /// <returns>A predicate over joined items. The default predicate matches nothing.</returns>
        protected virtual Func<JoinedItem<T, TT>, bool> MatchItems(V item)
        {
            return t => false;
        }

        /// <summary>
        /// Creates the in-memory predicate identifying joined items absent from a merge input.
        /// </summary>
        /// <param name="items">The complete incoming model set.</param>
        /// <returns>A predicate over joined items. The default predicate matches nothing.</returns>
        protected virtual Func<JoinedItem<T, TT>, bool> NotMatchItems(List<V> items)
        {
            return t => false;
        }

        /// <summary>
        /// Creates a localized model using the request context's main content culture.
        /// </summary>
        /// <param name="item">The localized model to create.</param>
        /// <returns><see langword="true"/> when persistence succeeds.</returns>
        public override bool Create(V item)
        {
            return Create(item, false);
        }

        /// <summary>
        /// Creates localized models using the request context's main content culture.
        /// </summary>
        /// <param name="items">The models to create.</param>
        /// <returns><see langword="true"/> when both persistence submissions succeed.</returns>
        public virtual bool BatchCreate(List<V> items)
        {
            return BatchCreate(items, false);
        }

        /// <summary>
        /// Creates a localized model, optionally using the model's content culture instead of the main culture.
        /// </summary>
        /// <param name="item">The localized model to create.</param>
        /// <param name="useModelCulture">Whether to use <see cref="ILocalizableModel.ContentCulture"/> when available.</param>
        /// <returns><see langword="true"/> when the primary and applicable translation submissions succeed.</returns>
        public virtual bool Create(V item, bool useModelCulture)
        {
            var result = false;

            var entity = new JoinedItem<T, TT>();

            using (var s = CreateStrategy())
            {
                CopyModelToItem(entity, item);
                s.Add(entity.Item);
                result = s.SubmitChanges();

                CopyModelToTrans(entity, item);
                UpdateTransCulture(entity.ItemTranslation, item as ILocalizableModel, useModelCulture);

                if (HasTranslation(entity.ItemTranslation))
                {
                    s.AddEntity(entity.ItemTranslation);
                    result &= s.SubmitChanges();
                }
            }

            CopyDataToModel(entity, item);
            MergeSubItems(item);

            return result;
        }

        /// <summary>
        /// Creates localized models in batches, then optionally merges their related data.
        /// </summary>
        /// <param name="items">The models to create.</param>
        /// <param name="useModelCulture">Whether each model's content culture controls its translation.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns><see langword="true"/> when both batch submissions succeed.</returns>
        public virtual bool BatchCreate(List<V> items, bool useModelCulture, bool mergeSubItems = true)
        {
            var result = false;

            if (items.Count == 0)
                return result;

            var entities = new List<JoinedItem<T, TT>>();
            using (var s = CreateStrategy())
            {
                s.AutoDetectChanges(false);

                foreach (var item in items)
                {
                    var entity = new JoinedItem<T, TT>();
                    CopyModelToItem(entity, item);
                    entities.Add(entity);
                    s.Add(entity.Item);
                }
                result = s.SubmitChanges();

                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var entity = entities[i];
                    CopyModelToTrans(entity, item);
                    UpdateTransCulture(entity.ItemTranslation, item as ILocalizableModel, useModelCulture);

                    if (HasTranslation(entity.ItemTranslation))
                    {
                        s.AddEntity(entity.ItemTranslation);
                    }
                }
                result &= s.SubmitChanges();
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
        /// Updates a localized item using the request context's main culture.
        /// </summary>
        /// <param name="m">The localized model values.</param>
        /// <param name="predicate">The predicate selecting the primary entity.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns><see langword="true"/> when the operation succeeds.</returns>
        protected override bool Update(V m, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = UpdateAdv(m, predicate, false, mergeSubItems);
            return result.Success;
        }

        /// <summary>
        /// Updates localized items using the request context's main culture.
        /// </summary>
        /// <param name="items">The incoming localized models.</param>
        /// <param name="predicate">The predicate selecting primary entities.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns><see langword="true"/> when the operation succeeds.</returns>
        protected virtual bool BatchUpdate(List<V> items, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = BatchUpdateAdv(items, predicate, false, mergeSubItems);
            return result.Success;
        }

        /// <summary>
        /// Updates a localized item, captures its old mapped value, and creates a translation when needed.
        /// </summary>
        /// <param name="m">The incoming model.</param>
        /// <param name="predicate">The predicate selecting the primary entity.</param>
        /// <param name="useModelCulture">Whether the model content culture controls translation selection.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns>A result containing success and the old-value snapshot.</returns>
        protected virtual DalSingleOperationResult<V> UpdateAdv(V m, Expression<Func<T, bool>> predicate,
            bool useModelCulture, bool mergeSubItems)
        {
            var result = new DalSingleOperationResult<V>();

            using (var s = CreateStrategy())
            {
                var entity = GetJoinedQuery(s, predicate, true).FirstOrDefault();
                if (entity == null)
                    return result;

                CopyDataToModel(entity, result.OldValue);

                var createTrans = false;
                if (entity.ItemTranslation == null)
                {
                    createTrans = true;
                    entity.ItemTranslation = new TT();
                }
                CopyModelToItem(entity, m);
                result.Success = s.SubmitChanges();

                CopyModelToTrans(entity, m);
                UpdateTransCulture(entity.ItemTranslation, m as ILocalizableModel, useModelCulture);

                if (HasTranslation(entity.ItemTranslation))
                {
                    if (createTrans)
                    {
                        s.AddEntity(entity.ItemTranslation);
                    }

                    result.Success &= s.SubmitChanges();
                }
                if (result.Success)
                {
                    CopyDataToModel(entity, m);
                }
            }

            if (mergeSubItems)
            {
                MergeSubItems(m);
            }

            return result;
        }

        /// <summary>
        /// Batch-updates localized items, recording old snapshots before primary and translation submissions.
        /// </summary>
        /// <param name="items">The incoming localized models.</param>
        /// <param name="predicate">The predicate selecting primary entities.</param>
        /// <param name="useModelCulture">Whether model content cultures control translations.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns>A batch result containing success and old-value snapshots.</returns>
        protected virtual DalBatchOperationResult<V> BatchUpdateAdv(List<V> items, Expression<Func<T, bool>> predicate,
            bool useModelCulture, bool mergeSubItems)
        {
            var result = new DalBatchOperationResult<V>();

            if (items.Count == 0)
                return result;

            using (var s = CreateStrategy())
            {
                var entities = GetJoinedQuery(s, predicate, true).ToList();
                if (entities.Count == 0)
                    return result;

                foreach (var item in items)
                {
                    var entity = entities.Where(MatchItems(item)).FirstOrDefault();

                    var oldValue = new V();
                    CopyDataToModel(entity, oldValue);
                    result.OldValues.Add(oldValue);

                    CopyModelToItem(entity, item);
                }

                result.Success = s.SubmitChanges();

                foreach (var item in items)
                {
                    var entity = entities.Where(MatchItems(item)).FirstOrDefault();

                    var createTrans = false;
                    if (entity.ItemTranslation == null)
                    {
                        createTrans = true;
                        entity.ItemTranslation = new TT();
                    }

                    CopyModelToTrans(entity, item);
                    UpdateTransCulture(entity.ItemTranslation, item as ILocalizableModel, useModelCulture);

                    if (createTrans && HasTranslation(entity.ItemTranslation))
                    {
                        s.AddEntity(entity.ItemTranslation);
                    }
                }
                result.Success &= s.SubmitChanges();

                foreach (var item in items)
                {
                    var entity = entities.Where(MatchItems(item)).FirstOrDefault();
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
        /// Deletes matching primary entities and their related data, returning localized old-value snapshots.
        /// </summary>
        /// <param name="predicate">The predicate selecting primary entities.</param>
        /// <returns>A batch deletion result.</returns>
        protected override DalBatchOperationResult<V> DeleteAdv(Expression<Func<T, bool>> predicate)
        {
            var result = new DalBatchOperationResult<V>();
            using (var s = CreateStrategy())
            {
                var entities = GetJoinedQuery(s, predicate, true).ToList();
                if (entities.Count == 0)
                    return result;

                foreach (var entity in entities)
                {
                    DeleteAllSubItems(entity.Item);

                    s.Delete(entity.Item);
                }
                result.Success = s.SubmitChanges();
                result.OldValues = ListPoco(entities);
            }

            return result;
        }

        /// <summary>
        /// Reconciles localized items: creates missing entities, updates translations, and deletes absent entities.
        /// </summary>
        /// <param name="items">The complete incoming localized model set.</param>
        /// <param name="predicate">The predicate selecting the existing set.</param>
        /// <param name="useModelCulture">Whether model content cultures control translations.</param>
        /// <param name="mergeSubItems">Whether related data is merged after persistence.</param>
        /// <returns>A merge result containing success and old-value snapshots.</returns>
        protected virtual DalMergeOperationResult<V> MergeList(List<V> items, Expression<Func<T, bool>> predicate,
            bool useModelCulture = false, bool mergeSubItems = true)
        {
            var result = new DalMergeOperationResult<V>();
            using (var s = CreateStrategy())
            {
                var entities = GetJoinedQuery(s, predicate, true).ToList();

                foreach (var entity in entities)
                {
                    var oldValue = new V();
                    CopyDataToModel(entity, oldValue);
                    result.OldValues.Add(oldValue);
                }

                var dictinary = new Dictionary<V, JoinedItem<T, TT>>();
                foreach (var item in items)
                {
                    var entity = entities.Where(MatchItems(item)).FirstOrDefault();
                    if (entity == null)
                    {
                        entity = new JoinedItem<T, TT>();
                        entity.ItemTranslation = null;
                        s.Add(entity.Item);
                    }

                    dictinary[item] = entity;
                    CopyModelToItem(entity, item);
                }

                result.Success = s.SubmitChanges();

                foreach (var pair in dictinary)
                {
                    var item = pair.Key;
                    var entity = pair.Value;

                    var createTrans = false;
                    if (entity.ItemTranslation == null)
                    {
                        createTrans = true;
                        entity.ItemTranslation = new TT();
                    }

                    CopyModelToTrans(entity, item);
                    UpdateTransCulture(entity.ItemTranslation, item as ILocalizableModel, useModelCulture);

                    if (createTrans && HasTranslation(entity.ItemTranslation))
                    {
                        s.AddEntity(entity.ItemTranslation);
                    }
                }

                var deletedEntities = entities.Where(NotMatchItems(items)).ToList();
                foreach (var entity in deletedEntities)
                {
                    DeleteAllSubItems(entity.Item);

                    s.Delete(entity.Item);
                }

                result.Success &= s.SubmitChanges();

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
        /// Sets a translation culture from the model when requested; otherwise uses the request context's main culture.
        /// The fallback culture is used only when selecting joined translations, not when writing this value.
        /// </summary>
        /// <param name="trans">The translation entity to update.</param>
        /// <param name="m">The optional localized model supplying its content culture.</param>
        /// <param name="useModelCulture">Whether the model culture should be used when available.</param>
        protected void UpdateTransCulture(TT trans, ILocalizableModel m, bool useModelCulture)
        {
            if (useModelCulture && m != null)
                trans.Culture = m.ContentCulture;
            else
                trans.Culture = Context.MainContentCulture;
        }

        /// <summary>Maps model values into the primary entity of a joined item.</summary>
        /// <param name="data">The joined item containing the destination entity.</param>
        /// <param name="model">The source model.</param>
        protected abstract void CopyModelToItem(JoinedItem<T, TT> data, V model);
        /// <summary>Maps model translation values into the translation entity of a joined item.</summary>
        /// <param name="data">The joined item containing the destination translation.</param>
        /// <param name="model">The source model.</param>
        protected abstract void CopyModelToTrans(JoinedItem<T, TT> data, V model);
        /// <summary>Maps the primary entity into a model.</summary>
        /// <param name="data">The primary entity.</param>
        /// <param name="model">The destination model.</param>
        protected abstract void CopyItemToModel(T data, V model);
        /// <summary>Maps a translation entity into a model.</summary>
        /// <param name="data">The translation entity.</param>
        /// <param name="model">The destination model.</param>
        protected abstract void CopyTransToModel(TT data, V model);

        /// <summary>
        /// Maps the primary entity and, when present, its selected translation into a model.
        /// </summary>
        /// <param name="data">The joined item to map.</param>
        /// <param name="model">The destination model.</param>
        protected virtual void CopyDataToModel(JoinedItem<T, TT> data, V model)
        {
            CopyItemToModel(data.Item, model);
            if (data.ItemTranslation != null)
                CopyTransToModel(data.ItemTranslation, model);
        }

        /// <summary>
        /// Determines whether a translation entity contains data worth persisting.
        /// </summary>
        /// <param name="trans">The translation entity under consideration.</param>
        /// <returns><see langword="true"/> when the translation should be persisted.</returns>
        protected virtual bool HasTranslation(TT trans)
        {
            return true;
        }

        /// <summary>
        /// Materializes the first joined item with no tracking and maps the selected culture translation.
        /// </summary>
        /// <param name="predicate">The predicate applied to joined items.</param>
        /// <returns>The mapped model, or <see langword="null"/> when none matches.</returns>
        protected virtual V FindFirstPoco(Expression<Func<JoinedItem<T, TT>, bool>> predicate)
        {
            using (var s = CreateReadStrategy())
            {
                var item = GetJoinedQuery(s).AsNoTracking().FirstOrDefault(predicate);
                return FirstPoco(item);
            }
        }

        /// <summary>
        /// Maps a joined item and invokes the single-model related-data hook.
        /// </summary>
        /// <param name="item">The joined item to map.</param>
        /// <returns>The mapped model, or <see langword="null"/> when the item is null.</returns>
        protected virtual V FirstPoco(JoinedItem<T, TT> item)
        {
            if (item == null)
                return null;

            var model = new V();
            CopyDataToModel(item, model);
            GetSubItems(model);
            return model;
        }

        /// <summary>
        /// Applies a query transformation to a no-tracking localized query and materializes the results.
        /// </summary>
        /// <param name="processQuery">The query transformation to apply.</param>
        /// <returns>Mapped localized models.</returns>
        protected virtual List<V> FindListPoco(
            Func<IQueryable<JoinedItem<T, TT>>, IQueryable<JoinedItem<T, TT>>> processQuery)
        {
            using (var s = CreateReadStrategy())
            {
                var items = processQuery(GetJoinedQuery(s).AsNoTracking()).ToList();
                return ListPoco(items);
            }
            ;
        }

        /// <summary>
        /// Maps joined items to models and invokes collection related-data processing when non-empty.
        /// </summary>
        /// <param name="items">The joined items to map.</param>
        /// <returns>The mapped localized models.</returns>
        protected virtual List<V> ListPoco(IEnumerable<JoinedItem<T, TT>> items)
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
        /// Applies one-based paging to a localized joined query.
        /// </summary>
        /// <param name="query">The query to page.</param>
        /// <param name="pageNumber">The one-based page number; values below one use page one.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The query with skip and take applied.</returns>
        protected virtual IQueryable<JoinedItem<T, TT>> GetPagedItems(IQueryable<JoinedItem<T, TT>> query,
            int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;
            return query.Skip(skip).Take(take);
        }

        /// <summary>
        /// Applies a localized query transformation and paging before materialization.
        /// </summary>
        /// <param name="processQuery">The query transformation to apply.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The requested mapped page.</returns>
        protected virtual List<V> FindListPoco(
            Func<IQueryable<JoinedItem<T, TT>>, IQueryable<JoinedItem<T, TT>>> processQuery, int pageNumber,
            int pageSize)
        {
            return FindListPoco(q => GetPagedItems(processQuery(q), pageNumber, pageSize));
        }

        /// <summary>
        /// Returns a page of localized models in the repository's default query order.
        /// </summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>The requested page.</returns>
        public virtual List<V> FindAllPaged(int pageNumber, int pageSize)
        {
            return FindListPoco(q => q.OrderBy(i => true), pageNumber, pageSize);
        }

        /// <summary>
        /// Builds the primary-to-translation group join, selecting main-culture translations before fallback translations.
        /// When <paramref name="singleCulture"/> is true, only the main content culture is considered.
        /// </summary>
        /// <param name="strategy">The strategy supplying primary and translation queries.</param>
        /// <param name="predicate">An optional predicate for the primary entity query.</param>
        /// <param name="singleCulture">Whether fallback-culture translations are excluded.</param>
        /// <returns>A query of joined items whose selected translation is main culture first, then fallback.</returns>
        protected IQueryable<JoinedItem<T, TT>> GetJoinedQuery(IRepoStrategy<T> strategy,
            Expression<Func<T, bool>> predicate = null, bool singleCulture = false)
        {
            var mainCulture = Context.MainContentCulture;
            var fallbackCulture = Context.FallbackContentCulture;

            var objSet = predicate == null ? strategy.GetQuery() : strategy.GetQuery(predicate);
            var translationSet =
                strategy.GetQuery<TT>(
                    it => it.Culture == mainCulture || (!singleCulture && it.Culture == fallbackCulture));

            return objSet.GroupJoin(translationSet, ItemKeyExp, ItemTranslationKeyExp,
                (i, it) =>
                    new JoinedItem<T, TT>
                    {
                        Item = i,
                        ItemTranslation = it.OrderBy(iit => iit.Culture == mainCulture ? 1 : 2).FirstOrDefault()
                    });
        }
    }
}
