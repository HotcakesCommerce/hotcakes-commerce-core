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

using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class PercentAmountSelection : UserControl
    {
        private string _validationGroup = string.Empty;

        public AmountTypes AmountType
        {
            get { return (AmountTypes) AmountDropDownList.SelectedIndex; }
            set { AmountDropDownList.SelectedIndex = (int) value; }
        }

        public decimal Amount
        {
            get { return decimal.Parse(AmountTextBox.Text, NumberStyles.Currency); }
            set
            {
                if (AmountType == AmountTypes.MonetaryAmount)
                {
                    AmountTextBox.Text = value.ToString("c");
                }
                else
                {
                    AmountTextBox.Text = value.ToString();
                }
            }
        }

        public string ValidationGroup
        {
            get { return _validationGroup; }
            set
            {
                _validationGroup = value;
                PercentCustomValidator.ValidationGroup = _validationGroup;
            }
        }

        protected void PercentCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (AmountDropDownList.SelectedIndex == (int) AmountTypes.Percent)
            {
                if (source is CustomValidator)
                {
                    ((CustomValidator) source).ErrorMessage = "Percent must be between 0.00 and 100.00 percent.";
                }
                decimal val = 0;
                if (decimal.TryParse(AmountTextBox.Text,
                    NumberStyles.Number,
                    Thread.CurrentThread.CurrentUICulture, out val))
                {
                    if (val < 0 | val > 100)
                    {
                        args.IsValid = false;
                    }
                }
                else
                {
                    args.IsValid = false;
                }
            }
            else if (AmountDropDownList.SelectedIndex == (int) AmountTypes.MonetaryAmount)
            {
                if (source is CustomValidator)
                {
                    ((CustomValidator) source).ErrorMessage = "Value must be a monetary amount.";
                }
                decimal val = 0;
                if (decimal.TryParse(AmountTextBox.Text, NumberStyles.Currency,
                    Thread.CurrentThread.CurrentUICulture, out val))
                {
                    if (val < 0)
                    {
                        args.IsValid = false;
                    }
                }
                else
                {
                    args.IsValid = false;
                }
            }
        }
    }
}