using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Tests.IRepository;
using Hotcakes.Commerce.Tests.XmlRepository;
using Hotcakes.Commerce.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    [TestClass]
	public class ProductInventoryRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductInventoryRepository _irepoproductinventory;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductInventoryRepositoryTest()
        {
            _irepoproductinventory = new XmlProductInventoryRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductInventory_TestInOrder()
        {
            CreateProduct();

            AddProductInventory();
            EditProductInventory();
            LoadProductInventory();
            ProductInventoryExists();
            Find();
            DeleteProductInventoryByProductId();

        }

        #region Product Inventory Load/Add/Edit/Delete Test Cases
        /// <summary>
        /// Loads the product inventory.
        /// </summary>
        //[TestMethod]
        public void LoadProductInventory()
        {
            //Arrange
            var prjinvinfo = _irepoproductinventory.GetProductInventoryInfo();
            var prjinv = _irepoproductinventory.GetProductInventory();
            var prj = GetRootProduct();

            var resultinv = _application.CatalogServices.ProductInventories.FindByProductId(prj.Bvin).FirstOrDefault();
            if (resultinv == null) return;

            //Assert
            Assert.AreEqual(prjinvinfo.IsAvailableForSale, prj.IsAvailableForSale);
            Assert.AreEqual(prjinvinfo.InventoryMode, prj.InventoryMode);
            Assert.AreEqual(prjinv.LowStockPoint, resultinv.LowStockPoint);
            Assert.AreEqual(prjinv.OutOfStockPoint, resultinv.OutOfStockPoint);
            Assert.AreEqual(prjinv.QuantityAvailableForSale, resultinv.QuantityAvailableForSale);
            Assert.AreEqual(prjinv.QuantityOnHand, resultinv.QuantityOnHand);
            Assert.AreEqual(prjinv.QuantityReserved, resultinv.QuantityReserved);

        }

        /// <summary>
        /// Adds the product inventory.
        /// </summary>
        //[TestMethod]
        public void AddProductInventory()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act
            _application.CatalogServices.InventoryGenerateForProduct(prj);
            _application.CatalogServices.CleanUpInventory(prj);
            _application.CatalogServices.UpdateProductVisibleStatusAndSave(prj);
            var inventorylst = _application.CatalogServices.ProductInventories.FindByProductId(prj.Bvin);

            var inventory = inventorylst.FirstOrDefault();
            if (inventory == null) Assert.Fail();

            //Assert
            Assert.IsTrue(prj.IsAvailableForSale);
            Assert.AreEqual(0, inventory.QuantityOnHand);
            Assert.AreEqual(0, inventory.LowStockPoint);
            Assert.AreEqual(0, inventory.OutOfStockPoint);
            Assert.AreEqual(0, inventory.QuantityAvailableForSale);

        }

        /// <summary>
        /// Edits the product inventory.
        /// </summary>
        //[TestMethod]
        public void EditProductInventory()
        {
            //Arrange
            var prj = GetRootProduct();
            var inventory = _irepoproductinventory.GetEditProductInventory();
            var inventoryinfo = _irepoproductinventory.GetEditProductInventoryInfo();
            var inventorylst = _application.CatalogServices.ProductInventories.FindByProductId(prj.Bvin);

            var inventoryedit = inventorylst.FirstOrDefault();
            if (inventoryedit == null) Assert.Fail();


            prj.InventoryMode = inventoryinfo.InventoryMode;

            inventoryedit.LowStockPoint = inventory.LowStockPoint;
            inventoryedit.QuantityOnHand = inventory.QuantityOnHand;
            inventoryedit.OutOfStockPoint = inventory.OutOfStockPoint;


            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductsUpdateWithSearchRebuild(prj));
            Assert.AreEqual(inventoryinfo.InventoryMode, prj.InventoryMode);

            Assert.IsTrue(_application.CatalogServices.ProductInventories.Update(inventoryedit));
            var resultinventory = _application.CatalogServices.ProductInventories.FindByProductId(prj.Bvin).FirstOrDefault();
            if (resultinventory == null) Assert.Fail();
            Assert.AreEqual(inventory.QuantityOnHand, resultinventory.QuantityOnHand);
            Assert.AreEqual(inventory.LowStockPoint, resultinventory.LowStockPoint);
            Assert.AreEqual(inventory.OutOfStockPoint, resultinventory.OutOfStockPoint);

            _application.CatalogServices.UpdateProductVisibleStatusAndSave(prj);
            Assert.IsTrue(prj.IsAvailableForSale);
        }


        /// <summary>
        /// Deletes the product inventory by product identifier.
        /// </summary>
        //[TestMethod]
        public void DeleteProductInventoryByProductId()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductInventories.DeleteByProductId(prj.Bvin));
        }

        /// <summary>
        /// Finds all low stock.
        /// </summary>
        [TestMethod]
		public void ProductInventory_FindAllLowStock()
        {
            //Arrange
            var count = _irepoproductinventory.GetFindAllLowStockCount();
            //Act/Assert
            Assert.AreEqual(count,_applicationDB.CatalogServices.ProductInventories.FindAllLowStock().Count);
        }


        /// <summary>
        /// Finds this instance.
        /// </summary>
        //[TestMethod]
        public void Find()
        {
            //Arrange
            var prj = GetRootProduct();
            var inventorylst = _application.CatalogServices.ProductInventories.FindByProductId(prj.Bvin);
            var inventory = inventorylst.FirstOrDefault();
            if (inventory == null) Assert.Fail();

            //Act/Assert
            Assert.IsNotNull(_application.CatalogServices.ProductInventories.Find(inventory.Bvin));
        }

        /// <summary>
        /// Products the inventory exists.
        /// </summary>
        //[TestMethod]
        public void ProductInventoryExists()
        {
            //Arrange
            var prj = GetRootProduct();
            var resultinv = _application.CatalogServices.ProductInventories.FindByProductId(prj.Bvin).FirstOrDefault();
            if (resultinv == null) Assert.Fail();

            //Act/Assert
            Assert.IsNotNull(_application.CatalogServices.ProductInventories.InventoryExists(resultinv));

        }

        #endregion

    }
}
