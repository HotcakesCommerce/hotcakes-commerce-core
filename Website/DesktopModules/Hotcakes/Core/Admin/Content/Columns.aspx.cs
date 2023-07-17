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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class Columns : BaseAdminPage
    {
        protected int RowCount { get; set; }

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
                LocalizeView();
                LoadColumns();
            }
        }

        private void LocalizeView()
        {
            var localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
            LocalizationUtils.LocalizeGridView(gvBlocks, localization);
        }

        private void LoadColumns()
        {
            var cols = HccApp.ContentServices.Columns.FindAll().OrderBy(c => c.SystemColumn);
            gvBlocks.DataSource = cols;
            gvBlocks.DataBind();
            RowCount = cols.Count();
        }

        protected void gvBlocks_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            msg.ClearMessage();

            var bvin = (string) gvBlocks.DataKeys[e.RowIndex].Value;
            if (HccApp.ContentServices.Columns.Delete(bvin) == false)
            {
                msg.ShowWarning(Localization.GetString("SystemColumnError"));
            }

            LoadColumns();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            msg.ClearMessage();

            if (NewNameField.Text.Trim().Length < 1)
            {
                msg.ShowWarning(Localization.GetString("NoNameError"));
            }
            else
            {
                var c = new ContentColumn();
                c.DisplayName = NewNameField.Text.Trim();
                c.SystemColumn = false;
                if (HccApp.ContentServices.Columns.Create(c))
                {
                    Response.Redirect("Columns_Edit.aspx?id=" + c.Bvin);
                }
                else
                {
                    msg.ShowError(Localization.GetString("DeleteError"));
                    EventLog.LogEvent("Create Content column Button", "Unable to create content column",
                        EventLogSeverity.Error);
                }
            }
        }

        protected void gvBlocks_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var bvin = (string) gvBlocks.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("Columns_edit.aspx?id=" + bvin);
        }
    }
}