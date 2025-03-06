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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Products_Edit_Categories : BaseProductPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Edit Product Categories";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                string EditID;
                EditID = Request.QueryString["id"];
                if (EditID.Trim().Length < 1)
                {
                    msg.ShowError("Unable to load the requested product.");
                }
                else
                {
                    var p = new Product();
                    p = HccApp.CatalogServices.Products.Find(EditID);
                    if (p != null)
                    {
                        ViewState["ID"] = EditID;
                    }
                    else
                    {
                        msg.ShowError("Unable to load the requested product.");
                    }
                    p = null;
                }

                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            chkCategories.Items.Clear();

            var t = HccApp.CatalogServices.FindCategoriesForProduct(Request.QueryString["ID"]);
            var tree = CategoriesHelper.ListFullTreeWithIndents(HccApp.CatalogServices.Categories, true);

            foreach (var li in tree)
            {
                chkCategories.Items.Add(li);
            }

            foreach (var ca in t)
            {
                foreach (ListItem l in chkCategories.Items)
                {
                    if (l.Value == ca.Bvin)
                    {
                        l.Selected = true;
                    }
                }
            }
        }

        private void SaveSettings()
        {
            foreach (ListItem li in chkCategories.Items)
            {
                if (li.Selected)
                {
                    HccApp.CatalogServices.AddProductToCategory(Request.QueryString["id"], li.Value);
                }
                else
                {
                    HccApp.CatalogServices.RemoveProductFromCategory(Request.QueryString["id"], li.Value);
                }
            }
        }

        protected void CancelButton_Click1(object sender, EventArgs e)
        {
            Cancel();
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Cancel()
        {
            Response.Redirect("Products_edit.aspx?id=" + ViewState["ID"]);
        }

        protected bool Save()
        {
            msg.ClearMessage();
            SaveSettings();
            LoadCategories();
            msg.ShowOk("Changes Saved!");
            return true;
        }
    }
}