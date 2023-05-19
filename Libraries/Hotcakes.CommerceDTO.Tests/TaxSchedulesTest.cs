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

using Hotcakes.CommerceDTO.v1.Taxes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This method is used to perform test against Tax Schedule end points in REST API.
    /// </summary>
    [TestClass]
    public class TaxSchedulesTest : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test get all Tax Schedules.
        /// </summary>
        [TestMethod]
        public void TaxSchedules_FindAll()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test TaxSchedule as prerequisites          
            var createTaxScheduleResponse = SampleData.CreateTestTaxSchedules(proxy);

            //Get all Tax Schedules.
            var findResponse = proxy.TaxSchedulesFindAll();
            CheckErrors(findResponse);

            //Remove Test TaxSchedule
            SampleData.RemoveTestTaxSchedules(proxy, createTaxScheduleResponse.Id);
        }

        /// <summary>
        ///     This method is used to test Create,Find and Delete Tax Schedule.
        /// </summary>
        [TestMethod]
        public void TaxSchedules_TestCreateAndFindAndDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Tax Schedule.
            var taxSchedule = new TaxScheduleDTO
            {
                DefaultRate = 5,
                DefaultShippingRate = 5,
                Name = "TestCaseSchedule",
                StoreId = 1
            };
            var createResponse = proxy.TaxSchedulesCreate(taxSchedule);
            CheckErrors(createResponse);
            Assert.IsFalse(createResponse.Content.Id == 0);

            //Find Tax Schedule.
            var findResponse = proxy.TaxSchedulesFind(createResponse.Content.Id);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.DefaultRate, findResponse.Content.DefaultRate);
            Assert.AreEqual(createResponse.Content.Name, findResponse.Content.Name);

            //Update Tax Schedule.
            createResponse.Content.DefaultRate = 10;
            createResponse.Content.DefaultShippingRate = 10;
            var updateResponse = proxy.TaxSchedulesUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.DefaultRate, updateResponse.Content.DefaultRate);
            Assert.AreEqual(createResponse.Content.DefaultShippingRate, updateResponse.Content.DefaultShippingRate);

            //Delete Tax Schedule.
            var deleteResponse = proxy.TaxSchedulesDelete(createResponse.Content.Id);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }
    }
}