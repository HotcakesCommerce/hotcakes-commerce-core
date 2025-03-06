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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Reporting;

namespace Hotcakes.Commerce.Catalog
{
    public interface IProductRepository
    {
        int CountOfAll();

        /// <summary>
        ///     Creates the specified product. Use CatalogService.ProductsCreateWithInventory instead to update search index too.
        /// </summary>
        /// <param name="product">The product to create</param>
        /// <returns>If operation succeeded</returns>
        bool Create(Product product);

        bool Delete(string bvin);
        bool DeleteForStore(string bvin, long storeId);
        Product Find(string bvin);
        int FindAllCount();
        int FindAllCount(long storeId);
        int FindAllForAllStoresCount();
        List<Product> FindAllPaged(int pageNumber, int pageSize);
        List<Product> FindAllPagedWithCache(int pageNumber, int pageSize);
        List<Product> FindAllPagedForAllStores(int pageNumber, int pageSize);
        List<Product> FindAllPagedForAllStoresWithCache(int pageNumber, int pageSize);
        int FindCountByCriteria(ProductSearchCriteria criteria);
        List<Product> FindByCriteria(ProductSearchCriteria criteria, bool useCache = true);

        List<Product> FindByCriteria(ProductSearchCriteria criteria, int pageNumber, int pageSize, ref int totalCount,
            bool useCache = true);

        Product FindBySku(string sku);
        Product FindBySkuForStore(string sku, long storeId);
        Product FindBySlug(string urlSlug);
        Product FindBySlugForStore(string urlSlug, long storeId);
        int FindCountByProductType(string productTypeId);
        List<Product> FindFeatured(int pageNumber, int pageSize);
        List<string> FindFeaturedProductBvins(int pageNumber, int pageSize);
        Product FindForAllStores(string bvin);
        List<Product> FindMany(IEnumerable<string> bvins);
        List<Product> FindManyWithCache(IEnumerable<string> bvins);
        Product FindWithCache(string bvin);

        bool Clone(string productId, string newSku, string newUrlSlug, ProductStatus newStatus, bool cloneImages,
            out string newProductId);

        List<Product> GetMostPurchasedWith(string productBvin, SalesPeriod period, int maxItemsToReturn);
        bool IsSkuExist(string sku, Guid? excludeProductId = null);

        /// <summary>
        ///     Updates the specified product. Use CatalogService.ProductsUpdateWithSearchRebuild instead  to update search index
        ///     too.
        /// </summary>
        /// <param name="product">The product to update.</param>
        /// <param name="mergeSubItems">if set to <c>true</c> [merge sub items].</param>
        /// <returns>
        ///     If operation succeeded
        /// </returns>
        bool Update(Product product, bool mergeSubItems = true);

        DalSingleOperationResult<Product> UpdateAdv(Product c, bool mergeSubItems = true);
        List<Product> FindManySkus(List<string> skus);
        List<string> FindAllBvinsForStore(long storeId);
        int FindProductsCountByCriteria(ProductSearchCriteria criteria, bool useCache = true);
    }
}