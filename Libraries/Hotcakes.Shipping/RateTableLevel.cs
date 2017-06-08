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

namespace Hotcakes.Shipping
{
    [Serializable]
    public class RateTableLevel : IComparable<RateTableLevel>, IEquatable<RateTableLevel>
    {
        public RateTableLevel()
        {
            Level = 0;
            Rate = 0;
            Percent = 0;
        }

        public decimal Level { get; set; }
        public decimal Rate { get; set; }
        public decimal Percent { get; set; }

        public int CompareTo(RateTableLevel other)
        {
            return Level.CompareTo(other.Level);
        }

        #region IEquatable<RateTableLevel> Members

        public bool Equals(RateTableLevel other)
        {
            if ((other.Level == Level)
                && (other.Rate == Rate)
                && (other.Percent == Percent)) return true;

            return false;
        }

        #endregion

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