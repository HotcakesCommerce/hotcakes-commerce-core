#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Shipping;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.CommerceDTO.v1.Shipping;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order Package
    /// </summary>
    /// <remarks>The REST API equivalent is OrderPackageDTO.</remarks>
    [Serializable]
    public class OrderPackage : IReplaceable
    {
        private const string TrackingNumberMarkup = "<a href=\"{0}\">{1}</a>";

        public OrderPackage()
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            Items = new List<OrderPackageItem>();
            Description = string.Empty;
            OrderId = string.Empty;
            Width = 0m;
            Height = 0m;
            Length = 0m;
            SizeUnits = LengthType.Inches;
            Weight = 0m;
            WeightUnits = WeightType.Pounds;
            ShippingProviderId = string.Empty;
            ShippingProviderServiceCode = string.Empty;
            ShippingMethodId = string.Empty;
            TrackingNumber = string.Empty;
            HasShipped = false;
            ShipDateUtc = DateTime.MinValue;
            EstimatedShippingCost = 0m;
            CustomProperties = new CustomPropertyCollection();
        }

        /// <summary>
        ///     A collection of the tokens and the replaceable content for email templates.
        /// </summary>
        /// <param name="context">An instance of the Hotcakes Request context.</param>
        /// <returns>List of HtmlTemplateTag</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();
            result.Add(new HtmlTemplateTag("[[Package.ShipDate]]", ShipDateUtc.ToString("d")));

            if (TrackingNumber == string.Empty)
            {
                result.Add(new HtmlTemplateTag("[[Package.TrackingNumber]]", "None Available"));
            }
            else
            {
                result.Add(new HtmlTemplateTag("[[Package.TrackingNumber]]", TrackingNumber));
            }

            var tagsEntered = false;
            var currentStore = context.CurrentStore;

            foreach (var item in AvailableServices.FindAll(currentStore))
            {
                if (item.Id == ShippingProviderId)
                {
                    tagsEntered = true;

                    var trackingUrl = string.Empty;
                    if (item.IsSupportsTracking && !string.IsNullOrEmpty(TrackingNumber))
                    {
                        trackingUrl = item.GetTrackingUrl(TrackingNumber);
                        result.Add(new HtmlTemplateTag("[[Package.TrackingNumberMarkup]]",
                            string.Format(TrackingNumberMarkup, trackingUrl, TrackingNumber)));
                    }
                    else
                    {
                        result.Add(new HtmlTemplateTag("[[Package.TrackingNumberMarkup]]", TrackingNumber));
                    }
                    result.Add(new HtmlTemplateTag("[[Package.TrackingNumberLink]]", trackingUrl));

                    var serviceCodes = item.ListAllServiceCodes();
                    var shipperServiceFound = false;

                    foreach (var serviceCode in serviceCodes)
                    {
                        if (string.Compare(ShippingProviderServiceCode, serviceCode.Code, true) == 0)
                        {
                            shipperServiceFound = true;
                            result.Add(new HtmlTemplateTag("[[Package.ShipperService]]", serviceCode.DisplayName));
                            break;
                        }
                    }

                    var shipperName = string.Empty;
                    var orderService = Factory.CreateService<OrderService>(context);
                    var order = orderService.Orders.FindForCurrentStore(OrderId);
                    if (order != null && order.bvin != string.Empty)
                    {
                        if (!string.IsNullOrEmpty(ShippingMethodId))
                        {
                            var shippingMethod = orderService.ShippingMethods.Find(ShippingMethodId);
                            shipperName = shippingMethod.Name;
                        }
                        else
                        {
                            if (shipperServiceFound)
                            {
                                shipperName = item.Name;
                            }
                            else
                            {
                                shipperName = order.ShippingMethodDisplayName;
                            }
                        }
                    }

                    if (!shipperServiceFound)
                    {
                        result.Add(new HtmlTemplateTag("[[Package.ShipperService]]", shipperName));
                    }

                    result.Add(new HtmlTemplateTag("[[Package.ShipperName]]", shipperName));
                }
            }

            if ((Items != null) && (Items.Count > 0))
            {
                var sb = new StringBuilder();
                sb.Append("<table class=\"packageitems\">");
                sb.Append("<tr>");
                sb.Append("<td class=\"itemnamehead\">Name</td>");
                sb.Append("<td class=\"itemquantityhead\">Quantity</td>");
                sb.Append("</tr>");
                //sb.Append("<ul>")
                var count = 0;
                foreach (var item in Items)
                {
                    if (item.Quantity > 0)
                    {
                        if (count%2 == 0)
                        {
                            sb.Append("<tr>");
                        }
                        else
                        {
                            sb.Append("<tr class=\"alt\">");
                        }

                        //sb.Append("<li>")
                        var productRepository = Factory.CreateRepo<ProductRepository>(context);
                        var prod = productRepository.FindWithCache(item.ProductBvin);
                        if (prod != null)
                        {
                            sb.Append("<td class=\"itemname\">");
                            sb.Append(prod.ProductName);
                            sb.Append("</td>");
                            sb.Append("<td class=\"itemquantity\">");
                            sb.Append(item.Quantity.ToString());
                            sb.Append("</td>");
                        }
                        //sb.Append("</li>")
                        sb.Append("</tr>");
                        count += 1;
                    }
                }
                sb.Append("</table>");

                //sb.Append("</ul>")
                result.Add(new HtmlTemplateTag("[[Package.Items]]", sb.ToString()));
            }
            else
            {
                result.Add(new HtmlTemplateTag("[[Package.Items]]", string.Empty));
            }

            //these are only here so that they get added to the list of available tags
            if (!tagsEntered)
            {
                result.Add(new HtmlTemplateTag("[[Package.TrackingNumberLink]]", string.Empty));
                result.Add(new HtmlTemplateTag("[[Package.TrackingNumberMarkup]]", string.Empty));
                result.Add(new HtmlTemplateTag("[[Package.ShipperName]]", string.Empty));
                result.Add(new HtmlTemplateTag("[[Package.ShipperService]]", string.Empty));
            }

            return result;
        }

        /// <summary>
        ///     Creates a copy of the current order package.
        /// </summary>
        /// <returns>OrderPackage</returns>
        public OrderPackage Clone()
        {
            var memoryStream = new MemoryStream();

            var formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, this);
            memoryStream.Position = 0;

            var newPackage = (OrderPackage) formatter.Deserialize(memoryStream);

            return newPackage;
        }

        #region Properties

        /// <summary>
        ///     This is the ID of the current order package.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the order package was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     A listing of the products or line items in the order package
        /// </summary>
        public List<OrderPackageItem> Items { get; set; }

        /// <summary>
        ///     A description of the package consisting of the SKU and product names.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The unique ID of the order that this package belongs to.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        ///     The width of the package.
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        ///     The height of the package.
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        ///     The length of the package.
        /// </summary>
        public decimal Length { get; set; }

        /// <summary>
        ///     The units used to measure the dimensions of the package.
        /// </summary>
        public LengthType SizeUnits { get; set; }

        /// <summary>
        ///     The weight of the package.
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        ///     The units used to measure the weight of the package.
        /// </summary>
        public WeightType WeightUnits { get; set; }

        /// <summary>
        ///     The unique ID of the shipping provider used for this package.
        /// </summary>
        public string ShippingProviderId { get; set; }

        /// <summary>
        ///     The service code used by the shipping provider to describe the type of package.
        /// </summary>
        public string ShippingProviderServiceCode { get; set; }

        /// <summary>
        ///     A unique ID used by the shipping provider to track the package.
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        ///     A boolean value to determine if the package has shipped or not.
        /// </summary>
        public bool HasShipped { get; set; }

        /// <summary>
        ///     The date/time stamp with the package shipped.
        /// </summary>
        public DateTime ShipDateUtc { get; set; }

        /// <summary>
        ///     A shipping estimate returned from the shipping provider.
        /// </summary>
        public decimal EstimatedShippingCost { get; set; }

        /// <summary>
        ///     The unique ID of the shipping provider method
        /// </summary>
        public string ShippingMethodId { get; set; }

        /// <summary>
        ///     A collection of settings or meta data used for the package.
        /// </summary>
        /// <remarks>
        ///     Highly useful for things like ERP integrations.
        /// </remarks>
        public CustomPropertyCollection CustomProperties { get; set; }

        #endregion

        #region XML

        /// <summary>
        ///     Creates and returns an XML-serialized version of the line items or products in this package.
        /// </summary>
        /// <returns>String - XML representation of the line items in the current package.</returns>
        public string ItemsToXml()
        {
            var result = string.Empty;

            try
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(Items.GetType());
                xs.Serialize(sw, Items);
                result = sw.ToString();
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = string.Empty;
            }

            return result;
        }

        public bool ItemsFromXml(string data)
        {
            var result = false;

            try
            {
                var tr = new StringReader(data);
                var xs = new XmlSerializer(Items.GetType());
                Items = (List<OrderPackageItem>) xs.Deserialize(tr);
                if (Items != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                Items = new List<OrderPackageItem>();
                result = false;
            }

            return result;
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current order package object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderPackageDTO</returns>
        public OrderPackageDTO ToDto()
        {
            var dto = new OrderPackageDTO();

            dto.CustomProperties = new List<CustomPropertyDTO>();
            foreach (var prop in CustomProperties)
            {
                dto.CustomProperties.Add(prop.ToDto());
            }
            dto.Description = Description ?? string.Empty;
            dto.EstimatedShippingCost = EstimatedShippingCost;
            dto.HasShipped = HasShipped;
            dto.Height = Height;
            dto.Id = Id;
            dto.Items = new List<OrderPackageItemDTO>();
            {
                foreach (var item in Items)
                {
                    dto.Items.Add(item.ToDto());
                }
            }
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.Length = Length;
            dto.OrderId = OrderId ?? string.Empty;
            dto.ShipDateUtc = ShipDateUtc;
            dto.ShippingMethodId = ShippingMethodId ?? string.Empty;
            dto.ShippingProviderId = ShippingProviderId ?? string.Empty;
            dto.ShippingProviderServiceCode = ShippingProviderServiceCode ?? string.Empty;
            dto.SizeUnits = (LengthTypeDTO) (int) SizeUnits;
            dto.StoreId = StoreId;
            dto.TrackingNumber = TrackingNumber ?? string.Empty;
            dto.Weight = Weight;
            dto.WeightUnits = (WeightTypeDTO) (int) WeightUnits;
            dto.Width = Width;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current order package object using a OrderPackageDTO instance
        /// </summary>
        /// <param name="dto">An instance of the order package from the REST API</param>
        public void FromDto(OrderPackageDTO dto)
        {
            if (dto == null) return;

            if (dto.CustomProperties != null)
            {
                CustomProperties.Clear();
                foreach (var prop in dto.CustomProperties)
                {
                    var p = new CustomProperty();
                    p.FromDto(prop);
                    CustomProperties.Add(p);
                }
            }
            Description = dto.Description ?? string.Empty;
            EstimatedShippingCost = dto.EstimatedShippingCost;
            HasShipped = dto.HasShipped;
            Height = dto.Height;
            Id = dto.Id;
            if (dto.Items != null)
            {
                Items.Clear();
                foreach (var item in dto.Items)
                {
                    var pak = new OrderPackageItem();
                    pak.FromDto(item);
                    Items.Add(pak);
                }
            }
            LastUpdatedUtc = dto.LastUpdatedUtc;
            Length = dto.Length;
            OrderId = dto.OrderId ?? string.Empty;
            ShipDateUtc = dto.ShipDateUtc;
            ShippingMethodId = dto.ShippingMethodId ?? string.Empty;
            ShippingProviderId = dto.ShippingProviderId ?? string.Empty;
            ShippingProviderServiceCode = dto.ShippingProviderServiceCode ?? string.Empty;
            SizeUnits = (LengthType) (int) dto.SizeUnits;
            StoreId = dto.StoreId;
            TrackingNumber = dto.TrackingNumber ?? string.Empty;
            Weight = dto.Weight;
            WeightUnits = (WeightType) (int) dto.WeightUnits;
            Width = dto.Width;
        }

        #endregion
    }
}