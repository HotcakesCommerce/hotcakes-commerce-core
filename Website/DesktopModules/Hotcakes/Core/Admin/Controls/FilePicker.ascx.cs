#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class FilePicker : HccUserControl
    {
        private Mode _currentMode = Mode.NewUpload;

        private string _productId = string.Empty;

        public string ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        public bool DisplayShortDescription
        {
            get { return ShortDescriptionRow.Visible; }
            set { ShortDescriptionRow.Visible = value; }
        }

        protected void FileHasBeenSelectedCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;
            if (!NewFileUpload.HasFile)
            {
                if (FilesDropDownList.Visible)
                {
                    if (FilesDropDownList.SelectedIndex == 0)
                    {
                        args.IsValid = false;
                    }
                }
                else if (FileSelectedTextBox.Visible)
                {
                    if (FileSelectedTextBox.Text.Trim() == string.Empty)
                    {
                        args.IsValid = false;
                    }
                }
                else
                {
                    //if we got here, then somehow FilesDropDownList and FileSelectedTextBox are both not visible
                    args.IsValid = false;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RegisterWindowScripts();
            NewFileUpload.Attributes.Add("onchange", "FillFilename(this, ['" + ShortDescriptionTextBox.ClientID + "']);");
            FilesDropDownList.Attributes.Add("onchange",
                "FillFilename(this, ['" + ShortDescriptionTextBox.ClientID + "']);");

            if (!Page.IsPostBack)
            {
                InitializeFileLists();
            }

            if (NewFileUpload.HasFile)
            {
                _currentMode = Mode.NewUpload;
            }
            else if (FilesDropDownList.Visible)
            {
                _currentMode = Mode.DropDownList;
            }
            else if (FileSelectedTextBox.Visible)
            {
                _currentMode = Mode.FileBrowsed;
            }

            if (string.IsNullOrEmpty(ProductId))
            {
                AvailableMinutesRow.Visible = false;
                NumberDownloadsRow.Visible = false;
            }
            else
            {
                AvailableMinutesRow.Visible = true;
                NumberDownloadsRow.Visible = true;
            }
        }

        protected void InitializeFileLists()
        {
            // This was commented out because FileDownloadBrowser.aspx is missing in our codebase
            //if (HccApp.CatalogServices.ProductFiles.CountOfAll() > 30)
            //{
            //	FilesDropDownList.Visible = false;
            //	FileSelectedTextBox.Visible = true;
            //	browseButton.Visible = true;
            //}
            //else
            //{
            var files = HccApp.CatalogServices.ProductFiles.FindAll(1, 1000);
            FilesDropDownList.Visible = true;
            FileSelectedTextBox.Visible = false;
            browseButton.Visible = false;

            var item = FilesDropDownList.Items[0];
            FilesDropDownList.Items.Clear();
            FilesDropDownList.Items.Add(item);

            FilesDropDownList.DataSource = files;
            FilesDropDownList.DataTextField = "CombinedDisplay";
            FilesDropDownList.DataValueField = "Bvin";
            FilesDropDownList.DataBind();
            //}
        }

        private void RegisterWindowScripts()
        {
            var sb = new StringBuilder();

            sb.Append("var w;");
            sb.Append("function popUpWindow(parameters) {");
            sb.Append("w = window.open('FileDownloadBrowser.aspx' + parameters, null, 'height=480, width=640');");
            sb.Append("}");

            sb.Append("function closePopup(id, shortDescription, fileName) {");
            sb.Append("w.close();");
            if (FileSelectedTextBox.Visible)
            {
                sb.Append("document.getElementById('" + FileSelectedTextBox.ClientID + "').value = fileName;");
            }
            if (ShortDescriptionTextBox.Visible)
            {
                sb.Append("document.getElementById('" + ShortDescriptionTextBox.ClientID +
                          "').value = shortDescription;");
            }
            sb.Append("document.getElementById('" + FileIdHiddenField.ClientID + "').value = id;");
            sb.Append("}");

            sb.Append("String.prototype.trim = function() {");
            sb.Append("    a = this.replace(/^\\s+/, '');");
            sb.Append("    return a.replace(/\\s+$/, '');");
            sb.Append("};");

            sb.Append("function FillFilename(control, FieldsToFill){");
            sb.Append("    if (control.type == \"select-one\"){");
            sb.Append("        if (control.selectedIndex != 0){");
            sb.Append("            for(i = 0; i < FieldsToFill.length; i++){");
            sb.Append(
                "                document.getElementById(FieldsToFill[i]).value = control.options[control.selectedIndex].text.split(\"[\")[0].trim();");
            sb.Append("            }");
            sb.Append("        } else {");
            sb.Append("            for(i = 0; i < FieldsToFill.length; i++){");
            sb.Append("                document.getElementById(FieldsToFill[i]).value = \"\";");
            sb.Append("            }");
            sb.Append("        }");
            sb.Append("    } else {");
            sb.Append("        var arr = control.value.split(\"\\\\\");");
            sb.Append("        for(i = 0; i < FieldsToFill.length; i++){");
            sb.Append("            document.getElementById(FieldsToFill[i]).value = arr[arr.length - 1];");
            sb.Append("        }");
            sb.Append("    }");
            sb.Append("}");

            Page.ClientScript.RegisterClientScriptBlock(typeof (Page), "WindowScripts", sb.ToString(), true);
        }

        protected void DescriptionIsUniqueToProductCustomValidator_ServerValidate(object source,
            ServerValidateEventArgs args)
        {
            args.IsValid = true;
            ProductFile file = null;
            if (_currentMode == Mode.DropDownList)
            {
                //file = Catalog.ProductFile.FindByBvin(FilesDropDownList.SelectedValue);
            }
            else if (_currentMode == Mode.FileBrowsed)
            {
                //file = Catalog.ProductFile.FindByBvin(FileIdHiddenField.Value);
            }
            else if (_currentMode == Mode.NewUpload)
            {
                file = new ProductFile();
            }
            if (file != null)
            {
                InitializeProductFile(file, false);
                var result = HccApp.CatalogServices.ProductFiles.FindByFileNameAndDescription(file.FileName,
                    ShortDescriptionTextBox.Text);
                if (result.Count > 0)
                {
                    args.IsValid = false;
                }
            }
        }

        protected void InitializeProductFile(ProductFile file, bool updateFileName)
        {
            if (updateFileName)
            {
                ProductFile otherFile = null;
                if (_currentMode == Mode.DropDownList)
                {
                    otherFile = HccApp.CatalogServices.ProductFiles.Find(FilesDropDownList.SelectedValue);
                    file.StoreId = otherFile.StoreId;
                    file.Bvin = otherFile.Bvin;
                    file.FileName = otherFile.FileName;
                }
                else if (_currentMode == Mode.FileBrowsed)
                {
                    otherFile = HccApp.CatalogServices.ProductFiles.Find(FileIdHiddenField.Value);
                    file.StoreId = otherFile.StoreId;
                    file.Bvin = otherFile.Bvin;
                    file.FileName = otherFile.FileName;
                }
                else if (_currentMode == Mode.NewUpload)
                {
                    file.FileName = Path.GetFileName(NewFileUpload.FileName);
                }
            }
            else
            {
                if (_currentMode == Mode.NewUpload)
                {
                    file.FileName = Path.GetFileName(NewFileUpload.FileName);
                }
            }

            if (DisplayShortDescription)
            {
                file.ShortDescription = ShortDescriptionTextBox.Text.Trim();
            }

            if (ProductId != string.Empty)
            {
                file.ProductId = ProductId;
                file.SetMinutes(AvailableForTimespanPicker.Months, AvailableForTimespanPicker.Days,
                    AvailableForTimespanPicker.Hours, AvailableForTimespanPicker.Minutes);
                if (NumberOfDownloadsTextBox.Text.Trim() != string.Empty)
                {
                    file.MaxDownloads = int.Parse(NumberOfDownloadsTextBox.Text);
                }
                else
                {
                    file.MaxDownloads = 0;
                }
            }
        }

        public void Clear()
        {
            ShortDescriptionTextBox.Text = string.Empty;
            AvailableForTimespanPicker.Months = 0;
            AvailableForTimespanPicker.Days = 0;
            AvailableForTimespanPicker.Hours = 0;
            AvailableForTimespanPicker.Minutes = 0;
            NumberOfDownloadsTextBox.Text = string.Empty;
        }

        protected void FileIsUniqueToProductCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var files = HccApp.CatalogServices.ProductFiles.FindByProductId(ProductId);
            args.IsValid = true;
            foreach (var file in files)
            {
                if (_currentMode == Mode.DropDownList)
                {
                    if (file.Bvin == FilesDropDownList.SelectedValue)
                    {
                        args.IsValid = false;
                    }
                }
                else if (_currentMode == Mode.FileBrowsed)
                {
                    if (file.Bvin == FileIdHiddenField.Value)
                    {
                        args.IsValid = false;
                    }
                }
            }
        }

        public void DownloadOrLinkFile(ProductFile file, IMessageBox MessageBox1)
        {
            //if they browsed a file then this overrides all other behavior
            if (_currentMode == Mode.NewUpload)
            {
                if (file == null)
                {
                    file = new ProductFile();
                }
                InitializeProductFile(file, true);

                if (Save(file))
                {
                    if (ProductFile.SaveFile(HccApp.CurrentStore.Id, file.Bvin, file.FileName, NewFileUpload.PostedFile))
                    {
                        MessageBox1.ShowOk(Localization.GetString("FileSaveSuccess"));
                    }
                    else
                    {
                        MessageBox1.ShowError(Localization.GetString("FileSaveFailure"));
                    }
                }
                else
                {
                    MessageBox1.ShowError(Localization.GetString("FileSaveFailure"));
                }
            }

            else if (_currentMode == Mode.DropDownList)
            {
                if (!string.IsNullOrWhiteSpace(FilesDropDownList.SelectedValue))
                {
                    if (file == null)
                    {
                        file = new ProductFile();
                    }
                    InitializeProductFile(file, true);

                    if (Save(file))
                    {
                        MessageBox1.ShowOk(Localization.GetString("FileSaveSuccess"));
                    }
                    else
                    {
                        MessageBox1.ShowError(Localization.GetString("FileSaveFailure"));
                    }
                }
            }
            else if (_currentMode == Mode.FileBrowsed)
            {
                if (file == null)
                {
                    file = new ProductFile();
                }
                InitializeProductFile(file, true);
                if (Save(file))
                {
                    MessageBox1.ShowOk(Localization.GetString("FileSaveSuccess"));
                }
                else
                {
                    MessageBox1.ShowError(Localization.GetString("FileSaveFailure"));
                }
            }
            InitializeFileLists();
        }

        private bool Save(ProductFile f)
        {
            if (string.IsNullOrEmpty(f.Bvin))
            {
                return HccApp.CatalogServices.ProductFiles.Create(f);
            }
            return HccApp.CatalogServices.ProductFiles.Update(f);
        }

        private enum Mode
        {
            NewUpload = 1,
            DropDownList = 2,
            FileBrowsed = 3
        }
    }
}