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
	public class ProductRoleRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductRoleRepository _irepoproductrole;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductRoleRepositoryTest()
        {
            _irepoproductrole = new XmlProductRoleRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductRole_TestInOrder()
        {
            CreateProduct();
            

            AddProductRole();
            TotalProductRoleCount();
            DeleteProductRole();
            
        }

       
        #region Product Role Load/Add/Delete
        /// <summary>
        /// Adds the role.
        /// </summary>
        //[TestMethod]
        public void AddProductRole()
        {
            //Arrange
            var rolename = _irepoproductrole.GetAddProductRole();
            var prj = GetRootProduct();
            var role = new CatalogRole
                {
                    RoleName = rolename,
                    ReferenceId = new Guid(prj.Bvin),
                    RoleType = CatalogRoleType.ProductRole
                };


            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.CatalogRoles.Create(role));
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        //[TestMethod]
        public void DeleteProductRole()
        {
            //Arrange
            var rolename = _irepoproductrole.GetDeleteProductRole();
            var prj = GetRootProduct();

            var role = _application.CatalogServices.CatalogRoles.FindByProductId(new Guid(prj.Bvin)).FirstOrDefault(x => x.RoleName.Equals(rolename));

            //Act
            _application.CatalogServices.CatalogRoles.Delete(role.CatalogRoleId);
            var resultrole = _application.CatalogServices.CatalogRoles.FindByCategoryId(new Guid(prj.Bvin)).FirstOrDefault(x => x.RoleName.Equals(rolename));


            //Assert
            Assert.AreEqual(null, resultrole);

        }

        /// <summary>
        /// Totals the product role count.
        /// </summary>
        //[TestMethod]
        public void TotalProductRoleCount()
        {
            //Arrange
            var count = _irepoproductrole.GetTotalProductRoleCount();
            var prj = GetRootProduct();

            //Act
            var resultcount = _application.CatalogServices.CatalogRoles.FindByProductId(new Guid(prj.Bvin));

            //Assert
            Assert.AreEqual(count, resultcount.Count);
        }
        #endregion

       }
}
