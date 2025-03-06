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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Tests.IRepository;
using Hotcakes.Commerce.Tests.XmlRepository;
using Hotcakes.Commerce.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.Commerce.Tests
{
    /// <summary>
    /// Summary description for CategoryRepositoryTest
    /// </summary>
    [TestClass]
    public class CategoryRepositoryTest : BaseTest
    {
        /// <summary>
        /// The _irepocategory
        /// </summary>
        private IXmlCategoryRepository _irepocategory;

        /// <summary>
        /// Initializes.
        /// </summary>
        public CategoryRepositoryTest()
        {
            _irepocategory = new XmlCategoryRepository();
        }

        [TestMethod]
		public void Category_TestInOrder()
        {
            CreateCategory();
            CreateChildCategory();
            AddLink();
            AddChildLink();

            AddCategoryProduct();
            LoadCategoryProduct();
            DeleteCategoryProduct();
            DeleteAllForCategory();

            AddRole();
            TotalCategoryRoleCount();
            DeleteRole();

            EditCategory();
            EditLink();

            DeleteCategory();
            DeleteLink();

            //DeleteAllCategory();
        }


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Loads the display template.
        /// </summary>
        [TestMethod]
		public void Category_LoadDisplayTemplate()
        {
            //Arrange
            var count = _irepocategory.GetTotalDisplayTemplateCount();

            //Act
            var result = DnnPathHelper.GetViewNames("Category");
            //TODO:Need to change DnnCustomerAccountRepository to set fake portalId

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the total category.
        /// </summary>
        [TestMethod]
		public void Category_LoadTotalCategory()
        {
            //Arrange
            var count1 = _irepocategory.GetTotalCategoryCount();
            var count2 = _irepocategory.GetTotalCategoryChildCount();

            //Act
            var result1 = _applicationDB.CatalogServices.Categories.FindAllSnapshotsPaged(1, int.MaxValue);
            //var result1 = _application.CatalogServices.Categories.FindAllSnapshotsPaged(1, int.MaxValue);
            var pid = string.Empty;
            var catlst = result1.FirstOrDefault(x => x.Name.Equals(count2.FirstOrDefault().Key));
            if (catlst != null)
                pid = catlst.Bvin;
            var result2 = Category.FindChildrenInList(result1, pid, true);

            //Assert
            Assert.AreEqual(count1, result1.Count);
            Assert.AreEqual(count2, result2.Count);
        }


        /// <summary>
        /// Loads the vendoe.
        /// </summary>
        [TestMethod]
		public void Category_LoadVendoe()
        {
            //Arrange
            var count = _irepocategory.GetTotalVendorCount();

            //Act
            var result = _applicationDB.ContactServices.Vendors.FindAll();

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the manufacturer.
        /// </summary>
        [TestMethod]
		public void Category_LoadManufacturer()
        {
            //Arrange
            var count = _irepocategory.GetTotalManufacturerCount();

            //Act
            var result = _applicationDB.ContactServices.Manufacturers.FindAll();

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Loads the columns.
        /// </summary>
        [TestMethod]
		public void Category_LoadColumns()
        {
            //Arrange
            var count = _irepocategory.GetTotalPopulateColumnsCount();

            //Act
            var result = _applicationDB.ContentServices.Columns.FindAll();

            //Assert
            Assert.AreEqual(count, result.Count);
        }

        /// <summary>
        /// Creates the category.
        /// </summary>
        //[TestMethod]
        public void CreateCategory()
        {
            //Arange
            var cat = _irepocategory.GetAddCategory();
            var column = _applicationDB.ContentServices.Columns.FindAll();

            var col1 = column.FirstOrDefault(x => x.DisplayName.Equals(cat.PreContentColumnId));
            var col2 = column.FirstOrDefault(x => x.DisplayName.Equals(cat.PostContentColumnId));
            var newcat = cat;
            {
                newcat.SourceType = CategorySourceType.Manual;
                newcat.PreContentColumnId = col1 == null ? string.Empty : col1.Bvin;
                newcat.PostContentColumnId = col2 == null ? string.Empty : col2.Bvin;
                newcat.RewriteUrl = Hotcakes.Web.Text.Slugify(cat.RewriteUrl, true);
            }

            if (UrlRewriter.IsCategorySlugInUse(newcat.RewriteUrl, newcat.Bvin, _application))
                Assert.Fail();

            //Act
            var result = _application.CatalogServices.Categories.Create(newcat);
            if (result)
                result = _application.CatalogServices.Categories.SubmitChanges();
            //TODO: Need to change EnsureJournalType function for CI
            var taxonomyTags = _irepocategory.GetCategoryTaxonomy().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var resultcat = _application.CatalogServices.Categories.FindMany(newcat.Name).FirstOrDefault();

            if (resultcat == null) Assert.Fail();
           _application.SocialService.UpdateCategoryTaxonomy(resultcat, taxonomyTags);
            var resulttex = _application.SocialService.GetTaxonomyTerms(resultcat);
            //TODO:Need to confirm taxonomy update functionality not implemented 

            //Assert
            Assert.IsTrue(result);
            //Assert.AreEqual(taxonomyTags.Count(), resulttex.Count());

        }

        /// <summary>
        /// Creates the category.
        /// </summary>
        //[TestMethod]
        public void CreateChildCategory()
        {
            //Arange
            var oldcatname = _irepocategory.GetEditCategoryName();
            var pcat = _application.CatalogServices.Categories.FindMany(oldcatname).FirstOrDefault();
            if (pcat == null) Assert.Fail();
            var pcatid = pcat.Bvin;
            var cat = _irepocategory.GetAddChildCategory();
            var column = _applicationDB.ContentServices.Columns.FindAll();
            var col1 = column.FirstOrDefault(x => x.DisplayName.Equals(cat.PreContentColumnId));
            var col2 = column.FirstOrDefault(x => x.DisplayName.Equals(cat.PostContentColumnId));
            var newcat = cat;
            {
                newcat.SourceType = CategorySourceType.Manual;
                newcat.PreContentColumnId = col1 == null ? string.Empty : col1.Bvin;
                newcat.PostContentColumnId = col2 == null ? string.Empty : col2.Bvin;
                newcat.RewriteUrl = Hotcakes.Web.Text.Slugify(cat.RewriteUrl, true);
                newcat.ParentId = pcatid;
            }

            if (UrlRewriter.IsCategorySlugInUse(newcat.RewriteUrl, newcat.Bvin, _application))
                Assert.Fail();

            //Act
            var result = _application.CatalogServices.Categories.Create(newcat);
            if (result)
                result = _application.CatalogServices.Categories.SubmitChanges();
            var taxonomyTags = _irepocategory.GetCategoryTaxonomy().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            var resultcat = _application.CatalogServices.Categories.FindMany(newcat.Name).FirstOrDefault();

            if (resultcat == null) Assert.Fail();
            _application.SocialService.UpdateCategoryTaxonomy(newcat, taxonomyTags);
            var resulttex =_application.SocialService.GetTaxonomyTerms(resultcat);
            //TODO:Need to confirm taxonomy update functionality not implemented 

            //Assert
            Assert.IsTrue(result);
            //Assert.AreEqual(taxonomyTags.Count(), resulttex.Count());

        }

        /// <summary>
        /// Edits the category.
        /// </summary>
        //[TestMethod]
        public void EditCategory()
        {
            //Arange
            var oldcatname = _irepocategory.GetEditCategoryName();
            var cat = _irepocategory.GetEditCategory();
            var column = _applicationDB.ContentServices.Columns.FindAll();
            var col1 = column.FirstOrDefault(x => x.DisplayName.Equals(cat.PreContentColumnId));
            var col2 = column.FirstOrDefault(x => x.DisplayName.Equals(cat.PostContentColumnId));
            var edit = _application.CatalogServices.Categories.FindMany(oldcatname).FirstOrDefault();
            {
                edit.PreContentColumnId = col1 == null ? string.Empty : col1.Bvin;
                edit.PostContentColumnId = col2 == null ? string.Empty : col2.Bvin;
                edit.RewriteUrl = Hotcakes.Web.Text.Slugify(cat.RewriteUrl, true);
                edit.Name = cat.Name;
                edit.MetaDescription = cat.MetaDescription;
                edit.MetaTitle = cat.MetaTitle;
                edit.MetaKeywords = cat.MetaKeywords;
                edit.ShowInTopMenu = cat.ShowInTopMenu;
                edit.Hidden = cat.Hidden;
                edit.TemplateName = cat.TemplateName;
                edit.DisplaySortOrder = cat.DisplaySortOrder;
                edit.ShowTitle = cat.ShowTitle;
                edit.Keywords = cat.Keywords;
            }

            if (UrlRewriter.IsCategorySlugInUse(edit.RewriteUrl, edit.Bvin, _application))
                Assert.Fail();

            //Act
            var result = _application.CatalogServices.Categories.Update(edit);
            if (result)
                result = _application.CatalogServices.Categories.SubmitChanges();
            //TODO: Need to change EnsureJournalType function for CI
            if (result)
                _application.ContentServices.CustomUrls.Register301(cat.RewriteUrl, edit.RewriteUrl,
                                                    edit.Bvin, CustomUrlType.Category, _application.CurrentRequestContext, _application);


            var resultcat = _application.CatalogServices.Categories.Find(edit.Bvin);
            if (resultcat == null) Assert.Fail();


            //Assert
            Assert.IsTrue(result);
            Assert.AreEqual(edit.RewriteUrl, resultcat.RewriteUrl);

        }

        /// <summary>
        /// Deletes the category.
        /// </summary>
        //[TestMethod]
        public void DeleteCategory()
        {
            //Arrange
            var catname = _irepocategory.GetDeleteCategory();
            var cat = _application.CatalogServices.Categories.FindMany(catname).FirstOrDefault();

            //Act/Assert
            Assert.IsTrue(_application.DestroyCategory(cat.Bvin));
        }

        /// <summary>
        /// Deletes all category.
        /// </summary>
        //[TestMethod]
        public void DeleteAllCategory()
        {
            //Arrange
            var catname = _irepocategory.GetDeleteCategory();
            var cat = _application.CatalogServices.Categories.FindMany(catname).FirstOrDefault();

            //Act
            //_application.DestroyAllCategoriesForStore(_application.CurrentStore.Id) TODO:Need to change function

            var result = _application.CatalogServices.Categories.Find(cat.Bvin);

            //Assert
            Assert.AreEqual(null, result);
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        //[TestMethod]
        public void AddLink()
        {
            //Arange
            var cat = _irepocategory.GetAddCustomLink();
            var newcat = cat;
            {
                newcat.SourceType = CategorySourceType.CustomLink;
            }

            //Act
            var result = _application.CatalogServices.Categories.Create(newcat);
            if (result)
                result = _application.CatalogServices.Categories.SubmitChanges();
            //TODO: Need to change EnsureJournalType function for CI
            //Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Adds the child link.
        /// </summary>
        //[TestMethod]
        public void AddChildLink()
        {
            //Arange
            var oldcatname = _irepocategory.GetCustomLinkName();
            var pcat = _application.CatalogServices.Categories.FindMany(oldcatname).FirstOrDefault();
            if (pcat == null) Assert.Fail();
            var pcatid = pcat.Bvin;
            var cat = _irepocategory.GetAddChildCustomLink();
            var column = _applicationDB.ContentServices.Columns.FindAll();
            var newcat = cat;
            {
                newcat.SourceType = CategorySourceType.CustomLink;
                newcat.ParentId = pcatid;
            }

            //Act
            var result = _application.CatalogServices.Categories.Create(newcat);
            if (result)
                result = _application.CatalogServices.Categories.SubmitChanges();

            //Assert
            Assert.IsTrue(result);

        }

        /// <summary>
        /// Edits the link.
        /// </summary>
        //[TestMethod]
        public void EditLink()
        {
            //Arange
            var oldcatname = _irepocategory.GetCustomLinkName();
            var cat = _irepocategory.GetEditCustomLink();

            var edit = _application.CatalogServices.Categories.FindMany(oldcatname).FirstOrDefault(); ;
            {
                edit.Name = cat.Name;
                edit.MetaTitle = cat.MetaTitle;
                edit.ShowInTopMenu = cat.ShowInTopMenu;
                edit.Hidden = cat.Hidden;
                edit.CustomPageUrl = cat.CustomPageUrl;
            }


            //Act
            var result = _application.CatalogServices.Categories.Update(edit);
            if (result)
                result = _application.CatalogServices.Categories.SubmitChanges();
            //TODO: Need to change EnsureJournalType function for CI
            //Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Deletes the link.
        /// </summary>
        //[TestMethod]
        public void DeleteLink()
        {
            //Arrange
            var catname = _irepocategory.GetDeleteCustomLink();
            var cat = _application.CatalogServices.Categories.FindMany(catname).FirstOrDefault();

            //Act/Assert
            Assert.IsTrue(_application.DestroyCategory(cat.Bvin));
        }

        /// <summary>
        /// Loads the category product.
        /// </summary>
        //[TestMethod]
        public void LoadCategoryProduct()
        {
            //Arrange
            var catproduct = _irepocategory.GetCategoryProductCount();
            var cat = _application.CatalogServices.Categories.FindMany(catproduct.FirstOrDefault().Key).FirstOrDefault();

            //Act
            //var result = _application.CatalogServices.FindProductForCategoryWithSort(cat.Bvin, cat.DisplaySortOrder, true);
            //TODO:Need to check functionality count not found

            var result1 = _application.CatalogServices.CategoriesXProducts.FindForCategory(cat.Bvin, 0, int.MaxValue);

            //Assert
            //Assert.AreEqual(catproduct.FirstOrDefault().Value, result.Count);
            Assert.AreEqual(catproduct.FirstOrDefault().Value, result1.Count);
        }

        /// <summary>
        /// Searches the product.
        /// </summary>
        [TestMethod]
		public void Category_SearchProduct()
        {
            //Arrange
            var prjsearch = _irepocategory.GetTotalProductCount();
            var searchprm = prjsearch.FirstOrDefault().Value;
            var totalCount = 0;
            searchprm = SetProductSearchCriteria(searchprm);

            //Act
            _applicationDB.CatalogServices.Products.FindByCriteria(searchprm, 1, 10, ref totalCount);

            //Assert
            Assert.AreEqual(prjsearch.FirstOrDefault().Key, totalCount);
        }

        /// <summary>
        /// Adds the category product.
        /// </summary>
        //[TestMethod]
        public void AddCategoryProduct()
        {
            //Arrange
            var productlst = _irepocategory.GetAddProductToCategory();
            var prjsearch = _irepocategory.GetTotalProductCount();
            var searchprm = prjsearch.FirstOrDefault().Value;
            var totalCount = 0;
            searchprm = SetProductSearchCriteria(searchprm);
            var allprj = _applicationDB.CatalogServices.Products.FindByCriteria(searchprm, 1, 10, ref totalCount);
            var cat = _application.CatalogServices.Categories.FindMany(productlst.FirstOrDefault().Key).FirstOrDefault();

            //Act
            foreach (var p in productlst.FirstOrDefault().Value.Select(prj => allprj.FirstOrDefault(x => x.ProductName.Equals(prj))).Where(p => p != null))
            {
                _application.CatalogServices.CategoriesXProducts.AddProductToCategory(p.Bvin, cat.Bvin);
            }
            var resultcount = _application.CatalogServices.CategoriesXProducts.FindForCategory(cat.Bvin, 1, 5000);

            //Assert
            Assert.AreEqual(productlst.FirstOrDefault().Value.Count, resultcount.Count);
        }

        /// <summary>
        /// Deletes the category product.
        /// </summary>
        //[TestMethod]
        public void DeleteCategoryProduct()
        {
            var productlst = _irepocategory.GetDeleteProductFromCategory();
            var prjsearch = _irepocategory.GetTotalProductCount();
            var searchprm = prjsearch.FirstOrDefault().Value;
            var totalCount = 0;
            searchprm = SetProductSearchCriteria(searchprm);
            var allprj = _applicationDB.CatalogServices.Products.FindByCriteria(searchprm, 1, 10, ref totalCount);
            var cat = _application.CatalogServices.Categories.FindMany(productlst.FirstOrDefault().Key).FirstOrDefault();

            //Act

            foreach (var p in productlst.FirstOrDefault().Value.Select(prj => allprj.FirstOrDefault(x => x.ProductName.Equals(prj))).Where(p => p != null))
            {
                _application.CatalogServices.CategoriesXProducts.RemoveProductFromCategory(p.Bvin, cat.Bvin);
            }
            var resultcount = _application.CatalogServices.CategoriesXProducts.FindForCategory(cat.Bvin, 1, 5000);

            //Assert
            Assert.AreEqual(1, resultcount.Count);
        }

        /// <summary>
        /// Deletes all for category.
        /// </summary>
        //[TestMethod]
        public void DeleteAllForCategory()
        {
            //Arrange
            var productlst = _irepocategory.GetDeleteProductFromCategory();
            var cat = _application.CatalogServices.Categories.FindMany(productlst.FirstOrDefault().Key).FirstOrDefault();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.CategoriesXProducts.DeleteAllForCategory(cat.Bvin));

        }


        /// <summary>
        /// Resorts the category product.
        /// </summary>
        [TestMethod]
		public void Category_ResortCategoryProduct()
        {
            //Arrange
            var catproduct = _irepocategory.GetCategoryProductCount();
            var cat = _application.CatalogServices.Categories.FindMany(catproduct.FirstOrDefault().Key).FirstOrDefault();
            var prjlst = _application.CatalogServices.CategoriesXProducts.FindForCategory(cat.Bvin, 1, int.MaxValue).Select(x => x.ProductId).ToList();
            prjlst.Reverse();

            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.CategoriesXProducts.ResortProducts(cat.Bvin, prjlst));

        }




        /// <summary>
        /// Adds the role.
        /// </summary>
        //[TestMethod]
        public void AddRole()
        {
            //Arrange
            var catname = _irepocategory.GetEditCategoryName();
            var cat = _application.CatalogServices.Categories.FindMany(catname).FirstOrDefault();
            if (cat == null) Assert.Fail();
            var role = _irepocategory.GetAddRole(cat.Bvin);


            //Act/Assert
            Assert.IsTrue(_application.CatalogServices.CatalogRoles.Create(role));
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        //[TestMethod]
        public void DeleteRole()
        {
            //Arrange
            var rolename = _irepocategory.GetDeleteRole();
            var catname = _irepocategory.GetEditCategoryName();
            var cat = _application.CatalogServices.Categories.FindMany(catname).FirstOrDefault();
            if (cat == null) Assert.Fail();
            var role = _application.CatalogServices.CatalogRoles.FindByCategoryId(new Guid(cat.Bvin)).FirstOrDefault(x => x.RoleName.Equals(rolename));

            //Act
            _application.CatalogServices.CatalogRoles.Delete(role.CatalogRoleId);
            var resultrole = _application.CatalogServices.CatalogRoles.FindByCategoryId(new Guid(cat.Bvin)).FirstOrDefault(x => x.RoleName.Equals(rolename));


            //Assert
            Assert.AreEqual(null, resultrole);

        }

        /// <summary>
        /// Totals the category role count.
        /// </summary>
        //[TestMethod]
        public void TotalCategoryRoleCount()
        {
            //Arrange
            var count = _irepocategory.GetTotalCategoryRoleCount();
            var catname = _irepocategory.GetEditCategoryName();
            var cat = _application.CatalogServices.Categories.FindMany(catname).FirstOrDefault();
            if (cat == null) Assert.Fail();

            //Act
            var resultcount = _application.CatalogServices.CatalogRoles.FindByCategoryId(new Guid(cat.Bvin));

            //Assert
            Assert.AreEqual(count, resultcount.Count);
        }

        /// <summary>
        /// Totals the role count.
        /// </summary>
        [TestMethod]
		public void Category_TotalRoleCount()
        {
            //Arrange
            var count = _irepocategory.GetTotalRoleCount();
            var resultcount =DnnUserController.Instance.GetRoles();
            //TODO: Not found RoleController().GetRoles() function

            //Act/Assert
            Assert.AreEqual(count, resultcount.Count);
        }





        /// <summary>
        /// Finds this instance.
        /// </summary>
        [TestMethod]
		public void Category_Find()
        {
            //Arrange
            var catname = _irepocategory.GetCategoryName();
            var cat = _applicationDB.CatalogServices.Categories.FindMany(catname).FirstOrDefault();
            if (cat == null) Assert.Fail();

            //Act
            var resultcat = _applicationDB.CatalogServices.Categories.Find(cat.Bvin);

            //Assert
            Assert.AreEqual(cat.Bvin, resultcat.Bvin);

        }


        /// <summary>
        /// Finds the by slug.
        /// </summary>
        [TestMethod]
		public void Category_FindBySlug()
        {
            //Arrange
            var catslug = _irepocategory.GetCategorySlug();

            //Act
            var cat = _applicationDB.CatalogServices.Categories.FindBySlug(catslug);

            //Assert
            Assert.AreNotEqual(cat, null);

        }


        /// <summary>
        /// Finds the many.
        /// </summary>
        [TestMethod]
		public void Category_FindMany()
        {
            //Arrange
            var catids = _irepocategory.GetCateIds();

            //Act
            var cat = _applicationDB.CatalogServices.Categories.FindMany(catids);

            //Assert
            Assert.AreNotEqual(0, cat.Count);
        }


        /// <summary>
        /// Stores the category snapshot.
        /// </summary>
        [TestMethod]
		public void Category_StoreCategorySnapshot()
        {
            //Arrange
            var count = _irepocategory.GetStoreCatSnapCount();

            //Act
            var cat = _applicationDB.CatalogServices.Categories.FindAllSnapshotsPaged(1, int.MaxValue);

            //Assert
            Assert.AreNotEqual(count, cat.Count);
        }

        /// <summary>
        /// Totals the category snapshot.
        /// </summary>
        [TestMethod]
		public void Category_TotalCategorySnapshot()
        {
            //Arrange
            var count = _irepocategory.GetTotalCatSnapCount();

            //Act
            var cat = _applicationDB.CatalogServices.Categories.FindAllForAllStores();

            //Assert
            Assert.AreNotEqual(count, cat.Count);

        }


        /// <summary>
        /// Totals the category page.
        /// </summary>
        [TestMethod]
		public void Category_TotalCategoryPage()
        {
            //Arrange
            var count = _irepocategory.GetTotalCatPages();

            //Act
            var cat = _applicationDB.CatalogServices.Categories.FindAllPaged(1, int.MaxValue);

            //Assert
            Assert.AreNotEqual(count, cat.Count);

        }

        /// <summary>
        /// Visibles the child category.
        /// </summary>
        [TestMethod]
		public void Category_VisibleChildCategory()
        {
            //Arrange
            var catname = _irepocategory.GetCategoryName();
            var cat = _applicationDB.CatalogServices.Categories.FindMany(catname).FirstOrDefault();
            if (cat == null) Assert.Fail();

            var count = _irepocategory.GetTotalVisibleChildCatCount();

            //Act
            var resultcat = _applicationDB.CatalogServices.Categories.FindVisibleChildren(cat.Bvin);

            //Assert
            Assert.AreNotEqual(count, resultcat.Count);


        }

        /// <summary>
        /// Totals the child category.
        /// </summary>
        [TestMethod]
		public void Category_TotalChildCategory()
        {
            //Arrange
            var catname = _irepocategory.GetCategoryName();
            var cat = _applicationDB.CatalogServices.Categories.FindMany(catname).FirstOrDefault();
            if (cat == null) Assert.Fail();
            var count = _irepocategory.GetTotalChildCatCount();


            //Act
            var resultcat = _applicationDB.CatalogServices.Categories.FindChildren(cat.Bvin);

            //Assert
            Assert.AreNotEqual(count, resultcat.Count);

        }

    }
}
