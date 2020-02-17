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
    ///     This method is used to perform test against Product Volume discount end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductVolumeDiscountTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test Create, Update , Find and Delete Product volume discounts.
        /// </summary>
        [TestMethod]
        public void ProductVolumeDiscount_CreateUpdateFindDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Product Volume Discount.
            var productVolumeDiscount = new ProductVolumeDiscountDTO
            {
                Amount = 50,
                DiscountType = ProductVolumeDiscountTypeDTO.Amount,
                ProductId = TestConstants.TestProductBvin,
                Qty = 5
            };
            var createResponse = proxy.ProductVolumeDiscountsCreate(productVolumeDiscount);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Product Volume Discount
            var findResponse = proxy.ProductVolumeDiscountsFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.DiscountType, findResponse.Content.DiscountType);
            Assert.AreEqual(createResponse.Content.ProductId, findResponse.Content.ProductId);

            //Update Product Volume Discount
            createResponse.Content.Amount = 75;
            var updateResponse = proxy.ProductVolumeDiscountsUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.Amount, updateResponse.Content.Amount);

            //Delete Product Volume Discount.
            var deleteResponse = proxy.ProductVolumeDiscountsDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }
    }
}