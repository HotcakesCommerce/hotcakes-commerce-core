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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog.Options
{
    public class RadioButtonList : IOptionProcessor
    {
        private const string RADIOBUTTON_MARKUP_FORMAT = "<input type=\"radio\" name=\"opt{0}{1}\" value=\"{2}\" style=\"display: inline !important;\" class=\"hcIsOption radio{1} {3}\" ";

        public OptionTypes GetOptionType()
        {
            return OptionTypes.RadioButtonList;
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
                    
                    sb.AppendFormat(RADIOBUTTON_MARKUP_FORMAT, prefix, bvin, oiBvin, className);
                    //sb.Append("<input type=\"radio\" name=\"opt" + prefix + bvin + "\" value=\"" + oiBvin +
                    //          "\" style=\"display: inline !important;\"");
                    //sb.Append(" class=\"hcIsOption radio" + bvin + "\" ");

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

        public void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null, string className = null)
        {
            foreach (var o in baseOption.Items)
            {
                ph.Controls.Add(new LiteralControl(" <label class=\"CreateOrderChoicelabel\">"));
                if (!o.IsLabel)
                {
                    var rb = new HtmlInputRadioButton();
                    rb.ClientIDMode = ClientIDMode.Static;
                    rb.ID = string.Concat("opt", prefix, o.Bvin.Replace("-", string.Empty));
                    rb.Name = string.Concat("opt", prefix, baseOption.Bvin.Replace("-", string.Empty));
                    rb.Attributes["class"] =
                            string.Format("hcIsOption radio{0} {1}", baseOption.Bvin.Replace("-", string.Empty), className);

                    rb.Value = o.Bvin.Replace("-", string.Empty);
                    ph.Controls.Add(rb);
                }
                ph.Controls.Add(new LiteralControl(string.Concat(" ", o.Name, " </label> <br /> ")));
            }
        }

        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            var result = new OptionSelection();
            result.OptionBvin = baseOption.Bvin;

            foreach (var o in baseOption.Items)
            {
                if (!o.IsLabel)
                {
                    var radioId = string.Concat("opt", prefix, o.Bvin.Replace("-", string.Empty));
                    var rb = (HtmlInputRadioButton) ph.FindControl(radioId);
                    if (rb != null)
                    {
                        if (rb.Checked)
                        {
                            result.SelectionData = o.Bvin;
                            return result;
                        }
                    }
                }
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

            var radioId = string.Concat("opt", val.SelectionData.Replace("-", string.Empty));
            var rb = (HtmlInputRadioButton) ph.FindControl(radioId);
            if (rb != null)
            {
                rb.Checked = true;
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
                    return string.Concat(baseOption.Name, ": ", oi.Name);
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
                vals = option.Items.Where(i => data == i.Bvin.Replace("-", "")).Select(i => i.Name).ToList();
            }

            return vals;
        }

        private bool IsSelected(OptionItem item, OptionSelectionList selections)
        {
            var result = false;
            if (selections == null || selections.Count < 1)
            {
                return item.IsDefault;
            }

            var val = selections.FindByOptionId(item.OptionBvin);
            if (val == null) return result;

            if (val.SelectionData == item.Bvin.Replace("-", string.Empty)) return true;

            return result;
        }
    }
}