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
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    // T  = the linq to sql proxy class
    // TT = translation of T
    // V  = your POCO class
    [Serializable]
    public abstract class LocalizationRepositoryBase<T, TT, V, TKey>
        where T : class, new()
        where TT : class, ITranslationEntity, new()
        where V : class, new()
        where TKey : struct
    {
        protected ILogger _logger = new NullLogger();
        protected IRepositoryStrategy<T> _rep = null;
        protected IRepositoryStrategy<TT> _repTrans = null;

        protected string MainCulture { get; set; }
        protected string FallbackCulture { get; set; }

        protected abstract Expression<Func<T, TKey>> ItemKeyExp { get; }
        protected abstract Expression<Func<TT, TKey>> ItemTranslationKeyExp { get; }

        public virtual bool AutoSubmit
        {
            get { return _rep.AutoSubmit; }
            set { _rep.AutoSubmit = value; }
        }

        protected abstract Expression<Func<JoinedItem<T, TT>, bool>> GetSelectorByPrimaryKey(PrimaryKey key);

        // override these to translate between LINQ and POCO
        protected abstract void CopyModelToItem(JoinedItem<T, TT> data, V model);
        protected abstract void CopyModelToTrans(JoinedItem<T, TT> data, V model);
        protected abstract void CopyItemToModel(T data, V model);
        protected abstract void CopyTransToModel(TT data, V model);

        protected virtual void GetSubItems(V model)
        {
        }

        protected virtual void MergeSubItems(V model)
        {
        }

        protected virtual void DeleteAllSubItems(V model)
        {
        }

        protected virtual void GetSubItems(List<V> models)
        {
            foreach (var model in models)
            {
                GetSubItems(model);
            }
        }

        protected virtual void CopyDataToModel(JoinedItem<T, TT> data, V model)
        {
            CopyItemToModel(data.Item, model);
            if (data.ItemTranslation != null)
                CopyTransToModel(data.ItemTranslation, model);
        }

        // Use these methods in your Find queries to tranlate from an
        // IQueryable<T> to a single or list of V
        protected virtual V SinglePoco(IQueryable<JoinedItem<T, TT>> items)
        {
            var item = items.SingleOrDefault();
            if (item != null)
            {
                var result = new V();
                CopyDataToModel(item, result);
                GetSubItems(result);
                return result;
            }

            return null;
        }

        // Use these methods in your Find queries to tranlate from an
        // IQueryable<T> to a single or list of V
        protected virtual V FirstPoco(IQueryable<JoinedItem<T, TT>> items)
        {
            var item = items.FirstOrDefault();
            if (item != null)
            {
                var result = new V();
                CopyDataToModel(item, result);
                GetSubItems(result);
                return result;
            }

            return null;
        }

        protected virtual List<V> ListPoco(IQueryable<JoinedItem<T, TT>> itemsQuery)
        {
            var result = new List<V>();

            if (itemsQuery != null)
            {
                var items = itemsQuery.ToList();
                foreach (var item in items)
                {
                    var temp = new V();
                    CopyDataToModel(item, temp);
                    result.Add(temp);
                }

                GetSubItems(result);
            }

            return result;
        }

        public virtual bool Create(V item)
        {
            return Create(item, false);
        }

        public virtual bool Create(V item, bool useModelCulture = false)
        {
            if (item == null)
                return false;

            var result = true;
            var dataObject = new JoinedItem<T, TT>();
            CopyModelToItem(dataObject, item);
            result &= _rep.Create(dataObject.Item);
            CopyModelToTrans(dataObject, item);
            if (useModelCulture && item is ILocalizableModel)
                dataObject.ItemTranslation.Culture = (item as ILocalizableModel).ContentCulture;
            else
                dataObject.ItemTranslation.Culture = MainCulture;
            result &= _repTrans.Create(dataObject.ItemTranslation);
            result &= _rep.SubmitChanges();

            if (result)
            {
                CopyDataToModel(dataObject, item);
                MergeSubItems(item);
                GetSubItems(item);
            }
            return result;
        }

        protected virtual V Find(PrimaryKey key)
        {
            var result = default(V);

            var existing = FindByPrimaryKey(key);
            if (existing != null)
            {
                result = new V();
                CopyDataToModel(existing, result);
                GetSubItems(result);
            }
            else
            {
                return null;
            }

            return result;
        }

        public virtual List<V> FindAllPaged(int pageNumber, int pageSize)
        {
            // Note: silly OrderBy(y => true) is so that entity framework provider
            // won't freak out with skip and take operators.
            // They only work on a sorted result because they are LINQ operators
            IQueryable<JoinedItem<T, TT>> items = Find().OrderBy(y => true);
            items = PageItems(pageNumber, pageSize, items);
            if (items != null)
            {
                return ListPoco(items);
            }

            return new List<V>();
        }

        protected IQueryable<JoinedItem<T, TT>> Find()
        {
            return FindInternal();
        }

        protected IQueryable<JoinedItem<T, TT>> FindInternal(bool singleCulture = false)
        {
            var objSet = _rep.Find();
            var translationSet =
                _repTrans.Find()
                    .Where(it => it.Culture == MainCulture || (!singleCulture && it.Culture == FallbackCulture));

            return objSet.GroupJoin(translationSet, ItemKeyExp, ItemTranslationKeyExp,
                (i, it) =>
                    new JoinedItem<T, TT>
                    {
                        Item = i,
                        ItemTranslation = it.OrderBy(iit => iit.Culture == MainCulture ? 1 : 2).FirstOrDefault()
                    });
        }

        protected JoinedItem<T, TT> FindByPrimaryKey(PrimaryKey key)
        {
            return FindByPrimaryKeyInternal(key);
        }

        protected JoinedItem<T, TT> FindByPrimaryKeyInternal(PrimaryKey key, bool singleCulture = false)
        {
            var selector = GetSelectorByPrimaryKey(key);
            return FindInternal(singleCulture).Where(selector).FirstOrDefault();
        }

        protected virtual IQueryable<JoinedItem<T, TT>> PageItems(int pageNumber, int pageSize,
            IQueryable<JoinedItem<T, TT>> items)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;
            return items.Skip(skip).Take(take);
        }

        protected virtual bool Update(V m, PrimaryKey key, bool mergeSubItems = true, bool useModelCulture = false)
        {
            if (m == null)
                return false;

            var result = true;
            try
            {
                var existing = FindByPrimaryKeyInternal(key, true);
                if (existing == null)
                    return false;

                var createTrans = false;
                if (existing.ItemTranslation == null)
                {
                    createTrans = true;
                    existing.ItemTranslation = new TT();
                }
                CopyModelToItem(existing, m);
                result &= _rep.SubmitChanges();
                CopyModelToTrans(existing, m);
                if (useModelCulture && m is ILocalizableModel)
                    existing.ItemTranslation.Culture = (m as ILocalizableModel).ContentCulture;
                else
                    existing.ItemTranslation.Culture = MainCulture;
                if (createTrans)
                {
                    _repTrans.Create(existing.ItemTranslation);
                }
                result &= _repTrans.SubmitChanges();
                if (result && mergeSubItems)
                {
                    CopyDataToModel(existing, m);
                    MergeSubItems(m);
                }
            }
            catch (Exception ex)
            {
                result = false;
                _logger.LogException(ex);
            }
            return result;
        }

        protected virtual bool Delete(PrimaryKey key)
        {
            DeleteAllSubItems(Find(key));
            return _rep.Delete(key);
        }

        public virtual int CountOfAll()
        {
            return _rep.CountOfAll();
        }

        public virtual bool SubmitChanges()
        {
            return _rep.SubmitChanges() && _repTrans.SubmitChanges();
        }
    }
}