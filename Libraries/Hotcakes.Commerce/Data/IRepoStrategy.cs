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
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TDefaultEntity"></typeparam>
    public interface IRepoStrategy<TDefaultEntity> : IDisposable where TDefaultEntity : class
    {
        ILogger Logger { get; set; }
        bool SubmitChanges();

        IQueryable<TDefaultEntity> GetQuery(Expression<Func<TDefaultEntity, bool>> predicate = null);
        IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class;

        void Add(TDefaultEntity item);
        void AddEntity<TEntity>(TEntity item) where TEntity : class;

        void Delete(TDefaultEntity item);
        void DeleteEntity<TEntity>(TEntity item) where TEntity : class;

        IList<TResult> ExecFunction<TResult>(string functionName, object parameters);

        void ExecScalarFunction(string functionName, List<KeyValuePair<string, object>> parameters);

        void Detach(object item);

        void AutoDetectChanges(bool detect);
    }
}