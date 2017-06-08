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
    public class XmlProductVariantRepository : IXmlProductVariantRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlProductVariantRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.ProductVariant)).GetXml();
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

        #region Product Variant Repository Service

        /// <summary>
        /// Gets the total product variant option count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProduct_VO_Count()
        {
            try
            {
                var element = _xmldoc.Elements("ProductVariant").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductVariantOptionCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total product variant count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductVariantCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductVariant").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductVariantCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the edit product variant.
        /// </summary>
        /// <returns></returns>
        public Variant GetEditProductVariant()
        {
            try
            {
                return _xmldoc.Elements("ProductVariant").Elements("EditVariant").Select(x => new Variant
                    {
                        Sku = Convert.ToString(x.Element("Sku").Value),
                        Price = x.Element("Price") == null ? 0 : Convert.ToDecimal(x.Element("Price").Value)
                    }).FirstOrDefault();
            }
            catch (Exception)
            {
                return new Variant();
            }
        }

        #endregion

    }
}
