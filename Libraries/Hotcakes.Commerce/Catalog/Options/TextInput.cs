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
        public OptionTypes GetOptionType()
        {
            return OptionTypes.TextInput;
        }

        //set { BaseOption.Settings.AddOrUpdate("rows", value); }
        //set { BaseOption.Settings.AddOrUpdate("cols", value); }

        public string Render(Option baseOption)
        {
            return RenderWithSelection(baseOption, null);
        }

        public string RenderWithSelection(Option baseOption, OptionSelectionList selections, string prefix = null)
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
            var c = GetColumns(baseOption);
            if (string.IsNullOrEmpty(c)) c = "20";
            var r = GetRows(baseOption);
            if (string.IsNullOrEmpty(r)) r = "1";

            var optionNameId = "opt" + prefix + baseOption.Bvin.Replace("-", string.Empty);
            if (r != "1")
            {
                sb.Append("<textarea id=\"" + optionNameId + "\" cols=\"" + c + "\" rows=\"" + r + "\" ");
                sb.Append(" name=\"" + optionNameId + "\" ");
                sb.Append(">");
                sb.Append(selected);
                sb.Append("</textarea>");
            }
            else
            {
                sb.Append("<input type=\"text\" id=\"" + optionNameId + "\" cols=\"" + c + "\" maxlength=\"" +
                          GetMaxLength(baseOption) + "\"");
                sb.Append(" name=\"" + optionNameId + "\" ");
                sb.Append(" value=\"" + selected + "\"");
                sb.Append("/>");
            }

            return sb.ToString();
        }

        public void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new TextBox();
            result.ID = "opt" + prefix + baseOption.Bvin.Replace("-", string.Empty);
            result.ClientIDMode = ClientIDMode.Static;

            var c = GetColumns(baseOption);
            if (string.IsNullOrEmpty(c)) c = "20";
            var r = GetRows(baseOption);
            if (string.IsNullOrEmpty(r)) r = "1";

            var rint = 1;
            int.TryParse(r, out rint);
            var cint = 20;
            int.TryParse(c, out cint);
            var mint = 255;
            int.TryParse(GetMaxLength(baseOption), out mint);

            result.Rows = rint;
            result.Columns = cint;
            result.MaxLength = mint;

            if (r != "1")
            {
                result.TextMode = TextBoxMode.MultiLine;
            }

            ph.Controls.Add(result);
        }

        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;

            var tb = (TextBox)ph.FindControl("opt" + prefix + baseOption.Bvin.Replace("-", string.Empty));
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
            var formid = "opt" + prefix + baseOption.Bvin.Replace("-", string.Empty);
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

            var tb = (TextBox)ph.FindControl("opt" + baseOption.Bvin.Replace("-", string.Empty));
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
                return baseOption.Name + ": " + HttpUtility.HtmlEncode(val.SelectionData);
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
            baseOption.Settings.AddOrUpdate("rows", value);
        }

        public string GetRows(Option baseOption)
        {
            return baseOption.Settings.GetSettingOrEmpty("rows");
        }

        public void SetColumns(Option baseOption, string value)
        {
            baseOption.Settings.AddOrUpdate("cols", value);
        }

        public string GetColumns(Option baseOption)
        {
            return baseOption.Settings.GetSettingOrEmpty("cols");
        }

        public void SetMaxLength(Option baseOption, string value)
        {
            baseOption.Settings.AddOrUpdate("maxlength", value);
        }

        public string GetMaxLength(Option baseOption)
        {
            return baseOption.Settings.GetSettingOrEmpty("maxlength");
        }
    }
}