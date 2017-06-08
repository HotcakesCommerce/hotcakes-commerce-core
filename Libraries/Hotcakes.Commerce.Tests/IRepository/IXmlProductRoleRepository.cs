using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductRoleRepository : IDisposable
    {
        #region Product Role Repository Service

        /// <summary>
        /// Gets the total product role count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductRoleCount();

        /// <summary>
        /// Gets the delete product role.
        /// </summary>
        /// <returns></returns>
        string GetDeleteProductRole();

        /// <summary>
        /// Gets the add product role.
        /// </summary>
        /// <returns></returns>
        string GetAddProductRole();

        #endregion
    }
}
