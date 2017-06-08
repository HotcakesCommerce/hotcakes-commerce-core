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
    public abstract class HccRepoBase<T, V> : IRepo
        where T : class, new()
        where V : class, new()
    {
        protected HccRepoBase(HccRequestContext context)
        {
            Context = context;
        }

        protected HccRequestContext Context { get; private set; }

        public abstract bool Create(V item);
        protected abstract bool Update(V m, Expression<Func<T, bool>> predicate, bool mergeSubItems = true);

        protected virtual bool Delete(Expression<Func<T, bool>> predicate)
        {
            var result = DeleteAdv(predicate);
            return result.Success;
        }

        protected abstract DalBatchOperationResult<V> DeleteAdv(Expression<Func<T, bool>> predicate);

        protected virtual void GetSubItems(List<V> items)
        {
        }

        protected void GetSubItems(V item)
        {
            GetSubItems(new List<V> {item});
        }

        protected virtual void MergeSubItems(V model)
        {
        }

        protected virtual void DeleteAllSubItems(T data)
        {
        }

        public virtual int CountOfAll()
        {
            using (var s = CreateStrategy())
            {
                return s.GetQuery().Count();
            }
        }

        protected IRepoStrategy<T> CreateStrategy()
        {
            return Factory.Instance.CreateStrategy<T>();
        }
    }
}