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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Shipping.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductType = Hotcakes.Commerce.Marketing.PromotionQualifications.ProductType;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    ///     This is a test class for PromotionTest and is intended
    ///     to contain all PromotionTest Unit Tests
    /// </summary>
    [TestClass]
    public class PromotionTest : BaseTest
    {
        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Promotion_CanCreateAPromotion()
        {
            var app = CreateHccAppInMemory();

            var p = new Promotion();
            var r = new PromotionRepository(new HccRequestContext());

            Assert.IsTrue(r.Create(p), "Create should be true");
            Assert.IsNotNull(p);
            Assert.IsTrue(p.Id > 0, "Promotion was not assigned an Id number");
        }

        [TestMethod]
        public void Promotion_CanFindPromotionInRepository()
        {
            var app = CreateHccAppInMemory();

            var p = new Promotion();
            p.Name = "FindMe";
            var r = new PromotionRepository(new HccRequestContext());

            Assert.IsTrue(r.Create(p), "Create should be true");
            Assert.IsNotNull(p);
            Assert.IsTrue(p.Id > 0, "Promotion was not assigned an Id number");

            var target = r.Find(p.Id);
            Assert.IsNotNull(target, "Found should not be null");
            Assert.AreEqual(p.Id, target.Id, "Id didn't match");
            Assert.AreEqual(p.StoreId, target.StoreId, "Store ID Didn't Match");
            Assert.AreEqual(p.Name, target.Name, "Name didn't match");
        }

        [TestMethod]
        public void Promotion_CanSaveQualificationsAndActionsInRepository()
        {
            var app = CreateHccAppInMemory();

            var p = new Promotion {Mode = PromotionType.Sale, Name = "FindMe"};
            p.AddQualification(new ProductBvin(new List<string> {"ABC123"}));
            p.AddQualification(new ProductType("TYPE0"));
            p.AddAction(new ProductPriceAdjustment(AmountTypes.Percent, 50));
            var r = new PromotionRepository(new HccRequestContext());


            Assert.IsTrue(r.Create(p), "Create should be true");
            Assert.IsNotNull(p);
            Assert.IsTrue(p.Id > 0, "Promotion was not assigned an Id number");

            var target = r.Find(p.Id);
            Assert.IsNotNull(target, "Found should not be null");
            Assert.AreEqual(p.Id, target.Id, "Id didn't match");
            Assert.AreEqual(p.StoreId, target.StoreId, "Store ID Didn't Match");
            Assert.AreEqual(p.Name, target.Name, "Name didn't match");

            Assert.AreEqual(p.Qualifications.Count, target.Qualifications.Count, "Qualification count didn't match");
            for (var i = 0; i < p.Qualifications.Count; i++)
            {
                Assert.AreEqual(p.Qualifications[0].Id, target.Qualifications[0].Id,
                    "Id of index " + i + " didn't match");
                Assert.AreEqual(p.Qualifications[0].GetType(), target.Qualifications[0].GetType(),
                    "Type of index " + i + " didn't match");
            }

            Assert.AreEqual(p.Actions.Count, target.Actions.Count, "Action count didn't match");
            for (var i = 0; i < p.Actions.Count; i++)
            {
                Assert.AreEqual(p.Actions[0].Id, target.Actions[0].Id, "Id of action index " + i + " didn't match");
                Assert.AreEqual(p.Actions[0].GetType(), target.Actions[0].GetType(),
                    "Type of action index " + i + " didn't match");
            }
        }

        [TestMethod]
        public void Promotion_CanUpdateNameOfPromotion()
        {
            var app = CreateHccAppInMemory();

            var p = new Promotion();
            p.Name = "Old";
            var r = new PromotionRepository(new HccRequestContext());
            r.Create(p);

            var updatedName = "New";
            p.Name = updatedName;
            Assert.IsTrue(r.Update(p), "Update should be true");
            var target = r.Find(p.Id);

            Assert.IsNotNull(target, "Found should not be null");
            Assert.AreEqual(p.Id, target.Id, "Id didn't match");
            Assert.AreEqual(p.StoreId, target.StoreId, "Store ID Didn't Match");
            Assert.AreEqual("New", target.Name, "Name didn't match");
        }

        //[TestMethod()]
        //public void CanGetActiveStatus()
        //{
        //	Promotion target = new Promotion();
        //	target.IsEnabled = true;
        //	target.StartDateUtc = new DateTime(2010, 09, 1, 0, 0, 0); // Sept 1st, 2010
        //	target.EndDateUtc = new DateTime(2010, 11, 1, 23, 59, 59); // Nov. 1, 2010


        //	DateTime currentUtcTime = new DateTime(2010,9,15,23,59,59); // Sept. 15th, 11:59:59 pm 
        //	PromotionStatus expected = PromotionStatus.Active;
        //	PromotionStatus actual;
        //	actual = target.GetStatus(currentUtcTime);
        //	Assert.AreEqual(expected, actual, "Sept 15th should return active status");


        //	DateTime currentUtcTime2 = new DateTime(2010, 9, 1, 0, 0, 0); // Sept. 1th, 2010
        //	PromotionStatus expected2 = PromotionStatus.Active;
        //	PromotionStatus actual2;
        //	actual2 = target.GetStatus(currentUtcTime2);
        //	Assert.AreEqual(expected2, actual2, "Sept 1st should return active status");

        //	DateTime currentUtcTime3 = new DateTime(2010, 11, 1, 23, 59, 59); // Sept. 1th, 2010
        //	PromotionStatus expected3 = PromotionStatus.Active;
        //	PromotionStatus actual3;
        //	actual3 = target.GetStatus(currentUtcTime3);
        //	Assert.AreEqual(expected3, actual3, "Nov 1st should return active status");            
        //}

        //[TestMethod()]
        //public void CanGetDisabledStatus()
        //{
        //	Promotion target = new Promotion();
        //	target.IsEnabled = false;
        //	target.StartDateUtc = new DateTime(2010, 09, 1, 0, 0, 0); // Sept 1st, 2010
        //	target.EndDateUtc = new DateTime(2010, 11, 1, 23, 59, 59); // Nov. 1, 2010


        //	DateTime currentUtcTime = new DateTime(2010, 9, 15, 23, 59, 59); // Sept. 15th, 11:59:59 pm 
        //	PromotionStatus expected = PromotionStatus.Disabled;
        //	PromotionStatus actual;
        //	actual = target.GetStatus(currentUtcTime);
        //	Assert.AreEqual(expected, actual, "Sept 15th should return disabled");


        //	DateTime currentUtcTime2 = new DateTime(2009, 9, 1, 0, 0, 0); // Sept. 1th, 2009
        //	PromotionStatus expected2 = PromotionStatus.Disabled;
        //	PromotionStatus actual2;
        //	actual2 = target.GetStatus(currentUtcTime2);
        //	Assert.AreEqual(expected2, actual2, "Sept 1st from one year ago should return disabled status");

        //	DateTime currentUtcTime3 = new DateTime(2012, 11, 1, 23, 59, 59); // Sept. 1th, 2012
        //	PromotionStatus expected3 = PromotionStatus.Disabled;
        //	PromotionStatus actual3;
        //	actual3 = target.GetStatus(currentUtcTime3);
        //	Assert.AreEqual(expected3, actual3, "Nov 1st from 2012 should return disabled status");
        //}

        //[TestMethod()]
        //public void CanGetExpiredStatus()
        //{
        //	Promotion target = new Promotion();
        //	target.IsEnabled = true;
        //	target.StartDateUtc = new DateTime(2010, 09, 1, 0, 0, 0); // Sept 1st, 2010
        //	target.EndDateUtc = new DateTime(2010, 11, 1, 23, 59, 59); // Nov. 1, 2010


        //	DateTime currentUtcTime = new DateTime(2010, 11, 2, 0, 0, 0); // Nov. 2nd
        //	PromotionStatus expected = PromotionStatus.Expired;
        //	PromotionStatus actual;
        //	actual = target.GetStatus(currentUtcTime);
        //	Assert.AreEqual(expected, actual, "Nov 2nd should return Expired status");


        //	DateTime currentUtcTime2 = new DateTime(2011, 9, 1, 0, 0, 0); // Sept. 1th, 2011
        //	PromotionStatus expected2 = PromotionStatus.Expired;
        //	PromotionStatus actual2;
        //	actual2 = target.GetStatus(currentUtcTime2);
        //	Assert.AreEqual(expected2, actual2, "Sept 1st should return Expired status");
        //}

        //[TestMethod()]
        //public void CanGetUpcomingStatus()
        //{
        //	Promotion target = new Promotion();
        //	target.IsEnabled = true;
        //	target.StartDateUtc = new DateTime(2010, 09, 1, 0, 0, 0); // Sept 1st, 2010
        //	target.EndDateUtc = new DateTime(2010, 11, 1, 23, 59, 59); // Nov. 1, 2010


        //	DateTime currentUtcTime = new DateTime(2010, 08, 1, 0, 0, 0); 
        //	PromotionStatus expected = PromotionStatus.Upcoming;
        //	PromotionStatus actual;
        //	actual = target.GetStatus(currentUtcTime);
        //	Assert.AreEqual(expected, actual, "Augst 1st should return upcoming status");


        //	DateTime currentUtcTime2 = new DateTime(2010, 8, 31, 23, 59, 59); // August 31st
        //	PromotionStatus expected2 = PromotionStatus.Upcoming;
        //	PromotionStatus actual2;
        //	actual2 = target.GetStatus(currentUtcTime2);
        //	Assert.AreEqual(expected2, actual2, "August 31th should return Upcoming status");
        //}

        [TestMethod]
        public void Promotion_CanPutAProductOnSale()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            var p = new Promotion();
            p.IsEnabled = true;
            p.Name = "Product Sale Test";
            p.CustomerDescription = "A Customer Discount";
            p.StartDateUtc = DateTime.UtcNow.AddDays(-1);
            p.EndDateUtc = DateTime.UtcNow.AddDays(1);
            p.StoreId = 1;
            p.Id = 1;

            var prod = new Product();
            prod.Bvin = "553690e2-6372-431f-97c0-83e11a05f298";
            prod.SitePrice = 59.99m;
            app.CatalogServices.Products.Create(prod);

            Assert.IsTrue(p.AddQualification(new ProductBvin(prod.Bvin)), "Add Qualification Failed");
            Assert.IsTrue(p.AddAction(new ProductPriceAdjustment(AmountTypes.MonetaryAmount, -20m)), "Add Action Failed");


            var userPrice = new UserSpecificPrice(prod, null, app.CurrentStore.Settings);


            var actual = p.ApplyToProduct(app.CurrentRequestContext, prod, userPrice, null);

            Assert.IsTrue(actual, "Promotion should have applied with return value of true");
            Assert.AreEqual(39.99m, userPrice.PriceWithAdjustments(), "Price should have been reduced by $20.00");
            Assert.AreEqual(p.CustomerDescription, userPrice.DiscountDetails[0].Description,
                "Description should match promotion");
        }

        [TestMethod]
        public void Promotion_CanPutAProductOnSalePricedByApp()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            // Create a Promotion to Test
            var p = new Promotion();
            p.Mode = PromotionType.Sale;
            p.IsEnabled = true;
            p.Name = "Product Sale Test";
            p.CustomerDescription = "$10.00 off Test Sale!";
            p.StartDateUtc = DateTime.UtcNow.AddDays(-1);
            p.EndDateUtc = DateTime.UtcNow.AddDays(1);
            p.StoreId = 1;
            p.Id = 0;
            p.AddQualification(new ProductBvin("b991f9de-2bf2-4472-a913-bfaa08e230cf"));
            p.AddAction(new ProductPriceAdjustment(AmountTypes.MonetaryAmount, -10m));
            app.MarketingServices.Promotions.Create(p);

            // Create a test Product
            var prod = new Product();
            prod.Bvin = "b991f9de-2bf2-4472-a913-bfaa08e230cf";
            prod.SitePrice = 59.99m;
            prod.StoreId = 1;

            var actualPrice = app.PriceProduct(prod, null, null, app.CurrentlyActiveSales);

            Assert.IsNotNull(actualPrice, "Price should not be null");
            Assert.AreEqual(49.99m, actualPrice.PriceWithAdjustments(), "Price should be $49.99");
            Assert.AreEqual(1, actualPrice.DiscountDetails.Count, "Discount Details count should be one");
            Assert.AreEqual(p.CustomerDescription, actualPrice.DiscountDetails[0].Description,
                "Description should match promotion");
        }

        [TestMethod]
        public void Promotion_CanSkipPromotionIfNoProduct()
        {
            var app = CreateHccAppInMemory();

            var p = new Promotion();
            p.IsEnabled = true;
            p.Name = "Product Sale Test";
            p.StartDateUtc = DateTime.UtcNow.AddDays(-1);
            p.EndDateUtc = DateTime.UtcNow.AddDays(1);
            p.StoreId = 1;
            p.Id = 1;
            Assert.IsTrue(p.AddQualification(new ProductBvin("b991f9de-2bf2-4472-a913-bfaa08e230cf")),
                "Add Qualification Failed");
            Assert.IsTrue(p.AddAction(new ProductPriceAdjustment(AmountTypes.MonetaryAmount, -20m)), "Add Action Failed");

            var actual = p.ApplyToProduct(app.CurrentRequestContext, null, null, null);

            Assert.IsFalse(actual, "Promotion should not have applied");
        }

        /// <summary>
        ///     A test for ActionsToXml
        /// </summary>
        [TestMethod]
        public void Promotion_ActionsToXmlTest()
        {
            var target = new Promotion();
            var a1 = new ProductPriceAdjustment(AmountTypes.MonetaryAmount, 1.23m);
            target.AddAction(a1);

            var expected = "<Actions>" + Environment.NewLine;

            expected += "  <Action>" + Environment.NewLine;
            expected += "    <Id>" + a1.Id + "</Id>" + Environment.NewLine;
            expected += "    <TypeId>" + a1.TypeId + "</TypeId>" + Environment.NewLine;
            expected += "    <Settings>" + Environment.NewLine;
            expected += "      <Setting>" + Environment.NewLine;
            expected += "        <Key>AdjustmentType</Key>" + Environment.NewLine;
            expected += "        <Value>1</Value>" + Environment.NewLine;
            expected += "      </Setting>" + Environment.NewLine;
            expected += "      <Setting>" + Environment.NewLine;
            expected += "        <Key>Amount</Key>" + Environment.NewLine;
            expected += "        <Value>1.23</Value>" + Environment.NewLine;
            expected += "      </Setting>" + Environment.NewLine;
            expected += "    </Settings>" + Environment.NewLine;
            expected += "  </Action>" + Environment.NewLine;

            expected += "</Actions>";

            string actual;
            actual = target.ActionsToXml();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     A test for QualificationsFromXml
        /// </summary>
        [TestMethod]
        public void Promotion_QualificationsFromXmlTest()
        {
            var expected = new Promotion();
            var q1 = new ProductBvin("abc123");
            expected.AddQualification(q1);

            var xml = "<Qualifications>" + Environment.NewLine;
            xml += "  <Qualification>" + Environment.NewLine;
            xml += "    <Id>" + q1.Id + "</Id>" + Environment.NewLine;
            xml += "    <TypeId>" + q1.TypeId + "</TypeId>" + Environment.NewLine;
            xml += "    <ProcessingCost>0</ProcessingCost>" + Environment.NewLine;
            xml += "    <Settings>" + Environment.NewLine;
            xml += "      <Setting>" + Environment.NewLine;
            xml += "        <Key>products</Key>" + Environment.NewLine;
            xml += "        <Value>abc123</Value>" + Environment.NewLine;
            xml += "      </Setting>" + Environment.NewLine;
            xml += "    </Settings>" + Environment.NewLine;
            xml += "  </Qualification>" + Environment.NewLine;
            xml += "</Qualifications>";

            var actual = new Promotion();
            actual.QualificationsFromXml(xml);

            Assert.AreEqual(expected.Qualifications.Count, actual.Qualifications.Count,
                "Qualifications count did not match");
            Assert.AreEqual(q1.GetProductIds()[0], ((ProductBvin) actual.Qualifications[0]).GetProductIds()[0],
                "Product bvin didn't come through");
            for (var i = 0; i < expected.Qualifications.Count; i++)
            {
                Assert.AreEqual(expected.Qualifications[i].Id, actual.Qualifications[i].Id,
                    "Id didn't match for qualification index " + i);
                Assert.AreEqual(expected.Qualifications[i].ProcessingCost, actual.Qualifications[i].ProcessingCost,
                    "Processing Cost didn't match for qualification index " + i);
                Assert.AreEqual(expected.Qualifications[i].Settings.Count, actual.Qualifications[i].Settings.Count,
                    "Settings Count didn't match for qualification index " + i);
                Assert.AreEqual(expected.Qualifications[i].TypeId, actual.Qualifications[i].TypeId,
                    "TypeId didn't match for qualification index " + i);
            }
        }

        /// <summary>
        ///     A test for QualificationsToXml
        /// </summary>
        [TestMethod]
        public void Promotion_QualificationsToXmlTest()
        {
            var target = new Promotion();
            var q1 = new ProductBvin("abc123");
            target.AddQualification(q1);

            var expected = "<Qualifications>" + Environment.NewLine;

            expected += "  <Qualification>" + Environment.NewLine;
            expected += "    <Id>" + q1.Id + "</Id>" + Environment.NewLine;
            expected += "    <TypeId>" + q1.TypeId + "</TypeId>" + Environment.NewLine;
            expected += "    <ProcessingCost>0</ProcessingCost>" + Environment.NewLine;
            expected += "    <Settings>" + Environment.NewLine;
            expected += "      <Setting>" + Environment.NewLine;
            expected += "        <Key>products</Key>" + Environment.NewLine;
            expected += "        <Value>abc123</Value>" + Environment.NewLine;
            expected += "      </Setting>" + Environment.NewLine;
            expected += "    </Settings>" + Environment.NewLine;
            expected += "  </Qualification>" + Environment.NewLine;

            expected += "</Qualifications>";

            string actual;
            actual = target.QualificationsToXml();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     A test for ActionsFromXml
        /// </summary>
        [TestMethod]
        public void Promotion_ActionsFromXmlTest()
        {
            var expected = new Promotion();
            var a1 = new ProductPriceAdjustment(AmountTypes.MonetaryAmount, 1.23m);
            expected.AddAction(a1);

            var xml = "<Actions>" + Environment.NewLine;
            xml += "  <Action>" + Environment.NewLine;
            xml += "    <Id>" + a1.Id + "</Id>" + Environment.NewLine;
            xml += "    <TypeId>" + a1.TypeId + "</TypeId>" + Environment.NewLine;
            xml += "    <Settings>" + Environment.NewLine;
            xml += "      <Setting>" + Environment.NewLine;
            xml += "        <Key>AdjustmentType</Key>" + Environment.NewLine;
            xml += "        <Value>1</Value>" + Environment.NewLine;
            xml += "      </Setting>" + Environment.NewLine;
            xml += "      <Setting>" + Environment.NewLine;
            xml += "        <Key>Amount</Key>" + Environment.NewLine;
            xml += "        <Value>1.23</Value>" + Environment.NewLine;
            xml += "      </Setting>" + Environment.NewLine;
            xml += "    </Settings>" + Environment.NewLine;
            xml += "  </Action>" + Environment.NewLine;
            xml += "</Actions>";

            var actual = new Promotion();
            actual.ActionsFromXml(xml);

            Assert.AreEqual(expected.Actions.Count, actual.Actions.Count, "Actions count did not match");
            Assert.AreEqual(a1.Amount, ((ProductPriceAdjustment) actual.Actions[0]).Amount, "Amount didn't come through");
            Assert.AreEqual(a1.AdjustmentType, ((ProductPriceAdjustment) actual.Actions[0]).AdjustmentType,
                "Adjustment Type didn't come through");
            for (var i = 0; i < expected.Actions.Count; i++)
            {
                Assert.AreEqual(expected.Actions[i].Id, actual.Actions[i].Id, "Id didn't match for action index " + i);
                Assert.AreEqual(expected.Actions[i].Settings.Count, actual.Actions[i].Settings.Count,
                    "Settings Count didn't match for action index " + i);
                Assert.AreEqual(expected.Actions[i].TypeId, actual.Actions[i].TypeId,
                    "TypeId didn't match for action index " + i);
            }
        }

        [TestMethod]
        public void Promotion_CanDiscountAnOrderByCoupon()
        {
            var app = CreateHccAppInMemory();

            // Create a Promotion to Test
            var p = new Promotion();
            p.Mode = PromotionType.OfferForOrder;
            p.IsEnabled = true;
            p.Name = "Discount By Coupon Test";
            p.CustomerDescription = "$20.00 off Test Offer!";
            p.StartDateUtc = DateTime.UtcNow.AddDays(-1);
            p.EndDateUtc = DateTime.UtcNow.AddDays(1);
            p.StoreId = 1;
            p.Id = 0;
            var q = new OrderHasCoupon();
            q.AddCoupon("COUPON");
            p.AddQualification(q);
            p.AddAction(new OrderTotalAdjustment(AmountTypes.MonetaryAmount, -20m));
            app.MarketingServices.Promotions.Create(p);

            // Create a test Order
            var o = new Order();
            o.Items.Add(new LineItem {BasePricePerItem = 59.99m, ProductName = "Sample Product", ProductSku = "ABC123"});
            app.CalculateOrderAndSave(o);

            Assert.AreEqual(59.99m, o.TotalOrderAfterDiscounts, "Order total should be $59.99 before discounts");

            o.AddCouponCode("COUPON");
            app.CalculateOrderAndSave(o);

            Assert.AreEqual(39.99m, o.TotalOrderAfterDiscounts, "Order total after discounts should be $39.99");
            Assert.AreEqual(-20m, o.TotalOrderDiscounts, "Discount should be -20");
            Assert.AreEqual(59.99m, o.TotalOrderBeforeDiscounts,
                "Order total with coupon but before discount should be $59.99");
        }

        [TestMethod]
        public void Promotion_CanDiscountShipping()
        {
            var app = CreateHccAppInMemory();

            // Create a Promotion to Test
            var p = new Promotion();
            p.Mode = PromotionType.OfferForShipping;
            p.IsEnabled = true;
            p.Name = "10% Off Shipping Test";
            p.CustomerDescription = "10% Off Shipping Test!";
            p.StartDateUtc = DateTime.UtcNow.AddDays(-1);
            p.EndDateUtc = DateTime.UtcNow.AddDays(1);
            p.StoreId = 1;
            p.Id = 0;
            var q = new OrderHasCoupon();
            q.AddCoupon("COUPON");
            p.AddQualification(q);
            p.AddAction(new OrderShippingAdjustment(AmountTypes.Percent, -10m));
            app.MarketingServices.Promotions.Create(p);

            // Create shipping method
            var method = new ShippingMethod();
            method.Adjustment = 0m;
            method.AdjustmentType = ShippingMethodAdjustmentType.Amount;
            method.Bvin = string.Empty;
            method.Name = "Test Name";
            method.ShippingProviderId = new FlatRatePerOrder().Id;
            method.Settings = new FlatRatePerItemSettings {Amount = 100};
            method.ZoneId = -100;
            app.OrderServices.ShippingMethods.Create(method);
            app.OrderServices.EnsureDefaultZones(app.CurrentStore.Id);

            var prod1 = new Product {ProductName = "Sample Product", Sku = "ABC123", SitePrice = 50};
            app.CatalogServices.Products.Create(prod1);

            // Create a test Order
            var o = new Order {StoreId = app.CurrentStore.Id};
            o.ShippingMethodId = method.Bvin;
            o.ShippingProviderId = method.ShippingProviderId;

            o.Items.Add(prod1.ConvertToLineItem(app, 1));
            app.CalculateOrderAndSave(o);

            Assert.AreEqual(100.00m, o.TotalShippingAfterDiscounts, "Shipping should be $100 before discounts");
            Assert.AreEqual(150.00m, o.TotalGrand, "Grand Total should be $150");

            o.AddCouponCode("COUPON");
            app.CalculateOrderAndSave(o);

            Assert.AreEqual(90.00m, o.TotalShippingAfterDiscounts, "Shipping After Discount should be $90.00");
            Assert.AreEqual(-10m, o.TotalShippingDiscounts, "Discount should be -10");
            Assert.AreEqual(100.00m, o.TotalShippingBeforeDiscounts, "Total Before Discounts Should be $100.00");
            Assert.AreEqual(140.00m, o.TotalGrand, "Grand Total should be $140");
        }

        [TestMethod]
        public void Promotion_TestLineItemFreeShipping()
        {
            var app = CreateHccAppInMemory();

            // Prepare products
            var catA = new Category {Name = "Category A"};
            var catB = new Category {Name = "Category B"};
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50};

            app.CatalogServices.Categories.Create(catA);
            app.CatalogServices.Categories.Create(catB);
            app.CatalogServices.Products.Create(prod1);
            app.CatalogServices.Products.Create(prod2);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod1.Bvin, catA.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod2.Bvin, catB.Bvin);

            // Prepare promotion
            var promFreeShipping = new Promotion
            {
                Mode = PromotionType.OfferForLineItems,
                Name = "FREE SHIPPING",
                IsEnabled = true
            };
            promFreeShipping.AddQualification(new LineItemCategory(catA.Bvin));
            promFreeShipping.AddAction(new LineItemFreeShipping());
            app.MarketingServices.Promotions.Create(promFreeShipping);

            var o = new Order {StoreId = app.CurrentStore.Id};
            var li1 = prod1.ConvertToLineItem(app, 1);
            var li2 = prod2.ConvertToLineItem(app, 1);
            o.Items.Add(li1);
            o.Items.Add(li2);

            app.CalculateOrder(o);

            Assert.IsTrue(li1.IsMarkedForFreeShipping);
            Assert.IsFalse(li2.IsMarkedForFreeShipping);
        }

        [TestMethod]
        public void Promotion_TestLineItemIsProduct()
        {
            var app = CreateHccAppInMemory();

            // Prepare products
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50};
            app.CatalogServices.Products.Create(prod1);
            app.CatalogServices.Products.Create(prod2);

            // Prepare promotion
            var prom = new Promotion
            {
                Mode = PromotionType.OfferForLineItems,
                Name = "SPECIAL PRODUCTS",
                IsEnabled = true
            };
            var qIsProduct = new LineItemIsProduct();
            qIsProduct.AddProductIds(new List<string> {prod1.Bvin});
            prom.AddQualification(qIsProduct);
            prom.AddAction(new LineItemFreeShipping());

            var o = new Order {StoreId = app.CurrentStore.Id};
            var li1 = prod1.ConvertToLineItem(app, 1);
            var li2 = prod2.ConvertToLineItem(app, 1);
            o.Items.Add(li1);
            o.Items.Add(li2);


            prom.ApplyToOrder(app.CurrentRequestContext, o);

            Assert.IsTrue(li1.IsMarkedForFreeShipping);
            Assert.IsFalse(li2.IsMarkedForFreeShipping);
        }

        [TestMethod]
        public void Promotion_MigrateOldPromotions()
        {
#pragma warning disable 0612, 0618
            var app = CreateHccAppInMemory();

            // Add Sale promo
            var sale = new Promotion {Mode = PromotionType.Sale, Name = "SALE"};
            sale.AddAction(new ProductPriceAdjustment());
            app.MarketingServices.Promotions.Create(sale);

            // Add Empty Offer
            var offer = new Promotion {Mode = PromotionType.Offer, Name = "OFFER"};
            app.MarketingServices.Promotions.Create(offer);

            // Add Candidate to split
            var offer2s = new Promotion {Mode = PromotionType.Offer, Name = "Should Be Splitted"};
            // 2 qualifications
            offer2s.AddQualification(new AnyOrder());
            offer2s.AddQualification(new AnyProduct());
            // 5 actions
            offer2s.AddAction(new LineItemAdjustment());
            offer2s.AddAction(new LineItemFreeShipping());
            offer2s.AddAction(new OrderShippingAdjustment());
            offer2s.AddAction(new OrderTotalAdjustment());
            offer2s.AddAction(new OrderTotalAdjustment());
            app.MarketingServices.Promotions.Create(offer2s);

            var total = 0;
            app.MarketingServices.Promotions.FindAllWithFilter(PromotionType.Offer, "", true, 1, int.MaxValue, ref total);
            Assert.AreEqual(2, total);

            app.MarketingServices.MigrateOldPromotions();

            var proms = app.MarketingServices.Promotions.FindAllPaged(1, int.MaxValue);
            Assert.AreEqual(0, proms.Where(p => p.Mode == PromotionType.Offer).Count(),
                "All 'Offer' promotions should be migrated to other modes");
            Assert.AreEqual(5, proms.Where(p => p.Mode != PromotionType.Offer).Count());
            Assert.AreEqual(1, proms.Where(p => p.Mode == PromotionType.Sale).Count());

            Assert.AreEqual(1, proms.Where(p => p.Mode == PromotionType.OfferForLineItems).Count());
            Assert.AreEqual(1, proms.Where(p => p.Mode == PromotionType.OfferForShipping).Count());
            Assert.AreEqual(2, proms.Where(p => p.Mode == PromotionType.OfferForOrder).Count());

            var pLi = proms.FirstOrDefault(p => p.Mode == PromotionType.OfferForLineItems);
            var pSh = proms.FirstOrDefault(p => p.Mode == PromotionType.OfferForShipping);
            var pO = proms.FirstOrDefault(p => p.Mode == PromotionType.OfferForOrder && p.Name == "Should Be Splitted");

            Assert.AreEqual(2, pLi.Actions.Count);
            Assert.AreEqual(2, pLi.Qualifications.Count);
            Assert.AreEqual(1, pSh.Actions.Count);
            Assert.AreEqual(2, pSh.Qualifications.Count);
            Assert.AreEqual(2, pO.Actions.Count);
            Assert.AreEqual(2, pO.Qualifications.Count);

            Assert.AreEqual(typeof (OrderTotalAdjustment), pO.Actions[0].GetType());
            Assert.AreEqual(typeof (OrderTotalAdjustment), pO.Actions[1].GetType());
            Assert.AreEqual(typeof (AnyOrder), pO.Qualifications[0].GetType());
            Assert.AreEqual(typeof (AnyProduct), pO.Qualifications[1].GetType());
#pragma warning restore 0612, 0618
        }

        [TestMethod]
        public void Promotion_ResortItems()
        {
            var app = CreateHccAppInMemory();

            var prom1 = new Promotion {Mode = PromotionType.Sale, Name = "Promo1"};
            var prom2 = new Promotion {Mode = PromotionType.Sale, Name = "Promo2"};
            var prom3 = new Promotion {Mode = PromotionType.Sale, Name = "Promo3"};
            var prom4 = new Promotion {Mode = PromotionType.Sale, Name = "Promo4"};
            app.MarketingServices.Promotions.Create(prom1);
            app.MarketingServices.Promotions.Create(prom2);
            app.MarketingServices.Promotions.Create(prom3);
            app.MarketingServices.Promotions.Create(prom4);

            Assert.AreEqual(4, app.MarketingServices.Promotions.CountOfAll());

            app.MarketingServices.Promotions.Resort(new[] {prom1.Id, prom4.Id, prom2.Id, prom3.Id}.ToList());
            var actual = app.MarketingServices.Promotions.FindAll();

            Assert.AreEqual("Promo1", actual[0].Name);
            Assert.AreEqual("Promo4", actual[1].Name);
            Assert.AreEqual("Promo2", actual[2].Name);
            Assert.AreEqual("Promo3", actual[3].Name);

            Assert.AreEqual(1, actual[0].SortOrder);
            Assert.AreEqual(2, actual[1].SortOrder);
            Assert.AreEqual(3, actual[2].SortOrder);
            Assert.AreEqual(4, actual[3].SortOrder);

            app.MarketingServices.Promotions.Resort(new[] {prom2.Id, prom4.Id}.ToList(), 1);
            actual = app.MarketingServices.Promotions.FindAll();

            Assert.AreEqual("Promo1", actual[0].Name);
            Assert.AreEqual("Promo2", actual[1].Name);
            Assert.AreEqual("Promo4", actual[2].Name);
            Assert.AreEqual("Promo3", actual[3].Name);

            Assert.AreEqual(1, actual[0].SortOrder);
            Assert.AreEqual(2, actual[1].SortOrder);
            Assert.AreEqual(3, actual[2].SortOrder);
            Assert.AreEqual(4, actual[3].SortOrder);
        }

        [TestMethod]
        public void Promotion_TestSumOfProductsWithinCategories()
        {
            var app = CreateHccAppInMemory();

            // Prepare products
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50};
            var prod3 = new Product {ProductName = "Product 3", SitePrice = 30};
            app.CatalogServices.Products.Create(prod1);
            app.CatalogServices.Products.Create(prod2);
            app.CatalogServices.Products.Create(prod3);

            var catA = new Category {Name = "CatA"};
            var catB = new Category {Name = "CatB"};
            app.CatalogServices.Categories.Create(catA);
            app.CatalogServices.Categories.Create(catB);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod1.Bvin, catA.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod2.Bvin, catA.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod3.Bvin, catB.Bvin);

            // Prepare promotion
            var prom = new Promotion {Mode = PromotionType.OfferForOrder, Name = "SPECIAL PRODUCTS", IsEnabled = true};
            var qIsProduct = new SumOrCountOfProducts();
            qIsProduct.AddCategoryId(catA.Bvin);
            qIsProduct.SumAmount = 149;
            prom.AddQualification(qIsProduct);
            prom.AddAction(new OrderTotalAdjustment(AmountTypes.MonetaryAmount, -10));

            var o = new Order {StoreId = app.CurrentStore.Id};
            var li1 = prod1.ConvertToLineItem(app, 1);
            var li2 = prod2.ConvertToLineItem(app, 1);
            var li3 = prod3.ConvertToLineItem(app, 1);
            o.Items.Add(li1);
            o.Items.Add(li2);
            o.Items.Add(li3);

            app.CalculateOrder(o);

            Assert.AreEqual(180m, o.TotalGrand);

            prom.ApplyToOrder(app.CurrentRequestContext, o);

            Assert.AreEqual(170m, o.TotalGrand);
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion
    }
}