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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class ProductIsEditor : BaseQualificationControl
    {
        private HasProductsQualificationBase TypedQualification
        {
            get { return Qualification as HasProductsQualificationBase; }
        }

        public string Title { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (gvProductBvins != null) gvProductBvins.RowDeleting += gvProductBvins_RowDeleting;
            if (btnAddProduct != null) btnAddProduct.Click += btnAddProduct_Click;

            if (!string.IsNullOrEmpty(Title))
            {
                Title = Localization.GetString(Title);
            }
            else
            {
                Title = Localization.GetString("WhenProductIs");
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            if (q == null || ucProductPicker == null) return;

            var selected = ucProductPicker.SelectedProducts;
            if (selected == null) return;

            // Ensure items are strings and trimmed
            var ids = selected.OfType<string>()
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();

            if (ids.Count == 0) return;

            q.AddProductIds(ids);
            UpdatePromotion();
            LoadQualification();
        }

        private void gvProductBvins_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null || e?.Keys == null || e.Keys.Count == 0) return;

            var keyObj = e.Keys[0];
            if (keyObj == null) return;

            var bvin = keyObj as string ?? keyObj.ToString();
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveProductId(bvin.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            if (ucProductPicker != null) ucProductPicker.LoadSearch();

            var q = TypedQualification;
            if (q == null)
            {
                if (gvProductBvins != null)
                {
                    gvProductBvins.DataSource = Enumerable.Empty<object>();
                    gvProductBvins.DataBind();
                }
                return;
            }

            var ids = q.GetProductIds() ?? Enumerable.Empty<string>();
            var products = HccApp?.CatalogServices?.Products?.FindManyWithCache(ids) ?? Enumerable.Empty<object>();

            if (gvProductBvins != null)
            {
                gvProductBvins.DataSource = products;
                gvProductBvins.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            return UpdatePromotion();
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}