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
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// Defines the unit-of-work operations used by Hotcakes Commerce repositories.
    /// </summary>
    /// <typeparam name="TDefaultEntity">The default entity type for non-generic query and mutation methods.</typeparam>
    /// <remarks>
    /// Implementations wrap the repository's data context and are disposable. Instances created
    /// for read-only work may expose the same query surface while using a read-only context;
    /// callers should use the factory's corresponding creation method for that behavior.
    /// </remarks>
    public interface IRepoStrategy<TDefaultEntity> : IDisposable where TDefaultEntity : class
    {
        /// <summary>
        /// Gets or sets the logger used to record strategy-level diagnostics.
        /// </summary>
        /// <value>The logger associated with this strategy.</value>
        ILogger Logger { get; set; }

        /// <summary>
        /// Persists pending changes in the underlying data context.
        /// </summary>
        /// <returns><c>true</c> when the strategy reports that changes were submitted; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The current Entity Framework implementation logs a concurrency exception and still
        /// returns <c>true</c> after that exception. This documented behavior is an implementation
        /// quirk and is not changed by this interface contract.
        /// </remarks>
        bool SubmitChanges();

        /// <summary>
        /// Creates a query for the default entity type, optionally constrained by a predicate.
        /// </summary>
        /// <param name="predicate">An optional filter expression; <c>null</c> returns the unfiltered entity query.</param>
        /// <returns>An <see cref="IQueryable{T}" /> for <typeparamref name="TDefaultEntity" />.</returns>
        IQueryable<TDefaultEntity> GetQuery(Expression<Func<TDefaultEntity, bool>> predicate = null);

        /// <summary>
        /// Creates a query for an entity type, optionally constrained by a predicate.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to query.</typeparam>
        /// <param name="predicate">An optional filter expression; <c>null</c> returns the unfiltered entity query.</param>
        /// <returns>An <see cref="IQueryable{T}" /> for <typeparamref name="TEntity" />.</returns>
        IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class;

        /// <summary>
        /// Adds an instance of the default entity type to the pending unit of work.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        void Add(TDefaultEntity item);

        /// <summary>
        /// Adds an entity of the specified type to the pending unit of work.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to add.</typeparam>
        /// <param name="item">The entity to add.</param>
        void AddEntity<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// Marks an instance of the default entity type for deletion.
        /// </summary>
        /// <param name="item">The entity to delete.</param>
        void Delete(TDefaultEntity item);

        /// <summary>
        /// Marks an entity of the specified type for deletion.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to delete.</typeparam>
        /// <param name="item">The entity to delete.</param>
        void DeleteEntity<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// Executes a mapped data-store function and returns its result rows.
        /// </summary>
        /// <typeparam name="TResult">The result-row type returned by the function.</typeparam>
        /// <param name="functionName">The mapped function name to execute.</param>
        /// <param name="parameters">An object whose readable properties provide function parameters.</param>
        /// <returns>The result rows returned by the function.</returns>
        IList<TResult> ExecFunction<TResult>(string functionName, object parameters);

        /// <summary>
        /// Executes a scalar or command-style data-store function without returning result rows.
        /// </summary>
        /// <param name="functionName">The function or command name to execute.</param>
        /// <param name="parameters">The ordered parameter names and values supplied to the command.</param>
        /// <remarks>
        /// The current Entity Framework implementation builds a command string from the supplied
        /// names and values rather than using parameter objects. The values are therefore part of
        /// the generated command text, and callers must preserve the implementation's expected input.
        /// </remarks>
        void ExecScalarFunction(string functionName, List<KeyValuePair<string, object>> parameters);

        /// <summary>
        /// Detaches an item from the underlying data context.
        /// </summary>
        /// <param name="item">The tracked item to detach.</param>
        /// <remarks>The item is no longer tracked by this strategy after a successful detach.</remarks>
        void Detach(object item);

        /// <summary>
        /// Configures change detection for the underlying data context.
        /// </summary>
        /// <param name="detect">The requested change-detection setting.</param>
        /// <remarks>
        /// The current Entity Framework implementation disables automatic change detection
        /// regardless of the value supplied for <paramref name="detect" />. This observed quirk
        /// is retained for compatibility and is documented without changing behavior.
        /// </remarks>
        void AutoDetectChanges(bool detect);
    }
}
