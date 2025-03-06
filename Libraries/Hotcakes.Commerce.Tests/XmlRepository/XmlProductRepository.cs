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
using System.Text;
using System.Xml.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Tests.IRepository;
using Hotcakes.Commerce.Tests.TestData;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Tests.XmlRepository
{
    public class XmlProductRepository : IXmlProductRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlProductRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.Product)).GetXml();
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

        #region Product Repository Service
        #region Product Load/Add/Edit/Delete Functions
        /// <summary>
        /// Gets the total product type count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductTypeCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductTypeCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total product count.
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, ProductSearchCriteria> GetTotalProdutCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("TotalProductSearchCount").FirstOrDefault();
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
                                  ProductTypeId = Convert.ToString(element.Element("ProductTypeId").Value),
                                  Status = (ProductStatus)(element.Element("Status")==null?0:Convert.ToInt32(element.Element("Status").Value)),
                                  InventoryStatus =(ProductInventoryStatus)(element.Element("InventoryStatus")==null?0:Convert.ToInt32(element.Element("InventoryStatus").Value)),
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
        /// Gets the total vendor count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalVendorCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").FirstOrDefault();
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
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalManufacturerCount").Value);
            }
            catch (Exception)
            {
                return 0;
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
                var element = _xmldoc.Elements("Product").FirstOrDefault();
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
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalPopulateColumnsCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total tax.
        /// </summary>
        /// <returns></returns>
        public int GetTotalTax()
        {
            try
            {
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalTaxCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the delete product.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteProductSku()
        {
            try
            {
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("Delete").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the edit product.
        /// </summary>
        /// <returns></returns>
        public string GetEditProductSku()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("Edit").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("OldSku").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the add product.
        /// </summary>
        /// <returns></returns>
        public Product GetAddProduct()
        {
            return GetProduct(_xmldoc.Elements("Product").Elements("Add").FirstOrDefault());
        }

        /// <summary>
        /// Gets the edit product.
        /// </summary>
        /// <returns></returns>
        public Product GetEditProduct()
        {
            return GetProduct(_xmldoc.Elements("Product").Elements("Edit").FirstOrDefault());
        }

        /// <summary>
        /// Gets the product.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Product GetProduct(XElement element)
        {
            try
            {
                if (element == null) return new Product();
                return new Product
                {
                    Featured = (element.Element("Featured") != null && Convert.ToBoolean(element.Element("Featured").Value)),
                    TemplateName = Convert.ToString(element.Element("TemplateName").Value),
                    IsBundle = (element.Element("IsBundle") != null && Convert.ToBoolean(element.Element("IsBundle").Value)),
                    IsSearchable = (element.Element("IsSearchable") != null || Convert.ToBoolean(element.Element("IsSearchable").Value)),
                    AllowUpcharge = (element.Element("AllowUpcharge") != null || Convert.ToBoolean(element.Element("AllowUpcharge").Value)),
                    UpchargeAmount = element.Element("UpchargeAmount") == null ? 1 : Convert.ToInt32(element.Element("UpchargeAmount").Value),
                    UpchargeUnit = Convert.ToString(element.Element("UpchargeUnit").Value),
                    Status = (element.Element("Status") == null ? ProductStatus.Active : (ProductStatus)Convert.ToInt32(element.Element("Status").Value)),
                    Sku = Convert.ToString(element.Element("Sku").Value),
                    ProductName = Convert.ToString(element.Element("ProductName").Value),
                    ProductTypeId = Convert.ToString(element.Element("ProductTypeId").Value),
                    UserSuppliedPriceLabel = Convert.ToString(element.Element("UserSuppliedPriceLabel").Value),
                    HideQty = (element.Element("HideQty") != null && Convert.ToBoolean(element.Element("HideQty").Value)),
                    IsUserSuppliedPrice = (element.Element("IsUserSuppliedPrice") != null && Convert.ToBoolean(element.Element("IsUserSuppliedPrice").Value)),
                    SitePriceOverrideText = Convert.ToString(element.Element("SitePriceOverrideText").Value),
                    SitePrice = Money.RoundCurrency(element.Element("SitePrice") == null ? 0 : Convert.ToDecimal(element.Element("SitePrice").Value)),
                    SiteCost = Money.RoundCurrency(element.Element("SiteCost") == null ? 0 : Convert.ToDecimal(element.Element("SiteCost").Value)),
                    ListPrice = Money.RoundCurrency(element.Element("ListPrice") == null ? 0 : Convert.ToDecimal(element.Element("ListPrice").Value)),
                    VendorId = Convert.ToString(element.Element("VendorId").Value),
                    ManufacturerId = Convert.ToString(element.Element("ManufacturerId").Value),
                    PostContentColumnId = Convert.ToString(element.Element("PostContentColumnId").Value),
                    PreContentColumnId = Convert.ToString(element.Element("PreContentColumnId").Value),
                    UrlSlug = Web.Text.Slugify(Convert.ToString(element.Element("UrlSlug").Value), true),
                    TaxSchedule = element.Element("TaxSchedule") == null ? -1 : Convert.ToInt32(element.Element("TaxSchedule").Value),
                    Keywords = Convert.ToString(element.Element("Keywords").Value),
                    MetaKeywords = Convert.ToString(element.Element("MetaKeywords").Value),
                    MetaDescription = Convert.ToString(element.Element("MetaDescription").Value),
                    MetaTitle = Convert.ToString(element.Element("MetaTitle").Value),
                    SwatchPath = Convert.ToString(element.Element("MetaTitle").Value),
                    AllowReviews = (element.Element("AllowReviews") == null || string.IsNullOrWhiteSpace(element.Element("AllowReviews").Value) ? (bool?)null : Convert.ToBoolean(element.Element("AllowReviews").Value)),
                    TaxExempt = (element.Element("TaxExempt") != null && Convert.ToBoolean(element.Element("TaxExempt").Value)),
                    MinimumQty = element.Element("MinimumQty") == null ? 1 : Convert.ToInt32(element.Element("MinimumQty").Value),
                    ShippingDetails = new ShippableItem
                        {
                            ShipSeparately = (element.Element("ShipSeparately") != null && Convert.ToBoolean(element.Element("ShipSeparately").Value)),
                            IsNonShipping = (element.Element("IsNonShipping") != null && Convert.ToBoolean(element.Element("IsNonShipping").Value)),
                            ExtraShipFee = Money.RoundCurrency(element.Element("ExtraShipFee") == null ? 0 : Convert.ToDecimal(element.Element("ExtraShipFee").Value)),
                            Height = element.Element("Height") == null ? 0 : Convert.ToDecimal(element.Element("Height").Value),
                            Width = element.Element("Width") == null ? 0 : Convert.ToDecimal(element.Element("Width").Value),
                            Length = element.Element("Length") == null ? 0 : Convert.ToDecimal(element.Element("Length").Value),
                            Weight = element.Element("Weight") == null ? 0 : Convert.ToDecimal(element.Element("Weight").Value),
                        },
                    ShippingMode = (ShippingMode)(element.Element("ShippingMode") == null ? 1 : Convert.ToInt32(element.Element("ShippingMode").Value)),
                    ImageFileSmall = Convert.ToString(element.Element("ImageFileSmall").Value),
                    ImageFileMedium = Convert.ToString(element.Element("ImageFileMedium").Value),
                };
            }
            catch (Exception)
            {
                return new Product();
            }
        }

        /// <summary>
        /// Gets the add product taxonomy tags.
        /// </summary>
        /// <returns></returns>
        public string GetProductTaxonomyTags()
        {
            try
            {
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("Taxonomy").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the clone product information.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> GetCloneProductInfo()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("Clone").FirstOrDefault();

                if (element == null) return new Dictionary<string, Dictionary<string, string>>();

                var dic = new Dictionary<string, Dictionary<string, string>>
                    {
                        {
                            Convert.ToString(element.Element("OldSku").Value),
                            new Dictionary<string, string>
                                {
                                    {
                                        "ProductName",
                                        Convert.ToString(element.Element("ProductName").Value)
                                    },
                                    {
                                        "Sku",
                                        Convert.ToString(element.Element("Sku").Value)
                                    },
                                    {
                                        "CloneProductChoices",
                                       (element.Element("CloneProductChoices")==null?"false": Convert.ToString(element.Element("CloneProductChoices").Value))
                                    },
                                    {
                                        "CloneProductImages",
                                       (element.Element("CloneProductImages")==null?"false": Convert.ToString(element.Element("CloneProductImages").Value))
                                    },
                                    {
                                        "CloneCategoryPlacement",
                                        (element.Element("CloneCategoryPlacement")==null?"false": Convert.ToString(element.Element("CloneCategoryPlacement").Value))
                                    },
                                     {
                                        "CreateAsInactive",
                                        (element.Element("CreateAsInactive")==null?"1": Convert.ToString(element.Element("CreateAsInactive").Value))
                                    },
                                }
                        },
                    };

                return dic;
            }
            catch (Exception)
            {
                return new Dictionary<string, Dictionary<string, string>>();
            }

        }

        /// <summary>
        /// Gets the add product property value.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ProductPropertyType, string> GetAddProductPropertyValue()
        {
            return GetProductPropertyValue(_xmldoc.Elements("Product").Elements("Add").Elements("ProductTypeProperty").FirstOrDefault());
        }

        /// <summary>
        /// Gets the edit product property value.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ProductPropertyType, string> GetEditProductPropertyValue()
        {
            return GetProductPropertyValue(_xmldoc.Elements("Product").Elements("Edit").Elements("ProductTypeProperty").FirstOrDefault());
        }

        /// <summary>
        /// Gets the product property value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public Dictionary<ProductPropertyType, string> GetProductPropertyValue(XElement element)
        {
            try
            {
                if (element == null) return new Dictionary<ProductPropertyType, string>();

                return new Dictionary<ProductPropertyType, string>
                       {
                        {ProductPropertyType.DateField, Convert.ToString(element.Element("Date").Value)},
                        {ProductPropertyType.TextField, Convert.ToString(element.Element("Text").Value)},
                        {ProductPropertyType.CurrencyField, Convert.ToString(element.Element("Currency").Value)},
                        {ProductPropertyType.MultipleChoiceField, Convert.ToString(element.Element("Multiple").Value)},
                       };
            }
            catch (Exception)
            {
                return new Dictionary<ProductPropertyType, string>();
            }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <returns></returns>
        public string GetProductSku()
        {
            try
            {
                var element = _xmldoc.Elements("Product").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("RootProductSku").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion
        #region Product Find Functions

        /// <summary>
        /// Finds all paged for all store count.
        /// </summary>
        /// <returns></returns>
        public int FindAllPagedForAllStoreCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllPagedForAllStoreCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Finds all paged count.
        /// </summary>
        /// <returns></returns>
        public int FindAllPagedCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllPagedCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Finds all count for all store count.
        /// </summary>
        /// <returns></returns>
        public int FindAllCountForAllStoreCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllCountForAllStoreCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Finds all count.
        /// </summary>
        /// <returns></returns>
        public int FindAllCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Features the product count.
        /// </summary>
        /// <returns></returns>
        public int FeatureProductCount()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FeatureProductCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Finds the many by sku.
        /// </summary>
        /// <returns></returns>
        public List<string> FindManyBySku()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").Elements("FindMany").Elements("Skus").FirstOrDefault();
                return element == null ? new List<string>() : element.Elements("Sku").Select(x => x.Value).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Finds the by slug.
        /// </summary>
        /// <returns></returns>
        public string FindBySlug()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("FindBySlug").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Findmanies the by bvin.
        /// </summary>
        /// <returns></returns>
        public List<string> FindmanyByBvin()
        {
            try
            {
                var element = _xmldoc.Elements("Product").Elements("ProductRepoFunction").Elements("FindMany").Elements("Bvin").FirstOrDefault();
                return element == null ? new List<string>() : element.Elements("Id").Select(x => x.Value).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        #endregion
        #endregion

        #region Product Tab

        /// <summary>
        /// Gets the total product tab.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductTabCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductTab").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductTabCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the add product tab.
        /// </summary>
        /// <returns></returns>
        public ProductDescriptionTab GetAddProductTab()
        {
            try
            {
                return _xmldoc.Elements("ProductTab").Elements("AddTab").Select(x => new ProductDescriptionTab
                    {
                        TabTitle = Convert.ToString(x.Element("Title").Value),
                        HtmlData = Convert.ToString(x.Element("Desc").Value),
                    }).FirstOrDefault();
            }
            catch (Exception)
            {
                return new ProductDescriptionTab();
            }
        }

        /// <summary>
        /// Gets the edit product tab.
        /// </summary>
        /// <returns></returns>
        public ProductDescriptionTab GetEditProductTab()
        {
            try
            {
                return _xmldoc.Elements("ProductTab").Elements("EditTab").Select(x => new ProductDescriptionTab
                {
                    TabTitle = Convert.ToString(x.Element("Title").Value),
                    HtmlData = Convert.ToString(x.Element("Desc").Value),
                }).FirstOrDefault();
            }
            catch (Exception)
            {
                return new ProductDescriptionTab();
            }
        }

        /// <summary>
        /// Gets the name of the edit product tab.
        /// </summary>
        /// <returns></returns>
        public string GetEditProductTabName()
        {
            try
            {
                var element = _xmldoc.Elements("ProductTab").Elements("EditTab").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("OldTitle").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Gets the name of the delete product tab.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteProductTabName()
        {
            try
            {
                var element = _xmldoc.Elements("ProductTab").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("DeleteTab").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
