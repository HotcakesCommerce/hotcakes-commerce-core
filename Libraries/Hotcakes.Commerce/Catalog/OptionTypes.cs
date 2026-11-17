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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Gets or sets the option types.
    /// </summary>
    /// <summary>
    /// Identifies the processor used to render and parse a product option. The numeric values are persisted and must not be changed.
    /// </summary>
    public enum OptionTypes
    {
        /// <summary>Renders a single-value drop-down list.</summary>
        DropDownList = 100,
        /// <summary>Renders a single-value radio-button list.</summary>
        RadioButtonList = 200,
        /// <summary>Renders a multi-value checkbox list.</summary>
        CheckBoxes = 300,
        /// <summary>Renders informational HTML without a customer selection.</summary>
        Html = 400,
        /// <summary>Renders a text input or multi-line text area.</summary>
        TextInput = 500,
        /// <summary>Renders the file-upload option and its upload metadata.</summary>
        FileUpload = 600
    }
}
