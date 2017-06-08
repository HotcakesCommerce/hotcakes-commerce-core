using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductInventoryRepository : IDisposable
    {
        #region Product Inventory Repository Service

        /// <summary>
        /// Gets the product inventory.
        /// </summary>
        /// <returns></returns>
        ProductInventory GetProductInventory();

        /// <summary>
        /// Gets the product inventory information.
        /// </summary>
        /// <returns></returns>
        Product GetProductInventoryInfo();

        /// <summary>
        /// Gets the add product inventory.
        /// </summary>
        /// <returns></returns>
        ProductInventory GetEditProductInventory();

        /// <summary>
        /// Gets the add product inventory information.
        /// </summary>
        /// <returns></returns>
        Product GetEditProductInventoryInfo();

        /// <summary>
        /// Gets the find all low stock count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllLowStockCount();


        #endregion
    }
}
