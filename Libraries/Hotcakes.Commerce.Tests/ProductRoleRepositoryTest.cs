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
