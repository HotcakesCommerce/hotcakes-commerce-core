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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.BusinessRules.OrderTasks;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class ReceiveFreeProductAdjustment : PromotionActionBase
    {
        public const string TypeIdString = "a000c557-9e29-47cc-a1f8-7de40b19f48c";

        private const string HCC_KEY = "hcc";
        private const string ZERO = "0";
        private const string ONE = "1";

        #region Constructor

        public ReceiveFreeProductAdjustment()
        {
            Settings = new Dictionary<string, string>();
        }

        #endregion

        #region Properties

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        #endregion

        #region Public methods

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = string.Empty;
            var products = GetQuantities();

            result = "Receive Free Product<ul>";

            foreach (var bvin in products.Keys)
            {
                var p = app.CatalogServices.Products.FindWithCache(bvin);

                if (p != null)
                {
                    result += string.Format("<li>[{0}]{1} ({2})</li>", p.Sku, p.ProductName, products[bvin]);
                }
            }

            result += "</ul>";

            return result;
        }

        public void AddItemId(string itemid, int quantity)
        {
            var _ItemIds = GetQuantities();

            var possible = itemid.Trim().ToUpperInvariant();
            if (possible == string.Empty) return;

            if (_ItemIds.ContainsKey(possible))
            {
                _ItemIds[possible] = _ItemIds[possible] + quantity;
            }

            _ItemIds.Add(possible, quantity);

            SaveQuantitiesToSettings(_ItemIds);
        }

        public void RemoveItemId(string itemid)
        {
            var _ItemIds = GetQuantities();

            if (_ItemIds.ContainsKey(itemid.Trim().ToUpperInvariant()))
            {
                _ItemIds.Remove(itemid.Trim().ToUpperInvariant());

                SaveQuantitiesToSettings(_ItemIds);
            }
        }

        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.Mode != PromotionType.OfferForFreeItems) return false;

            if (CheckSuccessfullFlag(context))
            {
                return true; // It means that free product has been already applied
            }

            var products = GetQuantities();
            var outProductIds = new List<LineItem>();
            var needToClearCart = true;
            foreach (var item in products)
            {
                var prod = context.HccApp.CatalogServices.Products.FindWithCache(item.Key);
                var li = prod.ConvertToLineItem(context.HccApp, item.Value);
                var IsFreeQuantity = false;
                var freeQtyFlag = context.CurrentlyProcessingLineItem.CustomProperties.GetProperty(HCC_KEY, "freeQuantity");

                if (li.ProductId.ToUpperInvariant() == context.CurrentlyProcessingLineItem.ProductId.ToUpperInvariant() &&
                    li.SelectionData.Equals(context.CurrentlyProcessingLineItem.SelectionData) && freeQtyFlag != "false")
                {
                    IsFreeQuantity = true;
                }

                var added = false;
                added = AddFreeItemToCart(context, li, prod, outProductIds, IsFreeQuantity);

                if (!added)
                {
                    var orderli =
                        context.Order.Items
                            .FirstOrDefault(i => i.ProductId.ToUpperInvariant() == li.ProductId.ToUpperInvariant());
                    if (orderli != null && orderli.Quantity != li.Quantity)
                    {
                        needToClearCart = false;
                    }
                }
            }

            if (outProductIds.Any())
            {
                if (needToClearCart)
                {
                    CannotAddProductsNotification(context, outProductIds);
                    CleanItemFromCart(context, products);
                    RemoveSuccessfullFlag(context);
                }
                else
                {
                    CannotAddProducts(context, outProductIds);
                    AddSuccessfullFlag(context);
                }
                return false;
            }
            ResetOrderFlags(context);
            AddSuccessfullFlag(context);

            return true;
        }

        public override bool CancelAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.Order == null) return false;

            //ResetOrderFlags(context);

            var products = GetQuantities();
            CleanCart(context, GetQuantities());

            return true;
        }

        public Dictionary<string, int> GetQuantities()
        {
            var result = new List<string>();
            var all = GetSetting("quantityids");
            result = all
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToUpperInvariant())
                .ToList();

            var hash = new Dictionary<string, int>(result.Count);

            foreach (var item in result)
            {
                var parts = item.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                var id = parts[0];
                var quantity = int.Parse(parts[1]);

                hash.Add(id, quantity);
            }

            return hash;
        }

        public void SaveQuantitiesToSettings(Dictionary<string, int> items)
        {
            var all = string.Join(",", items.Select(s => s.Key.Trim().ToUpperInvariant() + "=" + s.Value));

            SetSetting("quantityids", all);
        }

        #endregion

        #region Helper Methods

        private void CleanItemFromCart(PromotionContext context, Dictionary<string, int> products)
        {
            var needUpdate = false;

            foreach (var bvin in products.Keys)
            {
                var li = context.Order.Items.FirstOrDefault(i => i.ProductId.ToUpperInvariant() == bvin);

                if (li != null)
                {
                    var freeCount = li.GetFreeCountByPromotionId(context.PromotionId);

                    if (freeCount != -1)
                    {
                        needUpdate = true;
                        context.Order.Items.Remove(li);
                    }
                }
            }

            if (needUpdate)
            {
                context.HccApp.OrderServices.Orders.Update(context.Order);
            }
        }

        private void CleanCart(PromotionContext context, Dictionary<string, int> products)
        {
            var needUpdate = false;

            foreach (var bvin in products.Keys)
            {
                var li = context.Order.Items.FirstOrDefault(i => i.ProductId.ToUpperInvariant() == bvin);

                if (li != null)
                {
                    var freeCount = li.GetFreeCountByPromotionId(context.PromotionId);

                    if (freeCount != -1)
                    {
                        needUpdate = true;

                        li.RemovePromotionId(context.PromotionId);
                        li.CustomProperties.SetProperty(HCC_KEY, "freeQuantity", "false");
                        var disc =
                            li.DiscountDetails.FirstOrDefault(s => s.DiscountType == PromotionType.OfferForFreeItems &&
                                    s.PromotionId == context.PromotionId);
                        li.DiscountDetails.Remove(disc);
                    }
                }
            }

            if (needUpdate)
            {
                context.HccApp.OrderServices.Orders.Update(context.Order);
            }
        }

        private bool AddFreeItemToCart(PromotionContext context, LineItem listItem, Product prod,
            List<LineItem> outProducts, bool FreeQuantity)
        {
            var wasAdded = false;
            var productInCartList =
                context.Order.Items.Where(i => i.ProductId.ToUpperInvariant() == listItem.ProductId.ToUpperInvariant())
                    .ToList();
            var addProd = false;

            if (productInCartList != null && productInCartList.Any())
            {
                foreach (var productInCart in productInCartList)
                {
                    var areEqual = listItem.SelectionData.Equals(productInCart.SelectionData);

                    if (areEqual && productInCart.GetFreeCountByPromotionId(context.PromotionId) == listItem.Quantity)
                    {
                        if (
                            !productInCart.DiscountDetails.Any(s => s.DiscountType == PromotionType.OfferForFreeItems &&
                                    s.PromotionId == context.PromotionId))
                        {
                            productInCart.DiscountDetails.Add(new DiscountDetail
                            {
                                Amount = -listItem.BasePricePerItem*listItem.Quantity,
                                DiscountType = PromotionType.OfferForFreeItems,
                                Description = context.CustomerDescription,
                                PromotionId = context.PromotionId,
                                ActionId = Id
                            });

                            if (FreeQuantity)
                            {
                                productInCart.CustomProperties.SetProperty(HCC_KEY, "freeQuantity", "true");
                            }
                        }
                        if (
                            !CheckProduct(context, prod, productInCart,
                                Math.Max(productInCart.Quantity, listItem.Quantity)))
                        {
                            outProducts.Add(listItem);

                            return false;
                        }

                        wasAdded = true;
                        break;
                    }

                    if (areEqual)
                    {
                        if (listItem.IsUserSuppliedPrice)
                        {
                            if (productInCart.BasePricePerItem == listItem.BasePricePerItem)
                            {
                                addProd = true;
                            }
                        }
                        else
                        {
                            addProd = true;
                        }

                        if (addProd)
                        {
                            productInCart.DiscountDetails.Add(new DiscountDetail
                            {
                                Amount = -listItem.BasePricePerItem*listItem.Quantity,
                                DiscountType = PromotionType.OfferForFreeItems,
                                Description = context.CustomerDescription,
                                PromotionId = context.PromotionId,
                                ActionId = Id
                            });
                            productInCart.AddPromotionId(context.PromotionId, listItem.Quantity);

                            var IsQuantity = true;
                            if (FreeQuantity)
                            {
                                productInCart.PromotionIds = "";
                                var noOfFreeItem = productInCart.CustomProperties.GetProperty(HCC_KEY, "freeNoOfQuantity");
                                var noOfItem = productInCart.CustomProperties.GetProperty(HCC_KEY, "NoOfQuantity");
                                var isFreeQty = productInCart.CustomProperties.GetProperty(HCC_KEY, "freeQuantity");
                                if (productInCart.Quantity <= productInCart.FreeQuantity ||
                                    (productInCart.Quantity.ToString() == noOfItem &&
                                     productInCart.FreeQuantity.ToString() == noOfFreeItem) ||
                                    string.IsNullOrEmpty(isFreeQty))
                                {
                                    productInCart.CustomProperties.SetProperty(HCC_KEY, "freeNoOfQuantity",
                                        productInCart.FreeQuantity);
                                    productInCart.CustomProperties.SetProperty(HCC_KEY, "NoOfQuantity",
                                        productInCart.Quantity);

                                    IsQuantity = false;
                                    productInCart.Quantity += productInCart.FreeQuantity;
                                }
                                productInCart.CustomProperties.SetProperty(HCC_KEY, "freeQuantity", "true");
                            }

                            if (IsQuantity)
                            {
                                productInCart.Quantity = Math.Max(productInCart.Quantity, productInCart.FreeQuantity);
                            }

                            wasAdded = true;

                            if (
                                !CheckProduct(context, prod, productInCart,
                                    Math.Max(productInCart.Quantity, listItem.Quantity)))
                            {
                                outProducts.Add(listItem);
                                return false;
                            }
                        }
                    }
                }
            }

            if (!wasAdded)
            {
                listItem.DiscountDetails.Add(new DiscountDetail
                {
                    Amount = -listItem.BasePricePerItem*listItem.Quantity,
                    DiscountType = PromotionType.OfferForFreeItems,
                    Description = context.CustomerDescription,
                    PromotionId = context.PromotionId,
                    ActionId = Id
                });
                listItem.AddPromotionId(context.PromotionId, listItem.Quantity);


                context.Order.IsAbandonedEmailSent = false;
                context.Order.TimeOfOrderUtc = DateTime.UtcNow;
                context.HccApp.OrderServices.AddItemToOrder(context.Order, listItem);

                context.HccApp.OrderServices.Orders.Upsert(context.Order);

                wasAdded = true;
                if (!CheckProduct(context, prod, listItem, listItem.Quantity))
                {
                    outProducts.Add(listItem);
                    return false;
                }
            }

            return wasAdded;
        }

        private bool CheckProduct(PromotionContext context, Product prod, LineItem li, int mainQuantity)
        {
            InventoryCheckData check = null;

            if (!prod.IsBundle)
            {
                var prodQuantity = CalculateProductQuantity(context, prod, li, mainQuantity);
                check = context.HccApp.CatalogServices.SimpleProductInventoryCheck(prod,
                    li.SelectionData.OptionSelectionList, prodQuantity);

                return check.IsInStock;
            }
            var results = new List<InventoryCheckData>();

            foreach (var bundledProductAdv in prod.BundledProducts)
            {
                var bundledProduct = bundledProductAdv.BundledProduct;
                if (bundledProduct == null)
                    continue;

                var optionSelection = li.SelectionData.GetSelections(bundledProductAdv.Id);
                var quantity = bundledProductAdv.Quantity*mainQuantity;
                var prodQuantity = CalculateProductQuantity(context, bundledProductAdv, optionSelection, quantity);
                var singleResult = context.HccApp.CatalogServices.SimpleProductInventoryCheck(bundledProduct,
                    optionSelection, prodQuantity);

                if (!singleResult.IsInStock)
                {
                    return false;
                }
            }

            return true;
        }

        private int CalculateProductQuantity(PromotionContext context, BundledProductAdv prod, OptionSelectionList osl,
            int mainQuantity)
        {
            var quantity = mainQuantity;

            foreach (var item in context.Order.Items)
            {
                if (item.IsBundle)
                {
                    var p = context.HccApp.CatalogServices.Products.FindWithCache(item.ProductId);

                    var prods =
                        p.BundledProducts.Where(
                            pr =>
                                pr.BundledProduct != null &&
                                pr.BundledProduct.Bvin.ToLowerInvariant() == prod.ProductId.ToLowerInvariant()).ToList();

                    foreach (var bundledProd in prods)
                    {
                        var options = item.SelectionData.GetSelections(bundledProd.Id);

                        if (osl.Equals(options))
                        {
                            quantity = Math.Max(quantity, bundledProd.Quantity);
                        }
                    }
                }
                else
                {
                    if (item.ProductId.ToLowerInvariant() == prod.BundledProduct.Bvin.ToLowerInvariant() &&
                        item.SelectionData.OptionSelectionList.Equals(osl))
                    {
                        quantity += item.Quantity;
                    }
                }
            }
            return quantity;
        }

        private int CalculateProductQuantity(PromotionContext context, Product prod, LineItem li, int mainQuantity)
        {
            var quantity = mainQuantity;

            foreach (var item in context.Order.Items)
            {
                if (item.IsBundle)
                {
                    var p = context.HccApp.CatalogServices.Products.FindWithCache(item.ProductId);

                    var prods =
                        p.BundledProducts.Where(
                            pr => pr.BundledProductId.ToLowerInvariant() == li.ProductId.ToLowerInvariant()).ToList();

                    foreach (var bundledProd in prods)
                    {
                        var options = item.SelectionData.GetSelections(bundledProd.Id);

                        if (li.SelectionData.OptionSelectionList.Equals(options))
                        {
                            quantity += bundledProd.Quantity;
                        }
                    }
                }
                else
                {
                    if (item.ProductId.ToLowerInvariant() == li.ProductId.ToLowerInvariant() &&
                        item.SelectionData.Equals(li.SelectionData))
                    {
                        quantity = Math.Max(quantity, item.Quantity);
                    }
                }
            }
            return quantity;
        }

        private void CannotAddProductsNotification(PromotionContext context, List<LineItem> productIds)
        {
            var flag =
                context.Order.CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "sendflag");

            if (flag == null || flag.Value != ONE)
            {
                var itemF = new CustomProperty(HCC_KEY, "sendflag", "1");
                context.Order.CustomProperties.Add(itemF);

                foreach (var product in productIds)
                {
                    var note = new OrderNote();
                    note.IsPublic = false;
                    note.Note = string.Format(GlobalLocalization.GetString("SkippingReceiveFreeProduct"),
                        product.ProductName, product.ProductSku);
                    context.Order.Notes.Add(note);
                }

                // sent mail to the store
                var epio = new EmailProductIsOut();
                epio.Execute(context.HccApp, context.Order);
            }
        }

        private void CannotAddProducts(PromotionContext context, List<LineItem> productIds)
        {
            var flag =
                context.Order.CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "sendflag");

            if (flag == null || flag.Value != ONE)
            {
                var itemF = new CustomProperty(HCC_KEY, "sendflag", ONE);
                context.Order.CustomProperties.Add(itemF);

                foreach (var product in productIds)
                {
                    var note = new OrderNote();
                    note.IsPublic = false;
                    note.Note = string.Format(GlobalLocalization.GetString("SkippingReceiveFreeProduct"),
                        product.ProductName, product.ProductSku);
                    context.Order.Notes.Add(note);
                }

                // sent mail to the store
                var epio = new EmailProductIsOut();
                epio.Execute(context.HccApp, context.Order);
            }

            var skus = string.Join(",", productIds.Select(p => p.ProductId).ToList());
            var item =
                context.Order.CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "outfreeitems");

            if (item == null)
            {
                item = new CustomProperty(HCC_KEY, "outfreeitems", skus);
                context.Order.CustomProperties.Add(item);
            }
            else
            {
                item.Value = skus;
            }
        }

        private void ResetOrderFlags(PromotionContext context)
        {
            var flag =
                context.Order.CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "sendflag");
            var item =
                context.Order.CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "outfreeitems");

            if (flag != null)
            {
                flag.Value = ZERO;
            }

            if (item != null)
            {
                item.Value = string.Empty;
            }
        }

        private bool CheckSuccessfullFlag(PromotionContext context)
        {
            var flag = context
                .Order
                .CustomProperties
                .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "freePromotions");

            if (flag == null)
            {
                return false;
            }
            var list = flag.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            return list.Contains(context.PromotionId.ToString());
        }

        private void AddSuccessfullFlag(PromotionContext context)
        {
            var flag = context
                .Order
                .CustomProperties
                .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "freePromotions");

            if (flag == null)
            {
                var itemF = new CustomProperty(HCC_KEY, "freePromotions", context.PromotionId.ToString());
                context.Order.CustomProperties.Add(itemF);
            }
            else
            {
                var list = flag.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (!list.Contains(context.PromotionId.ToString()))
                {
                    list.Add(context.PromotionId.ToString());
                    flag.Value = string.Join(",", list);
                }
            }
        }

        private void RemoveSuccessfullFlag(PromotionContext context)
        {
            var flag = context
                .Order
                .CustomProperties
                .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "freePromotions");

            if (flag != null)
            {
                var list = flag.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (list.Contains(context.PromotionId.ToString()))
                {
                    list.Remove(context.PromotionId.ToString());
                    flag.Value = string.Join(",", list);
                }
            }
        }

        #endregion
    }
}