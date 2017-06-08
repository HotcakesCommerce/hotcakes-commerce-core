using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductImageRepository : IDisposable
    {

        #region Product Image Repository Service
        /// <summary>
        /// Gets the total product image count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductImageCount();

        /// <summary>
        /// Gets the add product image.
        /// </summary>
        /// <returns></returns>
        List<ProductImage> GetAddProductImage();

        /// <summary>
        /// Gets the merge product image.
        /// </summary>
        /// <returns></returns>
        List<ProductImage> GetMergeProductImage();

        #endregion

    }
}
