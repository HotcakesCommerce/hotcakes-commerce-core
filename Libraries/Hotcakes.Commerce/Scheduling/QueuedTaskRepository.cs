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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Scheduling
{
    public class QueuedTaskRepository : HccSimpleRepoBase<hcc_QueuedTask, QueuedTask>
    {
        public QueuedTaskRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_QueuedTask data, QueuedTask model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.FriendlyName = data.FriendlyName;
            model.Payload = data.Payload;
            model.StartAtUtc = data.StartAtUtc;
            model.Status = (QueuedTaskStatus) data.Status;
            model.StatusNotes = data.StatusNotes;
            model.TaskProcessorId = data.TaskProcessorId;
            model.TaskProcessorName = data.TaskProcessorName;
        }

        protected override void CopyModelToData(hcc_QueuedTask data, QueuedTask model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.FriendlyName = model.FriendlyName;
            data.Payload = model.Payload;
            data.StartAtUtc = model.StartAtUtc;
            data.Status = (int) model.Status;
            data.StatusNotes = model.StatusNotes;
            data.TaskProcessorId = model.TaskProcessorId;
            data.TaskProcessorName = model.TaskProcessorName;
        }

        public QueuedTask Find(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        public QueuedTask FindNextQueuedByProcessorId(Guid processorId)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                var item = s.GetQuery().Where(y => y.StoreId == storeId)
                    .Where(y => y.Status == (int) QueuedTaskStatus.Pending)
                    .Where(y => y.TaskProcessorId == processorId)
                    .OrderBy(y => y.StartAtUtc).FirstOrDefault();

                var result = FirstPoco(item);
                if (result == null) return null;
                return result;
            }
        }

        public QueuedTask FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public override bool Create(QueuedTask item)
        {
            if (item.StoreId < 1)
            {
                item.StoreId = Context.CurrentStore.Id;
            }
            return base.Create(item);
        }

        public bool Update(QueuedTask c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<QueuedTask> FindAll()
        {
            return FindAllPaged(1, 100);
        }

        public new List<QueuedTask> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderByDescending(y => y.Id); },
                pageNumber, pageSize);
        }

        // Finds all stores that have a pending task ready to run at this time.
        public List<long> ListStoresWithTasksToRun()
        {
            using (var s = CreateStrategy())
            {
                var storeIds = s.GetQuery().Where(y => y.Status == (int) QueuedTaskStatus.Pending)
                    .Where(y => y.StartAtUtc <= DateTime.UtcNow)
                    .Select(y => y.StoreId).Distinct().ToList();

                if (storeIds == null) return new List<long>();
                return storeIds;
            }
        }

        public QueuedTask PopATaskForRun(long storeId)
        {
            using (var s = CreateStrategy())
            {
                var item = s.GetQuery().Where(y => y.StoreId == storeId)
                    .Where(y => y.Status == (int) QueuedTaskStatus.Pending)
                    .OrderBy(y => y.Id).ToList().FirstOrDefault();

                var result = FirstPoco(item);
                if (result == null) return null;

                result.Status = QueuedTaskStatus.Running;
                result.StatusNotes = "Started Running at " + DateTime.UtcNow;
                Update(result, y => y.Id == result.Id && y.StoreId == storeId);

                return result;
            }
        }

        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static QueuedTaskRepository InstantiateForMemory(HccRequestContext c)
        {
            return new QueuedTaskRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static QueuedTaskRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new QueuedTaskRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        private QueuedTaskRepository(HccRequestContext c, IRepositoryStrategy<hcc_QueuedTask> r,
            ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}