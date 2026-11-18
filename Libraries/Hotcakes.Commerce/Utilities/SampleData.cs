#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Threading;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Storage;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Payment;
using Hotcakes.Web;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Creates, detects, and removes demonstration catalog, order, and analytics data for a store.
    /// </summary>
    /// <remarks>
    /// Operations write to the current store through the supplied <see cref="HccRequestContext"/>. Sample-product
    /// removal destroys matching sample products and categories, while analytics creation creates synthetic events and
    /// orders; these operations are destructive or data-generating and should be used only for demonstration data.
    /// </remarks>
    public class SampleData
    {
        #region Static methods

        /// <summary>
        /// Determines whether the store contains one of the known sample products or categories.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        /// <returns><see langword="true"/> when sample catalog data is found; otherwise, <see langword="false"/>.</returns>
        public static bool SampleStoreDataExists(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            return sampleData.IsSampleDataExist();
        }

        /// <summary>
        /// Adds the standard sample catalog and synthetic analytics data to a store.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        public static void AddSampleProductsToStore(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            sampleData.AddSampleProducts();
        }

        /// <summary>
        /// Adds the standard sample catalog without creating analytics and order statistics.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        public static void AddSampleProductsToStoreWithoutStatistics(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            sampleData.AddSampleProducts();
        }

        /// <summary>
        /// Removes the known sample catalog and its sample analytics from a store.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        /// <remarks>This permanently destroys matching sample products and categories through the catalog services.</remarks>
        public static void RemoveSampleProductsFromStore(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            sampleData.RemoveSampleProducts();
        }

        /// <summary>
        /// Determines whether the store contains the sample order used to identify sample analytics.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        /// <returns><see langword="true"/> when a <c>Sample Order</c> record exists; otherwise, <see langword="false"/>.</returns>
        public static bool SampleAnalyticsExists(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            return sampleData.IsSampleAnalyticsExist();
        }

        /// <summary>
        /// Creates synthetic analytics events and orders for the specified products.
        /// </summary>
        /// <param name="products">The products used to generate sample activity.</param>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        /// <remarks>This writes demonstration analytics, orders, line items, and transactions to the store.</remarks>
        public static void CreateSampleAnalyticsForStore(List<Product> products, HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            sampleData.CreateSampleAnalytics(products);
        }

        /// <summary>
        /// Adds sample products without statistics and queues synthetic analytics creation on the thread pool.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        /// <remarks>The method returns before the background analytics operation completes. Progress is written to the
        /// event log, and failures in the queued work are not returned to the caller.</remarks>
        public static void CreateSampleAnalyticsForStoreAsync(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);

            sampleData.AddSampleProductsWithoutStatistics();

            EventLog.LogEvent("SampleData", "Product and categories created", EventLogSeverity.Information);

            var objArguments = new SampleDataConfiguration
            {
                HccRequestContext = HccRequestContext.Current
            };

            //Start process in background
            ThreadPool.QueueUserWorkItem(_ => sampleData.CreateSamplePerMonth(objArguments));
        }

        /// <summary>
        /// Starts a background thread for the supplied delegate when it is not <see langword="null"/>.
        /// </summary>
        /// <param name="threadStart">The delegate executed by the background thread.</param>
        public static void StartBackgroundThread(ThreadStart threadStart)
        {
            if (threadStart != null)
            {
                var thread = new Thread(threadStart);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        /// <summary>
        /// Removes sample analytics data from a store.
        /// </summary>
        /// <param name="context">The store request context, or <see langword="null"/> to use the current context.</param>
        /// <remarks>This deletes sample analytics through the data context and is destructive.</remarks>
        public static void RemoveSampleAnalyticsForStore(HccRequestContext context = null)
        {
            var sampleData = new SampleData(context ?? HccRequestContext.Current);
            sampleData.DeleteSampleAnalytics(false);
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// Gets or sets the request context used for store-scoped operations.
        /// </summary>
        public HccRequestContext Context { get; set; }

        /// <summary>
        /// Gets or sets the catalog service used to create, find, and remove sample catalog records.
        /// </summary>
        public CatalogService CatalogServices { get; set; }

        /// <summary>
        /// Initializes sample-data operations for a request context.
        /// </summary>
        /// <param name="context">The request context that identifies the target store.</param>
        public SampleData(HccRequestContext context)
        {
            Context = context;
            CatalogServices = Factory.CreateService<CatalogService>(Context);
        }

        /// <summary>
        /// Determines whether any of the known sample products or categories still exist in the store.
        /// </summary>
        /// <returns><see langword="true"/> when one or more sample products or categories remain; otherwise, <see langword="false"/>.</returns>
        public bool IsSampleDataExist()
        {
            // execute the product search
            var smapleProductSkus = new List<string>
            {
                "SAMPLE001",
                "SAMPLE002",
                "SAMPLE003",
                "SAMPLE004",
                "SAMPLE005",
                "SAMPLE006"
            };
            var products = CatalogServices.Products.FindManySkus(smapleProductSkus);

            // it's okay to simply return true now if the condition permits, to save us from the expensive operation below
            if (products.Any()) return true;

            // continue to make sure that we no longer have any of the sample categories
            var categories = CatalogServices.Categories.FindAll()
                .Where(x => x.StoreId == Context.CurrentStore.Id)
                .Where(x => x.RewriteUrl == "sample-products" ||
                            x.RewriteUrl == "more-sample" ||
                            x.RewriteUrl == "demo-category");

            // return true if there are sample categories found
            if (categories.Any()) return true;

            // no conditions were met, so there isn't any sample data
            return false;
        }

        /// <summary>
        /// Determines whether a sample order exists in the store.
        /// </summary>
        /// <returns><see langword="true"/> when an order numbered <c>Sample Order</c> exists; otherwise, <see langword="false"/>.</returns>
        public bool IsSampleAnalyticsExist()
        {
            var orderService = Factory.CreateService<OrderService>();
            var sampleOrder = orderService.Orders.FindByOrderNumber("Sample Order");

            if (sampleOrder != null)
                return true;

            return false;
        }

        /// <summary>
        /// Creates the standard sample categories and products, then creates synthetic analytics data.
        /// </summary>
        public void AddSampleProducts()
        {
            var categories = CreateSampleCategories();
            var products = CreateSampleProducts(categories);

            CreateSampleAnalytics(products);
        }

        /// <summary>
        /// Creates the standard sample categories and products without analytics or order statistics.
        /// </summary>
        public void AddSampleProductsWithoutStatistics()
        {
            var categories = CreateSampleCategories();
            var products = CreateSampleProducts(categories);
        }

        /// <summary>
        /// Removes the standard sample analytics, categories, and products from the store.
        /// </summary>
        /// <remarks>Matching records are permanently destroyed through the catalog services.</remarks>
        public void RemoveSampleProducts()
        {
            DeleteSampleAnalytics(true);

            DeleteSampleCategoryBySlug("sample-products");
            DeleteSampleCategoryBySlug("more-sample");
            DeleteSampleCategoryBySlug("demo-category");
            DeleteSampleProduct("SAMPLE001");
            DeleteSampleProduct("SAMPLE002");
            DeleteSampleProduct("SAMPLE003");
            DeleteSampleProduct("SAMPLE004");
            DeleteSampleProduct("SAMPLE005");
            DeleteSampleProduct("SAMPLE006");
        }

        #region Private

        protected void CreateSamplePerMonth(SampleDataConfiguration conf)
        {
            //var conf = e.Argument as SampleDataConfiguration;
            HccRequestContext.Current = conf.HccRequestContext;
            for (var i = 0; i < 24; i++)
            {
                CreateSampleAnalyticsByMonth(i);
            }
        }

        internal void CreateSampleAnalytics(List<Product> products)
        {
            var productsCount = products.Count;

            var storeId = Context.CurrentStore.Id;
            var now = DateTime.UtcNow;
            var startDate = now.AddMonths(-24);

            var index = 0;
            var currDate = startDate;

            var sessionGuid = Guid.NewGuid();
            var analyticsEventsList = new List<AnalyticsEvent>();
            var analyticsEventTypes = new[] {ActionTypes.ProductViewed, ActionTypes.ProductAddedToCart};
            while (currDate < now)
            {
                foreach (var product in products)
                {
                    for (var i = 0; i < currDate.Month + currDate.Year%2 + index%6; i++)
                    {
                        var analyticsEvent = new AnalyticsEvent
                        {
                            SessionGuid = sessionGuid,
                            ShoppingSessionGuid = sessionGuid,
                            StoreId = storeId,
                            Action = analyticsEventTypes[i%2],
                            ObjectId = DataTypeHelper.BvinToGuid(product.Bvin),
                            DateTime = currDate
                        };
                        analyticsEventsList.Add(analyticsEvent);

                        sessionGuid = Guid.NewGuid();
                    }

                    index++;
                }

                currDate = currDate.AddDays(1);
            }
            var analyticsEventsRepository = Factory.CreateRepo<AnalyticsEventsRepository>();
            analyticsEventsRepository.BatchCreate(analyticsEventsList);

            index = 0;
            currDate = startDate;
            var ordersList = new List<Order>();
            var address = new Address
            {
                FirstName = "Jane",
                LastName = "Doe",
                Line1 = "548 Market St.",
                Line2 = "Suite 65401",
                City = "San Francisco",
                RegionBvin = "CA",
                PostalCode = "94104",
                CountryBvin = Country.UnitedStatesCountryBvin,
                Phone = "(650) 381-9160"
            };

            while (currDate < now)
            {
                foreach (var product in products)
                {
                    var order = new Order
                    {
                        bvin = Guid.NewGuid().ToString(),
                        StoreId = storeId,
                        BillingAddress = address,
                        ShippingAddress = address,
                        UserID = "1",
                        UserEmail = "solutions@upendoventures.com",
                        OrderNumber = "Sample Order",
                        ShippingMethodId = "TOBEDETERMINED",
                        ShippingProviderId = string.Empty,
                        ShippingProviderServiceCode = string.Empty,
                        ShippingMethodDisplayName = "To Be Determined. Contact Store for Details",
                        PaymentStatus = OrderPaymentStatus.Paid,
                        ShippingStatus = OrderShippingStatus.Unshipped,
                        StatusCode = "F37EC405-1EC6-4a91-9AC4-6836215FBBBC",
                        StatusName = "Received",
                        IsPlaced = true,
                        IsAbandonedEmailSent = true,
                        UserDeviceType = (DeviceType) (index%3),
                        LastUpdatedUtc = currDate,
                        TimeOfOrderUtc = currDate
                    };
                    var qty = (currDate.Month + 4 + currDate.Year%2 + index%6)%12 + 4;
                    var lineItem = new LineItem
                    {
                        StoreId = storeId,
                        OrderBvin = order.bvin,
                        ProductId = product.Bvin,
                        ProductSku = product.Sku,
                        ProductName = product.ProductName,
                        Quantity = qty,
                        BasePricePerItem = product.SitePrice,
                        AdjustedPricePerItem = product.SitePrice,
                        LineTotal = product.SitePrice*qty,
                        ShipFromMode = ShippingMode.ShipFromSite,
                        LastUpdatedUtc = currDate
                    };
                    order.Items.Add(lineItem);
                    ordersList.Add(order);

                    index++;

                    var order2 = new Order
                    {
                        bvin = Guid.NewGuid().ToString(),
                        StoreId = storeId,
                        BillingAddress = address,
                        ShippingAddress = address,
                        UserID = "1",
                        UserEmail = "solutions@upendoventures.com",
                        OrderNumber = "Sample Order",
                        ShippingMethodId = "TOBEDETERMINED",
                        ShippingProviderId = string.Empty,
                        ShippingProviderServiceCode = string.Empty,
                        ShippingMethodDisplayName = "To Be Determined. Contact Store for Details",
                        PaymentStatus = OrderPaymentStatus.Paid,
                        ShippingStatus = OrderShippingStatus.Unshipped,
                        StatusCode = OrderStatusCode.Received,
                        StatusName = "Received",
                        IsPlaced = false,
                        IsAbandonedEmailSent = true,
                        UserDeviceType = (DeviceType) (index%3),
                        LastUpdatedUtc = currDate,
                        TimeOfOrderUtc = currDate
                    };
                    var qty2 = (currDate.Month + 8 + currDate.Year%2 + index%6)%12 + 2;
                    var lineItem2 = new LineItem
                    {
                        StoreId = storeId,
                        OrderBvin = order2.bvin,
                        ProductId = product.Bvin,
                        ProductSku = product.Sku,
                        ProductName = product.ProductName,
                        Quantity = qty2,
                        BasePricePerItem = product.SitePrice,
                        AdjustedPricePerItem = product.SitePrice,
                        LineTotal = product.SitePrice*qty2,
                        ShipFromMode = ShippingMode.ShipFromSite,
                        LastUpdatedUtc = currDate
                    };
                    order2.Items.Add(lineItem2);
                    ordersList.Add(order2);

                    index++;
                }

                currDate = currDate.AddDays(1);
            }
            var orderRepository = Factory.CreateRepo<OrderRepository>();
            orderRepository.BatchCreate(ordersList, false);

            var lineItemList = ordersList.SelectMany(o => o.Items).ToList();
            var lineItemRepository = Factory.CreateRepo<LineItemRepository>();
            lineItemRepository.BatchCreate(lineItemList);


            var orderTransactionList = new List<OrderTransaction>();
            foreach (var order in ordersList)
            {
                if (!order.IsPlaced)
                    continue;

                var orderTransaction = new OrderTransaction
                {
                    StoreId = storeId,
                    OrderId = order.bvin,
                    OrderNumber = "Sample Order",
                    TimeStampUtc = order.TimeOfOrderUtc,
                    Action = ActionType.CashReceived,
                    Amount = order.TotalGrand,
                    Success = true
                };
                orderTransactionList.Add(orderTransaction);
            }
            var orderTransactionRepository = Factory.CreateRepo<OrderTransactionRepository>();
            orderTransactionRepository.BatchCreate(orderTransactionList);
        }

        internal void CreateSampleAnalyticsByMonth(int month)
        {
            var smapleProductSkus = new List<string>
            {
                "SAMPLE001",
                "SAMPLE002",
                "SAMPLE003",
                "SAMPLE004",
                "SAMPLE005",
                "SAMPLE006"
            };
            var products = CatalogServices.Products.FindManySkus(smapleProductSkus);

            var productsCount = products.Count;

            var storeId = Context.CurrentStore.Id;

            var todaysDate = DateTime.UtcNow;
            var startDate = todaysDate.AddMonths(-month);

            EventLog.LogEvent("SampleData", "Creating Analytics info for " + startDate.ToString("Y"),
                EventLogSeverity.Information);

            var endDate = startDate.AddMonths(-1);

            var index = 0;
            var currDate = startDate;

            var sessionGuid = Guid.NewGuid();
            var analyticsEventsList = new List<AnalyticsEvent>();
            var analyticsEventTypes = new[] {ActionTypes.ProductViewed, ActionTypes.ProductAddedToCart};
            while (currDate > endDate)
            {
                if (currDate != todaysDate)
                {
                    foreach (var product in products)
                    {
                        for (var i = 0; i < currDate.Month + currDate.Year%2 + index%6; i++)
                        {
                            var analyticsEvent = new AnalyticsEvent
                            {
                                SessionGuid = sessionGuid,
                                ShoppingSessionGuid = sessionGuid,
                                StoreId = storeId,
                                Action = analyticsEventTypes[i%2],
                                ObjectId = DataTypeHelper.BvinToGuid(product.Bvin),
                                DateTime = currDate
                            };
                            analyticsEventsList.Add(analyticsEvent);

                            sessionGuid = Guid.NewGuid();
                        }

                        index++;
                    }
                }
                currDate = currDate.AddDays(-1);
            }

            var analyticsEventsRepository = Factory.CreateRepo<AnalyticsEventsRepository>();
            analyticsEventsRepository.BatchCreate(analyticsEventsList);

            index = 0;
            currDate = startDate;

            var ordersList = new List<Order>();
            var address = new Address
            {
                FirstName = "John",
                LastName = "Dough",
                Line1 = "890 Laurel St.",
                //Line2 = "",
                City = "San Carlos",
                RegionBvin = "CA",
                PostalCode = "94070",
                CountryBvin = Country.UnitedStatesCountryBvin,
                Phone = "(650) 123-4567"
            };

            while (currDate > endDate)
            {
                if (currDate != todaysDate)
                {
                    foreach (var product in products)
                    {
                        var order = new Order
                        {
                            bvin = Guid.NewGuid().ToString(),
                            StoreId = storeId,
                            BillingAddress = address,
                            ShippingAddress = address,
                            UserID = "1",
                            UserEmail = "info@hotcakescommerce.com",
                            OrderNumber = "Sample Order",
                            ShippingMethodId = "TOBEDETERMINED",
                            ShippingProviderId = string.Empty,
                            ShippingProviderServiceCode = string.Empty,
                            ShippingMethodDisplayName = "To Be Determined. Contact Store for Details",
                            PaymentStatus = OrderPaymentStatus.Paid,
                            ShippingStatus = OrderShippingStatus.Unshipped,
                            StatusCode = "F37EC405-1EC6-4a91-9AC4-6836215FBBBC",
                            StatusName = "Received",
                            IsPlaced = true,
                            IsAbandonedEmailSent = true,
                            UserDeviceType = (DeviceType) (index%3),
                            LastUpdatedUtc = currDate,
                            TimeOfOrderUtc = currDate
                        };
                        var qty = (currDate.Month + 4 + currDate.Year%2 + index%6)%12 + 4;
                        var lineItem = new LineItem
                        {
                            StoreId = storeId,
                            OrderBvin = order.bvin,
                            ProductId = product.Bvin,
                            ProductSku = product.Sku,
                            ProductName = product.ProductName,
                            Quantity = qty,
                            BasePricePerItem = product.SitePrice,
                            AdjustedPricePerItem = product.SitePrice,
                            LineTotal = product.SitePrice*qty,
                            ShipFromMode = ShippingMode.ShipFromSite,
                            LastUpdatedUtc = currDate
                        };
                        order.Items.Add(lineItem);
                        ordersList.Add(order);

                        index++;

                        var order2 = new Order
                        {
                            bvin = Guid.NewGuid().ToString(),
                            StoreId = storeId,
                            BillingAddress = address,
                            ShippingAddress = address,
                            UserID = "1",
                            UserEmail = "info@hotcakescommerce.com",
                            OrderNumber = "Sample Order",
                            ShippingMethodId = "TOBEDETERMINED",
                            ShippingProviderId = string.Empty,
                            ShippingProviderServiceCode = string.Empty,
                            ShippingMethodDisplayName = "To Be Determined. Contact Store for Details",
                            PaymentStatus = OrderPaymentStatus.Paid,
                            ShippingStatus = OrderShippingStatus.Unshipped,
                            StatusCode = OrderStatusCode.Received,
                            StatusName = "Received",
                            IsPlaced = false,
                            IsAbandonedEmailSent = true,
                            UserDeviceType = (DeviceType) (index%3),
                            LastUpdatedUtc = currDate,
                            TimeOfOrderUtc = currDate
                        };
                        var qty2 = (currDate.Month + 8 + currDate.Year%2 + index%6)%12 + 2;
                        var lineItem2 = new LineItem
                        {
                            StoreId = storeId,
                            OrderBvin = order2.bvin,
                            ProductId = product.Bvin,
                            ProductSku = product.Sku,
                            ProductName = product.ProductName,
                            Quantity = qty2,
                            BasePricePerItem = product.SitePrice,
                            AdjustedPricePerItem = product.SitePrice,
                            LineTotal = product.SitePrice*qty2,
                            ShipFromMode = ShippingMode.ShipFromSite,
                            LastUpdatedUtc = currDate
                        };
                        order2.Items.Add(lineItem2);
                        ordersList.Add(order2);

                        index++;
                    }
                }

                currDate = currDate.AddDays(-1);
            }
            var orderRepository = Factory.CreateRepo<OrderRepository>();
            orderRepository.BatchCreate(ordersList, false);

            var lineItemList = ordersList.SelectMany(o => o.Items).ToList();
            var lineItemRepository = Factory.CreateRepo<LineItemRepository>();
            lineItemRepository.BatchCreate(lineItemList);


            var orderTransactionList = new List<OrderTransaction>();
            foreach (var order in ordersList)
            {
                if (!order.IsPlaced)
                    continue;

                var orderTransaction = new OrderTransaction
                {
                    StoreId = storeId,
                    OrderId = order.bvin,
                    OrderNumber = "Sample Order",
                    TimeStampUtc = order.TimeOfOrderUtc,
                    Action = ActionType.CashReceived,
                    Amount = order.TotalGrand,
                    Success = true
                };
                orderTransactionList.Add(orderTransaction);
            }
            var orderTransactionRepository = Factory.CreateRepo<OrderTransactionRepository>();

            orderTransactionRepository.BatchCreate(orderTransactionList);
        }

        internal void DeleteSampleAnalytics(bool forSampleDataOnly)
        {
            using (var db = Factory.CreateHccDbContext())
            {
                var storeId = Context.CurrentStore.Id;
                db.DeleteSampleAnalytics(storeId, forSampleDataOnly);
            }
        }

        private void DeleteSampleCategoryBySlug(string slug)
        {
            var c = CatalogServices.Categories.FindBySlug(slug);
            if (c != null)
                CatalogServices.DestroyCategory(c.Bvin);
        }

        private void DeleteSampleProduct(string sku)
        {
            var p = CatalogServices.Products.FindBySku(sku);
            if (p != null)
                CatalogServices.DestroyProduct(p.Bvin, p.StoreId);
        }

        private List<Category> CreateSampleCategories()
        {
            var samples = new List<Category>();
            samples.Add(CreateSampleCategory("Sample Products", "sample-products", "This is a sample category", "Grid"));
            samples.Add(CreateSampleCategory("More Sample Products", "more-sample", string.Empty, "DetailedList"));
            samples.Add(CreateSampleCategory("Demo Category", "demo-category", string.Empty, "BulkQuantityList"));

            return samples;
        }

        private Category CreateSampleCategory(string name, string url, string description, string template)
        {
            var result = new Category();
            result.Name = name;
            result.RewriteUrl = url;
            result.Description = description;
            result.MetaTitle = Text.TrimToLength(name, 250);
            result.MetaDescription = Text.TrimToLength(description, 250);
            result.Keywords = Text.TrimToLength(name, 250);
            result.ShowTitle = true;
            result.ShowInTopMenu = true;
            result.TemplateName = template;
            CatalogServices.Categories.Create(result);
            return result;
        }

        private List<Product> CreateSampleProducts(List<Category> categories)
        {
            var sample1 = CreateBlueBracelet();
            var sample2 = CreateBrownFedora();
            var sample3 = CreateButterflyEarrings();
            var sample4 = CreateCupCake();
            var sample5 = CreateLaptop();
            var sample6 = CreateShirt();

            AssignToCategories(categories, sample1);
            AssignToCategories(categories, sample2);
            AssignToCategories(categories, sample3);
            AssignToCategories(categories, sample4);
            AssignToCategories(categories, sample5);
            AssignToCategories(categories, sample6);

            return new List<Product> {sample1, sample2, sample3, sample4, sample5, sample6};
        }

        private void AssignToCategories(List<Category> cats, Product p)
        {
            foreach (var c in cats)
            {
                CatalogServices.CategoriesXProducts.AddProductToCategory(p.Bvin, c.Bvin);
            }
        }

        private Product CreateBlueBracelet()
        {
            var p = new Product();
            p.Sku = "SAMPLE001";
            p.ProductName = "Blue Bracelet";
            p.Featured = true;
            p.IsSearchable = true;
            p.AllowUpcharge = false;
            p.UpchargeAmount = Constants.UpchargeAmount;
            p.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            p.ImageFileSmall = "BraceletBlue.png";
            p.ImageFileMedium = "BraceletBlue.png";
            p.ImageFileSmallAlternateText = "Blue Bracelet SAMPLE001";
            p.InventoryMode = ProductInventoryMode.AlwayInStock;
            p.Keywords = "bracelett";
            p.ListPrice = 0;
            p.LongDescription =
                "An incredible blue bracelet sample product. This item is not actually for sale. It is a sample product to demonstrate how your store may look with products loaded";
            p.MetaDescription = "Sample Blue Bracelet for Demonstration";
            p.MetaKeywords = "Blue,Bracelet,Sample,Demo";
            p.MetaTitle = "Sample Blue Bracelet";
            p.SitePrice = 42.95m;
            p.Status = ProductStatus.Active;
            p.UrlSlug = "blue-bracelet";
            p.Tabs.Add(new ProductDescriptionTab
            {
                TabTitle = "Sustainability",
                HtmlData =
                    "<p>All of our jewelry products are recycled and made in sustainable eco-friendly environments</p>"
            });
            CatalogServices.ProductsCreateWithInventory(p, true);

            DiskStorage.CopyDemoProductImage(p.StoreId, p.Bvin, p.ImageFileSmall);

            // Create Review
            var review = new ProductReview();
            review.Approved = true;
            review.Description =
                "This is a great bracelet. I would recommend it to anyone who appreciates sample products. Easy to wear!";
            review.ProductBvin = p.Bvin;
            review.Rating = ProductReviewRating.FourStars;
            CatalogServices.ProductReviews.Create(review);

            return p;
        }

        private Product CreateBrownFedora()
        {
            var p = new Product();
            p.Sku = "SAMPLE004";
            p.ProductName = "Brown Fedora";
            p.Featured = true;
            p.IsSearchable = true;
            p.AllowUpcharge = false;
            p.UpchargeAmount = Constants.UpchargeAmount;
            p.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            p.ImageFileSmall = "brown-fedora-01.jpg";
            p.ImageFileMedium = "brown-fedora-01.jpg";
            p.ImageFileSmallAlternateText = "Brown Fedora SAMPLE004";
            p.InventoryMode = ProductInventoryMode.AlwayInStock;
            p.LongDescription =
                "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
            p.SitePrice = 59.87m;
            p.Status = ProductStatus.Active;
            p.UrlSlug = "brown-fedora";
            CatalogServices.ProductsCreateWithInventory(p, true);

            DiskStorage.CopyDemoProductImage(p.StoreId, p.Bvin, p.ImageFileSmall);

            return p;
        }

        private Product CreateButterflyEarrings()
        {
            var p = new Product();
            p.Sku = "SAMPLE006";
            p.ProductName = "Butterfly Earrings";
            p.Featured = true;
            p.IsSearchable = true;
            p.AllowUpcharge = false;
            p.UpchargeAmount = Constants.UpchargeAmount;
            p.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            p.ImageFileSmall = "Earrings.jpg";
            p.ImageFileMedium = "Earrings.jpg";
            p.ImageFileSmallAlternateText = "Butterfly Earring SAMPLE006";
            p.InventoryMode = ProductInventoryMode.AlwayInStock;
            p.LongDescription =
                "Sample Butterfly Earrings Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
            p.SitePrice = 29.95m;
            p.Status = ProductStatus.Active;
            p.UrlSlug = "butterfly-earrings";
            CatalogServices.ProductsCreateWithInventory(p, true);

            DiskStorage.CopyDemoProductImage(p.StoreId, p.Bvin, p.ImageFileSmall);

            return p;
        }

        private Product CreateCupCake()
        {
            var p = new Product();
            p.Sku = "SAMPLE002";
            p.ProductName = "Cupcake Sample";
            p.Featured = true;
            p.IsSearchable = true;
            p.AllowUpcharge = false;
            p.UpchargeAmount = Constants.UpchargeAmount;
            p.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            p.ImageFileSmall = "CupCake.jpg";
            p.ImageFileMedium = "CupCake.jpg";
            p.ImageFileSmallAlternateText = "Cupcake Sample SAMPLE002";
            p.InventoryMode = ProductInventoryMode.AlwayInStock;
            p.LongDescription =
                "Savor this sweet treat from our famous collection of sample items. This product is not for sale and is a demonstration of how items could appear in your store";
            p.MetaDescription = "Vanilla Cupcake with Rich Frosting";
            p.MetaKeywords = "cup,cake,cupcake,valentine,small,treats,baked goods";
            p.MetaTitle = "Vanilla Cupcake with Rich Frosting";
            p.SitePrice = 1.99m;
            p.Status = ProductStatus.Active;
            p.UrlSlug = "cupcake-sample";
            CatalogServices.ProductsCreateWithInventory(p, true);

            DiskStorage.CopyDemoProductImage(p.StoreId, p.Bvin, p.ImageFileSmall);

            return p;
        }

        private Product CreateLaptop()
        {
            var p = new Product();
            p.Sku = "SAMPLE005";
            p.ProductName = "Laptop Computer Sample";
            p.Featured = true;
            p.IsSearchable = true;
            p.AllowUpcharge = false;
            p.UpchargeAmount = Constants.UpchargeAmount;
            p.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            p.ImageFileSmall = "Laptop.jpg";
            p.ImageFileMedium = "Laptop.jpg";
            p.ImageFileSmallAlternateText = "Laptop Computer Sample SAMPLE005";
            p.InventoryMode = ProductInventoryMode.AlwayInStock;
            p.LongDescription =
                "This is a sample laptop computer. It is not for sale and is a demonstration of what products could look like in your store";
            p.MetaTitle = "Laptop Sample";
            p.ListPrice = 1999.00m;
            p.SitePrice = 1299.00m;
            p.Status = ProductStatus.Active;
            p.UrlSlug = "laptop-computer-sample";
            CatalogServices.ProductsCreateWithInventory(p, true);

            var opt1 = Option.Factory(OptionTypes.DropDownList);
            opt1.IsShared = false;
            opt1.IsVariant = true;
            opt1.Name = "Screen Size";
            opt1.NameIsHidden = false;
            CatalogServices.ProductOptions.Create(opt1);
            opt1.AddItem("15 inch LCD");
            opt1.AddItem("17 inch LCD");
            CatalogServices.ProductOptions.Update(opt1);
            CatalogServices.ProductsAddOption(p, opt1.Bvin);

            var opt2 = Option.Factory(OptionTypes.RadioButtonList);
            opt2.IsShared = false;
            opt2.IsVariant = true;
            opt2.Name = "Memory (RAM)";
            opt2.NameIsHidden = false;
            CatalogServices.ProductOptions.Create(opt2);
            opt2.AddItem("2GB");
            opt2.AddItem("4GB");
            opt2.AddItem("8GB");
            CatalogServices.ProductOptions.Update(opt2);
            CatalogServices.ProductsAddOption(p, opt2.Bvin);

            var opt3 = Option.Factory(OptionTypes.CheckBoxes);
            opt3.IsShared = false;
            opt3.IsVariant = false;
            opt3.Name = "Warranty";
            opt3.NameIsHidden = false;
            CatalogServices.ProductOptions.Create(opt3);
            opt3.AddItem(new OptionItem {Name = "3 Year Extended Warranty [+$199]", PriceAdjustment = 199});
            CatalogServices.ProductOptions.Update(opt3);
            CatalogServices.ProductsAddOption(p, opt3.Bvin);

            int possibleCount;
            CatalogServices.VariantsGenerateAllPossible(p, out possibleCount);

            DiskStorage.CopyDemoProductImage(p.StoreId, p.Bvin, p.ImageFileSmall);

            return p;
        }

        private Product CreateShirt()
        {
            var p = new Product();
            p.Sku = "SAMPLE003";
            p.ProductName = "Purple Top";
            p.Featured = true;
            p.IsSearchable = true;
            p.AllowUpcharge = false;
            p.UpchargeAmount = Constants.UpchargeAmount;
            p.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            p.ImageFileSmall = "PurpleTop.jpg";
            p.ImageFileMedium = "PurpleTop.jpg";
            p.ImageFileSmallAlternateText = "Purple Top SAMPLE003";
            p.InventoryMode = ProductInventoryMode.AlwayInStock;
            p.LongDescription =
                "This sample purple top is made of 100% cotton and is made in the USA. Durable and easy to clean, this versatile top will be one of the best sample items not available for purchase you've ever seen!";
            p.MetaTitle = "Purple Top";
            p.SitePrice = 39.95m;
            p.Status = ProductStatus.Active;
            p.UrlSlug = "purple-top";
            CatalogServices.ProductsCreateWithInventory(p, true);

            var opt1 = Option.Factory(OptionTypes.DropDownList);
            opt1.IsShared = false;
            opt1.IsVariant = true;
            opt1.Name = "Size";
            opt1.NameIsHidden = false;
            CatalogServices.ProductOptions.Create(opt1);
            opt1.AddItem("Small");
            opt1.AddItem("Medium");
            opt1.AddItem("Large");
            CatalogServices.ProductOptions.Update(opt1);
            CatalogServices.ProductsAddOption(p, opt1.Bvin);

            var opt2 = Option.Factory(OptionTypes.DropDownList);
            opt2.IsShared = false;
            opt2.IsVariant = true;
            opt2.Name = "Sleeve Length";
            opt2.NameIsHidden = false;
            CatalogServices.ProductOptions.Create(opt2);
            opt2.AddItem("Short");
            opt2.AddItem("Long");
            CatalogServices.ProductOptions.Update(opt2);
            CatalogServices.ProductsAddOption(p, opt2.Bvin);

            int possibleCount;
            CatalogServices.VariantsGenerateAllPossible(p, out possibleCount);

            DiskStorage.CopyDemoProductImage(p.StoreId, p.Bvin, p.ImageFileSmall);

            return p;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Carries the request context used by asynchronous sample-data generation.
    /// </summary>
    public class SampleDataConfiguration
    {
        /// <summary>
        /// Gets or sets the request context restored by the background analytics operation.
        /// </summary>
        public HccRequestContext HccRequestContext { get; set; }
    }
}
