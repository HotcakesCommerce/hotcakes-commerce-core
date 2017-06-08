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

using Hotcakes.Commerce.Controls;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class MonetaryModifierField : ModificationControl<decimal>, ITextBoxBasedControl
    {
        public void AddTextBoxAttribute(string key, string value)
        {
            MonetaryTextBox.Attributes.Add(key, value);
        }

        public override decimal ApplyChanges(decimal item)
        {
            decimal val = 0;
            if (decimal.TryParse(MonetaryTextBox.Text, out val))
            {
                if (MonetaryDropDownList.SelectedIndex == (int) Modes.SetTo)
                {
                    return val;
                }
                if (MonetaryDropDownList.SelectedIndex == (int) Modes.IncreaseByAmount)
                {
                    return Money.ApplyIncreasedAmount(item, val);
                }
                if (MonetaryDropDownList.SelectedIndex == (int) Modes.DecreaseByAmount)
                {
                    return Money.ApplyDiscountAmount(item, val);
                }
                if (MonetaryDropDownList.SelectedIndex == (int) Modes.IncreaseByPercent)
                {
                    return Money.ApplyIncreasedPercent(item, val);
                }
                if (MonetaryDropDownList.SelectedIndex == (int) Modes.DecreaseByPercent)
                {
                    return Money.ApplyDiscountPercent(item, val);
                }
            }
            else
            {
                return item;
            }

            return item;
        }

        private enum Modes
        {
            SetTo = 0,
            IncreaseByAmount = 1,
            DecreaseByAmount = 2,
            IncreaseByPercent = 3,
            DecreaseByPercent = 4
        }
    }
}