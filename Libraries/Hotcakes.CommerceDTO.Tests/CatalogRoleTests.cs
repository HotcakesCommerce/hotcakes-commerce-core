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
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform various test against CatalogRole end points in REST API.
    /// </summary>
    [TestClass]
    public class CatalogRoleTests : ApiTestBase
    {
        /// <summary>
        ///     This method perform Create, Find ,Delete operation for CatalogRole. Role type for this function is ProductRole.
        /// </summary>
        [TestMethod]
        public void CatalogRole_TestCreateAndFindAndDelete()
        {
            //Create proxy
            var proxy = CreateApiProxy();

            //Create Test product as prerequisites
            var prodDto = CreateTestProduct(proxy);

            //Create CatalogRole
            var dto = new CatalogRoleDTO
            {
                ReferenceId = new Guid(prodDto.Bvin),
                RoleName = "RoleA",
                RoleType = CatalogRoleTypeDTO.ProductRole
            };
            var res = proxy.CatalogRoleCreate(dto);
            CheckErrors(res);
            var roleDto = res.Content;
            Assert.IsNotNull(roleDto);

            //Find Catalog Role
            var actualDto = proxy.CatalogRoleFindByProduct(roleDto.ReferenceId.ToString())
                .Content
                .FirstOrDefault();
            Assert.AreEqual(roleDto.CatalogRoleId, actualDto.CatalogRoleId);
            Assert.AreEqual(roleDto.ReferenceId, actualDto.ReferenceId);
            Assert.AreEqual(roleDto.RoleName, actualDto.RoleName);
            Assert.AreEqual(roleDto.RoleType, actualDto.RoleType);

            //Delete Catalog Role
            var res2 = proxy.CatalogRoleDelete(roleDto.CatalogRoleId);
            CheckErrors(res2);
            Assert.IsTrue(res2.Content);

            //Remove test product
            RemoveTestProduct(proxy, prodDto.Bvin);
        }

        /// <summary>
        ///     This method perform Create, Find ,Delete operation for CatalogRole. Role type for this function is CategoryRole.
        /// </summary>
        [TestMethod]
        public void CatalogRole_TestCreateAndFindAndDeleteByCategory()
        {
            //Create proxy
            var proxy = CreateApiProxy();

            //Create test category as prerequisites
            var category = CreateTestCategory();

            //Create CatalogRole
            var dto = new CatalogRoleDTO
            {
                ReferenceId = new Guid(category.Bvin),
                RoleName = "RoleCat",
                RoleType = CatalogRoleTypeDTO.CategoryRole
            };
            var res = proxy.CatalogRoleCreate(dto);
            CheckErrors(res);
            var roleDto = res.Content;
            Assert.IsNotNull(roleDto);


            //Find CatalogRole
            var actualDto = proxy.CatalogRoleFindByCategory(roleDto.ReferenceId.ToString())
                .Content
                .FirstOrDefault();
            Assert.AreEqual(roleDto.CatalogRoleId, actualDto.CatalogRoleId);
            Assert.AreEqual(roleDto.ReferenceId, actualDto.ReferenceId);
            Assert.AreEqual(roleDto.RoleName, actualDto.RoleName);
            Assert.AreEqual(roleDto.RoleType, actualDto.RoleType);

            //Delete CatalogRole
            var res2 = proxy.CatalogRoleDelete(roleDto.CatalogRoleId);
            CheckErrors(res2);
            Assert.IsTrue(res2.Content);

            //Remove test category
            RemoveTestCategory(proxy, category.Bvin);
        }


        /// <summary>
        ///     This method perform Create, Find ,Delete operation for CatalogRole. Role type for this function is ProductType.
        /// </summary>
        [TestMethod]
        public void CatalogRole_TestCreateAndFindAndDeleteByProductType()
        {
            //Create Proxy
            var proxy = CreateApiProxy();

            //Create test product type as prerequisites
            var productType = CreateTestProductType();

            //Create CatalogRole
            var dto = new CatalogRoleDTO
            {
                ReferenceId = new Guid(productType.Bvin),
                RoleName = "RoleProductType",
                RoleType = CatalogRoleTypeDTO.ProductTypeRole
            };
            var res = proxy.CatalogRoleCreate(dto);
            CheckErrors(res);
            var roleDto = res.Content;
            Assert.IsNotNull(roleDto);

            //Find CatalogRole
            var actualDto = proxy.CatalogRoleFindByProductType(roleDto.ReferenceId.ToString())
                .Content
                .FirstOrDefault();
            Assert.AreEqual(roleDto.CatalogRoleId, actualDto.CatalogRoleId);
            Assert.AreEqual(roleDto.ReferenceId, actualDto.ReferenceId);
            Assert.AreEqual(roleDto.RoleName, actualDto.RoleName);
            Assert.AreEqual(roleDto.RoleType, actualDto.RoleType);

            //Delete CatalogRole
            var res2 = proxy.CatalogRoleDelete(roleDto.CatalogRoleId);
            CheckErrors(res2);
            Assert.IsTrue(res2.Content);

            //Remove test product
            RemoveTestProductType(proxy, productType.Bvin);
        }

        /// <summary>
        ///     This method create test category, to be used during CatalogRole creation.
        /// </summary>
        /// <returns></returns>
        public CategoryDTO CreateTestCategory()
        {
            //Create proxy
            var proxy = CreateApiProxy();

            //Create category
            var resParent = proxy.CategoriesFindBySlug(TestConstants.TestCategorySlug);
            CheckErrors(resParent);
            var dto = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category",
                ParentId = resParent.Content.Bvin
            };
            var resC = proxy.CategoriesCreate(dto);

            return resC.Content;
        }

        /// <summary>
        ///     This method create test product, to be used during CatalogRole creation.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        private ProductDTO CreateTestProduct(Api proxy)
        {
            var product = new ProductDTO
            {
                ProductName = "Unit tests product",
                AllowReviews = true,
                ListPrice = 687,
                LongDescription = "This is test product",
                Sku = "TST100",
                StoreId = 1,
                TaxExempt = true
            };

            var response = proxy.ProductsCreate(product, null);
            return response.Content;
        }

        /// <summary>
        ///     This method create test product type, to be used during CatalogRole creation.
        /// </summary>
        /// <returns></returns>
        private ProductTypeDTO CreateTestProductType()
        {
            var proxy = CreateApiProxy();

            var productType = new ProductTypeDTO
            {
                ProductTypeName = "UnitTest Type",
                TemplateName = "TestTemplate",
                StoreId = 1,
                IsPermanent = true
            };

            var createResponse = proxy.ProductTypesCreate(productType);

            return createResponse.Content;
        }


        /// <summary>
        ///     This method remove test product.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="bvin">Product unique identifier.</param>
        private void RemoveTestProduct(Api proxy, string bvin)
        {
            var response = proxy.ProductsDelete(bvin);
        }

        /// <summary>
        ///     This moethod remove test category.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="bvin">Category unique identifier.</param>
        private void RemoveTestCategory(Api proxy, string bvin)
        {
            var response = proxy.CategoriesDelete(bvin);
        }

        /// <summary>
        ///     This method remove test product type.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="bvin">Product type unique identifier.</param>
        private void RemoveTestProductType(Api proxy, string bvin)
        {
            var response = proxy.ProductTypesDelete(bvin);
        }
    }
}