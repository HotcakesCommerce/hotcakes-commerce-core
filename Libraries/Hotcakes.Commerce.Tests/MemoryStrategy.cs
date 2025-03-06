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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Hotcakes.Commerce.Data;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Tests
{
    internal class MemoryStrategy<TDefaultEntity> : IRepoStrategy<TDefaultEntity> where TDefaultEntity : class
    {
        private static int _identityCounter;

        private Dictionary<Type, object> _objectSets = new Dictionary<Type, object>();

        public MemoryStrategy()
        {
            Logger = new NullLogger();
        }

        public ILogger Logger { get; set; }

        public bool SubmitChanges()
        {
            return true;
        }

        public IQueryable<TDefaultEntity> GetQuery(Expression<Func<TDefaultEntity, bool>> predicate = null)
        {
            return GetQuery<TDefaultEntity>(predicate);
        }

        public IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            var q = GetObjectSet<TEntity>().AsQueryable();

            if (predicate != null)
                q = q.Where(predicate);

            return q;
        }

        public void Add(TDefaultEntity item)
        {
            AddEntity(item);
        }

        public void AddEntity<TEntity>(TEntity item) where TEntity : class
        {
            UpdateIdentityColumn(item);
            GetObjectSet<TEntity>().Add(item);
        }

        public void Delete(TDefaultEntity item)
        {
            DeleteEntity(item);
        }

        public void DeleteEntity<TEntity>(TEntity item) where TEntity : class
        {
            GetObjectSet<TEntity>().Remove(item);
        }

        public IList<TResult> ExecFunction<TResult>(string functionName, object parameters)
        {
            return new List<TResult>();
        }

        public void ExecScalarFunction(string functionName, List<KeyValuePair<string, object>> parameters)
        {
        }

        public void Dispose()
        {
        }

        public void Detach(object item)
        {
            throw new NotImplementedException();
        }

        public void AutoDetectChanges(bool detect)
        {
        }

        private List<TEntity> GetObjectSet<TEntity>() where TEntity : class
        {
            return MemoryStrategyFactory.CreateItems<TEntity>();
        }

        private void UpdateIdentityColumn<TEntity>(TEntity item)
        {
            var prop = item.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

            if (prop != null &&
                prop.CanWrite &&
                (prop.PropertyType == typeof (int) || prop.PropertyType == typeof (long)))
            {
                prop.SetValue(item, _identityCounter, null);
                _identityCounter++;
            }
        }
    }
}