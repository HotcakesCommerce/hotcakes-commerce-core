﻿#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DotNetNuke.Security;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Integration;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Web.Validation;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class ProductsController : BaseStoreController
    {
        private PortalSecurity security = new PortalSecurity();

        #region Internal declarations

        public class ProductValidateResponse
        {
            public bool IsValid = true;
            public string MediumImageUrl = string.Empty;
            public string Message = string.Empty;
            public ProductPrices Prices;
            public string Sku = string.Empty;
            public string StockMessage = string.Empty;
            public string OriginalImageUrl { get; set; }
        }

        #endregion

        #region Constants

        private const int MaxRelatedItemsToShow = 3;
        private const bool RelatedItemsIncludeAutoSuggestions = true;

        #endregion

        #region Properties

        public bool IsConcreteItemModule
        {
            get { return Convert.ToBoolean(RouteData.Values["isConcreteItemModule"]); }
        }

        private bool IsSaveLaterAction
        {
            get { return Request.Form["savelaterbutton"] != null; }
        }

        #endregion

        #region Public methods

        // GET: /{*slug}
        public ActionResult Index(string slug)
        {
            if (!string.IsNullOrEmpty(slug))
            {
                slug = Server.UrlDecode(slug);
            }

            var model = LoadProductModel(slug);

            IndexSetup(model);
            LoadSelections(model);
            InventoryCheck(model);

            LogProductViewActivity(model);

            var viewName = GetViewName(model);
            return View(viewName, model);
        }

        // POST: /{*slug}
        [HccHttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(string slug)
        {
            if (!string.IsNullOrEmpty(slug))
            {
                slug = Server.UrlDecode(slug);
            }

            var model = LoadProductModel(slug);

            IndexSetup(model);
            LoadFormValues(model);
            var result = InventoryCheck(model);

            if (result.Success)
            {
                if (IsSaveLaterAction)
                {
                    AddToWishlist(model);
                    return Redirect(Url.RouteHccUrl(HccRoute.WishList));
                }
                var intResult = ProductIntegration.Create(HccApp).BeforeProductAddedToCart(HccApp, model);

                if (!intResult.IsAborted)
                {
                    if (AddToCart(model))
                        return Redirect(Url.RouteHccUrl(HccRoute.Cart));
                    if (!string.IsNullOrEmpty(model.ValidationMessage))
                    {
                        FlashWarning(model.ValidationMessage);
                    }
                    else
                    {
                        FlashFailure(Localization.GetString("AddFailed"));
                    }
                }
                else
                {
                    FlashWarning(intResult.AbortMessage);
                }
            }
            else
            {
                FlashFailure(result.Message);
            }

            var viewName = GetViewName(model);
            return View(viewName, model);
        }

        [ChildActionOnly]
        public ActionResult RenderSingleProduct(Product product)
        {
            if (product == null)
            {
                return Content(string.Empty);
            }

            var model = new SingleProductViewModel(product, HccApp);
            return View("_RenderSingleProduct", model);
        }

        [HccHttpPost]
        public ActionResult Validate()
        {
            var productBvin = Request.Form["productbvin"];

            if (!string.IsNullOrEmpty(productBvin)) productBvin = security.InputFilter(productBvin.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var product = HccApp.CatalogServices.Products.FindWithCache(productBvin);
            var validateResult = new ProductValidateResponse();

            if (product != null)
            {
                var selections = ParseSelections(product, false);
                var price = HccApp.PriceProduct(product, HccApp.CurrentCustomer, selections, HccApp.CurrentlyActiveSales);

                ValidateSelections(validateResult, product, selections);
                ValidatePrice(validateResult, price);
                ValidateInventory(validateResult, product, selections);

                UpdateVariantImage(validateResult, product, price);

                if (validateResult != null)
                {
                    if (!string.IsNullOrEmpty(validateResult.MediumImageUrl))
                    {
                        if (HccApp.IsCurrentRequestSecure())
                        {
                            validateResult.MediumImageUrl = validateResult.MediumImageUrl.Replace("http", "https");
                        }
                    }

                    if (!string.IsNullOrEmpty(validateResult.OriginalImageUrl))
                    {
                        if (HccApp.IsCurrentRequestSecure())
                        {
                            validateResult.OriginalImageUrl = validateResult.OriginalImageUrl.Replace("http", "https");
                        }
                    }
                }
            }

            return new PreJsonResult(Web.Json.ObjectToJson(validateResult));
        }

        #endregion

        #region Implementation / Load product model

        private ProductPageViewModel LoadProductModel(string slug)
        {
            CustomUrl customUrl;
            var product = HccApp.ParseProductBySlug(slug, out customUrl);
            if (customUrl != null && !IsConcreteItemModule)
            {
                var redirectUrl = HccUrlBuilder.RouteHccUrl(HccRoute.Product, new {slug = customUrl.RedirectToUrl});
                if (customUrl.IsPermanentRedirect)
                    RedirectPermanent(redirectUrl);
                else
                    Redirect(redirectUrl);
            }
            if (product == null)
            {
                StoreExceptionHelper.ShowInfo(Localization.GetString("ProductNotFound"));
            }
            else if (product.Status != ProductStatus.Active)
            {
                StoreExceptionHelper.ShowInfo(Localization.GetString("ProductNotActive"));
            }
            else if (!HccApp.CatalogServices.TestProductAccess(product))
            {
                StoreExceptionHelper.ShowInfo(Localization.GetString("ProductNotEnoughPermission"));
            }

            var model = new ProductPageViewModel {LocalProduct = product};

            LoadImageUrls(model);
            model.Prices = CreateProductPrices(product);
            LoadRelatedItems(model);
            LoadBundledItems(model);
            model.IsAvailableForWishList = SessionManager.IsUserAuthenticated(HccApp);
            model.AllowReviews = product.AllowReviews.HasValue
                ? product.AllowReviews.Value
                : HccApp.CurrentStore.Settings.AllowProductReviews;
            LoadAlternateImages(model);
            model.PreRenderedTypeValues = product.RenderTypeProperties();
            model.SwatchHtml = ImageHelper.GenerateSwatchHtmlForProduct(product, HccApp);
            model.LineItemId = Request.QueryString["LineItemId"].ConvertToNullable<long>();

            // make the minimum quantity be the new default if necessary, otherwise use the actual default (1)
            if (product.MinimumQty > 0) model.Quantity = product.MinimumQty;

            LoadGiftCardAmounts(model);

            if (AuthorizedToEditCatalog())
            {
                model.ProductAnalyticsUrl = string.Format(ProductAnalyticsUrlFormat, product.Bvin);
                model.ProductEditUrl = string.Format(ProductEditUrlFormat, product.Bvin);
                model.StoreAdminUrl = DashboardAdminUrl;
                model.AuthorizedToEditCatalog = true;
            }
            else
            {
                model.AuthorizedToEditCatalog = false;
            }

            if (!string.IsNullOrEmpty(product.ManufacturerId))
            {
                var manufacturer = HccApp.ContactServices.Manufacturers.Find(product.ManufacturerId);
                if (manufacturer != null)
                {
                    model.ManufacturerName = manufacturer.DisplayName;
                }
            }

            if (!string.IsNullOrEmpty(product.VendorId))
            {
                var vendor = HccApp.ContactServices.Vendors.Find(product.VendorId);
                if (vendor != null)
                {
                    model.VendorName = vendor.DisplayName;
                }
            }

            return model;
        }

        private void LoadImageUrls(ProductPageViewModel model)
        {
            model.ImageUrls = new ProductImageUrls();
            model.ImageUrls.LoadProductImageUrls(HccApp, model.LocalProduct);
        }

        private ProductPrices CreateProductPrices(Product product)
        {
            var userPrice = HccApp.PriceProduct(product, HccApp.CurrentCustomer, null, HccApp.CurrentlyActiveSales);
            return new ProductPrices(userPrice);
        }

        private void LoadRelatedItems(ProductPageViewModel model)
        {
            var relatedItems
                = HccApp.CatalogServices.ProductRelationships.FindForProduct(model.LocalProduct.Bvin);

            if (relatedItems == null)
                relatedItems = new List<ProductRelationship>();

            var maxItems = relatedItems.Count;

            // we have fewer available than max to show
            if (relatedItems.Count < MaxRelatedItemsToShow)
            {
                if (RelatedItemsIncludeAutoSuggestions)
                {
                    // try to fill in auto suggestions
                    var toAuto = MaxRelatedItemsToShow - relatedItems.Count;
                    var autos = HccApp.GetAutoSuggestedRelatedItems(model.LocalProduct.Bvin, toAuto);
                    if (autos != null)
                    {
                        relatedItems.AddRange(autos);
                    }
                }

                maxItems = relatedItems.Count;
            }

            if (relatedItems.Count < 1)
                return;

            foreach (var relatedItem in relatedItems)
            {
                var related = HccApp.CatalogServices.Products.FindWithCache(relatedItem.RelatedProductId);
                if (related != null)
                {
                    var item = new SingleProductViewModel(related, HccApp);
                    if (item.Item.Status == ProductStatus.Active)
                    {
                        model.RelatedItems.Add(item);
                    }
                }
            }
        }

        private void LoadBundledItems(ProductPageViewModel model)
        {
            if (model.LocalProduct.IsBundle)
            {
                foreach (var bundledProductAdv in model.LocalProduct.BundledProducts)
                {
                    if (bundledProductAdv.BundledProduct == null)
                        continue;

                    var item = new BundledProductViewModel(bundledProductAdv, HccApp);
                    model.BundledItems.Add(item);
                }
            }
        }

        private void LoadAlternateImages(ProductPageViewModel model)
        {
            foreach (var prodImg in model.LocalProduct.Images)
            {
                var imgUrls = new ProductImageUrls();
                imgUrls.LoadAlternateImageUrls(HccApp, prodImg);

                model.AlternateImageUrls.Add(imgUrls);
            }

            if (model.AlternateImageUrls.Count > 0)
            {
                model.AlternateImageUrls.Insert(0, model.ImageUrls);
            }
        }

        private void LoadGiftCardAmounts(ProductPageViewModel model)
        {
            model.IsGiftCard = model.LocalProduct.IsGiftCard;

            if (model.IsGiftCard)
            {
                model.GiftCardPredefinedAmounts = new List<decimal>();
                var amounts = HccApp.CurrentStore.Settings.GiftCard.PredefinedAmounts.Split(',');
                foreach (var amount in amounts)
                {
                    decimal dec = 0;

                    if (decimal.TryParse(amount, out dec))
                    {
                        model.GiftCardPredefinedAmounts.Add(dec);
                    }
                }
            }
        }

        private SystemOperationResult InventoryCheck(ProductPageViewModel model)
        {
            var data = HccApp.CatalogServices.InventoryCheck(model.LocalProduct, model.Selections);
            model.StockMessage = data.InventoryMessage;
            model.IsAvailableForSale = data.IsAvailableForSale;

            var formQuantity = Request.Form["qty"];
            if (model.LocalProduct.IsUserSuppliedPrice)
                formQuantity = "1";

            if (!string.IsNullOrEmpty(formQuantity)) formQuantity = security.InputFilter(formQuantity.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var qty = 0;
            if (int.TryParse(formQuantity, out qty))
            {
                var li = ConvertProductToLineItem(model);
                li.Quantity = Convert.ToInt16(formQuantity);
                return HccApp.CheckForStockOnItems(li);
            }
            return new SystemOperationResult(false, Localization.GetString("EnterProperQty"));
        }

        #endregion

        #region Implementation / Index setup

        private void IndexSetup(ProductPageViewModel model)
        {
            ViewBag.IsLoggedIn = SessionManager.IsUserAuthenticated(HccApp);
            ViewBag.Avg = CalculateAverageRating(model.LocalProduct.ReviewsApproved);

            SetPageMetaData(model.LocalProduct);
            RegisterSocialFunctionality(model);
        }

        private static int CalculateAverageRating(List<ProductReview> reviews)
        {
            var averageRating = 3;
            var tempRating = 3.0;
            var sumRatings = 0.0;

            for (var i = 0; i <= reviews.Count - 1; i++)
            {
                sumRatings += (int) reviews[i].Rating;
            }

            if (sumRatings > 0)
            {
                tempRating = sumRatings/reviews.Count;
                averageRating = (int) Math.Ceiling(tempRating);
            }

            return averageRating;
        }

        private void RegisterSocialFunctionality(ProductPageViewModel model)
        {
            var socialService = HccApp.SocialService;
            socialService.SaveProductToJournal(model.LocalProduct);

            // Obsolete - may need to add a method to retrieve the social item in the future (if integrating directly into Evoq Social again)
            //model.SocialItem = socialService.GetProductSocialItem(model.LocalProduct);

            // Social Media Globals
            ViewBag.UseFaceBook = HccApp.CurrentStore.Settings.FaceBook.UseFaceBook;

            ViewBag.UseTwitter = HccApp.CurrentStore.Settings.Twitter.UseTwitter;
            ViewBag.TwitterHandle = HccApp.CurrentStore.Settings.Twitter.TwitterHandle;
            ViewBag.TwitterDefaultTweetText = HccApp.CurrentStore.Settings.Twitter.DefaultTweetText;

            ViewBag.UseGooglePlus = HccApp.CurrentStore.Settings.GooglePlus.UseGooglePlus;

            ViewBag.UsePinterest = HccApp.CurrentStore.Settings.Pinterest.UsePinterest;

            RenderFacebookMetaTags(model);
        }

        private void RenderFacebookMetaTags(ProductPageViewModel model)
        {
            if (ViewBag.UseFaceBook)
            {
                var faceBookAdmins = HccApp.CurrentStore.Settings.FaceBook.Admins;
                var faceBookAppId = HccApp.CurrentStore.Settings.FaceBook.AppId;
                var canonicalUrl = HccUrlBuilder.RouteHccUrl(HccRoute.Product, new { slug = model.LocalProduct.UrlSlug.ToLower() });

                var currencyInfo =
                    HccApp.GlobalizationServices.Countries.FindAllForCurrency()
                        .FirstOrDefault(c => c.CultureCode == HccApp.CurrentStore.Settings.CurrencyCultureCode);

                var sb = new StringBuilder();

                sb.AppendFormat(Constants.TAG_CANONICAL, canonicalUrl);
                sb.AppendFormat(Constants.TAG_OGTITLE, PageTitle);
                sb.Append(Constants.TAG_OGTYPE);
                sb.AppendFormat(Constants.TAG_OGURL, ViewBag.CurrentUrl);
                sb.AppendFormat(Constants.TAG_OGIMAGE, model.ImageUrls.MediumlUrl);
                sb.AppendFormat(Constants.TAG_IOGPRICEAMOUNT, model.Prices.SitePrice.Text);

                // TODO: Replace this with ISO 4217-3 currency code
                // 
                // This will require:
                // - Adding the necessary column to hcc_Countries table
                // - Updating the upgrade script to include the official currency codes for all out of the box currencies
                // - Adding the necessary column to DAL classes (updating EF)
                // - Adding the currency to the Settings > Countries admin view to edit
                // 
                // Documentation:
                //      https://developers.facebook.com/docs/payments/product
                //      https://developers.pinterest.com/rich_pins_product/
                //      http://www.nationsonline.org/oneworld/currencies.htm
                sb.AppendFormat(Constants.TAG_IOGPRICECURRENCY, currencyInfo.IsoAlpha3);

                sb.AppendFormat(Constants.TAG_OGSITENAME, ViewBag.StoreName);
                sb.AppendFormat(Constants.TAG_OGFBADMIN, faceBookAdmins);
                sb.AppendFormat(Constants.TAG_OGFBAPPID, faceBookAppId);

                RenderToHead("FaceBookMetaTags", sb.ToString());
            }
        }

        private void SetPageMetaData(Product prod)
        {
            if (!IsConcreteItemModule)
            {
                var title = string.Empty;
                if (!string.IsNullOrWhiteSpace(prod.MetaTitle))
                    title = prod.MetaTitle;
                else
                    title = prod.ProductName;
                if (!string.IsNullOrWhiteSpace(title))
                    PageTitle = title;

                if (!string.IsNullOrWhiteSpace(prod.MetaKeywords))
                    PageKeywords = prod.MetaKeywords;
                if (!string.IsNullOrWhiteSpace(prod.MetaDescription))
                    PageDescription = prod.MetaDescription;
            }
        }

        #endregion

        #region Implementation / Validate

        private void ValidateSelections(ProductValidateResponse validateResult, Product product,
            OptionSelections selections)
        {
            string message;

            if (!ValidateSelections(product, selections, out message))
            {
                validateResult.IsValid = false;
                validateResult.Message = message;
            }
        }

        private bool ValidateSelections(Product product, OptionSelections selections, out string message)
        {
            message = null;
            var result = product.ValidateSelections(selections);

            switch (result)
            {
                case ValidateSelectionsResult.RequiredOptionNotSelected:
                    message = Localization.GetString("RequiredOptions");
                    return false;
                case ValidateSelectionsResult.LabelsSelected:
                    message = Localization.GetString("AllSelectionsError");
                    return false;
                case ValidateSelectionsResult.OptionsNotAvailable:
                    message = Localization.GetString("OptionsNotAvailable");
                    return false;
            }

            return true;
        }

        private void ValidatePrice(ProductValidateResponse validateResult, UserSpecificPrice price)
        {
            validateResult.Prices = new ProductPrices(price);
            validateResult.Sku = price.Sku;
            validateResult.IsValid &= price.IsValid;
        }

        private void ValidateInventory(ProductValidateResponse validateResult, Product product,
            OptionSelections selections)
        {
            // Make sure we have stock on the product or variant
            var data = HccApp.CatalogServices.InventoryCheck(product, selections);

            validateResult.StockMessage = data.InventoryMessage;
            validateResult.IsValid &= data.IsAvailableForSale;
        }

        private void UpdateVariantImage(ProductValidateResponse validateResult, Product product, UserSpecificPrice price)
        {
            if (!string.IsNullOrEmpty(price.VariantId))
            {
                validateResult.MediumImageUrl = DiskStorage.ProductVariantImageUrlMedium(HccApp, product.Bvin,
                    product.ImageFileMedium, price.VariantId, false);
                validateResult.OriginalImageUrl = DiskStorage.ProductVariantImageUrlOriginal(HccApp, product.Bvin,
                    product.ImageFileMedium, price.VariantId, false);
            }
            else
            {
                validateResult.MediumImageUrl = DiskStorage.ProductImageUrlMedium(HccApp, product.Bvin,
                    product.ImageFileMedium, false);
                validateResult.OriginalImageUrl = DiskStorage.ProductImageUrlOriginal(HccApp, product.Bvin,
                    product.ImageFileMedium, false);
            }
        }

        #endregion

        #region Implementation

        private void LoadSelections(ProductPageViewModel model)
        {
            if (CurrentCart != null && model.LineItemId.HasValue)
            {
                var li = CurrentCart.Items.SingleOrDefault(y => y.Id == model.LineItemId.Value);
                if (li != null)
                {
                    model.Quantity = li.Quantity;
                    model.Selections = li.SelectionData;
                }
            }
        }

        private void LogProductViewActivity(ProductPageViewModel model)
        {
            PersonalizationServices.RecordProductViews(model.LocalProduct.Bvin, HccApp);

            if (!IsConcreteItemModule)
                HccApp.AnalyticsService.RegisterEvent(HccApp.CurrentCustomerId, ActionTypes.ProductViewed,
                    model.LocalProduct.Bvin);
        }

        private void LoadFormValues(ProductPageViewModel model)
        {
            model.Selections = ParseSelections(model.LocalProduct, true);
            model.GiftCardRecEmail = Request.Form["GiftCardRecEmail"];
            model.GiftCardRecName = Request.Form["GiftCardRecName"];
            model.GiftCardMessage = Request.Form["GiftCardMessage"];

            if (!string.IsNullOrEmpty(model.GiftCardRecEmail)) model.GiftCardRecEmail = security.InputFilter(model.GiftCardRecEmail.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(model.GiftCardRecName)) model.GiftCardRecName = security.InputFilter(model.GiftCardRecName.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(model.GiftCardMessage)) model.GiftCardMessage = security.InputFilter(model.GiftCardMessage.Trim(), PortalSecurity.FilterFlag.NoMarkup);
        }

        private void AddToWishlist(ProductPageViewModel model)
        {
            var quantity = 1;
            var isValidQty = DetermineQuantityToAdd(model);
            if (isValidQty)
                quantity = model.Quantity;

            HccApp.CatalogServices.SaveProductToWishList(model.LocalProduct, model.Selections, quantity, HccApp);
        }

        private bool AddToCart(ProductPageViewModel model)
        {
            var isValidSelections = ValidateSelections(model);
            var isValidQty = DetermineQuantityToAdd(model);
            var isValidUserPrice = ValidateUserPrice(model);
            var isValidGiftCardPrice = ValidateGiftCardAmount(model);
            var isNotRecurringMixed = ValidateRecurringItems(model);

            if (isValidSelections &&
                isValidQty &&
                isValidUserPrice &&
                isValidGiftCardPrice &&
                isNotRecurringMixed)
            {
                var li = ConvertProductToLineItem(model);
                var currentCart = HccApp.OrderServices.EnsureShoppingCart();

                RemovePreviousLineItem(model, currentCart);
                return HccApp.AddToOrderWithCalculateAndSave(currentCart, li);
            }

            return false;
        }

        private bool DetermineQuantityToAdd(ProductPageViewModel model)
        {
            var validQuantity = true;
            var quantity = 0;
            var formQuantity = Request.Form["qty"];

            if (!string.IsNullOrEmpty(formQuantity)) formQuantity = security.InputFilter(formQuantity.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var minQuantity = model.LocalProduct.MinimumQty > 0 ? model.LocalProduct.MinimumQty : 1;
            if (int.TryParse(formQuantity, out quantity))
            {
                if (quantity < minQuantity)
                {
                    FlashWarning(Localization.GetFormattedString("ProductMinimumQty", minQuantity));
                    quantity = minQuantity;
                    validQuantity = false;
                }
            }
            else
            {
                quantity = minQuantity;
            }

            model.Quantity = quantity;
            return validQuantity;
        }

        private bool ValidateSelections(ProductPageViewModel model)
        {
            string message;

            if (!ValidateSelections(model.LocalProduct, model.Selections, out message))
            {
                model.ValidationMessage = message;
                return false;
            }

            return true;
        }

        private bool ValidateUserPrice(ProductPageViewModel model)
        {
            var valid = true;

            if (model.LocalProduct.IsUserSuppliedPrice)
            {
                var strUserPrice = Request.Form["userprice"];

                if (!string.IsNullOrEmpty(strUserPrice)) strUserPrice = security.InputFilter(strUserPrice.Trim(), PortalSecurity.FilterFlag.NoMarkup);

                decimal userPrice = 0;
                if (decimal.TryParse(strUserPrice, out userPrice))
                {
                    model.UserSuppliedPrice = Money.RoundCurrency(userPrice);
                }

                if (model.UserSuppliedPrice <= 0)
                {
                    valid = false;
                    FlashWarning(Localization.GetString("UserPriceInvalid"));
                }
                if (model.UserSuppliedPrice > WebAppSettings.ProductMaxPrice)
                {
                    valid = false;
                    FlashWarning(Localization.GetString("UserPriceTooBig"));
                }
            }

            return valid;
        }

        private bool ValidateGiftCardAmount(ProductPageViewModel model)
        {
            var valid = true;

            if (model.IsGiftCard)
            {
                var gcSetting = HccApp.CurrentStore.Settings.GiftCard;

                var strAmount = Request.Form["giftcardamount"];

                if (!string.IsNullOrEmpty(strAmount)) strAmount = security.InputFilter(strAmount.Trim(), PortalSecurity.FilterFlag.NoMarkup);

                decimal gcAmount = 0;

                if (decimal.TryParse(strAmount, out gcAmount))
                {
                    model.GiftCardAmount = Money.RoundCurrency(gcAmount);
                    if (model.GiftCardAmount < gcSetting.MinAmount)
                    {
                        valid = false;
                        model.ValidationMessage = Localization.GetString("GiftCardAmountTooSmall")
                            .Replace("{0}", gcSetting.MinAmount.ToString());
                    }
                    if (model.GiftCardAmount > gcSetting.MaxAmount)
                    {
                        valid = false;
                        model.ValidationMessage = Localization.GetString("GiftCardAmountTooBig")
                            .Replace("{0}", gcSetting.MaxAmount.ToString());
                    }
                }
                else
                {
                    valid = false;
                    model.ValidationMessage = Localization.GetString("GiftCardAmountInvalid");
                }

                if (valid && !string.IsNullOrEmpty(model.GiftCardRecEmail))
                {
                    if (
                        !Regex.IsMatch(model.GiftCardRecEmail, ValidationHelper.EmailValidationRegex,
                            RegexOptions.CultureInvariant))
                    {
                        valid = false;
                        model.ValidationMessage = Localization.GetString("GiftCardEmailInvalid");
                    }
                }
            }

            return valid;
        }

        private bool ValidateRecurringItems(ProductPageViewModel model)
        {
            if (CurrentCart != null && CurrentCart.Items.Count > 0)
            {
                if (model.LocalProduct.IsRecurring != CurrentCart.IsRecurring)
                {
                    model.ValidationMessage = Localization.GetString("YouCannotMixRecurringLineItems");
                    return false;
                }
            }

            return true;
        }

        private LineItem ConvertProductToLineItem(ProductPageViewModel model)
        {
            var userPrice = model.LocalProduct.IsUserSuppliedPrice ? (decimal?) model.UserSuppliedPrice : null;

            var li = model.LocalProduct.ConvertToLineItem(HccApp, model.Quantity, model.Selections, userPrice);

            if (model.IsGiftCard)
            {
                li.CustomPropGiftCardEmail = model.GiftCardRecEmail;
                li.CustomPropGiftCardName = model.GiftCardRecName;
                li.CustomPropGiftCardMessage = model.GiftCardMessage;
                li.BasePricePerItem = model.GiftCardAmount;
            }

            return li;
        }

        private static void RemovePreviousLineItem(ProductPageViewModel model, Order cart)
        {
            if (model.LineItemId.HasValue)
            {
                var lineItemId = model.LineItemId.Value;
                var toRemove = cart.Items.SingleOrDefault(y => y.Id == lineItemId);
                if (toRemove != null) cart.Items.Remove(toRemove);
            }
        }

        private OptionSelections ParseSelections(Product product, bool allOptions)
        {
            var result = new OptionSelections();

            if (!product.IsBundle)
            {
                foreach (var opt in product.Options)
                {
                    if (!allOptions)
                    {
                        if (opt.OptionType == OptionTypes.Html ||
                            opt.OptionType == OptionTypes.TextInput)
                        {
                            continue;
                        }
                    }
                    var selected = opt.ParseFromForm(Request.Form);
                    if (selected != null)
                        result.OptionSelectionList.Add(selected);
                }
            }
            else
            {
                foreach (var bundledProductAdv in product.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;
                    foreach (var opt in bundledProduct.Options)
                    {
                        if (!allOptions)
                        {
                            if (opt.OptionType == OptionTypes.Html ||
                                opt.OptionType == OptionTypes.TextInput)
                            {
                                continue;
                            }
                        }

                        var selected = opt.ParseFromForm(Request.Form, bundledProductAdv.Id.ToString());
                        if (selected != null)
                            result.AddBundleSelections(bundledProductAdv.Id, selected);
                    }
                }
            }

            return result;
        }

        private string GetViewName(ProductPageViewModel model)
        {
            var productType = HccApp.CatalogServices.ProductTypes.Find(model.LocalProduct.ProductTypeId);
            var productTypeTemplateName = productType != null ? productType.TemplateName : string.Empty;
            string[] viewNames = {ModuleViewName, productTypeTemplateName, model.LocalProduct.TemplateName};
            if (IsConcreteItemModule)
                return viewNames.FirstOrDefault(view => !string.IsNullOrWhiteSpace(view));
            return viewNames.LastOrDefault(view => !string.IsNullOrWhiteSpace(view));
        }

        #endregion
    }
}