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
using Hotcakes.CommerceDTO.v1.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     The <c>ProductInventoryTests</c> type
    ///     gives methods to do test for the different end points related to
    ///     the  <c> ProductInventory </c>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class got inherited from the <c>ApiTestBase</c> class
    ///         which provide base method to connect to the API service and provide the end point to call the different
    ///         methods created with API.
    ///     </para>
    ///     <para>
    ///         This class needs to be updated each time the new end points got created for the
    ///         Product Inventory.
    ///     </para>
    ///     <para>
    ///         This class Uses some default product Bvin and other data from the <c>TestConstants.cs</c> class
    ///         so the database needs to be populated with all that data before hand using this test methods.
    ///     </para>
    /// </remarks>
    [TestClass]
    public class ProductInventoryTests : ApiTestBase
    {
        /// <summary>
        ///     This method gets the list of all the Product Inventory
        ///     and do assert based on specific condition
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method used the API Proxy method from base class <c>ApiTestBase</c>
        ///         and then call the API method to get all Product Inventory.
        ///         The values returned from the API call is the list of <see cref="ProductInventoryDTO" />
        ///     </para>
        ///     <para>
        ///         This method will Assert the result for Count is greather than <c>0</c> for the returned result.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     None
        /// </returns>
        [TestMethod]
        public void ProductInventory_TestFindAll()
        {
            var proxy = CreateApiProxy();

            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Inventory as prerequisites
            var productInventoryDTO = new ProductInventoryDTO
            {
                LowStockPoint = 5,
                OutOfStockPoint = 2,
                QuantityOnHand = 50,
                QuantityReserved = 4,
                ProductBvin = productRespose.Bvin,

            };
            var productInventoryRespose = proxy.ProductInventoryCreate(productInventoryDTO);

            var findResponse = proxy.ProductInventoryFindAll();

            Assert.IsTrue(findResponse.Content.Count > 0);

            //Delete Product Inventory
            proxy.ProductInventoryDelete(productInventoryRespose.Content.Bvin);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);

            CheckErrors(findResponse);
        }


        /// <summary>
        ///     This method get the list of all the Product Inventory for specific product only
        ///     and assert based on the returned result.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method used the API Proxy method from base class <c>ApiTestBase</c>
        ///         and then call the API method to get  Product Inventory for specific product. Product Bvin got read from the
        ///         <see cref="TestConstants.TestProductBvin" /> in order to get the results.
        ///         The values returned from the API call is the list of <see cref="ProductInventoryDTO" />
        ///     </para>
        ///     <para>
        ///         This method will Assert the result for Count is greather than <c>0</c> for the returned result.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     None
        /// </returns>
        [TestMethod]
        public void ProductInventory_TestFindForProduct()
        {
            var proxy = CreateApiProxy();

            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Inventory as prerequisites
            var productInventory = new ProductInventoryDTO
            {
                LowStockPoint = 5,
                OutOfStockPoint = 2,
                QuantityOnHand = 50,
                QuantityReserved = 4,
                ProductBvin = productRespose.Bvin
            };
            var productInventoryRespose = proxy.ProductInventoryCreate(productInventory);

            var findResponse = proxy.ProductInventoryFindForProduct(productRespose.Bvin);

            Assert.IsTrue(findResponse.Content.Count > 0);

            //Delete Product Inventory
            proxy.ProductInventoryDelete(productInventoryRespose.Content.Bvin);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method used to test the api method
        ///     for create,find and delete operation with
        ///     API methods
        /// </summary>
        [TestMethod]
        public void ProductInventory_TestFindProductInventory()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();


            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Inventory
            var productInventory = new ProductInventoryDTO
            {
                LowStockPoint = 5,
                OutOfStockPoint = 2,
                QuantityOnHand = 50,
                QuantityReserved = 4,
                ProductBvin = productRespose.Bvin
            };
            var createResponse = proxy.ProductInventoryCreate(productInventory);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Product Inventory
            var findResponse = proxy.ProductInventoryFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.LowStockPoint, findResponse.Content.LowStockPoint);
            Assert.AreEqual(createResponse.Content.OutOfStockPoint, findResponse.Content.OutOfStockPoint);

            //Delete Product Inventory
            var deleteResponse = proxy.ProductInventoryDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);

        }


        /// <summary>
        ///     This method combines both method toghether get list of all Inventory and use the first inventory record's Product
        ///     Bvin to
        ///     get the list of all Inventories for that specific product.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method used the API Proxy method from base class <c>ApiTestBase</c>
        ///         and then call the API method to get all Product Inventory.
        ///         The values returned from the API call is the list of <see cref="ProductInventoryDTO" />
        ///     </para>
        ///     <para>
        ///         After values returned for the all product inventory we get the first item <see cref="ProductInventoryDTO" />
        ///         and use its <c>ProductBvin</c>
        ///         property to call another API method <c>ProductInventoryFindForProduct</c> from <see cref="Api" />.
        ///     </para>
        ///     <para>
        ///         This method will Assert the result for count is greather than <c>0</c> for the returned result.
        ///         It also assert the result to compare the values returned from the <c>ProductInventoryFindForProduct</c>
        ///         and the first record of the results returned by <c>ProductInventoryFindAll</c>.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     None
        /// </returns>
        [TestMethod]
        public void ProductInventory_TestFindAllAndForProduct()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();


            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Product Inventory as prerequisites 
            var productInventoryDTO = new ProductInventoryDTO
            {
                LowStockPoint = 5,
                OutOfStockPoint = 2,
                QuantityOnHand = 50,
                QuantityReserved = 4,
                ProductBvin = productRespose.Bvin
            };
            var productInventoryRespose = proxy.ProductInventoryCreate(productInventoryDTO);

            //Find all inventory
            var findAllResponse = proxy.ProductInventoryFindAll();
            CheckErrors(findAllResponse);
            Assert.IsTrue(findAllResponse.Content.Count > 0);

            if (findAllResponse.Content.Count > 0)
                // Get Inventory for specific product if we have some data in database
            {
                var findForProductResponse = proxy.ProductInventoryFindForProduct(findAllResponse.Content[0].ProductBvin);

                Assert.IsTrue(findForProductResponse.Content.Count > 0);

                var allInventoryOfProduct =
                    findAllResponse.Content.Where(p => p.ProductBvin == findAllResponse.Content[0].ProductBvin).ToList();

                var productInventory =
                    findForProductResponse.Content
                        .FirstOrDefault(p => p.Bvin == findAllResponse.Content[0].Bvin);

                Assert.AreEqual(productInventory.Bvin, findAllResponse.Content[0].Bvin);

                Assert.AreEqual(productInventory.ProductBvin, findAllResponse.Content[0].ProductBvin);

                Assert.AreEqual(allInventoryOfProduct.Count, findForProductResponse.Content.Count);

                CheckErrors(findForProductResponse);

                //Remove Test ProductInventory
                proxy.ProductInventoryDelete(productInventoryRespose.Content.Bvin);

                //Remove Test Product
                SampleData.RemoveTestProduct(proxy, productRespose.Bvin);
            }
        }

        /// <summary>
        ///     This test verifies that IsAvailableFoSale product property is updated correctly after inventory updates.
        /// </summary>
        [TestMethod]
        public void ProductInventory_TestIsAvailableFoSaleUpdate()
        {
            var proxy = CreateApiProxy();

            //Create Test Product as prerequisites
            var productDTO = new ProductDTO
            {
                ProductName = "Unit tests product",
                AllowReviews = true,
                ListPrice = 687,
                LongDescription = "This is test product",
                Sku = "TST106",
                StoreId = 1,
                TaxExempt = true,
                IsAvailableForSale = true,
                InventoryMode = ProductInventoryModeDTO.AlwayInStock
            };

            //Create
            var productRespose = proxy.ProductsCreate(productDTO, null);


            //Create Product Inventory as prerequisites
            var productInventoryDTO = new ProductInventoryDTO
            {
                LowStockPoint = 5,
                OutOfStockPoint = 2,
                QuantityOnHand = 50,
                QuantityReserved = 4,
                ProductBvin = productRespose.Content.Bvin,
                
            };

            var productInventoryRespose = proxy.ProductInventoryCreate(productInventoryDTO);


            var productFind1Response = proxy.ProductsFind(productRespose.Content.Bvin);
            CheckErrors(productFind1Response);
            var product = productFind1Response.Content;


            product.InventoryMode = ProductInventoryModeDTO.AlwayInStock;
            var productUpdate1Response = proxy.ProductsUpdate(product);
            CheckErrors(productUpdate1Response);
            Assert.AreEqual(productUpdate1Response.Content.IsAvailableForSale, true);


            product.InventoryMode = ProductInventoryModeDTO.WhenOutOfStockHide;
            var productUpdate2Response = proxy.ProductsUpdate(product);
            CheckErrors(productUpdate2Response);

            var productInventoryFindResponse = proxy.ProductInventoryFindForProduct(productRespose.Content.Bvin);
            CheckErrors(productInventoryFindResponse);
            var productInventories = productInventoryFindResponse.Content;

            foreach (var productInventory in productInventories)
            {
                productInventory.QuantityOnHand = 0;
                var productInventoryResponse = proxy.ProductInventoryUpdate(productInventory);
                CheckErrors(productInventoryResponse);
            }
            var productFind2Response = proxy.ProductsFind(productRespose.Content.Bvin);
            CheckErrors(productFind2Response);
            Assert.AreEqual(productFind2Response.Content.IsAvailableForSale, false);

            //Delete Product Inventory
            proxy.ProductInventoryDelete(productInventoryRespose.Content.Bvin);

            //Remove Test Product
            proxy.ProductsDelete(productRespose.Content.Bvin);


        }

        

    }
}