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
using System.Web.UI;
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class HtmlEditor : UserControl
    {
        private TextEditorBase _Editor;

        public string Text
        {
            get { return _Editor.Text; }
            set { _Editor.Text = value; }
        }

        public int EditorWidth
        {
            get { return int.Parse(WidthField.Value); }
            set { WidthField.Value = value.ToString(); }
        }

        public int EditorHeight
        {
            get { return int.Parse(HeightField.Value); }
            set { HeightField.Value = value.ToString(); }
        }

        public bool EditorWrap
        {
            get
            {
                if (WrapField.Value == "1")
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    WrapField.Value = "1";
                }
                else
                {
                    WrapField.Value = "0";
                }
            }
        }

        public int TabIndex
        {
            get
            {
                var obj = ViewState["TabIndex"];
                if (obj != null)
                {
                    return (int) obj;
                }
                return -1;
            }
            set { ViewState["TabIndex"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _Editor = (TextEditorBase) HccPartController.LoadDefaultEditor(Page);
            phEditor.Controls.Add(_Editor);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                _Editor.EditorHeight = EditorHeight;
                _Editor.EditorWidth = EditorWidth;
                _Editor.EditorWrap = EditorWrap;
                if (TabIndex != -1)
                {
                    _Editor.TabIndex = TabIndex;
                }
            }
        }
    }
}