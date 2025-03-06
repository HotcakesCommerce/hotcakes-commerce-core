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
    public class ProductVariantRepositoryTest : ProductChoicesRepositoryTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlProductVariantRepository _irepoproductvariant;

        /// <summary>
        /// Initializes.
        /// </summary>
        public ProductVariantRepositoryTest()
        {
            _irepoproductvariant = new XmlProductVariantRepository();
        }

        /// <summary>
        /// Tests the in order.
        /// </summary>
        [TestMethod]
		public void ProductVariant_TestInOrder()
        {
            CreateProduct();
            
            AddProductChoice();
            EditProductChoice();
           
            AddProductVariant();
            EditProductVariant();
            AddPossibleProductVariant();
            LoadProductVariantOption();
            LoadProductVariant();
            MergeProductVariant();
            DeleteProductVariant();
            DeleteVariantForProductId();
           
        }

       #region Product Variant Load/Add/Edit/Delete Test Cases
        /// <summary>
        /// Loads the product variant option.
        /// </summary>
        //[TestMethod]
        public void LoadProductVariantOption()
        {
            //Arrange
            var count = _irepoproductvariant.GetTotalProduct_VO_Count();
            var prj = GetRootProduct();

            //Act/Assert
            Assert.AreEqual(count, prj.Options.VariantsOnly().Count);
        }

        /// <summary>
        /// Loads the product variant.
        /// </summary>
        //[TestMethod]
        public void LoadProductVariant()
        {
            //Arrange
            var count = _irepoproductvariant.GetTotalProduct_VO_Count();
            var prj = GetRootProduct();

            //Act
            var possibleVariants = _application.CatalogServices.VariantsGenerateAllPossibleSelections(prj.Options);
            var lstv = new List<Variant>();
            foreach (var possible in possibleVariants)
            {
                var possibleKey = OptionSelection.GenerateUniqueKeyForSelections(possible);
                var v = prj.Variants.FindByKey(possibleKey);
                if (v != null)
                    lstv.Add(v);
            }

            //Assert
            Assert.AreEqual(count, lstv.Count);

        }

        /// <summary>
        /// Deletes the product variant.
        /// </summary>
        //[TestMethod]
        public void DeleteProductVariant()
        {
            //Arrange
            var prj = GetRootProduct();

            var v = prj.Variants.FirstOrDefault();
            if (v == null) return;

            var pvariant = _application.CatalogServices.ProductVariants.Find(v.Bvin);
            if (pvariant == null) Assert.Fail();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductVariants.Delete(pvariant.Bvin));
            //var prj1 = GetRootProduct();
            //Assert.IsFalse(prj1.HasVariants());
        }


        /// <summary>
        /// Adds the product variant.
        /// </summary>
        //[TestMethod]
        public void AddProductVariant()
        {
            //Arrange
            var prj = GetRootProduct();
            var selections = new List<OptionSelection>();
            var variantOptions = prj.Options.VariantsOnly();
            if (variantOptions.Count > 0)
            {
                selections.AddRange(variantOptions.Select(x => new OptionSelection
                {
                    OptionBvin = x.Bvin,
                    SelectionData = x.Items.FirstOrDefault().Name,
                }).ToList());
            }

            if (selections.Count != variantOptions.Count) Assert.Fail();

            //Act

            var v = new Variant { ProductId = prj.Bvin };
            v.Selections.AddRange(selections);
            Assert.IsTrue(_application.CatalogServices.ProductVariants.Create(v));
        }

        /// <summary>
        /// Edits the product variant.
        /// </summary>
        //[TestMethod]
        public void EditProductVariant()
        {
            //Arrange
            var prj = GetRootProduct();
            var variant = _irepoproductvariant.GetEditProductVariant();

            var editvariant = _application.CatalogServices.ProductVariants.FindByProductId(prj.Bvin).FirstOrDefault();
            if (editvariant == null) Assert.Fail();
            editvariant.Sku = variant.Sku;
            editvariant.Price = variant.Price;

            //Assert
            Assert.IsTrue(_application.CatalogServices.ProductVariants.Update(editvariant));

        }

        /// <summary>
        /// Adds the possible product variant.
        /// </summary>
        //[TestMethod]
        public void AddPossibleProductVariant()
        {
            //Arrange
            var prj = GetRootProduct();
            _application.CatalogServices.VariantsGenerateAllPossible(prj);
            var count = _irepoproductvariant.GetTotalProductVariantCount();

            //Act
            var possibleVariants = _application.CatalogServices.VariantsGenerateAllPossibleSelections(prj.Options);
            var lstv = new List<Variant>();
            var prj1 = GetRootProduct();
            foreach (var possible in possibleVariants)
            {
                var possibleKey = OptionSelection.GenerateUniqueKeyForSelections(possible);
                var v = prj1.Variants.FindByKey(possibleKey);
                if (v != null)
                    lstv.Add(v);
            }


            //Assert
            Assert.AreEqual(count, lstv.Count);
        }

        /// <summary>
        /// Deletes the product variant.
        /// </summary>
        //[TestMethod]
        public void DeleteVariantForProductId()
        {
            //Arrange
            var prj = GetRootProduct();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.ProductVariants.DeleteForProductId(prj.Bvin));
        }

        /// <summary>
        /// Merges the product variant.
        /// </summary>
        //[TestMethod]
        public void MergeProductVariant()
        {
            #region Arrange
            //Arrange
            var prj = GetRootProduct();
            var selections = new List<OptionSelection>();
            var variant = _irepoproductvariant.GetEditProductVariant();
            var count = prj.Variants.Count;
            var editvariant = _application.CatalogServices.ProductVariants.FindByProductId(prj.Bvin).FirstOrDefault();
            if (editvariant == null) Assert.Fail();
            editvariant.Sku = variant.Sku;
            editvariant.Price = variant.Price;

            var variantOptions = prj.Options.VariantsOnly();
            if (variantOptions.Count > 0)
            {
                selections.AddRange(variantOptions.Select(x => new OptionSelection
                {
                    OptionBvin = x.Bvin,
                    SelectionData = x.Items.Last().Name,
                }).ToList());
            }

            if (selections.Count != variantOptions.Count) Assert.Fail();
            #endregion

            //Act
            var v = new Variant { ProductId = prj.Bvin };
            v.Selections.AddRange(selections);
            var lstvariant = new List<Variant> { v, editvariant };
            _application.CatalogServices.ProductVariants.MergeList(prj.Bvin, lstvariant);

            var prj1 = GetRootProduct();
            var resultcount = prj1.Variants.Count;

            //Assert
            Assert.AreEqual(count + 1, resultcount);
        }

        #endregion

    }
}
