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
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog.Options
{
    public class TextInput : IOptionProcessor
    {
        private const string TEXTAREA_MARKUP_FORMAT = "<textarea id=\"{0}\" cols=\"{1}\" rows=\"{2}\" name=\"{0}\" class=\"{4}\">{3}</textarea>";
        private const string TEXTBOX_MARKUP_FORMAT = "<input type=\"text\" id=\"{0}\" maxlength=\"{1}\" name=\"{0}\" class=\"{3}\" value=\"{2}\" />";

        private const string OPTION_NAME_ROWS = "rows";
        private const string OPTION_NAME_COLUMNS = "cols";
        private const string OPTION_NAME_MAXLENGTH = "maxlength";

        private const string DEFAULT_COLS = "20";
        private const string DEFAULT_ROWS = "1";

        public OptionTypes GetOptionType()
        {
            return OptionTypes.TextInput;
        }

        public string Render(Option baseOption)
        {
            return RenderWithSelection(baseOption, null);
        }

        public string RenderWithSelection(Option baseOption, OptionSelectionList selections, string prefix = null, string className = null)
        {
            var selected = string.Empty;
            if (selections != null)
            {
                var sel = selections.FindByOptionId(baseOption.Bvin);
                if (sel != null)
                {
                    selected = sel.SelectionData;
                }
            }

            var sb = new StringBuilder();

            var cols = GetColumns(baseOption);
            if (string.IsNullOrEmpty(cols)) cols = DEFAULT_COLS;

            var rows = GetRows(baseOption);
            if (string.IsNullOrEmpty(rows)) rows = DEFAULT_ROWS;

            var optionNameId = string.Concat("opt", prefix, baseOption.Bvin.Replace("-", string.Empty));

            if (rows != DEFAULT_ROWS)
            {
                sb.AppendFormat(TEXTAREA_MARKUP_FORMAT, optionNameId, cols, rows, selected, className);
                //sb.Append("<textarea id=\"" + optionNameId + "\" cols=\"" + c + "\" rows=\"" + r + "\" ");
                //sb.Append(" name=\"" + optionNameId + "\" ");
                //sb.Append(">");
                //sb.Append(selected);
                //sb.Append("</textarea>");
            }
            else
            {
                var maxLength = GetMaxLength(baseOption);

                // HCC 03.03.00 ignoring the cols HTML attribute in <input type="text" />
                sb.AppendFormat(TEXTBOX_MARKUP_FORMAT, optionNameId, maxLength, selected, className);
                //sb.Append("<input type=\"text\" id=\"" + optionNameId + "\" cols=\"" + c + "\" maxlength=\"" +
                //          maxLength + "\"");
                //sb.Append(" name=\"" + optionNameId + "\" ");
                //sb.Append(" value=\"" + selected + "\"");
                //sb.Append("/>");
            }

            return sb.ToString();
        }

        public void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null, string className = null)
        {
            var textControl = new TextBox();
            textControl.ID = string.Concat("opt", prefix, baseOption.Bvin.Replace("-", string.Empty));
            textControl.ClientIDMode = ClientIDMode.Static;

            var cols = GetColumns(baseOption);
            if (string.IsNullOrEmpty(cols)) cols = DEFAULT_COLS;
            var rows = GetRows(baseOption);
            if (string.IsNullOrEmpty(rows)) rows = DEFAULT_ROWS;

            var rint = 1;
            int.TryParse(rows, out rint);
            var cint = 20;
            int.TryParse(cols, out cint);
            var mint = 255;
            int.TryParse(GetMaxLength(baseOption), out mint);

            textControl.MaxLength = mint;

            if (rows != DEFAULT_ROWS)
            {
                // HCC 03.03.00 ignoring the cols HTML attribute in <input type="text" />
                textControl.Rows = rint;
                textControl.Columns = cint;
                textControl.TextMode = TextBoxMode.MultiLine;
            }

            textControl.CssClass = className;

            ph.Controls.Add(textControl);
        }

        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;

            var tb = (TextBox)ph.FindControl(string.Concat("opt", prefix, baseOption.Bvin.Replace("-", string.Empty)));
            if (tb != null)
            {
                result.SelectionData = tb.Text.Trim();
            }

            return result;
        }

        public OptionSelection ParseFromForm(Option baseOption, NameValueCollection form, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;
            var formid = string.Concat("opt", prefix, baseOption.Bvin.Replace("-", string.Empty));
            var value = form[formid];
            if (value != null)
            {
                result.SelectionData = value.Trim();
            }
            return result;
        }

        public void SetSelectionsInPlaceholder(Option baseOption, PlaceHolder ph, OptionSelectionList selections)
        {
            if (ph == null) return;
            if (selections == null) return;
            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null) return;

            var tb = (TextBox)ph.FindControl(string.Concat("opt", baseOption.Bvin.Replace("-", string.Empty)));
            if (tb != null)
            {
                tb.Text = val.SelectionData;
            }
        }

        public string CartDescription(Option baseOption, OptionSelectionList selections)
        {
            if (selections == null) return string.Empty;
            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null) return string.Empty;

            if (val.SelectionData.Trim().Length > 0)
            {
                return string.Concat(baseOption.Name, ": ", HttpUtility.HtmlEncode(val.SelectionData));
            }

            return string.Empty;
        }

        public List<string> GetSelectionValues(Option option, OptionSelectionList selections)
        {
            var opSel = selections.FindByOptionId(option.Bvin);
            var data = opSel != null ? opSel.SelectionData : null;
            var vals = new List<string> {data};
            return vals;
        }

        public void SetRows(Option baseOption, string value)
        {
            baseOption.Settings.AddOrUpdate(OPTION_NAME_ROWS, value);
        }

        public string GetRows(Option baseOption)
        {
            return baseOption.Settings.GetSettingOrEmpty(OPTION_NAME_ROWS);
        }

        public void SetColumns(Option baseOption, string value)
        {
            baseOption.Settings.AddOrUpdate(OPTION_NAME_COLUMNS, value);
        }

        public string GetColumns(Option baseOption)
        {
            return baseOption.Settings.GetSettingOrEmpty(OPTION_NAME_COLUMNS);
        }

        public void SetMaxLength(Option baseOption, string value)
        {
            baseOption.Settings.AddOrUpdate(OPTION_NAME_MAXLENGTH, value);
        }

        public string GetMaxLength(Option baseOption)
        {
            return baseOption.Settings.GetSettingOrEmpty(OPTION_NAME_MAXLENGTH);
        }
    }
}