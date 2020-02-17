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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Products_ProductTypes : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Product Types";
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
            dgList.DataSource = HccApp.CatalogServices.ProductTypes.FindAll().Where(pt => !pt.IsPermanent);
            dgList.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var productType = new ProductType();
                productType.ProductTypeName = txtNewNameField.Text;
                if (HccApp.CatalogServices.ProductTypes.Create(productType))
                {
                    Response.Redirect("ProductTypesEdit.aspx?id=" + productType.Bvin);
                }
                else
                {
                    msg.ShowError("Error while attempting to create new product type.");
                }
            }
        }

        protected void dgList_EditCommand(object source, DataGridCommandEventArgs e)
        {
            var productTypeId = (string) dgList.DataKeys[e.Item.ItemIndex];
            Response.Redirect("ProductTypesEdit.aspx?id=" + productTypeId);
        }

        protected void dgList_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var deleteID = (string) dgList.DataKeys[e.Item.ItemIndex];
            HccApp.CatalogServices.ProductTypeDestroy(deleteID);
            FillList();
        }

        protected void dgList_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem | e.Item.ItemType == ListItemType.Item)
            {
                var productTypeId = (string) dgList.DataKeys[e.Item.ItemIndex];
                var btnDelete = e.Item.FindControl("btnDelete") as LinkButton;
                btnDelete.Visible = HccApp.CatalogServices.Products.FindCountByProductType(productTypeId) <= 0;
            }
        }
    }
}