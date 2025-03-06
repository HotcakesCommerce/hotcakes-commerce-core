﻿#region License

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
using Hotcakes.CommerceDTO.v1.Taxes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This method is used to perform tests against Tax end points in REST API.
    /// </summary>
    [TestClass]
    public class TaxesTest : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test Get all taxes.
        /// </summary>
        [TestMethod]
        public void Taxes_FindAll()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Test TaxSchedule as prerequisites          
            var createTaxScheduleResponse = SampleData.CreateTestTaxSchedules(proxy);

            //Create Test Tax as prerequisites          
            var createTaxResponse = SampleData.CreateTestTax(proxy, createTaxScheduleResponse.Id);

            //Get List of all Taxes
            var findResponse = proxy.TaxesFindAll();
            CheckErrors(findResponse);

            //Remove Test Tax
            SampleData.RemoveTestTax(proxy, createTaxResponse.Id);

            //Remove Test TaxSchedule
            SampleData.RemoveTestTaxSchedules(proxy, createTaxScheduleResponse.Id);
        }

        /// <summary>
        ///     This method is used to test Create, Find and Delete Tax.
        /// </summary>
        [TestMethod]
        public void Taxes_TestCreateAndFindAndDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Test TaxSchedule as prerequisites          
            var createTaxScheduleResponse = SampleData.CreateTestTaxSchedules(proxy);

            //Get list of all tax schedules
            var lstOfTaxSchedule = proxy.TaxSchedulesFindAll();

            //Create Tax
            var tax = new TaxDTO
            {
                Rate = 5,
                ShippingRate = 5,
                ApplyToShipping = true,
                StoreId = 1,
                PostalCode = "33401",
                CountryIsoAlpha3 = "US",
                TaxScheduleId = lstOfTaxSchedule.Content.First().Id
            };
            var createResponse = proxy.TaxesCreate(tax);
            CheckErrors(createResponse);
            Assert.IsFalse(createResponse.Content.Id == 0);

            //Find Tax
            var findResponse = proxy.TaxesFind(createResponse.Content.Id);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.Rate, findResponse.Content.Rate);
            Assert.AreEqual(createResponse.Content.ShippingRate, findResponse.Content.ShippingRate);

            //Update Tax
            createResponse.Content.ShippingRate = 10;
            createResponse.Content.Rate = 10;
            var updateResponse = proxy.TaxesUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.ShippingRate, updateResponse.Content.ShippingRate);
            Assert.AreEqual(createResponse.Content.Rate, updateResponse.Content.Rate);

            //Delete Tax
            var deleteResponse = proxy.TaxesDelete(createResponse.Content.Id);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);

            //Remove Test TaxSchedule
            SampleData.RemoveTestTaxSchedules(proxy, createTaxScheduleResponse.Id);
        }
    }
}