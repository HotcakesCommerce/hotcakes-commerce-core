#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.IO;
using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This method is used to perform the test against Product images end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductImageTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test Create, Find , Update and Delete end points.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\ProductImage.jpg", "Data")]
        public void ProductImages_CreateFindUpdateDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Product Image 
            var productImage = new ProductImageDTO
            {
                AlternateText = "test alternate",
                Caption = "test caption",
                FileName = "ProductImage.jpg",
                ProductId = TestConstants.TestProductBvin,
                StoreId = 1
            };

            //Create product image
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ProductImage.jpg");
            var imageData = File.ReadAllBytes(imagePath);
            var createResponse = proxy.ProductImagesCreate(productImage, imageData);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Product image
            var findResponse = proxy.ProductImagesFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.Caption, findResponse.Content.Caption);
            Assert.AreEqual(createResponse.Content.FileName, findResponse.Content.FileName);

            //Find Product image
            var findByProductResponse = proxy.ProductImagesFindAllByProduct(productImage.ProductId);
            CheckErrors(findByProductResponse);

            //Update product image
            findResponse.Content.Caption = findResponse.Content.Caption + "updated";
            var updateResponse = proxy.ProductImagesUpdate(findResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(findResponse.Content.Caption, updateResponse.Content.Caption);

            //Upload product image
            var imageUplaodResponse = proxy.ProductImagesUpload(TestConstants.TestProductBvin,
                createResponse.Content.Bvin, "NewProductImage.jpg", imageData);
            CheckErrors(imageUplaodResponse);
            Assert.IsTrue(imageUplaodResponse.Content);

            //Delete Product Image
            var deleteResponse = proxy.ProductImagesDelete(findResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }

        /// <summary>
        ///     This test method is used to test find all product images.
        /// </summary>
        [TestMethod]
        public void ProductImage_TestFindAll()
        {
            // Create API proxy
            var proxy = CreateApiProxy();

            var findAllResponse = proxy.ProductImagesFindAll();

            CheckErrors(findAllResponse);
        }
    }
}