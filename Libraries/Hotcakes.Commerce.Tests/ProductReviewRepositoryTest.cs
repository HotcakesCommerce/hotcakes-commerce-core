#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020-2025 Upendo Ventures, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Tests.IRepository;
using Hotcakes.Commerce.Tests.XmlRepository;
using Hotcakes.Commerce.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    [TestClass]
	public class ProductReviewRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductReviewRepository _irepoproductreview;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductReviewRepositoryTest()
        {
            _irepoproductreview = new XmlProductReviewRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductReview_TestInOrder()
        {
            CreateProduct();

            CreateProductReview();
            EditProductReview();
            LoadProductReview();
            FindByProductIdPaged();
            MergeProductReview();
            DeleteProductReview();
            DeleteForProductId();

        }

        #region Product Review Load/Add/Edit/Delete Test Cases
        /// <summary>
        /// Loads the product review.
        /// </summary>
        //[TestMethod]
        public void LoadProductReview()
        {
            //Arrange
            var count = _irepoproductreview.GetTotalProductReviewCount();
            var prj = GetRootProduct();

            //Act
            var resultcount = _application.CatalogServices.ProductReviews.FindByProductId(prj.Bvin);

            //Assert
            Assert.AreEqual(count, resultcount.Count);
        }

        /// <summary>
        /// Creates the product review.
        /// </summary>
        //[TestMethod]
        public void CreateProductReview()
        {
            //Arrange
            var review = _irepoproductreview.GetAddProductReview();
            var prj = GetRootProduct();
            review.ProductBvin = prj.Bvin;

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductReviews.Create(review));
        }

        /// <summary>
        /// Edits the product review.
        /// </summary>
        //[TestMethod]
        public void EditProductReview()
        {
            //Arrange
            var review = _irepoproductreview.GetEditProductReview();
            var prj = GetRootProduct();

            var editreview = _application.CatalogServices.ProductReviews.FindByProductId(prj.Bvin).OrderBy(x => x.ReviewDateUtc).FirstOrDefault();
            if (editreview == null) return;
            editreview.Approved = review.Approved;
            editreview.Karma = review.Karma;
            editreview.Rating = review.Rating;
            editreview.Description = review.Description;

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductReviews.Update(editreview));
        }

        /// <summary>
        /// Deletes the product review.
        /// </summary>
        //[TestMethod]
        public void DeleteProductReview()
        {
            //Arrange
            var prj = GetRootProduct();
            var deletereview = _application.CatalogServices.ProductReviews.FindByProductId(prj.Bvin).OrderBy(x => x.ReviewDateUtc).FirstOrDefault();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductReviews.Delete(deletereview.Bvin));
        }



        /// <summary>
        /// Finds the by product identifier paged.
        /// </summary>
        //TestMethod]
        public void FindByProductIdPaged()
        {
            //Arrange
            var prj = GetRootProduct();
            var count = _irepoproductreview.FindByProductIdPagedCount();

            //Act
            var resultcount = _application.CatalogServices.ProductReviews.FindByProductIdPaged(prj.Bvin, 1, int.MaxValue).Count;

            //Assert
            Assert.AreEqual(count, resultcount);
        }

        /// <summary>
        /// Finds the not approved.
        /// </summary>
        [TestMethod]
		public void ProductReview_FindNotApproved()
        {
            //Arrange
            var count = _irepoproductreview.FindNotApprovedCount();

            //Act
            var resultcount = _applicationDB.CatalogServices.ProductReviews.FindNotApproved(1, int.MaxValue).Count;

            //Assert
            Assert.AreEqual(count, resultcount);
        }

        /// <summary>
        /// Deletes for product identifier.
        /// </summary>
        //[TestMethod]
        public void DeleteForProductId()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductReviews.DeleteForProductId(prj.Bvin));
        }

        /// <summary>
        /// Merges the product review.
        /// </summary>
        //[TestMethod]
        public void MergeProductReview()
        {
            //Arrange
            var prj = GetRootProduct();
            var addreview = _irepoproductreview.GetAddProductReview();
            addreview.ProductBvin = prj.Bvin;
            var editreview1 = _irepoproductreview.GetEditProductReview();
            var editreview = _application.CatalogServices.ProductReviews.FindByProductId(prj.Bvin).OrderBy(x => x.ReviewDateUtc).FirstOrDefault();
            if (editreview == null) return;
            editreview.Approved = editreview1.Approved;
            editreview.Karma = editreview1.Karma;
            editreview.Rating = editreview1.Rating;
            editreview.Description = editreview1.Description;
            var lst = new List<ProductReview> { addreview, editreview };

            //Act
            _application.CatalogServices.ProductReviews.MergeList(prj.Bvin, lst);
            var prj1 = GetRootProduct();

            //Assert
            Assert.AreEqual(prj.Reviews.Count + 1, prj1.Reviews.Count);

        }


        #endregion
    }
}
