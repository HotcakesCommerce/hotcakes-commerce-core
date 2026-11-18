#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// Represents the shipping characteristics and origin metadata for a product or other shippable item.
    /// </summary>
    /// <remarks>
    /// The XML representation contains the scalar shipping fields and omits
    /// <see cref="ShippingSourceAddress"/>. The DTO conversion likewise transfers the scalar fields only.
    /// </remarks>
    [Serializable]
    public class ShippableItem
    {
        /// <summary>
        /// Initializes an item with non-shipping disabled, zero fees and dimensions, site shipping as the source,
        /// and an empty source identifier and address.
        /// </summary>
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
        /// Gets or sets whether the associated product is not shipped and should not have shipping logic applied.
        /// </summary>
        public bool IsNonShipping { get; set; }

        /// <summary>
        /// Gets or sets an additional fee to add to the shipping fee presented to the customer when greater than zero.
        /// </summary>
        public decimal ExtraShipFee { get; set; }

        /// <summary>
        /// Gets or sets the shippable product weight in pounds.
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the shippable product length in inches.
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        /// Gets or sets the shippable product width in inches.
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// Gets or sets the shippable product height in inches.
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// Gets or sets the mode that identifies where the product will be shipped from.
        /// </summary>
        public ShippingMode ShippingSource { get; set; }

        /// <summary>
        /// Gets or sets the address of the location from which the product will be shipped.
        /// </summary>
        /// <remarks>This property is retained in memory but is not included by <see cref="ToXmlWriter"/> or <see cref="ToDto"/>.</remarks>
        public Address ShippingSourceAddress { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the vendor or manufacturer when the corresponding <see cref="ShippingSource"/> is used.
        /// </summary>
        public string ShippingSourceId { get; set; }

        /// <summary>
        /// Gets or sets whether the associated product must be shipped separately from other products.
        /// </summary>
        public bool ShipSeparately { get; set; }

        #region XML

        /// <summary>
        /// Populates this item from its XML representation.
        /// </summary>
        /// <param name="xml">The XML serialized form of the shippable item.</param>
        /// <remarks>The serialized source address is not read because <see cref="ToXmlWriter"/> does not write it.</remarks>
        public void FromXml(string xml)
        {
            var sw = new StringReader(xml);
            var xr = XmlReader.Create(sw);
            FromXml(ref xr);
            sw.Dispose();
            xr.Close();
        }

        /// <summary>
        /// Populates this item from the current XML reader contents.
        /// </summary>
        /// <param name="xr">An XML reader from which to populate this item.</param>
        /// <returns><see langword="true"/> when parsing completes without an <see cref="XmlException"/>; otherwise <see langword="false"/>.</returns>
        /// <remarks>Only the scalar fields written by <see cref="ToXmlWriter"/> are recognized; unknown elements are ignored.</remarks>
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
        /// Writes this item to an XML writer for use by application and API consumers.
        /// </summary>
        /// <param name="xw">A reference to the XML writer receiving the <c>ShippableItem</c> element.</param>
        /// <remarks>
        /// The output includes <c>IsNonShipping</c>, <c>ShipSeparately</c>, fee, dimensions,
        /// <c>ShippingSource</c>, and <c>ShippingSourceId</c>. It intentionally omits
        /// <see cref="ShippingSourceAddress"/>.
        /// </remarks>
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
        /// Converts this item to its REST API DTO equivalent.
        /// </summary>
        /// <returns>A new <see cref="ShippableItemDTO"/> containing the scalar shipping fields.</returns>
        /// <remarks><see cref="ShippingSourceAddress"/> is not copied to the DTO.</remarks>
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
        /// Populates this item from a REST API DTO.
        /// </summary>
        /// <param name="dto">The DTO containing the scalar shipping fields.</param>
        /// <remarks>The source address is not populated from the DTO.</remarks>
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
