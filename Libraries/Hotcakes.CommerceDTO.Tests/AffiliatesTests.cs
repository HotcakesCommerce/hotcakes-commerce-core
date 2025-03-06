#region License

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

using System;
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Hotcakes.CommerceDTO.v1.Membership;
using Hotcakes.CommerceDTO.v1.Taxes;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used perform test against the Affiliate and Affiliate Referral end points in
    ///     REST API.
    /// </summary>
    [TestClass]
    public class AffiliatesTests : ApiTestBase
    {
        /// <summary>
        ///     This method used to test get all affiliates info.
        /// </summary>
        [TestMethod]
        public void Affiliates_FindAll()
        {
            //Create API object
            var proxy = CreateApiProxy();

            //Create Test Affiliate as prerequisites
            var createAffiliateResponse = SampleData.AffiliatesTestCreate(proxy);

            // Get list of all affiliates
            var findResponse = proxy.AffiliatesFindAll(); 

            //Remove Affiliate Test
            SampleData.AffiliatesTestDelete(proxy, createAffiliateResponse);

            CheckErrors(findResponse);
        }

        /// <summary>
        ///     This method is used Create, Find, Update, Delete affiliate info.
        ///     This method also include function to test affiliate referral’s  Create and Find method.
        /// </summary>
        [TestMethod]
        public void Affiliates_TestCreateAndFindAndDelete()
        {
            //Create API proxy
            var proxy = CreateApiProxy();

            //Create Affiliate
            var affiliate = new AffiliateDTO
            {
                StoreId = 1,
                DisplayName = "TestUserName1" + DateTime.Now.ToString("ss"),
                Enabled = true,
                CommissionAmount = 50,
                ReferralId = "TestReferall1" + DateTime.Now.ToString("ss")
            };

            var createResponse = proxy.AffiliatesCreate(affiliate);

            CheckErrors(createResponse);
            Assert.IsFalse(createResponse.Content.Id == 0);

            //Find Affiliate
            var findResponse = proxy.AffiliatesFind(createResponse.Content.Id);
            CheckErrors(findResponse);
            Assert.AreEqual(createResponse.Content.DisplayName, findResponse.Content.DisplayName);
            Assert.AreEqual(createResponse.Content.CommissionAmount, findResponse.Content.CommissionAmount);
            Assert.AreEqual(createResponse.Content.Enabled, findResponse.Content.Enabled);
            Assert.AreEqual(createResponse.Content.ReferralId, findResponse.Content.ReferralId);

            //Create Affiliate Referral
            var affiliateReferral = new AffiliateReferralDTO
            {
                AffiliateId = createResponse.Content.Id,
                ReferrerUrl = string.Empty,
                StoreId = 1,
                TimeOfReferralUtc = DateTime.UtcNow
            };
            var affiliateRefferalCreateResponse = proxy.AffiliateReferralsCreate(affiliateReferral);
            CheckErrors(affiliateRefferalCreateResponse);
            Assert.IsFalse(affiliateRefferalCreateResponse.Content.Id == 0);

            //Find Affiliate Referral
            var findAffiliateReferralResponse = proxy.AffiliateReferralsFindForAffiliate(createResponse.Content.Id);
            CheckErrors(findAffiliateReferralResponse);
            Assert.AreEqual(affiliateRefferalCreateResponse.Content.AffiliateId,
                findAffiliateReferralResponse.Content.First().AffiliateId);
            Assert.AreEqual(affiliateRefferalCreateResponse.Content.StoreId,
                findAffiliateReferralResponse.Content.First().StoreId);

            //Update Affiliate
            createResponse.Content.CommissionAmount = 55;
            var updateResponse = proxy.AffiliatesUpdate(createResponse.Content);
            CheckErrors(updateResponse);
            Assert.AreEqual(createResponse.Content.CommissionAmount, updateResponse.Content.CommissionAmount);

            //Delete Affiliate
            var deleteResponse = proxy.AffiliatesDelete(createResponse.Content.Id);
            CheckErrors(deleteResponse);
            Assert.IsTrue(deleteResponse.Content);

        }
    }
}
