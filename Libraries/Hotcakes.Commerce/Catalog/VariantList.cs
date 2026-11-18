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
using System.Linq;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>Collection helpers for locating variants by selection keys or identifiers.</summary>
    public class VariantList : List<Variant>
    {
        public bool ContainsKey(string uniqueKey)
        {
            return this.Any(v => v.UniqueKey() == uniqueKey);
        }

        /// <summary>Finds the first variant with the specified selection key, or null.</summary>
        /// <param name="uniqueKey">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public Variant FindByKey(string uniqueKey)
        {
            return this.FirstOrDefault(v => v.UniqueKey() == uniqueKey);
        }

        /// <summary>Finds the first variant with the specified Bvin, or null.</summary>
        /// <param name="bvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public Variant FindByBvin(string bvin)
        {
            return this.FirstOrDefault(v => v.Bvin == bvin);
        }

        /// <summary>Finds a variant matching the variant-bearing selections in the supplied options.</summary>
        /// <param name="selections">Input used by this operation.</param>
        /// <param name="options">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public Variant FindBySelectionData(OptionSelectionList selections, OptionList options)
        {
            var variantSelections = new OptionSelectionList();

            foreach (var opt in options)
            {
                if (opt.IsVariant)
                {
                    var sel = selections.FindByOptionId(opt.Bvin);
                    if (sel != null)
                    {
                        variantSelections.Add(sel);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            var selectionKey = OptionSelection.GenerateUniqueKeyForSelections(variantSelections);
            return FindByKey(selectionKey);
        }
    }
}
