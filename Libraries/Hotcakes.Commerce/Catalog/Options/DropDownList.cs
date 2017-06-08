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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog.Options
{
    public class DropDownList : IOptionProcessor
    {
        public OptionTypes GetOptionType()
        {
            return OptionTypes.DropDownList;
        }

        public string Render(Option baseOption)
        {
            return RenderWithSelection(baseOption, null);
        }

        public string RenderWithSelection(Option baseOption, OptionSelectionList selections, string prefix = null)
        {
            string selected = null;
            if (selections != null)
            {
                var sel = selections.FindByOptionId(baseOption.Bvin);
                if (sel != null)
                {
                    selected = sel.SelectionData;
                }
            }

            var sb = new StringBuilder();

            sb.Append("<select id=\"opt" + prefix + baseOption.Bvin.Replace("-", string.Empty) + "\" ");
            sb.Append(" name=\"opt" + prefix + baseOption.Bvin.Replace("-", "") + "\" ");
            sb.Append(" class=\"hcIsOption\" >");

            foreach (var o in baseOption.Items)
            {
                if (o.IsLabel)
                {
                    sb.Append("<option value=\"\" disabled");
                    //sb.Append("<option value=\"systemlabel\" disabled");

                    if (string.IsNullOrEmpty(selected) && o.IsDefault)
                    {
                        sb.Append(" selected ");
                    }
                    sb.Append(">" + o.Name + "</option>");
                }
                else
                {
                    sb.Append("<option value=\"" + o.Bvin.Replace("-", string.Empty) + "\"");
                    if (o.Bvin.Replace("-", string.Empty) == selected || (string.IsNullOrEmpty(selected) && o.IsDefault))
                    {
                        sb.Append(" selected ");
                    }
                    sb.Append(">" + o.Name + "</option>");
                }
            }
            sb.Append("</select>");

            return sb.ToString();
        }

        public void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new System.Web.UI.WebControls.DropDownList();
            result.ID = "opt" + prefix + baseOption.Bvin.Replace("-", string.Empty);
            result.ClientIDMode = ClientIDMode.Static;
            result.CssClass = "hcIsOption";

            foreach (var o in baseOption.Items)
            {
                var li = new ListItem();
                li.Text = o.Name;

                if (o.IsLabel)
                {
                    li.Value = string.Empty;
                    li.Enabled = false;
                }
                else
                {
                    li.Value = o.Bvin.Replace("-", string.Empty);
                }
                if (o.IsDefault)
                    li.Selected = true;
                result.Items.Add(li);
            }

            ph.Controls.Add(result);
        }

        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;

            var ddl =
                (System.Web.UI.WebControls.DropDownList)
                    ph.FindControl("opt" + prefix + baseOption.Bvin.Replace("-", string.Empty));
            if (ddl != null && !string.IsNullOrEmpty(ddl.SelectedValue))
            {
                result.SelectionData = ddl.SelectedValue;
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
                result.SelectionData = value;
            }
            return result;
        }

        public void SetSelectionsInPlaceholder(Option baseOption, PlaceHolder ph, OptionSelectionList selections)
        {
            if (ph == null) return;
            if (selections == null) return;
            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null) return;

            var ddl = (System.Web.UI.WebControls.DropDownList)ph.FindControl("opt" + baseOption.Bvin.Replace("-", string.Empty));
            if (ddl != null)
            {
                if (ddl.Items.FindByValue(val.SelectionData) != null)
                {
                    ddl.ClearSelection();
                    ddl.Items.FindByValue(val.SelectionData).Selected = true;
                }
            }
        }

        public string CartDescription(Option baseOption, OptionSelectionList selections)
        {
            if (selections == null) return string.Empty;
            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null) return string.Empty;

            foreach (var oi in baseOption.Items)
            {
                var cleaned = OptionSelection.CleanBvin(oi.Bvin);
                if (cleaned == val.SelectionData)
                {
                    return baseOption.Name + ": " + oi.Name;
                }
            }

            return string.Empty;
        }

        public List<string> GetSelectionValues(Option option, OptionSelectionList selections)
        {
            var opSel = selections.FindByOptionId(option.Bvin);
            var data = opSel != null ? opSel.SelectionData : null;
            var vals = new List<string>();

            if (!string.IsNullOrEmpty(data))
            {
                vals = option.Items.Where(i => data == i.Bvin.Replace("-", string.Empty)).Select(i => i.Name).ToList();
            }

            return vals;
        }
    }
}