#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    ///     This class is used to perform test against the Product end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductsTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test get list of paged products.
        /// </summary>
        [TestMethod]
        public void Product_TestFindPage()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Get paged products
            var findResponse = proxy.ProductsFindPage(1, 100);
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to test get list of all products.
        /// </summary>
        [TestMethod]
        public void Product_TestFindAll()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Get All products
            var findResponse = proxy.ProductsFindAll();
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to test Find product by Sku and Slug.
        /// </summary>
        [TestMethod]
        public void Product_TestFindSkuAndSlug()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Fiend product by Sku.
            var skuResponse = proxy.ProductsFindBySku(TestConstants.TestProduct1Sku);

            //Find product by Slug.
            var slugResponse = proxy.ProductsBySlug(TestConstants.TestProduct1Slug);

            CheckErrors(skuResponse);
            CheckErrors(slugResponse);

            Assert.AreEqual(skuResponse.Content.Bvin, slugResponse.Content.Bvin);
            Assert.AreEqual(skuResponse.Content.ProductName, slugResponse.Content.ProductName);
            Assert.AreEqual(skuResponse.Content.ShippingDetails.ExtraShipFee,
                slugResponse.Content.ShippingDetails.ExtraShipFee);
        }

        /// <summary>
        ///     This method is used to test count of products.
        /// </summary>
        [TestMethod]
        public void Product_TestCountOfAll()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Get count of all products.
            var countResponse = proxy.ProductsCountOfAll();
            CheckErrors(countResponse);
            Assert.IsTrue(countResponse.Content > 0);
        }

        /// <summary>
        ///     This method is used to test Create and Delete product.
        /// </summary>
        [TestMethod]
        public void Product_TestCreateAndDelete()
        {
            var proxy = CreateApiProxy();

            var dto = new ProductDTO
            {
                ProductName = "Unit tests product",
                AllowReviews = true,
                ListPrice = 687,
                LongDescription = "This is test product",
                Sku = "TST100",
                StoreId = 1,
                TaxExempt = true
            };

            //Create
            var resP1 = proxy.ProductsCreate(dto, null);
            Assert.AreEqual(dto.ProductName, resP1.Content.ProductName);
            Assert.AreEqual(dto.ListPrice, resP1.Content.ListPrice);
            Assert.AreEqual(dto.Sku, resP1.Content.Sku);
            Assert.AreEqual(dto.LongDescription, resP1.Content.LongDescription);

            var resP2 = proxy.ProductsCreate(dto, null);
            Assert.AreNotEqual(resP1.Content.UrlSlug, resP2.Content.UrlSlug);

            //Delete
            var resD1 = proxy.ProductsDelete(resP1.Content.Bvin);
            CheckErrors(resD1);
            Assert.IsTrue(resD1.Content);

            var resD2 = proxy.ProductsDelete(resP2.Content.Bvin);
            CheckErrors(resD2);
            Assert.IsTrue(resD2.Content);
        }

        /// <summary>
        ///     This method is used to test Product update.
        /// </summary>
        [TestMethod]
        public void Product_TestUpdate()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find product by SKU
            var findResponse = proxy.ProductsFindBySku(TestConstants.TestProduct1Sku);
            CheckErrors(findResponse);

            //Update product
            var product = findResponse.Content;
            var oldMetaTitle = product.MetaTitle;
            product.MetaTitle = "New meta title";
            var updateResponse = proxy.ProductsUpdate(product);
            CheckErrors(updateResponse);
            Assert.AreEqual(updateResponse.Content.MetaTitle, "New meta title");

            //Update product
            product.MetaTitle = oldMetaTitle;
            var update2Response = proxy.ProductsUpdate(product);
            CheckErrors(update2Response);
            Assert.AreEqual(update2Response.Content.MetaTitle, oldMetaTitle);
        }

        /// <summary>
        ///     This method is used to test the Image upload for the product.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\ProductImage.jpg", "Data")]
        public void Product_TestImageUpload()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find product by SKU.
            var findResponse = proxy.ProductsFindBySku(TestConstants.TestProduct1Sku);
            CheckErrors(findResponse);

            //Upload image
            var product = findResponse.Content;
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ProductImage.jpg");
            var imageData = File.ReadAllBytes(imagePath);
            var fileName = Path.GetFileName(imagePath);

            var uploadResponse = proxy.ProductsMainImageUpload(product.Bvin, fileName, imageData);
            CheckErrors(uploadResponse);
            Assert.IsTrue(uploadResponse.Content);
        }

        /// <summary>
        ///     This method is used to test get list of products for category.
        /// </summary>
        [TestMethod]
        public void Product_ProductsFindForCategory()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Get Category by Slug.
            var categoryInfo = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);

            //Get list of products by Category unique identifier.
            var pagedResults = proxy.ProductsFindForCategory(categoryInfo.Content.Bvin, 1, 15);
            CheckErrors(pagedResults);
        }

        /// <summary>
        ///     This method is used test Product indexing.
        /// </summary>
        [TestMethod]
        public void Product_SearchManagerIndexProduct()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Index product by its unique identifier
            var searchIndexResponse = proxy.SearchManagerIndexProduct(TestConstants.TestProductBvin);

            CheckErrors(searchIndexResponse);

            Assert.IsTrue(searchIndexResponse.Content);
        }

        /// <summary>
        ///     This method is used to test slug function.
        /// </summary>
        [TestMethod]
        public void Product_SlugifyTest()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Test slug method
            var sluggedText = proxy.UtilitiesSlugify("abcXX??&&newtest");

            CheckErrors(sluggedText);

            Assert.IsFalse(string.IsNullOrEmpty(sluggedText.Content));
        }

        /// <summary>
        ///     This method is used to test clear all products.
        /// </summary>
        [TestMethod]
        public void Product_ClearAll()
        {
            var proxy = CreateApiProxy();
            var clearResponse = proxy.ProductsClearAll(100);
            CheckErrors(clearResponse);
            Assert.IsTrue(clearResponse.Content.ProductsCleared > 0);
        }
    }
}