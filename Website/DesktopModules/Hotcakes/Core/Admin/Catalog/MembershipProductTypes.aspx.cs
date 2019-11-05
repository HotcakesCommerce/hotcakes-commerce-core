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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Telerik.Web.UI;

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
            PageTitle = "Membership Product Types";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);

            rgProductTypes.NeedDataSource += rgProductTypes_NeedDataSource;
            rgProductTypes.DeleteCommand += rgProductTypes_DeleteCommand;
            rgProductTypes.EditCommand += rgProductTypes_EditCommand;
            btnCreate.Click += btnCreate_Click;
            ucMembershipTypeEdit.SaveData += ucMembershipTypeEdit_SaveData;
            ucMembershipTypeEdit.CancelButton.Click += CancelButton_Click;

            LocalizationUtils.LocalizeRadGrid(rgProductTypes, Localization);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            ucMembershipTypeEdit.Model = new MembershipProductType();
            ucMembershipTypeEdit.DataBind();
            ShowEditor(true);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            rgProductTypes.EditIndexes.Clear();
            rgProductTypes.Rebind();
            ShowEditor(false);
        }

        private void rgProductTypes_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            rgProductTypes.DataSource = _repository.GetList(HccApp.CurrentStore.Id);
        }

        private void rgProductTypes_EditCommand(object sender, GridCommandEventArgs e)
        {
            e.Canceled = true;
            e.Item.Edit = false;
            e.Item.Selected = false;

            var productTypeId = (string) (e.Item as GridDataItem).GetDataKeyValue("ProductTypeId");
            ucMembershipTypeEdit.Model = _repository.Find(productTypeId);
            ucMembershipTypeEdit.DataBind();
            ShowEditor(true);
        }

        private void rgProductTypes_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var productTypeId = (string) (e.Item as GridDataItem).GetDataKeyValue("ProductTypeId");
            _repository.Delete(productTypeId);
            rgProductTypes.Rebind();
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

            rgProductTypes.EditIndexes.Clear();
            rgProductTypes.Rebind();
            ShowEditor(false);
        }

        #endregion
    }
}