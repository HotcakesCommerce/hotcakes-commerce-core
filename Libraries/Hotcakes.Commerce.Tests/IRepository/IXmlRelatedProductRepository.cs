using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlRelatedProductRepository : IDisposable
    {
        #region Related Product Repository Service

        /// <summary>
        /// Gets the total related product count.
        /// </summary>
        /// <returns></returns>
        int GetTotalRelatedProductCount();

        /// <summary>
        /// Gets the total product count.
        /// </summary>
        /// <returns></returns>
        Dictionary<int, ProductSearchCriteria> GetTotalProductCount();

        /// <summary>
        /// Gets the add related product.
        /// </summary>
        /// <returns></returns>
        List<string> GetAddRelatedProduct();

        /// <summary>
        /// Gets the delete related product.
        /// </summary>
        /// <returns></returns>
        List<string> GetDeleteRelatedProduct();

        #endregion
    }
}
