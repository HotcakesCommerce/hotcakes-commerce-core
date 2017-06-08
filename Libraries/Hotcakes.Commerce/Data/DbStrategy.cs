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
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Data
{
    public class DbStrategy<TDefaultEntity> : IRepoStrategy<TDefaultEntity> where TDefaultEntity : class
    {
        private readonly DbContext _db;

        private readonly Dictionary<Type, object> _dbSets = new Dictionary<Type, object>();

        public DbStrategy(DbContext db)
        {
            _db = db;
        }

        public ILogger Logger { get; set; }

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

        public IQueryable<TDefaultEntity> GetQuery(Expression<Func<TDefaultEntity, bool>> predicate = null)
        {
            return GetQuery<TDefaultEntity>(predicate);
        }

        public IQueryable<TEntity> GetQuery<TEntity>(Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class
        {
            IQueryable<TEntity> q = GetDbSet<TEntity>();

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
            GetDbSet<TEntity>().Add(item);
        }

        public void Delete(TDefaultEntity item)
        {
            DeleteEntity(item);
        }

        public void DeleteEntity<TEntity>(TEntity item) where TEntity : class
        {
            GetDbSet<TEntity>().Remove(item);
        }

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

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Detach(object item)
        {
            ((IObjectContextAdapter) _db).ObjectContext.Detach(item);
        }

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