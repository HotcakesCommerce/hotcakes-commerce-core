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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    public partial class RoleAdministration : BaseAdminPage
    {
        #region Event handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            Title = Localization.GetString("RoleAdministration");
            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnSave.Click += btnSave_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRoles();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var roleSett = HccApp.CurrentStore.Settings.AdminRoles;
                roleSett.RoleCatalogManagement = ddlCatalogManagement.SelectedValue;
                roleSett.RoleOrdersAndCustomers = ddlOrdersManagement.SelectedValue;
                roleSett.RoleStoreAdministration = ddlStoreAdministration.SelectedValue;
                roleSett.RoleMobileAccess = ddlMobileAccess.SelectedValue;

                HccApp.UpdateCurrentStore();

                msg.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
        }

        #endregion

        #region Implementation

        private void BindRoles()
        {
            var roles = RoleController.Instance
                .GetRoles(PortalSettings.Current.PortalId)
                .OfType<RoleInfo>()
                .ToList();

            var roleSett = HccApp.CurrentStore.Settings.AdminRoles;
            var adminRole = PortalSettings.Current.AdministratorRoleName;

            BindRole(ddlCatalogManagement, roles, roleSett.RoleCatalogManagement ?? adminRole);
            BindRole(ddlOrdersManagement, roles, roleSett.RoleOrdersAndCustomers ?? adminRole);
            BindRole(ddlStoreAdministration, roles, roleSett.RoleStoreAdministration ?? adminRole);
            BindRole(ddlMobileAccess, roles, roleSett.RoleMobileAccess ?? adminRole);
        }

        private void BindRole(DropDownList ddl, List<RoleInfo> roles, string selRoleName)
        {
            ddl.DataSource = roles;
            ddl.DataTextField = "RoleName";
            ddl.DataValueField = "RoleName";
            ddl.DataBind();

            ddl.SelectedValue = selRoleName;
        }

        #endregion
    }
}