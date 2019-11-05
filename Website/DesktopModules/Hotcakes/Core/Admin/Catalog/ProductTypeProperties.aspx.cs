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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductTypeProperties : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Product Type Properties";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                FillList();
            }
        }

        private void FillList()
        {
            dgList.DataSource = HccApp.CatalogServices.ProductProperties.FindAll();
            dgList.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var p = new ProductProperty();
            p.DisplayName = "New Property";
            p.TypeCode = (ProductPropertyType) int.Parse(lstPropertyType.SelectedValue);
            var success = HccApp.CatalogServices.ProductProperties.Create(p);
            if (success)
            {
                Response.Redirect("ProductTypePropertiesEdit.aspx?id=" + p.Id);
            }
            else
            {
                msg.ShowError("Error while attempting to create new property.");
            }
        }

        protected void dgList_EditCommand(object source, DataGridCommandEventArgs e)
        {
            var editID = (long) dgList.DataKeys[e.Item.ItemIndex];
            Response.Redirect("ProductTypePropertiesEdit.aspx?id=" + editID);
        }

        protected void dgList_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var deleteID = (long) dgList.DataKeys[e.Item.ItemIndex];
            HccApp.CatalogServices.ProductPropertiesDestroy(deleteID);
            FillList();
        }
    }
}