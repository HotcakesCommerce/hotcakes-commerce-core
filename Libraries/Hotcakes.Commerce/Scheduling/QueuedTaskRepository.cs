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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Scheduling
{
    /// <summary>
    /// Provides persistence operations for store-scoped <see cref="QueuedTask"/> records.
    /// </summary>
    /// <remarks>
    /// Most queries use <see cref="HccRequestContext.CurrentStore"/> as their store boundary. The explicit all-store lookup and
    /// store-listing operation are exceptions documented on their respective members.
    /// </remarks>
    public class QueuedTaskRepository : HccSimpleRepoBase<hcc_QueuedTask, QueuedTask>
    {
        /// <summary>Initializes a repository using the supplied commerce request context.</summary>
        /// <param name="c">The request context used for store-scoped persistence.</param>
        public QueuedTaskRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Copies persisted queue data into its model representation.</summary>
        /// <param name="data">The persisted queue record.</param>
        /// <param name="model">The model receiving the persisted values.</param>
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

        /// <summary>Copies a queue model into its persisted data representation.</summary>
        /// <param name="data">The persisted queue record receiving the values.</param>
        /// <param name="model">The model supplying the values.</param>
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

        /// <summary>Finds a task by identifier in the current store.</summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>The matching task, or <see langword="null"/> when it is not in the current store.</returns>
        public QueuedTask Find(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>Finds the earliest pending task for a processor in the current store.</summary>
        /// <param name="processorId">The processor identifier to match.</param>
        /// <returns>The pending task with the earliest <see cref="QueuedTask.StartAtUtc"/>, or <see langword="null"/> when none matches.</returns>
        /// <remarks>This method does not additionally require the task's start time to have arrived.</remarks>
        public QueuedTask FindNextQueuedByProcessorId(Guid processorId)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var item = s.GetQuery().AsNoTracking().Where(y => y.StoreId == storeId)
                    .Where(y => y.Status == (int) QueuedTaskStatus.Pending)
                    .Where(y => y.TaskProcessorId == processorId)
                    .OrderBy(y => y.StartAtUtc).FirstOrDefault();

                var result = FirstPoco(item);
                if (result == null) return null;
                return result;
            }
        }

        /// <summary>Finds a task by identifier without restricting the query to the current store.</summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>The matching task, or <see langword="null"/> when no task has that identifier.</returns>
        public QueuedTask FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Creates a queued task, assigning the current store when the model has no store identifier.</summary>
        /// <param name="item">The task to persist.</param>
        /// <returns><see langword="true"/> when the base repository creates the record.</returns>
        /// <remarks>A positive <see cref="QueuedTask.StoreId"/> is preserved rather than replaced with the current store.</remarks>
        public override bool Create(QueuedTask item)
        {
            if (item.StoreId < 1)
            {
                item.StoreId = Context.CurrentStore.Id;
            }
            return base.Create(item);
        }

        /// <summary>Updates a task only when its store is the current store.</summary>
        /// <param name="c">The task values to persist.</param>
        /// <returns><see langword="false"/> when the task store differs from the current store; otherwise the base update result.</returns>
        public bool Update(QueuedTask c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>Deletes a task by identifier.</summary>
        /// <param name="id">The task identifier.</param>
        /// <returns>The result returned by the base repository delete operation.</returns>
        /// <remarks>This method does not add a current-store predicate; callers should use an identifier within the intended store scope.</remarks>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>Returns the first page of tasks for the current store.</summary>
        /// <returns>Up to 100 tasks, ordered by descending identifier.</returns>
        public List<QueuedTask> FindAll()
        {
            return FindAllPaged(1, 100);
        }

        /// <summary>Returns a page of tasks for the current store.</summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of tasks in the page.</param>
        /// <returns>Tasks ordered by descending identifier and restricted to the current store.</returns>
        public new List<QueuedTask> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderByDescending(y => y.Id); },
                pageNumber, pageSize);
        }

        /// <summary>Lists stores that have a pending task ready to run at the current UTC time.</summary>
        /// <returns>Distinct store identifiers for pending tasks whose start time is not later than now.</returns>
        /// <remarks>This query intentionally spans all stores rather than using the current store context.</remarks>
        public List<long> ListStoresWithTasksToRun()
        {
            using (var s = CreateReadStrategy())
            {
                var storeIds = s.GetQuery().AsNoTracking().Where(y => y.Status == (int) QueuedTaskStatus.Pending)
                    .Where(y => y.StartAtUtc <= DateTime.UtcNow)
                    .Select(y => y.StoreId).Distinct().ToList();

                if (storeIds == null) return new List<long>();
                return storeIds;
            }
        }

        /// <summary>Claims the oldest pending task for a store for processing.</summary>
        /// <param name="storeId">The store whose queue should be searched.</param>
        /// <returns>The claimed task, or <see langword="null"/> when that store has no pending task.</returns>
        /// <remarks>The task is selected by ascending identifier, not by start time. A successful claim changes its status to
        /// <see cref="QueuedTaskStatus.Running"/> and records a UTC start message in <see cref="QueuedTask.StatusNotes"/>.</remarks>
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
    }
}
