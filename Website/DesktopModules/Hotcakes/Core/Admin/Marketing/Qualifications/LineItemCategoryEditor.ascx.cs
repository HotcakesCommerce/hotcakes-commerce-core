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

using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class LineItemCategoryEditor : BaseQualificationControl
    {
        private LineItemCategory TypedQualification
        {
            get { return Qualification as LineItemCategory; }
        }

        protected void btnAddLineItemCategory_Click(object sender, EventArgs e)
        {
            var selected = lstLineItemCategories?.SelectedValue?.Trim();
            var q = TypedQualification;
            if (!string.IsNullOrEmpty(selected) && q != null)
            {
                q.AddCategoryId(selected);
                UpdatePromotion();
            }

            LoadQualification();
        }

        protected void gvLineItemCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null || e?.Keys == null || e.Keys.Count == 0) return;

            var bvin = e.Keys[0] as string;
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveCategoryId(bvin.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            var allCats = HccApp?.CatalogServices?.Categories?.FindAll() ?? new List<CategorySnapshot>();
            var available = CategoriesHelper.ListFullTreeWithIndents(allCats, true) ?? new Collection<ListItem>();

            var displayData = new List<FriendlyBvinDisplay>();

            var currentIds = TypedQualification?.CurrentCategoryIds() ?? new List<string>();
            foreach (var bvin in currentIds)
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                var t = available.FirstOrDefault(y => string.Equals(y.Value, bvin, StringComparison.OrdinalIgnoreCase));
                if (t != null)
                {
                    item.DisplayName = t.Text;
                    available.Remove(t);
                }
                displayData.Add(item);
            }

            lstLineItemCategories.Items.Clear();
            foreach (var li in available)
            {
                lstLineItemCategories.Items.Add(li);
            }

            chkLineItemCategoryNot.Checked = TypedQualification?.CategoryNot ?? false;
            gvLineItemCategories.DataSource = displayData;
            gvLineItemCategories.DataBind();
        }

        public override bool SaveQualification()
        {
            var q = TypedQualification;
            if (q != null)
            {
                q.CategoryNot = chkLineItemCategoryNot.Checked;
            }

            return UpdatePromotion();
        }

        protected void btnDeleteLineItemCategory_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}