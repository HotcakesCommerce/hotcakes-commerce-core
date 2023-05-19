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

using Hotcakes.CommerceDTO.v1.Client;
using Hotcakes.CommerceDTO.v1.Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform the test against Price group end points in REST API.
    /// </summary>
    [TestClass]
    public class PriceGroupTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test Find all price group end point.
        /// </summary>
        [TestMethod]
        public void PriceGroups_FindAll()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test PriceGroup as prerequisites
            var priceGroup = SampleData.CreateTestPriceGroup(proxy);

            //Get all price groups
            var findResponse = proxy.PriceGroupsFindAll();

            //Remove Test PriceGroup
            SampleData.RemoveTestPriceGroup(proxy, priceGroup.Bvin);
            
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to test create, find and Delete end point.
        /// </summary>
        [TestMethod]
        public void PriceGroups_TestCreateAndFindAndDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Price group.
            var priceGroup = new PriceGroupDTO
            {
                StoreId = 1,
                Name = "Test PriceGroup",
                AdjustmentAmount = 5,
                PricingType = PricingTypesDTO.AmountAboveCost
            };
            var createResponse = proxy.PriceGroupsCreate(priceGroup);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Price group by its unique identifer.
            var findResponse = proxy.PriceGroupsFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.Name, findResponse.Content.Name);
            Assert.AreEqual(createResponse.Content.AdjustmentAmount, findResponse.Content.AdjustmentAmount);

            //Update Price group.
            priceGroup.AdjustmentAmount = 50;
            var updateResponse = proxy.PriceGroupsUpdate(priceGroup);
            CheckErrors(updateResponse);
            Assert.AreEqual(priceGroup.AdjustmentAmount, updateResponse.Content.AdjustmentAmount);

            //Delete Price group.
            var deleteResponse = proxy.PriceGroupsDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }

        
    }
}