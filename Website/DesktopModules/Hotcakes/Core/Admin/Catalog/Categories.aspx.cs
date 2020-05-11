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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using System.Web.UI.WebControls;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Categories : BaseAdminPage
    {
        private string IconImage(CategorySourceType type)
        {
            switch (type)
            {
                case CategorySourceType.DrillDown:
                    return "IconDrillDown.png";
                default:
                    return "IconCategory.png";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                RenderCategoryTree();
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Categories";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }


        private void RenderCategoryTree()
        {
            var sb = new StringBuilder();

            var allCats = HccApp.CatalogServices.Categories.FindAllSnapshotsPaged(1, 5000);

            lstParents.Items.Clear();
            
            lstParents.Items.Add(new ListItem("(Root)", string.Empty));
            var parents = CategoriesHelper.ListFullTreeWithIndents(allCats, true);
            foreach (var li in parents)
            {
                lstParents.Items.Add(new ListItem(li.Text, li.Value));
            }

            RenderChildren(string.Empty, allCats, sb);

            litMain.Text = sb.ToString();
        }

        private void RenderChildren(string bvin, List<CategorySnapshot> allCats, StringBuilder sb)
        {
            var children = Category.FindChildrenInList(allCats, bvin, true);
            if (children == null) return;
            if (children.Count > 0)
            {
                sb.Append("<div id=\"children" + bvin +
                          "\" class=\"ui-sortable nested hcGrid\" unselectable=\"on\" style=\"-moz-user-select: none; margin-bottom: 0px !important;\">");
                foreach (var child in children)
                {
                    var perfUrl = string.Format("~/DesktopModules/Hotcakes/Core/Admin/Catalog/Categories_Performance.aspx?id={0}", child.Bvin);
                    var editUrl = string.Format("~/DesktopModules/Hotcakes/Core/Admin/Catalog/Categories_Edit.aspx?id={0}", child.Bvin);
                    editUrl = ResolveUrl(editUrl);
                    perfUrl = ResolveUrl(perfUrl);
                    var icon = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/" + IconImage(child.SourceType));

                    sb.Append("<div id=\"" + child.Bvin + "\" class=\"dragitem2 nested\">");
                    sb.Append("<table width=\"100%\" class=\"formtable\" style=\"table-layout: fixed;\">");
                    sb.Append("<tbody><tr>");
                    sb.AppendFormat(
                        "<td width=\"5%\" style=\"text-align: center;\" class=\"handle\" align=\"center\"><a href=\"#\" class=\"hcIconMove\" alt=\"Move\"></a></td>");
                    sb.Append("<td width=\"85%\">");

                    var childNodeStyle = "";
                    if (!string.IsNullOrEmpty(bvin))
                    {
                        var i = 0;
                        var parentId = child.ParentId;
                        var padding = 0;
                        for (i = 0; i < 1; i++)
                        {
                            if (!string.IsNullOrEmpty(parentId))
                            {
                                var parent = allCats.Where(y => y.Bvin == parentId).FirstOrDefault();
                                if (parent != null)
                                {
                                    parentId = parent.ParentId;
                                    i = -1;
                                    padding += 40;
                                }
                            }
                        }
                        childNodeStyle = "padding-left: " + (padding == 0 ? 40 : padding) + "px;";
                    }

                    sb.Append("<a href=\"" + perfUrl + "\" style=\"" + childNodeStyle + "\">" + child.Name + "</a>");
                    sb.Append("</td>");
                    sb.Append("<td width=\"5%\" style=\"text-align: center;\">");
                    sb.Append("<a href=\"" + editUrl + "\" alt=\"edit\" class=\"hcIconEdit\"></a></td>");

                    if (Category.FindChildrenInList(allCats, child.Bvin).Count > 0)
                    {
                        sb.Append("<td width=\"5%\" style=\"text-align: center;\">&nbsp;</td>");
                    }
                    else
                    {
                        sb.Append("<td width=\"5%\" style=\"text-align: center;\"><a id=\"rem" + child.Bvin +
                                  "\" class=\"trash hcIconDelete\" href=\"javascript:void(0);\" alt=\"Delete\">");
                        sb.AppendFormat("</a></td>");
                    }

                    sb.AppendFormat("</tr></tbody></table>",
                        ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/draghandle.png"));
                    RenderChildren(child.Bvin, allCats, sb);
                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var sourceType = CategorySourceType.Manual;
            Enum.TryParse(lstType.SelectedItem.Value, out sourceType);

            var parentId = lstParents.SelectedItem.Value;

            var editUrl = HccApp.CatalogServices.EditorRouteForCategory(sourceType, null, parentId);
            Response.Redirect(editUrl);
        }
    }
}