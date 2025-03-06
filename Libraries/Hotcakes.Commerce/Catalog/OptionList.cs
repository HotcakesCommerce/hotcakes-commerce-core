#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Text;

namespace Hotcakes.Commerce.Catalog
{
    public class OptionList : List<Option>
    {
        private const string UL_OPEN = "<ul class=\"lineitemoptions\">";
        private const string LINE_ITEM = "<li>{0}</li>";
        private const string UL_CLOSE = "</ul>";

        public List<Option> VariantsOnly()
        {
            return this.Where(y => y.IsVariant).ToList();
        }

        public bool ContainsVariantSelection(OptionSelection selection)
        {
            // look through a list of options to see if it contains a valid option
            // for the given selection data

            var result = false;

            foreach (var o in VariantsOnly())
            {
                if (o.Bvin.Replace("-", string.Empty) == selection.OptionBvin.Replace("-", string.Empty))
                {
                    if (o.ItemsContains(selection.SelectionData))
                    {
                        return true;
                    }
                }
            }

            return result;
        }

        public string CartDescription(OptionSelectionList selections)
        {
            var sb = new StringBuilder();
            sb.Append(UL_OPEN);
            foreach (var opt in this)
            {
                var desc = opt.CartDescription(selections);
                if (desc.Length > 0)
                {
                    sb.AppendFormat(LINE_ITEM, desc);
                }
            }
            sb.Append(UL_CLOSE);
            return sb.ToString();
        }
    }
}