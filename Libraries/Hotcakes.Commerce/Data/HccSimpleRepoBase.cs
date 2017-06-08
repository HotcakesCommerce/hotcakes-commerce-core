#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Linq;
using System.Linq.Expressions;

namespace Hotcakes.Commerce.Data
{
    // T = the linq to sql proxy class
    // V = your POCO class
    public abstract class HccSimpleRepoBase<T, V> : HccRepoBase<T, V>
        where T : class, new()
        where V : class, new()
    {
        protected HccSimpleRepoBase(HccRequestContext context)
            : base(context)
        {
        }

        protected virtual Func<T, bool> MatchItems(V item)
        {
            return t => false;
        }

        protected virtual Func<T, bool> NotMatchItems(List<V> items)
        {
            return t => false;
        }

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

        protected override bool Update(V m, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = UpdateAdv(m, predicate, mergeSubItems);
            return result.Success;
        }

        protected virtual bool BatchUpdate(List<V> items, Expression<Func<T, bool>> predicate, bool mergeSubItems = true)
        {
            var result = BatchUpdateAdv(items, predicate, mergeSubItems);
            return result.Success;
        }

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

        // override these to translate between LINQ and POCO
        protected abstract void CopyModelToData(T data, V model);
        protected abstract void CopyDataToModel(T data, V model);

        protected virtual V FindFirstPoco(Expression<Func<T, bool>> predicate)
        {
            using (var s = CreateStrategy())
            {
                var item = s.GetQuery(predicate).FirstOrDefault();
                return FirstPoco(item);
            }
        }

        protected virtual V FirstPoco(T item)
        {
            if (item == null)
                return null;

            var model = new V();
            CopyDataToModel(item, model);
            GetSubItems(model);
            return model;
        }

        protected virtual List<V> FindListPoco(Func<IQueryable<T>, IQueryable<T>> processQuery)
        {
            using (var s = CreateStrategy())
            {
                var items = processQuery(s.GetQuery()).ToList();
                return ListPoco(items);
            }
            ;
        }

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

        protected virtual IQueryable<T> GetPagedItems(IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;
            return query.Skip(skip).Take(take);
        }

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

        public virtual List<V> FindAllPaged(int pageNumber, int pageSize)
        {
            return FindListPoco(q => q.OrderBy(i => true), pageNumber, pageSize);
        }
    }
}