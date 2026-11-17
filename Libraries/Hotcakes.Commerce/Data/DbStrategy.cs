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
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// Provides Entity Framework-backed repository operations for a default entity type.
    /// </summary>
    /// <typeparam name="TDefaultEntity">The entity type used by the non-generic convenience members.</typeparam>
    /// <remarks>
    /// The strategy wraps the supplied <see cref="DbContext"/>. Entity sets are resolved lazily and cached by
    /// entity type for the lifetime of the strategy. Queries remain deferred until the returned
    /// <see cref="IQueryable{T}"/> is enumerated or otherwise executed by the caller.
    /// </remarks>
    public class DbStrategy<TDefaultEntity> : IRepoStrategy<TDefaultEntity> where TDefaultEntity : class
    {
        private readonly DbContext _db;

        private readonly Dictionary<Type, object> _dbSets = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a strategy over an existing Entity Framework context.
        /// </summary>
        /// <param name="db">The context used for queries, tracking, persistence, and function execution.</param>
        public DbStrategy(DbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets or sets the logger used for concurrency diagnostics during persistence.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Persists changes currently tracked by the underlying context.
        /// </summary>
        /// <returns>
        /// Always returns <see langword="true"/>. A <see cref="DbUpdateConcurrencyException"/> is logged at
        /// debug severity and is not rethrown by this implementation.
        /// </returns>
        /// <remarks>
        /// The underlying context performs its normal change detection and transaction behavior when
        /// <see cref="DbContext.SaveChanges()"/> is called.
        /// </remarks>
        public bool SubmitChanges()
        {
            try
            {
                // Use optimistic concurrency when saving changes.
                var result = _db.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException cex)
            {
                Logger.LogMessage("Hotcakes.Commerce.Data",
                    "Concurrency exception while saving changes in EntityFrameworkRespository" + cex.Message + " " +
                    cex.StackTrace, EventLogSeverity.Debug);
            }

            // TODO: remove this try/catch
            return true;
        }

        /// <summary>
        /// Gets a deferred query for the default entity type.
        /// </summary>
        /// <param name="predicate">An optional predicate applied to the query before it is returned.</param>
        /// <returns>A deferred <see cref="IQueryable{T}"/> over the default entity set.</returns>
        public IQueryable<TDefaultEntity> GetQuery(Expression<Func<TDefaultEntity, bool>> predicate = null)
        {
            return GetQuery<TDefaultEntity>(predicate);
        }

        /// <summary>
        /// Gets a deferred query for an entity type supported by the underlying context.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to query.</typeparam>
        /// <param name="predicate">An optional predicate applied to the query before it is returned.</param>
        /// <returns>A deferred <see cref="IQueryable{T}"/> over the requested entity set.</returns>
        /// <remarks>
        /// The entity set is cached by type after its first resolution. This method does not enumerate the
        /// query or execute database work by itself.
        /// </remarks>
        public IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            IQueryable<TEntity> q = GetDbSet<TEntity>();

            if (predicate != null)
                q = q.Where(predicate);

            return q;
        }

        /// <summary>
        /// Marks a default entity for insertion in the underlying context.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        public void Add(TDefaultEntity item)
        {
            AddEntity(item);
        }

        /// <summary>
        /// Marks an entity for insertion in the underlying context.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to add.</typeparam>
        /// <param name="item">The entity to add.</param>
        public void AddEntity<TEntity>(TEntity item) where TEntity : class
        {
            GetDbSet<TEntity>().Add(item);
        }

        /// <summary>
        /// Marks a default entity for deletion in the underlying context.
        /// </summary>
        /// <param name="item">The entity to remove.</param>
        public void Delete(TDefaultEntity item)
        {
            DeleteEntity(item);
        }

        /// <summary>
        /// Marks an entity for deletion in the underlying context.
        /// </summary>
        /// <typeparam name="TEntity">The entity type to remove.</typeparam>
        /// <param name="item">The entity to remove.</param>
        public void DeleteEntity<TEntity>(TEntity item) where TEntity : class
        {
            GetDbSet<TEntity>().Remove(item);
        }

        /// <summary>
        /// Executes a mapped Entity Framework function and materializes its results.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the function.</typeparam>
        /// <param name="functionName">The mapped function name passed to the object context.</param>
        /// <param name="parameters">An object whose readable properties provide function parameter names and values.</param>
        /// <returns>A materialized list containing the function results.</returns>
        /// <remarks>
        /// Parameter descriptors are created from the properties of <paramref name="parameters"/>. Null values
        /// are represented with the corresponding property type. The function is executed immediately.
        /// </remarks>
        public IList<TResult> ExecFunction<TResult>(string functionName, object parameters)
        {
            var oParams = new List<ObjectParameter>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(parameters))
            {
                var val = property.GetValue(parameters);

                if (val == null)
                {
                    oParams.Add(new ObjectParameter(property.Name, property.PropertyType));
                }
                else
                {
                    oParams.Add(new ObjectParameter(property.Name, val));
                }
            }

            return
                ((IObjectContextAdapter) _db).ObjectContext.ExecuteFunction<TResult>(functionName, oParams.ToArray())
                    .ToList();
        }

        /// <summary>
        /// Executes a function intended to produce a scalar or command result without returning rows.
        /// </summary>
        /// <param name="functionName">The database function or procedure name to execute.</param>
        /// <param name="parameters">The named parameter values appended to the command text.</param>
        /// <remarks>
        /// The command is executed immediately through the underlying object context. This method preserves
        /// the existing command construction and does not return the command result.
        /// </remarks>
        public void ExecScalarFunction(string functionName, List<KeyValuePair<string, object>> parameters)
        {
            var oParams = new List<ObjectParameter>();
            var parameterList = "exec " + functionName + "   ";

            foreach (var parameter in parameters)
            {
                parameterList += "@" + parameter.Key + "= " + parameter.Value + "    ";
                //oParams.Add(new ObjectParameter(parameter.Key, parameter.Value));
            }


            ((IObjectContextAdapter) _db).ObjectContext.ExecuteStoreCommand(parameterList);

            // ((IObjectContextAdapter)_db).ObjectContext.ExecuteFunction(functionName, oParams.ToArray());
        }

        /// <summary>
        /// Disposes the underlying context.
        /// </summary>
        /// <remarks>
        /// Because the strategy wraps the supplied context, disposing the strategy also disposes that context.
        /// </remarks>
        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// Detaches an object from the underlying object context.
        /// </summary>
        /// <param name="item">The object to detach from change tracking.</param>
        public void Detach(object item)
        {
            ((IObjectContextAdapter) _db).ObjectContext.Detach(item);
        }

        /// <summary>
        /// Configures automatic change detection for the underlying context.
        /// </summary>
        /// <param name="detect">The requested change-detection setting.</param>
        /// <remarks>
        /// The current implementation always disables automatic change detection and does not use the value
        /// of <paramref name="detect"/>. Callers should account for this compatibility-sensitive behavior.
        /// </remarks>
        public void AutoDetectChanges(bool detect)
        {
            _db.Configuration.AutoDetectChangesEnabled = false;
        }

        private DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            object oSet;

            if (!_dbSets.TryGetValue(typeof (TEntity), out oSet))
            {
                oSet = _db.Set<TEntity>();
                _dbSets.Add(typeof (TEntity), oSet);
            }

            return (DbSet<TEntity>) oSet;
        }
    }
}
