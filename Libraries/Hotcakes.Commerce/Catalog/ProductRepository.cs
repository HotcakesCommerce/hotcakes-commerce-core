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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    [Serializable]
    public class ProductRepository : HccLocalizationRepoBase<hcc_Product, hcc_ProductTranslation, Product, Guid>,
        IProductRepository
    {
        #region Constructor

        public ProductRepository(HccRequestContext c)
            : base(c)
        {
            imageRepository = new ProductImageRepository(c);
            variantRepository = new VariantRepository(c);
            optionRespository = new OptionRepository(c);
            bundleRepository = new BundledProductsRepository(c);
            reviewRepository = new ProductReviewRepository(Context);
        }

        #endregion

        #region Fields

        protected OptionRepository optionRespository;
        protected VariantRepository variantRepository;
        protected ProductImageRepository imageRepository;
        protected ProductReviewRepository reviewRepository;
        protected BundledProductsRepository bundleRepository;

        #endregion

        #region Public methods

        public override bool Create(Product item)
        {
            item.LastUpdated = DateTime.UtcNow;
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;

            return base.Create(item);
        }

        public virtual bool Update(Product c, bool mergeSubItems = true)
        {
            var result = UpdateAdv(c, mergeSubItems);
            return result.Success;
        }

        public virtual DalSingleOperationResult<Product> UpdateAdv(Product c, bool mergeSubItems = true)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return new DalSingleOperationResult<Product>();
            }
            CacheManager.ClearProduct(c.Bvin, c.StoreId, Context.MainContentCulture);
            c.LastUpdated = DateTime.UtcNow;
            var bvinId = DataTypeHelper.BvinToGuid(c.Bvin);
            return UpdateAdv(c, y => y.bvin == bvinId, false, mergeSubItems);
        }

        public virtual bool Delete(string bvin)
        {
            CacheManager.ClearProduct(bvin, Context.CurrentStore.Id);
            var bvinId = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == bvinId);
        }

        public virtual bool DeleteForStore(string bvin, long storeId)
        {
            CacheManager.ClearProduct(bvin, storeId);
            var bvinId = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == bvinId && y.StoreId == Context.CurrentStore.Id);
        }

        public Product FindWithCache(string bvin)
        {
            var result = CacheManager.GetProduct(bvin, Context.CurrentStore.Id, Context.MainContentCulture);
            if (result != null) return result;

            return Find(bvin);
        }

        /// <summary>
        ///     Finds the specified bvin. This methods only populates cache but shouldn't use it to get items
        /// </summary>
        /// <param name="bvin">The bvin.</param>
        /// <returns></returns>
        public Product Find(string bvin)
        {
            var result = FindForAllStores(bvin);

            if (result != null && result.StoreId == Context.CurrentStore.Id)
            {
                //add product to cache
                CacheManager.AddProduct(result);
                return result;
            }

            return null;
        }

        public Product FindForAllStores(string bvin)
        {
            var bvinId = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == bvinId);
        }

        public Product FindBySlug(string urlSlug)
        {
            return FindBySlugForStore(urlSlug, Context.CurrentStore.Id);
        }

        public Product FindBySlugForStore(string urlSlug, long storeId)
        {
            var data = FindFirstPoco(y => y.Item.RewriteUrl == urlSlug && y.Item.StoreId == storeId);
            if (data != null)
            {
                CacheManager.AddProduct(data);
                return data;
            }
            return null;
        }

        public Product FindBySku(string sku)
        {
            return FindBySkuForStore(sku, Context.CurrentStore.Id);
        }

        public Product FindBySkuForStore(string sku, long storeId)
        {
            var data = FindFirstPoco(y => y.Item.SKU == sku && y.Item.StoreId == storeId);
            if (data != null)
            {
                CacheManager.AddProduct(data);
                return data;
            }
            return null;
        }

        public bool IsSkuExist(string sku, Guid? excludeProductId = null)
        {
            sku = sku.ToLower();
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                var q = s.GetQuery();
                if (excludeProductId.HasValue)
                {
                    q = q.Where(i => i.bvin != excludeProductId);
                }

                var exist = q.Any(i => i.StoreId == storeId && i.SKU.ToLower() == sku);

                if (!exist)
                {
                    exist = variantRepository.IsSkuExist(sku, excludeProductId);
                }

                return exist;
            }
        }

        public int FindAllCount()
        {
            var storeId = Context.CurrentStore.Id;
            return FindAllCount(storeId);
        }

        public int FindAllCount(long storeId)
        {
            using (var s = CreateStrategy())
            {
                return GetSecureQueryForStore(s, storeId).Count();
            }
        }

        public int FindAllForAllStoresCount()
        {
            return CountOfAll();
        }

        public int FindCountByProductType(string productTypeId)
        {
            var productTypeGuid = DataTypeHelper.BvinToNullableGuid(productTypeId);
            using (var s = CreateStrategy())
            {
                var result = GetSecureQueryForCurrentStore(s)
                    .Where(y => y.Item.ProductTypeId == productTypeGuid)
                    .Count();
                return result;
            }
        }

        public override List<Product> FindAllPaged(int pageNumber, int pageSize)
        {
            using (var s = CreateStrategy())
            {
                IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> items;
                items = GetSecureQueryForCurrentStore(s).OrderBy(y => y.ItemTranslation.ProductName);
                items = GetPagedItems(items, pageNumber, pageSize);
                var products = ListPoco(items);
                products.ForEach(p => CacheManager.AddProduct(p));
                return products;
            }
        }

        public List<Product> FindAllPagedWithCache(int pageNumber, int pageSize)
        {
            using (var s = CreateStrategy())
            {
                IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> items;
                items = GetSecureQueryForCurrentStore(s).OrderBy(y => y.ItemTranslation.ProductName);
                items = GetPagedItems(items, pageNumber, pageSize);

                var guids = items.Select(i => i.Item.bvin).ToList();
                var bvins = guids.Select(g => DataTypeHelper.GuidToBvin(g)).ToList();

                return FindManyWithCache(bvins);
            }
        }

        public List<Product> FindAllPagedForAllStores(int pageNumber, int pageSize)
        {
            using (var s = CreateStrategy())
            {
                IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> items;
                items = GetSecureQuery(s).OrderBy(y => y.ItemTranslation.ProductName);
                items = GetPagedItems(items, pageNumber, pageSize);

                var products = ListPoco(items);
                products.ForEach(p => CacheManager.AddProduct(p));
                return products;
            }
        }

        public List<Product> FindAllPagedForAllStoresWithCache(int pageNumber, int pageSize)
        {
            using (var s = CreateStrategy())
            {
                IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> items;
                items = GetSecureQuery(s).OrderBy(y => y.ItemTranslation.ProductName);
                items = GetPagedItems(items, pageNumber, pageSize);
                var guids = items.Select(i => i.Item.bvin).ToList();
                var bvins = guids.Select(g => DataTypeHelper.GuidToBvin(g)).ToList();
                return FindManyWithCache(bvins);
            }
        }

        public int FindCountByCriteria(ProductSearchCriteria criteria)
        {
            using (var s = CreateStrategy())
            {
                var query = BuildCriteriaQuery(criteria, s);
                return query.Count();
            }
        }

        public List<Product> FindByCriteria(ProductSearchCriteria criteria, bool useCache = true)
        {
            var temp = -1;
            return FindByCriteria(criteria, 1, int.MaxValue, ref temp);
        }

        public List<Product> FindByCriteria(ProductSearchCriteria criteria, int pageNumber, int pageSize,
            ref int totalCount, bool useCache = true)
        {
            using (var s = CreateStrategy())
            {
                var query = BuildCriteriaQuery(criteria, s);

                // Get Total Count
                totalCount = query.Count();

                // Get Paged            
                query = GetPagedItems(query, pageNumber, pageSize);

                if (useCache)
                {
                    var guids = query.Select(i => i.Item.bvin).ToList();
                    var bvins = guids.Select(g => DataTypeHelper.GuidToBvin(g)).ToList();
                    return FindManyWithCache(bvins);
                }
                return ListPoco(query);
            }
        }

        public int FindProductsCountByCriteria(ProductSearchCriteria criteria, bool useCache = true)
        {
            var totalCount = 0;
            using (var s = CreateStrategy())
            {
                var query = BuildCriteriaQuery(criteria, s);

                // Get Total Count
                totalCount = query.Count();
            }
            return totalCount;
        }

        private IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> BuildCriteriaQuery(
            ProductSearchCriteria criteria, IRepoStrategy<hcc_Product> s)
        {
            var items = GetSecureQueryForCurrentStore(s);

            var categoryGuid = DataTypeHelper.BvinToNullableGuid(criteria.CategoryId);
            var productTypeGuid = DataTypeHelper.BvinToNullableGuid(criteria.ProductTypeId);
            var manufacturerGuid = DataTypeHelper.BvinToNullableGuid(criteria.ManufacturerId);
            var vendorGuid = DataTypeHelper.BvinToNullableGuid(criteria.VendorId);

            if (criteria.NotCategorized)
            {
                items = items.Where(y => !y.Item.hcc_ProductXCategory.Any());
            }
            else if (categoryGuid.HasValue)
            {
                items =
                    items.Where(
                        y =>
                            y.Item.hcc_ProductXCategory.Where(z => z.CategoryId == categoryGuid).FirstOrDefault() !=
                            null);
            }

            // Display Inactive
            if (!criteria.DisplayInactiveProducts)
            {
                items = items.Where(y => y.Item.Status == 1);
            }
            // Display Bundles
            if (!criteria.DisplayBundles)
            {
                items = items.Where(y => !y.Item.IsBundle);
            }
            // Display Recurring
            if (!criteria.DisplayRecurring)
            {
                items = items.Where(y => !y.Item.IsRecurring);
            }
            // Display Gift Cards
            if (!criteria.DisplayGiftCards)
            {
                items = items.Where(y => !y.Item.IsGiftCard);
            }
            // Status
            if (criteria.Status != ProductStatus.NotSet)
            {
                items = items.Where(y => y.Item.Status == (int) criteria.Status);
            }
            // Inventory Status
            if (criteria.InventoryStatus != ProductInventoryStatus.NotSet)
            {
                if (criteria.InventoryStatus == ProductInventoryStatus.Available)
                {
                    // show products, excluding those that are not available due to inventory settings
                    items =
                        items.Where(
                            y =>
                                y.Item.IsAvailableForSale ||
                                y.Item.OutOfStockMode != (int) ProductInventoryMode.WhenOutOfStockHide);
                }
                else if (criteria.InventoryStatus == ProductInventoryStatus.NotAvailable)
                {
                    // show only products that are not currently availabe in the store due to inventory settings
                    items =
                        items.Where(
                            y =>
                                !y.Item.IsAvailableForSale &&
                                y.Item.OutOfStockMode == (int) ProductInventoryMode.WhenOutOfStockHide);
                }
                else
                {
                    // filter by products with inventory levels that are set (ignore zeroes)
                    items =
                        items.Where(
                            y =>
                                y.Item.hcc_ProductInventory.Where(
                                    z =>
                                        z.QuantityOnHand <= z.LowStockPoint &&
                                        z.QuantityOnHand != 0 & z.LowStockPoint != 0).Any());
                }
            }
            // Product Type
            if (productTypeGuid.HasValue)
            {
                items = items.Where(y => y.Item.ProductTypeId == productTypeGuid);
            }
            // Manufacturer
            if (manufacturerGuid.HasValue)
            {
                items = items.Where(y => y.Item.ManufacturerID == manufacturerGuid);
            }
            // Vendor
            if (vendorGuid.HasValue)
            {
                items = items.Where(y => y.Item.VendorID == vendorGuid);
            }
            // Keywords
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                items = items.Where(y => y.Item.SKU.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.ProductName.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.MetaDescription.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.MetaKeywords.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.ShortDescription.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.LongDescription.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.Keywords.Contains(criteria.Keyword) ||
                                         y.ItemTranslation.HiddenSearchKeywords.Contains(criteria.Keyword)
                    );
            }

            if (!criteria.DisplayProductWithChoice)
            {
                var optionQry = s.GetQuery<hcc_ProductXOption>();
                items = items.Where(y => optionQry.Where(x => x.ProductBvin == y.Item.bvin).Count() <= 0)
                    .Where(
                        y =>
                            y.Item.hcc_BundledProducts.Where(
                                x => optionQry.Any(xx => xx.ProductBvin == x.BundledProductId)).Count() <= 0);
            }

            switch (criteria.CategorySort)
            {
                case CategorySortOrder.ProductName:
                    items = items.OrderBy(y => y.ItemTranslation.ProductName);
                    break;
                case CategorySortOrder.ProductNameDescending:
                    items = items.OrderByDescending(y => y.ItemTranslation.ProductName);
                    break;
                case CategorySortOrder.ProductPriceAscending:
                    items = items.OrderBy(y => y.Item.SitePrice);
                    break;
                case CategorySortOrder.ProductPriceDescending:
                    items = items.OrderByDescending(y => y.Item.SitePrice);
                    break;
                case CategorySortOrder.ProductSKUAscending:
                    items = items.OrderBy(y => y.Item.SKU);
                    break;
                case CategorySortOrder.ProductSKUDescending:
                    items = items.OrderByDescending(y => y.Item.SKU);
                    break;
                default:
                    if (categoryGuid.HasValue)
                        items =
                            items.OrderBy(
                                y =>
                                    y.Item.hcc_ProductXCategory.Where(z => z.CategoryId == categoryGuid)
                                        .Select(z => z.SortOrder)
                                        .FirstOrDefault());
                    else
                        items = items.OrderBy(y => y.ItemTranslation.ProductName);
                    break;
            }
            return items;
        }

        public List<Product> FindFeatured(int pageNumber, int pageSize)
        {
            var bvins = FindFeaturedProductBvins(pageNumber, pageSize);
            return FindManyWithCache(bvins);
        }

        public List<Product> FindMany(IEnumerable<string> productBvins)
        {
            if (productBvins.Count() == 0)
                return new List<Product>();

            using (var s = CreateStrategy())
            {
                var productGuids = productBvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();

                IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> items;
                items = GetSecureQueryForCurrentStore(s);
                items = items.Where(pj => productGuids.Contains(pj.Item.bvin));

                var products = ListPoco(items);
                products.ForEach(p => CacheManager.AddProduct(p));
                return products;
            }
        }

        public List<Product> FindManyWithCache(IEnumerable<string> productBvins)
        {
            var cachedProducts = new List<Product>();
            var needsRetrieval = new List<string>();
            foreach (var productBvin in productBvins)
            {
                var product = CacheManager.GetProduct(productBvin, Context.CurrentStore.Id, Context.MainContentCulture);
                if (product != null)
                    cachedProducts.Add(product);
                else
                    needsRetrieval.Add(productBvin);
            }
            var retrievedProducts = FindMany(needsRetrieval);
            retrievedProducts.ForEach(p => CacheManager.AddProduct(p));

            var result = new List<Product>();
            foreach (var productBvin in productBvins)
            {
                var cached = cachedProducts.Where(p => p.Bvin == productBvin).FirstOrDefault();
                if (cached != null)
                {
                    result.Add(cached);
                }
                else
                {
                    var retrieved = retrievedProducts.Where(p => p.Bvin == productBvin).FirstOrDefault();
                    if (retrieved != null)
                        result.Add(retrieved);
                }
            }
            return result;
        }

        public List<Product> FindManySkus(List<string> skus)
        {
            using (var s = CreateStrategy())
            {
                var items = GetSecureQueryForCurrentStore(s)
                    .Where(y => skus.Contains(y.Item.SKU))
                    .OrderBy(y => y.Item.Id);
                var guids = items.Select(i => i.Item.bvin).ToList();
                var bvins = guids.Select(g => DataTypeHelper.GuidToBvin(g)).ToList();

                return FindManyWithCache(bvins);
            }
        }

        public List<string> FindFeaturedProductBvins(int pageNumber, int pageSize)
        {
            IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> items;
            using (var s = CreateStrategy())
            {
                items = GetSecureQueryForCurrentStore(s)
                    .Where(y => y.Item.Featured)
                    .Where(y => y.Item.Status == 1)
                    .OrderByDescending(y => y.Item.LastUpdated);

                items = GetPagedItems(items, pageNumber, pageSize);

                return items
                    .Select(y => y.Item.bvin).ToList()
                    .Select(g => DataTypeHelper.GuidToBvin(g)).ToList();
            }
        }

        public List<string> FindAllBvinsForStore(long storeId)
        {
            using (var s = CreateStrategy())
            {
                return GetSecureQueryForCurrentStore(s)
                    .Select(y => y.Item.bvin).ToList()
                    .Select(g => DataTypeHelper.GuidToBvin(g)).ToList();
            }
        }

        public bool Clone(string productId, string newSku, string newUrlSlug, ProductStatus newStatus, bool cloneImages,
            out string newProductId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);

            using (var s = CreateStrategy())
            {
                var product = s.GetQuery()
                    .Where(p => p.bvin == productGuid)
                    .FirstOrDefault();
                s.Detach(product);

                var newProductGuid = Guid.NewGuid();
                product.bvin = newProductGuid;
                product.SKU = newSku;
                product.RewriteUrl = newUrlSlug;
                product.Status = (int) newStatus;
                product.LastUpdated = DateTime.UtcNow;

                if (!cloneImages)
                {
                    product.ImageFileMedium = string.Empty;
                    product.ImageFileSmall = string.Empty;
                }

                s.Add(product);

                var productTranslations = s.GetQuery<hcc_ProductTranslation>()
                    .Where(pt => pt.ProductId == productGuid)
                    .ToList();

                foreach (var productTranslation in productTranslations)
                {
                    s.Detach(productTranslation);

                    productTranslation.ProductId = newProductGuid;
                    if (!cloneImages)
                    {
                        productTranslation.MediumImageAlternateText = string.Empty;
                        productTranslation.SmallImageAlternateText = string.Empty;
                    }

                    s.AddEntity(productTranslation);
                }
                s.SubmitChanges();

                newProductId = newProductGuid.ToString();
            }

            return true;
        }

        public List<Product> GetMostPurchasedWith(string productBvin, SalesPeriod period, int maxItemsToReturn)
        {
            using (var context = Factory.CreateHccDbContext())
            {
                var storeId = Context.CurrentStore.Id;
                var productGuid = DataTypeHelper.BvinToGuid(productBvin);
                var range = DateHelper.GetDateRange(period);

                var productGuids = context.hcc_Order
                    .Where(o => o.StoreId == storeId && o.IsPlaced == 1
                                && o.TimeOfOrder >= range.StartDate && o.TimeOfOrder <= range.EndDate)
                    .Join(context.hcc_LineItem, o => o.bvin, li => li.OrderBvin, (o, li) => new {o, li})
                    .Join(context.hcc_Product, j => j.li.ProductId, p => p.bvin, (j, p) => new {j.o, j.li, p})
                    .Where(j => j.p.bvin != productGuid)
                    .Where(
                        j =>
                            context.hcc_LineItem.Where(li => li.ProductId == productGuid)
                                .Select(li => li.OrderBvin)
                                .Contains(j.o.bvin))
                    .GroupBy(j => j.p.bvin)
                    .Select(g => new {ProductId = g.Key, TotalOrdered = g.Sum(v => v.li.Quantity)})
                    .OrderBy(v => v.TotalOrdered)
                    .Select(v => v.ProductId)
                    .Take(maxItemsToReturn)
                    .ToList();

                var productIds = productGuids.Select(p => DataTypeHelper.GuidToBvin(p)).ToList();

                return FindManyWithCache(productIds);
            }
        }

        #endregion

        #region Implementation

        protected override Expression<Func<hcc_Product, Guid>> ItemKeyExp
        {
            get { return p => p.bvin; }
        }

        protected override Expression<Func<hcc_ProductTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return pt => pt.ProductId; }
        }

        protected override void CopyItemToModel(hcc_Product data, Product model)
        {
            model.Id = data.Id;
            model.AllowReviews = data.AllowReviews;
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.CreationDateUtc = data.CreationDate;
            model.CustomPropertiesFromXml(data.CustomProperties);
            model.Featured = data.Featured;
            model.IsSearchable = data.IsSearchable;
            model.GiftWrapAllowed = data.GiftWrapAllowed == 1 ? true : false;
            model.GiftWrapPrice = data.GiftWrapPrice;
            model.ImageFileMedium = data.ImageFileMedium;
            model.ImageFileSmall = data.ImageFileSmall;
            model.InventoryMode = (ProductInventoryMode) data.OutOfStockMode;
            model.IsAvailableForSale = data.IsAvailableForSale;
            model.IsUserSuppliedPrice = data.IsUserPrice;
            model.HideQty = data.HideQty;
            model.LastUpdated = data.LastUpdated;
            model.ListPrice = data.ListPrice;
            model.ManufacturerId = DataTypeHelper.GuidToBvin(data.ManufacturerID);
            model.MinimumQty = data.MinimumQty;
            model.PostContentColumnId = data.PostContentColumnId;
            model.PreContentColumnId = data.PreContentColumnId;
            model.ProductTypeId = DataTypeHelper.GuidToBvin(data.ProductTypeId);
            model.ShippingDetails.ExtraShipFee = data.ExtraShipFee;
            model.ShippingDetails.Height = data.ShippingHeight;
            model.ShippingDetails.IsNonShipping = data.NonShipping == 1 ? true : false;
            model.ShippingDetails.Length = data.ShippingLength;
            model.ShippingDetails.Weight = data.ShippingWeight;
            model.ShippingDetails.Width = data.ShippingWidth;
            model.ShippingDetails.ShipSeparately = data.ShipSeparately == 1 ? true : false;
            model.ShippingMode = (ShippingMode) data.ShippingMode;
            model.ShippingCharge = (ShippingChargeType) data.ShippingCharge;
            model.SiteCost = data.SiteCost;
            model.SitePrice = data.SitePrice;
            model.Sku = data.SKU;
            model.Status = (ProductStatus) data.Status;
            model.StoreId = data.StoreId;
            model.TaxExempt = data.TaxExempt == 1 ? true : false;
            var taxString = data.TaxClass;
            long tempTax = -1;
            if (long.TryParse(taxString, out tempTax)) model.TaxSchedule = tempTax;
            model.TemplateName = data.TemplateName;
            model.UrlSlug = data.RewriteUrl;
            model.VendorId = DataTypeHelper.GuidToBvin(data.VendorID);
            model.IsBundle = data.IsBundle;
            model.IsGiftCard = data.IsGiftCard;
            model.IsRecurring = data.IsRecurring;
            model.RecurringInterval = data.RecurringInterval ?? 0;
            model.RecurringIntervalType = (RecurringIntervalType) (data.RecurringIntervalType ?? 0);
        }

        protected override void CopyTransToModel(hcc_ProductTranslation data, Product model)
        {
            model.ContentCulture = data.Culture;
            model.ProductName = data.ProductName;
            model.ShortDescription = data.ShortDescription;
            model.LongDescription = data.LongDescription;
            model.ImageFileSmallAlternateText = data.SmallImageAlternateText;
            model.ImageFileMediumAlternateText = data.MediumImageAlternateText;
            model.MetaDescription = data.MetaDescription;
            model.MetaKeywords = data.MetaKeywords;
            model.MetaTitle = data.MetaTitle;
            model.Keywords = data.Keywords;
            model.UserSuppliedPriceLabel = data.UserPriceLabel;
            model.SitePriceOverrideText = data.SitePriceOverrideText;
            model.TabsFromXml(data.DescriptionTabs);
        }

        protected override void CopyModelToItem(JoinedItem<hcc_Product, hcc_ProductTranslation> data, Product model)
        {
            data.Item.AllowReviews = model.AllowReviews;
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.CreationDate = model.CreationDateUtc;
            data.Item.CustomProperties = model.CustomPropertiesToXml();
            data.Item.Featured = model.Featured;
            data.Item.IsSearchable = model.IsSearchable;
            data.Item.GiftWrapAllowed = model.GiftWrapAllowed ? 1 : 0;
            data.Item.GiftWrapPrice = model.GiftWrapPrice;
            data.Item.ImageFileMedium = model.ImageFileMedium;
            data.Item.ImageFileSmall = model.ImageFileSmall;
            data.Item.OutOfStockMode = (int) model.InventoryMode;
            data.Item.IsAvailableForSale = model.IsAvailableForSale;
            data.Item.IsUserPrice = model.IsUserSuppliedPrice;
            data.Item.HideQty = model.HideQty;
            data.Item.LastUpdated = model.LastUpdated;
            data.Item.ListPrice = model.ListPrice;
            data.Item.ManufacturerID = DataTypeHelper.BvinToNullableGuid(model.ManufacturerId);
            data.Item.MinimumQty = model.MinimumQty;
            data.Item.PostContentColumnId = model.PostContentColumnId;
            data.Item.PreContentColumnId = model.PreContentColumnId;
            data.Item.ProductTypeId = DataTypeHelper.BvinToNullableGuid(model.ProductTypeId);
            data.Item.ExtraShipFee = model.ShippingDetails.ExtraShipFee;
            data.Item.ShippingHeight = model.ShippingDetails.Height;
            data.Item.NonShipping = model.ShippingDetails.IsNonShipping ? 1 : 0;
            data.Item.ShippingLength = model.ShippingDetails.Length;
            data.Item.ShippingWeight = model.ShippingDetails.Weight;
            data.Item.ShippingWidth = model.ShippingDetails.Width;
            data.Item.ShipSeparately = model.ShippingDetails.ShipSeparately ? 1 : 0;
            data.Item.ShippingMode = (int) model.ShippingMode;
            data.Item.SiteCost = model.SiteCost;
            data.Item.SitePrice = model.SitePrice;
            data.Item.SKU = model.Sku;
            data.Item.Status = (int) model.Status;
            data.Item.StoreId = model.StoreId;
            data.Item.TaxExempt = model.TaxExempt ? 1 : 0;
            data.Item.TaxClass = model.TaxSchedule.ToString();
            data.Item.TemplateName = model.TemplateName;
            data.Item.RewriteUrl = model.UrlSlug;
            data.Item.VendorID = DataTypeHelper.BvinToNullableGuid(model.VendorId);
            data.Item.IsBundle = model.IsBundle;
            data.Item.IsGiftCard = model.IsGiftCard;
            data.Item.IsRecurring = model.IsRecurring;
            data.Item.RecurringInterval = model.RecurringInterval;
            data.Item.RecurringIntervalType = (int) model.RecurringIntervalType;
            data.Item.ShippingCharge = (int) model.ShippingCharge;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_Product, hcc_ProductTranslation> data, Product model)
        {
            data.ItemTranslation.ProductId = data.Item.bvin;

            data.ItemTranslation.ProductName = model.ProductName;
            data.ItemTranslation.ShortDescription = model.ShortDescription;
            data.ItemTranslation.LongDescription = model.LongDescription;
            data.ItemTranslation.SmallImageAlternateText = model.ImageFileSmallAlternateText;
            data.ItemTranslation.MediumImageAlternateText = model.ImageFileMediumAlternateText;
            data.ItemTranslation.MetaDescription = model.MetaDescription;
            data.ItemTranslation.MetaKeywords = model.MetaKeywords;
            data.ItemTranslation.MetaTitle = model.MetaTitle;
            data.ItemTranslation.Keywords = model.Keywords;
            data.ItemTranslation.UserPriceLabel = model.UserSuppliedPriceLabel;
            data.ItemTranslation.SitePriceOverrideText = model.SitePriceOverrideText;
            data.ItemTranslation.DescriptionTabs = model.TabsToXml();

            // Save Variant Skus to Searchable column
            var variantSkus = model.Variants.Select(y => y.Sku).ToList();
            var skuslist = string.Join(",", variantSkus.ToArray());
            data.ItemTranslation.HiddenSearchKeywords = skuslist;
        }

        protected override void DeleteAllSubItems(hcc_Product data)
        {
            var productBvin = DataTypeHelper.GuidToBvin(data.bvin);

            optionRespository.DeleteForProductId(productBvin);
        }

        protected override void GetSubItems(List<Product> models)
        {
            var productIds = models.Select(p => p.Bvin).ToList();
            var allImages = imageRepository.FindByProductIds(productIds);
            var allVariants = variantRepository.FindByProductIds(productIds);
            var allOptions = optionRespository.FindByProductIds(productIds);
            var allReviews = reviewRepository.FindByProductIds(productIds);
            var allBundledProducts = bundleRepository.FindForProducts(productIds);

            foreach (var model in models)
            {
                model.Options.Clear();
                model.Variants.Clear();

                model.Options.AddRange(allOptions[model.Bvin]);
                model.Variants.AddRange(allVariants.Where(v => v.ProductId == model.Bvin).ToList());

                model.Images = allImages.Where(i => i.ProductId == model.Bvin).ToList();
                model.Reviews = allReviews.Where(r => r.ProductBvin == model.Bvin).ToList();

                if (model.IsBundle)
                {
                    var bundledProducts = allBundledProducts.Where(bp => bp.ProductId == model.Bvin).ToList();
                    var bvins = bundledProducts.Select(bp => bp.BundledProductId).ToList();
                    var products = FindManyWithCache(bvins);

                    foreach (var bundledProduct in bundledProducts)
                    {
                        // preventative code to clean up imported or corrupted data
                        // products cannot be bundled to themselves
                        if (bundledProduct.BundledProductId.Equals(model.Bvin))
                        {
                            // remove the bundled product from the parent product
                            bundleRepository.Delete(bundledProduct.Id);
                            continue;
                        }

                        var bundledProductAdv = new BundledProductAdv(bundledProduct);
                        bundledProductAdv.BundledProduct = products.
                            Where(p => p.Bvin == bundledProduct.BundledProductId).
                            FirstOrDefault();
                        model.BundledProducts.Add(bundledProductAdv);
                    }
                }
            }
        }

        protected override void MergeSubItems(Product model)
        {
            variantRepository.MergeList(model.Bvin, model.Variants);
            optionRespository.MergeList(model.Bvin, model.Options);
            imageRepository.MergeList(model.Bvin, model.Images);
            reviewRepository.MergeList(model.Bvin, model.Reviews);
        }

        protected virtual IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> GetSecureQuery(
            IRepoStrategy<hcc_Product> strategy)
        {
            return GetJoinedQuery(strategy);
        }

        protected IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> GetSecureQueryForStore(
            IRepoStrategy<hcc_Product> strategy, long storeId)
        {
            return GetSecureQuery(strategy).Where(i => i.Item.StoreId == storeId);
        }

        protected IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> GetSecureQueryForCurrentStore(
            IRepoStrategy<hcc_Product> strategy)
        {
            return GetSecureQuery(strategy).Where(i => i.Item.StoreId == Context.CurrentStore.Id);
        }

        #endregion
    }
}