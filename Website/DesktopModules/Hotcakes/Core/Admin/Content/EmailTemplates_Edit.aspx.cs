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
using System.Linq;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Content
{
    partial class EmailTemplates_Edit : BaseAdminPage
    {
        #region Properties

        protected long TemplateId
        {
            get
            {
                var strTemplId = (string) ViewState["TemplateId"] ?? Request.QueryString["id"];
                long tempId;

                if (long.TryParse(strTemplId, out tempId))
                {
                    return tempId;
                }

                return -1;
            }
            set { ViewState["TemplateId"] = value.ToString(); }
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("EditHtmlTemplate");
            CurrentTab = AdminTabType.Content;
            ValidateCurrentUserHasPermission(SystemPermissions.ContentView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadEmailTemplate();
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                Response.Redirect("EmailTemplates.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmailTemplates.aspx");
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            var templ = HccApp.ContentServices.HtmlTemplates.Find(TemplateId);
            if (templ != null)
            {
                HccApp.ContentServices.UpdateTemplateToDefault(templ);
                Response.Redirect("EmailTemplates.aspx");
            }
        }

        #endregion

        #region Implementation

        private void LoadEmailTemplate()
        {
            var templ = HccApp.ContentServices.HtmlTemplates.Find(TemplateId);

            if (templ != null)
            {
                txtFrom.Text = templ.From;
                txtDisplayName.Text = templ.DisplayName;
                txtSubject.Text = templ.Subject;
                txtBody.Text = templ.Body;
                txtRepeatingSection.Text = templ.RepeatingSection;

                var defTemplate = HccApp.ContentServices.HtmlTemplates.FindByStoreAndType(0, templ.TemplateType);
                if (defTemplate != null)
                {
                    lblTemplateType.Text = defTemplate.DisplayName;
                }
                else
                {
                    lblTemplateType.Text = Localization.GetString("CustomTemplate");
                }

                divRepeatingSection.Visible = templ.HasRepeatingSection();
                btnReset.Visible = templ.TemplateType != HtmlTemplateType.Custom;
            }
            else
            {
                templ = new HtmlTemplate {TemplateType = HtmlTemplateType.Custom};
                lblTemplateType.Text = Localization.GetString("CustomTemplate");
            }

            PopulateTags(templ);
        }

        private void PopulateTags(HtmlTemplate templ)
        {
            var emailTokens = templ.GetEmptyReplacementTags(HccApp.CurrentRequestContext);
            emailTokens = emailTokens.Where(p => p.IsObsolete == false).ToList();
            lstTags.DataSource = emailTokens;
            lstTags.DataValueField = "Tag";
            lstTags.DataTextField = "Tag";
            lstTags.DataBind();
        }

        private bool Save()
        {
            var result = false;

            var templ = HccApp.ContentServices.HtmlTemplates.Find(TemplateId);
            if (templ == null)
            {
                templ = new HtmlTemplate {TemplateType = HtmlTemplateType.Custom};
            }

            templ.Body = txtBody.Text.Trim();
            templ.DisplayName = txtDisplayName.Text.Trim();
            templ.From = txtFrom.Text.Trim();
            templ.RepeatingSection = txtRepeatingSection.Text.Trim();
            templ.Subject = txtSubject.Text.Trim();

            if (templ.Id == 0)
            {
                result = HccApp.ContentServices.HtmlTemplates.Create(templ);
                if (result)
                {
                    TemplateId = templ.Id;
                }
            }
            else
            {
                result = HccApp.ContentServices.HtmlTemplates.Update(templ);
            }

            return result;
        }

        #endregion
    }
}