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
	public class RelatedProductRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlRelatedProductRepository _ireporelatedproduct;

        /// <summary>
        /// Initializes.
        /// </summary>
        public RelatedProductRepositoryTest()
        {
            _ireporelatedproduct = new XmlRelatedProductRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void RelatedProduct_TestInOrder()
        {
            CreateProduct();

            AddRelatedProduct();
            LoadRelatedProduct();
            ReSortRelatedProduct();
            FindRelatedProductById();
            FindRelatedProductByIdAndRelatedId();
            DeleteRelatedProduct();
            DeleteRelatedAllProduct();

        }

        #region Product Related Load/Add/Delete Test Cases
        /// <summary>
        /// Loads the related product.
        /// </summary>
        //[TestMethod]
        public void LoadRelatedProduct()
        {
            //Arrange
            var count = _ireporelatedproduct.GetTotalRelatedProductCount();
            var prj = GetRootProduct();
            var resultcount = _application.CatalogServices.ProductRelationships.FindForProduct(prj.Bvin);


            //Act/Assert
            Assert.AreEqual(count, resultcount.Count);
        }

        /// <summary>
        /// Loads the product search.
        /// </summary>
        [TestMethod]
		public void RelatedProduct_LoadProductSearch()
        {
            //Arrange
            var prjsearch = _ireporelatedproduct.GetTotalProductCount();
            var searchprm = prjsearch.FirstOrDefault().Value;
            var totalCount = 0;
            searchprm = SetProductSearchCriteria(searchprm);

            //Act
            _applicationDB.CatalogServices.Products.FindByCriteria(searchprm, 1, 10, ref totalCount);

            //Assert
            Assert.AreEqual(prjsearch.FirstOrDefault().Key, totalCount);
        }

        /// <summary>
        /// Adds the related product.
        /// </summary>
        //[TestMethod]
        public void AddRelatedProduct()
        {
            //Arrange
            var lstprj = _ireporelatedproduct.GetAddRelatedProduct();
            var prj = GetRootProduct();


            //Act/Assert
            foreach (var tempprj in lstprj.Select(product => _applicationDB.CatalogServices.Products.FindBySku(product)).Where(tempprj => tempprj != null))
            {
                Assert.IsTrue(_application.CatalogServices.ProductRelationships.RelateProducts(prj.Bvin, tempprj.Bvin, false));
            }

        }

        /// <summary>
        /// Deletes the related product.
        /// </summary>
        //[TestMethod]
        public void DeleteRelatedProduct()
        {
            //Arrange
            var lstprj = _ireporelatedproduct.GetDeleteRelatedProduct();
            var prj = GetRootProduct();

            var prjrelatedlst = _application.CatalogServices.ProductRelationships.FindForProduct(prj.Bvin);
            var tmpprj = _applicationDB.CatalogServices.Products.FindManySkus(lstprj);

            var prjdel = (from p1 in prjrelatedlst
                          join p2 in tmpprj on p1.RelatedProductId equals p2.Bvin
                          select p1).ToList();

            //Act/Assert
            foreach (var tempprj in prjdel)
            {
                Assert.IsTrue(_application.CatalogServices.ProductRelationships.UnrelateProducts(prj.Bvin, tempprj.RelatedProductId));
                //Assert.IsTrue(_application.CatalogServices.ProductRelationships.Delete(tempprj.Id));
            }
        }

        /// <summary>
        /// Deletes the related product.
        /// </summary>
        //[TestMethod]
        public void DeleteRelatedAllProduct()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductRelationships.DeleteAllForProduct(prj.Bvin));
        }

        /// <summary>
        /// Res the sort related product.
        /// </summary>
        //[TestMethod]
        public void ReSortRelatedProduct()
        {
            //Arrange
            var prj = GetRootProduct();
            var lstrelatedprj = _application.CatalogServices.ProductRelationships.FindForProduct(prj.Bvin).Select(x => x.RelatedProductId).ToList();
            lstrelatedprj.Reverse();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductRelationships.ResortRelationships(prj.Bvin, lstrelatedprj));
        }

        /// <summary>
        /// Finds the related product by identifier.
        /// </summary>
        //[TestMethod]
        public void FindRelatedProductById()
        {
            //Arrange
            var prj = GetRootProduct();
            var prjrelated = _application.CatalogServices.ProductRelationships.FindForProduct(prj.Bvin).FirstOrDefault();
            if (prjrelated == null) return;

            //Act/Assert
            Assert.AreEqual(prjrelated.RelatedProductId, _application.CatalogServices.ProductRelationships.Find(prjrelated.Id).RelatedProductId);
        }

        /// <summary>
        /// Finds the related product by identifier.
        /// </summary>
        //[TestMethod]
        public void FindRelatedProductByIdAndRelatedId()
        {
            //Arrange
            var prj = GetRootProduct();
            var prjrelated = _application.CatalogServices.ProductRelationships.FindForProduct(prj.Bvin).FirstOrDefault();
            if (prjrelated == null) return;

            //Act/Assert
            Assert.AreEqual(prjrelated.RelatedProductId, _application.CatalogServices.ProductRelationships.FindByProductAndRelated(prj.Bvin, prjrelated.RelatedProductId).RelatedProductId);
        }

        #endregion

    }
}
