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

using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Profile.Fakes;
using DotNetNuke.Entities.Users;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    [TestClass]
    public class AffiliateTest : BaseTest
    {
        public static void InitShims()
        {
            ShimProfileController.GetPropertyDefinitionByNameInt32String =
                (portalId, name) => new ProfilePropertyDefinition(portalId) {PropertyName = name};
        }

        [TestMethod]
        public void Affiliate_CanAddAffiliate()
        {
            using (ShimsContext.Create())
            {
                InitShims();

                var hccApp = CreateHccAppInMemory();
                var target = hccApp.ContactServices.Affiliates;

                var expected = new Affiliate
                {
                    AffiliateId = "001",
                    TaxId = "123",
                    Username = "Username"
                };

                var status = CreateUserStatus.None;
                var res = target.Create(expected, ref status);
                var actual = target.FindAllPaged(1, 1).First();

                Assert.AreEqual(1, target.CountOfAll());
                Assert.AreEqual(expected.TaxId, actual.TaxId);
                Assert.AreEqual(expected.Username, actual.Username);
            }
        }

        [TestMethod]
        public void Affiliate_CanAddAffiliateWithExistedUser()
        {
            using (ShimsContext.Create())
            {
                InitShims();
                var users = new List<UserInfo>();
                var dnnUserCtl = GetStubIUserController(users);
                DnnGlobal.SetTestableInstance(GetStubIDnnGlobal());
                DnnUserController.SetTestableInstance(dnnUserCtl);
                users.Add(dnnUserCtl.BuildUserInfo(
                    "user1", "User", "First", "email@test",
                    "12345", DnnGlobal.Instance.GetPortalId()));

                var hccApp = CreateHccAppInMemory();
                var target = hccApp.ContactServices.Affiliates;

                var expected = new Affiliate
                {
                    AffiliateId = "001",
                    UserId = users[0].UserID,
                    TaxId = "123",
                    Username = "Username"
                };

                var status = CreateUserStatus.None;
                var res = target.Create(expected, ref status);
                var actual = target.FindAllPaged(1, 1).First();

                Assert.AreEqual(1, target.CountOfAll());
                Assert.AreEqual(expected.TaxId, actual.TaxId);
                Assert.AreEqual(expected.Username, actual.Username);
                Assert.AreEqual(1, users.Count);
            }
        }

        [TestMethod]
        public void Affiliate_CanUpdateAffiliate()
        {
            using (ShimsContext.Create())
            {
                InitShims();

                var hccApp = CreateHccAppInMemory();
                var target = hccApp.ContactServices.Affiliates;

                var expected = new Affiliate
                {
                    AffiliateId = "001",
                    StoreId = 2,
                    TaxId = "123",
                    Username = "Username"
                };

                var status = CreateUserStatus.None;
                var res = target.Create(expected, ref status);

                expected.TaxId = "1234";
                expected.Username = "Username2";

                target.Update(expected);
                var actual = target.FindAllPaged(1, 1).First();

                Assert.AreEqual(1, target.CountOfAll());
                Assert.AreEqual(expected.TaxId, actual.TaxId);
                Assert.AreEqual(expected.Username, actual.Username);
            }
        }

        [TestMethod]
        public void Affiliate_TestFindAllWithFilter()
        {
            using (ShimsContext.Create())
            {
                InitShims();

                var hccApp = CreateHccAppInMemory();
                var target = hccApp.ContactServices.Affiliates;

                // Create 3 affiliates
                var aff1 = new Affiliate {StoreId = 2, AffiliateId = "a001", Username = "Username"};
                var aff2 = new Affiliate
                {
                    StoreId = 2,
                    AffiliateId = "a002",
                    Username = "Username2",
                    ReferralAffiliateId = "a001"
                };
                var aff3 = new Affiliate
                {
                    StoreId = 2,
                    AffiliateId = "a003",
                    Username = "Username3",
                    ReferralAffiliateId = "a001"
                };

                var status = CreateUserStatus.None;
                target.Create(aff1, ref status);
                target.Create(aff2, ref status);
                target.Create(aff3, ref status);

                // Add orders using affiliateid
                var o = new Order {AffiliateID = aff3.Id};
                var li = new LineItem
                {
                    BasePricePerItem = 19.99m,
                    ProductName = "Sample Product",
                    ProductSku = "ABC123",
                    Quantity = 2
                };
                o.Items.Add(li);

                hccApp.OrderServices.Orders.Upsert(o);

                var rowCount = 0;
                var filter = new AffiliateReportCriteria();
                var actual = target.FindAllWithFilter(filter, 1, 10, ref rowCount);

                Assert.AreEqual(3, actual.Count);
                var actual1 = actual.FirstOrDefault(a => a.AffiliateId == "a001");
                var actual3 = actual.FirstOrDefault(a => a.AffiliateId == "a003");
                Assert.IsNotNull(actual1);
                Assert.AreEqual(2, actual1.SignupsCount);
                Assert.AreEqual(1, actual3.OrdersCount);
            }
        }

        [TestMethod]
        public void Affiliate_TestFindAllWithFilter_ShowNonApproved()
        {
            using (ShimsContext.Create())
            {
                InitShims();

                var hccApp = CreateHccAppInMemory();
                var target = hccApp.ContactServices.Affiliates;

                // Create 3 affiliates
                var aff1 = new Affiliate {StoreId = 2, AffiliateId = "a001", Username = "Username", Approved = true};
                var aff2 = new Affiliate
                {
                    StoreId = 2,
                    AffiliateId = "a002",
                    Username = "Username2",
                    ReferralAffiliateId = "a001",
                    Approved = true
                };
                var aff3 = new Affiliate
                {
                    StoreId = 2,
                    AffiliateId = "a003",
                    Username = "Username3",
                    ReferralAffiliateId = "a001"
                };

                var status = CreateUserStatus.None;
                target.Create(aff1, ref status);
                target.Create(aff2, ref status);
                target.Create(aff3, ref status);

                // Add orders using affiliateid
                var o = new Order {AffiliateID = aff3.Id};
                var li = new LineItem
                {
                    BasePricePerItem = 19.99m,
                    ProductName = "Sample Product",
                    ProductSku = "ABC123",
                    Quantity = 2
                };
                o.Items.Add(li);
                hccApp.OrderServices.Orders.Upsert(o);

                var rowCount = 0;
                var filter = new AffiliateReportCriteria {ShowOnlyNonApproved = true};
                var actual = target.FindAllWithFilter(filter, 1, 10, ref rowCount);

                Assert.AreEqual(1, actual.Count);
                var actual3 = actual.FirstOrDefault(a => a.AffiliateId == "a003");
                Assert.AreEqual(1, actual3.OrdersCount);
            }
        }

        #region Fakes factory

        #endregion
    }
}