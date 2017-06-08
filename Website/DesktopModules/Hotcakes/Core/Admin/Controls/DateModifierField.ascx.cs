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
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Controls;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class DateModifierField : ModificationControl<DateTime>
    {
        public void InitializeValue(string val)
        {
            DateTextBox.Text = val;
        }

        public override DateTime ApplyChanges(DateTime item)
        {
            var num = 0;
            try
            {
                if (DateDropDownList.SelectedIndex > (int) Modes.SetTo)
                {
                    if (!int.TryParse(DateTextBox.Text, out num))
                    {
                        num = 0;
                    }
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.SetTo)
                {
                    DateTime val;
                    if (DateTime.TryParse(DateTextBox.Text, out val))
                    {
                        return val;
                    }
                    return item;
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.AddDays)
                {
                    return item.AddDays(num);
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.SubtractDays)
                {
                    return item.AddDays(num*-1);
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.AddMonths)
                {
                    return item.AddMonths(num);
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.SubtractMonths)
                {
                    return item.AddMonths(num*-1);
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.AddYears)
                {
                    return item.AddYears(num);
                }
                if (DateDropDownList.SelectedIndex == (int) Modes.SubtractYears)
                {
                    return item.AddYears(num*-1);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                EventLog.LogEvent(ex);
                return item;
            }
            return item;
        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (args.Value != string.Empty)
            {
                if (DateDropDownList.SelectedIndex == (int) Modes.SetTo)
                {
                    DateTime temp;
                    if (!DateTime.TryParse(args.Value, out temp))
                    {
                        ((CustomValidator) source).ErrorMessage = "Field must be a valid date.";
                        args.IsValid = false;
                    }
                }
                else
                {
                    int temp;
                    if (!int.TryParse(args.Value, out temp))
                    {
                        ((CustomValidator) source).ErrorMessage = "Field must be a number.";
                        args.IsValid = false;
                    }
                }
            }
        }

        private enum Modes
        {
            SetTo = 0,
            AddDays = 1,
            SubtractDays = 2,
            AddMonths = 3,
            SubtractMonths = 4,
            AddYears = 5,
            SubtractYears = 6
        }
    }
}