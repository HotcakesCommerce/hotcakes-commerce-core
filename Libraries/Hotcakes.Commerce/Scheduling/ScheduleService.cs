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
using System.Linq;

namespace Hotcakes.Commerce.Scheduling
{
    /// <summary>
    /// Provides scheduling operations backed by a <see cref="QueuedTaskRepository"/>.
    /// </summary>
    public class ScheduleService : HccServiceBase
    {
        /// <summary>Initializes the service and its queued-task repository.</summary>
        /// <param name="context">The commerce request context for the service.</param>
        public ScheduleService(HccRequestContext context)
            : base(context)
        {
            QueuedTasks = Factory.CreateRepo<QueuedTaskRepository>(Context);
        }

        /// <summary>Gets or sets the repository used for queued-task operations.</summary>
        /// <remarks>The setter is protected so derived services can replace the repository implementation.</remarks>
        public QueuedTaskRepository QueuedTasks { get; protected set; }

        /// <summary>Removes pending tasks for a processor.</summary>
        /// <param name="storeId">The store identifier supplied by the caller.</param>
        /// <param name="processorId">The processor identifier whose pending tasks should be removed.</param>
        /// <remarks>
        /// The current implementation reads the first page of tasks through <see cref="QueuedTaskRepository.FindAllPaged(int, int)"/>
        /// and deletes matching pending tasks. The <paramref name="storeId"/> argument is not used to filter that query; the repository
        /// applies the store associated with its current request context. Non-pending tasks are retained.
        /// </remarks>
        public void RemoveAllTasksForProcessor(long storeId, Guid processorId)
        {
            var tasks = QueuedTasks.FindAllPaged(1, 1000);
            if (tasks == null) return;
            if (tasks.Count < 1) return;
            var toDelete =
                tasks.Where(y => y.TaskProcessorId == processorId).Where(y => y.Status == QueuedTaskStatus.Pending);
            foreach (var t in toDelete)
            {
                QueuedTasks.Delete(t.Id);
            }
        }
    }
}
