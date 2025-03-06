#region License

// Distributed under the MIT License
// ============================================================
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
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductRepository : IDisposable
    {
        #region Product Repository Service
        #region Product Load/Add/Edit/Delete Functions
        /// <summary>
        /// Gets the total produt count.
        /// </summary>
        /// <returns></returns>
        Dictionary<int, ProductSearchCriteria> GetTotalProdutCount();

        /// <summary>
        /// Gets the total product type count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductTypeCount();

        /// <summary>
        /// Gets the total populate columns count.
        /// </summary>
        /// <returns></returns>
        int GetTotalPopulateColumnsCount();

        /// <summary>
        /// Gets the category display template count.
        /// </summary>
        /// <returns></returns>
        int GetTotalDisplayTemplateCount();

        /// <summary>
        /// Gets the total manufacturer count.
        /// </summary>
        /// <returns></returns>
        int GetTotalManufacturerCount();

        /// <summary>
        /// Gets the total vendor count.
        /// </summary>
        /// <returns></returns>
        int GetTotalVendorCount();

        /// <summary>
        /// Gets the total tax.
        /// </summary>
        /// <returns></returns>
        int GetTotalTax();

        /// <summary>
        /// Gets the delete product.
        /// </summary>
        /// <returns></returns>
        string GetDeleteProductSku();

        /// <summary>
        /// Gets the name of the edit product.
        /// </summary>
        /// <returns></returns>
        string GetEditProductSku();

        /// <summary>
        /// Gets the add product.
        /// </summary>
        /// <returns></returns>
        Product GetAddProduct();

        /// <summary>
        /// Gets the edit product.
        /// </summary>
        /// <returns></returns>
        Product GetEditProduct();

        /// <summary>
        /// Gets the add product taxonomy tags.
        /// </summary>
        /// <returns></returns>
        string GetProductTaxonomyTags();

        /// <summary>
        /// Gets the clone product information.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, Dictionary<string, string>> GetCloneProductInfo();

        /// <summary>
        /// Gets the add product property value.
        /// </summary>
        /// <returns></returns>
        Dictionary<ProductPropertyType, string> GetAddProductPropertyValue();

        /// <summary>
        /// Gets the edit product property value.
        /// </summary>
        /// <returns></returns>
        Dictionary<ProductPropertyType, string> GetEditProductPropertyValue();

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <returns></returns>
        string GetProductSku();
        #endregion
        #region Product Find Functions
        /// <summary>
        /// Finds all paged for all store count.
        /// </summary>
        /// <returns></returns>
        int FindAllPagedForAllStoreCount();

        /// <summary>
        /// Finds all paged count.
        /// </summary>
        /// <returns></returns>
        int FindAllPagedCount();

        /// <summary>
        /// Finds all count for all store count.
        /// </summary>
        /// <returns></returns>
        int FindAllCountForAllStoreCount();

        /// <summary>
        /// Finds all count.
        /// </summary>
        /// <returns></returns>
        int FindAllCount();

        /// <summary>
        /// Features the product count.
        /// </summary>
        /// <returns></returns>
        int FeatureProductCount();

        /// <summary>
        /// Finds the many by sku.
        /// </summary>
        /// <returns></returns>
        List<string> FindManyBySku();

        /// <summary>
        /// Finds the by slug.
        /// </summary>
        /// <returns></returns>
        string FindBySlug();

        /// <summary>
        /// Findmanies the by bvin.
        /// </summary>
        /// <returns></returns>
        List<string> FindmanyByBvin();

        #endregion
        #endregion

        #region Product Tab

        /// <summary>
        /// Gets the total product tab.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductTabCount();

        /// <summary>
        /// Gets the add product tab.
        /// </summary>
        /// <returns></returns>
        ProductDescriptionTab GetAddProductTab();

        /// <summary>
        /// Gets the edit product tab.
        /// </summary>
        /// <returns></returns>
        ProductDescriptionTab GetEditProductTab();

        /// <summary>
        /// Gets the name of the edit product tab.
        /// </summary>
        /// <returns></returns>
        string GetEditProductTabName();

        /// <summary>
        /// Gets the name of the delete product tab.
        /// </summary>
        /// <returns></returns>
        string GetDeleteProductTabName();


        #endregion

    }
}
