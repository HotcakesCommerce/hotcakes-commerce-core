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

using System.Web.UI;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class TimespanPicker : UserControl
    {
        public int Months
        {
            get
            {
                if (!string.IsNullOrEmpty(MonthsDropDownList.SelectedValue))
                {
                    return int.Parse(MonthsDropDownList.SelectedValue);
                }
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    MonthsDropDownList.SelectedValue = value.ToString();
                }
            }
        }

        public int Days
        {
            get
            {
                if (!string.IsNullOrEmpty(DaysDropDownList.SelectedValue))
                {
                    return int.Parse(DaysDropDownList.SelectedValue);
                }
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    DaysDropDownList.SelectedValue = value.ToString();
                }
            }
        }

        public int Hours
        {
            get
            {
                if (!string.IsNullOrEmpty(HoursDropDownList.SelectedValue))
                {
                    return int.Parse(HoursDropDownList.SelectedValue);
                }
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    HoursDropDownList.SelectedValue = value.ToString();
                }
            }
        }

        public int Minutes
        {
            get
            {
                if (!string.IsNullOrEmpty(MinutesDropDownList.SelectedValue))
                {
                    return int.Parse(MinutesDropDownList.SelectedValue);
                }
                return 0;
            }
            set
            {
                if (value > 0)
                {
                    MinutesDropDownList.SelectedValue = value.ToString();
                }
            }
        }
    }
}