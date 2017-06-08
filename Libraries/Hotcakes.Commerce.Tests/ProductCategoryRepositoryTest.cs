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
	public class ProductCategoryRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductCategoryRepository _irepoproductcategory;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductCategoryRepositoryTest()
        {
            _irepoproductcategory = new XmlProductCategoryRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductCategory_TestInOrder()
        {
            CreateProduct();

            AddCategory();
            LoadCategory();
            DeleteCategory();
            DeleteAllForProduct();
        }

        #region Product Category Load/Add/Delete Test Cases
        /// <summary>
        /// Loads the category.
        /// </summary>
        //[TestMethod]
        public void LoadCategory()
        {
            //Arrange
            var pcount = _irepoproductcategory.GetTotalProductCategory();
            var tcount = _irepoproductcategory.GetTotalCategory();
            var prj = GetRootProduct();

            //var resultpcat = _application.CatalogServices.FindCategoriesForProduct(prj.Bvin);
            //TODO:Need to change FindManySnapshots function in CategoryRepository for CI
            var resultpcat = _application.CatalogServices.CategoriesXProducts.FindForProduct(prj.Bvin, 1, int.MaxValue);
            var resulttcat = _applicationDB.CatalogServices.Categories.FindAllSnapshotsPaged(1, int.MaxValue);

            //Act/Assert
            Assert.AreEqual(tcount, resulttcat.Count);
            Assert.AreEqual(pcount, resultpcat.Count);

        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        //[TestMethod]
        public void AddCategory()
        {
            //Arrange
            var lstcat = _irepoproductcategory.GetAddProductCategory();
            var prj = GetRootProduct();
            _application.CatalogServices.CategoriesXProducts.DeleteAllForProduct(prj.Bvin);

            //Act
            foreach (var objcat in lstcat.Select(cat => _applicationDB.CatalogServices.Categories.FindMany(cat).FirstOrDefault()).Where(objcat => objcat != null))
            {
                _application.CatalogServices.CategoriesXProducts.AddProductToCategory(prj.Bvin, objcat.Bvin);
            }
            //var resultcatcount = _application.CatalogServices.FindCategoriesForProduct(prj.Bvin);
            //TODO:Need to change FindManySnapshots function in CategoryRepository for CI
            var resultcatcount = _application.CatalogServices.CategoriesXProducts.FindForProduct(prj.Bvin, 1, int.MaxValue);

            //Assert
            Assert.AreEqual(lstcat.Count, resultcatcount.Count);
        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        //[TestMethod]
        public void DeleteCategory()
        {
            //Arrange
            var lstcat = _irepoproductcategory.GetDeleteProductCategory();
            var prj = GetRootProduct();
            //var catcount = _application.CatalogServices.FindCategoriesForProduct(prj.Bvin);
            //TODO:Need to change FindManySnapshots function in CategoryRepository for CI
            var catcount = _application.CatalogServices.CategoriesXProducts.FindForProduct(prj.Bvin, 1, int.MaxValue);

            //Act
            foreach (var objcat in lstcat.Select(cat => _applicationDB.CatalogServices.Categories.FindMany(cat).FirstOrDefault()).Where(objcat => objcat != null))
            {
                _application.CatalogServices.CategoriesXProducts.RemoveProductFromCategory(prj.Bvin, objcat.Bvin);
            }
            //var resultcatcount = _application.CatalogServices.FindCategoriesForProduct(prj.Bvin);
            //TODO:Need to change FindManySnapshots function in CategoryRepository for CI
            var resultcatcount = _application.CatalogServices.CategoriesXProducts.FindForProduct(prj.Bvin, 1, int.MaxValue);


            //Assert
        }


        /// <summary>
        /// Deletes all for product.
        /// </summary>
        //[TestMethod]
        public void DeleteAllForProduct()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.CategoriesXProducts.DeleteAllForProduct(prj.Bvin));
        }


        /// <summary>
        /// Finds all.
        /// </summary>
        [TestMethod]
		public void ProductCategory_FindAll()
        {
            //Arrange
            var count = _irepoproductcategory.GetFindAllCount();

            //Act/Assert
            Assert.AreEqual(count, _applicationDB.CatalogServices.CategoriesXProducts.FindAll().Count);
        }


        /// <summary>
        /// Finds all for all store count.
        /// </summary>
        [TestMethod]
		public void ProductCategory_FindAllForAllStoreCount()
        {
            //Arrange
            var count = _irepoproductcategory.GetFindAllForAllStoreCount();

            //Act/Assert
            Assert.AreEqual(count, _applicationDB.CatalogServices.CategoriesXProducts.FindAllForAllStores().Count);
        }

        /// <summary>
        /// Finds all paged count.
        /// </summary>
        [TestMethod]
		public void ProductCategory_FindAllPagedCount()
        {
            //Arrange
            var count = _irepoproductcategory.GetFindAllPagedCount();

            //Act/Assert
            Assert.AreEqual(count, _applicationDB.CatalogServices.CategoriesXProducts.FindAllPaged(1, int.MaxValue).Count);
        }
        #endregion

    }
}
