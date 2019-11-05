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
using System.Xml;
using Hotcakes.Commerce.Contacts;
using Hotcakes.CommerceDTO.v1.Shipping;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    ///     This is the primary class used for all shippable items in the application
    /// </summary>
    [Serializable]
    public class ShippableItem
    {
        public ShippableItem()
        {
            IsNonShipping = false;
            ExtraShipFee = 0m;
            Weight = 0m;
            Length = 0m;
            Width = 0m;
            Height = 0m;
            ShippingSource = ShippingMode.ShipFromSite;
            ShippingSourceId = string.Empty;
            ShipSeparately = false;
            ShippingSourceAddress = new Address();
        }

        /// <summary>
        ///     If true, the associated product will not be shipped and therefore should not have shipping logic applied.
        /// </summary>
        public bool IsNonShipping { get; set; }

        /// <summary>
        ///     If greater than zero, the specified fee should be added to the shipping fee presented to the customer.
        /// </summary>
        public decimal ExtraShipFee { get; set; }

        /// <summary>
        ///     The shippable weight of the product in pounds.
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        ///     The shippable length of the product in inches.
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        ///     The shippable width of the product in inches.
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        ///     The shippable height of the product in inches.
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        ///     This defines where the product will be shipped from.
        /// </summary>
        public ShippingMode ShippingSource { get; set; }

        /// <summary>
        ///     This property contains a collection of address information for the location that the product will be shipped from.
        /// </summary>
        public Address ShippingSourceAddress { get; set; }

        /// <summary>
        ///     This ID value should match a vendor or manufacture when that respective ShippingSource is specified.
        /// </summary>
        public string ShippingSourceId { get; set; }

        /// <summary>
        ///     If true, the associated product cannot be shipped with other products.
        /// </summary>
        public bool ShipSeparately { get; set; }

        #region XML

        /// <summary>
        ///     Allows for this class to be populated from an XML data source.
        /// </summary>
        /// <param name="xml">String - the XML serialized form of the ShippingItem.</param>
        public void FromXml(string xml)
        {
            var sw = new StringReader(xml);
            var xr = XmlReader.Create(sw);
            FromXml(ref xr);
            sw.Dispose();
            xr.Close();
        }

        /// <summary>
        ///     Allows for this class to be populated from an XML data source.
        /// </summary>
        /// <param name="xr">An instance of an XML reader from which to populate this class.</param>
        /// <returns>If true, no errors occured and the class is populated.</returns>
        public bool FromXml(ref XmlReader xr)
        {
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
                                case "IsNonShipping":
                                    xr.Read();
                                    IsNonShipping = bool.Parse(xr.ReadString());
                                    break;
                                case "ShipSeparately":
                                    xr.Read();
                                    ShipSeparately = bool.Parse(xr.ReadString());
                                    break;
                                case "ExtraShipFee":
                                    xr.Read();
                                    ExtraShipFee = decimal.Parse(xr.ReadString());
                                    break;
                                case "Weight":
                                    xr.Read();
                                    Weight = decimal.Parse(xr.ReadString());
                                    break;
                                case "Length":
                                    xr.Read();
                                    Length = decimal.Parse(xr.ReadString());
                                    break;
                                case "Width":
                                    xr.Read();
                                    Width = decimal.Parse(xr.ReadString());
                                    break;
                                case "Height":
                                    xr.Read();
                                    Height = decimal.Parse(xr.ReadString());
                                    break;
                                case "ShippingSource":
                                    xr.Read();
                                    ShippingSource = (ShippingMode) int.Parse(xr.ReadString());
                                    break;
                                case "ShippingSourceId":
                                    xr.Read();
                                    ShippingSourceId = xr.ReadString();
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
        ///     This method allows for this class to be serialized into XML for use in other parts of the application and API.
        /// </summary>
        /// <param name="xw">A reference to an existing XmlWriter</param>
        public void ToXmlWriter(ref XmlWriter xw)
        {
            xw.WriteStartElement("ShippableItem");

            xw.WriteStartElement("IsNonShipping");
            xw.WriteValue(IsNonShipping);
            xw.WriteEndElement();
            xw.WriteStartElement("ShipSeparately");
            xw.WriteValue(ShipSeparately);
            xw.WriteEndElement();
            Xml.WriteDecimal("ExtraShipFee", ExtraShipFee, ref xw);
            Xml.WriteDecimal("Weight", Weight, ref xw);
            Xml.WriteDecimal("Length", Length, ref xw);
            Xml.WriteDecimal("Width", Width, ref xw);
            Xml.WriteDecimal("Height", Height, ref xw);
            Xml.WriteInt("ShippingSource", (int) ShippingSource, ref xw);
            xw.WriteElementString("ShippingSourceId", ShippingSourceId);

            xw.WriteEndElement();
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current ShippableItem object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ShippableItemDTO</returns>
        public ShippableItemDTO ToDto()
        {
            var dto = new ShippableItemDTO();
            dto.ExtraShipFee = ExtraShipFee;
            dto.Height = Height;
            dto.IsNonShipping = IsNonShipping;
            dto.Length = Length;
            dto.ShippingSource = (ShippingModeDTO) (int) ShippingSource;
            dto.ShippingSourceId = ShippingSourceId;
            dto.ShipSeparately = ShipSeparately;
            dto.Weight = Weight;
            dto.Width = Width;
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current ShippableItem object using a ShippableItemDTO instance
        /// </summary>
        /// <param name="dto">An instance of the ShippableItem from the REST API</param>
        public void FromDto(ShippableItemDTO dto)
        {
            ExtraShipFee = dto.ExtraShipFee;
            Height = dto.Height;
            IsNonShipping = dto.IsNonShipping;
            Length = dto.Length;
            ShippingSource = (ShippingMode) (int) dto.ShippingSource;
            ShippingSourceId = dto.ShippingSourceId ?? string.Empty;
            ShipSeparately = dto.ShipSeparately;
            Weight = dto.Weight;
            Width = dto.Width;
        }

        #endregion
    }
}