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
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductTypesEdit : BaseAdminPage
    {
        #region Properties

        public string TypeId
        {
            get { return Request.QueryString["id"] ?? string.Empty; }
        }

        #endregion

        protected void gvProperties_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var row = e.Row;
                var rw = (ProductProperty) e.Row.DataItem;
                row.Attributes["id"] = rw.Id.ToString();
            }
        }

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ProductTypesEdit");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvProperties.RowDeleting += gvProperties_RowDeleting;
            btnAddRole.Click += btnAddRole_Click;
            gvRoles.RowDeleting += gvRoles_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadType();
                LoadRoles();
            }
        }

        private void gvRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var roleId = (long) e.Keys[0];
            HccApp.CatalogServices.CatalogRoles.Delete(roleId);
            LoadRoles();
        }

        private void btnAddRole_Click(object sender, EventArgs e)
        {
            var role = new CatalogRole
            {
                RoleName = ddlRoles.SelectedValue,
                ReferenceId = new Guid(TypeId),
                RoleType = CatalogRoleType.ProductTypeRole
            };

            HccApp.CatalogServices.CatalogRoles.Create(role);
            LoadRoles();
        }

        private void gvProperties_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var propertyId = Convert.ToInt64(e.Keys[0]);
            HccApp.CatalogServices.ProductTypeRemoveProperty(TypeId, propertyId);
            LoadPropertyLists();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProductTypes.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var productType = HccApp.CatalogServices.ProductTypes.Find(TypeId);
                if (productType == null)
                {
                    msg.ShowError(Localization.GetString("ProductTypeNotFound"));
                    return;
                }

                productType.ProductTypeName = txtProductTypeNameField.Text;
                productType.TemplateName = ddlTemplateList.SelectedValue;

                if (HccApp.CatalogServices.ProductTypes.Update(productType))
                {
                    Response.Redirect("ProductTypes.aspx");
                }
                else
                {
                    msg.ShowError(Localization.GetString("SaveError"));
                }
            }
        }

        protected void btnAddProperty_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!string.IsNullOrEmpty(lstAvailableProperties.SelectedValue))
                {
                    var propertyId = long.Parse(lstAvailableProperties.SelectedValue);
                    HccApp.CatalogServices.ProductTypeAddProperty(TypeId, propertyId);
                }
                else
                {
                    msg.ShowError(Localization.GetString("NoPropertySelected"));
                }

                LoadPropertyLists();
            }
        }

        #endregion

        #region Implementation

        private void LoadType()
        {
            var prodType = HccApp.CatalogServices.ProductTypes.Find(TypeId);
            if (prodType != null)
            {
                txtProductTypeNameField.Text = prodType.ProductTypeName;

                PopulateTemplates();
                if (ddlTemplateList.Items.FindByValue(prodType.TemplateName) != null)
                {
                    ddlTemplateList.ClearSelection();
                    ddlTemplateList.Items.FindByValue(prodType.TemplateName).Selected = true;
                }

                LoadPropertyLists();

                if (HccApp.CatalogServices.Products.FindCountByProductType(prodType.Bvin) > 0)
                {
                    msg.ShowWarning(Localization.GetString("MultipleTypeWarning"));
                }
            }
            else
            {
                msg.ShowError(string.Format(Localization.GetString("LoadTypeError"), TypeId));
            }
        }

        private void LoadPropertyLists()
        {
            var selectedProperties = HccApp.CatalogServices.ProductPropertiesFindForType(TypeId);

            LocalizationUtils.LocalizeGridView(gvProperties, Localization);

            gvProperties.DataSource = selectedProperties;
            gvProperties.DataBind();

            lstAvailableProperties.DataSource = HccApp.CatalogServices.ProductPropertiesFindNotAssignedToType(TypeId);
            lstAvailableProperties.DataTextField = "PropertyName";
            lstAvailableProperties.DataValueField = "Id";
            lstAvailableProperties.DataBind();
        }

        private void PopulateTemplates()
        {
            ddlTemplateList.Items.Clear();
            ddlTemplateList.Items.Add(new ListItem(Localization.GetString("NotSet"), string.Empty));
            ddlTemplateList.AppendDataBoundItems = true;
            ddlTemplateList.DataSource = DnnPathHelper.GetViewNames("Products");
            ddlTemplateList.DataBind();
        }

        private void LoadRoles()
        {
            var selRoles = HccApp.CatalogServices.CatalogRoles.FindByProductTypeId(new Guid(TypeId));

            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleName";
            ddlRoles.DataSource =
                DnnUserController.Instance.GetRoles().Where(r => !selRoles.Any(sr => sr.RoleName == r.RoleName));
            ddlRoles.DataBind();

            LocalizationUtils.LocalizeGridView(gvRoles, Localization);

            gvRoles.DataSource = selRoles;
            gvRoles.DataBind();
        }

        #endregion
    }
}