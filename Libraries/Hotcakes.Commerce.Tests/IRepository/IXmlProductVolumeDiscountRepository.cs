using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductVolumeDiscountRepository : IDisposable
    {
       
        #region Product Volume Discount Repository Service

        /// <summary>
        /// Gets the total product volume discount count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProduct_VD_Count();


        /// <summary>
        /// Gets the add product volume discount.
        /// </summary>
        /// <returns></returns>
        ProductVolumeDiscount GetAddProduct_VD();


        /// <summary>
        /// Gets the edit product volume discount.
        /// </summary>
        /// <returns></returns>
        ProductVolumeDiscount GetEditProduct_VD();

        #endregion

    }
}
