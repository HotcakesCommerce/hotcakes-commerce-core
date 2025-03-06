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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Products_Edit_Reviews : BaseProductPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit Product Reviews";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                CurrentTab = AdminTabType.Catalog;

                LoadReviews();
            }
        }

        private void LoadReviews()
        {
            var reviews = HccApp.CatalogServices.ProductReviews.FindByProductId(ProductId);
            if (reviews.Count == 0)
            {
                lblNoReviews.Visible = true;
                dlReviews.DataSource = null;
                dlReviews.Visible = false;
            }
            else
            {
                dlReviews.Visible = true;
                dlReviews.DataSource = reviews;
                dlReviews.DataBind();
            }
        }

        protected void dlReviews_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var review = e.Item.DataItem as ProductReview;

                var panelRating = e.Item.FindControl("panelRating") as Panel;
                var lblReviewDate = e.Item.FindControl("lblReviewDate") as Label;
                var lblProductID = e.Item.FindControl("lblProductID") as Label;
                var lblReview = e.Item.FindControl("lblReview") as Label;

                var rating = new Label();
                rating.Text = string.Format("{0}", (int) review.Rating);

                var product = HccApp.CatalogServices.Products.Find(review.ProductBvin);

                panelRating.Controls.Add(rating);
                lblReviewDate.Text = DateHelper.ConvertUtcToStoreTime(HccApp, review.ReviewDateUtc).ToString();
                lblProductID.Text = product.ProductName;
                lblReview.Text = review.Description.Replace("\r\n", "<br/>");
            }
        }

        protected void dlReviews_EditCommand(object source, DataGridCommandEventArgs e)
        {
            var reviewID = (string) dlReviews.DataKeys[e.Item.ItemIndex];
            Response.Redirect("Reviews_Edit.aspx?reviewID="
                              + reviewID + "&pid=" + Server.UrlEncode(ProductId));
        }

        protected void dlReviews_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            var reviewID = (string) dlReviews.DataKeys[e.Item.ItemIndex];
            HccApp.CatalogServices.ProductReviews.Delete(reviewID);
            LoadReviews();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var pr = new ProductReview();
            pr.Approved = true;
            pr.Description = "New Review";
            pr.UserID = HccApp.CurrentCustomerId;
            pr.ReviewDateUtc = DateTime.UtcNow;
            pr.ProductBvin = ProductId;

            if (HccApp.CatalogServices.ProductReviews.Create(pr))
            {
                Response.Redirect("Reviews_Edit.aspx?reviewID=" + pr.Bvin + "&DOC=1" + "&pid=" + ProductId);
            }
        }
    }
}