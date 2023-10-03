#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class Columns_Edit : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                PopulateAdvancedOptions();
                if (Request.QueryString["id"] != null)
                {
                    ContentColumnEditor.ColumnId = Request.QueryString["id"];
                }
            }
        }

        private void PopulateAdvancedOptions()
        {
            CopyToList.DataSource = HccApp.ContentServices.Columns.FindAll();
            CopyToList.DataTextField = "DisplayName";
            CopyToList.DataValueField = "bvin";
            CopyToList.DataBind();
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Response.Redirect("Columns.aspx");
        }

        protected void btnCopyBlocks_Click(object sender, EventArgs e)
        {
            msg.ClearMessage();

            var c = HccApp.ContentServices.Columns.Find(ContentColumnEditor.ColumnId);
            if (c != null)
            {
                var destinationColumnId = CopyToList.SelectedValue;
                foreach (var b in c.Blocks)
                {
                    HccApp.ContentServices.Columns.CopyBlockToColumn(b.Bvin, destinationColumnId);
                }
                msg.ShowOk(Localization.GetString("CopySuccess"));
            }
            else
            {
                msg.ShowError(Localization.GetString("CopyFailure"));
            }
            ContentColumnEditor.LoadColumn();
        }

        protected void btnClone_Click(object sender, EventArgs e)
        {
            msg.ClearMessage();

            if (string.IsNullOrEmpty(CloneNameField.Text.Trim()))
            {
                msg.ShowWarning(Localization.GetString("CloneNameEmpty"));
            }
            else
            {
                var clone = HccApp.ContentServices.Columns.Clone(ContentColumnEditor.ColumnId,
                    CloneNameField.Text.Trim());
                msg.ShowOk(Localization.GetString("CopySuccess"));
            }
            ContentColumnEditor.LoadColumn();
        }
    }
}