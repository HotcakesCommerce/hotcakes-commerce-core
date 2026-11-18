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
using System.Web.UI.WebControls;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Represents the catalog interface <see cref="IOptionProcessor" /> and its associated behavior.
    /// </summary>
    public interface IOptionProcessor
    {
        /// <summary>
        /// Performs the catalog operation represented by <c>GetOptionType</c>.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        OptionTypes GetOptionType();
        /// <summary>
        /// Performs the catalog operation represented by <c>Render</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        string Render(Option baseOption);
        /// <summary>
        /// Performs the catalog operation represented by <c>RenderWithSelection</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="selections">Input used by the operation.</param>
        /// <param name="prefix">Input used by the operation.</param>
        /// <param name="className">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        string RenderWithSelection(Option baseOption, OptionSelectionList selections, string prefix = null, string className = null);
        /// <summary>
        /// Performs the catalog operation represented by <c>RenderAsControl</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="ph">Input used by the operation.</param>
        /// <param name="prefix">Input used by the operation.</param>
        /// <param name="className">Input used by the operation.</param>
        void RenderAsControl(Option baseOption, PlaceHolder ph, string prefix = null, string className = null);
        /// <summary>
        /// Performs the catalog operation represented by <c>ParseFromPlaceholder</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="ph">Input used by the operation.</param>
        /// <param name="prefix">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        OptionSelection ParseFromPlaceholder(Option baseOption, PlaceHolder ph, string prefix = null);
        /// <summary>
        /// Performs the catalog operation represented by <c>ParseFromForm</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="form">Input used by the operation.</param>
        /// <param name="prefix">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        OptionSelection ParseFromForm(Option baseOption, NameValueCollection form, string prefix = null);
        /// <summary>
        /// Performs the catalog operation represented by <c>SetSelectionsInPlaceholder</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="ph">Input used by the operation.</param>
        /// <param name="selections">Input used by the operation.</param>
        void SetSelectionsInPlaceholder(Option baseOption, PlaceHolder ph, OptionSelectionList selections);
        /// <summary>
        /// Performs the catalog operation represented by <c>CartDescription</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="selections">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        string CartDescription(Option baseOption, OptionSelectionList selections);

        /// <summary>
        /// Performs the catalog operation represented by <c>GetSelectionValues</c>.
        /// </summary>
        /// <param name="baseOption">Input used by the operation.</param>
        /// <param name="selections">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<string> GetSelectionValues(Option baseOption, OptionSelectionList selections);
    }
}
