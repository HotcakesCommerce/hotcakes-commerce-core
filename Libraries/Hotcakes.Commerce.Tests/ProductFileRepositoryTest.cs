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
	public class ProductFileRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductFileRepository _irepoproductfile;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductFileRepositoryTest()
        {
            _irepoproductfile = new XmlProductFileRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductFile_TestInOrder()
        {
            CreateProduct();

            AddProductFile();
            EditProductFile();
            LoadProductFile();
            DeleteProductFile();


            AddAssociatedProduct();
            FindByBvinAndProductBvin();
            FileAlreadyExists();
            FindProductIdsForFile();
            CountOfProductsUsingFile();
            DeleteForProductId();
        }

        #region Product File Load/Add/Delete Test Cases
        /// <summary>
        /// Loads the product file.
        /// </summary>
        //[TestMethod]
        public void LoadProductFile()
        {
            //Arrange
            var count = _irepoproductfile.GetTotalProductFileCount();
            var prj = GetRootProduct();

            //Act
            var resultcount = _application.CatalogServices.ProductFiles.FindByProductId(prj.Bvin);

            //Assert
            Assert.AreEqual(count, resultcount.Count);
        }

        /// <summary>
        /// Loads the aviable files.
        /// </summary>
        [TestMethod]
		public void ProductFile_LoadAvailableFiles()
        {
            //Arrange
            var count = _irepoproductfile.GetTotalAvailableFileCount();

            //Act
            var resultcount = _applicationDB.CatalogServices.ProductFiles.FindAll(1, 1000);
            var resultcount2 = _applicationDB.CatalogServices.ProductFiles.FindAllCount();

            //Assert
            Assert.AreEqual(count, resultcount.Count);
            Assert.AreEqual(count, resultcount2);
        }

        /// <summary>
        /// Deletes the product file.
        /// </summary>
        //[TestMethod]
        public void DeleteProductFile()
        {
            //Arrange
            var prj = GetRootProduct();
            var prjfile = _application.CatalogServices.ProductFiles.FindByProductId(prj.Bvin);

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductFiles.RemoveAssociatedProduct(prjfile.FirstOrDefault().Bvin, prj.Bvin));
        }

        /// <summary>
        /// Adds the product file.
        /// </summary>
        //[TestMethod]
        public void AddProductFile()
        {
            var pfile = _irepoproductfile.GetAddProductFile();
            var prj = GetRootProduct();
            var filelst = new List<ProductFile>();
            foreach (var file in pfile.Select(productFile => _applicationDB.CatalogServices.ProductFiles.FindByFileNameAndDescription(productFile.FileName, productFile.ShortDescription).FirstOrDefault()))
            {
                if (file == null)
                {
                    var newfile = new ProductFile
                        {
                            StoreId = _application.CurrentStore.Id,
                            ProductId = prj.Bvin,
                            MaxDownloads = 0,
                            FileName = "test.jpg",
                            ShortDescription = "test.jpg",
                        };
                    newfile.SetMinutes(2, 2, 2, 4);
                    filelst.Add(newfile);
                }
                else
                {
                    file.StoreId = _application.CurrentStore.Id;
                    file.ProductId = prj.Bvin;
                    file.MaxDownloads = 0;
                    file.SetMinutes(2, 2, 2, 4);
                    filelst.Add(file);
                }
            }

            //Act/Assert
            foreach (var productFile in filelst)
            {
                Assert.IsTrue(_application.CatalogServices.ProductFiles.Create(productFile));
            }
        }

        /// <summary>
        /// Edits the product file.
        /// </summary>
        //[TestMethod]
        public void EditProductFile()
        {
            var pfile = _irepoproductfile.GetAddProductFile();
            var prj = GetRootProduct();
            var l = _application.CatalogServices.ProductFiles.FindByProductId(prj.Bvin);
            var filelst = new List<ProductFile>();
            foreach (var file in pfile.Select(productFile => _application.CatalogServices.ProductFiles.FindByFileNameAndDescription(productFile.FileName, productFile.ShortDescription).FirstOrDefault()))
            {
                file.StoreId = _application.CurrentStore.Id;
                file.ProductId = prj.Bvin;
                file.MaxDownloads = 0;
                file.SetMinutes(2, 2, 2, 4);
                filelst.Add(file);

            }

            //Act/Assert
            foreach (var productFile in filelst)
            {
                Assert.IsTrue(_application.CatalogServices.ProductFiles.Update(productFile));
            }
        }




        /// <summary>
        /// Deletes for product identifier.
        /// </summary>
        //[TestMethod]
        public void DeleteForProductId()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductFiles.DeleteForProductId(prj.Bvin));
            //TODO:Need to change function for delete file to CI
        }

        /// <summary>
        /// Counts the of products using file.
        /// </summary>
        //[TestMethod]
        public void CountOfProductsUsingFile()
        {
            //Arrange
            var prj = GetRootProduct();
            var pfile = _irepoproductfile.GetAddProductFile().FirstOrDefault();

            //Act
            var file = _application.CatalogServices.ProductFiles.FindByFileNameAndDescription(pfile.FileName, pfile.ShortDescription).FirstOrDefault();
            if (file == null) Assert.Fail();
            var count = _application.CatalogServices.ProductFiles.CountOfProductsUsingFile(file.Bvin);


            //Assert
            Assert.AreNotEqual(0, count);
        }

        /// <summary>
        /// Finds the product ids for file.
        /// </summary>
        //[TestMethod]
        public void FindProductIdsForFile()
        {
            //Arrange
            var pfile = _irepoproductfile.GetAddProductFile().FirstOrDefault();

            //Act
            var file = _application.CatalogServices.ProductFiles.FindByFileNameAndDescription(pfile.FileName, pfile.ShortDescription).FirstOrDefault();
            if (file == null) Assert.Fail();
            var count = _application.CatalogServices.ProductFiles.FindProductIdsForFile(file.Bvin).Count;

            //Assert
            Assert.AreNotEqual(0, count);
        }

        /// <summary>
        /// Files the already exists.
        /// </summary>
        //[TestMethod]
        public void FileAlreadyExists()
        {
            //Arrange
            var pfile = _irepoproductfile.GetAddProductFile().FirstOrDefault();

            //Act
            var file = _application.CatalogServices.ProductFiles.FindByFileNameAndDescription(pfile.FileName, pfile.ShortDescription).FirstOrDefault();
            if (file == null) Assert.Fail();

            //Assert
            Assert.IsTrue(_application.CatalogServices.ProductFiles.FileAlreadyExists(file.Bvin));
        }

        /// <summary>
        /// Finds the by bvin and product bvin.
        /// </summary>
        //[TestMethod]
        public void FindByBvinAndProductBvin()
        {
            //Arrange
            var prj = GetRootProduct();
            var pfile = _irepoproductfile.GetAddProductFile().FirstOrDefault();

            //Act
            var file = _application.CatalogServices.ProductFiles.FindByFileNameAndDescription(pfile.FileName, pfile.ShortDescription).FirstOrDefault();
            if (file == null) Assert.Fail();
            var result = _application.CatalogServices.ProductFiles.FindByBvinAndProductBvin(file.Bvin, prj.Bvin);

            //Assert
            Assert.AreNotEqual(null, result);

        }

        /// <summary>
        /// Adds the associated product.
        /// </summary>
        //[TestMethod]
        public void AddAssociatedProduct()
        {
            //Arrange
            var prj = GetRootProduct();
            var pfile = _irepoproductfile.GetAddProductFile().FirstOrDefault();

            //Act
            var file = _application.CatalogServices.ProductFiles.FindByFileNameAndDescription(pfile.FileName, pfile.ShortDescription).Last();
            if (file == null) Assert.Fail();

            //Assert
            Assert.IsTrue(_application.CatalogServices.ProductFiles.AddAssociatedProduct(file.Bvin, prj.Bvin, 1, 0));
        }

        #endregion

    }
}
