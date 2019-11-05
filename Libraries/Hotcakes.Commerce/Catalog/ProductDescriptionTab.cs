#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.IO;
using System.Text;
using System.Xml;
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This object holds a single instance of an "Info Tab" that customers use to list product specs and other details.
    /// </summary>
    [Serializable]
    public class ProductDescriptionTab : IEquatable<ProductDescriptionTab>
    {
        private readonly XmlReaderSettings _hccXmlReaderSettings = new XmlReaderSettings();


        private readonly XmlWriterSettings _hccXmlWriterSettings = new XmlWriterSettings();

        public ProductDescriptionTab()
        {
            Bvin = Guid.NewGuid().ToString().Replace("{", string.Empty).Replace("}", string.Empty);
            LastUpdated = DateTime.MinValue;
            SortOrder = 1;
        }

        /// <summary>
        ///     The unique ID &amp; primary key of the info tab.
        /// </summary>
        public virtual string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the info tab was last updated.
        /// </summary>
        public virtual DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The localized title as you want a customer to see it.
        /// </summary>
        public string TabTitle { get; set; }

        /// <summary>
        ///     This is the content of the info tab that is shown to customers.
        /// </summary>
        public string HtmlData { get; set; }

        /// <summary>
        ///     Sorting is supported by giving each info tab a sequential number.
        /// </summary>
        public int SortOrder { get; set; }

        protected XmlWriterSettings HccXmlWriterSettings
        {
            get { return _hccXmlWriterSettings; }
        }

        protected XmlReaderSettings HccXmlReaderSettings
        {
            get { return _hccXmlReaderSettings; }
        }

        public bool Equals(ProductDescriptionTab other)
        {
            return Bvin == other.Bvin
                   && TabTitle == other.TabTitle
                   && HtmlData == other.HtmlData
                   && SortOrder == other.SortOrder;
        }

        /// <summary>
        ///     This method is meant to output the info tab as XML.
        /// </summary>
        /// <param name="omitDeclaration">Paramter not used.</param>
        /// <returns>String - an XML representation of the info tab.</returns>
        /// <remarks>The application does not use this method anywhere. </remarks>
        public virtual string ToXml(bool omitDeclaration)
        {
            var response = string.Empty;
            var sb = new StringBuilder();
            _hccXmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;

            // this doesn't appear that it will actually output anything but an empty string...
            var xw = XmlWriter.Create(sb, _hccXmlWriterSettings);

            ToXmlWriter(ref xw);
            xw.Flush();
            xw.Close();

            response = sb.ToString();

            return response;
        }

        /// <summary>
        ///     This method accepts an XML representation of an info tab and populates the info tab object with its contents.
        /// </summary>
        /// <param name="x">String - the XML serialized version of an info tab.</param>
        /// <returns>Boolean - if true, the conversion from XML to an object happened successfully.</returns>
        /// <remarks>This method is only used in the Product.cs class.</remarks>
        public virtual bool FromXmlString(string x)
        {
            var sw = new StringReader(x);
            var xr = XmlReader.Create(sw);
            var result = FromXml(ref xr);
            sw.Dispose();
            xr.Close();
            return result;
        }

        /// <summary>
        ///     This method accepts an XmlReader and converts its contents to populate the current info tab properties.
        /// </summary>
        /// <param name="xr">XmlReader - a streamed representation of the XML version of this object</param>
        /// <returns>Boolean - if true, the XmlReader was properly parsed and its contents converted to properties in this object.</returns>
        public bool FromXml(ref XmlReader xr)
        {
            /*
			 * This method is only called by this class and probably can be made Private.
			 */

            var results = false;

            try
            {
                while (xr.Read())
                {
                    if (xr.IsStartElement())
                    {
                        if (!xr.IsEmptyElement)
                        {
                            switch (xr.Name)
                            {
                                case "Bvin":
                                    xr.Read();
                                    Bvin = xr.ReadString();
                                    break;
                                case "TabTitle":
                                    xr.Read();
                                    TabTitle = xr.ReadString();
                                    break;
                                case "HtmlData":
                                    xr.Read();
                                    HtmlData = xr.ReadString();
                                    break;
                                case "SortOrder":
                                    xr.Read();
                                    SortOrder = int.Parse(xr.ReadString());
                                    break;
                            }
                        }
                    }
                }

                results = true;
            }

            catch (XmlException XmlEx)
            {
                EventLog.LogEvent(XmlEx);
                results = false;
            }

            return results;
        }

        /// <summary>
        ///     This method is called when a Product is serialized to XML to help build the XML serialized version of a product.
        /// </summary>
        /// <param name="xw">XmlWriter - a byreference instance of an XmlWriter to transfer the serialized info tab to.</param>
        public void ToXmlWriter(ref XmlWriter xw)
        {
            if (xw != null)
            {
                xw.WriteStartElement("ProductDescriptionTab");
                xw.WriteElementString("Bvin", Bvin);
                xw.WriteElementString("TabTitle", TabTitle);
                xw.WriteElementString("HtmlData", HtmlData);
                xw.WriteElementString("SortOrder", SortOrder.ToString());
                xw.WriteEndElement();
            }
        }

        #region DTO

        /// <summary>
        ///     Allows you to populate the current info tab object using a ProductDescriptionTabDTO instance
        /// </summary>
        /// <param name="dto">An instance of the info tab from the REST API</param>
        public void FromDto(ProductDescriptionTabDTO dto)
        {
            Bvin = dto.Bvin;
            HtmlData = dto.HtmlData;
            SortOrder = dto.SortOrder;
            TabTitle = dto.TabTitle;
        }

        /// <summary>
        ///     Allows you to convert the current info tab object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductDescriptionTabDTO</returns>
        public ProductDescriptionTabDTO ToDto()
        {
            var dto = new ProductDescriptionTabDTO();
            dto.Bvin = Bvin;
            dto.HtmlData = HtmlData;
            dto.SortOrder = SortOrder;
            dto.TabTitle = TabTitle;
            return dto;
        }

        #endregion
    }
}