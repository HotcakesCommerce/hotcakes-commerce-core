using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlProductReviewRepository : IDisposable
    {

        #region Product Review Repository Service

        /// <summary>
        /// Gets the total product review count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductReviewCount();

        /// <summary>
        /// Gets the add product review.
        /// </summary>
        /// <returns></returns>
        ProductReview GetAddProductReview();

        /// <summary>
        /// Gets the edit product review.
        /// </summary>
        /// <returns></returns>
        ProductReview GetEditProductReview();






        /// <summary>
        /// Finds the by product identifier paged count.
        /// </summary>
        /// <returns></returns>
        int FindByProductIdPagedCount();

        /// <summary>
        /// Finds the not approved count.
        /// </summary>
        /// <returns></returns>
        int FindNotApprovedCount();



        #endregion

    }
}
