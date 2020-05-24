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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog.Options
{
    public class CheckBoxes : IOptionProcessor
    {
        private const string CHECKBOX_MARKUP_FORMAT = "<input type=\"checkbox\" style=\"display: inline !important;\" class=\"hcIsOption check{1} {0}\" name=\"opt{2}{1}\" value=\"{3}\"";

        public OptionTypes GetOptionType()
        {
            return OptionTypes.CheckBoxes;
        }

        public string Render(Option baseOption)
        {
            return RenderWithSelection(baseOption, null);
        }

        public string RenderWithSelection(Option option, OptionSelectionList selections, string prefix = null, string className = null)
        {
            var sb = new StringBuilder();
            var bvin = option.Bvin.Replace("-", string.Empty);

            foreach (var oi in option.Items)
            {
                sb.Append("<label>");
                if (!oi.IsLabel)
                {
                    var oiBvin = oi.Bvin.Replace("-", string.Empty);
                    sb.AppendFormat(CHECKBOX_MARKUP_FORMAT, className, bvin, prefix, oiBvin);
                    //sb.Append(
                    //    "<input type=\"checkbox\" style=\"display: inline !important;\" class=\"hcIsOption  check" +
                    //    bvin +
                    //    "\" name=\"opt" + prefix + bvin +
                    //    "\" value=\"" + oiBvin + "\"");

                    if (IsSelected(oi, selections))
                    {
                        sb.Append(" checked=\"checked\" ");
                    }

                    sb.Append("/>");
                }
                sb.Append(string.Concat(oi.Name, "</label><br />"));
            }

            return sb.ToString();
        }

        public void RenderAsControl(Option option, PlaceHolder ph, string prefix = null, string className = null)
        {
            foreach (var oi in option.Items)
            {
                ph.Controls.Add(new LiteralControl(" <label class=\"CreateOrderChoicelabel\">"));
                if (!oi.IsLabel)
                {
                    var cb = new HtmlInputCheckBox();
                    cb.ClientIDMode = ClientIDMode.Static;
                    cb.ID = string.Concat("opt", prefix, oi.Bvin.Replace("-", string.Empty));
                    cb.Name = string.Concat("opt", prefix, option.Bvin.Replace("-", string.Empty));
                    cb.Attributes["class"] = string.Format("hcIsOption check{0} {1}", option.Bvin.Replace("-", string.Empty), className);

                    cb.Value = oi.Bvin.Replace("-", string.Empty);
                    ph.Controls.Add(cb);
                }
                ph.Controls.Add(new LiteralControl(string.Concat(" ", oi.Name, "</label> <br /> ")));
            }
        }

        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;

            var optionBvins = new List<string>();
            foreach (var o in baseOption.Items)
            {
                if (!o.IsLabel)
                {
                    var checkId = "opt" + prefix + o.Bvin.Replace("-", string.Empty);
                    var cb = (HtmlInputCheckBox) ph.FindControl(checkId);
                    if (cb != null && cb.Checked)
                    {
                        optionBvins.Add(o.Bvin);
                    }
                }
            }

            result.SelectionData = string.Join(",", optionBvins);

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

            var vals = val.SelectionData.Split(',');
            foreach (var s in vals)
            {
                var checkId = string.Concat("opt", s.Replace("-", string.Empty));
                var cb = (HtmlInputCheckBox) ph.FindControl(checkId);
                if (cb != null)
                {
                    cb.Checked = true;
                }
            }
        }

        public string CartDescription(Option baseOption, OptionSelectionList selections)
        {
            if (selections == null) return string.Empty;
            var val = selections.FindByOptionId(baseOption.Bvin);
            if (val == null) return string.Empty;
            var vals = val.SelectionData.Split(',');

            var result = string.Concat(baseOption.Name, ": ");
            var first = true;

            foreach (var oi in baseOption.Items)
            {
                var cleaned = OptionSelection.CleanBvin(oi.Bvin);
                if (vals.Contains(cleaned))
                {
                    if (!first)
                    {
                        result += ", ";
                    }
                    else
                    {
                        first = false;
                    }
                    result += oi.Name;
                }
            }

            return result;
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

        #region Private

        private bool IsSelected(OptionItem item, OptionSelectionList selections)
        {
            var result = false;
            if (selections == null || selections.Count < 1)
            {
                return item.IsDefault;
            }

            var val = selections.FindByOptionId(item.OptionBvin);
            if (val == null) return result;

            var vals = val.SelectionData.Split(',');
            foreach (var s in vals)
            {
                if (s == item.Bvin.Replace("-", string.Empty))
                {
                    return true;
                }
            }
            return result;
        }

        #endregion
    }
}