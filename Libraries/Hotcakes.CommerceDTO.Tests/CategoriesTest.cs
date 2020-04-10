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
    ///     This class is used to perform the test against the Category end points in REST API.
    /// </summary>
    [TestClass]
    public class CategoriesTest : ApiTestBase
    {
        /// <summary>
        ///     This test method is used to test find all categories end points.
        /// </summary>
        [TestMethod]
        public void Category_TestFindAll()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.CategoriesFindAll();

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to test create, find, delete category end points.
        /// </summary>
        [TestMethod]
        public void Category_TestCreateAndFindAndDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Category
            var category = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category",
                RewriteUrl = "test-category-from-unit-tests"
            };
            var createResponse = proxy.CategoriesCreate(category);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));


            //Find Category by Bvin
            var findResponse = proxy.CategoriesFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.Name, findResponse.Content.Name);
            Assert.AreEqual(createResponse.Content.RewriteUrl, findResponse.Content.RewriteUrl);

            //Find Category by Slug
            var findBySlugResponse = proxy.CategoriesFindBySlug(createResponse.Content.RewriteUrl);
            CheckErrors(findBySlugResponse);
            Assert.AreEqual(createResponse.Content.Name, findBySlugResponse.Content.Name);
            Assert.AreEqual(createResponse.Content.RewriteUrl, findBySlugResponse.Content.RewriteUrl);

            //Delete Category by Bvin
            var deleteResponse = proxy.CategoriesDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }

        /// <summary>
        ///     This method used to test create and delete category end points.
        /// </summary>
        [TestMethod]
        public void Category_TestCreate()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Find Category by Slug.
            var resParent = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);
            CheckErrors(resParent);

            //Create Category.
            var dto = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category",
                ParentId = resParent.Content.Bvin
            };
            var resC = proxy.CategoriesCreate(dto);
            CheckErrors(resC);
            Assert.IsFalse(string.IsNullOrEmpty(resC.Content.Bvin));
            Assert.IsFalse(string.IsNullOrEmpty(resC.Content.RewriteUrl));
            Assert.AreEqual(dto.Name, resC.Content.Name);

            //Delete Category.
            var resD = proxy.CategoriesDelete(resC.Content.Bvin);
            CheckErrors(resD);
            Assert.IsTrue(resD.Content);
        }

        /// <summary>
        ///     This method is used to test update category end points.
        /// </summary>
        [TestMethod]
        public void Category_TestUpdate()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();


            //Find Category by slug.
            var resParent = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);
            CheckErrors(resParent);

            //Create Category.
            var dto = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category",
                ParentId = resParent.Content.Bvin
            };
            var resC = proxy.CategoriesCreate(dto);
            CheckErrors(resC);

            //Update Category.
            var testCategory = resC.Content;
            var oldName = testCategory.Name;
            testCategory.Hidden = !testCategory.Hidden;
            testCategory.Name = "Category Name was changed";
            var updateResponse = proxy.CategoriesUpdate(testCategory);
            CheckErrors(updateResponse);
            Assert.AreEqual(testCategory.Hidden, updateResponse.Content.Hidden);
            Assert.AreEqual(testCategory.Name, updateResponse.Content.Name);
            Assert.AreEqual(testCategory.Name, "Category Name was changed");

            //Delete Category.
            proxy.CategoriesDelete(testCategory.Bvin);
        }


        /// <summary>
        ///     This method is used to test Quick product and category association end point.
        /// </summary>
        [TestMethod]
        public void Category_TestAssociationsQuickCreate()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find category by Slug
            var resParent = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);
            CheckErrors(resParent);

            //Create Category and Product.
            var category = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category",
                ParentId = resParent.Content.Bvin
            };
            var product = new ProductDTO
            {
                ProductName = "HCC Unit tests product",
                AllowReviews = true,
                ListPrice = 687,
                LongDescription = "This is test product",
                Sku = "TST100",
                StoreId = 1,
                TaxExempt = true
            };
            var createCategoryResponse = proxy.CategoriesCreate(category);
            CheckErrors(createCategoryResponse);
            var createProductResponse = proxy.ProductsCreate(product, null);
            //CheckErrors(createProductResponse);

            //Create Product and Category association
            category.Bvin = createCategoryResponse.Content.Bvin;
            product.Bvin = createProductResponse.Content.Bvin;
            var assocResponse = proxy.CategoryProductAssociationsQuickCreate(product.Bvin, category.Bvin);
            CheckErrors(assocResponse);
            Assert.IsTrue(assocResponse.Content);

            //Delete test product.
            var productDeleteResponse = proxy.ProductsDelete(product.Bvin);
            CheckErrors(productDeleteResponse);

            //Delete test category.
            var categoryDeleteResponse = proxy.CategoriesDelete(category.Bvin);
            CheckErrors(categoryDeleteResponse);
        }

        /// <summary>
        ///     This method is used to test product and category association update.
        /// </summary>
        [TestMethod]
        public void Category_TestAssociationsUpdate()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Find Category by Slug.
            var resParent = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);
            CheckErrors(resParent);

            //Create test product and category.
            var category = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category",
                ParentId = resParent.Content.Bvin
            };
            var product = new ProductDTO
            {
                ProductName = "HCC Unit tests product",
                AllowReviews = true,
                ListPrice = 687,
                LongDescription = "This is test product",
                Sku = "TST100",
                StoreId = 1,
                TaxExempt = true
            };
            var createCategoryResponse = proxy.CategoriesCreate(category);
            CheckErrors(createCategoryResponse);

            var createProductResponse = proxy.ProductsCreate(product, null);
            //CheckErrors(createProductResponse);

            //Create category and product association.
            category.Bvin = createCategoryResponse.Content.Bvin;
            product.Bvin = createProductResponse.Content.Bvin;

            var assoc = new CategoryProductAssociationDTO
            {
                ProductId = product.Bvin,
                CategoryId = category.Bvin
            };
            var assocResponse = proxy.CategoryProductAssociationsCreate(assoc);
            CheckErrors(assocResponse);

            //Update category and product association.
            assocResponse.Content.SortOrder = 100;
            var assocUpdateResponse = proxy.CategoryProductAssociationsUpdate(assocResponse.Content);
            CheckErrors(assocUpdateResponse);

            var findAssociationResponse = proxy.CategoryProductAssociationsFind(assocResponse.Content.Id);
            CheckErrors(findAssociationResponse);
            Assert.AreEqual(assocUpdateResponse.Content.SortOrder, findAssociationResponse.Content.SortOrder);

            //Unrelate the category and product association.
            var unrelateResponse = proxy.CategoryProductAssociationsUnrelate(createProductResponse.Content.Bvin,
                createCategoryResponse.Content.Bvin);
            CheckErrors(unrelateResponse);
            Assert.IsTrue(unrelateResponse.Content);

            //Delete temporary product.
            var productDeleteResponse = proxy.ProductsDelete(product.Bvin);
            CheckErrors(productDeleteResponse);

            //Delete temporary category.
            var categoryDeleteResponse = proxy.CategoriesDelete(category.Bvin);
            CheckErrors(categoryDeleteResponse);
        }

        /// <summary>
        ///     This method is used to test find categories for product end point.
        /// </summary>
        [TestMethod]
        public void Category_CategoriesFindForProduct()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();


            // Product Bvin from sample product "Blue Bracelet"
            var lstOfProducts = proxy.ProductsFindAll();

            //Get List of products for category.
            var getCategoriesResponse = proxy.CategoriesFindForProduct(lstOfProducts.Content[0].Bvin);
                // ("3421370c-18e4-4bf4-8746-bab47b3df295");
            CheckErrors(getCategoriesResponse);
        }

        /// <summary>
        ///     This method is used to test upload image for category end point.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\ProductImage.jpg", "Data")]
        public void Category_UploadImages()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Find Category by Slug.
            var findResponse = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);
            CheckErrors(findResponse);

            //Upload image for Category icon.
            var category = findResponse.Content;
            var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/ProductImage.jpg");
            var imageData = File.ReadAllBytes(imagePath);
            var fileName = Path.GetFileName(imagePath);
            var uploadResponse = proxy.CategoriesImagesIconUpload(category.Bvin, fileName, imageData);
            CheckErrors(uploadResponse);
            Assert.IsTrue(uploadResponse.Content);

            //Upload image for Category banner.
            uploadResponse = proxy.CategoriesImagesBannerUpload(category.Bvin, fileName, imageData);
            CheckErrors(uploadResponse);
            Assert.IsTrue(uploadResponse.Content);
        }

        /// <summary>
        ///     This method is used to test clear all category end point.
        /// </summary>
        [TestMethod]
        public void Category_ClearAll()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Clear all category.
            var clearAllResponse = proxy.CategoriesClearAll();
            CheckErrors(clearAllResponse);
            Assert.IsTrue(clearAllResponse.Content);
        }
    }
}