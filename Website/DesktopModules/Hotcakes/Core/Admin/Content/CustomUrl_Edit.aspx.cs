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
using System.Web.UI;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class CustomUrl_Edit : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Edit Custom Url";
            CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                RequestedUrlField.Focus();

                if (Request.QueryString["id"] != null)
                {
                    BvinField.Value = Request.QueryString["id"];
                    LoadUrl();
                    LoadPreview();
                }
                else
                {
                    BvinField.Value = string.Empty;
                }
            }
        }

        private void LoadUrl()
        {
            Commerce.Content.CustomUrl c;
            c = HccApp.ContentServices.CustomUrls.Find(BvinField.Value);
            if (c != null)
            {
                if (c.Bvin != string.Empty)
                {
                    RequestedUrlField.Text = c.RequestedUrl;
                    RedirectToUrlField.Text = c.RedirectToUrl;
                    chkPermanent.Checked = c.IsPermanentRedirect;
                }
            }
        }

        protected void btnSaveChanges_Click(object sender, ImageClickEventArgs e)
        {
            if (Save())
            {
                Response.Redirect("CustomUrl.aspx");
            }
        }

        protected void btnCancel_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("CustomUrl.aspx");
        }

        private bool Save()
        {
            var result = false;

            if (UrlRewriter.IsUrlInUse(RequestedUrlField.Text.Trim(), BvinField.Value, HccApp.CurrentRequestContext,
                HccApp))
            {
                MessageBox1.ShowWarning("Another item already uses this URL. Please choose another one");
                return false;
            }

            Commerce.Content.CustomUrl c;
            c = HccApp.ContentServices.CustomUrls.Find(BvinField.Value);
            if (c == null) c = new Commerce.Content.CustomUrl();
            if (c != null)
            {
                c.RequestedUrl = RequestedUrlField.Text.Trim();
                c.RedirectToUrl = RedirectToUrlField.Text.Trim();
                c.IsPermanentRedirect = chkPermanent.Checked;

                if (BvinField.Value == string.Empty)
                {
                    result = HccApp.ContentServices.CustomUrls.Create(c);
                }
                else
                {
                    result = HccApp.ContentServices.CustomUrls.Update(c);
                }

                if (result)
                {
                    // Update bvin field so that next save will call updated instead of create
                    BvinField.Value = c.Bvin;
                }
            }

            LoadPreview();
            return result;
        }

        protected void btnUpdate_Click(object sender, ImageClickEventArgs e)
        {
            if (Save())
            {
                MessageBox1.ShowOk("Changes saved at " + DateTime.Now);
            }
            else
            {
                MessageBox1.ShowWarning("Unable to save! Unknown error.");
            }
        }

        private void LoadPreview()
        {
            Commerce.Content.CustomUrl c;
            c = HccApp.ContentServices.CustomUrls.Find(BvinField.Value);
            if (c != null)
            {
                if (c.Bvin != string.Empty)
                {
                    var appUrl = HccApp.StoreUrl(false, false);
                    litPreviewFrom.Text = appUrl + c.RequestedUrl.TrimStart('/');
                    litPreviewTo.Text = appUrl + c.RedirectToUrl.TrimStart('/');
                }
            }
        }
    }
}