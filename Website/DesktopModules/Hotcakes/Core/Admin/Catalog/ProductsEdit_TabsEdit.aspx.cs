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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class ProductsEdit_TabsEdit : BaseProductPage
    {
        private const string closeUrl = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/{0}?id={1}";
        protected internal string productBvin = string.Empty;


        private string tabid = string.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            tabid = Request.QueryString["tid"];
            productBvin = Request.QueryString["id"];
            PageTitle = "Edit Product Tab";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadItem();
            }
        }

        private void LoadItem()
        {
            var p = HccApp.CatalogServices.Products.Find(productBvin);
            if (p == null) return;
            if (p.Tabs.Count < 1) return;

            var tab = p.Tabs.FirstOrDefault(t => t.Bvin == tabid);
            if (tab != null)
            {
                TabTitleField.Text = tab.TabTitle;
                HtmlDataField.Text = tab.HtmlData;
            }
        }

        private bool SaveItem()
        {
            MessageBox1.ClearMessage();
            var success = false;

            var p = HccApp.CatalogServices.Products.Find(productBvin);
            if (p == null)
                return false;

            var tab = p.Tabs.FirstOrDefault(t => t.Bvin == tabid);
            if (tab == null)
            {
                tab = new ProductDescriptionTab();
                tab.Bvin = tabid;
                p.Tabs.Add(tab);
            }
            tab.TabTitle = TabTitleField.Text.Trim();
            tab.HtmlData = HtmlDataField.Text.Trim();
            success = HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);

            if (success)
            {
                MessageBox1.ShowOk("Changes Saved!");
            }
            else
            {
                MessageBox1.ShowWarning("Unable to save changes. An administrator has been alerted.");
            }

            return success;
        }

        protected void btnSaveOption_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        protected void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            if (SaveItem())
            {
                Response.Redirect("ProductsEdit_Tabs.aspx?id=" + productBvin);
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format(closeUrl, "ProductsEdit_Tabs.aspx", productBvin));
        }
    }
}