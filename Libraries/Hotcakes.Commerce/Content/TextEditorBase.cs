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

using System.Web.UI;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Provides the common contract for Web Forms text editor controls used by Hotcakes Commerce.
    /// </summary>
    /// <remarks>
    /// Concrete editors determine how these values are rendered. This base class only exposes the shared editor
    /// surface; it does not impose validation, storage, or markup behavior.
    /// </remarks>
    public abstract class TextEditorBase : UserControl
    {
        /// <summary>
        /// Gets or sets the editor width in the units defined by the concrete editor implementation.
        /// </summary>
        public abstract int EditorWidth { get; set; }

        /// <summary>
        /// Gets or sets the editor height in the units defined by the concrete editor implementation.
        /// </summary>
        public abstract int EditorHeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the concrete editor wraps text.
        /// </summary>
        public abstract bool EditorWrap { get; set; }

        /// <summary>
        /// Gets or sets the text currently represented by the editor.
        /// </summary>
        /// <remarks>
        /// Interpretation and encoding of the text are determined by the concrete editor implementation.
        /// </remarks>
        public abstract string Text { get; set; }

        /// <summary>
        /// Gets or sets the tab-order index assigned to the editor control.
        /// </summary>
        public abstract int TabIndex { get; set; }
    }
}
