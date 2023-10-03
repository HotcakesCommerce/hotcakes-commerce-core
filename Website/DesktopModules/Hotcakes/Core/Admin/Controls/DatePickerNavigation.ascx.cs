#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Web.UI;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class DatePickerNavigation : UserControl
    {
        private const string DATEFORMAT = "MM/dd/yyyy";
        private DateTime dteOutput;

        public DateTime SelectedDate
        {
            get
            {
                if (DateIsValid())
                {
                    return dteOutput;
                }
                var hccPage = Page as IHccPage;
                return DateHelper.ConvertUtcToStoreTime(hccPage.HccApp);
            }
            set { radDatePicker.Text = value.ToString(DATEFORMAT); }
        }

        public event EventHandler SelectedDateChanged;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            lnkPrev.Click += (s, a) =>
            {
                if (DateIsValid())
                {
                    radDatePicker.Text = dteOutput.AddDays(-1).ToString(DATEFORMAT);
                    if (SelectedDateChanged != null)
                        SelectedDateChanged(this, EventArgs.Empty);
                }
            };

            lnkNext.Click += (s, a) =>
            {
                if (DateIsValid())
                {
                    radDatePicker.Text = dteOutput.AddDays(1).ToString(DATEFORMAT);
                    if (SelectedDateChanged != null)
                        SelectedDateChanged(this, EventArgs.Empty);
                }
            };

            radDatePicker.TextChanged += (s, a) =>
            {
                if (SelectedDateChanged != null)
                    SelectedDateChanged(this, EventArgs.Empty);
            };
        }

        private bool DateIsValid()
        {
            if (!string.IsNullOrEmpty(radDatePicker.Text))
            {
                var blnIsValid = false;
                blnIsValid = DateTime.TryParse(radDatePicker.Text.Trim(), out dteOutput);
                return blnIsValid;
            }

            return false;
        }
    }
}