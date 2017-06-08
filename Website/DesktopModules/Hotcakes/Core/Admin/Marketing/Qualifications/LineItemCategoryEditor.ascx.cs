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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

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
            if (!string.IsNullOrEmpty(lstLineItemCategories.SelectedValue))
            {
                var q = TypedQualification;
                q.AddCategoryId(lstLineItemCategories.SelectedValue);
            }
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvLineItemCategories_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            var bvin = (string) e.Keys[0];
            q.RemoveCategoryId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            var allCats = HccApp.CatalogServices.Categories.FindAll();
            var available = CategoriesHelper.ListFullTreeWithIndents(allCats, true);

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in TypedQualification.CurrentCategoryIds())
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;

                var t = available.FirstOrDefault(y => y.Value == bvin);
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

            chkLineItemCategoryNot.Checked = TypedQualification.CategoryNot;
            gvLineItemCategories.DataSource = displayData;
            gvLineItemCategories.DataBind();
        }

        public override bool SaveQualification()
        {
            TypedQualification.CategoryNot = chkLineItemCategoryNot.Checked;

            return UpdatePromotion();
        }

        protected void btnDeleteLineItemCategory_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}