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
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class Categories_Roles : BaseCategoryPage
    {
        #region Implementation

        private void LoadRoles()
        {
            var selRoles = HccApp.CatalogServices.CatalogRoles.FindByCategoryId(new Guid(CategoryId));

            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleName";
            ddlRoles.DataSource =
                DnnUserController.Instance.GetRoles()
                    .Where(r => !selRoles.Any(sr => sr.RoleName == r.RoleName))
                    .OrderBy(y => y.RoleName);
            ddlRoles.DataBind();

            gvRoles.DataSource = selRoles.OrderBy(y => y.RoleName);
            gvRoles.DataBind();

            btnAdd.Enabled = ddlRoles.Items.Count > 0;
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnAdd.Click += btnAdd_Click;
            gvRoles.RowDeleting += gvRoles_RowDeleting;

            PageTitle = Localization.GetString("PageTitle");
            InitNavMenu(ucNavMenu);
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
            if (!string.IsNullOrEmpty(ddlRoles.SelectedValue))
            {
                var role = new CatalogRole
                {
                    RoleName = ddlRoles.SelectedValue,
                    ReferenceId = new Guid(CategoryId),
                    RoleType = CatalogRoleType.CategoryRole
                };
                HccApp.CatalogServices.CatalogRoles.Create(role);
            }
            LoadRoles();
        }

        #endregion
    }
}