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
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    // T = the linq to sql proxy class
    // V = your POCO class
    [Serializable]
    public abstract class ConvertingRepositoryBase<T, V>
        where T : class, new()
        where V : class, new()
    {
        protected ILogger logger = new NullLogger();
        protected IRepositoryStrategy<T> repository = null;

        public virtual bool AutoSubmit
        {
            get { return repository.AutoSubmit; }
            set { repository.AutoSubmit = value; }
        }

        // override these to translate between LINQ and POCO
        protected abstract void CopyModelToData(T data, V model);
        protected abstract void CopyDataToModel(T data, V model);

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

        // Use these methods in your Find queries to tranlate from an
        // IQueryable<T> to a single or list of V
        protected virtual V SinglePoco(IQueryable<T> items)
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
        protected virtual V FirstPoco(IQueryable<T> items)
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

        protected virtual List<V> ListPoco(IQueryable<T> items)
        {
            var result = new List<V>();

            if (items != null)
            {
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


        // Note: No longer creates GUID values automatically
        public virtual bool Create(V item)
        {
            var result = false;

            if (item != null)
            {
                var dataObject = new T();
                CopyModelToData(dataObject, item);
                result = repository.Create(dataObject);

                if (result)
                {
                    CopyDataToModel(dataObject, item);
                    MergeSubItems(item);
                    GetSubItems(item);
                }
            }


            return result;
        }

        protected virtual V Find(PrimaryKey key)
        {
            var result = default(V);

            var existing = repository.FindByPrimaryKey(key);
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
            var result = new List<V>();

            if (pageNumber < 1) pageNumber = 1;

            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;

            // Note: silly OrderBy(y => true) is so that entity framework provider
            // won't freak out with skip and take operators.
            // They only work on a sorted result because they are LINQ operators
            var items = repository.Find().OrderBy(y => true).Skip(skip).Take(take);
            if (items != null)
            {
                result = ListPoco(items);
            }

            return result;
        }

        protected virtual IQueryable<T> PageItems(int pageNumber, int pageSize, IQueryable<T> items)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;
            return items.Skip(skip).Take(take);
        }

        protected virtual bool Update(V m, PrimaryKey key)
        {
            var result = false;

            if (m != null)
            {
                try
                {
                    var existing = repository.FindByPrimaryKey(key);
                    if (existing == null)
                    {
                        return false;
                    }
                    CopyModelToData(existing, m);
                    result = repository.SubmitChanges();
                    if (result)
                    {
                        MergeSubItems(m);
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                    logger.LogException(ex);
                }
            }

            return result;
        }

        protected virtual bool Delete(PrimaryKey key)
        {
            DeleteAllSubItems(Find(key));
            return repository.Delete(key);
        }

        public virtual int CountOfAll()
        {
            return repository.CountOfAll();
        }

        public virtual bool SubmitChanges()
        {
            return repository.SubmitChanges();
        }
    }
}