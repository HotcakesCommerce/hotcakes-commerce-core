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
using Hotcakes.Commerce.Contacts;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Orders;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     The OrderSnapshot class is used for Orders and carts placed by customers, merchants, as well as the shopping cart.
    /// </summary>
    /// <remarks>
    ///     This class will generally map to the hcc_Order table in the database.
    /// </remarks>
    [Serializable]
    public class OrderSnapshot
    {
        public OrderSnapshot()
        {
            bvin = string.Empty;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            TimeOfOrderUtc = DateTime.MinValue;
            OrderNumber = string.Empty;
            ThirdPartyOrderId = string.Empty;
            UserEmail = string.Empty;
            UserID = string.Empty;
            CustomProperties = new CustomPropertyCollection();

            PaymentStatus = OrderPaymentStatus.Unknown;
            ShippingStatus = OrderShippingStatus.Unknown;
            IsPlaced = false;
            StatusCode = string.Empty;
            StatusName = string.Empty;

            BillingAddress = new Address();
            ShippingAddress = new Address();

            ItemsTax = 0m;
            ShippingTax = 0m;
            TotalTax = 0m;
            TotalOrderBeforeDiscounts = 0m;
            TotalOrderAfterDiscounts = 0m;
            TotalShippingBeforeDiscounts = 0m;
            TotalShippingDiscounts = 0m;
            TotalOrderDiscounts = 0m;
            TotalHandling = 0m;
            TotalGrand = 0m;

            AffiliateID = null;
            FraudScore = -1m;
            Instructions = string.Empty;
            ShippingMethodId = string.Empty;
            ShippingMethodDisplayName = string.Empty;
            ShippingProviderId = string.Empty;
            ShippingProviderServiceCode = string.Empty;
            UsedCulture = "en-US";
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current order snapshot object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderSnapshotDTO</returns>
        public OrderSnapshotDTO ToDto()
        {
            var dto = new OrderSnapshotDTO();

            dto.AffiliateID = AffiliateID;
            dto.BillingAddress = BillingAddress.ToDto();
            dto.bvin = bvin ?? string.Empty;
            dto.CustomProperties = new List<CustomPropertyDTO>();
            foreach (var prop in CustomProperties)
            {
                dto.CustomProperties.Add(prop.ToDto());
            }
            dto.FraudScore = FraudScore;
            dto.Id = Id;
            dto.Instructions = Instructions ?? string.Empty;
            dto.IsPlaced = IsPlaced;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.OrderNumber = OrderNumber ?? string.Empty;
            dto.PaymentStatus = (OrderPaymentStatusDTO) (int) PaymentStatus;
            dto.ShippingAddress = ShippingAddress.ToDto();
            dto.ShippingMethodDisplayName = ShippingMethodDisplayName ?? string.Empty;
            dto.ShippingMethodId = ShippingMethodId ?? string.Empty;
            dto.ShippingProviderId = ShippingProviderId ?? string.Empty;
            dto.ShippingProviderServiceCode = ShippingProviderServiceCode ?? string.Empty;
            dto.ShippingStatus = (OrderShippingStatusDTO) (int) ShippingStatus;
            dto.StatusCode = StatusCode ?? string.Empty;
            dto.StatusName = StatusName ?? string.Empty;
            dto.StoreId = StoreId;
            dto.ThirdPartyOrderId = ThirdPartyOrderId ?? string.Empty;
            dto.TimeOfOrderUtc = TimeOfOrderUtc;
            dto.TotalGrand = TotalGrand;
            dto.TotalHandling = TotalHandling;
            dto.TotalOrderBeforeDiscounts = TotalOrderBeforeDiscounts;
            dto.TotalOrderDiscounts = TotalOrderDiscounts;
            dto.TotalShippingBeforeDiscounts = TotalShippingBeforeDiscounts;
            dto.TotalShippingDiscounts = TotalShippingDiscounts;
            dto.ItemsTax = ItemsTax;
            dto.ShippingTax = ShippingTax;
            dto.TotalTax = TotalTax;
            dto.UserEmail = UserEmail ?? string.Empty;
            dto.UserID = UserID ?? string.Empty;

            return dto;
        }

        #endregion

        #region Basics

        /// <summary>
        ///     This is an ID that is used primarily for the SQL data source.
        /// </summary>
        /// <returns>Integer</returns>
        /// <remarks>
        ///     not used as primary key, only for insert order in SQL pages
        /// </remarks>
        public int Id { get; set; } // not used as primary key, only for insert order in SQL pages

        /// <summary>
        ///     This is the primary key to uniquely identify a single order.
        /// </summary>
        /// <returns>String (GUID)</returns>
        /// <remarks>Primary Key</remarks>
        public string bvin { get; set; } // Primary Key

        /// <summary>
        ///     The identifier of the store that this order belongs to.
        /// </summary>
        /// <returns>Long</returns>
        /// <remarks>
        ///     This value will always be the same, except in multi-tenant store scenarios.
        /// </remarks>
        public long StoreId { get; set; }

        /// <summary>
        ///     The date and time that the order was last updated, in UTC format
        /// </summary>
        /// <returns>DateTime (UTC)</returns>
        /// <remarks>UTC</remarks>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The date and time that the order was placed, in UTC format
        /// </summary>
        /// <returns>DateTime (UTC)</returns>
        /// <remarks>UTC</remarks>
        public DateTime TimeOfOrderUtc { get; set; }

        /// <summary>
        ///     This is a text version of the order number and is assigned once the order reaches the "ToDo" state.
        /// </summary>
        /// <returns>String (numeric or empty)</returns>
        /// <remarks>
        ///     This is assigned during the AssignOrderNumber() workflow task. If a value doesn't exist, this order is likely
        ///     an abandoned cart.
        /// </remarks>
        public string OrderNumber { get; set; }

        /// <summary>
        ///     This property is primarily used for third party payment providers and other integrations, such as PayPal Express
        ///     that have their own order numbers.
        /// </summary>
        /// <returns>String - the format depends on the third party integration</returns>
        /// <remarks>Currently used with the out of the box PayPal Express payment provider.</remarks>
        public string ThirdPartyOrderId { get; set; }

        /// <summary>
        ///     The email address of the person placing the order, usually pulled from the user account.
        /// </summary>
        /// <returns>String (email address)</returns>
        /// <remarks>Either pulled from the user account email address property, or from the guest checkout email address field.</remarks>
        public string UserEmail { get; set; }

        /// <summary>
        ///     The ID that will map to the user in the Hotcakes customer repository.
        /// </summary>
        /// <returns>String (integer ID of the user)</returns>
        /// <remarks>This does not match the CMS UserID. If empty, this is likely a guest checkout.</remarks>
        public string UserID { get; set; }

        /// <summary>
        ///     A collection of custom properties that contain additional meta data about the order.
        /// </summary>
        /// <returns>CustomPropertyCollection</returns>
        /// <remarks>This is an extension point for developers to use for integration, as well as Hotcakes related meta data.</remarks>
        public CustomPropertyCollection CustomProperties { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this order has recurring items.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is recurring; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecurring { get; set; }

        #endregion

        #region Status

        /// <summary>
        ///     This is an object that describes the current status of payment for the order.
        /// </summary>
        /// <returns>OrderPaymentStatus</returns>
        public OrderPaymentStatus PaymentStatus { get; set; }

        /// <summary>
        ///     This is an object that describes the current shipping status of the order.
        /// </summary>
        /// <returns>OrderShippingStatus</returns>
        /// <remarks>
        ///     This value should not be manually set, as it will be set conditionally based upon actions taken by the
        ///     merchant.
        /// </remarks>
        public OrderShippingStatus ShippingStatus { get; set; }

        /// <summary>
        ///     The order is not placed until the order is submitted from "New" to the "ToDo" state.
        /// </summary>
        /// <returns>Boolean</returns>
        /// <remarks>
        ///     This value should not be changed manually. The value will always return true for orders that successfully are
        ///     processed during checkout.
        /// </remarks>
        public bool IsPlaced { get; set; }

        /// <summary>
        ///     This is the ID of the payment status code of the order.
        /// </summary>
        /// <returns>String (GUID)</returns>
        /// <remarks>This should match the bvin property of the OrderPaymentStatus object.</remarks>
        public string StatusCode { get; set; }

        /// <summary>
        ///     The name of the status that matches the StatusCode property as well as the store administration views.
        /// </summary>
        /// <returns>String</returns>
        public string StatusName { get; set; }

        /// <summary>
        ///     Gets or sets the culture that was last used when creating the order
        /// </summary>
        public string UsedCulture { get; set; }

        #endregion

        #region Addresses

        /// <summary>
        ///     Address object for who is being billed for the order.
        /// </summary>
        public Address BillingAddress { get; set; }

        /// <summary>
        ///     Address object for who is receiving the order, when shipped.
        /// </summary>
        public Address ShippingAddress { get; set; }

        #endregion

        #region Totals

        /// <summary>
        ///     Total amount of tax for the line items in the order.
        /// </summary>
        public decimal ItemsTax { get; set; }

        /// <summary>
        ///     Amount of tax for shipping.
        /// </summary>
        public decimal ShippingTax { get; set; }

        /// <summary>
        ///     Total amount of tax for the order.
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        ///     The order total without discounts applied, but with user supplied price line items included. This does not include
        ///     shipping charges.
        /// </summary>
        public decimal TotalOrderBeforeDiscounts { get; set; }

        /// <summary>
        ///     Total amount of the order after discounts have been applied. This does not include shipping charges and discounts.
        /// </summary>
        /// <remarks>This value will never be less than zero.</remarks>
        public decimal TotalOrderAfterDiscounts { get; set; }

        /// <summary>
        ///     Total amount for shipping before discounts are applied.
        /// </summary>
        public decimal TotalShippingBeforeDiscounts { get; set; }

        /// <summary>
        ///     Returns a sum of all of the shipping discounts applied to the order.
        /// </summary>
        public decimal TotalShippingDiscounts { get; set; }

        /// <summary>
        ///     The sum of the discounts being applied to this order, excluding shipping discounts.
        /// </summary>
        public decimal TotalOrderDiscounts { get; set; }

        /// <summary>
        ///     Total amount for handling charges.
        /// </summary>
        public decimal TotalHandling { get; set; }

        /// <summary>
        ///     The order grand total that includes all discounts, shipping, and VAT.
        /// </summary>
        public decimal TotalGrand { get; set; }

        #endregion

        #region Others

        /// <summary>
        ///     If the order is being attributed to an affiliate, this ID will not be null.
        /// </summary>
        public long? AffiliateID { get; set; }

        /// <summary>
        ///     The value determined by the fraud screening configuration.
        /// </summary>
        public decimal FraudScore { get; set; }

        /// <summary>
        ///     Special instructions saved by the customer that submitted the order.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        ///     Unique ID of the chosen shipping method.
        /// </summary>
        public string ShippingMethodId { get; set; }

        /// <summary>
        ///     A localized display name matching the shipping method ID.
        /// </summary>
        public string ShippingMethodDisplayName { get; set; }

        /// <summary>
        ///     Unique ID of the provider to be used for shipment.
        /// </summary>
        public string ShippingProviderId { get; set; }

        /// <summary>
        ///     A code used by the shipping provider to indicate the type of service.
        /// </summary>
        public string ShippingProviderServiceCode { get; set; }

        #endregion
    }
}