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

using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Taxes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform test against Vendors end points in REST API.
    /// </summary>
    [TestClass]
    public class VendorsTest : ApiTestBase
    {
        /// <summary>
        ///     This method is used to test get all Vendors.
        /// </summary>
        [TestMethod]
        public void Vendors_FindAll()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Test Vendor as prerequisites          
            var vendorRespose = SampleData.CreateTestVendor(proxy);

            //Get All Vendors.
            var findResponse = proxy.VendorFindAll();
            CheckErrors(findResponse);

            //Remove Test Vendor
            SampleData.RemoveTestVendor(proxy, vendorRespose.Bvin);
        }

        /// <summary>
        ///     This method is used to test Create, Find and Delete.
        /// </summary>
        [TestMethod]
        public void Vendors_TestCreateAndFindAndDelete()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Create Vendor.
            var vendorManufacture = new VendorManufacturerDTO
            {
                Address = new AddressDTO(),
                ContactType = VendorManufacturerTypeDTO.Vendor,
                DisplayName = "New Vendor",
                EmailAddress = "testvendor@test.com",
                StoreId = 1
            };
            var createResponse = proxy.VendorCreate(vendorManufacture);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Vendor
            var findResponse = proxy.VendorFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.EmailAddress, findResponse.Content.EmailAddress);
            Assert.AreEqual(createResponse.Content.DisplayName, findResponse.Content.DisplayName);

            //Update Vendor
            createResponse.Content.EmailAddress = "testvendorupdate@test.com";
            var updateResponse = proxy.VendorUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.EmailAddress, updateResponse.Content.EmailAddress);

            //Delete Vendor
            var deleteResponse = proxy.VendorDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }
    }
}