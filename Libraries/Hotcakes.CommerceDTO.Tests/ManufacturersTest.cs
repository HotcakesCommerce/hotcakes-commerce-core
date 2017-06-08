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

using Hotcakes.CommerceDTO.v1.Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class used to perform different test against the REST API methods related to Manufacturer.
    /// </summary>
    [TestClass]
    public class ManufacturersTest : ApiTestBase
    {
        /// <summary>
        ///     Get all Manufacturer detail.
        /// </summary>
        [TestMethod]
        public void Manufacturers_FindAll()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.ManufacturerFindAll();

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used to create, update,delete and find Manufacturers.
        /// </summary>
        [TestMethod]
        public void Manufacturers_TestCreateAndFindAndDelete()
        {
            //Create Manufacturer proxy
            var proxy = CreateApiProxy();

            //Create Manufacturer
            var vendorManufacture = new VendorManufacturerDTO
            {
                Address = new AddressDTO(),
                ContactType = VendorManufacturerTypeDTO.Vendor,
                DisplayName = "New Manufacturer",
                EmailAddress = "testmanufacturer@test.com",
                StoreId = 1
            };
            var createResponse = proxy.ManufacturerCreate(vendorManufacture);
            CheckErrors(createResponse);
            Assert.IsFalse(string.IsNullOrEmpty(createResponse.Content.Bvin));

            //Find Manufacturer
            var findResponse = proxy.ManufacturerFind(createResponse.Content.Bvin);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.EmailAddress, findResponse.Content.EmailAddress);
            Assert.AreEqual(createResponse.Content.DisplayName, findResponse.Content.DisplayName);

            //Update Manufacturer
            createResponse.Content.EmailAddress = "testmanufacturerupdate@test.com";
            var updateResponse = proxy.ManufacturerUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.EmailAddress, updateResponse.Content.EmailAddress);

            //Delete Manufacturer
            var deleteResponse = proxy.ManufacturerDelete(createResponse.Content.Bvin);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }
    }
}