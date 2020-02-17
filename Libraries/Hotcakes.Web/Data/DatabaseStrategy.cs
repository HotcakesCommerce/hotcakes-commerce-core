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
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web.Data
{
    [Serializable]
    public class DatabaseStrategy<T> : IRepositoryStrategy<T> where T : class, new()
    {
        private bool _AutoSubmit = true;
        private readonly DbContext _dbContext;
        private readonly ObjectContext _objectContext;
        private ObjectSet<T> _objectSet;

        public DatabaseStrategy(DbContext context)
        {
            Logger = new NullLogger();

            _dbContext = context;
            _objectContext = (context as IObjectContextAdapter).ObjectContext;

            // Call this to force init on _objectset
            var obj = objectSet;
        }

        private IObjectSet<T> objectSet
        {
            get
            {
                if (_objectSet == null)
                {
                    _objectSet = _objectContext
                        .CreateObjectSet<T>();
                }
                return _objectSet;
            }
        }

        public bool AutoSubmit
        {
            get { return _AutoSubmit; }
            set { _AutoSubmit = value; }
        }

        public ILogger Logger { get; set; }

        public bool SubmitChanges()
        {
            if (_objectContext != null)
            {
                try
                {
                    _dbContext.SaveChanges();
                    return true;
                }
                catch (OptimisticConcurrencyException cex)
                {
                    Logger.LogMessage("Hotcakes.Web.Data",
                        "Concurrency exception while saving changes in EntityFrameworkRespository" + cex.Message + " " +
                        cex.StackTrace, EventLogSeverity.Debug);
                }
            }
            return false;
        }

        public T FindByPrimaryKey(PrimaryKey id)
        {
            var setName = _objectSet.Context.DefaultContainerName + "." + _objectSet.EntitySet.Name;

            var key = new EntityKey(setName, id.KeyName, id.KeyAsObject());

            try
            {
                var found = _objectContext.GetObjectByKey(key);
                if (found != null)
                {
                    return (T) found;
                }
            }
            catch (ObjectNotFoundException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Hotcakes.Web.Data", "Find by Primary Key Failure: " +
                                                       "KEY=" + id.KeyAsObject()
                                                       + ", EntitySetName=" + _objectSet.EntitySet.Name
                                                       + ", EXCEPTION=" + ex + " " + ex.StackTrace,
                    EventLogSeverity.Debug);
            }
            return null;
        }

        public IQueryable<T> Find()
        {
            try
            {
                return objectSet;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, EventLogSeverity.Debug);
            }
            return null;
        }

        public bool Create(T item)
        {
            try
            {
                objectSet.AddObject(item);
                if (AutoSubmit) SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, EventLogSeverity.Debug);
            }
            return false;
        }

        public bool Delete(PrimaryKey id)
        {
            var found = FindByPrimaryKey(id);
            if (found == null) return false;

            objectSet.DeleteObject(found);
            if (_AutoSubmit) SubmitChanges();
            return true;
        }

        public void Detach(T item)
        {
            objectSet.Detach(item);
        }

        public int CountOfAll()
        {
            try
            {
                return objectSet.Count();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, EventLogSeverity.Debug);
            }
            return -1;
        }
    }
}