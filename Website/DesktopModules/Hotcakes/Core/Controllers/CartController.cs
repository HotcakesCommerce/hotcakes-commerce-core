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
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Integration;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class CartController : BaseStoreController
    {
        private const string HCC_KEY = "hcc";

        private void AddSingleProduct(Product p, int quantity)
        {
            var qty = quantity;
            if (qty < 1)
            {
                qty = 1;
            }

            if (p != null)
            {
                var li = p.ConvertToLineItem(HccApp, qty);

                var cart = CurrentCart;
                if (cart == null)
                {
                    cart = HccApp.OrderServices.EnsureShoppingCart();
                }

                li.Quantity = qty;

                HccApp.AddToOrderWithCalculateAndSave(cart, li);
            }
        }

        private void CheckFreeItems(CartViewModel model)
        {
            if (model.CurrentOrder != null)
            {
                // Add warning logic( when product is out of stock. )
                var freeItem = model.CurrentOrder
                    .CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "outfreeitems");

                if (freeItem != null)
                {
                    var ids = freeItem.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                    if (ids.Any())
                    {
                        var prods = HccApp.CatalogServices.Products.FindManyWithCache(ids);
                        var value = string.Join(", ", prods.Select(p => p.ProductName + " (" + p.Sku + ")"));
                        var text = Localization.GetString("FreeProductIsOut");
                        var message = string.Format(text, value);
                        FlashWarning(message);
                    }
                }
                else
                {
                    var flag2 =
                        model.CurrentOrder.CustomProperties
                            .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "sendflag"); //Reset flag to send mail.

                    if (flag2 != null)
                    {
                        flag2.Value = "0";
                    }
                }

                //Check if promotion was applied otherwise remove free products from the cart.
                var flag = model.CurrentOrder
                    .CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "freePromotions");

                //Get Applied Free Promotions
                var freePromotions = new List<long>();
                if (flag != null)
                {
                    freePromotions =
                        flag.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => long.Parse(s))
                            .ToList();
                }

                var allItemsLength = model.CurrentOrder.Items.Count;
                var freeItemsCount = 0;
                var needUpdate = false;

                //Remove item from order if it has free promotion but that was not applied
                for (var i = 0; i < model.CurrentOrder.Items.Count; i++)
                {
                    var li = model.CurrentOrder.Items[i];

                    var freeQuantity = li.CustomProperties.GetProperty(HCC_KEY, "freeQuantity");

                    if (li.IsFreeItem && freeQuantity != "true")
                    {
                        var prodFreePromotionsHash = li.GetFreePromotions();
                        var removedItems = 0;

                        foreach (var prodProm in prodFreePromotionsHash.Keys)
                        {
                            if (!freePromotions.Contains(prodProm))
                            {
                                if (prodFreePromotionsHash[prodProm] + removedItems >= li.Quantity)
                                {
                                    model.CurrentOrder.Items.Remove(li);
                                    var lim = model.LineItems.FirstOrDefault(l => l.Item == li);

                                    if (lim != null)
                                    {
                                        model.LineItems.Remove(lim);
                                    }

                                    needUpdate = true;
                                    i--;
                                }
                                else
                                {
                                    removedItems += prodFreePromotionsHash[prodProm];
                                    li.RemovePromotionId(prodProm);
                                    needUpdate = true;
                                }
                            }
                        }

                        if (li.FreeQuantity == li.Quantity)
                        {
                            freeItemsCount++;
                        }
                    }
                }

                if (needUpdate)
                {
                    HccApp.OrderServices.Orders.Update(model.CurrentOrder);
                }

                if (freeItemsCount == allItemsLength) // Clear cart if we have just free products.
                {
                    model.CurrentOrder.Items.Clear();
                }

                if (model.CurrentOrder == null || model.CurrentOrder.Items == null ||
                    model.CurrentOrder.Items.Count == 0)
                {
                    model.CartEmpty = true;
                }
            }
        }

        private void UpdateCartCustomerInfo()
        {
            // TODO: Replace this with an event hook when the CMS has the Login/Logout event hooks added
            // This is a workaround to re-calculate the cart in the event that the user has logged in or logged off
            if (HccApp.CurrentCustomer != null && !string.IsNullOrEmpty(HccApp.CurrentCustomer.Bvin))
            {
                CurrentCart.UserID = HccApp.CurrentCustomer.Bvin;
                CurrentCart.UserEmail = HccApp.CurrentCustomer.Email;
            }
            else
            {
                CurrentCart.UserID = string.Empty;
                CurrentCart.UserEmail = string.Empty;
            }

            HccApp.CalculateOrderAndSave(CurrentCart);
        }

        private bool CheckForStockOnItems(CartViewModel model)
        {
            var result = HccApp.CheckForStockOnItems(model.CurrentOrder);
            if (result.Success)
            {
                return true;
            }
            FlashFailure(result.Message);
            return false;
        }

        public void ForwardToCheckout(CartViewModel model)
        {
            if (Request["paypalexpress"] != null && Request["paypalexpress"] == "true")
            {
                ForwardToPayPalExpress(model);
                return;
            }

            var c = new OrderTaskContext
            {
                UserId = HccApp.CurrentCustomerId,
                Order = model.CurrentOrder
            };

            if (Workflow.RunByName(c, WorkflowNames.VerifyOrderSize))
            {
                Response.Redirect(Url.RouteHccUrl(HccRoute.Checkout, null, Uri.UriSchemeHttps));
            }
            else
            {
                var customerMessageFound = false;
                foreach (var msg in c.Errors)
                {
                    EventLog.LogEvent(msg.Name, msg.Description, EventLogSeverity.Error);
                    if (msg.CustomerVisible)
                    {
                        customerMessageFound = true;
                        FlashFailure(msg.Description);
                    }
                }
                if (!customerMessageFound)
                {
                    EventLog.LogEvent("Verify Order Size Workflow", "Checkout failed but no errors were recorded.",
                        EventLogSeverity.Error);
                    FlashFailure(Localization.GetString("CheckoutFailed"));
                }
            }
        }

        public void ForwardToPayPalExpress(CartViewModel model)
        {
            // Save as Order
            var c = new OrderTaskContext
            {
                UserId = HccApp.CurrentCustomerId,
                Order = model.CurrentOrder
            };

            var checkoutFailed = false;
            
            if (!Workflow.RunByName(c, WorkflowNames.VerifyOrderSize))
            {
                checkoutFailed = true;
                var customerMessageFound = false;
                foreach (var msg in c.Errors)
                {
                    EventLog.LogEvent(msg.Name, msg.Description, EventLogSeverity.Error);
                    if (msg.CustomerVisible)
                    {
                        customerMessageFound = true;
                        FlashWarning(msg.Description);
                    }
                }
                if (!customerMessageFound)
                {
                    EventLog.LogEvent("Verify Order Size Workflow", "Checkout failed but no errors were recorded.",
                        EventLogSeverity.Error);
                    FlashWarning(Localization.GetString("CheckoutFailed"));
                }
            }

            if (!checkoutFailed)
            {
                c.Inputs.Add(HCC_KEY, "MethodId", PaymentMethods.PaypalExpressId);

                if (!Workflow.RunByName(c, WorkflowNames.ThirdPartyCheckoutSelected))
                {
                    EventLog.LogEvent("Paypal Express Checkout Failed", "Specific Errors to follow",
                        EventLogSeverity.Error);
                    foreach (var item in c.GetCustomerVisibleErrors())
                    {
                        FlashFailure(item.Description);
                    }
                }
            }
        }

        private List<ShippingRateDisplay> GetRates()
        {
            SortableCollection<ShippingRateDisplay> Rates;
            Rates = HccApp.OrderServices.FindAvailableShippingRates(CurrentCart);
            return Rates.ToList();
        }

        #region Main Cart Actions

        // GET: /Cart/
        [NonCacheableResponseFilter]
        public ActionResult Index()
        {
            var model = IndexSetup();
            HandleActionParams();
            CheckForQuickAdd();
            LoadCart(model);
            ValidateOrderCoupons();
            CheckFreeItems(model);

            if (ModuleContext != null && ModuleContext.Configuration.DesktopModule.ModuleName == "Hotcakes.Cart")
                HccApp.AnalyticsService.RegisterEvent(HccApp.CurrentCustomerId, ActionTypes.GoToCart, null);

            CheckForStockOnItems(model);
            return View(model);
        }

        // GET: /MiniCart/
        [NonCacheableResponseFilter]
        public ActionResult MiniCart()
        {
            var model = new MiniCartViewModel
            {
                CartBvin = (CurrentCart != null && string.IsNullOrEmpty(CurrentCart.bvin)) ? CurrentCart.bvin : string.Empty,
                TotalQuantity = CurrentCart?.Items?.Sum(x => x.Quantity) ?? 0
            };

            return View(model);
        }

        // POST: /MiniCartItems
        [ActionName("MiniCartItems")]
        [HccHttpPost]
        public JsonResult GetMiniCartItems()
        {
            var model = IndexSetup();
            HandleActionParams();
            CheckForQuickAdd();
            LoadCart(model);
            ValidateOrderCoupons();
            CheckFreeItems(model);
            CheckForStockOnItems(model);

            return Json(model);
        }

        // POST: /Cart/
        [ActionName("Index")]
        [HccHttpPost]
        public ActionResult IndexPost()
        {
            var model = IndexSetup();
            LoadCart(model);

            var intResult = CartIntegration.Create(HccApp).BeforeProceedToCheckout(HccApp, model);

            if (!intResult.IsAborted)
            {
                if (CheckForStockOnItems(model))
                {
                    ForwardToCheckout(model);
                }
            }
            else
            {
                FlashWarning(intResult.AbortMessage);
            }

            return View(model);
        }

        // POST: /Cart/BulkAdd
        [HccHttpPost]
        public ActionResult BulkAdd(FormCollection form)
        {
            if (form["bulkitem"] != null)
            {
                var allIds = form["bulkitem"];
                var ids = allIds.Split(',');
                foreach (var single in ids)
                {
                    var p = HccApp.CatalogServices.Products.FindWithCache(single);
                    if (p != null)
                    {
                        AddSingleProduct(p, 1);
                    }
                }
            }
            foreach (var key in form.AllKeys)
            {
                if (key.StartsWith("bulkqty"))
                {
                    var id = key.Substring(7, key.Length - 7);
                    var qtyString = form[key];
                    var qty = 1;
                    int.TryParse(qtyString, out qty);
                    if (qty >= 1)
                    {
                        var p = HccApp.CatalogServices.Products.FindWithCache(id);
                        if (p != null)
                        {
                            AddSingleProduct(p, qty);
                        }
                    }
                }
            }
            return Redirect(Url.RouteHccUrl(HccRoute.Cart));
        }

        #endregion

        #region AJAX actions

        [HccHttpPost]
        public ActionResult RemoveLineItem()
        {
            var lineItemId = long.Parse(Request["lineitemid"]);

            var lineItem = CurrentCart.Items.SingleOrDefault(li => li.Id == lineItemId);
            if (lineItem != null)
            {
                CurrentCart.Items.Remove(lineItem);
                HccApp.CalculateOrderAndSave(CurrentCart);
            }
            return Redirect(Url.RouteHccUrl(HccRoute.Cart));
        }

        [HccHttpPost]
        public ActionResult UpdateLineItem()
        {
            var lineItemId = long.Parse(Request["lineitemid"]);

            var lineItem = CurrentCart.Items.SingleOrDefault(li => li.Id == lineItemId);
            if (lineItem != null)
            {
                var product = lineItem.GetAssociatedProduct(HccApp);
                if (product != null)
                {
                    var minQuantity = Math.Max(lineItem.FreeQuantity, Math.Max(product.MinimumQty, 1));
                    var quantity = 0;
                    if (int.TryParse(Request["lineitemquantity"], out quantity))
                    {
                        if (quantity >= minQuantity)
                        {
                            lineItem.Quantity = quantity;

                            HccApp.CalculateOrderAndSave(CurrentCart);
                        }
                    }
                }
            }
            return Redirect(Url.RouteHccUrl(HccRoute.Cart));
        }

        [HccHttpPost]
        public ActionResult AddCoupon()
        {
            var code = Request["couponcode"] ?? string.Empty;
            CurrentCart.AddCouponCode(code.Trim());
            HccApp.CalculateOrderAndSave(CurrentCart);
            return Redirect(Url.RouteHccUrl(HccRoute.Cart));
        }

        [HccHttpPost]
        public ActionResult RemoveCoupon()
        {
            var couponid = Request["couponid"] ?? string.Empty;
            long tempid = 0;
            long.TryParse(couponid, out tempid);

            CurrentCart.RemoveCouponCode(tempid);
            HccApp.CalculateOrderAndSave(CurrentCart);

            return Redirect(Url.RouteHccUrl(HccRoute.Cart));
        }

        #endregion

        #region Index Setup

        private CartViewModel IndexSetup()
        {
            var model = new CartViewModel {KeepShoppingUrl = GetKeepShoppingLocation()};
            SetPayPalVisibility(model);
            return model;
        }

        private string GetKeepShoppingLocation()
        {
            var category = HccApp.CatalogServices.Categories.Find(SessionManager.CategoryLastId);
            if (category == null)
                category = new Category();

            var result = UrlRewriter.BuildUrlForCategory(new CategorySnapshot(category));
            return result;
        }

        private void SetPayPalVisibility(CartViewModel model)
        {
            var isRecurring = CurrentCart != null ? CurrentCart.IsRecurring : false;
            var enabledMethods = PaymentMethods.EnabledMethods(HccApp.CurrentStore, isRecurring);
            model.PayPalExpressAvailable = enabledMethods.Any(m => m.MethodId == PaymentMethods.PaypalExpressId);
        }

        private void HandleActionParams()
        {
            //1. AddSKU - this is the sku that should be added to the cart automatically
            //2. AddSKUQTY - this is the quantity of the provided sku that should be added.
            //3. CouponCode - the coupon that should automatically be applied, if valid.
            //4. RedirectToCheckout=True - redirects the user automatically to the checkout.
            if (ModuleContext == null || ModuleContext.Configuration.DesktopModule.ModuleName != "Hotcakes.Cart")
                return;

            var clearCart = (string) RouteData.Values["ClearCart"];
            var addSKU = (string) RouteData.Values["AddSKU"];
            var addSKUQTY = (string) RouteData.Values["AddSKUQTY"];
            var CouponCode = (string) RouteData.Values["CouponCode"];
            var RedirectToCheckout = (string) RouteData.Values["RedirectToCheckout"];

            if (!string.IsNullOrWhiteSpace(addSKU)
                || !string.IsNullOrWhiteSpace(addSKUQTY)
                || !string.IsNullOrWhiteSpace(CouponCode))
                HccApp.OrderServices.EnsureShoppingCart();

            if (!string.IsNullOrWhiteSpace(clearCart))
            {
                var clear = bool.Parse(clearCart);
                if (clear)
                    HccApp.ClearOrder(CurrentCart);
            }

            string[] SKUs = null;
            string[] SKUQTYs = null;
            if (!string.IsNullOrWhiteSpace(addSKU))
                SKUs = addSKU.Split(',');
            if (!string.IsNullOrWhiteSpace(addSKUQTY))
                SKUQTYs = addSKUQTY.Split(',');
            if (addSKU == null)
                return;
            if (SKUQTYs == null)
                SKUQTYs = new string[SKUs.Length];
            if (SKUQTYs != null && SKUs.Length != SKUQTYs.Length)
            {
                FlashFailure(Localization.GetString("ParamsCountMismatch"));
                return;
            }

            var errorsPresent = false;
            for (var i = 0; i < SKUs.Length; i++)
            {
                var sku = SKUs[i];
                var skuQty = SKUQTYs[i];
                var qty = 1;
                if (!string.IsNullOrEmpty(skuQty))
                {
                    if (!int.TryParse(skuQty, out qty))
                    {
                        errorsPresent = true;
                        FlashFailure(Localization.GetFormattedString("QuantityParsingError", skuQty));
                    }
                }

                var p = HccApp.CatalogServices.Products.FindBySku(sku);
                if (p != null)
                {
                    // TODO: check for products with options
                    AddSingleProduct(p, qty);
                }
                else
                {
                    errorsPresent = true;
                    FlashFailure(Localization.GetFormattedString("SKUNotExists", sku));
                }
            }

            if (!string.IsNullOrWhiteSpace(CouponCode))
            {
                CurrentCart.AddCouponCode(CouponCode.Trim());
                HccApp.CalculateOrderAndSave(CurrentCart);
            }

            if (!errorsPresent)
            {
                if (!string.IsNullOrWhiteSpace(RedirectToCheckout))
                {
                    var redirect = bool.Parse(RedirectToCheckout);
                    if (redirect)
                        Response.Redirect(Url.RouteHccUrl(HccRoute.Checkout));
                }
                Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
            }
        }

        private void CheckForQuickAdd()
        {
            if ((bool) (RouteData.Values["MiniCart"] ?? false))
            {
                return;
            }

            if (Request.QueryString["quickaddid"] != null
                || Request.QueryString["quickaddsku"] != null
                || Request.QueryString["multi"] != null)
                HccApp.OrderServices.EnsureShoppingCart();

            if (Request.QueryString["quickaddid"] != null)
            {
                var bvin = Request.QueryString["quickaddid"];
                var prod = HccApp.CatalogServices.Products.FindWithCache(bvin);
                if (prod != null)
                {
                    var quantity = 1;
                    if (Request.QueryString["quickaddqty"] != null)
                    {
                        var val = 0;
                        if (int.TryParse(Request.QueryString["quickaddqty"], out val))
                        {
                            quantity = val;
                        }
                    }
                    AddSingleProduct(prod, quantity);
                }
                Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
            }
            else if (Request.QueryString["quickaddsku"] != null)
            {
                var sku = Request.QueryString["quickaddsku"];
                var prod = HccApp.CatalogServices.Products.FindBySku(sku);
                if (prod != null)
                {
                    var quantity = 1;
                    if (Request.QueryString["quickaddqty"] != null)
                    {
                        var val = 0;
                        if (int.TryParse(Request.QueryString["quickaddqty"], out val))
                        {
                            quantity = val;
                        }
                    }
                    AddSingleProduct(prod, quantity);
                }
                Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
            }
            else if (Request.QueryString["multi"] != null)
            {
                var skus = Request.QueryString["multi"].Split(';');

                foreach (var s in skus)
                {
                    var skuparts = s.Split(':');
                    var newsku = skuparts[0];
                    var bvin = string.Empty;
                    var p = HccApp.CatalogServices.Products.FindBySku(newsku);
                    if (p != null)
                    {
                        if (p.Bvin.Trim().Length > 0)
                        {
                            var qty = 1;
                            if (skuparts.Length > 1)
                            {
                                int.TryParse(skuparts[1], out qty);
                            }
                            if (qty < 1)
                            {
                                qty = 1;
                            }
                            AddSingleProduct(p, qty);
                        }
                    }
                }
                Response.Redirect(Url.RouteHccUrl(HccRoute.Cart));
            }
        }

        private void LoadCart(CartViewModel model)
        {
            model.CurrentOrder = CurrentCart ?? new Order();

            if (CurrentCart == null || CurrentCart.Items == null || CurrentCart.Items.Count == 0)
            {
                model.CartEmpty = true;
                return;
            }
            model.Rates = GetRates();

            for (var i = CurrentCart.Items.Count - 1; i >= 0; i--)
            {
                var lineItem = CurrentCart.Items[i];
                var product = lineItem.GetAssociatedProduct(HccApp);
                if (product != null)
                {
                    var ci = new CartLineItemViewModel
                    {
                        Item = lineItem,
                        Product = product,
                        ShowImage = true,
                        ImageUrl = DiskStorage.ProductVariantImageUrlMedium(
                            HccApp, lineItem.ProductId,
                            product.ImageFileSmall,
                            lineItem.VariantId, Request.IsSecureConnection),
                        LinkUrl = UrlRewriter.BuildUrlForProduct(product,
                            new {lineItem.OrderBvin, LineItemId = lineItem.Id}),
                        HasDiscounts = lineItem.HasAnyDiscounts
                    };
                    
                    model.LineItems.Add(ci);
                }
                else
                {
                    CurrentCart.Items.RemoveAt(i);
                    HccApp.OrderServices.Orders.Update(CurrentCart);
                }
            }

            UpdateCartCustomerInfo();
        }

        private void ValidateOrderCoupons()
        {
            if (CurrentCart == null)
                return;

            var promotions = HccApp.MarketingServices.Promotions.FindAll();
            var couponQulificationTypeId = new Guid(PromotionQualificationBase.TypeIdOrderHasCoupon);

            var cartCoupons = CurrentCart.Coupons.Select(c => c.CouponCode).ToList();
            foreach (var coupon in cartCoupons)
            {
                var bestMatchIsMet = false;
                var bestMatchDoNotCombine = false;
                var bestMatchStatus = PromotionStatus.Unknown;

                foreach (var promotion in promotions)
                {
                    var couponQulifications = promotion.Qualifications
                        .Where(q => q.TypeId == couponQulificationTypeId)
                        .ToList();

                    var promotionCoupons =
                        couponQulifications.SelectMany(q => ((OrderHasCoupon) q).CurrentCoupons()).ToList();
                    var isMet = promotionCoupons.Contains(coupon);
                    if (!bestMatchIsMet && isMet)
                    {
                        bestMatchIsMet = isMet;
                        if (!bestMatchIsMet)
                        {
                            bestMatchStatus = promotion.GetStatus();
                            bestMatchDoNotCombine = promotion.DoNotCombine;
                        }
                        else
                        {
                            if (bestMatchStatus != PromotionStatus.Active &&
                                promotion.GetStatus() == PromotionStatus.Active)
                            {
                                bestMatchStatus = promotion.GetStatus();
                                bestMatchDoNotCombine = promotion.DoNotCombine;
                            }
                        }
                    }
                }

                if (!bestMatchIsMet)
                {
                    FlashInfo(Localization.GetFormattedString("CoponInvalid", coupon));
                }
                else if (bestMatchStatus != PromotionStatus.Active)
                {
                    FlashInfo(Localization.GetFormattedString("CouponInactive", coupon));
                }
                else if (bestMatchDoNotCombine)
                {
                    FlashInfo(Localization.GetFormattedString("CouponDoNotCombine", coupon));
                }
            }
        }

        #endregion
    }
}