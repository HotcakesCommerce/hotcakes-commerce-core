using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductCategoryRepository : IDisposable
    {

        #region Product Category Repository Service

        /// <summary>
        /// Gets the total product category.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductCategory();

        /// <summary>
        /// Gets the total category.
        /// </summary>
        /// <returns></returns>
        int GetTotalCategory();

        /// <summary>
        /// Gets the add product category.
        /// </summary>
        /// <returns></returns>
        List<string> GetAddProductCategory();

        /// <summary>
        /// Gets the delete product category.
        /// </summary>
        /// <returns></returns>
        List<string> GetDeleteProductCategory();

        /// <summary>
        /// Gets the find all count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllCount();

        /// <summary>
        /// Gets the find all for all store count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllForAllStoreCount();

        /// <summary>
        /// Gets the find all paged count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllPagedCount();


        #endregion

    }
}
