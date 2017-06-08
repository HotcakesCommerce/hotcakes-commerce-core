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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class FileDownloads : BaseProductPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit File Downloads";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);

            if (!string.IsNullOrEmpty(ProductId))
            {
                FilePicker1.ProductId = ProductId;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeFileGrid();

            if (!Page.IsPostBack)
            {
                CurrentTab = AdminTabType.Catalog;
            }
        }

        protected void InitializeFileGrid()
        {
            FileGrid.DataSource = HccApp.CatalogServices.ProductFiles.FindByProductId(ProductId);
            FileGrid.DataBind();
        }

        protected void FileGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (
                !HccApp.CatalogServices.ProductFiles.RemoveAssociatedProduct(
                    (string) FileGrid.DataKeys[e.RowIndex].Value, ProductId))
            {
                MessageBox1.ShowError("An error occurred while trying to delete your file from the database.");
            }
            InitializeFileGrid();
        }

        protected void FileGrid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var primaryKey = (string) FileGrid.DataKeys[e.NewEditIndex].Value;
            var file = HccApp.CatalogServices.ProductFiles.Find(primaryKey);
            if (!ViewUtilities.DownloadFile(file, HccApp))
            {
                MessageBox1.ShowWarning("The file to download could not be found.");
            }
        }

        protected void AddFileButton_Click(object sender, ImageClickEventArgs e)
        {
            if (Page.IsValid)
            {
                FilePicker1.DownloadOrLinkFile(null, MessageBox1);
                InitializeFileGrid();
            }
        }
    }
}