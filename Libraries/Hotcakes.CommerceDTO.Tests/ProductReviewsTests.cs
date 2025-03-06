#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to test against Product review end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductReviewsTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to Find all Product reviews and for product.
        /// </summary>
        [TestMethod]
        public void ProductReviews_FindAllAndFindForProduct()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test ProductReview as prerequisites          
            var productReviewRespose = SampleData.CreateTestProductReview(proxy);

            //Get list of all reviews
            var getallResponse = proxy.ProductReviewsFindAll();
            CheckErrors(getallResponse);

            //Find reviews for the product
            var getallByProductResponse = proxy.ProductReviewsByProduct(getallResponse.Content.First().ProductBvin);
            CheckErrors(getallByProductResponse);

            //Remove Test ProductReview
            SampleData.RemoveTestProductReview(proxy, productReviewRespose.Bvin);
        }

        /// <summary>
        ///     This method is used to Create, Update, Find and Delete Product reviews.
        /// </summary>
        [TestMethod]
        public void ProductReviews_CreateUpdateFindDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Review
            var productReview = new ProductReviewDTO
            {
                Approved = true,
                ProductBvin = productRespose.Bvin,
                Rating = ProductReviewRatingDTO.FiveStars,
                UserID = "1",
                ReviewDateUtc = DateTime.UtcNow
            };
            var createResponse = proxy.ProductReviewsCreate(productReview);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Product review
            var findResponse = proxy.ProductReviewsFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.ProductBvin, findResponse.Content.ProductBvin);
            Assert.AreEqual(createResponse.Content.Rating, findResponse.Content.Rating);

            //Update product review
            createResponse.Content.Rating = ProductReviewRatingDTO.TwoStars;
            var updateResponse = proxy.ProductReviewsUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.Rating, updateResponse.Content.Rating);

            //Delete product review
            var deleteResponse = proxy.ProductReviewsDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);
        }
    }
}