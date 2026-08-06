    #region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class ProductCategoryEditor : BaseQualificationControl
    {
        private ProductCategory TypedQualification
        {
            get { return Qualification as ProductCategory; }
        }

        protected void btnAddProductCategory_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            if (q == null || lstProductCategories == null) return;

            var selected = lstProductCategories.SelectedValue;
            if (string.IsNullOrWhiteSpace(selected)) return;

            q.AddCategoryId(selected.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvProductCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null || e?.Keys == null || e.Keys.Count == 0) return;

            var keyObj = e.Keys[0];
            if (keyObj == null) return;

            var bvin = keyObj as string ?? keyObj.ToString();
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveCategoryId(bvin.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            var q = TypedQualification;
            if (q == null)
            {
                if (lstProductCategories != null) lstProductCategories.Items.Clear();
                if (gvProductCategories != null) gvProductCategories.DataSource = Enumerable.Empty<object>();
                if (gvProductCategories != null) gvProductCategories.DataBind();
                return;
            }

            var allCats = HccApp.CatalogServices.Categories.FindAll() ?? new List<CategorySnapshot>();
            var available = CategoriesHelper.ListFullTreeWithIndents(allCats, true) ?? new Collection<ListItem>();

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var rawBvin in q.CurrentCategoryIds() ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(rawBvin)) continue;
                var bvin = rawBvin.Trim();

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                var match = available.FirstOrDefault(y =>
                    string.Equals((y?.Value ?? string.Empty).Trim(), bvin, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    item.DisplayName = match.Text ?? bvin;
                    available.Remove(match);
                }

                displayData.Add(item);
            }

            if (lstProductCategories != null)
            {
                lstProductCategories.Items.Clear();
                foreach (var li in available)
                {
                    lstProductCategories.Items.Add(li);
                }
            }

            if (gvProductCategories != null)
            {
                gvProductCategories.DataSource = displayData;
                gvProductCategories.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            var q = TypedQualification;
            if (q == null) return false;

            return UpdatePromotion();
        }

        protected void btnDeleteProductCategory_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}