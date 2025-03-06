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
using System.IO;
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform test against Product files in REST API.
    /// </summary>
    [TestClass]
    public class ProductFilesTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test Create, Update and Delete Product files end points.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\ProductImage.jpg", "Data")]
        public void ProductFiles_CreateUpdateDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product File.
            var productfile = new ProductFileDTO
            {
                FileName = "Test" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                MaxDownloads = 5,
                ShortDescription = "My test file for product",
                ProductId = productRespose.Bvin,
                StoreId = 1,
                AvailableMinutes = 20
            };
            var createResponse = proxy.ProductFilesCreate(productfile);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Update product file.
            createResponse.Content.ShortDescription = createResponse.Content.ShortDescription + "updated";
            var updateResponse = proxy.ProductFilesUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.ShortDescription, updateResponse.Content.ShortDescription);

            //Find product file.
            var findResponse = proxy.ProductFilesFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(updateResponse.Content.MaxDownloads, findResponse.Content.MaxDownloads);


            //Upload product file first part.
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ProductImage.jpg");
            var imageData = File.ReadAllBytes(imagePath);
            var firstPartData = new byte[imageData.Length/2];
            var secondPart = new byte[imageData.Length/2];
            Array.Copy(imageData, 0, firstPartData, 0, imageData.Length/2);
            Array.Copy(imageData, imageData.Length/2, firstPartData, 0, imageData.Length/2);
            var firstPartResponse = proxy.ProductFilesDataUploadFirstPart(findResponse.Content.Bvin, "ProductImage.jpg",
                "Test UPload", firstPartData);
            Assert.IsTrue(firstPartResponse.Content);

            //Upload product file second part
            var secondPartResponse = proxy.ProductFilesDataUploadAdditionalPart(findResponse.Content.Bvin,
                "ProductImage.jpg", secondPart);
            Assert.IsTrue(firstPartResponse.Content);

            //Add file to product.
            var addfileToProductResponse = proxy.ProductFilesAddToProduct(productRespose.Bvin,
                findResponse.Content.Bvin, 5, 10);
            Assert.IsTrue(addfileToProductResponse.Content);

            //Find product files for Product
            var findforProductResponse = proxy.ProductFilesFindForProduct(productRespose.Bvin);
            CheckErrors(findforProductResponse);
            Assert.IsTrue(findforProductResponse.Content.Count > 0);

            //Remove product files for Product
            var removefileToProductResponse = proxy.ProductFilesRemoveFromProduct(productRespose.Bvin,
                findResponse.Content.Bvin);
            Assert.IsTrue(removefileToProductResponse.Content);

            //Delete product file
            var deleteResponse = proxy.ProductFilesDelete(findResponse.Content.Bvin);
            Assert.IsTrue(deleteResponse.Content);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);
        }

        
    }
}