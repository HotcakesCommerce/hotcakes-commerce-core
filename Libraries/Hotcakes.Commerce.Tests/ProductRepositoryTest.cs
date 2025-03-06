#region License

// Distributed under the MIT License
// ============================================================
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
    public class ProductRepositoryTest : BaseProductTest
    {

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductRepositoryTest()
        {
            _irepoproduct = new XmlProductRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
        public void Product_TestInOrder()
        {
            CreateProduct();
            CloneProduct();

            AddProductTab();
            EditProductTab();
            LoadProductTab();
            DeleteProductTab();

            EditProduct();
            DeleteProduct();
            DeleteAllProduct();
        }

        #region Product Repository Service
        #region Product Load/Add/Edit/Delete/Search Test Cases
        /// <summary>
        /// Loads the type of the product.
        /// </summary>
        [TestMethod]
		public void Product_LoadProductType()
        {
            //Arrange
            var count = _irepoproduct.GetTotalProductTypeCount();
            var resultcount = _applicationDB.CatalogServices.ProductTypes.FindAll();

            //Act/Assert
            Assert.AreEqual(count, resultcount.Count);

        }

        /// <summary>
        /// Loads the product.
        /// </summary>
        [TestMethod]
		public void Product_LoadProduct()
        {
            var prjsearch = _irepoproduct.GetTotalProdutCount();
            var searchprm = prjsearch.FirstOrDefault().Value;
            var totalCount = 0;
            searchprm = SetProductSearchCriteria(searchprm);

            //Act
            _applicationDB.CatalogServices.Products.FindByCriteria(searchprm, 1, int.MaxValue, ref totalCount);

            //Assert
            Assert.AreEqual(prjsearch.FirstOrDefault().Key, totalCount);
        }

        /// <summary>
        /// Loads the vendoe.
        /// </summary>
        [TestMethod]
		public void Product_LoadVendor()
        {
            //Arrange
            var count = _irepoproduct.GetTotalVendorCount();

            //Act
            var result = _applicationDB.ContactServices.Vendors.FindAll();

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the manufacturer.
        /// </summary>
        [TestMethod]
		public void Product_LoadManufacturer()
        {
            //Arrange
            var count = _irepoproduct.GetTotalManufacturerCount();

            //Act
            var result = _applicationDB.ContactServices.Manufacturers.FindAll();

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the columns.
        /// </summary>
        [TestMethod]
		public void Product_LoadColumns()
        {
            //Arrange
            var count = _irepoproduct.GetTotalPopulateColumnsCount();

            //Act
            var result = _applicationDB.ContentServices.Columns.FindAll();

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the template.
        /// </summary>
        [TestMethod]
		public void Product_LoadTemplate()
        {
            //Arrange
            var count = _irepoproduct.GetTotalDisplayTemplateCount();

            //Act
            var result = DnnPathHelper.GetViewNames("Products");
            //TODO:Need to change function for CI

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the tax.
        /// </summary>
        [TestMethod]
		public void Product_LoadTax()
        {
            //Arrange
            var count = _irepoproduct.GetTotalTax();

            //Act
            var result = _applicationDB.OrderServices.TaxSchedules.FindAll(_application.CurrentStore.Id);

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        #endregion
        #region Product Repo Functions Test Cases
        /// <summary>
        /// Products the find by bvin.
        /// </summary>
        [TestMethod]
		public void Product_FindByBvin()
        {
            //Arrange
            var bvin = _irepoproduct.FindmanyByBvin();

            //Act
            var result = _applicationDB.CatalogServices.Products.FindMany(bvin);

            //Assert
            Assert.AreNotEqual(0, result.Count);
        }

        /// <summary>
        /// Products the find by sku.
        /// </summary>
        [TestMethod]
		public void Product_FindBySku()
        {
            //Arrange
            var sku = _irepoproduct.FindManyBySku();

            //Act
            var result = _applicationDB.CatalogServices.Products.FindManySkus(sku);

            //Assert
            Assert.AreNotEqual(0, result.Count);
        }

        /// <summary>
        /// Products the find by slug.
        /// </summary>
        [TestMethod]
		public void Product_FindBySlug()
        {
            //Arrange
            var slug = _irepoproduct.FindBySlug();

            //Act
            var result = _applicationDB.CatalogServices.Products.FindBySlug(slug);

            //Assert
            Assert.AreEqual(slug, result.UrlSlug);
        }

        /// <summary>
        /// Loads the feature product.
        /// </summary>
        [TestMethod]
		public void Product_LoadFeatureProduct()
        {
            //Arrange
            var count = _irepoproduct.FeatureProductCount();

            //Acr
            var result = _applicationDB.CatalogServices.Products.FindFeatured(1, int.MaxValue);

            //Assert
            Assert.AreEqual(count, result.Count);

        }

        /// <summary>
        /// Loads all product.
        /// </summary>
        [TestMethod]
		public void Product_LoadAllProductCount()
        {
            //Arrange
            var count = _irepoproduct.FindAllCount();

            //Acr
            var result = _applicationDB.CatalogServices.Products.FindAllCount();

            //Assert
            Assert.AreEqual(count, result);

        }

        /// <summary>
        /// Loads all product for all store count.
        /// </summary>
        [TestMethod]
		public void Product_LoadAllProductForAllStoreCount()
        {
            //Arrange
            var count = _irepoproduct.FindAllCountForAllStoreCount();

            //Acr
            var result = _applicationDB.CatalogServices.Products.FindAllForAllStoresCount();

            //Assert
            Assert.AreEqual(count, result);
        }

        /// <summary>
        /// Loads all paged count.
        /// </summary>
        [TestMethod]
		public void Product_LoadAllPagedCount()
        {
            //Arrange
            var count = _irepoproduct.FindAllPagedCount();

            //Acr
            var result = _applicationDB.CatalogServices.Products.FindAllPaged(1, int.MaxValue);

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads all paged for all store count.
        /// </summary>
        [TestMethod]
		public void Product_LoadAllPagedForAllStoreCount()
        {
            //Arrange
            var count = _irepoproduct.FindAllPagedForAllStoreCount();

            //Acr
            var result = _applicationDB.CatalogServices.Products.FindAllPagedForAllStores(1, int.MaxValue);

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        #endregion
        #endregion

        #region Product Info Tabs Load/Add/Edit/Delete Test Cases
        /// <summary>
        /// Loads the product tab.
        /// </summary>
        //[TestMethod]
        public void LoadProductTab()
        {
            //Arrange
            var count = _irepoproduct.GetTotalProductTabCount();
            var prj = GetRootProduct();

            //Act/Assert
            Assert.AreEqual(count, prj.Tabs.Count);
        }

        /// <summary>
        /// Adds the product tab.
        /// </summary>
        //[TestMethod]
        public void AddProductTab()
        {
            //Arrange
            var tab = _irepoproduct.GetAddProductTab();
            var prj = GetRootProduct();
            tab.Bvin = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
            if (prj.Tabs.Count > 0)
            {
                var m = (from sort in prj.Tabs select sort.SortOrder).Max();
                tab.SortOrder = m + 1;
            }
            else
                tab.SortOrder = 1;

            //Act
            prj.Tabs.Add(tab);

            //Assert
            Assert.IsTrue(_application.CatalogServices.ProductsUpdateWithSearchRebuild(prj));
        }

        /// <summary>
        /// Edits the product tab.
        /// </summary>
        //[TestMethod]
        public void EditProductTab()
        {
            //Arrange
            var tab = _irepoproduct.GetEditProductTab();
            var tabname = _irepoproduct.GetEditProductTabName();
            var prj = GetRootProduct();

            var tabedit = prj.Tabs.FirstOrDefault(t => t.TabTitle.Equals(tabname));
            if (tabedit == null) Assert.Fail();

            tabedit.TabTitle = tab.TabTitle;
            tabedit.HtmlData = tab.HtmlData;


            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductsUpdateWithSearchRebuild(prj));
        }

        /// <summary>
        /// Deletes the product tab.
        /// </summary>
        //[TestMethod]
        public void DeleteProductTab()
        {
            //Arrange
            var tabname = _irepoproduct.GetDeleteProductTabName();
            var prj = GetRootProduct();
            var tcount = prj.Tabs.Count;
            var tablst = prj.Tabs.Where(x => !x.TabTitle.Equals(tabname)).ToList();
            var newTabs = new List<ProductDescriptionTab>();
            var currentSort = 1;
            foreach (var t in tablst)
            {
                t.SortOrder = currentSort;
                currentSort += 1;
                newTabs.Add(t);
            }


            //Act
            prj.Tabs = newTabs;
            _application.CatalogServices.ProductsUpdateWithSearchRebuild(prj);
            var rcount = prj.Tabs.Count;

            //Assert
            Assert.AreEqual(tcount - 1, rcount);
        }
        #endregion

    }
}
