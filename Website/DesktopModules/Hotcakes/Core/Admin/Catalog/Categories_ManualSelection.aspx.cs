#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Categories_ManualSelection : BaseCategoryPage
    {
        public string CategoryBvin = string.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitNavMenu(ucNavMenu);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    BvinField.Value = Request.QueryString["id"];
                    if (Request.QueryString["type"] != null)
                    {
                        ViewState["type"] = Request.QueryString["type"];
                    }
                    lnkBack.NavigateUrl = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/Categories_Edit.aspx?id=" +
                                          BvinField.Value;

                    var category = HccApp.CatalogServices.Categories.Find(BvinField.Value);
                    lnkViewInStore.NavigateUrl = UrlRewriter.BuildUrlForCategory(new CategorySnapshot(category));
                }

                else
                {
                    // No bvin so send back to categories page
                    Response.Redirect("Categories.aspx");
                }
            }


            LoadCategory();
            CategoryBvin = BvinField.Value;
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
        }

        private void LoadCategory()
        {
            Category c;
            c = HccApp.CatalogServices.Categories.Find(BvinField.Value);
            if (c != null)
            {
                if (c.Bvin != string.Empty)
                {
                    var allProducts = HccApp.CatalogServices.FindProductForCategoryWithSort(c.Bvin, c.DisplaySortOrder,
                        true);

                    litProducts.Text = RenderProductList(allProducts);

                    // Exclude existing products from search results
                    if (!Page.IsPostBack)
                    {
                        ProductPicker1.ExcludeCategoryBvin = c.Bvin;
                    }
                }
            }
        }

        private string RenderProductList(List<Product> products)
        {
            var result = string.Empty;

            var sb = new StringBuilder();

            if (products != null)
            {
                sb.Append("<div id=\"sortable\">");
                foreach (var p in products)
                {
                    RenderSingleProduct(p, sb);
                }
                sb.Append("</div>");
            }

            result = sb.ToString();

            return result;
        }

        private void RenderSingleProduct(Product p, StringBuilder sb)
        {
            sb.AppendFormat("<div class=\"dragitem list-item\" id=\"{0}\">", p.Bvin);
            sb.Append(
                "<table border=\"0\" cellspacing=\"0\" cellpadding=\"2\" width=\"100%\" class=\"formtable hcGrid hcCategoryManualSelection\" >");
            sb.Append("<tbody class=\"ui-sortable\">");
            sb.Append("<tr>");
            sb.AppendFormat(
                "<td class=\"handle\" align=\"center\"><a class=\"hcIconMove\" href=\"#\" alt=\"Move\"></a></td>");
            sb.AppendFormat("<td width=\"25%\">{0}</td>", p.Sku);
            sb.AppendFormat("<td width=\"42%\">{0}</td>", p.ProductName);
            sb.AppendFormat(
                "<td align=\"center\"><a class=\"trash hcIconDelete\" href=\"javascript:void(0);\" title=\"{1}\" id=\"rem{0}\" data-id=\"{0}\" alt=\"{2}\"></a></td>",
                p.Bvin, Localization.GetString("RemoveProduct"), Localization.GetString("Delete"));
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</div>");
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            msg.ClearMessage();
            if (ProductPicker1.SelectedProducts.Count == 0)
            {
                msg.ShowInformation(Localization.GetString("NoProducts"));
            }
            foreach (var s in ProductPicker1.SelectedProducts)
            {
                HccApp.CatalogServices.AddProductToCategory(s, BvinField.Value);
            }
            LoadCategory();
            ProductPicker1.LoadSearch();
        }
    }
}