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
	public class ProductVolumeDiscountRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductVolumeDiscountRepository _irepoproductvdiscount;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductVolumeDiscountRepositoryTest()
        {
            _irepoproductvdiscount = new XmlProductVolumeDiscountRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductVolumeDiscount_TestInOrder()
        {
            CreateProduct();
           
            AddProductVolumeDiscount();
            EditProductVolumeDiscount();
            LoadProductVolumeDiscount();
            DeleteProductVolumeDiscount();
            DeleteProductAllVolumeDiscount();
        }

        #region Product Volume Discount Load/Add/Edit/Delete Test Cases
        /// <summary>
        /// Loads the product volume discount.
        /// </summary>
        //[TestMethod]
        public void LoadProductVolumeDiscount()
        {
            //Arrange
            var count = _irepoproductvdiscount.GetTotalProduct_VD_Count();
            var prj = GetRootProduct();

            //Act
            var resultcount = _application.CatalogServices.VolumeDiscounts.FindByProductId(prj.Bvin);

            //Assert
            Assert.AreEqual(count, resultcount.Count);
        }

        /// <summary>
        /// Adds the product volume discount.
        /// </summary>
        //[TestMethod]
        public void AddProductVolumeDiscount()
        {
            //Arrange
            var vd = _irepoproductvdiscount.GetAddProduct_VD();
            var prj = GetRootProduct();
            vd.ProductId = prj.Bvin;

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.VolumeDiscounts.Create(vd));

        }

        /// <summary>
        /// Edits the product volume discount.
        /// </summary>
        //[TestMethod]
        public void EditProductVolumeDiscount()
        {
            //Arrange
            var vd = _irepoproductvdiscount.GetEditProduct_VD();
            var prj = GetRootProduct();
            var resultvd = _application.CatalogServices.VolumeDiscounts.FindByProductId(prj.Bvin).FirstOrDefault(x => x.Qty.Equals(vd.Qty));
            if (resultvd == null) Assert.Fail();
            resultvd.Amount = vd.Amount;

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.VolumeDiscounts.Update(resultvd));
        }

        /// <summary>
        /// Deletes the product volume discount.
        /// </summary>
        //[TestMethod]
        public void DeleteProductVolumeDiscount()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act
            var result = _application.CatalogServices.VolumeDiscounts.FindByProductId(prj.Bvin).OrderBy(x => x.LastUpdated).FirstOrDefault();
            if (result == null) return;
            var vdiscount = _application.CatalogServices.VolumeDiscounts.Find(result.Bvin);
            if (vdiscount == null) Assert.Fail();

            //Assert
            Assert.IsTrue(_application.CatalogServices.VolumeDiscounts.Delete(vdiscount.Bvin));
        }

        /// <summary>
        /// Deletes the product volume discount.
        /// </summary>
        //[TestMethod]
        public void DeleteProductAllVolumeDiscount()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.VolumeDiscounts.DeleteByProductId(prj.Bvin));
        }

        #endregion
       
    }
}
