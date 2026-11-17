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

namespace Hotcakes.Commerce.Data
{
    /// <summary>
    /// Represents the outcome of a repository operation against one value.
    /// </summary>
    /// <typeparam name="V">The POCO type represented by the operation.</typeparam>
    /// <remarks>
    /// A new instance starts with <see cref="Success" /> set to <c>false</c> and
    /// <see cref="OldValue" /> initialized with <c>new V()</c>. Repository implementations
    /// copy the prior persisted value into <see cref="OldValue" /> when one is found; the
    /// initialized value can therefore also represent an operation that found no match.
    /// </remarks>
    public class DalSingleOperationResult<V>
        where V : class, new()
    {
        /// <summary>
        /// Initializes a new single-operation result with a default value instance.
        /// </summary>
        public DalSingleOperationResult()
        {
            OldValue = new V();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the repository operation reported success.
        /// </summary>
        /// <value><c>true</c> when the operation reports success; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// The default value is <c>false</c>. A repository may leave this value unchanged when
        /// no matching record is found or when the operation returns before persistence.
        /// </remarks>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the value as it existed before the operation changed it.
        /// </summary>
        /// <value>The prior value, initialized to a new <typeparamref name="V" /> instance.</value>
        /// <remarks>
        /// The result type always initializes this property by invoking the generic type's
        /// parameterless constructor. Repository implementations may replace its contents with
        /// the prior persisted value; it is not <c>null</c> solely because no match was found.
        /// </remarks>
        public V OldValue { get; set; }
    }
}
