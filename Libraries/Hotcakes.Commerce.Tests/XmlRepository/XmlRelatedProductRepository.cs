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
    public class XmlRelatedProductRepository : IXmlRelatedProductRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlRelatedProductRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.RelatedProduct)).GetXml();
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

        #region Related Product Repository Service

        /// <summary>
        /// Gets the total related product count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalRelatedProductCount()
        {
            try
            {
                var element = _xmldoc.Elements("RelatedProduct").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalRelatedProductCount").Value);
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
        public Dictionary<int, ProductSearchCriteria> GetTotalProductCount()
        {
            try
            {
                var element = _xmldoc.Elements("RelatedProduct").Elements("TotalProductSearchCount").FirstOrDefault();
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
        /// Gets the add related product.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAddRelatedProduct()
        {
            try
            {
                return _xmldoc.Element("RelatedProduct").Elements("AddProduct").Elements("Sku").Select(x => Convert.ToString(x.Value)).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the delete related product.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDeleteRelatedProduct()
        {
            try
            {
                return _xmldoc.Element("RelatedProduct").Elements("DeleteProduct").Elements("Sku").Select(x => Convert.ToString(x.Value)).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        #endregion

    }
}
