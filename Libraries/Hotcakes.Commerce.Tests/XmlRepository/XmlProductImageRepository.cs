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
    public class XmlProductImageRepository : IXmlProductImageRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlProductImageRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.ProductImage)).GetXml();
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

        #region Product Image Repository Service

        /// <summary>
        /// Gets the total product image count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductImageCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductImage").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductImageCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the add product image.
        /// </summary>
        /// <returns></returns>
        public List<ProductImage> GetAddProductImage()
        {
            try
            {
                var element = _xmldoc.Elements("ProductImage").Elements("Image");
                if (element == null) return new List<ProductImage>();

                return element.Elements("IMG").Select(x => new ProductImage
                    {
                        AlternateText = Convert.ToString(x.Element("AlternateText").Value),
                        Bvin = new Guid().ToString(),
                        Caption = Convert.ToString(x.Element("Caption").Value),
                        FileName = Convert.ToString(x.Element("FileName").Value),

                    }).ToList();
            }
            catch (Exception)
            {
                return new List<ProductImage>();
            }
        }

        /// <summary>
        /// Gets the merge product image.
        /// </summary>
        /// <returns></returns>
        public List<ProductImage> GetMergeProductImage()
        {
            try
            {
                var element = _xmldoc.Elements("ProductImage").Elements("ProductImageRepo").Elements("Image");
                if (element == null) return new List<ProductImage>();

                return element.Elements("IMG").Select(x => new ProductImage
                {
                    AlternateText = Convert.ToString(x.Element("AlternateText").Value),
                    //Bvin = new Guid().ToString(),
                    Caption = Convert.ToString(x.Element("Caption").Value),
                    FileName = Convert.ToString(x.Element("FileName").Value),

                }).ToList();
            }
            catch (Exception)
            {
                return new List<ProductImage>();
            }
        }

      #endregion

    }
}
