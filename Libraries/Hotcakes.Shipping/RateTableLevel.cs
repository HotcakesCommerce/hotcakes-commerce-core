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
using System.Linq;

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Represents one threshold and its associated shipping charge or percentage.
    /// </summary>
    /// <seealso cref="IComparable{RateTableLevel}" />
    /// <seealso cref="IEquatable{RateTableLevel}" />
    [Serializable]
    public class RateTableLevel : IComparable<RateTableLevel>, IEquatable<RateTableLevel>
    {
        /// <summary>
        /// Initializes a rate-table level with zero values.
        /// </summary>
        public RateTableLevel()
        {
            Level = 0;
            Rate = 0;
            Percent = 0;
        }

        /// <summary>
        /// Gets or sets the threshold at which this level applies.
        /// </summary>
        public decimal Level { get; set; }

        /// <summary>
        /// Gets or sets the fixed rate associated with this level.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the percentage associated with this level.
        /// </summary>
        public decimal Percent { get; set; }

        /// <summary>
        /// Compares this level with another level by <see cref="Level" />.
        /// </summary>
        /// <param name="other">The level to compare with this instance.</param>
        /// <returns>A value indicating the relative order of the two levels.</returns>
        public int CompareTo(RateTableLevel other)
        {
            return Level.CompareTo(other.Level);
        }

        #region IEquatable<RateTableLevel> Members

        /// <summary>
        /// Determines whether another level has the same threshold, rate, and percentage.
        /// </summary>
        /// <param name="other">The level to compare with this instance.</param>
        /// <returns><see langword="true" /> when all rate-table values match; otherwise, <see langword="false" />.</returns>
        public bool Equals(RateTableLevel other)
        {
            if ((other.Level == Level)
                && (other.Rate == Rate)
                && (other.Percent == Percent)) return true;

            return false;
        }

        #endregion

        /// <summary>
        /// Finds the applicable rate-table level for a threshold value.
        /// </summary>
        /// <param name="levelValue">The value for which an applicable level is sought.</param>
        /// <param name="levels">The levels to inspect. The list is sorted as part of the lookup.</param>
        /// <returns>The highest qualifying level, or a level with <see cref="Level" /> set to -1 when no level qualifies.</returns>
        public static RateTableLevel FindRateForLevel(decimal levelValue, List<RateTableLevel> levels)
        {
            var result = new RateTableLevel
            {
                Percent = 0,
                Rate = 0,
                Level = -1
            };

            if (levels != null)
            {
                if (levels.Count > 0)
                {
                    // Make sure we're sorted
                    levels.Sort();

                    // Assign the last level in case we're above it
                    // but only if we qualify for it
                    if (levelValue >= levels[levels.Count - 1].Level)
                    {
                        result = levels[levels.Count - 1];
                    }

                    var PreviousLevel = decimal.MaxValue;

                    // Walk levels backwards
                    for (var i = levels.Count() - 1; i >= 0; i--)
                    {
                        if ((levelValue >= levels[i].Level) && (levelValue < PreviousLevel))
                        {
                            result = levels[i];
                        }
                        PreviousLevel = levels[i].Level;
                    }
                }
            }

            return result;
        }
    }
}
