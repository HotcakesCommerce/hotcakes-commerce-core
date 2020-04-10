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
using System.Web;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Metrics;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Scheduling;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce
{
    public class HotcakesApplication
    {
        private List<Promotion> _CurrentlyActiveSales;

        private WorkflowFactory _WorkflowFactory;

        public HotcakesApplication(HccRequestContext hccContext)
        {
            CurrentRequestContext = hccContext;
        }

        public WorkflowFactory WorkflowFactory
        {
            get
            {
                return _WorkflowFactory ??
                       (_WorkflowFactory = Factory.CreateWorklflowFactory(CurrentRequestContext));
            }
        }

        public static HotcakesApplication Current
        {
            get { return new HotcakesApplication(HccRequestContext.Current); }
        }

        public HccRequestContext CurrentRequestContext { get; set; }

        public Store CurrentStore
        {
            get { return CurrentRequestContext.CurrentStore; }
            set { CurrentRequestContext.CurrentStore = value; }
        }

        public string CurrentCustomerId
        {
            get
            {
                if (CurrentRequestContext.CurrentAccount != null)
                {
                    return CurrentRequestContext.CurrentAccount.Bvin;
                }

                return string.Empty;
            }
        }

        public CustomerAccount CurrentCustomer
        {
            get { return CurrentRequestContext.CurrentAccount; }
        }

        public string ViewsVirtualPath
        {
            get
            {
                var viewsVirtualPath = string.Empty;
                if (CurrentStore != null)
                    viewsVirtualPath = CurrentStore.Settings.Urls.ViewsVirtualPath;
                else
                    viewsVirtualPath = StoreSettingsUrls.DefaultViewsVirtualPath;
                return viewsVirtualPath;
            }
        }

        public List<Promotion> CurrentlyActiveSales
        {
            get
            {
                if (_CurrentlyActiveSales == null)
                {
                    _CurrentlyActiveSales = MarketingServices.Promotions.FindAllPotentiallyActiveSales(DateTime.Now);
                }
                return _CurrentlyActiveSales;
            }
        }

        public string StoreUrl(bool forceSecure, bool forceNonSecure)
        {
            var result = VirtualPathUtility.ToAbsolute("~");

            if (CurrentStore != null)
            {
                if (forceSecure)
                {
                    result = CurrentStore.RootUrlSecure();
                }
                else
                {
                    result = CurrentStore.RootUrl();
                }
            }
            if (!result.EndsWith("/"))
            {
                result += "/";
            }
            return result;
        }

        public bool IsCurrentRequestSecure()
        {
            var secure = false;
            try
            {
                secure = CurrentRequestContext.RoutingContext.HttpContext.Request.IsSecureConnection;
            }
            catch
            {
                secure = false;
            }
            return secure;
        }

        public bool UpdateCurrentStore()
        {
            return AccountServices.Stores.Update(CurrentStore);
        }

        public bool CalculateOrder(Order o)
        {
            var calc = new OrderCalculator(this);
            return calc.Calculate(o);
        }

        public bool CalculateOrderWithoutRepricing(Order o)
        {
            var calc = new OrderCalculator(this);
            calc.SkipRepricing = true;
            return calc.Calculate(o);
        }

        public bool CalculateOrderWithoutRepriceingWithoutDiscounts(Order o)
        {
            var calc = new OrderCalculator(this);
            calc.SkipDiscounts = true;
            calc.SkipRepricing = true;
            return calc.Calculate(o);
        }

        public bool CalculateOrderAndSave(Order o)
        {
            if (!CalculateOrder(o)) return false;
            return OrderServices.Orders.Upsert(o);
        }

        public bool CalculateOrderAndSaveWithoutRepricing(Order o)
        {
            if (!CalculateOrderWithoutRepricing(o)) return false;
            return OrderServices.Orders.Upsert(o);
        }

        public bool CalculateOrderAndSaveWithoutRepricingWithoutDiscounts(Order o)
        {
            if (!CalculateOrderWithoutRepriceingWithoutDiscounts(o)) return false;
            return OrderServices.Orders.Upsert(o);
        }

        public bool AddToOrderWithCalculateAndSave(Order o, LineItem li)
        {
            o.IsAbandonedEmailSent = false;
            o.TimeOfOrderUtc = DateTime.UtcNow;
            o.UsedCulture = CurrentRequestContext.MainContentCulture;
            OrderServices.AddItemToOrder(o, li);
            AnalyticsService.RegisterEvent(CurrentCustomerId, ActionTypes.ProductAddedToCart, li.ProductId);
            CalculateOrder(o);
            return OrderServices.Orders.Upsert(o);
        }

        public bool AddToOrderAndSave(Order o, LineItem li)
        {
            o.IsAbandonedEmailSent = false;
            o.TimeOfOrderUtc = DateTime.UtcNow;
            OrderServices.AddItemToOrder(o, li);
            AnalyticsService.RegisterEvent(CurrentCustomerId, ActionTypes.ProductAddedToCart, li.ProductId);
            return OrderServices.Orders.Upsert(o);
        }

        public void ClearOrder(Order order)
        {
            if (order == null)
                return;

            order.Items.Clear();
            CalculateOrderAndSave(order);
        }

        public SystemOperationResult CheckForStockOnItems(Order order)
        {
            // Build a list of product quantities to check
            var products = new Dictionary<string, ProductIdCombo>();
            foreach (var item in order.Items)
            {
                var product = item.GetAssociatedProduct(this);
                if (product == null || product.Status == ProductStatus.Disabled)
                {
                    var productNotAvailable = GlobalLocalization.GetFormattedString("ProductNotAvailable",
                        product.ProductName);
                    return new SystemOperationResult(false, productNotAvailable);
                }

                if (!product.IsBundle)
                {
                    var combo = new ProductIdCombo
                    {
                        ProductId = item.ProductId,
                        VariantId = item.VariantId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Product = product
                    };

                    if (products.ContainsKey(combo.Key()))
                        products[combo.Key()].Quantity += combo.Quantity;
                    else
                        products.Add(combo.Key(), combo);
                }
                else
                {
                    foreach (var bundledProductAdv in product.BundledProducts)
                    {
                        var bundledProduct = bundledProductAdv.BundledProduct;
                        if (bundledProduct == null)
                            continue;

                        var optionSelection = item.SelectionData.GetSelections(bundledProductAdv.Id);
                        var variant = bundledProduct.Variants.FindBySelectionData(optionSelection,
                            bundledProduct.Options);
                        var variantId = variant != null ? variant.Bvin : string.Empty;

                        var combo = new ProductIdCombo
                        {
                            ProductId = bundledProduct.Bvin,
                            VariantId = variantId,
                            ProductName = bundledProduct.ProductName,
                            Quantity = bundledProductAdv.Quantity,
                            Product = bundledProduct
                        };

                        if (products.ContainsKey(combo.Key()))
                            products[combo.Key()].Quantity += combo.Quantity;
                        else
                            products.Add(combo.Key(), combo);
                    }
                }
            }

            // Now check each quantity for the order
            foreach (var key in products.Keys)
            {
                var checkcombo = products[key];
                var prod = checkcombo.Product;

                var data = CatalogServices.SimpleProductInventoryCheck(prod, checkcombo.VariantId, checkcombo.Quantity);

                if (!data.IsAvailableForSale)
                {
                    if (data.Qty == 0)
                    {
                        var cartOutOfStock = GlobalLocalization.GetFormattedString("CartOutOfStock", prod.ProductName);
                        return new SystemOperationResult(false, cartOutOfStock);
                    }
                    var message = GlobalLocalization.GetFormattedString("CartNotEnoughQuantity", prod.ProductName,
                        data.Qty);
                    return new SystemOperationResult(false, message);
                }
            }

            return new SystemOperationResult(true, string.Empty);
        }

        public SystemOperationResult CheckForStockOnItems(LineItem item)
        {
            // Build a list of product quantities to check
            var products = new Dictionary<string, ProductIdCombo>();

            var product = item.GetAssociatedProduct(this);
            if (product == null || product.Status == ProductStatus.Disabled)
            {
                var productNotAvailable = GlobalLocalization.GetFormattedString("ProductNotAvailable",
                    product.ProductName);
                return new SystemOperationResult(false, productNotAvailable);
            }

            if (!product.IsBundle)
            {
                var combo = new ProductIdCombo
                {
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Product = product
                };

                if (products.ContainsKey(combo.Key()))
                    products[combo.Key()].Quantity += combo.Quantity;
                else
                    products.Add(combo.Key(), combo);
            }
            else
            {
                foreach (var bundledProductAdv in product.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;

                    var optionSelection = item.SelectionData.GetSelections(bundledProductAdv.Id);
                    var variant = bundledProduct.Variants.FindBySelectionData(optionSelection, bundledProduct.Options);
                    var variantId = variant != null ? variant.Bvin : string.Empty;

                    var combo = new ProductIdCombo
                    {
                        ProductId = bundledProduct.Bvin,
                        VariantId = variantId,
                        ProductName = bundledProduct.ProductName,
                        Quantity = bundledProductAdv.Quantity,
                        Product = bundledProduct
                    };

                    if (products.ContainsKey(combo.Key()))
                        products[combo.Key()].Quantity += combo.Quantity;
                    else
                        products.Add(combo.Key(), combo);
                }
            }


            // Now check each quantity for the order
            foreach (var key in products.Keys)
            {
                var checkcombo = products[key];
                var prod = checkcombo.Product;

                var data = CatalogServices.SimpleProductInventoryCheck(prod, checkcombo.VariantId, checkcombo.Quantity);

                if (!data.IsAvailableForSale)
                {
                    if (data.Qty == 0)
                    {
                        var cartOutOfStock = GlobalLocalization.GetFormattedString("CartOutOfStock", prod.ProductName);
                        return new SystemOperationResult(false, cartOutOfStock);
                    }
                    var message = GlobalLocalization.GetFormattedString("CartNotEnoughQuantity", prod.ProductName,
                        data.Qty);
                    return new SystemOperationResult(false, message);
                }
            }

            return new SystemOperationResult(true, string.Empty);
        }


        // Product Pricing
        public UserSpecificPrice PriceProduct(string productBvin, string userId, OptionSelections selections)
        {
            var p = CatalogServices.Products.FindWithCache(productBvin);
            var customer = MembershipServices.Customers.Find(userId);
            return PriceProduct(p, customer, selections, CurrentlyActiveSales);
        }

        public UserSpecificPrice PriceProduct(Product p, CustomerAccount currentUser, OptionSelections selections,
            List<Promotion> currentSales)
        {
            if (p == null) return null;
            var result = new UserSpecificPrice(p, selections, CurrentStore.Settings);
            AdjustProductPriceForUser(result, p, currentUser);
            ApplySales(result, p, currentUser, currentSales);
            CheckForPricesBelowZero(result);

            return result;
        }

        private void ApplySales(UserSpecificPrice price, Product p, CustomerAccount currentUser, List<Promotion> sales)
        {
            if (sales == null) return;
            foreach (var promo in sales)
            {
                promo.ApplyToProduct(CurrentRequestContext, p, price, currentUser);
            }
        }

        private void AdjustProductPriceForUser(UserSpecificPrice price, Product p, CustomerAccount currentUser)
        {
            if (currentUser == null) return;
            if (currentUser.Bvin == string.Empty) return;
            if (currentUser.PricingGroupId == string.Empty) return;
            if (p == null) return;
            if (price == null) return;

            var startingPrice = price.PriceWithAdjustments();

            var pricingGroup = ContactServices.PriceGroups.Find(currentUser.PricingGroupId);
            if (pricingGroup == null) return;

            var nonGroupPrice = price.BasePrice;

            var groupPrice = nonGroupPrice;
            //groupPrice = pricingGroup.GetAdjustedPriceForThisGroup(p.SitePrice, p.ListPrice, p.SiteCost);
            groupPrice = pricingGroup.GetAdjustedPriceForThisGroup(startingPrice, p.ListPrice, p.SiteCost);

            var amountOfDiscount = groupPrice - nonGroupPrice;
            price.AddAdjustment(amountOfDiscount, "Price Group");
        }

        private void CheckForPricesBelowZero(UserSpecificPrice price)
        {
            var final = price.PriceWithAdjustments();

            if (final < 0)
            {
                var tweak = -1*final;
                price.AddAdjustment(tweak, "Price can not be below zero");
            }
        }


        // Orders
        public bool OrdersReserveInventoryForAllItems(Order o, List<string> errors)
        {
            var result = true;
            foreach (var li in o.Items)
            {
                li.QuantityReserved = CatalogServices.InventoryLineItemReserveInventory(li);
                if (li.QuantityReserved != li.Quantity)
                {
                    if (errors != null)
                    {
                        errors.Add("Item " + li.ProductName + " did not have enough quantity to complete order.");
                    }
                    EventLog.LogEvent("Reserve Inventory For All Items",
                        "Unable to reserve quantity of " + li.Quantity + " for product " + li.ProductId,
                        EventLogSeverity.Debug);
                    result = false;
                }
            }
            return result;
        }

        public bool OrdersUnreserveInventoryForAllItems(Order o)
        {
            var result = true;
            foreach (var li in o.Items)
            {
                if (CatalogServices.InventoryLineItemUnreserveInventory(li) == false)
                {
                    EventLog.LogEvent("Unreserve Inventory For All Items",
                        "Unable to unreserve quantity of " + li.Quantity + " for product " + li.ProductId,
                        EventLogSeverity.Debug);
                }
            }
            return result;
        }

        // Order Packages
        public bool OrdersShipAllPackages(Order o)
        {
            foreach (var p in o.Packages)
            {
                if (!p.HasShipped)
                {
                    OrdersShipPackage(p, o);
                }
            }
            OrderServices.Orders.Update(o);
            return true;
        }

        public bool OrdersShipPackage(OrderPackage package, Order order)
        {
            if (order == null)
                throw new NullReferenceException("order");

            if (package == null)
                throw new NullReferenceException("package");

            package.HasShipped = true;
            OrdersShipItems(package, order);

            var c = new OrderTaskContext(CurrentRequestContext);
            c.Order = order;
            c.UserId = CurrentCustomerId;
            if (!Workflow.RunByName(c, WorkflowNames.PackageShipped))
            {
                EventLog.LogEvent("PackageShippedWorkflow", "Package Shipped Workflow Failed", EventLogSeverity.Debug);
                foreach (var item in c.Errors)
                {
                    EventLog.LogEvent("PackageShippedWorkflow", item.Description, EventLogSeverity.Debug);
                }
            }
            return true;
        }

        private void OrdersShipItems(OrderPackage package, Order order)
        {
            if (package.Items.Count == 0)
                return;

            foreach (var pi in package.Items)
            {
                var li = order.GetLineItem(pi.LineItemId);
                if (li == null || li.IsNonShipping)
                    continue;

                // Prevent shipping more than ordered if order is not recurring
                if (!order.IsRecurring && pi.Quantity > li.Quantity - li.QuantityShipped)
                {
                    pi.Quantity = li.Quantity - li.QuantityShipped;
                }

                if (pi.Quantity <= 0)
                {
                    pi.Quantity = 0;
                }
                else
                {
                    CatalogServices.InventoryLineItemShipQuantity(li, pi.Quantity);
                }
            }
        }

        public bool OrdersUnshipItems(OrderPackage package, Order order)
        {
            if (order == null)
                throw new NullReferenceException("order");

            if (package == null)
                throw new NullReferenceException("package");

            var result = true;

            if (package.Items.Count == 0)
                return result;

            foreach (var pi in package.Items)
            {
                var li = order.GetLineItem(pi.LineItemId);
                if (li == null || li.IsNonShipping)
                    continue;

                // Prevent unshipping more than shipped if order is not recurring
                var quantityToUnship = pi.Quantity;
                if (!order.IsRecurring && pi.Quantity > li.QuantityShipped)
                {
                    quantityToUnship = li.QuantityShipped;
                }
                CatalogServices.InventoryLineItemUnShipQuantity(li, quantityToUnship);
            }

            return result;
        }

        //Reporting
        public List<Product> ReportingTop10Sellers()
        {
            var results = new List<Product>();

            var startDateUtc = DateTime.UtcNow.AddMonths(-2);
            var endDateUtc = DateTime.UtcNow;

            var idList = OrderServices.Orders.FindPopularItems(startDateUtc, endDateUtc, 10);
            var allKeys = idList.Select(y => y.Key).ToList();
            results = CatalogServices.Products.FindManyWithCache(allKeys);

            return results;
        }

        public List<Product> ReportingTopSellersByDate(DateTime startDateUtc, DateTime endDateUtc, int maxResults)
        {
            var results = new List<Product>();

            var idList = OrderServices.Orders.FindPopularItems(startDateUtc, endDateUtc, maxResults);
            var allKeys = idList.Select(y => y.Key).ToList();
            results = CatalogServices.Products.FindManyWithCache(allKeys);

            return results;
        }

        public Product ParseProductBySlug(string slug, out CustomUrl customUrl)
        {
            customUrl = null;
            var result = CatalogServices.Products.FindBySlug(slug);
            if (result == null || result.Status == ProductStatus.Disabled)
            {
                // Check for custom URL
                customUrl = ContentServices.CustomUrls.FindByRequestedUrl(slug, CustomUrlType.Product);
                if (customUrl != null)
                {
                    result = CatalogServices.Products.FindBySlug(customUrl.RedirectToUrl);
                }
            }

            return result;
        }

        public Category ParseCategoryBySlug(string slug, out CustomUrl customUrl)
        {
            customUrl = null;
            var result = CatalogServices.Categories.FindBySlug(slug);
            if (result == null)
            {
                // Check for custom URL
                customUrl = ContentServices.CustomUrls.FindByRequestedUrl(slug, CustomUrlType.Category);
                if (customUrl != null)
                {
                    result = CatalogServices.Categories.FindBySlug(customUrl.RedirectToUrl);
                }
            }

            return result;
        }

        public List<ProductRelationship> GetAutoSuggestedRelatedItems(string bvin, int maxItemsToReturn)
        {
            using (var context = Factory.CreateHccDbContext())
            {
                var storeId = CurrentRequestContext.CurrentStore.Id;
                var productGuid = DataTypeHelper.BvinToGuid(bvin);

                var results = (from a in
                    (from l in context.hcc_LineItem
                        join o in context.hcc_Order on new {l.OrderBvin} equals new {OrderBvin = o.bvin}
                        join p in context.hcc_Product on new {bvin = l.ProductId} equals new {p.bvin}
                        where
                            o.IsPlaced == 1 &&
                            p.IsAvailableForSale &&
                            p.StoreId == storeId &&
                            p.bvin != productGuid &&
                            (from hcc_lineitem in context.hcc_LineItem
                                where
                                    hcc_lineitem.ProductId == productGuid &&
                                    hcc_lineitem.StoreId == storeId
                                select new
                                {
                                    hcc_lineitem.OrderBvin
                                }).Contains(new {l.OrderBvin})
                        select new
                        {
                            ProductID = p.bvin,
                            l.Quantity
                        })
                    group a by new
                    {
                        a.ProductID
                    }
                    into g
                    orderby
                        g.Sum(p => p.Quantity) descending
                    select new
                    {
                        g.Key.ProductID,
                        Total_Ordered = g.Sum(p => p.Quantity)
                    }).Take(maxItemsToReturn);

                var relationships = new List<ProductRelationship>();
                foreach (var r in results)
                {
                    var relatedProductId = DataTypeHelper.GuidToBvin(r.ProductID);
                    relationships.Add(new ProductRelationship
                    {
                        ProductId = bvin,
                        RelatedProductId = relatedProductId,
                        IsSubstitute = false,
                        StoreId = storeId
                    });
                }
                return relationships;
            }
        }

        public bool DestroyAllCategories()
        {
            var current = DateTime.UtcNow;
            var availableUntil = CurrentRequestContext.CurrentStore.Settings.AllowApiToClearUntil;
            var compareResult = DateTime.Compare(current, availableUntil);
            if (compareResult >= 0)
            {
                return false;
            }

            var all = CatalogServices.Categories.FindAll();
            foreach (var snap in all)
            {
                CatalogServices.DestroyCategory(snap.Bvin);
            }

            return true;
        }

        internal void DestroyAllCategoriesForStore(long storeId)
        {
            CatalogServices.Categories.DestroyAllForStore(storeId);
        }

        public bool DestroyCustomerAccount(string bvin)
        {
            CatalogServices.WishListItems.DeleteForCustomerId(bvin);
            return MembershipServices.Customers.Delete(bvin);
        }

        public ClearProductsData ClearProducts(int howMany)
        {
            var result = new ClearProductsData();

            var current = DateTime.UtcNow;
            var availableUntil = CurrentRequestContext.CurrentStore.Settings.AllowApiToClearUntil;
            var compareResult = DateTime.Compare(current, availableUntil);
            if (compareResult >= 0)
            {
                result.ProductsCleared = 0;
                result.ProductsRemaining = -1;
                return result;
            }

            return DeleteSome(CurrentRequestContext.CurrentStore.Id, howMany);
        }

        private ClearProductsData DeleteSome(long storeId, int howMany)
        {
            var items = CatalogServices.Products.FindAllPagedWithCache(1, howMany);
            var totalCount = CatalogServices.Products.FindAllCount(storeId);

            if (items != null)
            {
                foreach (var p in items)
                {
                    CatalogServices.DestroyProduct(p.Bvin, p.StoreId);
                }
            }

            var left = totalCount - howMany;
            if (left < 0) left = 0;
            var cleared = howMany;
            if (totalCount < howMany) cleared = totalCount;
            var result = new ClearProductsData();
            result.ProductsCleared = cleared;
            result.ProductsRemaining = left;

            return result;
        }

        public bool DestroyAllProductsForStore(long storeId)
        {
            var bvins = CatalogServices.Products.FindAllBvinsForStore(storeId);
            foreach (var bvin in bvins)
            {
                CatalogServices.DestroyProduct(bvin, storeId);
            }
            return true;
        }

        // Multi-Store Destroy
        public bool DestroyStore(long storeId)
        {
            EventLog.LogEvent("System", "Destroying Store " + storeId, EventLogSeverity.Debug);

            var result = true;

            var s = AccountServices.Stores.FindById(storeId);
            if (s == null) return false;
            if (s.Id != storeId) return false;

            DiskStorage.DestroyAllFilesForStore(storeId);
            //this.AccountServices.RemoveAllUsersFromStore(storeId);

            // Store Address
            var storeAddress = ContactServices.Addresses.FindStoreContactAddress();
            if (storeAddress != null) ContactServices.Addresses.Delete(storeAddress.Bvin);

            // Get rid of URLs so they won't stop category/product deletes
            ContentServices.CustomUrls.DestoryAllForStore(storeId);

            // Catalog Services
            DestroyAllProductsForStore(storeId);
            DestroyAllCategoriesForStore(storeId);
            CatalogServices.ProductProperties.DestroyAllForStore(storeId);
            CatalogServices.ProductTypeDestoryAllForStore(storeId);
            CatalogServices.WishListItems.DestroyAllForStore(storeId);

            // Contact Services
            ContactServices.Affiliates.DestoryForStore(storeId);
            ContactServices.MailingLists.DestoryForStore(storeId);
            ContactServices.Manufacturers.DestoryAllForStore(storeId);
            ContactServices.PriceGroups.DestoryAllForStore(storeId);
            ContactServices.Vendors.DestoryAllForStore(storeId);

            // Content Services
            ContentServices.Columns.DestroyForStore(storeId);

            ContentServices.HtmlTemplates.DestroyAllForStore(storeId);

            // Customer Points
            CustomerPointsManager.DestroyAllForStore(storeId);

            // Membership
            MembershipServices.UserQuestions.DestroyAllForStore(storeId);
            MembershipServices.Customers.DestroyAllForStore(storeId);

            // Marketing
            MarketingServices.Promotions.DestroyAllForStore(storeId);

            // Metrics
            MetricsSerices.SearchQueries.DestoryAllForStore(storeId);

            // Orders
            OrderServices.Orders.DestoryAllForStore(storeId);
            OrderServices.ShippingMethods.DestoryAllForStore(storeId);
            OrderServices.ShippingZones.DestoryAllForStore(storeId);
            OrderServices.Taxes.DestoryAllForStore(storeId);
            OrderServices.TaxSchedules.DestoryAllForStore(storeId);
            OrderServices.Transactions.DestoryAllForStore(storeId);

            // Tasks
            ScheduleServices.QueuedTasks.DestoryAllForStore(storeId);

            // Account Services 
            AccountServices.ApiKeys.DestoryAllForStore(storeId);
            AccountServices.Stores.Delete(storeId);

            if (result)
            {
                EventLog.LogEvent("System", "Finished Destroying Store " + storeId, EventLogSeverity.Debug);
            }
            else
            {
                EventLog.LogEvent("System", "Error Destroying Store " + storeId, EventLogSeverity.Warning);
            }

            return result;
        }

        private class ProductIdCombo
        {
            public string ProductId { get; set; }
            public string VariantId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public Product Product { get; set; }

            public string Key()
            {
                return ProductId + VariantId;
            }
        }

        #region Services

        private AccountService _AccountService;

        public AccountService AccountServices
        {
            get
            {
                if (_AccountService == null)
                {
                    _AccountService = Factory.CreateService<AccountService>(CurrentRequestContext);
                }
                return _AccountService;
            }
        }

        private CatalogService _CatalogService;

        public CatalogService CatalogServices
        {
            get
            {
                if (_CatalogService == null)
                {
                    _CatalogService = Factory.CreateService<CatalogService>(CurrentRequestContext);
                }
                return _CatalogService;
            }
        }

        private ContactService _ContactServices;

        public ContactService ContactServices
        {
            get
            {
                if (_ContactServices == null)
                {
                    _ContactServices = Factory.CreateService<ContactService>(CurrentRequestContext);
                }
                return _ContactServices;
            }
        }

        private ContentService _ContentServices;

        public ContentService ContentServices
        {
            get
            {
                if (_ContentServices == null)
                {
                    _ContentServices = Factory.CreateService<ContentService>(CurrentRequestContext);
                }
                return _ContentServices;
            }
        }

        private OrderService _OrderServices;

        public OrderService OrderServices
        {
            get
            {
                if (_OrderServices == null)
                {
                    _OrderServices = Factory.CreateService<OrderService>(CurrentRequestContext);
                }
                return _OrderServices;
            }
        }

        private MarketingService _MarketingServices;

        public MarketingService MarketingServices
        {
            get
            {
                if (_MarketingServices == null)
                {
                    _MarketingServices = Factory.CreateService<MarketingService>(CurrentRequestContext);
                }
                return _MarketingServices;
            }
        }

        private ScheduleService _ScheduleServices;

        public ScheduleService ScheduleServices
        {
            get
            {
                if (_ScheduleServices == null)
                {
                    _ScheduleServices = Factory.CreateService<ScheduleService>(CurrentRequestContext);
                }
                return _ScheduleServices;
            }
        }

        private MembershipServices _MembershipServices;

        public MembershipServices MembershipServices
        {
            get
            {
                if (_MembershipServices == null)
                {
                    _MembershipServices = Factory.CreateService<MembershipServices>(CurrentRequestContext);
                }
                return _MembershipServices;
            }
        }

        private MetricsServices _MetricsServices;

        public MetricsServices MetricsSerices
        {
            get
            {
                if (_MetricsServices == null)
                {
                    _MetricsServices = Factory.CreateService<MetricsServices>(CurrentRequestContext);
                }
                return _MetricsServices;
            }
        }

        private CustomerPointsManager _CustomerPointsManager;

        public CustomerPointsManager CustomerPointsManager
        {
            get
            {
                if (_CustomerPointsManager == null)
                {
                    _CustomerPointsManager = Factory.CreateService<CustomerPointsManager>(CurrentRequestContext);
                }
                return _CustomerPointsManager;
            }
        }

        private AnalyticsService _AnalyticsService;

        public AnalyticsService AnalyticsService
        {
            get
            {
                if (_AnalyticsService == null)
                {
                    _AnalyticsService = Factory.CreateService<AnalyticsService>(CurrentRequestContext);
                }
                return _AnalyticsService;
            }
        }

        private GlobalizationService _GlobalizationService;

        public GlobalizationService GlobalizationServices
        {
            get
            {
                if (_GlobalizationService == null)
                {
                    _GlobalizationService = Factory.CreateService<GlobalizationService>(CurrentRequestContext);
                }
                return _GlobalizationService;
            }
        }

        private ISocialService _SocialService;

        public ISocialService SocialService
        {
            get
            {
                if (_SocialService == null)
                {
                    _SocialService = Factory.CreateSocialService(CurrentRequestContext);
                }
                return _SocialService;
            }
        }

        #endregion

        #region Obsolete

        [Obsolete("Obsolete in 2.0.0.")]
        public string CurrentRelativeRoot { get; set; }

        [Obsolete("Obsolete in 2.0.0. Use constructor with other parameters instead")]
        public HotcakesApplication(HccRequestContext hccContext, bool isForMemoryOnly = false, bool initSetup = false)
            : this(hccContext)
        {
            CurrentRequestContext = hccContext;
        }

        // Customer Affiliate Relationships
        [Obsolete("Obsolete in 1.8.0. Affiliate contacts are not used")]
        public List<CustomerAccount> FindCustomersAssignedToAffiliate(long affiliateId)
        {
#pragma warning disable 0612, 0618
            var aff = ContactServices.Affiliates.Find(affiliateId);
            var ids = new List<string>();
            foreach (var rel in aff.Contacts)
            {
                ids.Add(rel.UserId);
            }
            return MembershipServices.Customers.FindMany(ids);
#pragma warning restore 0612, 0618
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of CatalogService class")]
        public bool DestroyProduct(string bvin, long storeId)
        {
            return CatalogServices.DestroyProduct(bvin, storeId);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of CatalogService class")]
        public bool DestroyCategory(string bvin)
        {
            return CatalogServices.DestroyCategory(bvin);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of CatalogService class")]
        public bool DestroyCategoryForStore(string bvin, long storeId)
        {
            return CatalogServices.DestroyCategoryForStore(bvin, storeId);
        }

        #region Sample Data

        /// <summary>
        ///     Use this method to see if any of the sample products or categories still exist in the store.
        /// </summary>
        /// <returns>If true, one or more sample products or categories are still in the store</returns>
        [Obsolete("Obsolete in 2.0.0. Use same method of SampleData class")]
        public bool SampleStoreDataExists()
        {
            return SampleData.SampleStoreDataExists();
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of SampleData class")]
        public void AddSampleProductsToStore()
        {
            SampleData.AddSampleProductsToStore();
        }

        [Obsolete("Obsolete in 2.0.0. Use same method of SampleData class")]
        public void RemoveSampleProductsFromStore()
        {
            SampleData.RemoveSampleProductsFromStore();
        }

        #endregion

        #endregion
    }
}