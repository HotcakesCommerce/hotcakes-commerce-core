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

using System;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;

namespace Hotcakes.Modules.Core.Admin.Parts.Editors.None
{
    partial class Editor : TextEditorBase
    {
        public override int EditorHeight
        {
            get { return (int) EditorField.Height.Value; }
            set { EditorField.Height = Unit.Pixel(value); }
        }

        public override int EditorWidth
        {
            get { return (int) EditorField.Width.Value; }
            set { EditorField.Width = Unit.Pixel(value); }
        }

        public override bool EditorWrap
        {
            get { return EditorField.Wrap; }
            set { EditorField.Wrap = value; }
        }

        public override int TabIndex
        {
            get { return EditorField.TabIndex; }
            set { EditorField.TabIndex = (short) value; }
        }

        public override string Text
        {
            get { return EditorField.Text; }
            set { EditorField.Text = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditorField.CausesValidation = false;
        }
    }
}