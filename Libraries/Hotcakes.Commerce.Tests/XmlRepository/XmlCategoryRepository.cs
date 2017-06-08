using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Tests.IRepository;
using Hotcakes.Commerce.Tests.TestData;

namespace Hotcakes.Commerce.Tests.XmlRepository
{
    /// <summary>
    /// Repository
    /// </summary>
    public class XmlCategoryRepository : IXmlCategoryRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlCategoryRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.Category)).GetXml();
        }


        #region Dispose Object
        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _xmldoc = null;
                }
            }
            this._disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Category Count
        /// <summary>
        /// Gets the total category count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalCategoryCount()
        {
            try
            {
                var element = _xmldoc.Elements("Category").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalCategoryCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total category child count.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetTotalCategoryChildCount()
        {
            try
            {
                var element = _xmldoc.Elements("Category").Elements("TotalCategoryChildCount").FirstOrDefault();
                if (element == null) return new Dictionary<string, int>();

                var dic = new Dictionary<string, int>
                    {
                        {
                            Convert.ToString(element.Element("Name").Value),
                            element.Element("Count") == null ? 0 : Convert.ToInt32(element.Element("Count").Value)
                        }
                    };

                return dic;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Gets the category display template count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalDisplayTemplateCount()
        {
            try
            {
                var element = _xmldoc.Elements("Category").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalDisplayTemplateCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total populate columns count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalPopulateColumnsCount()
        {

            try
            {
                var element = _xmldoc.Elements("Category").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalPopulateColumnsCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region Category Add/Edit/Delete
        /// <summary>
        /// Gets the delete category.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteCategory()
        {
            try
            {
                var element = _xmldoc.Elements("Category").FirstOrDefault();
                return Convert.ToString(element.Element("Delete").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the add category.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Category GetAddCategory()
        {
            return GetCategory(_xmldoc.Elements("Category").Elements("Add").FirstOrDefault());
        }

        /// <summary>
        /// Gets the add child category.
        /// </summary>
        /// <returns></returns>
        public Category GetAddChildCategory()
        {
            return GetCategory(_xmldoc.Elements("Category").Elements("AddChild").FirstOrDefault());
        }

        /// <summary>
        /// Gets the edit category.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Category GetEditCategory()
        {
            return GetCategory(_xmldoc.Elements("Category").Elements("Edit").FirstOrDefault());
        }

        /// <summary>
        /// Gets the category taxonomy.
        /// </summary>
        /// <returns></returns>
        public string GetCategoryTaxonomy()
        {
            try
            {
                var element = _xmldoc.Elements("Category").FirstOrDefault();
                return Convert.ToString(element.Element("Taxonomy").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <returns></returns>
        public string GetEditCategoryName()
        {
            try
            {
                var element = _xmldoc.Elements("Category").Elements("Edit").FirstOrDefault();
                return Convert.ToString(element.Element("OldName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <returns></returns>
        public string GetCategoryName()
        {
            try
            {
                var element = _xmldoc.Elements("Category").FirstOrDefault();
                return Convert.ToString(element.Element("RootCatName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Category GetCategory(XElement element)
        {
            try
            {
                if (element == null) return new Category();
                return new Category
                    {
                        Name = Convert.ToString(element.Element("Name").Value),
                        MetaDescription = Convert.ToString(element.Element("Description").Value),
                        MetaTitle = Convert.ToString(element.Element("MetaTitle").Value),
                        MetaKeywords = Convert.ToString(element.Element("MetaKeywords").Value),
                        ShowInTopMenu = element.Element("ShowInTopMenu") != null && Convert.ToBoolean(element.Element("ShowInTopMenu").Value),
                        Hidden = element.Element("Hidden") != null && Convert.ToBoolean(element.Element("Hidden").Value),
                        TemplateName = Convert.ToString(element.Element("TemplateName").Value),
                        PreContentColumnId = Convert.ToString(element.Element("PreContentColumnId").Value),
                        PostContentColumnId = Convert.ToString(element.Element("PostContentColumnId").Value),
                        DisplaySortOrder = (CategorySortOrder)(element.Element("DisplaySortOrder") == null ? 2 : Convert.ToInt32(element.Element("DisplaySortOrder").Value)),
                        ShowTitle = element.Element("ShowTitle") != null && Convert.ToBoolean(element.Element("ShowTitle").Value),
                        Keywords = Convert.ToString(element.Element("Keywords").Value),
                        ParentId = Convert.ToString(element.Element("ParentId").Value),
                        RewriteUrl = Convert.ToString(element.Element("PageName").Value),

                    };
            }
            catch (Exception)
            {
                return new Category();
            }
        }

        #endregion

        #region Link Add/Edit/Delete
        /// <summary>
        /// Gets the add custom link.
        /// </summary>
        /// <returns></returns>
        public Category GetAddCustomLink()
        {
            return GetLink(_xmldoc.Elements("CategoryLink").Elements("AddLink").FirstOrDefault());

        }

        /// <summary>
        /// Gets the add child custom link.
        /// </summary>
        /// <returns></returns>
        public Category GetAddChildCustomLink()
        {
            return GetLink(_xmldoc.Elements("CategoryLink").Elements("AddChildLink").FirstOrDefault());
        }

        /// <summary>
        /// Gets the edit custom link.
        /// </summary>
        /// <returns></returns>
        public Category GetEditCustomLink()
        {
            return GetLink(_xmldoc.Elements("CategoryLink").Elements("EditLink").FirstOrDefault());
        }

        /// <summary>
        /// Gets the name of the custom link.
        /// </summary>
        /// <returns></returns>
        public string GetCustomLinkName()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryLink").Elements("EditLink").FirstOrDefault();
                return Convert.ToString(element.Element("OldName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the delete custom link.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteCustomLink()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryLink").FirstOrDefault();
                return Convert.ToString(element.Element("DeleteLink").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Category GetLink(XElement element)
        {
            try
            {
                if (element == null) return new Category();
                return new Category
                {
                    Name = Convert.ToString(element.Element("Name").Value),
                    MetaTitle = Convert.ToString(element.Element("MetaTitle").Value),
                    ShowInTopMenu = element.Element("ShowInTopMenu") != null && Convert.ToBoolean(element.Element("ShowInTopMenu").Value),
                    Hidden = element.Element("Hidden") != null && Convert.ToBoolean(element.Element("Hidden").Value),
                    CustomPageUrl = Convert.ToString(element.Element("LinkTo").Value),
                };
            }
            catch (Exception)
            {
                return new Category();
            }
        }
        #endregion

        #region Category Product
        /// <summary>
        /// Gets the total vendor count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalVendorCount()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryProduct").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalVendorCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total manufacturer count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalManufacturerCount()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryProduct").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalManufacturerCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the category product count.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetCategoryProductCount()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryProduct").Elements("CategoryProductCount").FirstOrDefault();
                if (element == null) return new Dictionary<string, int>();

                var dic = new Dictionary<string, int>
                    {
                        {
                            Convert.ToString(element.Element("CategoryName").Value),
                            element.Element("ProductCount") == null ? 0 : Convert.ToInt32(element.Element("ProductCount").Value)
                        }
                    };

                return dic;

            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Gets the total product count.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, ProductSearchCriteria> GetTotalProductCount()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryProduct").Elements("TotalProductSearchCount").FirstOrDefault();
                if (element == null) return new Dictionary<int, ProductSearchCriteria>();

                return new Dictionary<int, ProductSearchCriteria>
                    {
                       {
                          element.Element("Count")==null?0:Convert.ToInt32(element.Element("Count").Value),
                          new ProductSearchCriteria
                              {
                                  Keyword =Convert.ToString(element.Element("Keyword").Value),
                                  CategoryId =Convert.ToString(element.Element("CategoryId").Value),
                                  VendorId = Convert.ToString(element.Element("VendorId").Value),
                                  ManufacturerId =Convert.ToString(element.Element("ManufacturerId").Value),
                              }
                        },
                      };
            }
            catch (Exception)
            {
                return new Dictionary<int, ProductSearchCriteria>();
            }
        }

        /// <summary>
        /// Gets the add product to category.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetAddProductToCategory()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryProduct").Elements("AddProductToCategory");
                return new Dictionary<string, List<string>>
                    {
                        {Convert.ToString(element.FirstOrDefault().Element("CategoryName").Value), 
                         element.Elements("Name").Select(x => Convert.ToString(x.Value)).ToList()}
                    };
            }
            catch (Exception)
            {
                return new Dictionary<string, List<string>>();
            }
        }

        /// <summary>
        /// Gets the delete product from category.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetDeleteProductFromCategory()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryProduct").Elements("DeleteProductToCategory");

                return new Dictionary<string, List<string>>
                    {
                        {Convert.ToString(element.FirstOrDefault().Element("CategoryName").Value), 
                        element.Elements("Name").Select(x => Convert.ToString(x.Value)).ToList()}
                    };
            }
            catch (Exception)
            {
                return new Dictionary<string, List<string>>();
            }
        }
        #endregion

        #region Category Role
        /// <summary>
        /// Gets the total role count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalRoleCount()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryRole").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalRoleCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total category role count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalCategoryRoleCount()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryRole").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalCategoryRoleCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the delete role.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteRole()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryRole").Elements("DeleteRole").FirstOrDefault();
                return Convert.ToString(element.Element("Name").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the add role.
        /// </summary>
        /// <returns></returns>
        public CatalogRole GetAddRole(string refid)
        {
            try
            {
                return _xmldoc.Elements("CategoryRole").Elements("AddRole").Select(x => new CatalogRole
                    {
                        RoleName = Convert.ToString(x.Element("Name").Value),
                        RoleType = CatalogRoleType.CategoryRole,
                        ReferenceId = string.IsNullOrEmpty(refid) ? new Guid() : new Guid(refid),
                    }).FirstOrDefault();
            }
            catch (Exception)
            {
                return new CatalogRole();
            }
        }

        /// <summary>
        /// Gets the role refname.
        /// </summary>
        /// <returns></returns>
        public string GetRoleName()
        {
            try
            {
                var element = _xmldoc.Elements("CategoryRole").Elements("AddRole").FirstOrDefault();
                return Convert.ToString(element.Element("ReferenceId").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion

        #region Category Find
        /// <summary>
        /// Gets the category slug.
        /// </summary>
        /// <returns></returns>
        public string GetCategorySlug()
        {
            try
            {
                var element = _xmldoc.Elements("CatRepoFunction").FirstOrDefault();
                return Convert.ToString(element.Element("CatSlug").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the cate ids.
        /// </summary>
        /// <returns></returns>
        public List<string> GetCateIds()
        {
            try
            {
                return _xmldoc.Elements("CatRepoFunction").Elements("FindMany").Elements("Id").Select(v1 => Convert.ToString(v1.Value)).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the store cat snap count.
        /// </summary>
        /// <returns></returns>
        public int GetStoreCatSnapCount()
        {
            try
            {
                var element = _xmldoc.Elements("CatRepoFunction").FirstOrDefault();
                return Convert.ToInt32(element.Element("StoreCatSnapCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total cat snap count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalCatSnapCount()
        {
            try
            {
                var element = _xmldoc.Elements("CatRepoFunction").FirstOrDefault();
                return Convert.ToInt32(element.Element("TotalCatSnapCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total cat pages.
        /// </summary>
        /// <returns></returns>
        public int GetTotalCatPages()
        {
            try
            {
                var element = _xmldoc.Elements("CatRepoFunction").FirstOrDefault();
                return Convert.ToInt32(element.Element("TotalCatPages").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total visible child cat count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalVisibleChildCatCount()
        {
            try
            {
                var element = _xmldoc.Elements("CatRepoFunction").FirstOrDefault();
                return Convert.ToInt32(element.Element("TotalVisibleChildCatCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total child cat count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalChildCatCount()
        {
            try
            {
                var element = _xmldoc.Elements("CatRepoFunction").FirstOrDefault();
                return Convert.ToInt32(element.Element("TotalChildCatCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

    }
}
