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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Data
{
    public abstract class HccLocalizationRepoBase<T, TT, V, TKey> : HccRepoBase<T, V>
        where T : class, new()
        where TT : class, ITranslationEntity, new()
        where V : class, new()
        where TKey : struct
    {
        protected HccLocalizationRepoBase(HccRequestContext context)
            : base(context)
        {
        }

        protected abstract Expression<Func<T, TKey>> ItemKeyExp { get; }
        protected abstract Expression<Func<TT, TKey>> ItemTranslationKeyExp { get; }

        protected virtual Func<JoinedItem<T, TT>, bool> MatchItems(V item)
        {
            return t => false;
        }

        protected virtual Func<JoinedItem<T, TT>, bool> NotMatchItems(List<V> items)
        {
            return t => false;
        }

        public override bool Create(V item)
        {
            return Create(item, false);
        }

        public virtual bool BatchCreate(List<V> items)
        {
            return BatchCreate(items, false);
        }

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

        protected override bool Update(V m, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = UpdateAdv(m, predicate, false, mergeSubItems);
            return result.Success;
        }

        protected virtual bool BatchUpdate(List<V> items, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = BatchUpdateAdv(items, predicate, false, mergeSubItems);
            return result.Success;
        }

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

        protected void UpdateTransCulture(TT trans, ILocalizableModel m, bool useModelCulture)
        {
            if (useModelCulture && m != null)
                trans.Culture = m.ContentCulture;
            else
                trans.Culture = Context.MainContentCulture;
        }

        // override these to translate between LINQ and POCO
        protected abstract void CopyModelToItem(JoinedItem<T, TT> data, V model);
        protected abstract void CopyModelToTrans(JoinedItem<T, TT> data, V model);
        protected abstract void CopyItemToModel(T data, V model);
        protected abstract void CopyTransToModel(TT data, V model);

        protected virtual void CopyDataToModel(JoinedItem<T, TT> data, V model)
        {
            CopyItemToModel(data.Item, model);
            if (data.ItemTranslation != null)
                CopyTransToModel(data.ItemTranslation, model);
        }

        protected virtual bool HasTranslation(TT trans)
        {
            return true;
        }

        protected virtual V FindFirstPoco(Expression<Func<JoinedItem<T, TT>, bool>> predicate)
        {
            using (var s = CreateReadStrategy())
            {
                var item = GetJoinedQuery(s).AsNoTracking().FirstOrDefault(predicate);
                return FirstPoco(item);
            }
        }

        protected virtual V FirstPoco(JoinedItem<T, TT> item)
        {
            if (item == null)
                return null;

            var model = new V();
            CopyDataToModel(item, model);
            GetSubItems(model);
            return model;
        }

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

        protected virtual IQueryable<JoinedItem<T, TT>> GetPagedItems(IQueryable<JoinedItem<T, TT>> query,
            int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;
            return query.Skip(skip).Take(take);
        }

        protected virtual List<V> FindListPoco(
            Func<IQueryable<JoinedItem<T, TT>>, IQueryable<JoinedItem<T, TT>>> processQuery, int pageNumber,
            int pageSize)
        {
            return FindListPoco(q => GetPagedItems(processQuery(q), pageNumber, pageSize));
        }

        public virtual List<V> FindAllPaged(int pageNumber, int pageSize)
        {
            return FindListPoco(q => q.OrderBy(i => true), pageNumber, pageSize);
        }

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