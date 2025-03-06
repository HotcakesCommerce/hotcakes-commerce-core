#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class Products_Edit_Roles : BaseProductPage
    {
        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ProductRoles");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnAdd.Click += btnAdd_Click;
            gvRoles.RowDeleting += gvRoles_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadRoles();
            }
        }

        private void gvRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var roleId = (long) e.Keys[0];
            HccApp.CatalogServices.CatalogRoles.Delete(roleId);
            LoadRoles();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var role = new CatalogRole
            {
                RoleName = ddlRoles.SelectedValue,
                ReferenceId = new Guid(ProductId),
                RoleType = CatalogRoleType.ProductRole
            };

            HccApp.CatalogServices.CatalogRoles.Create(role);
            LoadRoles();
        }

        #endregion

        #region Implementation

        private void LoadRoles()
        {
            var selRoles = HccApp.CatalogServices.CatalogRoles.FindByProductId(new Guid(ProductId));

            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleName";
            ddlRoles.DataSource =
                DnnUserController.Instance.GetRoles().Where(r => !selRoles.Any(sr => sr.RoleName == r.RoleName));
            ddlRoles.DataBind();

            if (selRoles.Count == 0)
            {
                DisplayInheritedRoles();
            }

            LocalizationUtils.LocalizeGridView(gvRoles, Localization);

            gvRoles.DataSource = selRoles;
            gvRoles.DataBind();
        }

        private void DisplayInheritedRoles()
        {
            gvRoles.EmptyDataText = Localization.GetString("NoRolesAdded");

            var product = HccApp.CatalogServices.Products.Find(ProductId);
            var roles = HccApp.CatalogServices.FindActualProductRoles(product);

            if (roles.Count == 0)
            {
                gvRoles.EmptyDataText += Localization.GetString("ProductIsPublic");
            }
            else
            {
                var rolesStr = string.Join(", ", roles.Select(r => r.RoleName));
                gvRoles.EmptyDataText += string.Format(Localization.GetString("InheritedRolesUsed"), rolesStr);
            }
        }

        #endregion
    }
}