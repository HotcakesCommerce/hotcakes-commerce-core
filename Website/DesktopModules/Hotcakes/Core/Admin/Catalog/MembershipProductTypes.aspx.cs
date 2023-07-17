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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class MembershipProductTypes : BaseAdminPage
    {
        #region Fields

        private MembershipProductTypeRepository _repository;

        #endregion

        #region Implementation

        private void ShowEditor(bool show)
        {
            pnlEditMembership.Visible = show;

            if (show)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "hcEditMembershipDialog",
                    "hcEditMembershipDialog();", true);
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _repository = HccApp.CatalogServices.MembershipTypes;
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);

            btnCreate.Click += btnCreate_Click;
            ucMembershipTypeEdit.SaveData += ucMembershipTypeEdit_SaveData;
            ucMembershipTypeEdit.CancelButton.Click += CancelButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LocalizeView();
                BindGrid();
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            ucMembershipTypeEdit.Model = new MembershipProductType();
            ucMembershipTypeEdit.DataBind();
            ShowEditor(true);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            BindGrid();
            ShowEditor(false);
        }

        private void LocalizeView()
        {
            var localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
            LocalizationUtils.LocalizeGridView(rgProductTypes, localization);
        }

        private void BindGrid()
        {
            var items = _repository.GetList(HccApp.CurrentStore.Id);
            if (items != null && items.Count > 0)
            {
                rgProductTypes.DataSource = items;
                rgProductTypes.DataBind();
            }
            else
            {
                msg.ShowWarning(Localization.GetString("NoProductTypes"));
            }
        }

        protected void rgProductTypes_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            var productTypeId = (string) rgProductTypes.DataKeys[e.NewEditIndex]["ProductTypeId"];

            ucMembershipTypeEdit.Model = _repository.Find(productTypeId);
            ucMembershipTypeEdit.DataBind();

            ShowEditor(true);
        }

        protected void rgProductTypes_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var productTypeId = (string)rgProductTypes.DataKeys[e.RowIndex]["ProductTypeId"];
            _repository.Delete(productTypeId);
            BindGrid();
            ShowEditor(false);
        }

        protected void ucMembershipTypeEdit_SaveData(object sender, EventArgs e)
        {
            var model = ucMembershipTypeEdit.Model;

            if (string.IsNullOrEmpty(model.ProductTypeId))
            {
                _repository.Create(model);
            }
            else
            {
                _repository.Update(model);
            }

            BindGrid();

            ShowEditor(false);
        }

        #endregion
    }
}