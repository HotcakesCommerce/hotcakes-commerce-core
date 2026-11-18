#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog.Options
{
    /// <summary>
    /// Implements the <see cref="IOptionProcessor" /> contract for informational HTML options.
    /// </summary>
    public class Html : IOptionProcessor
    {
        /// <summary>
        /// Performs the GetOptionType catalog operation.
        /// </summary>
        public OptionTypes GetOptionType()
        {
            return OptionTypes.Html;
        }

        /// <summary>
        /// Performs the Render catalog operation.
        /// </summary>
        public string Render(Option baseOption)
        {
            return baseOption.TextSettings.GetSettingOrEmpty("html");
        }

        /// <summary>
        /// Performs the RenderWithSelection catalog operation.
        /// </summary>
        public string RenderWithSelection(Option baseOption, OptionSelectionList selections, string prefix = null, string className = null)
        {
            return Render(baseOption);
        }

        /// <summary>
        /// Performs the RenderAsControl catalog operation.
        /// </summary>
        public void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null, string className = null)
        {
            var result = new LiteralControl(baseOption.TextSettings.GetSettingOrEmpty("html"));
            ph.Controls.Add(result);
        }

        /// <summary>
        /// Performs the ParseFromPlaceholder catalog operation.
        /// </summary>
        public OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null)
        {
            return null;
        }

        /// <summary>
        /// Performs the ParseFromForm catalog operation.
        /// </summary>
        public OptionSelection ParseFromForm(Option baseOption, NameValueCollection form, string prefix = null)
        {
            return null;
        }

        /// <summary>
        /// Performs the SetSelectionsInPlaceholder catalog operation.
        /// </summary>
        public void SetSelectionsInPlaceholder(Option baseOption, PlaceHolder ph, OptionSelectionList selections)
        {
            // do nothing;
        }

        /// <summary>
        /// Performs the CartDescription catalog operation.
        /// </summary>
        public string CartDescription(Option baseOption, OptionSelectionList selections)
        {
            return string.Empty;
        }

        /// <summary>
        /// Performs the GetSelectionValues catalog operation.
        /// </summary>
        public List<string> GetSelectionValues(Option baseOption, OptionSelectionList selections)
        {
            return new List<string>();
        }
    }
}
