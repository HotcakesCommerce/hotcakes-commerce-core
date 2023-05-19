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

using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class used to test against Product properties end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductPropertiesTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test the Find all product properties end point.
        /// </summary>
        [TestMethod]
        public void ProductProperty_FindPropertyAll()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Product Property as prerequisites
            var productProperty = new ProductPropertyDTO
            {
                DisplayName = "TestCase Property",
                DefaultValue = "test",
                PropertyName = "TestCaseProperty",
                TypeCode = ProductPropertyTypeDTO.TextField,
                StoreId = 1
            };
            var createproductPropertyResponse = proxy.ProductPropertiesCreate(productProperty);

            //Find all Product properties
            var response = proxy.ProductPropertiesFindAll();

            //Remove Test Product Properties
            proxy.ProductPropertiesDelete(createproductPropertyResponse.Content.Id);

            CheckErrors(response);
        }

        /// <summary>
        ///     This method is used to test find all properties by Product.
        /// </summary>
        [TestMethod]
        public void ProductProperty_FindPropertyByProduct()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Property as prerequisites
            var productProperty = new ProductPropertyDTO
            {
                DisplayName = "TestCase Property",
                DefaultValue = "test",
                PropertyName = "TestCaseProperty",
                TypeCode = ProductPropertyTypeDTO.TextField,
                StoreId = 1
            };
            var createproductPropertyResponse = proxy.ProductPropertiesCreate(productProperty);

            //Set Product properties value
            var setValueResponse = proxy.ProductPropertiesSetValueForProduct(createproductPropertyResponse.Content.Id,
               productRespose.Bvin, "settestvalue", 0);

            //Find all product properties for specific product
            var response = proxy.ProductPropertiesForProduct(productRespose.Bvin);

            if (response.Content == null)
            {
                Assert.IsTrue(response.Errors[0].Description == "No product properties found.");
            }
            else
            {
                CheckErrors(response);
            }

            //Remove Test Product Properties
            proxy.ProductPropertiesDelete(createproductPropertyResponse.Content.Id);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);
        }

        /// <summary>
        ///     This method is used to test Create, Update and Delete Product properties.
        /// </summary>
        [TestMethod]
        public void ProductProperty_CreateUpdateDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Property
            var productProperty = new ProductPropertyDTO
            {
                DisplayName = "TestCase Property",
                DefaultValue = "test",
                PropertyName = "TestCaseProperty",
                TypeCode = ProductPropertyTypeDTO.TextField,
                StoreId = 1
            };
            var createResponse = proxy.ProductPropertiesCreate(productProperty);
            CheckErrors(createResponse);
            Assert.IsFalse(createResponse.Content.Id == 0);

            //Find Product Property
            var findResponse = proxy.ProductPropertiesFind(createResponse.Content.Id);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.DisplayName, findResponse.Content.DisplayName);
            Assert.AreEqual(createResponse.Content.PropertyName, findResponse.Content.PropertyName);

            //Update product property
            createResponse.Content.DefaultValue = "testupdate";
            var updateResponse = proxy.ProductPropertiesUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.DefaultValue, updateResponse.Content.DefaultValue);

            //Set Product properties value
            var setValueResponse = proxy.ProductPropertiesSetValueForProduct(updateResponse.Content.Id,
               productRespose.Bvin, "settestvalue", 0);
            CheckErrors(setValueResponse);
            Assert.IsTrue(setValueResponse.Content);

            //Delete Product properties
            var deleteResponse = proxy.ProductPropertiesDelete(createResponse.Content.Id);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);
        }
    }
}