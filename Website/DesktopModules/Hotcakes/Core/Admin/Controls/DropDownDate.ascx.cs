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

using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class DropDownDate : UserControl
    {
        public DateTime SelectedDate
        {
            get
            {
                try
                {
                    var d = new DateTime(int.Parse(YearList.SelectedValue),
                        int.Parse(MonthList.SelectedValue),
                        int.Parse(DayList.SelectedValue));
                    return d;
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                    return new DateTime(1900, 1, 1);
                }
            }
            set
            {
                if (DayList.Items.Count < 1) PopulateDefaults();

                if (DayList.Items.FindByValue(value.Day.ToString()) != null)
                {
                    DayList.ClearSelection();
                    DayList.Items.FindByValue(value.Day.ToString()).Selected = true;
                }
                if (MonthList.Items.FindByValue(value.Month.ToString()) != null)
                {
                    MonthList.ClearSelection();
                    MonthList.Items.FindByValue(value.Month.ToString()).Selected = true;
                }
                if (YearList.Items.FindByValue(value.Year.ToString()) != null)
                {
                    YearList.ClearSelection();
                    YearList.Items.FindByValue(value.Year.ToString()).Selected = true;
                }
            }
        }

        public void SetYearRange(int startYear, int endYear)
        {
            EnsureChildControls();
            YearList.Items.Clear();
            for (var i = startYear; i <= endYear; i++)
            {
                YearList.Items.Add(i.ToString());
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (DayList.Items.Count < 1)
                {
                    PopulateDefaults();
                }
            }
        }

        public void PopulateDefaults()
        {
            for (var i = 1; i <= 12; i++)
            {
                var li = new ListItem();
                li.Value = i.ToString();
                li.Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                MonthList.Items.Add(li);
                li = null;
            }

            for (var i = 1; i <= 31; i++)
            {
                DayList.Items.Add(i.ToString());
            }

            for (var i = DateTime.Now.AddYears(-5).Year; i <= DateTime.Now.AddYears(10).Year; i++)
            {
                YearList.Items.Add(i.ToString());
            }
        }
    }
}