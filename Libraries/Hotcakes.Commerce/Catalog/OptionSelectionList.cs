#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Linq;

namespace Hotcakes.Commerce.Catalog
{
    public class OptionSelectionList : List<OptionSelection>
    {
        public bool ContainsSelectionForOption(string optionBvin)
        {
            var cleaned = OptionSelection.CleanBvin(optionBvin);
            return this.Any(os => os.OptionBvin == cleaned);
        }

        public OptionSelection FindByOptionId(string optionId)
        {
            var cleaned = OptionSelection.CleanBvin(optionId);
            return this.FirstOrDefault(os => os.OptionBvin == cleaned);
        }

        public decimal GetPriceAdjustmentForSelections(OptionList allOptions)
        {
            decimal result = 0;

            foreach (var selection in this)
            {
                var subSelections = selection.SelectionData.Split(',');

                result += allOptions
                    .Where(o => o.Items != null)
                    .Sum(o => o.Items
                        .Where(oi => subSelections.Contains(OptionSelection.CleanBvin(oi.Bvin)))
                        .Sum(oi => oi.PriceAdjustment)
                    );
            }

            return result;
        }

        public decimal GetWeightAdjustmentForSelections(OptionList allOptions)
        {
            decimal result = 0;

            foreach (var selection in this)
            {
                var subSelections = selection.SelectionData.Split(',');

                result += allOptions
                    .Where(o => o.Items != null)
                    .Sum(o => o.Items
                        .Where(oi => subSelections.Contains(OptionSelection.CleanBvin(oi.Bvin)))
                        .Sum(oi => oi.WeightAdjustment)
                    );
            }

            return result;
        }

        /// <summary>
        ///     Return true if both objects are equal. Otherwise false.
        /// </summary>
        /// <param name="other">The other option selections</param>
        /// <returns></returns>
        public bool Equals(OptionSelectionList other)
        {
            foreach (var item in this)
            {
                var otherItem =
                    other.FirstOrDefault(i => i.OptionBvin == item.OptionBvin && i.SelectionData == item.SelectionData);
                if (otherItem == null)
                    return false;
            }
            return true;
        }
    }
}