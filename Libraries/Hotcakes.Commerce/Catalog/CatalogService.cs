#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Service interface to talk with different catalog repositories for
    ///     product, category, variant, gift card, product properties to perform
    ///     different database operations.
    /// </summary>
    /// <remarks>All repository objects are read only to whole project.  Values cannot be set outside of this service class.</remarks>
    public abstract class CatalogService : HccServiceBase
    {
        /// <summary>
        ///     Initialize the repository from the database factory.
        /// </summary>
        /// <param name="context"></param>
        public CatalogService(HccRequestContext context)
            : base(context)
        {
            Categories = Factory.CreateRepo<CategoryRepository>(Context);
            Products = Factory.CreateRepo<ProductRepository>(Context);
            CategoriesXProducts = Factory.CreateRepo<CategoryProductAssociationRepository>(Context);
            ProductRelationships = Factory.CreateRepo<ProductRelationshipRepository>(Context);
            ProductImages = Factory.CreateRepo<ProductImageRepository>(Context);
            ProductVariants = Factory.CreateRepo<VariantRepository>(Context);
            ProductsXOptions = Factory.CreateRepo<ProductOptionAssociationRepository>(Context);
            ProductFiles = Factory.CreateRepo<ProductFileRepository>(Context);
            VolumeDiscounts = Factory.CreateRepo<ProductVolumeDiscountRepository>(Context);
            ProductInventories = Factory.CreateRepo<ProductInventoryRepository>(Context);
            ProductTypesXProperties = Factory.CreateRepo<ProductTypePropertyAssociationRepository>(Context);
            WishListItems = Factory.CreateRepo<WishListItemRepository>(Context);
            ProductReviews = Factory.CreateRepo<ProductReviewRepository>(Context);
            ProductOptions = Factory.CreateRepo<OptionRepository>(Context);
            ProductTypes = Factory.CreateRepo<ProductTypeRepository>(Context);
            ProductProperties = Factory.CreateRepo<ProductPropertyRepository>(Context);
            ProductPropertyValues = Factory.CreateRepo<ProductPropertyValueRepository>(Context);
            MembershipTypes = Factory.CreateRepo<MembershipProductTypeRepository>(Context);
            BundledProducts = Factory.CreateRepo<BundledProductsRepository>(Context);
            CatalogRoles = Factory.CreateRepo<CatalogRolesRepository>(Context);
            GiftCards = Factory.CreateRepo<GiftCardRepository>(Context);
            ProductPropertiesChoice = Factory.CreateRepo<ProductPropertyChoiceRepository>(Context);
        }

        /// <summary>
        ///     Category repository interface get resolved at run time based on the class impleted this interface.
        /// </summary>
        public ICategoryRepository Categories { get; protected set; }

        /// <summary>
        ///     Category and product mapping data repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductXCategory table.</remarks>
        public CategoryProductAssociationRepository CategoriesXProducts { get; protected set; }

        /// <summary>
        ///     Related product information repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductRelationships table.</remarks>
        public ProductRelationshipRepository ProductRelationships { get; protected set; }

        /// <summary>
        ///     Bundled products repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_BundledProducts table.</remarks>
        public BundledProductsRepository BundledProducts { get; protected set; }

        /// <summary>
        ///     Product images repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductImage table.</remarks>
        public ProductImageRepository ProductImages { get; protected set; }

        /// <summary>
        ///     Product reviews repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductReview table.</remarks>
        public ProductReviewRepository ProductReviews { get; protected set; }

        /// <summary>
        ///     Product options repository. This holds additional options available for any product.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductOptions table.</remarks>
        public OptionRepository ProductOptions { get; protected set; }

        /// <summary>
        ///     Products options which has been configured and set for the product. This options available when go to product
        ///     details page.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductXOption table.</remarks>
        public ProductOptionAssociationRepository ProductsXOptions { get; protected set; }

        /// <summary>
        ///     Products repository interface. This can be used to create the different class to as per business need to
        ///     perform operation on products.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_Product table.</remarks>
        public IProductRepository Products { get; protected set; }

        /// <summary>
        ///     Product files repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductFile table.</remarks>
        public ProductFileRepository ProductFiles { get; protected set; }

        /// <summary>
        ///     Product discounts information repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductVolumeDiscounts table.</remarks>
        public ProductVolumeDiscountRepository VolumeDiscounts { get; protected set; }

        /// <summary>
        ///     Product properties values repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductPropertyValue table.</remarks>
        public ProductPropertyValueRepository ProductPropertyValues { get; protected set; }

        /// <summary>
        ///     Product inventory information repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductInventory table.</remarks>
        public ProductInventoryRepository ProductInventories { get; protected set; }

        /// <summary>
        ///     Product types information repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductType and hcc_ProductTypeTranslation table.</remarks>
        public ProductTypeRepository ProductTypes { get; protected set; }

        /// <summary>
        ///     Product type properties information repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_ProductTypeXProductProperty table.</remarks>
        public ProductTypePropertyAssociationRepository ProductTypesXProperties { get; protected set; }

        /// <summary>
        ///     Product property information repository.
        /// </summary>
        /// <remarks>
        ///     Used to perform different queries and operation against hcc_ProductProperty and hcc_ProductPropertyTranslation
        ///     table.
        /// </remarks>
        public ProductPropertyRepository ProductProperties { get; protected set; }

        /// <summary>
        ///     Wishlist items information for users repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_WishListItem table.</remarks>
        public WishListItemRepository WishListItems { get; protected set; }

        /// <summary>
        ///     Product MembershipType information repository. Each product can be controlled by its type also based on the
        ///     membership assigned to product type.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_MembershipProductType table.</remarks>
        public MembershipProductTypeRepository MembershipTypes { get; protected set; }

        /// <summary>
        ///     Catalog roles information. Its get mapped to the product to control product view and operation.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_CatalogRoles table.</remarks>
        public CatalogRolesRepository CatalogRoles { get; protected set; }

        /// <summary>
        ///     Gift card information repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_GiftCard table.</remarks>
        public GiftCardRepository GiftCards { get; protected set; }

        /// <summary>
        ///     Product properties choice options information.
        /// </summary>
        /// <remarks>
        ///     Used to perform different queries and operation against hcc_ProductPropertyChoice and
        ///     hcc_ProductPropertyChoiceTranslation table.
        /// </remarks>
        public ProductPropertyChoiceRepository ProductPropertiesChoice { get; protected set; }

        /// <summary>
        ///     Product variant repository.
        /// </summary>
        /// <remarks>Used to perform different queries and operation against hcc_Variants table.</remarks>
        public VariantRepository ProductVariants { get; protected set; }


        // TODO: (REFACTORING) Move to other class (DiskStorage possible)
        /// <summary>
        ///     Editor page view URL generation based on the category id and its parent id.
        /// </summary>
        /// <param name="type">Different types of view available to see the category</param>
        /// <param name="bvin">Category unique id</param>
        /// <param name="parentid">Parent category unique id</param>
        /// <returns>Generated URL based on given criteria</returns>
        public string EditorRouteForCategory(CategorySourceType type, string bvin, string parentid)
        {
            var prefix = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/";

            if (!string.IsNullOrEmpty(bvin))
            {
                switch (type)
                {
                    //case CategorySourceType.DrillDown:
                    //	return prefix + "Categories_EditDrillDown.aspx?id=" + bvin;
                    case CategorySourceType.CustomLink:
                        return string.Format("{0}Categories_EditCustomLink.aspx?id={1}",
                            prefix,
                            bvin);
                    default:
                        return string.Format("{0}Categories_Edit.aspx?id={1}",
                            prefix,
                            bvin);
                }
            }
            switch (type)
            {
                case CategorySourceType.CustomLink:
                    return string.Format("{0}Categories_EditCustomLink.aspx?parentid={1}&type={2}",
                        prefix,
                        parentid,
                        type);
                default:
                    return string.Format("{0}Categories_Edit.aspx?parentid={1}&type={2}",
                        prefix,
                        parentid,
                        type);
            }
        }

        /// <summary>
        ///     Add product to wishlist for current user.
        /// </summary>
        /// <param name="p">Product which needs to be added in wishlist</param>
        /// <param name="selections">Options chosen by end user when adding product to wishlist.</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="app">Hotcakes application instance</param>
        /// <returns>Returns true if the product added successfully, otherwise false.</returns>
        public bool SaveProductToWishList(Product p, OptionSelections selections, int quantity, HotcakesApplication app)
        {
            var result = false;

            var products = WishListItems.FindByCustomerIdPaged(app.CurrentCustomerId, 1, int.MaxValue);
            var isProductExist = false;
            foreach (var product in products)
            {
                if (product.ProductId == p.Bvin)
                {
                    var areEqual = selections.Equals(product.SelectionData);
                    if (areEqual)
                    {
                        isProductExist = true;
                        product.Quantity += quantity;
                        WishListItems.Update(product);
                        result = true;
                    }
                }
            }

            if (!isProductExist)
            {
                var wi = new WishListItem();
                if (p != null)
                {
                    wi.ProductId = p.Bvin;
                    wi.Quantity = quantity;
                    wi.SelectionData = selections;
                    wi.CustomerId = app.CurrentCustomerId;
                }

                WishListItems.Create(wi);
                result = true;
            }

            return result;
        }

        #region Product Bundles

        /// <summary>
        ///     Create bundled product and add analytics information.
        /// </summary>
        /// <param name="bundledProduct"><see cref="BundledProduct" /> instance.</param>
        /// <returns>Returns true if the bundled product has been created successfully otherwise returns false.</returns>
        public bool BundledProductCreate(BundledProduct bundledProduct)
        {
            var result = BundledProducts.Create(bundledProduct);
            if (result)
            {
                RegisterAnalyticsEvents(bundledProduct);
            }
            return result;
        }

        #endregion

        #region GiftCards

        /// <summary>
        ///     Generate gift card number.
        /// </summary>
        /// <returns>Gift card number</returns>
        public string GenerateGiftCardNumber()
        {
            var useAZ = Context.CurrentStore.Settings.GiftCard.UseAZSymbols;
            var cardNumber = new StringBuilder(Context.CurrentStore.Settings.GiftCard.CardNumberFormat);
            var codeLength = cardNumber.ToString().Count(c => c == 'X' || c == 'x');
            var code = new StringBuilder();
            var random = new Random((int) DateTime.Now.Ticks);
            char ch;

            for (var i = 0; i < codeLength; i++)
            {
                if (useAZ && random.Next(2) == 0)
                {
                    ch = (char) random.Next('A', 'Z' + 1);
                }
                else
                {
                    ch = random.Next(0, 10).ToString()[0];
                }

                code.Append(ch);
            }

            for (int i = 0, j = 0; i < cardNumber.Length; i++)
            {
                ch = cardNumber[i];

                if (ch == 'X' || ch == 'x')
                {
                    cardNumber[i] = code[j];
                    j++;
                }
            }

            return cardNumber.ToString();
        }

        #endregion

        #region Categories

        /// <summary>
        ///     Update category information.
        /// </summary>
        /// <param name="category">Category instance which needs to be updated in database</param>
        /// <returns>Returns true if category updated successfully, otherwise false.</returns>
        public bool CategoryUpdate(Category category)
        {
            var result = Categories.UpdateAdv(category);
            if (result.Success)
            {
                RegisterAnalyticsEvents(result.OldValue, category);
            }
            return result.Success;
        }

        /// <summary>
        ///     Find all the categories in which specific product belongs to.
        /// </summary>
        /// <param name="productBvin">Product unique identifier. Usually be GUID.</param>
        /// <returns>List of categories returned in which given product belongs to.</returns>
        public List<CategorySnapshot> FindCategoriesForProduct(string productBvin)
        {
            var crosses = CategoriesXProducts.FindForProduct(productBvin, 1, int.MaxValue);

            if (crosses == null) return new List<CategorySnapshot>();

            var bvins = (from x in crosses
                         select x.CategoryId).ToList();

            return Categories.FindManySnapshots(bvins);
        }

        /// <summary>
        ///     Find list of the products for given category and in specific order.
        /// </summary>
        /// <remarks>Allow to control whether needs to get unavailable product in list or not.</remarks>
        /// <param name="categoryBvin">Category Unique identifier. Usually GUID.</param>
        /// <param name="sortOrder">
        ///     Order by criteria. More detail for ordering can be found at <see cref="CategorySortOrder" />
        /// </param>
        /// <param name="showUnavailable">
        ///     Show unavailable products or not. Unavailability generally determined by unavailability
        ///     rule set on product inventory in system.
        /// </param>
        /// <returns>Returns the list of the product under specific category.</returns>
        public List<Product> FindProductForCategoryWithSort(string categoryBvin, CategorySortOrder sortOrder,
            bool showUnavailable)
        {
            var temp = -1;
            var sensibleLimit = 2000;
            return FindProductForCategoryWithSort(categoryBvin, sortOrder, showUnavailable, 1, sensibleLimit, ref temp);
        }

        /// <summary>
        ///     Find list of the products for given category with paging.
        /// </summary>
        /// <param name="categoryBvin">Category Unique identifier. Usually GUID.</param>
        /// <param name="sortOrder">
        ///     Order by criteria. More detail for ordering can be found at <see cref="CategorySortOrder" />
        /// </param>
        /// <param name="showUnavailable">
        ///     Show unavailable products or not. Unavailability generally determined by unavailability
        ///     rule set on product inventory in system.
        /// </param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="rowCount">Number of records available in database for given criteria.</param>
        /// <returns>Returns the list of the products for given criteria.</returns>
        public List<Product> FindProductForCategoryWithSort(string categoryBvin, CategorySortOrder sortOrder,
            bool showUnavailable,
            int pageNumber, int pageSize, ref int rowCount)
        {
            var criteria = new ProductSearchCriteria
            {
                CategorySort = sortOrder,
                CategoryId = categoryBvin,
                DisplayInactiveProducts = showUnavailable,
                InventoryStatus = showUnavailable ? ProductInventoryStatus.NotSet : ProductInventoryStatus.Available
            };

            return Products.FindByCriteria(criteria, pageNumber, pageSize, ref rowCount);
        }

        /// <summary>
        ///     Get products count for the given category.
        /// </summary>
        /// <param name="categoryBvin">Category Unique identifier. Usually GUID.</param>
        /// <param name="showUnavailable">
        ///     Show unavailable products or not. Unavailability generally determined by unavailability
        ///     rule set on product inventory in system.
        /// </param>
        /// <returns>Return the count of the product for given criteria.</returns>
        public int FindProductCountsForCategory(string categoryBvin, bool showUnavailable)
        {
            var criteria = new ProductSearchCriteria
            {
                CategoryId = categoryBvin,
                DisplayInactiveProducts = showUnavailable,
                InventoryStatus = showUnavailable ? ProductInventoryStatus.NotSet : ProductInventoryStatus.Available
            };

            return Products.FindProductsCountByCriteria(criteria);
        }


        /// <summary>
        ///     Gives list of available sorting option for the category.
        /// </summary>
        /// <returns>List of category order option</returns>
        public List<CategorySortOrder> GetCategorySortOrderList()
        {
            var orders = new List<CategorySortOrder>
            {
                CategorySortOrder.ProductName,
                CategorySortOrder.ProductNameDescending,
                CategorySortOrder.ProductPriceDescending,
                CategorySortOrder.ProductPriceAscending,
                CategorySortOrder.ProductSKUAscending,
                CategorySortOrder.ProductSKUDescending
            };

            return orders;
        }


        /// <summary>
        ///     Remove the category.
        /// </summary>
        /// <param name="bvin">Category unique identifier. Usually GUID.</param>
        /// <returns>Returns true if category removed successfully, otherwise return false.</returns>
        public bool DestroyCategory(string bvin)
        {
            // CustomUrl
            var customUrl = Factory.CreateRepo<CustomUrlRepository>();
            customUrl.DeleteBySystemData(bvin);

            return Categories.Delete(bvin);
        }

        /// <summary>
        ///     Remove the category.
        /// </summary>
        /// <param name="bvin">Category unique identifier. Usually GUID.</param>
        /// <param name="storeId">Store unique identifier. </param>
        /// <returns>Returns true if category removed successfully, otherwise return false.</returns>
        public bool DestroyCategoryForStore(string bvin, long storeId)
        {
            // CustomUrl
            var customUrl = Factory.CreateRepo<CustomUrlRepository>();
            customUrl.DeleteBySystemData(bvin);

            return Categories.Delete(bvin);
        }

        #endregion

        #region Variants

        /// <summary>
        ///     Update the product inventory and remove orphan mappings for the product variants.
        /// </summary>
        /// <param name="p">Product instance for which needs to perform this operation.</param>
        /// <remarks>
        ///     This operation generally performed when do the add/update/remove choice for the product and each time when
        ///     variants list shown on administration page.
        /// </remarks>
        public void VariantsValidate(Product p)
        {
            // Check for Variants that contain non-associated options and delete
            VariantsRemoveInvalid(p);

            // Check for Variants that do not have all selections because a 
            // new variant option may have been added
            UpdateShortVariants(p);

            // Clear and Rebuild Inventory Objects
            var reloadedProduct = Products.Find(p.Bvin);
            InventoryGenerateForProduct(reloadedProduct);
            CleanUpInventory(reloadedProduct);
            UpdateProductVisibleStatusAndSave(reloadedProduct);

            p = Products.Find(p.Bvin);
        }

        /// <summary>
        ///     If the option is shared then update product options and inventory and remove orphan mapping for the products which
        ///     are mapped to the given option.
        /// </summary>
        /// <param name="o">Product option instance</param>
        /// <remarks>This operation performed when deleting or editing any shared option from administrator view.</remarks>
        public void VariantsValidateForSharedOption(Option o)
        {
            if (o.IsShared)
            {
                var productsUsing = ProductsFindByOption(o.Bvin);

                foreach (var p in productsUsing)
                {
                    ProductsReloadOptions(p);
                    VariantsReloadForProduct(p);
                    VariantsValidate(p);
                }
            }
        }

        /// <summary>
        ///     Get list of options which are variant and convert that to optionselection list.
        /// </summary>
        /// <param name="options">List of options.</param>
        /// <returns>List of the OptionSelectionList.</returns>
        /// <remarks>This is generally used on the product choices variant page to show the available options.</remarks>
        public List<OptionSelectionList> VariantsGetAllPossibleSelections(OptionList options)
        {
            var data = new List<OptionSelectionList>();

            var variantOptions = options.VariantsOnly();
            if (variantOptions == null) return data;
            if (variantOptions.Count < 1) return data;

            var selections = new OptionSelectionList();
            GenerateVariantSelections(data, variantOptions, 0, selections);

            return data;
        }

        /// <summary>
        ///     Generate possible variants for given product.
        /// </summary>
        /// <param name="p">Product instance</param>
        /// <param name="possibleVariantsCount">returns count of possible variants.</param>
        public void VariantsGenerateAllPossible(Product p, out int possibleVariantsCount)
        {
            // Make sure we clear out anything not needed
            VariantsValidate(p);

            var possibleSelections = VariantsGetAllPossibleSelections(p.Options);
            possibleVariantsCount = possibleSelections.Count;
            var variantsCount = p.Variants.Count;

            foreach (var selections in possibleSelections)
            {
                if (variantsCount < WebAppSettings.MaxVariants)
                {
                    var v = new Variant();
                    v.ProductId = p.Bvin;
                    v.Selections.AddRange(selections);

                    if (!p.Variants.ContainsKey(v.UniqueKey()))
                    {
                        if (ProductVariants.Create(v))
                        {
                            variantsCount++;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region Product Options

        /// <summary>
        ///     Add option to the product.
        /// </summary>
        /// <param name="p">Product instance for which needs to install option.</param>
        /// <param name="optionBvin">Option unique identifier. Usually be GUID.</param>
        /// <returns>Returns true if the option added successfully, otherwise returns false.</returns>
        public bool ProductsAddOption(Product p, string optionBvin)
        {
            var opt = ProductOptions.Find(optionBvin);
            if (opt == null) return false;

            var result = ProductsXOptions.AddOptionToProduct(p.Bvin, optionBvin);
            ProductsReloadOptions(p);

            if (opt.IsVariant)
            {
                VariantsValidate(p);
            }

            return result;
        }

        /// <summary>
        ///     Remove option for the product.
        /// </summary>
        /// <param name="p">Product instance for which needs to remove the option.</param>
        /// <param name="optionBvin">Unique identifier for the option.</param>
        /// <returns>Returns true if the option removed successfully, otherwise returns false.</returns>
        public bool ProductsRemoveOption(Product p, string optionBvin)
        {
            var opt = ProductOptions.Find(optionBvin);
            if (opt == null) return false;

            var result = ProductsXOptions.RemoveOptionFromProduct(p.Bvin, optionBvin);

            ProductsReloadOptions(p);

            // delete an option if it's not shared
            if (result)
            {
                if (!opt.IsShared)
                {
                    result = ProductOptions.Delete(opt.Bvin);
                }
            }

            if (opt.IsVariant)
            {
                VariantsValidate(p);
            }

            return result;
        }

        /// <summary>
        ///     Clear the options for the product and reload again from the database.
        /// </summary>
        /// <param name="p">Product instance for which needs to reload options.</param>
        public void ProductsReloadOptions(Product p)
        {
            p.Options.Clear();
            p.Options.AddRange(ProductOptions.FindByProductId(p.Bvin));
        }

        #endregion

        #region Products

        /// <summary>
        ///     Creates product, it's inventory data and search index for it
        /// </summary>
        /// <param name="item">Product to create</param>
        /// <param name="rebuildSearchIndex">If set to <c>true</c> rebuild search index</param>
        /// <returns>If operation succeeded</returns>
        public bool ProductsCreateWithInventory(Product item, bool rebuildSearchIndex)
        {
            if (item == null) return false;
            if (string.IsNullOrWhiteSpace(item.UrlSlug))
            {
                item.UrlSlug = Text.Slugify(item.ProductName, true);
            }
            if (string.IsNullOrWhiteSpace(item.UpchargeUnit))
            {
                item.UpchargeAmount = Constants.UpchargeAmount;
                item.UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
            }
            
            var result = Products.Create(item);
            if (rebuildSearchIndex)
            {
                var manager = new SearchManager(Context);
                manager.IndexSingleProduct(item);
            }
            if (result)
            {
                InventoryGenerateForProduct(item);
                UpdateProductVisibleStatusAndSave(item);
            }
            return result;
        }

        /// <summary>
        ///     Creates product and it's inventory data without creating search index for it
        /// </summary>
        /// <param name="item">Product to create.</param>
        /// <returns>If operation succeeded</returns>
        public bool ProductsCreateWithInventory(Product item)
        {
            return ProductsCreateWithInventory(item, false);
        }

        /// <summary>
        ///     Updates products and rebuilds search index for it
        /// </summary>
        /// <param name="item">Product to update</param>
        /// <returns>If operation succeeded</returns>
        public bool ProductsUpdateWithSearchRebuild(Product item)
        {
            item.IsAvailableForSale = InventoryIsProductVisible(item);
            var result = Products.UpdateAdv(item);
            if (result.Success)
            {
                var manager = new SearchManager(Context);
                manager.IndexSingleProduct(item);

                RegisterAnalyticsEvents(result.OldValue, item);
            }
            return result.Success;
        }

        /// <summary>
        ///     Find products associated to specific file.
        /// </summary>
        /// <param name="fileId">Unique file identifier. Generally be GUID.</param>
        /// <returns></returns>
        public List<Product> FindProductsForFile(string fileId)
        {
            var productBvins = ProductFiles.FindProductIdsForFile(fileId);
            return Products.FindManyWithCache(productBvins);
        }

        /// <summary>
        ///     Find list of products matching with passed comma separated keys for the product property values.
        /// </summary>
        /// <param name="key">Command separated unique product property values of the product.</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count returned back as reference</param>
        /// <returns>List of products matching given criteria</returns>
        public List<Product> FindProductsMatchingKey(string key, int pageNumber, int pageSize, ref int totalCount)
        {
            totalCount = ProductPropertyValues.FindCountProductIdsMatchingKey(key);
            var matches = ProductPropertyValues.FindProductIdsMatchingKey(key, pageNumber, pageSize);
            return Products.FindManyWithCache(matches);
        }

        /// <summary>
        ///     Add specific product to category and add the analytic data for category and product mapping.
        /// </summary>
        /// <param name="productId">Product unique identifier. Usually be GUID.</param>
        /// <param name="categoryId">Category unique identifier. Usually be GUID.</param>
        /// <returns>returns CategoryProductAssociation association instance.</returns>
        public CategoryProductAssociation AddProductToCategory(string productId, string categoryId)
        {
            var result = CategoriesXProducts.AddProductToCategory(productId, categoryId);
            if (result != null)
            {
                RegisterAnalyticsEvents(categoryId);
            }
            return result;
        }

        /// <summary>
        ///     Remove specific product from the category.
        /// </summary>
        /// <param name="productId">Product unique identifier.</param>
        /// <param name="categoryId">Category unique identifier</param>
        /// <returns>returns true if product removed from category successfully otherwise returns false.</returns>
        public bool RemoveProductFromCategory(string productId, string categoryId)
        {
            var result = CategoriesXProducts.RemoveProductFromCategory(productId, categoryId);
            if (result)
            {
                RegisterAnalyticsEvents(categoryId);
            }
            return result;
        }

        /// <summary>
        ///     Clone product with given information.
        /// </summary>
        /// <param name="productId">Source product unique identifier for cloning</param>
        /// <param name="newSku">New SKU for the target product.</param>
        /// <param name="newSlug">New Slug for the target product.</param>
        /// <param name="newStatus">New Status for the target product.</param>
        /// <param name="cloneProductRoles">Flag to determine whether needs to clone roles or not.</param>
        /// <param name="cloneProductChoices">Flag to determine whether needs to clone product choices or not.</param>
        /// <param name="cloneCategoryPlacement">Flag to determine whether needs to add product category mapping or not.</param>
        /// <param name="cloneImages">Flag to determine whether needs to clone images or not.</param>
        /// <param name="cloneReviews">Flag to determine whether needs to clone reviews or not.</param>
        public void CloneProduct(string productId, string newSku, string newSlug, ProductStatus newStatus,
            bool cloneProductRoles, bool cloneProductChoices, bool cloneCategoryPlacement, bool cloneImages,
            bool cloneReviews)
        {
            string newProductId = null;
            var productCreated = Products.Clone(productId, newSku, newSlug, newStatus, cloneImages, out newProductId);

            if (productCreated)
            {
                BundledProducts.CloneForProduct(productId, newProductId);

                ProductPropertyValues.CloneForProduct(productId, newProductId);

                if (cloneProductChoices)
                {
                    var productXOptions = ProductsXOptions.FindForProduct(productId, 1, int.MaxValue);
                    foreach (var productXOption in productXOptions)
                    {
                        var productOption = ProductOptions.Find(productXOption.OptionBvin);

                        if (!productOption.IsShared)
                        {
                            string newProductOptionId;
                            ProductOptions.Clone(productOption.Bvin, out newProductOptionId);
                            productXOption.OptionBvin = newProductOptionId;
                        }
                        productXOption.ProductBvin = newProductId;
                        ProductsXOptions.Create(productXOption);
                    }
                }

                if (cloneProductRoles)
                {
                    CatalogRoles.CloneForProduct(productId, newProductId);
                }

                if (cloneCategoryPlacement)
                {
                    var cats = CategoriesXProducts.FindForProduct(productId, 1, int.MaxValue);
                    foreach (var a in cats)
                    {
                        CategoriesXProducts.AddProductToCategory(newProductId, a.CategoryId);
                    }
                }

                if (cloneImages)
                {
                    DiskStorage.CloneAllProductFiles(Context.CurrentStore.Id, productId, newProductId);
                    DiskStorage.DeleteAdditionalProductImage(Context.CurrentStore.Id, newProductId);

                    var productImages = ProductImages.FindByProductId(productId);
                    foreach (var productImage in productImages)
                    {
                        var imageId = productImage.Bvin;
                        productImage.Bvin = Guid.NewGuid().ToString();
                        productImage.ProductId = newProductId;
                        var imageCreated = ProductImages.Create(productImage);
                        if (imageCreated)
                        {
                            DiskStorage.CloneAdditionalImage(Context.CurrentStore.Id, productId, imageId, newProductId,
                                productImage.Bvin);
                        }
                    }
                }

                if (cloneReviews)
                {
                    var productReviews = ProductReviews.FindByProductId(productId);
                    foreach (var productReview in productReviews)
                    {
                        productReview.Bvin = Guid.NewGuid().ToString();
                        productReview.ProductBvin = newProductId;
                        ProductReviews.Create(productReview);
                    }
                }

                var locales = Factory.Instance.CreateStoreSettingsProvider().GetLocales();
                foreach (var locale in locales)
                {
                    var context = new HccRequestContext();
                    context.CurrentStore = Context.CurrentStore;
                    context.CurrentAccount = Context.CurrentAccount;
                    context.MainContentCulture = locale.Code;
                    context.FallbackContentCulture = locale.Fallback;

                    var productRepository = Factory.CreateRepo<ProductRepository>(context);
                    var product = productRepository.Find(newProductId);

                    var manager = new SearchManager(context);
                    manager.IndexSingleProduct(product);
                }
            }
        }

        /// <summary>
        ///     Remove specific product from the given store.
        /// </summary>
        /// <param name="bvin">Product unique identifier.</param>
        /// <param name="storeId">Store unique identifier.</param>
        /// <returns>returns true if product removed from store otherwise returns false.</returns>
        public bool DestroyProduct(string bvin, long storeId)
        {
            // Custom Url
            var customUrl = Factory.CreateRepo<CustomUrlRepository>();
            customUrl.DeleteBySystemData(bvin);

            var searchManager = new SearchManager(Context);
            searchManager.RemoveSingleProduct(storeId, bvin);

            return Products.DeleteForStore(bvin, storeId);
        }

        #endregion

        #region Product Inventory

        /// <summary>
        ///     Update product's visibility based on the inventory and unavailability rules.
        /// </summary>
        /// <param name="productBvin">Product unique identifier.</param>
        public void UpdateProductVisibleStatusAndSave(string productBvin)
        {
            var product = Products.Find(productBvin);
            UpdateProductVisibleStatusAndSave(product);
        }

        /// <summary>
        ///     Update products visibility based on the inventory and unavailability rules.
        /// </summary>
        /// <param name="product">
        ///     Product instance. More information for the instance can be found at <see cref="Product" />
        /// </param>
        public void UpdateProductVisibleStatusAndSave(Product product)
        {
            if (product == null || product.IsBundle)
                return;
            product.IsAvailableForSale = InventoryIsProductVisible(product);
            Products.Update(product, false);
        }

        /// <summary>
        ///     Update product inventories and based on inventory set the visibility of the product on store.
        /// </summary>
        /// <param name="inv">
        ///     ProductInventory instance. More information for the instance can be found at
        ///     <see cref="ProductInventory" />
        /// </param>
        /// <returns>Returns true if the produce inventory updated successfully otherwise returns false.</returns>
        public bool InventoryUpdateWithStatusSave(ProductInventory inv)
        {
            var result = ProductInventories.Update(inv);
            if (result) UpdateProductVisibleStatusAndSave(inv.ProductBvin);
            return result;
        }

        /// <summary>
        ///     Create inventory for the product based on given inventory information.
        /// </summary>
        /// <param name="inv">
        ///     ProductInventory instance. More information for the instance can be found at
        ///     <see cref="ProductInventory" />
        /// </param>
        /// <returns>Returns true if the produce inventory updated successfully otherwise returns false.</returns>
        public bool InventoryCreateWithStatusSave(ProductInventory inv)
        {
            var result = ProductInventories.Create(inv);
            if (result)
                UpdateProductVisibleStatusAndSave(inv.ProductBvin);
            return result;
        }

        /// <summary>
        ///     Generate product inventory based on the variants for the given product.
        /// </summary>
        /// <param name="localProduct">
        ///     Product instance. More information for the given instance can be found at
        ///     <see cref="Product" />
        /// </param>
        public void InventoryGenerateForProduct(Product localProduct)
        {
            if (localProduct == null) return;

            if (localProduct.HasVariants())
            {
                foreach (var v in localProduct.Variants)
                {
                    InventoryGenerateSingleInventory(localProduct.Bvin, v.Bvin);
                }
            }
            else
            {
                InventoryGenerateSingleInventory(localProduct.Bvin, string.Empty);
            }
        }

        /// <summary>
        ///     Delete all inventory information for the given product.
        /// </summary>
        /// <param name="product">
        ///     Product instance. More information for the given instance can be found at <see cref="Product" />
        /// </param>
        public void CleanUpInventory(Product product)
        {
            if (product == null) return;
            var allInventory = ProductInventories.FindByProductId(product.Bvin);
            if (allInventory == null || allInventory.Count == 0)
                return;

            if (product.HasVariants())
            {
                foreach (var inv in allInventory)
                {
                    if (string.IsNullOrWhiteSpace(inv.VariantId))
                    {
                        // Remove non-variant inventory levels
                        ProductInventories.Delete(inv.Bvin);
                    }

                    if (product.Variants.Where(y => y.Bvin == inv.VariantId).Count() <= 0)
                    {
                        // Remove variant inventory levels that don't apply anymore
                        ProductInventories.Delete(inv.Bvin);
                    }
                }
            }
            else
            {
                // Remove all variant inventory levels
                foreach (var inv in allInventory)
                {
                    if (!string.IsNullOrWhiteSpace(inv.VariantId))
                    {
                        ProductInventories.Delete(inv.Bvin);
                    }
                }
            }
        }

        /// <summary>
        ///     Check whether given product is available with options selected by user. This can be bundled product or simple
        ///     product.
        /// </summary>
        /// <param name="product">
        ///     Product instance. More information for the given instance can be found at <see cref="Product" />
        /// </param>
        /// <param name="selections">Product option instance. This holds the information for the customization of the product.</param>
        /// <returns>
        ///     Returns instance of the <see cref="InventoryCheckData" /> with message as html string. This instance indicates
        ///     whether product is in stock or out of the stock for given criteria.
        /// </returns>
        public InventoryCheckData InventoryCheck(Product product, OptionSelections selections)
        {
            if (!product.IsBundle)
            {
                return SimpleProductInventoryCheck(product, selections.OptionSelectionList);
            }
            var results = new List<InventoryCheckData>();
            foreach (var bundledProductAdv in product.BundledProducts)
            {
                var bundledProduct = bundledProductAdv.BundledProduct;
                if (bundledProduct == null)
                    continue;

                var optionSelection = selections.GetSelections(bundledProductAdv.Id);
                var quantity = bundledProductAdv.Quantity;
                var singleResult = SimpleProductInventoryCheck(bundledProduct, optionSelection, quantity);
                singleResult.Qty /= quantity;
                results.Add(singleResult);
            }

            var result = new InventoryCheckData();
            result.IsInStock = results.All(r => r.IsInStock);
            result.IsAvailableForSale = results.All(r => r.IsAvailableForSale);
            result.Qty = results.Min(r => (int?) r.Qty) ?? 0;
            if (!result.IsAvailableForSale)
                result.InventoryMessage = "<span class=\"inventoryoutofstock\">" +
                                          GlobalLocalization.GetString("OutOfStockLabel",
                                              HccRequestContext.Current.MainContentCulture) + "</span>";
            else if (!result.IsInStock)
                result.InventoryMessage = "<span class=\"inventorybackordered\">" +
                                          GlobalLocalization.GetString("Backordered",
                                              HccRequestContext.Current.MainContentCulture) + "</span>";
            else
                GlobalLocalization.GetString("InStockLabel", HccRequestContext.Current.MainContentCulture);


            return result;
        }

        /// <summary>
        ///     Check inventory for the non-bundled product.
        /// </summary>
        /// <param name="product">
        ///     Product instance. More information for the given instance can be found at <see cref="Product" />
        /// </param>
        /// <param name="optionSelection">
        ///     <see cref="OptionSelectionList" /> instance. This holds the information for the
        ///     customization of the product.
        /// </param>
        /// <param name="quantity"></param>
        /// <returns>
        ///     Returns instance of the <see cref="InventoryCheckData" /> with message as html string. This instance indicates
        ///     whether product is in stock or out of the stock for given criteria.
        /// </returns>
        public InventoryCheckData SimpleProductInventoryCheck(Product product, OptionSelectionList optionSelection,
            int quantity = 1)
        {
            Variant variant = null;
            if (optionSelection != null)
                variant = product.Variants.FindBySelectionData(optionSelection, product.Options);
            var variantId = variant != null ? variant.Bvin : string.Empty;

            return SimpleProductInventoryCheck(product, variantId, quantity);
        }

        /// <summary>
        ///     Check inventory for the product with variant.
        /// </summary>
        /// <param name="product">
        ///     Product instance. More information for the given instance can be found at <see cref="Product" />
        /// </param>
        /// <param name="variantId">Variant unique identifier.</param>
        /// <param name="quantity">Required quantity.</param>
        /// <returns>
        ///     Returns instance of the <see cref="InventoryCheckData" /> with message as html string. This instance indicates
        ///     whether product is in stock or out of the stock for given criteria.
        /// </returns>
        public InventoryCheckData SimpleProductInventoryCheck(Product product, string variantId, int quantity = 1)
        {
            var result = new InventoryCheckData();
            result.Qty = InventoryQuantityAvailableForPurchase(product, variantId);

            switch (product.InventoryMode)
            {
                case ProductInventoryMode.AlwayInStock:
                    result.IsInStock = true;
                    // TODO: Localize
                    result.InventoryMessage = GlobalLocalization.GetString("InStockLabel",
                        HccRequestContext.Current.MainContentCulture);
                    result.IsAvailableForSale = true;
                    break;
                case ProductInventoryMode.WhenOutOfStockAllowBackorders:
                    if (result.Qty < quantity)
                    {
                        result.IsInStock = false;
                        // TODO: Localize
                        result.InventoryMessage = "<span class=\"inventorybackordered\">" +
                                                  GlobalLocalization.GetString("Backordered",
                                                      HccRequestContext.Current.MainContentCulture) + "</span>";
                        result.IsAvailableForSale = true;
                    }
                    else
                    {
                        result.IsInStock = true;

                        var localizeString = GlobalLocalization.GetString("InStockWithInventoryLevel",
                            HccRequestContext.Current.MainContentCulture);
                        if (!string.IsNullOrEmpty(localizeString))
                        {
                            if (localizeString.IndexOf("{0}") >= 0)
                            {
                                result.InventoryMessage = string.Format(localizeString, result.Qty);
                            }
                            else
                            {
                                result.InventoryMessage = localizeString;
                            }
                        }


                        result.IsAvailableForSale = true;
                    }
                    break;
                case ProductInventoryMode.WhenOutOfStockHide:
                case ProductInventoryMode.WhenOutOfStockShow:
                    if (result.Qty < quantity)
                    {
                        result.IsInStock = false;
                        // TODO: Localize
                        result.InventoryMessage = "<span class=\"inventoryoutofstock\">" +
                                                  GlobalLocalization.GetString("OutOfStockLabel",
                                                      HccRequestContext.Current.MainContentCulture) + "</span>";
                        result.IsAvailableForSale = false;
                    }
                    else
                    {
                        result.IsInStock = true;
                        var localizeString = GlobalLocalization.GetString("InStockWithInventoryLevel",
                            HccRequestContext.Current.MainContentCulture);
                        if (!string.IsNullOrEmpty(localizeString))
                        {
                            if (localizeString.IndexOf("{0}") >= 0)
                            {
                                result.InventoryMessage = string.Format(localizeString, result.Qty);
                            }
                            else
                            {
                                result.InventoryMessage = localizeString;
                            }
                        }

                        result.IsAvailableForSale = true;
                    }
                    break;
                default:
                    throw new NotImplementedException("InventoryMode is not implemented");
            }

            return result;
        }

        #endregion

        #region Product Images

        /// <summary>
        ///     Create product image and add analytics information for the new image.
        /// </summary>
        /// <param name="image"><see cref="ProductImage" /> instance</param>
        /// <returns>Returns true if the product image has been created successfully otherwise return false.</returns>
        public bool ProductImageCreate(ProductImage image)
        {
            var result = ProductImages.Create(image);
            if (result)
            {
                RegisterAnalyticsEvents(image);
            }
            return result;
        }

        /// <summary>
        ///     Update product image and add analytics information for the updated image.
        /// </summary>
        /// <param name="image"><see cref="ProductImage" /> instance</param>
        /// <returns>Returns true if the product image has been updated successfully otherwise return false</returns>
        public bool ProductImageUpdate(ProductImage image)
        {
            var result = ProductImages.Update(image);
            if (result)
            {
                RegisterAnalyticsEvents(image);
            }
            return result;
        }

        /// <summary>
        ///     Delete product image and add analytics information for the deleted image.
        /// </summary>
        /// <param name="productImageId"><see cref="ProductImage" /> instance</param>
        /// <returns>Returns true if the product image has been deleted successfully otherwise return false</returns>
        public bool ProductImageDelete(string productImageId)
        {
            var result = ProductImages.DeleteAdv(productImageId);
            if (result.Success)
            {
                var image = result.OldValues.First();
                RegisterAnalyticsEvents(image);
            }
            return result.Success;
        }

        #endregion

        #region Line Item Inventory

        /// <summary>
        ///     Used to update the inventory whenever any order items got shipped.
        /// </summary>
        /// <param name="li"> <see cref="Orders.LineItem" /> instance with order item information.</param>
        /// <param name="shippedQuantity">Quantity which going to be shipped for the order.</param>
        /// <returns>Returns true if inventory properly updated for the shipped item otherwise returns false.</returns>
        public bool InventoryLineItemShipQuantity(LineItem li, int shippedQuantity)
        {
            li.QuantityShipped += shippedQuantity;
            if (!li.IsBundle)
            {
                var reservedQuantity = Math.Min(shippedQuantity, li.QuantityReserved);
                InventoryShipQuantity(li.ProductId, li.VariantId, shippedQuantity, reservedQuantity);
            }
            else
            {
                var product = Products.FindWithCache(li.ProductId);
                if (product == null)
                    return false;
                foreach (var bundledProductAdv in product.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;

                    var optionSelection = li.SelectionData.GetSelections(bundledProductAdv.Id);
                    var variant = bundledProduct.Variants.FindBySelectionData(optionSelection, bundledProduct.Options);
                    var variantId = variant != null ? variant.Bvin : string.Empty;

                    InventoryShipQuantity(bundledProduct.Bvin, variantId, shippedQuantity*bundledProductAdv.Quantity,
                        li.QuantityReserved*bundledProductAdv.Quantity);
                }
            }
            return true;
        }

        /// <summary>
        ///     Used to update inventory whenever any order item needs to be marked as unshipped.
        /// </summary>
        /// <param name="li"><see cref="Orders.LineItem" /> instance with order item information.</param>
        /// <param name="quantity">Quantity which going to be unshipped for the order.</param>
        /// <returns>Returns true if inventory properly updated for the unshipped item otherwise returns false.</returns>
        public bool InventoryLineItemUnShipQuantity(LineItem li, int quantity)
        {
            li.QuantityShipped -= quantity;

            if (!li.IsBundle)
            {
                InventoryUnshipQuantity(li.ProductId, li.VariantId, quantity);
            }
            else
            {
                var product = Products.FindWithCache(li.ProductId);
                if (product == null)
                    return false;
                foreach (var bundledProductAdv in product.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;

                    var optionSelection = li.SelectionData.GetSelections(bundledProductAdv.Id);
                    var variant = bundledProduct.Variants.FindBySelectionData(optionSelection, bundledProduct.Options);
                    var variantId = variant != null ? variant.Bvin : string.Empty;

                    InventoryUnshipQuantity(bundledProduct.Bvin, variantId, quantity*bundledProductAdv.Quantity);
                }
            }
            return true;
        }

        /// <summary>
        ///     Used to update the inventory whenever its required to reserve some product quantity based on purchase.
        /// </summary>
        /// <param name="li"> <see cref="Orders.LineItem" /> instance with order item information</param>
        /// <returns>Returns the quantity reserved for the given order item</returns>
        public int InventoryLineItemReserveInventory(LineItem li)
        {
            if (!li.IsBundle)
            {
                return InventoryReserveQuantity(li.ProductId, li.VariantId, li.Quantity);
            }
            var product = Products.FindWithCache(li.ProductId);
            if (product == null)
                return 0;

            var rollbackNeeded = false;
            var reservationList = new List<ResevationItem>();
            foreach (var bundledProductAdv in product.BundledProducts)
            {
                var bundledProduct = bundledProductAdv.BundledProduct;
                if (bundledProduct == null)
                    continue;

                var optionSelection = li.SelectionData.GetSelections(bundledProductAdv.Id);
                var variant = bundledProduct.Variants.FindBySelectionData(optionSelection, bundledProduct.Options);
                var variantId = variant != null ? variant.Bvin : string.Empty;

                var orderedQuantity = li.Quantity*bundledProductAdv.Quantity;
                var reservedQuantity = InventoryReserveQuantity(bundledProduct.Bvin, variantId, orderedQuantity);
                reservationList.Add(new ResevationItem
                {
                    ProductId = bundledProduct.Bvin,
                    VariantId = variantId,
                    Quantity = reservedQuantity
                });
                if (orderedQuantity != reservedQuantity)
                {
                    rollbackNeeded = true;
                    break;
                }
            }
            if (rollbackNeeded)
            {
                foreach (var reservation in reservationList)
                {
                    InventoryUnreserveQuantity(reservation.ProductId, reservation.VariantId, reservation.Quantity);
                }
                return 0;
            }
            return li.Quantity;
        }

        /// <summary>
        ///     Used to update the inventory whenever needs unreserved the quantity in case of order is canceled or in other
        ///     scenarios.
        /// </summary>
        /// <param name="li"><see cref="Orders.LineItem" /> instance with order item information</param>
        /// <returns>Returns true if the inventory has been unreserved successfully otherwise returns false.</returns>
        public bool InventoryLineItemUnreserveInventory(LineItem li)
        {
            if (!li.IsBundle)
            {
                return InventoryUnreserveQuantity(li.ProductId, li.VariantId, li.QuantityReserved);
            }
            var product = Products.FindWithCache(li.ProductId);
            if (product == null)
                return false;

            var result = true;
            foreach (var bundledProductAdv in product.BundledProducts)
            {
                var bundledProduct = bundledProductAdv.BundledProduct;
                if (bundledProduct == null)
                    continue;

                var optionSelection = li.SelectionData.GetSelections(bundledProductAdv.Id);
                var variant = bundledProduct.Variants.FindBySelectionData(optionSelection, bundledProduct.Options);
                var variantId = variant != null ? variant.Bvin : string.Empty;

                result &= InventoryUnreserveQuantity(bundledProduct.Bvin, variantId,
                    li.QuantityReserved*bundledProductAdv.Quantity);
            }
            return result;
        }

        #endregion

        #region ProductTypes

        /// <summary>
        ///     Delete specific product type.
        /// </summary>
        /// <param name="bvin">Product type unique identifier.</param>
        /// <returns>Returns true if product type removed successfully otherwise false.</returns>
        public bool ProductTypeDestroy(string bvin)
        {
            // Then remove type
            return ProductTypes.Delete(bvin);
        }

        /// <summary>
        ///     Remove all product types for the given store.
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        public void ProductTypeDestoryAllForStore(long storeId)
        {
            ProductTypes.DestroyAllForStore(storeId);
        }

        /// <summary>
        ///     Add custom property to product type.
        /// </summary>
        /// <param name="typeBvin">Product type unique identifier</param>
        /// <param name="propertyId">Product property unique identifer</param>
        /// <returns>Returns true if the property has been added to product type otherwise returns false.</returns>
        public bool ProductTypeAddProperty(string typeBvin, long propertyId)
        {
            var item = ProductTypesXProperties.FindForTypeAndProperty(typeBvin, propertyId);
            if (item == null)
            {
                item = new ProductTypePropertyAssociation();
                item.ProductTypeBvin = typeBvin;
                item.PropertyId = propertyId;
                return ProductTypesXProperties.Create(item);
            }
            return true;
        }

        /// <summary>
        ///     Remove product type property.
        /// </summary>
        /// <param name="typeBvin">Product type unique identifier</param>
        /// <param name="propertyId">Product property unique identifier</param>
        /// <returns>Returns true if the property has been removed successfully otherwise returns false.</returns>
        public bool ProductTypeRemoveProperty(string typeBvin, long propertyId)
        {
            return ProductTypesXProperties.DeleteForTypeAndProperty(typeBvin, propertyId);
        }

        /// <summary>
        ///     Change display order of the property its used  to show on product detail page.
        /// </summary>
        /// <param name="productTypeBvin">Product type uniuqe identifier</param>
        /// <param name="propertyId">Property unique identifier</param>
        /// <returns>Returns true if the order has been updated successfully otherwise returns false.</returns>
        public bool ProductTypeMovePropertyUp(string productTypeBvin, long propertyId)
        {
            var ret = false;

            var peers = ProductTypesXProperties.FindByProductType(productTypeBvin);

            if (peers != null)
            {
                var currentSort = 0;
                var targetSort = 0;
                long targetId = -1;
                var foundTarget = false;

                for (var i = 0; i <= peers.Count - 1; i++)
                {
                    if (peers[i].Id == propertyId)
                    {
                        currentSort = peers[i].SortOrder;
                        // process last request
                        foundTarget = true;
                    }
                    else
                    {
                        if (foundTarget == false)
                        {
                            targetSort = peers[i].SortOrder;
                            targetId = peers[i].Id;
                        }
                    }
                }

                if (foundTarget & currentSort >= 1)
                {
                    if (peers.Count > 1)
                    {
                        ProductTypesXProperties.UpdateSortOrder(productTypeBvin, targetId, currentSort);
                        ProductTypesXProperties.UpdateSortOrder(productTypeBvin, propertyId, targetSort);
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        ///     Change display order of the property its used  to show on product detail page.
        /// </summary>
        /// <param name="productTypeBvin">Product type uniuqe identifier</param>
        /// <param name="propertyId">Property unique identifier</param>
        /// <returns>Returns true if the order has been updated successfully otherwise returns false.</returns>
        public bool ProductTypeMovePropertyDown(string productTypeBvin, long propertyId)
        {
            var ret = false;

            var peers = ProductTypesXProperties.FindByProductType(productTypeBvin);

            if (peers != null)
            {
                var currentSort = 0;
                var targetSort = 0;
                long targetId = -1;
                var foundCurrent = false;
                var foundTarget = false;

                for (var i = 0; i <= peers.Count - 1; i++)
                {
                    if (foundCurrent)
                    {
                        targetId = peers[i].Id;
                        targetSort = peers[i].SortOrder;
                        foundCurrent = false;
                        foundTarget = true;
                    }
                    if (peers[i].Id == propertyId)
                    {
                        currentSort = peers[i].SortOrder;
                        foundCurrent = true;
                    }
                }

                if (foundTarget)
                {
                    if (peers.Count > 1)
                    {
                        ProductTypesXProperties.UpdateSortOrder(productTypeBvin, propertyId, targetSort);
                        ProductTypesXProperties.UpdateSortOrder(productTypeBvin, targetId, currentSort);
                        ret = true;
                    }
                }
            }

            return ret;
        }

        #endregion

        #region ProductProperties

        /// <summary>
        ///     Delete specific product property
        /// </summary>
        /// <param name="id">Product property uniuqe identifier</param>
        /// <returns>Returns true if the product property has been removed successfully otherwise returns false.</returns>
        public bool ProductPropertiesDestroy(long id)
        {
            return ProductProperties.Delete(id);
        }

        /// <summary>
        ///     Get all product properties for a given product type.
        /// </summary>
        /// <param name="productTypeId">Product type unique identifier.</param>
        /// <returns>Returns list of <see cref="ProductProperty" /> for given product type</returns>
        public List<ProductProperty> ProductPropertiesFindForType(string productTypeId)
        {
            var crosses = ProductTypesXProperties.FindByProductType(productTypeId);
            var ids = new List<long>();
            foreach (var x in crosses)
            {
                ids.Add(x.PropertyId);
            }

            // FindMany sorts by PropertyName so we
            // need to resort based on option order
            // in ProductXOption table
            var unsorted = ProductProperties.FindMany(ids).OrderBy(p => p.DisplayName);
            var result = new List<ProductProperty>();
            foreach (var cross in crosses)
            {
                var sorted = unsorted.Where(y => y.Id == cross.PropertyId).FirstOrDefault();
                
                if (sorted != null)
                {
                    sorted.Choices = sorted.Choices.OrderBy(c => c.SortOrder).ToList();
                    result.Add(sorted);
                }
            }

            return result;
        }

        /// <summary>
        ///     Get list of product properties not assigned to given product type
        /// </summary>
        /// <param name="productTypeId">Product type unique identifier</param>
        /// <returns>Returns list of <see cref="ProductProperty" /> for given product type.</returns>
        public List<ProductProperty> ProductPropertiesFindNotAssignedToType(string productTypeId)
        {
            var assigned = ProductPropertiesFindForType(productTypeId);
            var all = ProductProperties.FindAll();
            return all.Where(y => assigned.Contains(y) == false).ToList();
        }

        #endregion

        #region Roles

        /// <summary>
        ///     Test method for the unit testing.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        protected virtual bool TestRolesAccess(List<CatalogRole> roles)
        {
            return false;
        }

        /// <summary>
        ///     Test method for unit testing.
        /// </summary>
        /// <param name="p">Product instance</param>
        /// <returns></returns>
        public bool TestProductAccess(Product p)
        {
            var roles = FindActualProductRoles(p);
            return TestRolesAccess(roles);
        }

        /// <summary>
        ///     Test method for unit testing.
        /// </summary>
        /// <param name="c">Category Instance</param>
        /// <returns></returns>
        public bool TestCategoryAccess(Category c)
        {
            var roles = CatalogRoles.FindByCategoryId(DataTypeHelper.BvinToGuid(c.Bvin));
            return TestRolesAccess(roles);
        }

        /// <summary>
        ///     Get the product roles assigned to specific product.
        /// </summary>
        /// <param name="p"><see cref="Product" /> instance</param>
        /// <returns>Returns list of product roles</returns>
        public List<CatalogRole> FindActualProductRoles(Product p)
        {
            var roles = CatalogRoles.FindByProductId(DataTypeHelper.BvinToGuid(p.Bvin));

            if (roles.Count == 0)
            {
                var crosses = CategoriesXProducts.FindForProduct(p.Bvin, 1, int.MaxValue);

                if (crosses != null && crosses.Count > 0)
                {
                    var catIds = crosses.Select(c => DataTypeHelper.BvinToGuid(c.CategoryId)).ToList();
                    roles = CatalogRoles.FindByCategoryIds(catIds);
                }

                if (roles.Count == 0 && !string.IsNullOrEmpty(p.ProductTypeId))
                {
                    roles = CatalogRoles.FindByProductTypeId(new Guid(p.ProductTypeId));
                }
            }

            return roles;
        }

        #endregion

        #region Implementation

        /// <summary>
        ///     Remove all invalid variations for the given product.
        /// </summary>
        /// <param name="p"><see cref="Product" /> instance</param>
        private void VariantsRemoveInvalid(Product p)
        {
            foreach (var v in p.Variants)
            {
                if (OptionSelection.ContainsInvalidSelectionForOptions(p.Options, v.Selections))
                {
                    ProductVariants.Delete(v.Bvin);
                }
            }
            VariantsReloadForProduct(p);
        }

        /// <summary>
        ///     Update soring order for the variants to make sure its proper after any variants removed or updated.
        /// </summary>
        /// <param name="p"><see cref="Product" /> instance</param>
        private void UpdateShortVariants(Product p)
        {
            // Find out how many variants we have.
            var variantOptions = p.Options.VariantsOnly();
            var variantCount = variantOptions.Count();
            if (variantCount < 1) return;

            foreach (var v in p.Variants)
            {
                if (v.Selections.Count < variantCount)
                {
                    AddMissingOptions(v, variantOptions);
                }
            }
        }

        /// <summary>
        ///     Clear variants for the given product and reload from database again.
        /// </summary>
        /// <param name="p"><see cref="Product" /> instance</param>
        private void VariantsReloadForProduct(Product p)
        {
            p.Variants.Clear();
            p.Variants.AddRange(ProductVariants.FindByProductId(p.Bvin));
        }

        /// <summary>
        ///     Get list of products having the specific option available.
        /// </summary>
        /// <param name="optionBvin">Product option unique identifier</param>
        /// <returns>Returns list of the products</returns>
        private List<Product> ProductsFindByOption(string optionBvin)
        {
            var crosses = ProductsXOptions.FindForOption(optionBvin, 1, int.MaxValue);
            var productIds = crosses.Select(c => c.ProductBvin).ToList();
            return Products.FindManyWithCache(productIds);
        }

        /// <summary>
        ///     Add options to product variant.
        /// </summary>
        /// <param name="v"><see cref="Variant" /> instance</param>
        /// <param name="options">List of <see cref="Option" /> to check for the given variant</param>
        private void AddMissingOptions(Variant v, List<Option> options)
        {
            foreach (var opt in options)
            {
                if (!v.Selections.ContainsSelectionForOption(opt.Bvin))
                {
                    if (opt.Items.Count > 0)
                    {
                        v.Selections.Add(new OptionSelection(opt.Bvin, opt.Items[0].Bvin));
                    }
                }
            }
            ProductVariants.Update(v);
        }

        /// <summary>
        ///     Check if given product is visible on store or not.
        /// </summary>
        /// <param name="product"><see cref="Product" /> instance</param>
        /// <returns>Returns true if the product is visible based on inventory criteria otherwise returns false.</returns>
        private bool InventoryIsProductVisible(Product product)
        {
            if (product.Status == ProductStatus.Disabled)
                return false;
            switch (product.InventoryMode)
            {
                case ProductInventoryMode.AlwayInStock:
                case ProductInventoryMode.WhenOutOfStockAllowBackorders:
                    return true;
                case ProductInventoryMode.WhenOutOfStockShow:
                case ProductInventoryMode.WhenOutOfStockHide:
                    var inv = ProductInventories.FindByProductId(product.Bvin);

                    // no inventory info so assume it's available
                    if (inv == null || inv.Count == 0)
                        return true;

                    foreach (var piv in inv)
                    {
                        if (piv.QuantityAvailableForSale > 0)
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    throw new NotImplementedException("InventoryMode is not implemented");
            }
        }

        // Loop through all possible variants and generate selection data
        private void GenerateVariantSelections(List<OptionSelectionList> data, List<Option> options, int optionIndex,
            OptionSelectionList tempSelections)
        {
            if (optionIndex > options.Count - 1)
            {
                // we've hit all options so add the selections to the data
                var temp = new OptionSelectionList();
                temp.AddRange(tempSelections);
                //tempSelections.RemoveAt(tempSelections.Count() - 1);
                //tempSelections.Clear();
                data.Add(temp);
            }
            else
            {
                var opt = options[optionIndex];
                foreach (var oi in opt.Items)
                {
                    if (!oi.IsLabel)
                    {
                        var localList = new OptionSelectionList();
                        localList.AddRange(tempSelections);
                        localList.Add(new OptionSelection(opt.Bvin, oi.Bvin));
                        GenerateVariantSelections(data, options, optionIndex + 1, localList);
                    }
                }
            }
        }

        /// <summary>
        ///     Update product inventory for the shipped product.
        /// </summary>
        /// <param name="productBvin">Product unique identifier</param>
        /// <param name="variantId">Variant unique identifier</param>
        /// <param name="shipQuantity">Shipped quantity for given product</param>
        /// <param name="reservedQunatity">Reserved quantity for given product</param>
        /// <returns>Returns true if the inventory updated properly for the shipped product.</returns>
        private bool InventoryShipQuantity(string productBvin, string variantId, int shipQuantity, int reservedQunatity)
        {
            var result = false;
            var inv = ProductInventories.FindByProductIdAndVariantId(productBvin, variantId);
            if (inv != null)
            {
                inv.QuantityReserved -= reservedQunatity;
                inv.QuantityOnHand -= shipQuantity;
                ProductInventories.Update(inv);
                UpdateProductVisibleStatusAndSave(productBvin);
            }
            return result;
        }

        /// <summary>
        ///     Update product inventory for the unshipped product quantity.
        /// </summary>
        /// <param name="productBvin">Product unique identifier</param>
        /// <param name="variantId">Variant unique identifier</param>
        /// <param name="quantity">Quantity needs to be unshipped</param>
        /// <returns>Returns true if the inventory updated propery for the unshipped product quantity.</returns>
        private bool InventoryUnshipQuantity(string productBvin, string variantId, int quantity)
        {
            var result = false;
            var inv = ProductInventories.FindByProductIdAndVariantId(productBvin, variantId);
            if (inv != null)
            {
                inv.QuantityReserved += quantity;
                inv.QuantityOnHand += quantity;
                ProductInventories.Update(inv);
                UpdateProductVisibleStatusAndSave(productBvin);
            }
            return result;
        }

        /// <summary>
        ///     Generate inventory for the given product based on the variant.
        /// </summary>
        /// <param name="bvin">Product unique identifier</param>
        /// <param name="variantId">Product variant unique identifier</param>
        private void InventoryGenerateSingleInventory(string bvin, string variantId)
        {
            if (ProductInventories.FindByProductIdAndVariantId(bvin, variantId) == null)
            {
                var i = new ProductInventory();
                i.ProductBvin = bvin;
                i.VariantId = variantId;

                ProductInventories.Create(i);
            }
        }


        /// <summary>
        ///     Update inventory of the given product and variant id
        ///     to reserve the quantity for order
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="variantId">Variant unique identifier</param>
        /// <param name="quantity">Quantity needs to be reserved</param>
        /// <returns>Returns reserved quantity</returns>
        private int InventoryReserveQuantity(string productId, string variantId, int quantity)
        {
            var result = InventoryReserveQuantity(productId, variantId, quantity, true);
            UpdateProductVisibleStatusAndSave(productId);
            return result;
        }

        /// <summary>
        ///     Reserve product quantity for given product and variant.
        /// </summary>
        /// <param name="productBvin">Product unique identifier</param>
        /// <param name="variantId">Variant unique identifier</param>
        /// <param name="quantity">Quantity needs to be reserved</param>
        /// <param name="reserveZeroWhenQuantityTooLow">Flag to indicate whether allow to reserve quantity if Quantity is low.</param>
        /// <returns>Returns reserved quantity</returns>
        private int InventoryReserveQuantity(string productBvin, string variantId, int quantity,
            bool reserveZeroWhenQuantityTooLow)
        {
            var inv = ProductInventories.FindByProductIdAndVariantId(productBvin, variantId);

            // If no inventory, assume available
            if (inv == null)
                return quantity;

            var prod = Products.FindWithCache(productBvin);
            if (prod == null)
                return quantity;

            switch (prod.InventoryMode)
            {
                case ProductInventoryMode.AlwayInStock:
                    return quantity;
                case ProductInventoryMode.WhenOutOfStockAllowBackorders:
                    inv.QuantityReserved += quantity;
                    ProductInventories.Update(inv);
                    return quantity;
                case ProductInventoryMode.WhenOutOfStockShow:
                case ProductInventoryMode.WhenOutOfStockHide:
                    if (inv.QuantityAvailableForSale < quantity)
                    {
                        if (reserveZeroWhenQuantityTooLow)
                            return 0;

                        inv.QuantityReserved += inv.QuantityAvailableForSale;
                        ProductInventories.Update(inv);
                        return inv.QuantityAvailableForSale;
                    }
                    inv.QuantityReserved += quantity;
                    ProductInventories.Update(inv);
                    return quantity;
                default:
                    throw new NotImplementedException("InventoryMode is not implemented");
            }
        }

        /// <summary>
        ///     Update product inventory to unreserve the product quantity.
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="variantId">Variant unique identifier</param>
        /// <param name="quantity">Quantity needs to be unreserved</param>
        /// <returns>Returns true if unreserved successfully otherwise returns false.</returns>
        private bool InventoryUnreserveQuantity(string productId, string variantId, int quantity)
        {
            var result = false;
            var inv = ProductInventories.FindByProductIdAndVariantId(productId, variantId);
            if (inv != null)
            {
                inv.QuantityReserved -= quantity;
                ProductInventories.Update(inv);
                UpdateProductVisibleStatusAndSave(productId);
            }
            return result;
        }

        /// <summary>
        ///     Check available quantity for purchase for given product and variant.
        /// </summary>
        /// <param name="p"><see cref="Product" /> instance</param>
        /// <param name="variantId">Variant unique identifier</param>
        /// <returns>Returns quantity available for purchase</returns>
        private int InventoryQuantityAvailableForPurchase(Product p, string variantId)
        {
            var result = 0;

            if (p.InventoryMode == ProductInventoryMode.AlwayInStock)
                return int.MaxValue;

            var inv = ProductInventories.FindByProductId(p.Bvin);
            if (!string.IsNullOrEmpty(variantId))
            {
                var vi = inv.Where(pi => pi.VariantId == variantId).FirstOrDefault();
                if (vi != null)
                {
                    result = vi.QuantityAvailableForSale;
                }
            }
            else
            {
                // no variants
                result = inv.Where(y => y.QuantityAvailableForSale >= 0).Sum(y => y.QuantityAvailableForSale);
            }

            return result;
        }

        /// <summary>
        ///     Register analytics events information for the given product.
        ///     Register events for product copy, price changed and image changed.
        /// </summary>
        /// <param name="oldValue">Old <see cref="Product" /> instance </param>
        /// <param name="newValue">New <see cref="Product" /> instance</param>
        private void RegisterAnalyticsEvents(Product oldValue, Product newValue)
        {
            var analyticsService = Factory.CreateService<AnalyticsService>(Context);

            if (oldValue.ProductName != newValue.ProductName
                || oldValue.ShortDescription != newValue.ShortDescription
                || oldValue.LongDescription != newValue.LongDescription
                || oldValue.Keywords != newValue.Keywords
                || oldValue.MetaTitle != newValue.MetaTitle
                || oldValue.MetaKeywords != newValue.MetaKeywords
                || oldValue.MetaDescription != newValue.MetaDescription
                || !oldValue.Tabs.SequenceEqual(newValue.Tabs)
                )
                analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.ProductCopyChanged,
                    newValue.Bvin);

            if (oldValue.ListPrice != newValue.ListPrice
                || oldValue.SitePrice != newValue.SitePrice
                || oldValue.SiteCost != newValue.SiteCost
                || oldValue.SitePriceOverrideText != newValue.SitePriceOverrideText)
                analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.ProductPriceChanged,
                    newValue.Bvin);


            if (oldValue.ImageFileSmall != newValue.ImageFileSmall
                || oldValue.ImageFileMedium != newValue.ImageFileMedium)
                analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.ProductImagesChanged,
                    newValue.Bvin);
        }

        /// <summary>
        ///     Register analytics events information for the category.
        ///     Register Category copy changed, image changed events.
        /// </summary>
        /// <param name="oldValue">Old <see cref="Category" /> instance </param>
        /// <param name="newValue">New  <see cref="Category" /> instance</param>
        private void RegisterAnalyticsEvents(Category oldValue, Category newValue)
        {
            var analyticsService = Factory.CreateService<AnalyticsService>(Context);

            if (oldValue.Name != newValue.Name
                || oldValue.Description != newValue.Description
                || oldValue.Keywords != newValue.Keywords
                || oldValue.MetaTitle != newValue.MetaTitle
                || oldValue.MetaKeywords != newValue.MetaKeywords
                || oldValue.MetaDescription != newValue.MetaDescription)
                analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.CategoryCopyChanged,
                    newValue.Bvin);

            if (oldValue.ImageUrl != newValue.ImageUrl
                || oldValue.BannerImageUrl != newValue.BannerImageUrl)
                analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.CategoryImagesChanged,
                    newValue.Bvin);
        }

        /// <summary>
        ///     Register product changed event
        /// </summary>
        /// <param name="image"><see cref="ProductImage" /> instance</param>
        private void RegisterAnalyticsEvents(ProductImage image)
        {
            var analyticsService = Factory.CreateService<AnalyticsService>(Context);
            analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.ProductImagesChanged,
                image.ProductId);
        }

        /// <summary>
        ///     Register product bundled event.
        /// </summary>
        /// <param name="bundledProduct"><see cref="BundledProduct" /> instance</param>
        private void RegisterAnalyticsEvents(BundledProduct bundledProduct)
        {
            var analyticsService = Factory.CreateService<AnalyticsService>(Context);
            analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.ProductWasBundled,
                bundledProduct.BundledProductId);
        }

        /// <summary>
        ///     Register category product updated event.
        /// </summary>
        /// <param name="categoryId">Category unique identifer</param>
        private void RegisterAnalyticsEvents(string categoryId)
        {
            var analyticsService = Factory.CreateService<AnalyticsService>(Context);
            analyticsService.RegisterEvent(Context.CurrentAccount.Bvin, ActionTypes.CategoryProductsUpdated, categoryId);
        }

        #endregion
    }
}