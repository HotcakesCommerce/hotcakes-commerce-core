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
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Membership;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform test against WishList end points in REST API.
    /// </summary>
    [TestClass]
    public class WishListTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test get all wish list.
        /// </summary>
        [TestMethod]
        public void WishList_TestFindAll()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test CustomerAccount as prerequisites
            var accountRespose = SampleData.CreateTestCustomerAccount(proxy);

            //Create Test ProductReview as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create Test WishList as prerequisites
            var item = new WishListItemDTO
            {
                ProductId = productRespose.Bvin,
                Quantity = 1,
                CustomerId = accountRespose.Bvin
            };

            // Create
            var wishListRespose = proxy.WishListItemsCreate(item);

            //Get all Wish Lists.
            var findResponse = proxy.WishListItemsFindAll();

            //Remove test WishList
            proxy.WishListItemsDelete(wishListRespose.Content.Id);
            CheckErrors(findResponse);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);

            //Remove Test CustomerAccount
            SampleData.RemoveTestCustomerAccount(proxy, accountRespose.Bvin);
        }

        /// <summary>
        ///     This method is used to test get WishList collection for a customer
        /// </summary>
        [TestMethod]
        public void WishList_TestFindByCustomer()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Test CustomerAccount as prerequisites
            var accountRespose = SampleData.CreateTestCustomerAccount(proxy);

            //Create Test ProductReview as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create test WishList as prerequisites
            var item = new WishListItemDTO
            {
                ProductId = productRespose.Bvin,
                Quantity = 1,
                CustomerId = accountRespose.Bvin
            };

            // Create
            var wishListRespose = proxy.WishListItemsCreate(item);

            //Get WishLists for a customer
            var findResponse = proxy.WishListItemsFindForCustomer(accountRespose.Bvin);

            //Remove Test WishList
            proxy.WishListItemsDelete(wishListRespose.Content.Id);
            CheckErrors(findResponse);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);

            //Remove Test CustomerAccount
            SampleData.RemoveTestCustomerAccount(proxy, accountRespose.Bvin);
        }

        /// <summary>
        ///     This method is used to Create, Update and Delete Wish List.
        /// </summary>
        [TestMethod]
        public void WishList_CreateUpdateDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test CustomerAccount as prerequisites
            var accountRespose = SampleData.CreateTestCustomerAccount(proxy);

            //Create Test Product as prerequisites          
            var productRespose = SampleData.CreateTestProduct(proxy);

            //Create WishList
            var item = new WishListItemDTO
            {
                ProductId = productRespose.Bvin,
                Quantity = 1,
                CustomerId = accountRespose.Bvin
            };

            // Create
            var create1Response = proxy.WishListItemsCreate(item);

            CheckErrors(create1Response);

            // Update
            item = create1Response.Content;
            item.Quantity = 10;

            var updateResponse = proxy.WishListItemsUpdate(item);

            CheckErrors(updateResponse);

            Assert.AreEqual(create1Response.Content.Quantity, updateResponse.Content.Quantity);

            var findResponse = proxy.WishListItemsFind(updateResponse.Content.Id);
            CheckErrors(findResponse);
            Assert.AreEqual(updateResponse.Content.Quantity, findResponse.Content.Quantity);

            // Delete
            var delete1Response = proxy.WishListItemsDelete(create1Response.Content.Id);

            CheckErrors(delete1Response);

            Assert.IsTrue(delete1Response.Content);

            //Remove Test Product
            SampleData.RemoveTestProduct(proxy, productRespose.Bvin);

            //Remove Test CustomerAccount
            SampleData.RemoveTestCustomerAccount(proxy, accountRespose.Bvin);
        }
    }
}