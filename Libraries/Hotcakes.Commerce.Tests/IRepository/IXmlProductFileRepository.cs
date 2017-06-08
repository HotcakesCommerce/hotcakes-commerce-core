using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductFileRepository : IDisposable
    {
        #region Product Files Repository Service

        /// <summary>
        /// Gets the total product file.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductFileCount();

        /// <summary>
        /// Gets the total all product file.
        /// </summary>
        /// <returns></returns>
        int GetTotalAvailableFileCount();

        /// <summary>
        /// Gets the add product file.
        /// </summary>
        /// <returns></returns>
        List<ProductFile> GetAddProductFile();

        #endregion
    }
}
