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

using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform tests against Product Types end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductTypesTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test get all Product Types.
        /// </summary>
        [TestMethod]
        public void ProductTypes_FindAll()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test ProductType as prerequisites          
            var productTypeRespose = SampleData.CreateTestProductType(proxy);
                        
            //Get All Product Types.
            var findResponse = proxy.ProductTypesFindAll();          
            CheckErrors(findResponse);

            //Remove Test ProductType
            SampleData.RemoveTestProductType(proxy, productTypeRespose.Bvin);
        }

        /// <summary>
        ///     This method is used to test Create, Find and Delete Product Type.
        /// </summary>
        [TestMethod]
        public void ProductTypes_TestCreateAndFindAndDelete()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Product Type
            var productType = new ProductTypeDTO
            {
                ProductTypeName = "UnitTest Type",
                TemplateName = "TestTemplate",
                StoreId = 1,
                IsPermanent = true
            };
            var createResponse = proxy.ProductTypesCreate(productType);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Product Type by its unique identifier
            var findResponse = proxy.ProductTypesFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.ProductTypeName, findResponse.Content.ProductTypeName);
            Assert.AreEqual(createResponse.Content.TemplateName, findResponse.Content.TemplateName);

            //Update Product Type
            createResponse.Content.IsPermanent = false;
            var updateResponse = proxy.ProductTypesUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.IsPermanent, updateResponse.Content.IsPermanent);

            //Create Product Property
            var productProperty = new ProductPropertyDTO
            {
                DefaultValue = "5",
                DisplayName = "UnitTestTypeProperty",
                PropertyName = "UnitTestTypeProperty",
                StoreId = 1,
                TypeCode = ProductPropertyTypeDTO.TextField
            };
            var propertyCreateResponse = proxy.ProductPropertiesCreate(productProperty);
            CheckErrors(propertyCreateResponse);
            Assert.IsFalse(propertyCreateResponse.Content.Id == 0);

            //Add Product Property to Product Type.
            var addPropertyResponse = proxy.ProductTypesAddProperty(createResponse.Content.Bvin,
                propertyCreateResponse.Content.Id, 1);
            Assert.IsTrue(addPropertyResponse.Content);

            //Remove Product Property from Product Type.
            var removePropertyResponse = proxy.ProductTypesRemoveProperty(createResponse.Content.Bvin,
                propertyCreateResponse.Content.Id);
            Assert.IsTrue(removePropertyResponse.Content);

            //Delete Product Type.
            var deleteResponse = proxy.ProductTypesDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }
    }
}