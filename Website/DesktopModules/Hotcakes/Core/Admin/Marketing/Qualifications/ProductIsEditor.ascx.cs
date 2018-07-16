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
            gvProductBvins.RowDeleting += gvProductBvins_RowDeleting;
            btnAddProduct.Click += btnAddProduct_Click;
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
            TypedQualification.AddProductIds(ucProductPicker.SelectedProducts.OfType<string>());
            UpdatePromotion();
            LoadQualification();
        }

        private void gvProductBvins_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var bvin = e.Keys[0] as string;
            TypedQualification.RemoveProductId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            ucProductPicker.LoadSearch();

            var ids = TypedQualification.GetProductIds();
            var products = HccApp.CatalogServices.Products.FindManyWithCache(ids);
            gvProductBvins.DataSource = products;
            gvProductBvins.DataBind();
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