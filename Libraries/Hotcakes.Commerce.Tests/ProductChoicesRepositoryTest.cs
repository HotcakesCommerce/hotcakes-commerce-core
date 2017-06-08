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
	public class ProductChoicesRepositoryTest : BaseProductTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductChoicesRepository _irepoproductchoice;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductChoicesRepositoryTest()
        {
            _irepoproductchoice = new XmlProductChoicesRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductChoice_TestInOrder()
        {
            CreateProduct();

            AddProductSharedChoice();
            AddProductChoice();
            EditProductChoice();
            ResortProductOptionItem();
            //LoadProductChoice();
            Find_OP_Many();
            Find_PO_ByProductId();
            FindAll_PO();
            


            Find_PXO_ByProductAndOptionCount();
            Find_PXO_ForOptionCount();
            Find_PXO_ForProductCount();
            FindAll_PXO_PagedCount();
            FindAll_PXO_ForAllStoresCount();
            FindAll_PXO_Count();
            ResortOptionsForProduct();
            DeleteAll_PXO_ForOption();
            DeleteAll_PXO_ForProduct();

            
            MergeProductOption();
            //DeleteProductChoice();
            Delete_PO_ForProductId();


        }

        #region Product Choice Repository Service
        #region Product Choice Load/Add/Edit/Delete Test Cases
        /// <summary>
        /// Loads the product shared choice.
        /// </summary>
        [TestMethod]
		public void ProductChoice_LoadProductSharedChoice()
        {
            //Arrange
            var count = _irepoproductchoice.GetTotalProductSharedChoiceCount();
            var resultcount = _applicationDB.CatalogServices.ProductOptions.FindAllShared(1, int.MaxValue);

            //Act/Assert
            Assert.AreEqual(count, resultcount.Count);
        }

        /// <summary>
        /// Loads the product choice.
        /// </summary>
        //[TestMethod]
        public void LoadProductChoice()
        {
            //Arrange
            var count = _irepoproductchoice.GetTotalProductChoiceCount();
            var prj = GetRootProduct();

            //Act/Assert
            Assert.AreEqual(count, prj.Options.Count);
        }

        /// <summary>
        /// Deletes the product choice.
        /// </summary>
        //[TestMethod]
        public void DeleteProductChoice()
        {
            //Arrange
            var choicecode = _irepoproductchoice.GetDeleteProductChoiceName();
            var prj = GetRootProduct();
            var choice = prj.Options.FirstOrDefault(x => x.OptionType.Equals((OptionTypes)choicecode));
            if (choice == null) Assert.Fail();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductsRemoveOption(prj, choice.Bvin));

        }

        /// <summary>
        /// Adds the product shared choice.
        /// </summary>
        //[TestMethod]
        public void AddProductSharedChoice()
        {
            //Arrange
            var choicename = _irepoproductchoice.GetAddProductSharedChoiceName();
            var option = _applicationDB.CatalogServices.ProductOptions.FindAllShared(1, int.MaxValue).FirstOrDefault(x => x.Name.Equals(choicename));
            var prj = GetRootProduct();

            if (option == null) Assert.Fail();

            var opt = new Option
                {
                    Bvin = option.Bvin,
                    IsShared = true,
                    Name = option.Name,
                    StoreId = _application.CurrentStore.Id,
                };


            //Act/Assert
            // var result = _application.CatalogServices.ProductsAddOption(prj, opt.Bvin); //TODO:Need to change function to check add existing option functionality for CI

            //Alternative code for add existing option for test only
            var result = _application.CatalogServices.ProductsXOptions.AddOptionToProduct(prj.Bvin, opt.Bvin);
            _application.CatalogServices.ProductsReloadOptions(prj);
            if (opt.IsVariant)
                _application.CatalogServices.VariantsValidate(prj);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Adds the product chilce.
        /// </summary>
        //[TestMethod]
        public void AddProductChoice()
        {
            //Arrange
            var typecode = _irepoproductchoice.GetAddProductChoiceTypeCode();
            var prj = GetRootProduct();

            foreach (var code in typecode)
            {
                var opt = new Option
                {
                    StoreId = _application.CurrentStore.Id,
                    IsShared = false,
                    IsVariant = false,
                    Name = "New Choice",
                };
                opt.SetProcessor((OptionTypes)code);

                switch (opt.OptionType)
                {
                    case OptionTypes.CheckBoxes:
                        opt.Name = "New Checkboxes";
                        break;
                    case OptionTypes.DropDownList:
                        opt.Name = "New Drop Down List";
                        break;
                    case OptionTypes.FileUpload:
                        opt.Name = "New File Upload";
                        break;
                    case OptionTypes.Html:
                        opt.Name = "New Html Description";
                        break;
                    case OptionTypes.RadioButtonList:
                        opt.Name = "New Radio Button List";
                        break;
                    case OptionTypes.TextInput:
                        opt.Name = "New Text Input";
                        break;
                }
                opt.StoreId = _application.CurrentStore.Id;

                prj.Options.Add(opt);

                //Act/Assert
                Assert.IsTrue(_application.CatalogServices.Products.Update(prj));
                Assert.IsTrue(_application.CatalogServices.ProductsAddOption(prj, opt.Bvin));
            }
        }

        /// <summary>
        /// Edits the product choice.
        /// </summary>
        //[TestMethod]
        public void EditProductChoice()
        {
            //Arrange
            var choicecode = _irepoproductchoice.GetEditProductChoiceName();
            var prj = GetRootProduct();
            var choice = _irepoproductchoice.GetEditProductChoice();

            foreach (var code in choicecode)
            {
                var choiceedit = prj.Options.FirstOrDefault(x => x.OptionType.Equals((OptionTypes)code));
                if (choiceedit == null) Assert.Fail();

                choiceedit.Name = choice.Name;
                choiceedit.NameIsHidden = choice.NameIsHidden;
                choiceedit.IsVariant = false;
                choiceedit.IsColorSwatch = choice.IsColorSwatch;
                if (code != 100 && code != 200 && code != 300)
                    _irepoproductchoice.SetProductChoiceInfo(ref choiceedit);

                if (code == 100 || code == 200)
                    choiceedit.IsVariant = choice.IsVariant;


                //Act/Assert
                //Add option
                _irepoproductchoice.AddProductChoiceItem(ref choiceedit);

                Assert.IsTrue(_application.CatalogServices.ProductOptions.Update(choiceedit));

                //Edit option
                _irepoproductchoice.EditProductChoiceItem(ref choiceedit);

                Assert.IsTrue(_application.CatalogServices.ProductOptions.Update(choiceedit));

                //Delete option
                _irepoproductchoice.DeleteProductChoiceItem(ref choiceedit);

                Assert.IsTrue(_application.CatalogServices.ProductOptions.Update(choiceedit));
            }

            //Assert
            Assert.IsTrue(_application.CatalogServices.Products.Update(prj));

        }

        #endregion
        #region Product Choice Repo Functions Test Cases

        #region Product OptionX
        /// <summary>
        /// Finds all count.
        /// </summary>
        //[TestMethod]
        public void FindAll_PXO_Count()
        {
            //Arrange
            var count = _irepoproductchoice.FindAll_PXO_Count();
            //Act
            var resultcount = _application.CatalogServices.ProductsXOptions.FindAll().Count;
            //Assert
            Assert.AreEqual(count, resultcount);

        }
        /// <summary>
        /// Finds all for all stores count.
        /// </summary>
        //[TestMethod]
        public void FindAll_PXO_ForAllStoresCount()
        {
            //Arrange
            var count = _irepoproductchoice.FindAll_PXO_ForAllStoresCount();
            //Act
            var resultcount = _application.CatalogServices.ProductsXOptions.FindAllForAllStores().Count;
            //Assert
            Assert.AreEqual(count, resultcount);
        }
        /// <summary>
        /// Finds all paged count.
        /// </summary>
        //[TestMethod]
        public void FindAll_PXO_PagedCount()
        {
            //Arrange
            var count = _irepoproductchoice.FindAll_PXO_PagedCount();
            //Act
            var resultcount = _application.CatalogServices.ProductsXOptions.FindAllPaged(1, int.MaxValue).Count;
            //Assert
            Assert.AreEqual(count, resultcount);
        }
        /// <summary>
        /// Finds for product count.
        /// </summary>
        //[TestMethod]
        public void Find_PXO_ForProductCount()
        {
            //Arrange
            var count = _irepoproductchoice.Find_PXO_ForProductCount();
            var prj = GetRootProduct();
            //Act
            var resultcount = _application.CatalogServices.ProductsXOptions.FindForProduct(prj.Bvin, 1, int.MaxValue).Count;
            //Assert
            Assert.AreEqual(count, resultcount);
        }
        /// <summary>
        /// Finds for option count.
        /// </summary>
        //[TestMethod]
        public void Find_PXO_ForOptionCount()
        {
            //Arrange
            var count = _irepoproductchoice.Find_PXO_ForOptionCount();
            var prj = GetRootProduct();
            var opname = _irepoproductchoice.GetOptionName();
            var option = prj.Options.FirstOrDefault(x => x.Name.Equals(opname));
            if (option == null) Assert.Fail();
            //Act
            var resultcount = _application.CatalogServices.ProductsXOptions.FindForOption(option.Bvin, 1, int.MaxValue).Count;
            //Assert
            Assert.AreEqual(count, resultcount);
        }
        /// <summary>
        /// Finds the by product and option count.
        /// </summary>
        //[TestMethod]
        public void Find_PXO_ByProductAndOptionCount()
        {
            //Arrange
            var prj = GetRootProduct();
            var opname = _irepoproductchoice.GetOptionName();
            var option = prj.Options.FirstOrDefault(x => x.Name.Equals(opname));
            if (option == null) Assert.Fail();
            //Act
            var result = _application.CatalogServices.ProductsXOptions.FindByProductAndOption(prj.Bvin, option.Bvin);
            //Assert
            Assert.AreEqual(option.Bvin, result.OptionBvin);
        }

        /// <summary>
        /// Deletes all for product.
        /// </summary>
        //[TestMethod]
        public void DeleteAll_PXO_ForProduct()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductsXOptions.DeleteAllForProduct(prj.Bvin));
        }

        /// <summary>
        /// Deletes all for option.
        /// </summary>
        //[TestMethod]
        public void DeleteAll_PXO_ForOption()
        {
            //Arrange
            var prj = GetRootProduct();
            var opname = _irepoproductchoice.GetOptionName();
            var option = prj.Options.FirstOrDefault(x => x.Name.Equals(opname));
            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductsXOptions.DeleteAllForOption(option.Bvin));
        }

        /// <summary>
        /// Resorts the options for product.
        /// </summary>
        //[TestMethod]
        public void ResortOptionsForProduct()
        {
            //Arrange
            var prj = GetRootProduct();
            var option = prj.Options.Select(x => x.Bvin).ToList();
            option.Reverse();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductsXOptions.ResortOptionsForProduct(prj.Bvin, option));
        }
        #endregion

        #region Product Option

        /// <summary>
        /// Finds product option all.
        /// </summary>
        //[TestMethod]
        public void FindAll_PO()
        {
            //Arrange
            var count = _irepoproductchoice.FindAll_PO_Count();
            var resultcount = _application.CatalogServices.ProductOptions.FindAll(1, int.MaxValue).Count;

            //Act/Assert
            Assert.AreEqual(count, resultcount);
        }

        /// <summary>
        /// Delete product option for product identifier.
        /// </summary>
        //[TestMethod]
        public void Delete_PO_ForProductId()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductOptions.DeleteForProductId(prj.Bvin));
        }

        /// <summary>
        /// Find product option by product identifier.
        /// </summary>
        //[TestMethod]
        public void Find_PO_ByProductId()
        {
            //Arrange
            var count = _irepoproductchoice.Find_PO_ByProductIdCount();
            var prj = GetRootProduct();
            var resultcount = _application.CatalogServices.ProductOptions.FindByProductId(prj.Bvin).Count;

            //Act/Assert
            Assert.AreEqual(count, resultcount);
        }


        /// <summary>
        /// Merges the product option.
        /// </summary>
        //[TestMethod]
        public void MergeProductOption()
        {
            //Arrange
            var prj = GetRootProduct();
            var lst = _irepoproductchoice.GetProductOptionList();
            var oplst = new OptionList();
            foreach (var op in lst)
            {
                var obj = prj.Options.FirstOrDefault(x => x.Name.Equals(op));
                if (obj == null)
                {
                    var op1 = new Option
                        {
                            StoreId = _application.CurrentStore.Id,
                            IsShared = false,
                            IsVariant = false,
                            Name = op,
                        };
                    op1.SetProcessor((OptionTypes)200);
                    oplst.Add(op1);
                }
                else
                    oplst.Add(obj);
            }

            //Act
            _application.CatalogServices.ProductOptions.MergeList(prj.Bvin, oplst);
            //Assert
            var prj1 = GetRootProduct();
            Assert.AreEqual(lst.Count, prj1.Options.Count);
        }

        /// <summary>
        /// Find product option many.
        /// </summary>
        //[TestMethod]
        public void Find_OP_Many()
        {
            //Arrange
            var count = _irepoproductchoice.FindMany_PO_Count();
            var prj = GetRootProduct();
            var opids = prj.Options.Select(x => x.Bvin).ToList();
            var resultcount = _application.CatalogServices.ProductOptions.FindMany(opids).Count;

            //Act/Assert
            Assert.AreEqual(count, resultcount);
        }

        #endregion

        #region Product OptionItems

        /// <summary>
        /// Resorts the product option item.
        /// </summary>
        //[TestMethod]
        public void ResortProductOptionItem()
        {
            //Arrange
            var prj = GetRootProduct();
            var items = prj.Options.FirstOrDefault().Items.Select(x => x.Bvin).ToList();
            items.Reverse();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductOptions.ResortOptionItems(prj.Bvin, items));
        }

        #endregion

        #endregion
        #endregion


    }
}
