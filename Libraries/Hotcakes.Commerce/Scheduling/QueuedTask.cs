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

namespace Hotcakes.Commerce.Scheduling
{
    /// <summary>
    /// Represents a task persisted in the store-scoped scheduling queue.
    /// </summary>
    /// <remarks>
    /// New instances start in <see cref="QueuedTaskStatus.Pending"/> and are scheduled for the current UTC time.
    /// The repository assigns the current store when a created task has no store identifier.
    /// </remarks>
    public class QueuedTask
    {
        /// <summary>
        /// Initializes a task with its persisted defaults.
        /// </summary>
        /// <remarks>
        /// The identifier and store identifier are zero, text values are empty, the processor identifier is an empty <see cref="Guid"/>,
        /// <see cref="Status"/> is <see cref="QueuedTaskStatus.Pending"/>, and <see cref="StartAtUtc"/> is set to <see cref="DateTime.UtcNow"/>.
        /// </remarks>
        public QueuedTask()
        {
            Id = 0;
            StoreId = 0;
            FriendlyName = string.Empty;
            TaskProcessorName = string.Empty;
            TaskProcessorId = new Guid();
            Payload = string.Empty;
            Status = QueuedTaskStatus.Pending;
            StatusNotes = string.Empty;
            StartAtUtc = DateTime.UtcNow;
        }

        /// <summary>Gets or sets the persisted task identifier.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the store that owns the task.</summary>
        /// <remarks>Repository queries normally restrict tasks to the current store.</remarks>
        public long StoreId { get; set; }

        /// <summary>Gets or sets the display name used to identify the task.</summary>
        public string FriendlyName { get; set; }

        /// <summary>Gets or sets the name of the component that processes the task.</summary>
        public string TaskProcessorName { get; set; }

        /// <summary>Gets or sets the identifier of the component that processes the task.</summary>
        public Guid TaskProcessorId { get; set; }

        /// <summary>Gets or sets the processor-specific payload persisted with the task.</summary>
        public string Payload { get; set; }

        /// <summary>Gets or sets the current lifecycle state of the task.</summary>
        /// <remarks>Tasks are normally claimed by <see cref="QueuedTaskRepository.PopATaskForRun(long)"/>, which changes them from pending to running.</remarks>
        public QueuedTaskStatus Status { get; set; }

        /// <summary>Gets or sets notes describing the task state or processing result.</summary>
        /// <remarks>Claiming a task records a UTC start message here; other status transitions are performed by callers.</remarks>
        public string StatusNotes { get; set; }

        /// <summary>Gets or sets the UTC time at which the task becomes eligible to run.</summary>
        /// <remarks>Tasks are considered ready by <see cref="QueuedTaskRepository.ListStoresWithTasksToRun"/> when this value is not later than the current UTC time.</remarks>
        public DateTime StartAtUtc { get; set; }
    }
}
