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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.Areas.ContentBlocks.Models;

namespace Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ImageRotator
{
    public partial class Editor : HccContentBlockPart
    {
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            lnkAddImage.Click += lnkAddImage_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);

            RegisterWindowScripts();

            if (!IsPostBack)
            {
                LoadItems(b);
                chkShowInOrder.Checked = b.BaseSettings.GetBoolSetting("ShowInOrder");
                cssclass.Text = b.BaseSettings.GetSettingOrEmpty("cssclass");

                WidthField.Text = b.BaseSettings.GetIntegerSetting("Width", 220).ToString();
                HeighField.Text = b.BaseSettings.GetIntegerSetting("Height", 220).ToString();
                PauseField.Text = b.BaseSettings.GetIntegerSetting("Pause", 3).ToString();
            }
        }

        private void lnkAddImage_Click(object sender, EventArgs e)
        {
            ShowImageDialog();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CloseImageDialog();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            var c = new ContentBlockSettingListItem();

            if (EditBvinField.Value != string.Empty)
            {
                //Updating
                c = b.Lists.FindSingleItem(EditBvinField.Value);
                c.Setting1 = ImageUrlField.Text.Trim();
                c.Setting2 = ImageLinkField.Text.Trim();
                if (chkOpenInNewWindow.Checked)
                {
                    c.Setting3 = "1";
                }
                else
                {
                    c.Setting3 = "0";
                }
                c.Setting4 = AltTextField.Text.Trim();
                HccApp.ContentServices.Columns.UpdateBlock(b);
            }
            else
            {
                //inserting
                c.Setting1 = ImageUrlField.Text.Trim();
                c.Setting2 = ImageLinkField.Text.Trim();
                if (chkOpenInNewWindow.Checked)
                {
                    c.Setting3 = "1";
                }
                else
                {
                    c.Setting3 = "0";
                }
                c.Setting4 = AltTextField.Text.Trim();
                c.ListName = "Images";
                b.Lists.AddItem(c);
                HccApp.ContentServices.Columns.UpdateBlock(b);
            }
            LoadItems(b);
            CloseImageDialog();
        }

        protected void btnOkay_Click(object sender, EventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);

            b.BaseSettings["cssclass"] = cssclass.Text.Trim();
            b.BaseSettings.SetBoolSetting("ShowInOrder", chkShowInOrder.Checked);

            var width = WidthField.Text.ConvertTo(220);
            b.BaseSettings.SetIntegerSetting("Width", width);

            var height = HeighField.Text.ConvertTo(220);
            b.BaseSettings.SetIntegerSetting("Height", height);

            var pause = PauseField.Text.ConvertTo(0);
            b.BaseSettings.SetIntegerSetting("Pause", pause);

            HccApp.ContentServices.Columns.UpdateBlock(b);

            NotifyFinishedEditing();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void gvImages_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);

            var bvin = string.Empty;
            bvin = ((GridView) sender).DataKeys[e.NewEditIndex].Value.ToString();

            var c = new ContentBlockSettingListItem();
            c = b.Lists.FindSingleItem(bvin);

            if (c.Id != string.Empty)
            {
                EditBvinField.Value = c.Id;
                ImageLinkField.Text = c.Setting2;
                ImageUrlField.Text = c.Setting1;
                if (c.Setting3 == "1")
                {
                    chkOpenInNewWindow.Checked = true;
                }
                else
                {
                    chkOpenInNewWindow.Checked = false;
                }
                AltTextField.Text = c.Setting4;

                ShowImageDialog();
            }
        }

        protected void gvImages_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);

            var bvin = string.Empty;
            bvin = ((GridView) sender).DataKeys[e.RowIndex].Value.ToString();
            b.Lists.RemoveItem(bvin);
            HccApp.ContentServices.Columns.UpdateBlock(b);
            LoadItems(b);
        }

        protected void gvImages_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var c = (ContentBlockSettingListItem) e.Row.DataItem;
                if (c == null) return;

                e.Row.Attributes["id"] = c.Id;
                var aPreview = e.Row.FindControl("aPreview") as HtmlAnchor;
                var imgPreview = e.Row.FindControl("imgPreview") as HtmlImage;

                var img = new ImageRotatorImageViewModel();
                img.ImageUrl = ResolveSpecialUrl(c.Setting1);
                img.Url = c.Setting2;
                if (img.Url.StartsWith("~"))
                {
                    img.Url = ResolveUrl(img.Url);
                }
                img.NewWindow = c.Setting3 == "1";
                img.Caption = c.Setting4;

                aPreview.HRef = img.Url;
                if (img.NewWindow)
                    aPreview.Target = "_blank";

                imgPreview.Src = img.ImageUrl;
                imgPreview.Alt = HttpUtility.HtmlEncode(img.Caption);
            }
        }

        #endregion

        #region Implementation

        private void ShowImageDialog()
        {
            pnlEditImageDialog.Visible = true;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcShowEditImageDialog",
                "hcShowEditImageDialog();", true);
        }

        private void CloseImageDialog()
        {
            pnlEditImageDialog.Visible = false;
            ClearEditor();
        }

        private void LoadItems(ContentBlock b)
        {
            gvImages.DataSource = b.Lists.FindList("Images");
            gvImages.DataBind();
        }

        private void ClearEditor()
        {
            EditBvinField.Value = string.Empty;
            ImageUrlField.Text = string.Empty;
            ImageLinkField.Text = string.Empty;
            chkOpenInNewWindow.Checked = false;
            AltTextField.Text = string.Empty;
        }

        private void RegisterWindowScripts()
        {
            var sb = new StringBuilder();

            sb.Append("var w;");
            sb.Append("function popUpWindow(parameters) {");
            sb.Append("w = window.open('" + ResolveUrl("~/DesktopModules/Hotcakes/API/mvc/filebrowser") +
                      "' + parameters, null, 'height=480, width=640');");
            sb.Append("}");

            sb.Append("function closePopup() {");
            sb.Append("w.close();");
            sb.Append("}");

            sb.Append("function SetImage(fileName) {");
            sb.Append("document.getElementById('");
            sb.Append(ImageUrlField.ClientID);
            sb.Append("').value = fileName;");
            sb.Append("w.close();");
            sb.Append("}");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "WindowScripts", sb.ToString(), true);
        }

        private string ResolveSpecialUrl(string raw)
        {
            // full url
            var tester = raw.Trim().ToLowerInvariant();
            if (tester.StartsWith("http:") || tester.StartsWith("https:")
                || tester.StartsWith("//")) return raw;

            // tag replaced url {{img}} or {{assets}
            if (tester.StartsWith("{{"))
            {
                return TagReplacer.ReplaceContentTags(raw, HccApp);
            }

            // app relative url
            if (tester.StartsWith("~"))
            {
                return ResolveUrl(raw);
            }

            // old style asset
            return DiskStorage.StoreUrl(
                HccApp,
                raw,
                HccApp.IsCurrentRequestSecure());
        }

        #endregion
    }
}