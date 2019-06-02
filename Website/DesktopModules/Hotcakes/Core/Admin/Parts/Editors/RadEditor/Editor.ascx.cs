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

using System;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;

namespace Hotcakes.Modules.Core.Admin.Parts.Editors.RadEditor
{
    public partial class Editor : TextEditorBase
    {
        public override int EditorWidth
        {
            get { return (int) radEditor.Width.Value; }
            set { radEditor.Width = Unit.Pixel(value); }
        }

        public override int EditorHeight
        {
            get { return (int) radEditor.Height.Value - 50; }
            set
            {
                // reserve 50px for toolbars
                radEditor.Height = Unit.Pixel(value + 50);
            }
        }

        public override bool EditorWrap { get; set; }


        public override string Text
        {
            get { return radEditor.Content; }
            set { radEditor.Content = value; }
        }

        public override int TabIndex { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}