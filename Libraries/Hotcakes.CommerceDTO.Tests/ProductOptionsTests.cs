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

using System.Collections.Generic;
using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform test against Product Options end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductOptionsTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test Find all Product Option end point.
        /// </summary>
        [TestMethod]
        public void ProductOption_FindAll()
        {
            var proxy = CreateApiProxy();

            var response = proxy.ProductOptionsFindAll();

            CheckErrors(response);

            var responseForProduct = proxy.ProductOptionsFindAllByProductId(TestConstants.TestProductBvin);

            CheckErrors(response);
        }

        /// <summary>
        ///     This method is used to test Create and Delete end point.
        /// </summary>
        [TestMethod]
        public void ProductOption_CreateAndDelete()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Product Option
            var option = new OptionDTO
            {
                StoreId = 1,
                Name = "Html Option",
                OptionType = OptionTypesDTO.Html,
                Items = new List<OptionItemDTO>()
            };
            var createResponse = proxy.ProductOptionsCreate(option);
            CheckErrors(createResponse);

            //Find Product Option
            var findResponse = proxy.ProductOptionsFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.Name, findResponse.Content.Name);
            Assert.AreEqual(createResponse.Content.StoreId, findResponse.Content.StoreId);

            //Update Product Option
            createResponse.Content.Name = option.Name + "updated";
            var updateResponse = proxy.ProductOptionsUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.Name, updateResponse.Content.Name);
            
            //Delete Product Option
            var deleteResponse = proxy.ProductOptionsDelete(createResponse.Content.Bvin);
            Assert.IsTrue(deleteResponse.Content);
        }

        //This method is used to test Create and Assign to product end point.
        [TestMethod]
        public void ProductOption_CreateAndAssignToProduct()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Production option
            var option = new OptionDTO
            {
                StoreId = 1,
                Name = "TEST Option",
                OptionType = OptionTypesDTO.DropDownList,
                Items = new List<OptionItemDTO>()
            };
            var createResponse = proxy.ProductOptionsCreate(option);
            CheckErrors(createResponse);           

            //Assign product option to test product
            var assingtoProductResponse = proxy.ProductOptionsAssignToProduct(createResponse.Content.Bvin,
                TestConstants.TestProductBvin, false);
            Assert.IsTrue(assingtoProductResponse.Content);            

            //Generate product variants for options
            var generateVariantsResponse = proxy.ProductOptionsGenerateAllVariants(TestConstants.TestProductBvin);
            Assert.IsTrue(generateVariantsResponse.Content);            
        }

        //This method is used to test Assign and Unassign to product end point.
        [TestMethod]
        public void ProductOption_AssignAndUnassignToProduct()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Production option
            var option = new OptionDTO
            {
                StoreId = 1,
                Name = "TEST Unassign Option",
                OptionType = OptionTypesDTO.DropDownList,
                Items = new List<OptionItemDTO>()
            };
            var createResponse = proxy.ProductOptionsCreate(option);
            CheckErrors(createResponse);

            //Assign product option to test product
            var assingtoProductResponse = proxy.ProductOptionsAssignToProduct(createResponse.Content.Bvin, TestConstants.TestProductBvin, false);
            Assert.IsTrue(assingtoProductResponse.Content);            

            //Generate product variants for options
            var unassignFromProductResponse = proxy.ProductOptionsUnassignFromProduct(createResponse.Content.Bvin, TestConstants.TestProductBvin);
            Assert.IsTrue(unassignFromProductResponse.Content);
        }
    }
}
