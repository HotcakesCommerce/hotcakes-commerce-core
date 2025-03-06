#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class Columns_EditBlock : BaseAdminPage
    {
        protected ContentBlock _block;

        protected string BlockId
        {
            get { return Request.QueryString["id"]; }
        }

        protected ContentBlock Block
        {
            get
            {
                if (_block == null)
                    _block = HccApp.ContentServices.Columns.FindBlock(BlockId);
                return _block;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadEditor();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                PopulateAdvancedOptions();
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Edit Content Block";
            CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        private void PopulateAdvancedOptions()
        {
            var columns = HccApp.ContentServices.Columns.FindAll();

            CopyToList.DataSource = columns;
            CopyToList.DataTextField = "DisplayName";
            CopyToList.DataValueField = "bvin";
            CopyToList.DataBind();

            MoveToList.DataSource = columns;
            MoveToList.DataTextField = "DisplayName";
            MoveToList.DataValueField = "bvin";
            MoveToList.DataBind();
        }

        private void LoadEditor()
        {
            var editor =
                HccPartController.LoadContentBlockEditor(Block.ControlName.Replace(" ", string.Empty), this) as
                    HccContentBlockPart;

            if (editor != null)
            {
                editor.BlockId = Block.Bvin;
                TitleLabel.Text = "Edit Content Block - " + Block.ControlName;
                phEditor.Controls.Add(editor);
            }
            else
            {
                msg.ShowError("Error, editor is not based on HccContentBlockPart class");
            }

            editor.EditingComplete += editor_EditingComplete;
        }

        protected void editor_EditingComplete(object sender, HccPartEventArgs e)
        {
            Response.Redirect("Columns_Edit.aspx?id=" + Block.ColumnId);
        }

        protected void btnGoCopy_Click(object sender, EventArgs e)
        {
            if (HccApp.ContentServices.Columns.CopyBlockToColumn(Block.Bvin, CopyToList.SelectedValue))
            {
                msg.ShowOk("Block Copied");
            }
            else
            {
                msg.ShowError("Copy failed. Unknown Error.");
            }
        }

        protected void btnGoMove_Click(object sender, EventArgs e)
        {
            if (HccApp.ContentServices.Columns.MoveBlockToColumn(Block.Bvin, MoveToList.SelectedValue))
            {
                msg.ShowOk("Block Moved");
            }
            else
            {
                msg.ShowError("Move failed. Unknown Error.");
            }
        }
    }
}