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
using Hotcakes.CommerceDTO.v1.Membership;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class used to perform different test against CustomerAccount end points in REST API.
    /// </summary>
    [TestClass]
    public class CustomerAccountTests : ApiTestBase
    {
        /// <summary>
        ///     Find Customer Account by its unique identifier.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_TestFind()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.CustomerAccountsFind(TestConstants.TestAccount1Id);
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     Find Customer account by Email address.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_TestFindByEmail1()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.CustomerAccountsFindByEmail(TestConstants.TestAccount1Email);
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     Find all customer accounts.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_TestFindAll()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.CustomerAccountsFindAll();
            CheckErrors(findResponse);
        }


        /// <summary>
        ///     Get Customer Account list with paging.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_TestFindAllByPage()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.CustomerAccountsFindAllByPage(2, 5);
            CheckErrors(findResponse);
            Assert.AreEqual(findResponse.Content.Count, 5);
        }

        /// <summary>
        ///     Get count of all Customer Account.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_TestCountOfAll()
        {
            var proxy = CreateApiProxy();

            var findResponse = proxy.CustomerAccountsCountOfAll();
            CheckErrors(findResponse);
        }

        /// <summary>
        ///     Create, Update and Delete Customer Account.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_TestCreateAndUpdateAndDelete()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Create Customer Account
            //User with tTest already exists
            var account = new CustomerAccountDTO
            {
                FirstName = "Tst",
                LastName = "Test",
                Email = "tstAccount@gmail.com",
                Password = "password1",
                BillingAddress = new AddressDTO
                {
                    City = "New York",
                    CountryName = "United States",
                    RegionName = "New York"
                }
            };
            var createResponse = proxy.CustomerAccountsCreate(account);
            CheckErrors(createResponse);

            //Update Customer Account.
            account = createResponse.Content;
            account.TaxExempt = true;
            var updateResponse = proxy.CustomerAccountsUpdate(account);
            CheckErrors(updateResponse);
            Assert.IsTrue(updateResponse.Content.TaxExempt);

            //Delete Customer Account.
            var accountId = createResponse.Content.Bvin;
            var deleteResponse = proxy.CustomerAccountsDelete(accountId);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);
        }

        /// <summary>
        ///     Clear All Customer account.
        /// </summary>
        [TestMethod]
        public void CustomerAccount_ClearAll()
        {
            var proxy = CreateApiProxy();

            var clearAllResponse = proxy.CustomerAccountsClearAll();
            CheckErrors(clearAllResponse);
            Assert.IsTrue(clearAllResponse.Content);
        }
    }
}