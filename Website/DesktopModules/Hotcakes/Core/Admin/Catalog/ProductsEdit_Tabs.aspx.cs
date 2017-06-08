#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class ProductsEdit_Tabs : BaseProductPage
    {
        private Product localProduct = new Product();
        private string productBvin = string.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit Product Tabs";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (Request.QueryString["id"] != null)
            {
                productBvin = Request.QueryString["id"];
                localProduct = HccApp.CatalogServices.Products.Find(productBvin);
            }

            if (!Page.IsPostBack)
            {
                CurrentTab = AdminTabType.Catalog;
                LoadItems();
            }
        }

        private void LoadItems()
        {
            RenderItems(localProduct.Tabs);
        }

        private void RenderItems(List<ProductDescriptionTab> tabs)
        {
            if (tabs == null)
            {
                litResults.Text = "This product does not have any tabs yet. Click &quot;New&quot; to create one.";
                return;
            }
            var sb = new StringBuilder();

            sb.Append("<div id=\"dragitem-list\">");
            foreach (var tab in tabs.OrderBy(y => y.SortOrder))
            {
                RenderSingleItem(sb, tab);
            }
            sb.Append("</div>");
            litResults.Text = sb.ToString();
        }

        private void RenderSingleItem(StringBuilder sb, ProductDescriptionTab t)
        {
            var destinationLink = string.Format("ProductsEdit_TabsEdit.aspx?tid={0}&id={1}", t.Bvin, productBvin);
            var trashcanImageUrl = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/trashcan.png");
            var draghandleImageUrl = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/draghandle.png");

            sb.Append("<div class=\"dragitem\" id=\"" + t.Bvin +
                      "\"><table class=\"formtable hcGrid\" width=\"100%\"><tbody class=\"ui-sortable\"><tr>");
            sb.AppendFormat(
                "<td width=\"30\"><a href=\"#\" class=\"handle\"><img class=\"hcIconMove\" alt=\"Move\" /></a></td>");
            sb.Append("<td><a href=\"" + destinationLink + "\">");
            sb.Append(t.TabTitle);
            sb.Append("</a></td>");
            sb.Append("<td width=\"75\"><a href=\"" + destinationLink +
                      "\"><img class=\"hcIconEdit\" alt=\"edit\" /></a>");
            sb.Append("<a href=\"#\" class=\"trash\" id=\"rem" + t.Bvin + "\"");
            sb.AppendFormat("><img class=\"hcIconDelete\" alt=\"Delete\" /></a></td>");
            sb.Append("</tr></tbody></table></div>");
        }

        protected void NewTabButton_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();

            var t = new ProductDescriptionTab();
            t.Bvin = Guid.NewGuid().ToString().Replace("{", string.Empty).Replace("}", string.Empty);

            if (localProduct.Tabs.Count > 0)
            {
                var m = (from sort in localProduct.Tabs
                    select sort.SortOrder).Max();
                t.SortOrder = m + 1;
            }
            else
            {
                t.SortOrder = 1;
            }

            localProduct.Tabs.Add(t);
            if (HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(localProduct))
            {
                Response.Redirect("ProductsEdit_TabsEdit.aspx?tid=" + t.Bvin + "&id=" + localProduct.Bvin);
            }
            else
            {
                MessageBox1.ShowError("Unable to update product tabs.");
            }

            localProduct = HccApp.CatalogServices.Products.Find(localProduct.Bvin);
            LoadItems();
        }
    }
}