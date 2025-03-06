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

using Hotcakes.Commerce.Controls;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class FloatModifierField : ModificationControl<double>, ITextBoxBasedControl
    {
        public void AddTextBoxAttribute(string key, string value)
        {
            FloatTextBox.Attributes.Add(key, value);
        }

        public override double ApplyChanges(double item)
        {
            if (FloatDropDownList.SelectedIndex == (int) Modes.SetTo)
            {
                double val = 0;
                if (double.TryParse(FloatTextBox.Text, out val))
                {
                    return val;
                }
                return item;
            }
            if (FloatDropDownList.SelectedIndex == (int) Modes.AddTo)
            {
                double val = 0;
                if (double.TryParse(FloatTextBox.Text, out val))
                {
                    return item + val;
                }
                return item;
            }
            if (FloatDropDownList.SelectedIndex == (int) Modes.SubtractFrom)
            {
                double val = 0;
                if (double.TryParse(FloatTextBox.Text, out val))
                {
                    return item - val;
                }
                return item;
            }

            return item;
        }

        private enum Modes
        {
            SetTo = 0,
            AddTo = 1,
            SubtractFrom = 2
        }
    }
}