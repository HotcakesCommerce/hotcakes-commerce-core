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

using System.Linq;
using System.Threading;
using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class used to test against Product relationship end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductRelationshipsTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to get all product relationship by Product unique identifier.
        /// </summary>
        [TestMethod]
        public void ProductRelationship_FindRelationshipByProduct()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Test relationship as prerequisites          
            var relationshipRespose = SampleData.CreateTestRelationship(proxy);

            //Find relation for product
            var response = proxy.ProductRelationshipsForProduct(relationshipRespose.ProductId);
            CheckErrors(response);

            //Find product relationship by unique identifier.
            var findResponse = proxy.ProductRelationshipsFind(response.Content.First().Id);
            CheckErrors(findResponse);

            //Remove test relationship
            SampleData.RemoveTestRelationship(proxy, relationshipRespose.Id);
        }

        /// <summary>
        ///     This method is used to Create, Update and Unrelate the Product relationship.
        /// </summary>
        [TestMethod]
        public void ProductRelationShip_CreateUpdateUnrelate()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test product 1 as prerequisites          
            var product1Respose = SampleData.CreateTestProduct(proxy);

            Thread.Sleep(1000);

            //Create Test product 2 as prerequisites          
            var product2Respose = SampleData.CreateTestProduct(proxy);

            //Create Product relationship
            var productRelationShip = new ProductRelationshipDTO
            {
                MarketingDescription = "Test Marketing Desc",
                ProductId = product1Respose.Bvin,
                RelatedProductId = product2Respose.Bvin,
                StoreId = 1
            };
            var createResponse = proxy.ProductRelationshipsCreate(productRelationShip);
            CheckErrors(createResponse);

            //Update Product relationship
            createResponse.Content.MarketingDescription = createResponse.Content.MarketingDescription + "updated";
            var updateResponse = proxy.ProductRelationshipsUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.MarketingDescription, updateResponse.Content.MarketingDescription);

            //Unrelate the product relationship.
            var unrelateResponse = proxy.ProductRelationshipsUnrelate(product1Respose.Bvin,
                product2Respose.Bvin);
            Assert.IsTrue(unrelateResponse.Content);

            //Quick create product relationship.
            var quickCreateResposne = proxy.ProductRelationshipsQuickCreate(product1Respose.Bvin,
                product2Respose.Bvin, false);
            Assert.IsTrue(quickCreateResposne.Content);

            //Unrelate the product relationship
            unrelateResponse = proxy.ProductRelationshipsUnrelate(product1Respose.Bvin,
                product2Respose.Bvin);
            Assert.IsTrue(unrelateResponse.Content);

            //Remove Test Product 2
            SampleData.RemoveTestProduct(proxy, product2Respose.Bvin);

            //Remove Test Product 1
            SampleData.RemoveTestProduct(proxy, product1Respose.Bvin);
        }
    }
}