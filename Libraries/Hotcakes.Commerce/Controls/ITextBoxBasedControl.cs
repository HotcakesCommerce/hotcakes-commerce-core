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

namespace Hotcakes.Commerce.Controls
{
    /// <summary>
    /// Defines the contract for a control that exposes attributes on its rendered text box.
    /// </summary>
    /// <remarks>
    /// Implementations decide how and when the supplied attributes are applied to the
    /// underlying text box. This interface does not require the control to expose the
    /// text box itself.
    /// </remarks>
    public interface ITextBoxBasedControl
    {
        /// <summary>
        /// Adds or updates an attribute on the text box represented by the control.
        /// </summary>
        /// <param name="key">The attribute name to apply.</param>
        /// <param name="value">The attribute value to apply.</param>
        /// <remarks>
        /// Validation, duplicate-key behavior, and the point in the page/control lifecycle
        /// at which the attribute becomes visible are defined by the implementation.
        /// </remarks>
        void AddTextBoxAttribute(string key, string value);
    }
}
