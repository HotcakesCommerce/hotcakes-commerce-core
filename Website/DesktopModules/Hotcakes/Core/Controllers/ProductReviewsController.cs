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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class ProductReviewsController : BaseStoreController
    {
        public ActionResult Index(string slug)
        {
            var model = new ProductReviewsViewModel();

            var p = ParseProductFromSlug(slug);

            model.ProductView = new SingleProductViewModel(p, HccApp);

            SetPageMetaData(p);

            var reviews = p.ReviewsApproved;

            CopyReviews(model, reviews);

            if (reviews != null)
            {
                ViewBag.Avg = CalculateAverageRating(reviews);
            }

            return View(model);
        }

        private Product ParseProductFromSlug(string slug)
        {
            Product result = null;

            if (!string.IsNullOrEmpty(slug))
            {
                result = HccApp.CatalogServices.Products.FindBySlug(slug);
                if (result == null || result.Status == ProductStatus.Disabled)
                {
                    // Check for custom URL
                    var url = HccApp.ContentServices.CustomUrls.FindByRequestedUrl(slug, CustomUrlType.Product);
                    if (url != null)
                    {
                        var redirectUrl = HccUrlBuilder.RouteHccUrl(HccRoute.ProductReview,
                            new {slug = url.RedirectToUrl});
                        if (url.IsPermanentRedirect)
                            RedirectPermanent(redirectUrl);
                        else
                            Redirect(redirectUrl);
                    }
                    StoreExceptionHelper.ShowInfo(Localization.GetString("ProductNotFound"));
                }
            }

            return result;
        }

        private void CopyReviews(ProductReviewsViewModel model, List<ProductReview> reviews)
        {
            model.Reviews.Clear();
            foreach (var productReview in reviews)
            {
                var viewModel = new ProductReviewViewModel(productReview);

                CustomerAccount customerAccount = null;
                if (productReview.UserID != "0")
                {
                    customerAccount = HccApp.MembershipServices.Customers.Find(productReview.UserID);
                }

                viewModel.UserID = productReview.UserID;
                if (customerAccount == null)
                {
                    viewModel.UserName = string.Empty;
                    viewModel.City = string.Empty;
                    viewModel.State = string.Empty;
                }
                else
                {
                    viewModel.UserName = string.Format("{0}{1}", customerAccount.FirstName,
                        string.IsNullOrEmpty(customerAccount.LastName)
                            ? string.Empty
                            : customerAccount.LastName.Substring(0, 1));
                    viewModel.City = customerAccount.ShippingAddress == null ||
                                     string.IsNullOrEmpty(customerAccount.ShippingAddress.City)
                        ? string.Empty
                        : customerAccount.ShippingAddress.City;
                    viewModel.State = customerAccount.ShippingAddress == null ||
                                      string.IsNullOrEmpty(customerAccount.ShippingAddress.RegionDisplayName)
                        ? string.Empty
                        : customerAccount.ShippingAddress.RegionDisplayName;
                }
                model.Reviews.Add(viewModel);
            }
        }

        private int CalculateAverageRating(List<ProductReview> reviews)
        {
            var AverageRating = 3;
            var tempRating = 3.0;
            var sumRatings = 0.0;
            for (var i = 0; i <= reviews.Count - 1; i++)
            {
                sumRatings += (int) reviews[i].Rating;
            }
            if (sumRatings > 0)
            {
                tempRating = sumRatings/reviews.Count;
                AverageRating = (int) Math.Ceiling(tempRating);
            }

            return AverageRating;
        }

        public ActionResult TopReviews(int howMany, List<ProductReview> reviews)
        {
            var model = new ProductReviewsViewModel();

            // Trim List of reviews
            if (reviews != null)
            {
                if (reviews.Count > howMany)
                {
                    CopyReviews(model, reviews.Take(howMany).ToList());
                }
                else
                {
                    CopyReviews(model, reviews);
                }
            }

            ViewBag.Avg = CalculateAverageRating(reviews);

            return View(model);
        }

        [HccHttpPost]
        public ActionResult Create()
        {
            var result = string.Empty;
            var success = false;

            var productbvin = Request.Form["productbvin"];
            var newreview = Request.Form["newreview"];
            var rating = Request.Form["rating"];

            if (newreview.Trim().Length < 1)
            {
                result = Localization.GetString("EmptyReviewError");
            }
            else
            {
                var rev = new ProductReview
                {
                    ProductBvin = productbvin,
                    Karma = 0
                };

                rev.UserID = SessionManager.IsUserAuthenticated(HccApp) ? HccApp.CurrentCustomerId : "0";

                rev.Description = HttpUtility.HtmlEncode(newreview.Trim());
                rev.ReviewDateUtc = DateTime.UtcNow;
                rev.Rating = (ProductReviewRating) int.Parse(rating);

                //Auto approve if moderation disabled
                rev.Approved = !HccApp.CurrentStore.Settings.ProductReviewModerate;

                success = HccApp.CatalogServices.ProductReviews.Create(rev);

                if (success)
                {
                    result = HccApp.CurrentStore.Settings.ProductReviewModerate
                        ? Localization.GetString("ReviewsAreModerated")
                        : Localization.GetString("ThanksForReview");
                }
                else
                {
                    result = Localization.GetString("UnexpectedError");
                }
            }

            return Json(new {message = result, ok = success});
        }

        private void SetPageMetaData(Product prod)
        {
            var title = string.Empty;
            if (!string.IsNullOrWhiteSpace(prod.MetaTitle))
                title = prod.MetaTitle;
            else
                title = prod.ProductName;
            if (!string.IsNullOrWhiteSpace(title))
                PageTitle = "Reviews: " + title;

            if (!string.IsNullOrWhiteSpace(prod.MetaKeywords))
                PageKeywords = "reviews," + prod.MetaKeywords;
            if (!string.IsNullOrWhiteSpace(prod.MetaDescription))
                PageDescription = "Reviews: " + prod.MetaDescription;
        }
    }
}