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

using System.Linq;
using Hotcakes.Commerce.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests.CatalogService
{
    [TestClass]
    public class CatalogServiceTest : BaseTest
    {
        [TestMethod]
        public void CatalogService_VariantsValidate()
        {
            // This unit test should test following result:
            // 1. INVALID variants were removed
            // 2. SHORT variants were updated
            // 3. INVENTORY were regenerated

            var app = CreateHccAppInMemory();

            // 0. Prepare products with 2 choices and 6 variants
            var p = new Product
            {
                Sku = "S001",
                ProductName = "Sample Product1"
            };

            app.CatalogServices.ProductsCreateWithInventory(p);

            var oSize = Option.Factory(OptionTypes.DropDownList);
            oSize.Name = "Size";
            oSize.IsVariant = true;
            oSize.AddItem("Small");
            oSize.AddItem("Medium");
            oSize.AddItem("Large");

            var oColor = Option.Factory(OptionTypes.RadioButtonList);
            oColor.Name = "Color";
            oColor.IsVariant = true;
            oColor.AddItem("Black");
            oColor.AddItem("Red");

            p.Options.Add(oSize);
            p.Options.Add(oColor);
            app.CatalogServices.Products.Update(p);

            var variants = app.CatalogServices.ProductVariants.FindByProductId(p.Bvin);
            var actualInventories = app.CatalogServices.ProductInventories.FindByProductId(p.Bvin);
            Assert.AreEqual(0, variants.Count);
            Assert.AreEqual(1, actualInventories.Count);

            // Variants generating
            var possibleCount = 0;
            app.CatalogServices.VariantsGenerateAllPossible(p, out possibleCount);
            Assert.AreEqual(6, possibleCount);

            // RELOAD product (current p.Variants doesn contain any items!!!)
            p = app.CatalogServices.Products.Find(p.Bvin);
            variants = app.CatalogServices.ProductVariants.FindByProductId(p.Bvin);
            actualInventories = app.CatalogServices.ProductInventories.FindByProductId(p.Bvin);
            Assert.AreEqual(6, variants.Count);
            //Assert.AreEqual(6, actualInventories.Count); 
            Assert.AreEqual(1, actualInventories.Count); // !!! INVENTORY DOESNOT REGENERATE AUTOMATIC

            oSize = p.Options.FirstOrDefault(o => o.Name == "Size");
            Assert.IsNotNull(oSize);

            oSize.Items.RemoveAt(2); // Remove "Large" size item

            var oType = Option.Factory(OptionTypes.DropDownList);
            oType.Name = "Type";
            oType.IsVariant = true;
            oType.AddItem("Basic");
            oType.AddItem("Pro");
            p.Options.Add(oType); // Add new VARIANT option
            app.CatalogServices.Products.Update(p);

            // ===================== BEFORE Validate Variants =====================
            variants = app.CatalogServices.ProductVariants.FindByProductId(p.Bvin);
            actualInventories = app.CatalogServices.ProductInventories.FindByProductId(p.Bvin);
            Assert.AreEqual(6, variants.Count);
            Assert.AreEqual(1, actualInventories.Count);
            Assert.AreEqual(2, variants[0].Selections.Count);


            // ===================== AFTER Validate Variants =====================
            app.CatalogServices.VariantsValidate(p);

            variants = app.CatalogServices.ProductVariants.FindByProductId(p.Bvin);
            actualInventories = app.CatalogServices.ProductInventories.FindByProductId(p.Bvin);
            Assert.AreEqual(4, variants.Count);
            Assert.AreEqual(4, actualInventories.Count);
            Assert.AreEqual(3, variants[0].Selections.Count);
        }

        //- VariantsValidateForSharedOption
        //- VariantsGenerateAllPossible
        //- VariantsGetAllPossibleSelections

        [TestMethod]
        public void CatalogService_ProductsCreateWithInventory_Default()
        {
            // This unit test should test following result:
            // - new product created
            // - inventory generated for each variant
            // - updated product visibility status

            var app = CreateHccAppInMemory();

            var p = new Product
            {
                Sku = "S001",
                ProductName = "Sample Product1"
            };

            app.CatalogServices.ProductsCreateWithInventory(p);

            var actualProduct = app.CatalogServices.Products.Find(p.Bvin);
            var actualInventories = app.CatalogServices.ProductInventories.FindByProductId(p.Bvin);

            Assert.AreEqual(1, actualInventories.Count);
            Assert.AreEqual(0, actualInventories[0].QuantityOnHand);
            Assert.AreEqual(0, actualInventories[0].QuantityReserved);
        }

        [TestMethod]
        public void CatalogService_ProductsCreateWithInventory_Variants()
        {
            // This unit test should test following result:
            // - new product created
            // - inventory generated for each variant
            // - updated product visibility status

            var app = CreateHccAppInMemory();

            var p = new Product
            {
                Sku = "S001",
                ProductName = "Sample Product1"
            };

            var o = Option.Factory(OptionTypes.DropDownList);
            o.IsVariant = true;
            o.Name = "Size";
            o.AddItem("Small");
            o.AddItem("Large");

            p.Options.Add(o);
            app.CatalogServices.ProductsCreateWithInventory(p);
            int possibleCount;
            app.CatalogServices.VariantsGenerateAllPossible(p, out possibleCount);
            app.CatalogServices.VariantsValidate(p);

            var actualProduct = app.CatalogServices.Products.Find(p.Bvin);
            var actualInventories = app.CatalogServices.ProductInventories.FindByProductId(p.Bvin);
            var actualInv = actualInventories.FirstOrDefault();

            Assert.AreEqual(1, p.Options.Count);
            Assert.AreEqual(2, p.Variants.Count);
            Assert.AreEqual(2, actualInventories.Count);
            Assert.AreEqual(0, actualInv.QuantityOnHand);
            Assert.AreEqual(0, actualInv.QuantityReserved);
        }

        //{
        //public void CatalogService_VariantsGenerateAllPossible()

        //[TestMethod]
        //	// This 
        //	var app = CreateHccAppInMemory();

        //	var p = new Product
        //	{
        //		Sku = "S001",
        //		ProductName = "Sample Product1"
        //	};

        //	app.CatalogServices.ProductsCreateWithInventory(p);
        //	app.CatalogServices.VariantsGenerateAllPossible(p);
        //}

        //- FindProductForCategoryWithSort ???


        //- ProductsAddOption
        //- ProductsRemoveOption
        //- ProductsCreateWithInventory
        //- CloneProduct
        //- UpdateProductVisibleStatusAndSave
        //- CleanUpInventory
        //- SimpleProductInventoryCheck
        //- InventoryLineItemShipQuantity
        //- Inventory...
        //- ProductTypeMovePropertyUp
        //- ProductPropertiesFindForType
    }
}