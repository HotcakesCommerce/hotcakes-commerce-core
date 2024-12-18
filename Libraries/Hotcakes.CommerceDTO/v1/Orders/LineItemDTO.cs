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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Marketing;
using Hotcakes.CommerceDTO.v1.Shipping;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of LineItem in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is LineItem.</remarks>
    [DataContract]
    [Serializable]
    public class LineItemDTO
    {
        public LineItemDTO()
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            BasePricePerItem = 0;
            DiscountDetails = new List<DiscountDetailDTO>();
            OrderBvin = string.Empty;
            ProductId = string.Empty;
            VariantId = string.Empty;
            ProductName = string.Empty;
            ProductSku = string.Empty;
            ProductShortDescription = string.Empty;
            Quantity = 0;
            QuantityReturned = 0;
            QuantityShipped = 0;
            ShippingPortion = 0;
            StatusCode = string.Empty;
            StatusName = string.Empty;
            TaxRate = 0;
            TaxPortion = 0;
            SelectionData = new List<OptionSelectionDTO>();
            IsNonShipping = false;
            TaxSchedule = 0;
            ProductShippingWeight = 0;
            ProductShippingLength = 0;
            ProductShippingWidth = 0;
            ProductShippingHeight = 0;
            CustomProperties = new List<CustomPropertyDTO>();
            ShipFromMode = ShippingModeDTO.None;
            ShipFromNotificationId = string.Empty;
            ShipFromAddress = new AddressDTO();
            ShipSeparately = false;
            ExtraShipCharge = 0;
            ShippingCharge = ShippingChargeTypeDTO.ChargeShippingAndHandling;
            PromotionIds = null;
            FreeQuantity = 0;
            IsUpchargeAllowed = false;
        }

        /// <summary>
        ///     The unique ID of the current line item.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the line item was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The base price of the product matching this line item.
        /// </summary>
        [DataMember]
        public decimal BasePricePerItem { get; set; }

        /// <summary>
        ///     The cumulative total of the line item, which usually accounts for promotions.
        /// </summary>
        [DataMember]
        public decimal LineTotal { get; set; }

        /// <summary>
        ///     The cumulative total of the line item, which usually accounts for quanity and promotions.
        /// </summary>
        [DataMember]
        public decimal AdjustedPricePerItem { get; set; }

        /// <summary>
        ///     Retuns true if the product matching this line item is a user supplied price product.
        /// </summary>
        [DataMember]
        public bool IsUserSuppliedPrice { get; set; }

        /// <summary>
        ///     Retuns true if the product matching this line item is a bundle.
        /// </summary>
        [DataMember]
        public bool IsBundle { get; set; }

        /// <summary>
        ///     Retuns true if the product matching this line item is a gift card.
        /// </summary>
        [DataMember]
        public bool IsGiftCard { get; set; }

        /// <summary>
        ///     A collection of all promotions being applied to this line item.
        /// </summary>
        [DataMember]
        public List<DiscountDetailDTO> DiscountDetails { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the order that this line item belongs to.
        /// </summary>
        [DataMember]
        public string OrderBvin { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the product matching this line item.
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        ///     If the product is a variant, this will contain the unique ID or bvin of the matching variant.
        /// </summary>
        [DataMember]
        public string VariantId { get; set; }

        /// <summary>
        ///     The language-friendly name of the product matching this line item.
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        ///     The unique SKU of the product matching this line item.
        /// </summary>
        [DataMember]
        public string ProductSku { get; set; }

        /// <summary>
        ///     The description of the product matching this line item.
        /// </summary>
        [DataMember]
        public string ProductShortDescription { get; set; }

        /// <summary>
        ///     The number of products in this line item.
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        ///     Used to define how many products in this line item are being returned.
        /// </summary>
        [DataMember]
        public int QuantityReturned { get; set; }

        /// <summary>
        ///     Defines how many products in this line item have been shipped.
        /// </summary>
        [DataMember]
        public int QuantityShipped { get; set; }

        /// <summary>
        ///     The amount of shipping that was spread into this line from the entire order.
        /// </summary>
        [DataMember]
        public decimal ShippingPortion { get; set; }

        /// <summary>
        ///     Gets or sets the shipping charge.
        /// </summary>
        /// <value>
        ///     The shipping charge.
        /// </value>
        [DataMember]
        public ShippingChargeTypeDTO ShippingCharge { get; set; }

        /// <summary>
        ///     A built-in way to save the status code of a line item when integrating with other systems.
        /// </summary>
        [DataMember]
        public string StatusCode { get; set; }

        /// <summary>
        ///     A built-in way to save the status name of a line item when integrating with other systems.
        /// </summary>
        [DataMember]
        public string StatusName { get; set; }

        /// <summary>
        ///     The rate that the products this line item should be taxed at.
        /// </summary>
        [DataMember]
        public decimal TaxRate { get; set; }

        /// <summary>
        ///     The amount of tax that was spread into this line from the entire order.
        /// </summary>
        [DataMember]
        public decimal TaxPortion { get; set; }

        /// <summary>
        ///     The choices/options that were selected when adding the product to this line item.
        /// </summary>
        [DataMember]
        public List<OptionSelectionDTO> SelectionData { get; set; }

        /// <summary>
        ///     Reflects whether the matching product for this line item is shippable or not.
        /// </summary>
        [DataMember]
        public bool IsNonShipping { get; set; }

        /// <summary>
        ///     The unique ID of the tax schedule used for this line item.
        /// </summary>
        [DataMember]
        public long TaxSchedule { get; set; }

        /// <summary>
        ///     The weight of a single instance of the product in this line item.
        /// </summary>
        [DataMember]
        public decimal ProductShippingWeight { get; set; }

        /// <summary>
        ///     The length of a single instance of the product in this line item.
        /// </summary>
        [DataMember]
        public decimal ProductShippingLength { get; set; }

        /// <summary>
        ///     The width of a single instance of the product in this line item.
        /// </summary>
        [DataMember]
        public decimal ProductShippingWidth { get; set; }

        /// <summary>
        ///     The height of a single instance of the product in this line item.
        /// </summary>
        [DataMember]
        public decimal ProductShippingHeight { get; set; }

        /// <summary>
        ///     A collection of the custom properties for this line item - great to be used for integration.
        /// </summary>
        [DataMember]
        public List<CustomPropertyDTO> CustomProperties { get; set; }

        /// <summary>
        ///     Defines where the product should be shipped from to help support multiple warehouses and/or drop-shipping.
        /// </summary>
        [DataMember]
        public ShippingModeDTO ShipFromMode { get; set; }

        /// <summary>
        ///     The unique ID of the shipping source from the matching product.
        /// </summary>
        [DataMember]
        public string ShipFromNotificationId { get; set; }

        /// <summary>
        ///     The address of the source where the product will be shipped from.
        /// </summary>
        [DataMember]
        public AddressDTO ShipFromAddress { get; set; }

        /// <summary>
        ///     Reflects whether the matching product for this line item must be shipped separately or not.
        /// </summary>
        [DataMember]
        public bool ShipSeparately { get; set; }

        /// <summary>
        ///     A fixed amount added to the line item from the product in addition to the shipping fee required from the shipping
        ///     provider.
        /// </summary>
        [DataMember]
        public decimal ExtraShipCharge { get; set; }

        /// <summary>
        ///     Gets or sets if item is free.
        /// </summary>
        [DataMember]
        public string PromotionIds { get; set; }

        /// <summary>
        ///     Gets or sets how many items are free in the line item.
        /// </summary>
        [DataMember]
        public int FreeQuantity { get; set; }

        /// <summary>
        ///     Retuns true if the product matching this line item is Cover Credit Card Fees.
        /// </summary>
        [DataMember]
        public bool IsUpchargeAllowed { get; set; }
    }
}