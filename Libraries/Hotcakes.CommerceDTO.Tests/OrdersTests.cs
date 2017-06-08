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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Orders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class used to perform different tests against the REST API methods related to Orders.
    /// </summary>
    [TestClass]
    public class OrdersTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test create and delete end points.
        /// </summary>
        [TestMethod]
        public void Order_CreateAndDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Order
            var order = new OrderDTO
            {
                StoreId = 1,
                Items = new List<LineItemDTO>
                {
                    new LineItemDTO
                    {
                        StoreId = 1,
                        ProductId = "fcb5f832-0f72-4722-bfd9-ec099051fc00"
                    },
                    new LineItemDTO
                    {
                        StoreId = 1,
                        ProductId = "d6895ee4-8118-4e51-905b-fc8edad5d711"
                    }
                }
            };
            var createResponse = proxy.OrdersCreate(order);
            CheckErrors(createResponse);

            //Delete Order
            var orderBvin = createResponse.Content.Bvin;
            var deleteResponse = proxy.OrdersDelete(orderBvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }

        /// <summary>
        ///     This method is used to test find order by unique identifier end point.
        /// </summary>
        [TestMethod]
        public void Order_TestFind()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Find Order by unique identifier
            var findResponse = proxy.OrdersFind(TestConstants.TestOrder1Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(findResponse.Content.OrderNumber, TestConstants.TestOrder1Number);
        }

        /// <summary>
        ///     This method is used to test find all orders end point.
        /// </summary>
        [TestMethod]
        public void Order_TestFindAll()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find all Orders 
            var findResponse = proxy.OrdersFindAll();
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to test update order end point.
        /// </summary>
        [TestMethod]
        public void Order_TestUpdate()
        {
            //Create API proxy
            var proxy = CreateApiProxy();

            //Find order by Unique identifier
            var findResponse = proxy.OrdersFind("1e16239a-555c-435e-b8b4-1594d30bc17a");
            CheckErrors(findResponse);

            //Update Order
            var order = findResponse.Content;
            var oldInstructions = order.Instructions;
            order.Instructions = "New test notes";
            var updateResponse = proxy.OrdersUpdate(order);
            CheckErrors(updateResponse);
            order = updateResponse.Content;
            Assert.AreEqual(order.Instructions, "New test notes");

            //Update Order
            order.Instructions = oldInstructions;
            updateResponse = proxy.OrdersUpdate(order);
            CheckErrors(updateResponse);

            order = updateResponse.Content;
            Assert.AreEqual(order.Instructions, oldInstructions);
        }

        /// <summary>
        ///     This method is used to find transactions for order end point.
        /// </summary>
        [TestMethod]
        public void Order_TestFindTransactions()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find transactions by Order unique identifier
            var findResponse = proxy.OrderTransactionsFindForOrder("1e16239a-555c-435e-b8b4-1594d30bc17a");
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to find Transaction by its unique identifier.
        /// </summary>
        [TestMethod]
        public void Order_TestFindTransaction()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Find transaction by Transaction unique identifier.
            var findResponse = proxy.OrderTransactionsFind(new Guid("3B843756-A4EA-4787-A4FC-02A5B4AA65DC"));
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to Create, Update and Delete Transaction.
        /// </summary>
        [TestMethod]
        public void Order_CreateUpdateDeleteTransaction()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Order.
            var order = new OrderDTO
            {
                StoreId = 1,
                Items = new List<LineItemDTO>
                {
                    new LineItemDTO
                    {
                        StoreId = 1,
                        ProductId = "fcb5f832-0f72-4722-bfd9-ec099051fc00"
                    },
                    new LineItemDTO
                    {
                        StoreId = 1,
                        ProductId = "d6895ee4-8118-4e51-905b-fc8edad5d711"
                    }
                }
            };
            var createResponse = proxy.OrdersCreate(order);
            CheckErrors(createResponse);

            //Create Order Transaction
            var orderTransaction = new OrderTransactionDTO
            {
                Action = OrderTransactionActionDTO.CashReceived,
                Amount = 50,
                Messages = "test transaction",
                OrderId = createResponse.Content.Bvin,
                OrderNumber = createResponse.Content.OrderNumber,
                StoreId = 1,
                TimeStampUtc = DateTime.UtcNow
            };
            var createTransactionResponse = proxy.OrderTransactionsCreate(orderTransaction);
            CheckErrors(createTransactionResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createTransactionResponse.Content.Id.ToString()));

            //Update Transaction
            createTransactionResponse.Content.Messages = createTransactionResponse.Content.Messages + "updated";
            var updateTransactionResponse = proxy.OrderTransactionsUpdate(createTransactionResponse.Content);
            CheckErrors(updateTransactionResponse);
            Assert.AreEqual(createTransactionResponse.Content.Messages, updateTransactionResponse.Content.Messages);

            //Delete Transaction by Transaction unique identifier. 
            var deleteTransactionResponse = proxy.OrderTransactionsDelete(updateTransactionResponse.Content.Id);
            CheckErrors(deleteTransactionResponse);
            Assert.IsTrue(deleteTransactionResponse.Content);

            //Delete Order
            var orderBvin = createResponse.Content.Bvin;
            var deleteResponse = proxy.OrdersDelete(orderBvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }

        /// <summary>
        ///     This method is used to test Zero order total scenario related to bug 13354
        /// </summary>
        [TestMethod]
        public void Order_Bug13354_OrderTotalsZero()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find order by Unique Identifier
            var resOrder = proxy.OrdersFind(TestConstants.TestOrder1Bvin);
            CheckErrors(resOrder);
            var dto = resOrder.Content;
            Assert.AreEqual(dto.OrderNumber, TestConstants.TestOrder1Number);
            Assert.AreEqual(dto.TotalGrand, TestConstants.TestOrder1TotalGrand);

            //Find Order Properties
            var prop = dto.CustomProperties.FirstOrDefault(p => p.DeveloperId == "test" && p.Key == "TestProp");
            if (prop == null)
            {
                prop = new CustomPropertyDTO {DeveloperId = "test", Key = "TestProp"};
                dto.CustomProperties.Add(prop);
            }
            prop.Value = "Some Value";

            //Update Order
            var resOrder2 = proxy.OrdersUpdate(dto);
            CheckErrors(resOrder2);
            var dto2 = resOrder2.Content;
            Assert.AreEqual(dto2.OrderNumber, TestConstants.TestOrder1Number);
            Assert.AreEqual(dto2.TotalGrand, TestConstants.TestOrder1TotalGrand);
        }
    }
}