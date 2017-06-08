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
