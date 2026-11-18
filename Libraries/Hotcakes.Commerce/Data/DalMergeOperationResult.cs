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
    /// Represents the outcome of a repository merge operation.
    /// </summary>
    /// <typeparam name="V">The POCO type represented by the merge operation.</typeparam>
    /// <remarks>
    /// A merge may create, update, or delete records, but this result currently exposes only
    /// the prior values captured by the repository. The commented members below are retained
    /// as compatibility-sensitive TODOs and are not part of the current public API.
    /// A new instance starts with <see cref="Success" /> set to <c>false</c> and an empty
    /// <see cref="OldValues" /> collection.
    /// </remarks>
    public class DalMergeOperationResult<V>
        where V : class, new()
    {
        // TODO: Implement commented out functionality
        //public List<V> Created { get; set; }
        //public List<V> Updated { get; set; }
        //public List<V> Deleted { get; set; }

        /// <summary>
        /// Initializes a new merge-operation result with an empty collection of prior values.
        /// </summary>
        public DalMergeOperationResult()
        {
            OldValues = new List<V>();
            //Created = new List<V>();
            //Updated = new List<V>();
            //Deleted = new List<V>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the repository merge reported success.
        /// </summary>
        /// <value><c>true</c> when the merge reports success; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The default value is <c>false</c>. The repository sets this property after attempting
        /// to persist the merge, including any records created, updated, or deleted by that merge.
        /// </remarks>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the values that matched the repository query before the merge was applied.
        /// </summary>
        /// <value>A mutable list of prior values, initially empty.</value>
        /// <remarks>
        /// The current merge implementation captures existing values before applying changes.
        /// Newly created values are not added to this collection, and the collection does not
        /// identify which prior values were updated or deleted.
        /// </remarks>
        public List<V> OldValues { get; set; }
    }
}
