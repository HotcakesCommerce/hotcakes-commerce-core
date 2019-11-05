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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class ProductReviewEditor : HccUserControl
    {
        public string ReviewID
        {
            get { return Request.QueryString["ReviewID"]; }
            set { ViewState["ReviewID"] = value; }
        }

        public void LoadReview()
        {
            if (Request.QueryString["ReviewID"] != null)
            {
                var r = new ProductReview();
                r = HccApp.CatalogServices.ProductReviews.Find(ReviewID);
                if (r != null)
                {
                    var p = HccApp.CatalogServices.Products.FindWithCache(r.ProductBvin);
                    if (p != null)
                    {
                        lblProductName.Text = p.ProductName;
                    }
                    else
                    {
                        lblProductName.Text = "Unknown";
                    }
                    if (r.UserID != string.Empty)
                    {
                        var u = HccApp.MembershipServices.Customers.Find(r.UserID);
                        if (u == null)
                        {
                            lblUserName.Text = "Anonymous";
                        }
                        else
                        {
                            lblUserName.Text = u.LastName + ", " + u.FirstName + " " + u.Email;
                        }
                    }
                    lblReviewDate.Text = r.ReviewDateForTimeZone(HccApp.CurrentStore.Settings.TimeZone).ToString();
                    chkApproved.Checked = r.Approved;
                    KarmaField.Text = r.Karma.ToString();
                    switch (r.Rating)
                    {
                        case ProductReviewRating.ZeroStars:
                            lstRating.SelectedValue = "0";
                            break;
                        case ProductReviewRating.OneStar:
                            lstRating.SelectedValue = "1";
                            break;
                        case ProductReviewRating.TwoStars:
                            lstRating.SelectedValue = "2";
                            break;
                        case ProductReviewRating.ThreeStars:
                            lstRating.SelectedValue = "3";
                            break;
                        case ProductReviewRating.FourStars:
                            lstRating.SelectedValue = "4";
                            break;
                        case ProductReviewRating.FiveStars:
                            lstRating.SelectedValue = "5";
                            break;
                    }
                    DescriptionField.Text = r.Description;
                }
                r = null;
            }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (Request.QueryString["ReviewID"] != null)
                {
                    var r = new ProductReview();
                    r = HccApp.CatalogServices.ProductReviews.Find(ReviewID);
                    if (r != null)
                    {
                        r.Approved = chkApproved.Checked;
                        r.Karma = int.Parse(KarmaField.Text.Trim());
                        r.Rating = (ProductReviewRating) int.Parse(lstRating.SelectedValue);
                        r.Description = DescriptionField.Text.Trim();
                    }
                    HccApp.CatalogServices.ProductReviews.Update(r);
                    r = null;

                    if (Request.QueryString["pid"] == null)
                    {
                        Response.Redirect("ReviewsToModerate.aspx");
                    }
                    else
                    {
                        Response.Redirect("Products_Edit_Reviews.aspx?id=" + Request.QueryString["pid"]);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["pid"] == null)
            {
                Response.Redirect("ReviewsToModerate.aspx");
            }
            else
            {
                Response.Redirect("Products_Edit_Reviews.aspx?id=" + Request.QueryString["pid"]);
            }
        }
    }
}