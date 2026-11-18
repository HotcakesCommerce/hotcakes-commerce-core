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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Represents the catalog class <see cref="GiftCardSearchCriteria" /> and its associated behavior.
    /// </summary>
    public class GiftCardSearchCriteria
    {
        /// <summary>
        /// Text used to search gift card records.
        /// </summary>
        /// <value>The search text passed to the gift-card search procedure.</value>
        public string SearchText { get; set; }
        /// <summary>
        /// Inclusive lower date boundary, in UTC, used by gift card searches.
        /// </summary>
        /// <value>The beginning of the issue-date range.</value>
        public DateTime StartDateUtc { get; set; }
        /// <summary>
        /// Inclusive upper date boundary, in UTC, used by gift card searches.
        /// </summary>
        /// <value>The end of the issue-date range.</value>
        public DateTime EndDateUtc { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ShowDisabled" /> catalog value.
        /// </summary>
        /// <value>The current value of <see cref="ShowDisabled" />.</value>
        public bool ShowDisabled { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="ShowExpired" /> catalog value.
        /// </summary>
        /// <value>The current value of <see cref="ShowExpired" />.</value>
        public bool ShowExpired { get; set; }
        /// <summary>
        /// Amount comparison applied by the repository search procedure.
        /// </summary>
        /// <value>The operator and amount used to filter the original card amount.</value>
        public CompareCriteria<decimal> CompareAmount { get; set; }
        /// <summary>
        /// Remaining-balance comparison applied by the repository search procedure.
        /// </summary>
        /// <value>The operator and balance used to filter the remaining amount.</value>
        public CompareCriteria<decimal> CompareBalance { get; set; }
    }

    /// <summary>
    /// Describes a comparison value and operator used by a catalog search.
    /// </summary>
    /// <typeparam name="T">Comparable value type used by the comparison.</typeparam>
    public class CompareCriteria<T> where T : IComparable
    {
        /// <summary>
        /// Represents the catalog enum <see cref="CompareOperator" /> and its associated behavior.
        /// </summary>
        public enum CompareOperator
        {
    /// <summary>
    /// Serialized catalog value <c>Any = 0</c>.
    /// </summary>
            Any = 0,
    /// <summary>
    /// Serialized catalog value <c>GreaterThan = 1</c>.
    /// </summary>
            GreaterThan = 1,
    /// <summary>
    /// Serialized catalog value <c>LessThan = 2</c>.
    /// </summary>
            LessThan = 2,
    /// <summary>
    /// Serialized catalog value <c>Equal = 3</c>.
    /// </summary>
            Equal = 3
        }

        /// <summary>
        /// Gets or sets the <see cref="Operator" /> catalog value.
        /// </summary>
        /// <value>The current value of <see cref="Operator" />.</value>
        public CompareOperator Operator { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Value" /> catalog value.
        /// </summary>
        /// <value>The current value of <see cref="Value" />.</value>
        public T Value { get; set; }
    }
}
