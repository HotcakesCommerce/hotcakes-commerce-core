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
using System.Runtime.Serialization;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Marketing;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is Order. There is also a OrderSnapshot that is used for better performance.</remarks>
    [DataContract]
    [Serializable]
    public class OrderDTO
    {
        public OrderDTO()
        {
            BillingAddress = new AddressDTO();
            Bvin = string.Empty;
            Coupons = new List<OrderCouponDTO>();
            CustomProperties = new List<CustomPropertyDTO>();
            Instructions = string.Empty;
            Items = new List<LineItemDTO>();
            LastUpdatedUtc = DateTime.UtcNow;
            Notes = new List<OrderNoteDTO>();
            OrderDiscountDetails = new List<DiscountDetailDTO>();
            OrderNumber = string.Empty;
            Packages = new List<OrderPackageDTO>();
            PaymentStatus = OrderPaymentStatusDTO.Unknown;
            ShippingAddress = new AddressDTO();
            ShippingDiscountDetails = new List<DiscountDetailDTO>();
            ShippingMethodDisplayName = string.Empty;
            ShippingMethodId = string.Empty;
            ShippingProviderId = string.Empty;
            ShippingProviderServiceCode = string.Empty;
            ShippingStatus = OrderShippingStatusDTO.Unknown;
            StatusCode = string.Empty;
            StatusName = string.Empty;
            ThirdPartyOrderId = string.Empty;
            TimeOfOrderUtc = DateTime.UtcNow;
            UserEmail = string.Empty;
            UserID = string.Empty;
        }

        #region Sub Items

        /// <summary>
        ///     Contains a list of Coupons that have been applied to this order.
        /// </summary>
        /// <returns>List of OrderCouponDTO</returns>
        [DataMember]
        public List<OrderCouponDTO> Coupons { get; set; }

        /// <summary>
        ///     Contains a list of Line Items (products) that are in the order.
        /// </summary>
        /// <returns>List of LineItemDTO</returns>
        [DataMember]
        public List<LineItemDTO> Items { get; set; }

        /// <summary>
        ///     Contains a complete list of public and private notes that have been saved to the order.
        /// </summary>
        /// <returns>List of OrderNoteDTO</returns>
        [DataMember]
        public List<OrderNoteDTO> Notes { get; set; }

        /// <summary>
        ///     Contains a list of packages that each are a record of shipment for one or more line items.
        /// </summary>
        /// <returns>List of OrderPackageDTO</returns>
        [DataMember]
        public List<OrderPackageDTO> Packages { get; set; }

        #endregion

        #region Basics

        /// <summary>
        ///     This is an ID that is used primarily for the SQL data source.
        /// </summary>
        /// <returns>Integer</returns>
        /// <remarks>
        ///     not used as primary key, only for insert order in SQL pages
        /// </remarks>
        [DataMember]
        public int Id { get; set; } // not used as primary key, only for insert order in SQL pages

        /// <summary>
        ///     This is the primary key to uniquely identify a single order.
        /// </summary>
        /// <returns>String (GUID)</returns>
        /// <remarks>Primary Key</remarks>
        [DataMember]
        public string Bvin { get; set; } // Primary Key

        /// <summary>
        ///     The identifier of the store that this order belongs to.
        /// </summary>
        /// <returns>Long</returns>
        /// <remarks>
        ///     This value will always be the same, except in multi-tenant store scenarios.
        /// </remarks>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The date and time that the order was last updated, in UTC format
        /// </summary>
        /// <returns>DateTime (UTC)</returns>
        /// <remarks>UTC</remarks>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The date and time that the order was placed, in UTC format
        /// </summary>
        /// <returns>DateTime (UTC)</returns>
        /// <remarks>UTC</remarks>
        [DataMember]
        public DateTime TimeOfOrderUtc { get; set; }

        /// <summary>
        ///     This is a text version of the order number and is assigned once the order reaches the "ToDo" state.
        /// </summary>
        /// <returns>String (numeric or empty)</returns>
        /// <remarks>
        ///     This is assigned during the AssignOrderNumber() workflow task. If a value doesn't exist, this order is likely
        ///     an abandoned cart.
        /// </remarks>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        ///     This property is primarily used for third party payment providers and other integrations, such as PayPal Express
        ///     that have their own order numbers.
        /// </summary>
        /// <returns>String - the format depends on the third party integration</returns>
        /// <remarks>Currently used with the out of the box PayPal Express payment provider.</remarks>
        [DataMember]
        public string ThirdPartyOrderId { get; set; }

        /// <summary>
        ///     The email address of the person placing the order, usually pulled from the user account.
        /// </summary>
        /// <returns>String (email address)</returns>
        /// <remarks>Either pulled from the user account email address property, or from the guest checkout email address field.</remarks>
        [DataMember]
        public string UserEmail { get; set; }

        /// <summary>
        ///     The ID that will map to the user in the Hotcakes customer repository.
        /// </summary>
        /// <returns>String (integer ID of the user)</returns>
        /// <remarks>This does not match the CMS UserID. If empty, this is likely a guest checkout.</remarks>
        [DataMember]
        public string UserID { get; set; }

        /// <summary>
        ///     A collection of custom properties that contain additional meta data about the order.
        /// </summary>
        /// <returns>CustomPropertyDTO</returns>
        /// <remarks>This is an extension point for developers to use for integration, as well as Hotcakes related meta data.</remarks>
        [DataMember]
        public List<CustomPropertyDTO> CustomProperties { get; set; }

        #endregion

        #region Status

        /// <summary>
        ///     This is an object that describes the current status of payment for the order.
        /// </summary>
        /// <returns>OrderPaymentStatusDTO</returns>
        [DataMember]
        public OrderPaymentStatusDTO PaymentStatus { get; set; }

        /// <summary>
        ///     This is an object that describes the current shipping status of the order.
        /// </summary>
        /// <returns>OrderShippingStatusDTO</returns>
        /// <remarks>
        ///     This value should not be manually set, as it will be set conditionally based upon actions taken by the
        ///     merchant.
        /// </remarks>
        [DataMember]
        public OrderShippingStatusDTO ShippingStatus { get; set; }

        /// <summary>
        ///     The order is not placed until the order is submitted from "New" to the "ToDo" state.
        /// </summary>
        /// <returns>Boolean</returns>
        /// <remarks>
        ///     This value should not be changed manually. The value will always return true for orders that successfully are
        ///     processed during checkout.
        /// </remarks>
        [DataMember]
        public bool IsPlaced { get; set; }

        /// <summary>
        ///     This is the ID of the payment status code of the order.
        /// </summary>
        /// <returns>String (GUID)</returns>
        /// <remarks>This should match the bvin property of the OrderPaymentStatus object.</remarks>
        [DataMember]
        public string StatusCode { get; set; }

        /// <summary>
        ///     The name of the status that matches the StatusCode property as well as the store administration views.
        /// </summary>
        /// <returns>String</returns>
        [DataMember]
        public string StatusName { get; set; }

        #endregion

        #region Addresses

        /// <summary>
        ///     Address object for who is being billed for the order.
        /// </summary>
        [DataMember]
        public AddressDTO BillingAddress { get; set; }

        /// <summary>
        ///     Address object for who is receiving the order, when shipped.
        /// </summary>
        [DataMember]
        public AddressDTO ShippingAddress { get; set; }

        #endregion

        #region Others

        /// <summary>
        ///     If the order is being attributed to an affiliate, this ID will not be null.
        /// </summary>
        [DataMember]
        public long? AffiliateID { get; set; }

        /// <summary>
        ///     The value determined by the fraud screening configuration.
        /// </summary>
        [DataMember]
        public decimal FraudScore { get; set; }

        /// <summary>
        ///     Special instructions saved by the customer that submitted the order.
        /// </summary>
        [DataMember]
        public string Instructions { get; set; }

        /// <summary>
        ///     Unique ID of the chosen shipping method.
        /// </summary>
        [DataMember]
        public string ShippingMethodId { get; set; }

        /// <summary>
        ///     A localized display name matching the shipping method ID.
        /// </summary>
        [DataMember]
        public string ShippingMethodDisplayName { get; set; }

        /// <summary>
        ///     Unique ID of the provider to be used for shipment.
        /// </summary>
        [DataMember]
        public string ShippingProviderId { get; set; }

        /// <summary>
        ///     A code used by the shipping provider to indicate the type of service.
        /// </summary>
        [DataMember]
        public string ShippingProviderServiceCode { get; set; }

        #endregion

        #region Totals

        /// <summary>
        ///     Total amount of tax for the order.
        /// </summary>
        [DataMember]
        public decimal TotalTax { get; set; }

        /// <summary>
        ///     Total amount of tax for the line items in the order.
        /// </summary>
        [DataMember]
        public decimal ItemsTax { get; set; }

        /// <summary>
        ///     Amount of tax for shipping.
        /// </summary>
        [DataMember]
        public decimal ShippingTax { get; set; }

        /// <summary>
        ///     The tax rate for shipping.
        /// </summary>
        [DataMember]
        public decimal ShippingTaxRate { get; set; }

        /// <summary>
        ///     Total amount for shipping before discounts are applied.
        /// </summary>
        [DataMember]
        public decimal TotalShippingBeforeDiscounts { get; set; }

        /// <summary>
        ///     The order total without discounts applied, but with user supplied price line items included. This does not include
        ///     shipping charges.
        /// </summary>
        [DataMember]
        public decimal TotalOrderBeforeDiscounts { get; set; }

        /// <summary>
        ///     Returns a sum of all of the shipping discounts applied to the order.
        /// </summary>
        [DataMember]
        public decimal TotalShippingDiscounts { get; set; }

        /// <summary>
        ///     The sum of the discounts being applied to this order, excluding shipping discounts.
        /// </summary>
        [DataMember]
        public decimal TotalOrderDiscounts { get; set; }

        /// <summary>
        ///     Total amount for handling charges.
        /// </summary>
        [DataMember]
        public decimal TotalHandling { get; set; }

        /// <summary>
        ///     The order grand total that includes all discounts, shipping, and VAT.
        /// </summary>
        [DataMember]
        public decimal TotalGrand { get; set; }

        /// <summary>
        ///     Contains a listing of the discounts that have been applied to this order.
        /// </summary>
        [DataMember]
        public List<DiscountDetailDTO> OrderDiscountDetails { get; set; }

        /// <summary>
        ///     A listing of all of the discounts for the shipping only.
        /// </summary>
        [DataMember]
        public List<DiscountDetailDTO> ShippingDiscountDetails { get; set; }

        #endregion
    }
}