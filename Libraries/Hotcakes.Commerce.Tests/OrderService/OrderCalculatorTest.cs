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

using System;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Shipping.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    [TestClass]
    public class OrderCalculatorTest : BaseTest
    {
        /// <summary>
        ///     A test for Order Constructor
        /// </summary>
        [TestMethod]
        public void OrderCalculator_TestBugWithFreeShipping_12738()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            // Create taxes
            var tax1 = new TaxSchedule {Name = "Tax1"};
            var tax2 = new TaxSchedule {Name = "Tax2"};
            app.OrderServices.TaxSchedules.Create(tax1);
            app.OrderServices.TaxSchedules.Create(tax2);
            var taxrate1 = new Tax
            {
                Rate = 10,
                ShippingRate = 5,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax1.Id
            };
            var taxrate2 = new Tax
            {
                Rate = 10,
                ShippingRate = 10,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax2.Id
            };
            app.OrderServices.Taxes.Create(taxrate1);
            app.OrderServices.Taxes.Create(taxrate2);

            // Prepare products
            var catA = new Category {Name = "Category A"};
            var catB = new Category {Name = "Category B"};
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100, TaxSchedule = tax1.Id};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50, TaxSchedule = tax2.Id};

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

            // Create Shipping Method            
            var sm = new ShippingMethod();

            var flatRatePerOrder = new FlatRatePerOrder();
            flatRatePerOrder.Settings.Amount = 2m;

            sm.Bvin = Guid.NewGuid().ToString();
            sm.ShippingProviderId = flatRatePerOrder.Id;
            sm.Settings = flatRatePerOrder.Settings;
            sm.Adjustment = 0m;
            sm.Name = "Flat Rate Per Item";
            sm.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(sm);
            app.OrderServices.EnsureDefaultZones(app.CurrentStore.Id);

            o.ShippingAddress.City = "Richmond";
            o.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            o.ShippingAddress.Line1 = "124 Anywhere St.";
            o.ShippingAddress.PostalCode = "23233";
            o.ShippingAddress.RegionBvin = "VA";
            o.ShippingMethodId = sm.Bvin;
            o.ShippingProviderId = sm.ShippingProviderId;

            app.CalculateOrder(o);

            Assert.IsTrue(li1.IsMarkedForFreeShipping);
            Assert.IsFalse(li2.IsMarkedForFreeShipping);
            Assert.AreEqual(1.33m, li1.ShippingPortion);
            Assert.AreEqual(0.67m, li2.ShippingPortion);
            Assert.AreEqual(10m, li1.TaxPortion);
            Assert.AreEqual(5m, li2.TaxPortion);

            Assert.AreEqual(2m, o.TotalShippingBeforeDiscounts);
            Assert.AreEqual(2m, o.TotalShippingAfterDiscounts);

            Assert.AreEqual(15m, o.ItemsTax);
            Assert.AreEqual(0.14m, o.ShippingTax);
            Assert.AreEqual(15.14m, o.TotalTax);
        }

        [TestMethod]
        public void OrderCalculator_TestBugWithFreeShipping_ExtraShipping()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            // Create taxes
            var tax1 = new TaxSchedule {Name = "Tax1"};
            var tax2 = new TaxSchedule {Name = "Tax2"};
            app.OrderServices.TaxSchedules.Create(tax1);
            app.OrderServices.TaxSchedules.Create(tax2);
            var taxrate1 = new Tax
            {
                Rate = 10,
                ShippingRate = 5,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax1.Id
            };
            var taxrate2 = new Tax
            {
                Rate = 10,
                ShippingRate = 10,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax2.Id
            };
            app.OrderServices.Taxes.Create(taxrate1);
            app.OrderServices.Taxes.Create(taxrate2);

            // Prepare products
            var catA = new Category {Name = "Category A"};
            var catB = new Category {Name = "Category B"};
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100, TaxSchedule = tax1.Id};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50, TaxSchedule = tax2.Id};

            prod1.ShippingDetails.ExtraShipFee = 1.1m;
            prod2.ShippingDetails.ExtraShipFee = 1.2m;

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

            // Create Shipping Method            
            var sm = new ShippingMethod();

            var flatRatePerOrder = new FlatRatePerOrder();
            flatRatePerOrder.Settings.Amount = 2m;

            sm.Bvin = Guid.NewGuid().ToString();
            sm.ShippingProviderId = flatRatePerOrder.Id;
            sm.Settings = flatRatePerOrder.Settings;
            sm.Adjustment = 0m;
            sm.Name = "Flat Rate Per Item";
            sm.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(sm);
            app.OrderServices.EnsureDefaultZones(app.CurrentStore.Id);

            o.ShippingAddress.City = "Richmond";
            o.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            o.ShippingAddress.Line1 = "124 Anywhere St.";
            o.ShippingAddress.PostalCode = "23233";
            o.ShippingAddress.RegionBvin = "VA";
            o.ShippingMethodId = sm.Bvin;
            o.ShippingProviderId = sm.ShippingProviderId;

            app.CalculateOrder(o);

            Assert.IsTrue(li1.IsMarkedForFreeShipping);
            Assert.IsFalse(li2.IsMarkedForFreeShipping);
            Assert.AreEqual(2.13m, li1.ShippingPortion);
            Assert.AreEqual(1.07m, li2.ShippingPortion);
            Assert.AreEqual(10m, li1.TaxPortion);
            Assert.AreEqual(5m, li2.TaxPortion);

            Assert.AreEqual(3.2m, o.TotalShippingBeforeDiscounts);
            Assert.AreEqual(3.2m, o.TotalShippingAfterDiscounts);

            Assert.AreEqual(15m, o.ItemsTax);
            Assert.AreEqual(0.22m, o.ShippingTax);
            Assert.AreEqual(15.22m, o.TotalTax);
        }

        [TestMethod]
        public void OrderCalculator_TestBugWithFreeShipping_HandlingFee()
        {
            var app = CreateHccAppInMemory();

            app.CurrentStore.Settings.HandlingAmount = 0.5m;
            app.CurrentStore.Settings.HandlingType = (int) HandlingMode.PerItem;

            // Create taxes
            var tax1 = new TaxSchedule {Name = "Tax1"};
            var tax2 = new TaxSchedule {Name = "Tax2"};
            app.OrderServices.TaxSchedules.Create(tax1);
            app.OrderServices.TaxSchedules.Create(tax2);
            var taxrate1 = new Tax
            {
                Rate = 10,
                ShippingRate = 5,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax1.Id
            };
            var taxrate2 = new Tax
            {
                Rate = 10,
                ShippingRate = 10,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax2.Id
            };
            app.OrderServices.Taxes.Create(taxrate1);
            app.OrderServices.Taxes.Create(taxrate2);

            // Prepare products
            var catA = new Category {Name = "Category A"};
            var catB = new Category {Name = "Category B"};
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100, TaxSchedule = tax1.Id};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50, TaxSchedule = tax2.Id};

            prod1.ShippingDetails.ExtraShipFee = 1.1m;
            prod2.ShippingDetails.ExtraShipFee = 1.2m;

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

            // Create Shipping Method            
            var sm = new ShippingMethod();

            var flatRatePerOrder = new FlatRatePerOrder();
            flatRatePerOrder.Settings.Amount = 2m;

            sm.Bvin = Guid.NewGuid().ToString();
            sm.ShippingProviderId = flatRatePerOrder.Id;
            sm.Settings = flatRatePerOrder.Settings;
            sm.Adjustment = 0m;
            sm.Name = "Flat Rate Per Item";
            sm.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(sm);
            app.OrderServices.EnsureDefaultZones(app.CurrentStore.Id);

            o.ShippingAddress.City = "Richmond";
            o.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            o.ShippingAddress.Line1 = "124 Anywhere St.";
            o.ShippingAddress.PostalCode = "23233";
            o.ShippingAddress.RegionBvin = "VA";
            o.ShippingMethodId = sm.Bvin;
            o.ShippingProviderId = sm.ShippingProviderId;

            app.CalculateOrder(o);

            Assert.IsTrue(li1.IsMarkedForFreeShipping);
            Assert.IsFalse(li2.IsMarkedForFreeShipping);

            Assert.AreEqual(1m, o.TotalHandling);
            Assert.AreEqual(2.8m, li1.ShippingPortion);
            Assert.AreEqual(1.4m, li2.ShippingPortion);
            Assert.AreEqual(10m, li1.TaxPortion);
            Assert.AreEqual(5m, li2.TaxPortion);

            Assert.AreEqual(4.2m, o.TotalShippingBeforeDiscounts);
            Assert.AreEqual(4.2m, o.TotalShippingAfterDiscounts);

            Assert.AreEqual(15m, o.ItemsTax);
            Assert.AreEqual(0.28m, o.ShippingTax); // 0.67*0.05 + 3.53*0.1 = 0.3865, rounding = 0.39
            Assert.AreEqual(15.28m, o.TotalTax);
        }

        [TestMethod]
        public void OrderCalculator_TestBugWithFreeShipping_HandlingFee2()
        {
            var app = CreateHccAppInMemory();

            app.CurrentStore.Settings.HandlingAmount = 0.5m;
            app.CurrentStore.Settings.HandlingType = (int) HandlingMode.PerItem;

            // Create taxes
            var tax1 = new TaxSchedule {Name = "Tax1"};
            var tax2 = new TaxSchedule {Name = "Tax2"};
            app.OrderServices.TaxSchedules.Create(tax1);
            app.OrderServices.TaxSchedules.Create(tax2);
            var taxrate1 = new Tax
            {
                Rate = 10,
                ShippingRate = 5,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax1.Id
            };
            var taxrate2 = new Tax
            {
                Rate = 10,
                ShippingRate = 10,
                CountryIsoAlpha3 = "USA",
                ApplyToShipping = true,
                TaxScheduleId = tax2.Id
            };
            app.OrderServices.Taxes.Create(taxrate1);
            app.OrderServices.Taxes.Create(taxrate2);

            // Prepare products
            var catA = new Category {Name = "Category A"};
            var prod1 = new Product {ProductName = "Product 1", SitePrice = 100, TaxSchedule = tax1.Id};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50, TaxSchedule = tax2.Id};

            prod1.ShippingDetails.ExtraShipFee = 1.1m;
            prod2.ShippingDetails.ExtraShipFee = 1.2m;

            app.CatalogServices.Categories.Create(catA);
            app.CatalogServices.Products.Create(prod1);
            app.CatalogServices.Products.Create(prod2);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod1.Bvin, catA.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod2.Bvin, catA.Bvin);

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

            // Create Shipping Method            
            var sm = new ShippingMethod();

            var flatRatePerOrder = new FlatRatePerOrder();
            flatRatePerOrder.Settings.Amount = 2m;

            sm.Bvin = Guid.NewGuid().ToString();
            sm.ShippingProviderId = flatRatePerOrder.Id;
            sm.Settings = flatRatePerOrder.Settings;
            sm.Adjustment = 0m;
            sm.Name = "Flat Rate Per Item";
            sm.ZoneId = -100; // US All Zone
            app.OrderServices.ShippingMethods.Create(sm);
            app.OrderServices.EnsureDefaultZones(app.CurrentStore.Id);

            o.ShippingAddress.City = "Richmond";
            o.ShippingAddress.CountryBvin = Country.UnitedStatesCountryBvin;
            o.ShippingAddress.Line1 = "124 Anywhere St.";
            o.ShippingAddress.PostalCode = "23233";
            o.ShippingAddress.RegionBvin = "VA";

            o.ShippingMethodId = "FREESHIPPING";
            o.ShippingProviderId = string.Empty;

            app.CalculateOrder(o);

            Assert.IsTrue(li1.IsMarkedForFreeShipping);
            Assert.IsTrue(li2.IsMarkedForFreeShipping);

            Assert.AreEqual(1m, o.TotalHandling);
            Assert.AreEqual(0.67m, li1.ShippingPortion);
            Assert.AreEqual(0.33m, li2.ShippingPortion);
            Assert.AreEqual(10m, li1.TaxPortion);
            Assert.AreEqual(5m, li2.TaxPortion);

            Assert.AreEqual(1m, o.TotalShippingBeforeDiscounts);
            Assert.AreEqual(1m, o.TotalShippingAfterDiscounts);

            Assert.AreEqual(15m, o.ItemsTax);
            Assert.AreEqual(0.06m, o.ShippingTax); // 0.67 * 0.05 + 0.33 * 0.1 = 0,0665
            Assert.AreEqual(15.06m, o.TotalTax);
        }

        [TestMethod]
        public void OrderCalculator_TestBugWithGiftcardDiscounts_13878()
        {
            //InitBasicStubs();
            var app = CreateHccAppInMemory();

            // Prepare products
            var catA = new Category {Name = "Category A"};
            var catB = new Category {Name = "Category B"};
            var prod1 = new Product {ProductName = "Gift Card", SitePrice = 100, IsGiftCard = true};
            var prod2 = new Product {ProductName = "Product 2", SitePrice = 50};

            app.CatalogServices.Categories.Create(catA);
            app.CatalogServices.Categories.Create(catB);
            app.CatalogServices.Products.Create(prod1);
            app.CatalogServices.Products.Create(prod2);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod1.Bvin, catA.Bvin);
            app.CatalogServices.CategoriesXProducts.AddProductToCategory(prod2.Bvin, catB.Bvin);

            // Prepare promotion
            var orderTotalDiscount = new Promotion {Mode = PromotionType.OfferForOrder, Name = "-50%", IsEnabled = true};
            orderTotalDiscount.AddQualification(new AnyOrder());
            orderTotalDiscount.AddAction(new OrderTotalAdjustment(AmountTypes.Percent, -50));
            orderTotalDiscount.AddAction(new OrderTotalAdjustment(AmountTypes.MonetaryAmount, -100));
            app.MarketingServices.Promotions.Create(orderTotalDiscount);

            var o = new Order {StoreId = app.CurrentStore.Id};
            var li1 = prod1.ConvertToLineItem(app, 1);
            var li2 = prod2.ConvertToLineItem(app, 1);
            o.Items.Add(li1);
            o.Items.Add(li2);

            app.CalculateOrder(o);

            Assert.AreEqual(100m, li1.LineTotal);
            Assert.AreEqual(50m, li2.LineTotal);

            Assert.AreEqual(-25m, o.OrderDiscountDetails[0].Amount);
            Assert.AreEqual(-100m, o.OrderDiscountDetails[1].Amount);

            Assert.AreEqual(150m, o.TotalOrderBeforeDiscounts);
            Assert.AreEqual(100m, o.TotalOrderAfterDiscounts);
            Assert.AreEqual(100m, o.TotalGrand);
        }
    }
}