using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductVariantRepository : IDisposable
    {

        #region Product Variant Repository Service

        /// <summary>
        /// Gets the total product variant option count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProduct_VO_Count();

        /// <summary>
        /// Gets the total product variant count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductVariantCount();

        /// <summary>
        /// Gets the edit product variant.
        /// </summary>
        /// <returns></returns>
        Variant GetEditProductVariant();

        #endregion

    }
}
