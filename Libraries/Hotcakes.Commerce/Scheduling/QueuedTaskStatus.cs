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

namespace Hotcakes.Commerce.Scheduling
{
    /// <summary>
    /// Identifies the lifecycle state of a <see cref="QueuedTask"/>.
    /// </summary>
    /// <remarks>
    /// A newly constructed task defaults to <see cref="Pending"/>. The repository's run-claim operation changes a pending task to
    /// <see cref="Running"/>; processing code is responsible for later recording <see cref="Completed"/> or <see cref="Failed"/>.
    /// The numeric values are persisted and therefore compatibility-sensitive.
    /// </remarks>
    public enum QueuedTaskStatus
    {
        /// <summary>Indicates that no recognized status has been assigned.</summary>
        Unknown = 0,

        /// <summary>Indicates that the task is waiting to be claimed for processing.</summary>
        Pending = 1,

        /// <summary>Indicates that the task has been claimed and processing has started.</summary>
        Running = 2,

        /// <summary>Indicates that processing completed successfully.</summary>
        Completed = 3,

        /// <summary>Indicates that processing completed unsuccessfully.</summary>
        Failed = 4
    }
}
