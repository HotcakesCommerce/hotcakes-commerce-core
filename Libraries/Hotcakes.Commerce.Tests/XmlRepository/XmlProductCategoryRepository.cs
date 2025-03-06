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
    public class XmlProductCategoryRepository : IXmlProductCategoryRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlProductCategoryRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.ProductCategory)).GetXml();
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

        #region Product Category Repository Service

        /// <summary>
        /// Gets the total product category.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductCategory()
        {
            try
            {
                var element = _xmldoc.Elements("ProductCategory").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductCategoryCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total category.
        /// </summary>
        /// <returns></returns>
        public int GetTotalCategory()
        {
            try
            {
                var element = _xmldoc.Elements("ProductCategory").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalCategoryCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        /// <summary>
        /// Gets the find all count.
        /// </summary>
        /// <returns></returns>
        public int GetFindAllCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductCategory").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }



        /// <summary>
        /// Gets the find all for all store count.
        /// </summary>
        /// <returns></returns>
        public int GetFindAllForAllStoreCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductCategory").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllForAllStoreCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }



        /// <summary>
        /// Gets the find all paged count.
        /// </summary>
        /// <returns></returns>
        public int GetFindAllPagedCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductCategory").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllPagedCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the add product category.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAddProductCategory()
        {
            try
            {
                return _xmldoc.Elements("ProductCategory").Elements("AddCategory").Elements("Name").Select(x => Convert.ToString(x.Value)).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the delete product category.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDeleteProductCategory()
        {
            try
            {
                return _xmldoc.Elements("ProductCategory").Elements("DeleteCategory").Elements("Name").Select(x => Convert.ToString(x.Value)).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        #endregion
    }
}
