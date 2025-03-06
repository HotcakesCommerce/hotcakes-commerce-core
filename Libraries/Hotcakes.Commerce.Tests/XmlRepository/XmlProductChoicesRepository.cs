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
    public class XmlProductChoicesRepository : IXmlProductChoicesRepository
    {

        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlProductChoicesRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.ProductChoices)).GetXml();
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

        #region Product Choice Repository Service

        /// <summary>
        /// Gets the total product shared choice count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductSharedChoiceCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductSharedChoiceCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the total product choice count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalProductChoiceCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("TotalProductChoiceCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the name of the delete product choice.
        /// </summary>
        /// <returns></returns>
        public int GetDeleteProductChoiceName()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("DeleteChoice").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the name of the add product shared choice.
        /// </summary>
        /// <returns></returns>
        public string GetAddProductSharedChoiceName()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("AddSharedChoice").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the type of the add product choice.
        /// </summary>
        /// <returns></returns>
        public int[] GetAddProductChoiceTypeCode()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").FirstOrDefault();
                return element == null ? new int[] { 100 } : Convert.ToString(element.Element("AddChoiceTypeCode").Value).Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            }
            catch (Exception)
            {
                return new int[] { 100 };
            }
        }

        /// <summary>
        /// Gets the name of the edit product choice.
        /// </summary>
        /// <returns></returns>
        public int[] GetEditProductChoiceName()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("EditChoice").FirstOrDefault();
                return element == null ? new int[] { 100 } : Convert.ToString(element.Element("OldName").Value).Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            }
            catch (Exception)
            {
                return new int[] { 0 };
            }
        }

        /// <summary>
        /// Gets the edit product choice.
        /// </summary>
        /// <returns></returns>
        public Option GetEditProductChoice()
        {
            try
            {
                return _xmldoc.Elements("ProductChoices").Elements("EditChoice").Select(x => new Option
                    {
                        Name = Convert.ToString(x.Element("Name").Value),
                        NameIsHidden = x.Element("NameIsHidden") != null && Convert.ToBoolean(x.Element("NameIsHidden").Value),
                        IsColorSwatch = x.Element("IsColorSwatch") != null && Convert.ToBoolean(x.Element("IsColorSwatch").Value),
                        IsVariant = x.Element("IsVariant") != null && Convert.ToBoolean(x.Element("IsVariant").Value),
                    }).FirstOrDefault();
            }
            catch (Exception)
            {
                return new Option();
            }
        }

        /// <summary>
        /// Sets the product choice HTML.
        /// </summary>
        /// <param name="choice">The option.</param>
        public void SetProductChoiceInfo(ref Option choice)
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("EditChoice").FirstOrDefault();
                if (element == null) return;
                switch (choice.OptionType)
                {
                    case OptionTypes.Html:
                        choice.TextSettings.AddOrUpdate("html", Convert.ToString(element.Element("HTML").Value));
                        break;
                    case OptionTypes.TextInput:
                        var ti = (Hotcakes.Commerce.Catalog.Options.TextInput)choice.Processor;
                        ti.SetColumns(choice, Convert.ToString(element.Element("Column").Value));
                        ti.SetRows(choice, Convert.ToString(element.Element("Row").Value));
                        ti.SetMaxLength(choice, Convert.ToString(element.Element("MaxLength").Value));
                        break;
                    case OptionTypes.FileUpload:
                        var fu = (Hotcakes.Commerce.Catalog.Options.FileUpload)choice.Processor;
                        fu.SetMultipleFiles(choice, (element.Element("MultipleFile") != null && Convert.ToBoolean(element.Element("MultipleFile").Value)));
                        break;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Sets the product choice item.
        /// </summary>
        /// <param name="choice">The option.</param>
        public void AddProductChoiceItem(ref Option choice)
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("EditChoice").FirstOrDefault();
                if (element == null) return;
                foreach (var item in Convert.ToString(element.Element("AddOptionItem").Value).Split(','))
                {
                    choice.AddItem(item);
                }

            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Deletes the product choice item.
        /// </summary>
        /// <param name="choice">The choice.</param>
        public void DeleteProductChoiceItem(ref Option choice)
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("EditChoice").FirstOrDefault();
                if (element == null) return;

                foreach (var item in Convert.ToString(element.Element("DeleteOptionItem").Value).Split(','))
                {
                    var i = choice.Items.FirstOrDefault(x => x.Name.Equals(item));
                    choice.Items.Remove(i);
                }


            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Edits the product choice item.
        /// </summary>
        /// <param name="choice">The choice.</param>
        public void EditProductChoiceItem(ref Option choice)
        {
            try
            {
                var elementall = _xmldoc.Elements("ProductChoices").Elements("EditChoice").Elements("EditOptionItem");
                if (elementall == null || choice == null) return;

                foreach (var element in elementall)
                {
                    var itemname = Convert.ToString(element.Element("OldName").Value);

                    choice.Items.FirstOrDefault(x => x.Name.Equals(itemname)).IsLabel = element.Element("IsLabel") != null &&
                                                                                        Convert.ToBoolean(
                                                                                            element.Element("IsLabel").Value);
                    choice.Items.FirstOrDefault(x => x.Name.Equals(itemname)).IsDefault = element.Element("IsDefault") !=
                                                                                          null &&
                                                                                          Convert.ToBoolean(
                                                                                              element.Element("IsDefault")
                                                                                                     .Value);
                    choice.Items.FirstOrDefault(x => x.Name.Equals(itemname)).PriceAdjustment = element.Element("PriceAdjustment") == null
                                                                                                    ? 0
                                                                                                    : Convert.ToInt32(
                                                                                                        element.Element("PriceAdjustment").Value);
                    choice.Items.FirstOrDefault(x => x.Name.Equals(itemname)).WeightAdjustment = element.Element("WeightAdjustment") == null
                                                                                                    ? 0
                                                                                                    : Convert.ToInt32(
                                                                                                        element.Element("WeightAdjustment").Value);
                    choice.Items.FirstOrDefault(x => x.Name.Equals(itemname)).Name = Convert.ToString(element.Element("Name").Value);

                }
            }
            catch (Exception)
            {
                return;
            }
        }



        #region ProductXOption
        /// <summary>
        /// Finds all count.
        /// </summary>
        /// <returns></returns>
        public int FindAll_PXO_Count()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductXOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Finds all for all stores count.
        /// </summary>
        /// <returns></returns>
        public int FindAll_PXO_ForAllStoresCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductXOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllForAllStoresCount").Value);
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
        public int FindAll_PXO_PagedCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductXOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllPagedCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Finds for product count.
        /// </summary>
        /// <returns></returns>
        public int Find_PXO_ForProductCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductXOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindForProductCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Finds for option count.
        /// </summary>
        /// <returns></returns>
        public int Find_PXO_ForOptionCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductXOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindForOptionCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        /// <returns></returns>
        public string GetOptionName()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductXOption").FirstOrDefault();
                return element == null ? string.Empty : Convert.ToString(element.Element("OptionName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion


        #region ProductOption

        /// <summary>
        /// Finds the all product option count.
        /// </summary>
        /// <returns></returns>
        public int FindAll_PO_Count()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindAllCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Find product option by product identifier count.
        /// </summary>
        /// <returns></returns>
        public int Find_PO_ByProductIdCount()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindByProductIdCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Finds the many product option count.
        /// </summary>
        /// <returns></returns>
        public int FindMany_PO_Count()
        {
            try
            {
                var element = _xmldoc.Elements("ProductChoices").Elements("ProductOption").FirstOrDefault();
                return element == null ? 0 : Convert.ToInt32(element.Element("FindManyCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the product option list.
        /// </summary>
        /// <returns></returns>
        public List<string> GetProductOptionList()
        {
            try
            {
                return
                    _xmldoc.Elements("ProductChoices")
                           .Elements("ProductOption")
                           .Elements("MergeProductOption")
                           .Elements("Name")
                           .Select(x => Convert.ToString(x.Value))
                           .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        #endregion


        #region ProductOptionItems

        #endregion

        #endregion

    }
}
