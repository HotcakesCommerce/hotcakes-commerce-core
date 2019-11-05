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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class FileVault : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                BindFileGridView();
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "File Vault";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected void BindFileGridView()
        {
            var files = HccApp.CatalogServices.ProductFiles.FindAll(1, int.MaxValue);
            RenderFiles(files);
        }

        private void RenderFiles(List<ProductFile> files)
        {
            var sb = new StringBuilder();

            foreach (var f in files)
            {
                var productCount = HccApp.CatalogServices.ProductFiles.CountOfProductsUsingFile(f.Bvin);
                sb.Append("<tr class=\"hcGridRow\">");
                sb.Append("<td>" + HttpUtility.HtmlEncode(f.FileName) + "</td>");
                sb.Append("<td>" + HttpUtility.HtmlEncode(f.ShortDescription) + "</td>");
                sb.Append("<td>" + productCount + "</td>");
                sb.Append("<td><a href=\"FileVaultDetailsView.aspx?id=" + HttpUtility.UrlEncode(f.Bvin) +
                          "\" class=\"hcIconEdit\" alt=\"Edit\"></td>");
                sb.Append("<td><a href=\"FileVaultDelete.aspx?id=" + HttpUtility.UrlEncode(f.Bvin) +
                          "\" onclick=\"return window.confirm('Delete this file?');\" class=\"hcIconDelete\">Delete</a></td>");
                sb.Append("</tr>");
            }

            litFiles.Text = sb.ToString();
        }

        protected void ImportLinkButton_Click(object sender, EventArgs e)
        {
            var importDir = Path.Combine(Server.MapPath(Request.ApplicationPath), "Files");
            if (Directory.Exists(importDir))
            {
                var files = Directory.GetFiles(importDir);
                var errorOccurred = false;
                foreach (var fileName in files)
                {
                    if (!fileName.ToLower().EndsWith(".config"))
                    {
                        var ReadWasSuccess = false;

                        var file = new ProductFile();
                        file.FileName = Path.GetFileName(fileName);
                        file.ShortDescription = file.FileName;
                        if (HccApp.CatalogServices.ProductFiles.Create(file))
                        {
                            try
                            {
                                using (var fileStream = new FileStream(fileName, FileMode.Open))
                                {
                                    if (ProductFile.SaveFile(HccApp.CurrentStore.Id, file.Bvin, file.FileName,
                                        fileStream))
                                    {
                                        ReadWasSuccess = true;
                                    }
                                }

                                if (ReadWasSuccess)
                                {
                                    File.Delete(fileName);
                                }
                            }
                            catch (Exception ex)
                            {
                                errorOccurred = true;
                                EventLog.LogEvent(ex);
                            }
                        }
                    }
                }
                if (errorOccurred)
                {
                    MessageBox1.ShowError("An error occurred during import. Please check the event log.");
                }
                else
                {
                    MessageBox1.ShowOk("Files Imported Successfully.");
                }
            }
            else
            {
                MessageBox1.ShowInformation("Files folder doesn't exist");
            }

            BindFileGridView();
        }

        protected void AddFileButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                FilePicker.DownloadOrLinkFile(null, MessageBox1);
            }
            BindFileGridView();
        }
    }
}