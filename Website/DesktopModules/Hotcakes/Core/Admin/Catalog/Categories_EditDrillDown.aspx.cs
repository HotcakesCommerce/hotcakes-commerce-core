#region License

// Distributed under the MIT License
// ============================================================
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
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using MerchantTribe.Commerce.Catalog;
using MerchantTribe.Commerce.Content;
using MerchantTribe.Commerce.Membership;
using MerchantTribe.Commerce.Utilities;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MerchantTribeStore.BVAdmin.Catalog
{
    public partial class Categories_EditDrillDown : BaseAdminPage
    {

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                this.NameField.Focus();

                if (Request.QueryString["id"] != null)
                {
                    this.BvinField.Value = Request.QueryString["id"];
                    if (Request.QueryString["type"] != null)
                    {
                        ViewState["type"] = Request.QueryString["type"];
                    }
                    Category category = LoadCategory();

                    if (category != null)
                    {
                        if (category.SourceType != CategorySourceType.DrillDown)
                        {
                            Response.Redirect("categories.aspx");
                        }
                    }

                    if (ViewState["type"] == null)
                    {
                        ViewState["type"] = category.SourceType;
                    }
                    CategoryBreadCrumbTrail1.LoadTrail(Request.QueryString["id"]);
                    PopulateStoreLink(category);
                }
                else
                {
                    this.BvinField.Value = string.Empty;
                    if (Request.QueryString["ParentID"] != null)
                    {
                        CategoryBreadCrumbTrail1.LoadTrail(Request.QueryString["ParentID"]);
                        if (Request.QueryString["type"] != null)
                        {
                            ViewState["type"] = Request.QueryString["type"];
                        }
                        this.ParentIDField.Value = (string)Request.QueryString["ParentID"];
                    }
                    else
                    {
                        Response.Redirect("~/HCC/Admin/Catalog/Categories.aspx");
                    }
                }

                PopulatePropertyList();
                RenderFilterTree();
            }
        }

        private void PopulatePropertyList()
        {
            this.lstProperty.Items.Clear();
            foreach (ProductProperty p in MTApp.CatalogServices.ProductProperties.FindAll())
            {
                this.lstProperty.Items.Add(new System.Web.UI.WebControls.ListItem(p.PropertyName, p.Id.ToString()));
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Edit Category";
            this.CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        private Category LoadCategory()
        {
            Category c = MTApp.CatalogServices.Categories.Find(this.BvinField.Value);
            if (c != null)
            {

                if (c.Bvin != string.Empty)
                {
                    this.NameField.Text = c.Name;
                    this.DescriptionField.Text = c.Description;                    
                    this.MetaDescriptionField.Text = c.MetaDescription;
                    this.MetaKeywordsField.Text = c.MetaKeywords;
                    this.MetaTitleField.Text = c.MetaTitle;
                    this.RewriteUrlField.Text = c.RewriteUrl;

                    this.ParentIDField.Value = c.ParentId;
                }
            }
            return c;
        }

        private void PopulateStoreLink(Category c)
        {

            HyperLink m = new HyperLink();
            m.ImageUrl = "~/HCC/Admin/Images/Buttons/ViewInStore.png";
            m.ToolTip = c.MetaTitle;
            m.NavigateUrl = UrlRewriter.BuildUrlForCategory(new CategorySnapshot(c), MTApp.CurrentRequestContext.RoutingContext);
            m.EnableViewState = false;
            this.inStore.Controls.Add(m);

        }

        private bool Save()
        {
            bool result = false;

            Category c = MTApp.CatalogServices.Categories.Find(this.BvinField.Value);
            if (c == null)
            {
                c = new Category();
            }
            if (c != null)
            {
                c.Name = this.NameField.Text.Trim();
                c.Description = this.DescriptionField.Text.Trim();                
                c.MetaDescription = this.MetaDescriptionField.Text.Trim();
                c.MetaTitle = this.MetaTitleField.Text.Trim();
                c.MetaKeywords = this.MetaKeywordsField.Text.Trim();
                c.ShowInTopMenu = false;
                c.SourceType = CategorySourceType.DrillDown;

                // no entry, generate one
                if (c.RewriteUrl.Trim().Length < 1)
                {
                    c.RewriteUrl = MerchantTribe.Web.Text.Slugify(c.Name, true, true);
                }
                else
                {
                    c.RewriteUrl = MerchantTribe.Web.Text.Slugify(this.RewriteUrlField.Text, true, true);
                }
                this.RewriteUrlField.Text = c.RewriteUrl;

                c.CustomerChangeableSortOrder = true;

                if (this.BvinField.Value == string.Empty)
                {
                    c.ParentId = this.ParentIDField.Value;
                    result = MTApp.CatalogServices.Categories.Create(c);
                }
                else
                {
                    result = MTApp.CatalogServices.Categories.Update(c);
                }

                if (result == false)
                {
                    this.lblError.Text = "Unable to save category. Uknown error.";
                }
                else
                {
                    // Update bvin field so that next save will call updated instead of create
                    this.BvinField.Value = c.Bvin;
                }

            }

            return result;
        }

        protected void btnCancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("Categories.aspx");
        }

        protected void btnSaveChanges_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.lblError.Text = string.Empty;

            if (Save() == true)
            {
                Response.Redirect("Categories.aspx");
            }
            RenderFilterTree();
        }

        protected void btnSelectProducts_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (this.Save() == true)
            {
                Response.Redirect("Categories_ManualSelection.aspx?id=" + this.BvinField.Value + "&type=" + ViewState["type"]);
            }
        }

        protected void UpdateButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.lblError.Text = string.Empty;
            if (this.Save())
            {
                MessageBox1.ShowOk("Category Updated Successfully.");
                Category cat = MTApp.CatalogServices.Categories.Find(this.BvinField.Value);
                if (cat != null && cat.Bvin != string.Empty)
                {
                    PopulateStoreLink(cat);
                }
            }
            else
            {
                MessageBox1.ShowError("Error during update. Please check event log.");
            }
            RenderFilterTree();
        }


        /* Filters Area */

        private void RenderFilterTree()
        {
            StringBuilder sb = new StringBuilder();

            CategoryFacetManager manager = CategoryFacetManager.InstantiateForDatabase(MTApp.CurrentRequestContext);

            List<CategoryFacet> allFacets = manager.FindByCategory(this.BvinField.Value);

            this.lstParents.Items.Clear();
            this.lstParents.Items.Add(new System.Web.UI.WebControls.ListItem("Nothing (always show)", "0"));
            this.lstParents.Items.Add(new System.Web.UI.WebControls.ListItem("", "0"));
            foreach (CategoryFacet f in allFacets)
            {
                this.lstParents.Items.Add(new System.Web.UI.WebControls.ListItem(f.FilterName, f.Id.ToString()));
            }
            RenderChildren(0, allFacets, sb, manager);

            this.litMain.Text = sb.ToString();
        }

        private void RenderChildren(long id, List<CategoryFacet> allFacets, StringBuilder sb, CategoryFacetManager manager)
        {

            List<CategoryFacet> children = manager.FindByParentInList(allFacets, id);
            if (children == null) return;
            if (children.Count > 0)
            {
                sb.Append("<div id=\"children" + id.ToString() + "\" class=\"ui-sortable nested\" unselectable=\"on\" style=\"-moz-user-select: none;\">");
                foreach (CategoryFacet child in children)
                {

                    sb.Append("<div id=\"" + child.Id.ToString() + "\" class=\"dragitem2\">");
                    sb.Append("<table width=\"100%\" class=\"formtable\">");
                    sb.Append("<tbody><tr><td>");
                    sb.Append("<a href=\"#\">" + child.FilterName + "</a>");
                    //sb.Append("<br /><div class=\"nested\">+ new</div>");
                    sb.Append("</td>");
                    sb.Append("<td width=\"75\">");                    
                    //sb.Append("<a href=\"#\">");
                    //sb.Append("<img alt=\"edit\" src=\"/hcc/admin/images/buttons/edit.png\"></a>");
                    sb.Append("&nbsp;");
                    sb.Append("</td>");


                    if (manager.FindByParentInList(allFacets, child.Id).Count > 0)
                    {
                        sb.Append("<td width=\"30\">&nbsp;</td>");
                    }
                    else
                    {
                        sb.Append("<td width=\"30\"><a id=\"rem" + child.Id.ToString() + "\" class=\"trash\" href=\"#\">");
						sb.Append("<img alt=\"Delete\" src=\"/hcc/images/system/trashcan.png\"></a></td>");
                    }


                    sb.Append("<td width=\"30\"><a class=\"handle\" href=\"#\">");
					sb.Append("<img alt=\"Move\" src=\"/hcc/images/system/draghandle.png\"></a></td></tr></tbody></table>");
                    RenderChildren(child.Id, allFacets, sb, manager);
                    sb.Append("</div>");
                }
                sb.Append("</div>");
            }
        }

        protected void btnNew_Click(object sender, ImageClickEventArgs e)
        {
            this.Save();

            CategoryFacetManager manager = CategoryFacetManager.InstantiateForDatabase(MTApp.CurrentRequestContext);

            
            CategoryFacet f = new CategoryFacet();
            f.CategoryId = this.BvinField.Value;
            f.PropertyId = long.Parse(this.lstProperty.SelectedItem.Value);
            f.FilterName = this.lstProperty.SelectedItem.Text;
            f.ParentPropertyId = long.Parse(this.lstParents.SelectedItem.Value);
            f.DisplayMode = CategoryFacetDisplayMode.Single;

            f.SortOrder = manager.FindMaxSortForCategoryParent(this.BvinField.Value, f.ParentPropertyId);

            if (!ExistsAlready(manager, f.CategoryId, f.PropertyId))
            {
                manager.Create(f);
            }
            
            RenderFilterTree();            
        }

        private bool ExistsAlready(CategoryFacetManager manager, string categoryBvin, long propertyId)
        {
            var facets = manager.FindByCategory(categoryBvin);
            if (facets != null)
            {
                var count = facets.Where(y => y.PropertyId == propertyId).Count();
                if (count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}