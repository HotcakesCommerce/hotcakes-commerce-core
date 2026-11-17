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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// Represents the outcome of a repository operation that can affect multiple values.
    /// </summary>
    /// <typeparam name="V">The POCO type represented by the operation.</typeparam>
    /// <remarks>
    /// A new instance starts with <see cref="Success" /> set to <c>false</c> and an empty
    /// <see cref="OldValues" /> collection. Repository implementations populate the result
    /// as the operation progresses; an empty collection therefore does not by itself indicate
    /// whether the operation was successful.
    /// </remarks>
    public class DalBatchOperationResult<V>
        where V : class, new()
    {
        /// <summary>
        /// Initializes a new batch-operation result with an empty collection of prior values.
        /// </summary>
        public DalBatchOperationResult()
        {
            OldValues = new List<V>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the repository operation reported success.
        /// </summary>
        /// <value><c>true</c> when the operation reports success; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The default value is <c>false</c>. The concrete repository strategy is responsible
        /// for setting this property after attempting to persist changes.
        /// </remarks>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the values as they existed before the operation changed or deleted them.
        /// </summary>
        /// <value>A mutable list of prior values, initially empty.</value>
        /// <remarks>
        /// Implementations use this collection for batch updates and deletes. Its contents and
        /// ordering are determined by the repository operation; it is not automatically populated
        /// by this result type.
        /// </remarks>
        public List<V> OldValues { get; set; }
    }
}
