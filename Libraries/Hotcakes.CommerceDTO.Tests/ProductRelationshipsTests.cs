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

using System.Linq;
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

            //Find relation for product
            var response = proxy.ProductRelationshipsForProduct("a7b43865-6a89-4ebe-a43b-a00b5105ae3a");
            CheckErrors(response);

            //Find product relationship by unique identifier.
            var findResponse = proxy.ProductRelationshipsFind(response.Content.First().Id);
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to Create, Update and Unrelate the Product relationship.
        /// </summary>
        [TestMethod]
        public void ProductRelationShip_CreateUpdateUnrelate()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Product relationship
            var productRelationShip = new ProductRelationshipDTO
            {
                MarketingDescription = "Test Marketing Desc",
                ProductId = TestConstants.TestProductBvin,
                RelatedProductId = "a7b43865-6a89-4ebe-a43b-a00b5105ae3a",
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
            var unrelateResponse = proxy.ProductRelationshipsUnrelate(TestConstants.TestProductBvin,
                "a7b43865-6a89-4ebe-a43b-a00b5105ae3a");
            Assert.IsTrue(unrelateResponse.Content);

            //Quick create product relationship.
            var quickCreateResposne = proxy.ProductRelationshipsQuickCreate(TestConstants.TestProductBvin,
                "a7b43865-6a89-4ebe-a43b-a00b5105ae3a", false);
            Assert.IsTrue(quickCreateResposne.Content);

            //Unrelate the product relationship
            unrelateResponse = proxy.ProductRelationshipsUnrelate(TestConstants.TestProductBvin,
                "a7b43865-6a89-4ebe-a43b-a00b5105ae3a");
            Assert.IsTrue(unrelateResponse.Content);
        }
    }
}