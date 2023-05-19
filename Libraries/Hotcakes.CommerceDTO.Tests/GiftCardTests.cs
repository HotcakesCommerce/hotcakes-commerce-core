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
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class used to create different test cases for the REST API methods related to
    ///     Gift Cards.
    /// </summary>
    [TestClass]
    public class GiftCardTests : ApiTestBase
    {
        /// <summary>
        ///     Get all Gift Cards info.
        /// </summary>
        [TestMethod]
        public void GiftCard_TestFindAll()
        {
            var proxy = CreateApiProxy();

            //Create Test GiftCard type as prerequisites
            var giftCard = SampleData.CreateTestGiftCard(proxy);

            var findResponse = proxy.GiftCardFindAll();

            //Remove Test GiftCard
            SampleData.RemoveTestGiftCard(proxy, giftCard.GiftCardId);

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     Get Gift Card info with paging.
        /// </summary>
        [TestMethod]
        public void GiftCard_TestFindAllPaged()
        {
            var proxy = CreateApiProxy();

            //Create Test GiftCard type as prerequisites
            var giftCard = SampleData.CreateTestGiftCard(proxy);

            var findResponse = proxy.GiftCardFindAllByPage(1, int.MaxValue);

            //Remove Test GiftCard
            SampleData.RemoveTestGiftCard(proxy, giftCard.GiftCardId);

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     Get count of all available Gift Cards.
        /// </summary>
        [TestMethod]
        public void GiftCard_TestCountOnly()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.GiftCardCountOfAll();

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     Create, Update,Delete Gift Card.
        /// </summary>
        [TestMethod]
        public void GiftCard_CreateUpdateDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test GiftCard type as prerequisites
            var card = new GiftCardDTO
            {
                CardNumber = "GIFT0000-1111-22222",
                IssueDateUtc = DateTime.UtcNow,
                ExpirationDateUtc = DateTime.UtcNow.AddMonths(1),
                Amount = 100
            };

            //Create
            var create1Response = proxy.GiftCardCreate(card);

            CheckErrors(create1Response);

            //Update
            card = create1Response.Content;
            card.UsedAmount = 50;

            var updateResponse = proxy.GiftCardUpdate(card);

            CheckErrors(updateResponse);

            Assert.AreEqual(create1Response.Content.UsedAmount, updateResponse.Content.UsedAmount);

            //Find
            var findResponse = proxy.GiftCardFind(create1Response.Content.GiftCardId);

            CheckErrors(findResponse);

            Assert.AreEqual(updateResponse.Content.UsedAmount, findResponse.Content.UsedAmount);

            //Delete
            var delete1Response = proxy.GiftCardDelete(create1Response.Content.GiftCardId);

            CheckErrors(delete1Response);

            Assert.IsTrue(delete1Response.Content);

        }
    }
}