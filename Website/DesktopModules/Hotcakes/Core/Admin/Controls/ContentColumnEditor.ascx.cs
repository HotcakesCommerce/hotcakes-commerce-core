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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public class BlankBlock : LiteralControl
    {
        public string bvin { get; set; }
    }

    partial class ContentColumnEditor : HccUserControl
    {
        #region Properties

        public string ColumnId
        {
            get { return ViewState["ColumnId"] as string ?? string.Empty; }
            set { ViewState["ColumnId"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadBlockList();
                LoadColumn();
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var column = HccApp.ContentServices.Columns.Find(ColumnId);
            if (column != null)
            {
                var maxSortOrder = column.Blocks.Max(i => (int?) i.SortOrder) ?? 0;

                var block = new ContentBlock();
                block.ControlName = lstBlocks.SelectedValue;
                block.ColumnId = ColumnId;
                block.SortOrder = maxSortOrder + 1;
                column.Blocks.Add(block);
                HccApp.ContentServices.Columns.Update(column);
            }
            LoadColumn();
        }

        protected void gvBlocks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var b = (ContentBlock) e.Row.DataItem;

                var controlFound = false;

                var cell = e.Row.Cells[1];

                e.Row.Attributes["id"] = b.Bvin;

                // Control name gets spaces replaced for backwards compatibility
                var viewControl = HccPartController.LoadContentBlockAdminView(b.ControlName.Replace(" ", string.Empty), Page);
                if (viewControl == null)
                {
                    //No admin view, try standard view

                    // Block views are now MVC partial so we need a way to render them here
                    // There are some tricks but since this page will eventually go MVC itself
                    // we'll just put in placeholders for now.
                    //viewControl = HccPartController.LoadContentBlock(b.ControlName, Page);
                    viewControl = new BlankBlock {bvin = b.Bvin, Text = "<div>Block: " + b.ControlName + "</div>"};
                }

                if (viewControl is HccContentBlockPart || viewControl is BlankBlock)
                {
                    if (viewControl is HccContentBlockPart)
                    {
                        ((HccContentBlockPart) viewControl).BlockId = b.Bvin;
                    }
                    controlFound = true;
                    cell.Controls.Add(viewControl);
                }

                if (controlFound)
                {
                    // Check for Editor
                    var lnkEdit = (HyperLink) e.Row.FindControl("lnkEdit");
                    if (lnkEdit != null)
                    {
                        lnkEdit.Visible = EditorExists(b.ControlName);
                    }
                }
                else
                {
                    cell.Controls.Add(new LiteralControl("Control " + b.ControlName + "could not be located"));
                }
            }
        }

        protected void gvBlocks_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var bvin = e.Keys[0].ToString();

            HccApp.ContentServices.Columns.DeleteBlock(bvin);
            LoadColumn();
        }

        #endregion

        #region Implementation

        private void LoadBlockList()
        {
            lstBlocks.DataSource = HccPartController.FindContentBlocks();
            lstBlocks.DataBind();
        }

        public void LoadColumn()
        {
            var c = HccApp.ContentServices.Columns.Find(ColumnId);
            lblTitle.Text = c.DisplayName;
            gvBlocks.DataSource = c.Blocks;
            gvBlocks.DataBind();
        }

        private bool EditorExists(string controlName)
        {
            var result = false;
            Control editorControl;
            editorControl = HccPartController.LoadContentBlockEditor(controlName.Replace(" ", string.Empty), Page);
            if (editorControl != null)
            {
                if (editorControl is HccPart)
                {
                    result = true;
                }
            }
            return result;
        }

        #endregion
    }
}