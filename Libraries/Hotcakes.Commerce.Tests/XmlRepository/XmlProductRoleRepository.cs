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
    public class XmlProductRoleRepository : IXmlProductRoleRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlProductRoleRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.ProductRole)).GetXml();
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

        #region Product Role Repository Service

        /// <summary>
        /// Gets the total product role count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductRoleCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductRole").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductRoleCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the delete product role.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteProductRole()
        {
            try
            {
                var element = _xmldoc.Elements("ProductRole").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("DeleteRole").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the add product role.
        /// </summary>
        /// <returns></returns>
        public string GetAddProductRole()
        {
            try
            {
                var element = _xmldoc.Elements("ProductRole").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("AddRole").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
