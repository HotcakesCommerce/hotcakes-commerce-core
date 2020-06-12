#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Marketing;
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     The Order class is used for Orders and carts placed by customers, merchants, as well as the shopping cart.
    /// </summary>
    /// <remarks>
    ///     This class will generally map to the hcc_Order table in the database.
    /// </remarks>
    [Serializable]
    public class Order : IReplaceable
    {
        #region Constructor

        public Order()
        {
            Coupons = new List<OrderCoupon>();
            Items = new List<LineItem>();
            Notes = new List<OrderNote>();
            Packages = new List<OrderPackage>();
            Returns = new List<RMA>();

            bvin = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            TimeOfOrderUtc = DateTime.UtcNow;
            OrderNumber = string.Empty;
            ThirdPartyOrderId = string.Empty;
            UserEmail = string.Empty;
            UserID = string.Empty;
            CustomProperties = new CustomPropertyCollection();

            PaymentStatus = OrderPaymentStatus.Unknown;
            ShippingStatus = OrderShippingStatus.Unknown;
            StatusCode = string.Empty;
            StatusName = string.Empty;

            BillingAddress = new Address();
            ShippingAddress = new Address();

            ShippingDiscountDetails = new List<DiscountDetail>();
            OrderDiscountDetails = new List<DiscountDetail>();

            FraudScore = -1m;
            Instructions = string.Empty;
            ShippingMethodId = string.Empty;
            ShippingMethodDisplayName = string.Empty;
            ShippingProviderId = string.Empty;
            ShippingProviderServiceCode = string.Empty;
            UsedCulture = "en-US";
        }

        #endregion

        #region Properties / Sub Items

        /// <summary>
        ///     Contains a list of Coupons that have been applied to this order.
        /// </summary>
        /// <returns>List of OrderCoupon</returns>
        public List<OrderCoupon> Coupons { get; set; }

        /// <summary>
        ///     Contains a list of Line Items (products) that are in the order.
        /// </summary>
        /// <returns>List of LineItem</returns>
        public List<LineItem> Items { get; set; }

        /// <summary>
        ///     Contains a complete list of public and private notes that have been saved to the order.
        /// </summary>
        /// <returns>List of OrderNote</returns>
        public List<OrderNote> Notes { get; set; }

        /// <summary>
        ///     Contains a list of packages that each are a record of shipment for one or more line items.
        /// </summary>
        /// <returns>List of OrderPackage</returns>
        public List<OrderPackage> Packages { get; set; }

        /// <summary>
        ///     Contains a list of refunds for the order.
        /// </summary>
        /// <returns>List of RMA</returns>
        public List<RMA> Returns { get; set; }

        #endregion

        #region Properties / Basics

        /// <summary>
        ///     This is an ID that is used primarily for the SQL data source.
        /// </summary>
        /// <returns>Integer</returns>
        /// <remarks>
        ///     not used as primary key, only for insert order in SQL pages
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        ///     This is the primary key to uniquely identify a single order.
        /// </summary>
        /// <returns>String (GUID)</returns>
        /// <remarks>Primary Key</remarks>
        public string bvin { get; set; }

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
        ///     The device that user used to access site.
        /// </summary>
        public DeviceType UserDeviceType { get; set; }

        /// <summary>
        ///     Determines if email to user about abandoned cart sent
        /// </summary>
        public bool IsAbandonedEmailSent { get; set; }

        /// <summary>
        ///     A collection of custom properties that contain additional meta data about the order.
        /// </summary>
        /// <returns>CustomPropertyCollection</returns>
        /// <remarks>This is an extension point for developers to use for integration, as well as Hotcakes related meta data.</remarks>
        public CustomPropertyCollection CustomProperties { get; set; }

        /// <summary>
        ///     Gets or sets the culture that was last used when creating the order
        /// </summary>
        public string UsedCulture { get; set; }

        #endregion

        #region Properties / Status

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

        #endregion

        #region Properties / Addresses

        /// <summary>
        ///     Address object for who is being billed for the order.
        /// </summary>
        public Address BillingAddress { get; set; }

        /// <summary>
        ///     Address object for who is receiving the order, when shipped.
        /// </summary>
        public Address ShippingAddress { get; set; }

        #endregion

        #region Properties / Totals

        /// <summary>
        ///     Total amount of tax for the order.
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        ///     Total amount of tax for the line items in the order.
        /// </summary>
        public decimal ItemsTax { get; set; }

        /// <summary>
        ///     Amount of tax for shipping.
        /// </summary>
        public decimal ShippingTax { get; set; }

        /// <summary>
        ///     The tax rate for shipping.
        /// </summary>
        public decimal ShippingTaxRate { get; set; }

        /// <summary>
        ///     Amount from the order custom properties to override the total before discounts.
        /// </summary>
        public decimal TotalShippingBeforeDiscountsOverride
        {
            get
            {
                decimal result = -1;
                var setting = CustomProperties.GetProperty(Constants.HCC_KEY, "shippingoverride");
                if (setting.Trim().Length > 0)
                {
                    decimal.TryParse(setting, out result);
                }
                return result;
            }
            set { CustomProperties.SetProperty(Constants.HCC_KEY, "shippingoverride", value.ToString()); }
        }

        private decimal totalShippingBeforeDiscounts;

        /// <summary>
        ///     Total amount for shipping before discounts are applied.
        /// </summary>
        public decimal TotalShippingBeforeDiscounts
        {
            get
            {
                var totalOverride = TotalShippingBeforeDiscountsOverride;
                if (totalOverride >= 0)
                {
                    return totalOverride;
                }
                return totalShippingBeforeDiscounts;
            }
            set { totalShippingBeforeDiscounts = value; }
        }

        /// <summary>
        ///     Total amount for shipping after discounts are applied.
        /// </summary>
        public decimal TotalShippingAfterDiscounts { get; set; }

        /// <summary>
        ///     Total amount for handling charges.
        /// </summary>
        public decimal TotalHandling { get; set; }

        #endregion

        #region Properties / Others

        /// <summary>
        ///     Contains a listing of the discounts that have been applied to this order.
        /// </summary>
        public List<DiscountDetail> OrderDiscountDetails { get; set; }

        /// <summary>
        ///     A listing of all of the discounts for the shipping only.
        /// </summary>
        public List<DiscountDetail> ShippingDiscountDetails { get; set; }

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

        #region Calculated Properties

        /// <summary>
        /// </summary>
        public bool ApplyVATRules
        {
            get
            {
                var applyVATRules = CustomProperties.GetProperty(Constants.HCC_KEY, "ApplyVATRules");
                if (!string.IsNullOrEmpty(applyVATRules))
                    return bool.Parse(applyVATRules);
                return false;
            }
            set { CustomProperties.SetProperty(Constants.HCC_KEY, "ApplyVATRules", value.ToString()); }
        }

        /// <summary>
        ///     Returns a status of whether or not there are any sale discounts in the order line items.
        /// </summary>
        public bool HasAnyNonSaleDiscounts
        {
            get
            {
                foreach (var li in Items)
                {
                    if (li.HasAnyNonSaleDiscounts || li.MarkedForFreeShipping(ShippingMethodId)) return true;
                }
                if (ShippingDiscountDetails.Count > 0) return true;
                if (OrderDiscountDetails.Count > 0) return true;
                return false;
            }
        }

        /// <summary>
        ///     The order total without discounts applied, but with user supplied price line items included. This does not include
        ///     shipping charges.
        /// </summary>
        public decimal TotalOrderBeforeDiscounts
        {
            get { return GetTotal(true, true, false); }
        }

        /// <summary>
        ///     The sum of the discounts being applied to this order, excluding shipping discounts.
        /// </summary>
        public decimal TotalOrderDiscounts
        {
            get
            {
                var result = 0m;
                if (OrderDiscountDetails.Count > 0)
                {
                    result = OrderDiscountDetails.Sum(y => y.Amount);
                }
                return result;
            }
        }

        /// <summary>
        ///     Total amount of the order without user supplied price line items includes, but with discounts applied. This does
        ///     not include shipping charges.
        /// </summary>
        public decimal TotalOrderWithoutUserPricedProducts
        {
            get { return GetTotal(false, true, true); }
        }

        /// <summary>
        ///     Total amount of the order after discounts have been applied. This does not include shipping charges and discounts.
        /// </summary>
        /// <remarks>This value will never be less than zero.</remarks>
        public decimal TotalOrderAfterDiscounts
        {
            get { return GetTotal(true, true, true); }
        }

        /// <summary>
        ///     Returns a sum of all of the shipping discounts applied to the order.
        /// </summary>
        public decimal TotalShippingDiscounts
        {
            get
            {
                var result = 0m;
                if (ShippingDiscountDetails.Count > 0)
                {
                    result = ShippingDiscountDetails.Sum(y => y.Amount);
                }
                return result;
            }
        }

        /// <summary>
        ///     The order grand total that includes all discounts, shipping, and VAT.
        /// </summary>
        public decimal TotalGrand
        {
            get
            {
                var totalGrand = TotalOrderAfterDiscounts + TotalShippingAfterDiscounts;

                if (!ApplyVATRules)
                    totalGrand += TotalTax;

                return totalGrand;
            }
        }

        /// <summary>
        ///     A unique key that represents a specific shipping rate returnd by a shipping provider.
        /// </summary>
        public string ShippingMethodUniqueKey
        {
            get { return ShippingMethodId + ShippingProviderId + ShippingProviderServiceCode; }
        }

        /// <summary>
        ///     Allows you to determine if there are any shippable line items in the order.
        /// </summary>
        public bool HasShippingItems
        {
            get { return Items.Any(i => !i.IsNonShipping); }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has shipping charges.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has shipping charges; otherwise, <c>false</c>.
        /// </value>
        public bool HasShippingCharges
        {
            get
            {
                return
                    Items.Any(
                        i =>
                            i.ShippingCharge == ShippingChargeType.ChargeShippingAndHandling ||
                            i.ShippingCharge == ShippingChargeType.ChargeShipping);
            }
        }

        /// <summary>
        ///     The sum of the quantity of all line items in the order.
        /// </summary>
        public int TotalQuantity
        {
            get
            {
                var result = 0;
                foreach (var li in Items)
                {
                    result += li.Quantity;
                }
                return result;
            }
        }

        /// <summary>
        ///     The sum total number of products in the order that are shippable.
        /// </summary>
        public decimal TotalQuantityShipping
        {
            get
            {
                var result = 0m;
                foreach (var li in Items)
                {
                    if (!li.IsNonShipping)
                    {
                        result += li.Quantity;
                    }
                }
                return result;
            }
        }

        /// <summary>
        ///     A grand total of the weight of the line items in the order.
        /// </summary>
        public decimal TotalWeight
        {
            get
            {
                var result = 0m;
                foreach (var li in Items)
                {
                    result += li.GetTotalWeight();
                }
                return result;
            }
        }

        /// <summary>
        ///     The subtotal of all non-shipping line items.
        /// </summary>
        /// <returns></returns>
        public decimal SubTotalOfShippingItems()
        {
            decimal result = 0;

            foreach (var li in Items)
            {
                if (!li.IsNonShipping)
                {
                    result += li.LineTotal;
                }
            }

            return result;
        }

        /// <summary>
        ///     Total weight of all line items in the order that can be shipped.
        /// </summary>
        /// <returns>Decimal representation of the total weight</returns>
        public decimal TotalWeightOfShippingItems()
        {
            var result = 0m;
            foreach (var li in Items)
            {
                if (!li.IsNonShipping)
                {
                    result += li.GetTotalWeight();
                }
            }
            return result;
        }

        /// <summary>
        ///     Listing of all line items in the order in IReplaceable format.
        /// </summary>
        /// <returns>List of IReplaceable</returns>
        public List<IReplaceable> ItemsAsReplaceable()
        {
            var result = new List<IReplaceable>();

            foreach (var li in Items)
            {
                result.Add(li);
            }

            return result;
        }

        /// <summary>
        ///     Listing of all packages in the order in IReplaceable format.
        /// </summary>
        /// <returns>List of IReplaceable</returns>
        public List<IReplaceable> PackagesAsReplaceable()
        {
            var result = new List<IReplaceable>();

            foreach (var op in Packages)
            {
                result.Add(op);
            }

            return result;
        }

        /// <summary>
        ///     Returns the order amount total, with or without discounts.
        /// </summary>
        /// <param name="includeUserSuppliedPrice">If true, user supplied price line items will be included in the returned total.</param>
        /// <param name="includeGiftCards">if set to <c>true</c> [include gift cards].</param>
        /// <param name="includeDiscounts">If true, discounts will be included in the returned total.</param>
        /// <returns>
        ///     The total amount of the order, per the given parameters.
        /// </returns>
        public decimal GetTotal(bool includeUserSuppliedPrice, bool includeGiftCards, bool includeDiscounts)
        {
            var items = includeUserSuppliedPrice ? Items : Items.Where(i => !i.IsUserSuppliedPrice);
            items = includeGiftCards ? items : items.Where(i => !i.IsGiftCard);

            var total = items.Sum(i => i.LineTotal);

            if (includeDiscounts)
            {
                decimal minTotal = 0;
                if (includeUserSuppliedPrice)
                {
                    minTotal += Items.Where(i => i.IsUserSuppliedPrice).Sum(i => i.LineTotal);
                }
                if (includeGiftCards)
                {
                    minTotal += Items.Where(i => i.IsGiftCard).Sum(i => i.LineTotal);
                }

                total += TotalOrderDiscounts;

                if (total < minTotal)
                {
                    total = minTotal;
                }
            }

            return total;
        }

        /// <summary>
        ///     The grand total after store credit has been applied, if any.
        /// </summary>
        /// <param name="orderService">An instance of the OrderServer to use for data access.</param>
        /// <returns></returns>
        public decimal TotalGrandAfterStoreCredits(OrderService orderService)
        {
            var transaction = orderService.Transactions.FindForOrder(bvin);
            var potentialCredits = orderService.Transactions.TransactionsPotentialStoreCredits(transaction);
            return TotalGrand - potentialCredits;
        }

        /// <summary>
        ///     Information about the order amount total in a summary format.
        /// </summary>
        /// <returns>
        ///     An HTML table representation of the order summary, include all of the details about how the total was
        ///     generated.
        /// </returns>
        public string TotalsAsTable(string localizeCulture = null)
        {
            var sb = new StringBuilder();

            sb.Append("<table class=\"totaltable\">");

            // Sub Total
            if (OrderDiscountDetails.Count > 0)
            {
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("BeforeDiscounts", localizeCulture) + ":");
                sb.Append("</td>");
                sb.Append("<td class=\"totalsub\">" + TotalOrderBeforeDiscounts.ToString("c") + "</td>");
                sb.Append("</tr>");

                foreach (var d in OrderDiscountDetails)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class=\"totaldiscountdetail\">");
                    sb.Append(d.Description + ":");
                    sb.Append("</td>");
                    sb.Append("<td class=\"totaldiscount\">");
                    sb.Append(d.Amount.ToString("c"));
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
            }

            sb.Append("<tr>");
            sb.Append("<td class=\"totallabel\">");
            sb.Append(GetLocalizeString("SubTotal", localizeCulture) + ":</td>");
            sb.Append("<td class=\"totalsub\">");
            sb.Append(TotalOrderAfterDiscounts.ToString("c"));
            sb.Append("</td>");
            sb.Append("</tr>");

            // Shipping
            if (ShippingDiscountDetails.Count > 0)
            {
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("ShippingHandlingBeforeDiscounts", localizeCulture) + ":");
                sb.Append("</td>");
                sb.Append("<td class=\"totalshipping\">" + TotalShippingBeforeDiscounts.ToString("c") + "</td>");
                sb.Append("</tr>");

                foreach (var d in ShippingDiscountDetails)
                {
                    sb.Append("<tr>");
                    sb.Append("<td class=\"totaldiscountdetail\">");
                    sb.Append(d.Description + ":");
                    sb.Append("</td>");
                    sb.Append("<td class=\"totaldiscount\">");
                    sb.Append(d.Amount.ToString("c"));
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
            }

            sb.Append("<tr>");
            sb.Append("<td class=\"totallabel\">");
            sb.Append(GetLocalizeString("Shipping", localizeCulture) + ":<br /><span class=\"tiny\">" +
                      ShippingMethodDisplayName + "</span></td>");
            sb.Append("<td class=\"totalshipping\">");
            sb.Append(TotalShippingAfterDiscounts.ToString("c"));
            sb.Append("</td>");
            sb.Append("</tr>");

            //Add separator
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\" class=\"totallabel\">&nbsp;</td>");
            sb.Append("</tr>");

            if (!ApplyVATRules)
            {
                // Tax
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("Tax", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append(TotalTax.ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");
            }
            else
            {
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("SubtotalBeforeVAT", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append((TotalOrderBeforeDiscounts - ItemsTax).ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("ShippingBeforeVAT", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append((TotalShippingAfterDiscounts - ShippingTax).ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("TotalBeforeVAT", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append((TotalGrand - TotalTax).ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");

                // Tax
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("VAT", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append(TotalTax.ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append("<tr>");
            sb.Append("<td class=\"totalgrandlabel\">");
            sb.Append(GetLocalizeString("TotalGrand", localizeCulture));
            sb.Append(":</td>");
            sb.Append("<td class=\"totalgrand\">");
            sb.Append("<strong>");
            sb.Append(TotalGrand.ToString("c"));
            sb.Append("</strong></td>");
            sb.Append("</tr>");

            var orderServices = Factory.CreateService<OrderService>();

            var transactions = orderServices.Transactions.FindForOrder(bvin);
            var potentialRewardPoints = orderServices.Transactions.TransactionsPotentialValue(transactions,
                ActionType.RewardPointsInfo);
            if (potentialRewardPoints > 0)
            {
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("RewardPoints", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append(potentialRewardPoints.ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");
            }

            var potentialGiftCards = orderServices.Transactions.TransactionsPotentialValue(transactions,
                ActionType.GiftCardInfo);
            if (potentialGiftCards > 0)
            {
                sb.Append("<tr>");
                sb.Append("<td class=\"totallabel\">");
                sb.Append(GetLocalizeString("GiftCards", localizeCulture) + ":</td>");
                sb.Append("<td class=\"totaltax\">");
                sb.Append(potentialGiftCards.ToString("c"));
                sb.Append("</td>");
                sb.Append("</tr>");
            }

            if (potentialRewardPoints > 0 || potentialGiftCards > 0)
            {
                var totalAfterStoreCredits = TotalGrandAfterStoreCredits(orderServices);
                // Grand Total
                sb.Append("<tr>");
                sb.Append("<td class=\"totalgrandlabel\">");
                sb.Append(GetLocalizeString("TotalDue", localizeCulture));
                sb.Append(":</td>");
                sb.Append("<td class=\"totalgrand\">");
                sb.Append("<strong>");
                sb.Append(totalAfterStoreCredits.ToString("c"));
                sb.Append("</strong></td>");
                sb.Append("</tr>");
            }

            sb.Append("</table>");

            return sb.ToString();
        }

        private string GetLocalizeString(string localizeKey, string localizeCuture = null)
        {
            if (string.IsNullOrEmpty(localizeCuture))
            {
                return GlobalLocalization.GetString(localizeKey);
            }
            return GlobalLocalization.GetString(localizeKey, localizeCuture);
        }

        public bool IsRecurring
        {
            get { return Items.Any(i => i.IsRecurring); }
        }

        #endregion

        #region Public Methods / Discounts

        /// <summary>
        ///     Adds a discount to the order.
        /// </summary>
        /// <param name="amount">Amount of the discount to apply.</param>
        /// <param name="description">Description of what the discount is.</param>
        /// <param name="promotionId">The promotion identifier.</param>
        /// <param name="actionId">The action identifier.</param>
        public void AddOrderDiscount(decimal amount, string description, long promotionId, long actionId)
        {
            if (amount != 0 && OrderDiscountDetails.All(d => d.PromotionId != promotionId))
            {
                OrderDiscountDetails.Add(new DiscountDetail
                {
                    Amount = amount,
                    Description = description,
                    PromotionId = promotionId,
                    ActionId = actionId
                });
            }
        }

        /// <summary>
        ///     Adds a discount to the shipping.
        /// </summary>
        /// <param name="amount">Amount of the discount to apply.</param>
        /// <param name="description">Description of what the discount is.</param>
        /// <param name="promotionId">The promotion identifier.</param>
        /// <param name="actionId">The action identifier.</param>
        public void AddShippingDiscount(decimal amount, string description, long promotionId, long actionId)
        {
            if (OrderDiscountDetails.All(d => d.PromotionId != promotionId))
            {
                ShippingDiscountDetails.Add(new DiscountDetail
                {
                    Amount = amount,
                    Description = description,
                    PromotionId = promotionId,
                    ActionId = actionId,
                    DiscountType = PromotionType.OfferForShipping
                });
            }
        }

        #endregion

        #region Public Methods / Coupon Codes

        /// <summary>
        ///     Adds a coupon code to the order.
        /// </summary>
        /// <param name="code">The coupon code to add.</param>
        /// <returns>Boolean - if true, the coupon was successfully added.</returns>
        public bool AddCouponCode(string code)
        {
            if (!CouponCodeExists(code))
            {
                Coupons.Add(new OrderCoupon
                {
                    CouponCode = code.Trim().ToUpper(),
                    UserId = UserID,
                    StoreId = StoreId,
                    OrderBvin = bvin,
                    IsUsed = false
                });
            }
            return true;
        }

        /// <summary>
        ///     Allows for you to see if the coupon code is already added to the order.
        /// </summary>
        /// <param name="code">The coupon code to look for.</param>
        /// <returns>Boolean - if true, the coupon code has already been added to this order.</returns>
        public bool CouponCodeExists(string code)
        {
            var c = Coupons.Count(y => y.CouponCode.Trim().ToUpper() == code.Trim().ToUpper());
            return c > 0;
        }

        /// <summary>
        ///     Removes the specified coupon code from the order.
        /// </summary>
        /// <param name="id">The unique ID or primary key of the coupon.</param>
        /// <returns>Boolean - if true, the coupon was successfully found and removed.</returns>
        public bool RemoveCouponCode(long id)
        {
            var c = Coupons.SingleOrDefault(y => y.Id == id);
            if (c != null)
            {
                Coupons.Remove(c);
            }
            return true;
        }

        /// <summary>
        ///     Removes the specified coupon from the order, using the code.
        /// </summary>
        /// <param name="code">The code of the coupon to remove.</param>
        /// <returns>Boolean - if true, the coupon was successfully removed from the order.</returns>
        public bool RemoveCouponCodeByCode(string code)
        {
            var result = false;
            var testCode = code.Trim().ToUpper();
            var codes = Coupons.Where(y => y.CouponCode == testCode).ToList();
            var toRemove = new List<long>();
            foreach (var oc in codes)
            {
                toRemove.Add(oc.Id);
            }
            foreach (var id in toRemove)
            {
                RemoveCouponCode(id);
                result = true;
            }
            return result;
        }

        /// <summary>
        ///     Removes all coupon codes from the order.
        /// </summary>
        /// <returns>Boolean - if true, all coupons were successfully removed.</returns>
        public bool RemoveAllCouponCodes()
        {
            Coupons.Clear();
            return true;
        }

        #endregion

        #region Public Methods / Shipping

        /// <summary>
        ///     Parses the line items to return the proposed packages for shipping.
        /// </summary>
        /// <param name="shippingMethodId">The unique ID of the desired shipping method.</param>
        /// <returns>List of ShippingGroup</returns>
        public List<ShippingGroup> GetShippingGroups(string shippingMethodId)
        {
            var result = new List<ShippingGroup>();
            foreach (var item in Items)
            {
                // skip non-shipping items
                if (item.IsNonShipping)
                {
                    continue;
                }

                // skip excluded shipping items (from the product setting)
                if (item.ShippingCharge == ShippingChargeType.None ||
                    item.ShippingCharge == ShippingChargeType.ChargeHandling)
                {
                    continue;
                }

                //skip items marked as "free shipping" by discount engine. Check if quantity is grater than 1. If quantity 1 we have to allow to get rates and then
                // on checkout page show as discount
                if (item.MarkedForFreeShipping(shippingMethodId.ToUpperInvariant()) && Items.Count > 1 &&
                    !IsOrderHasAllItemsQualifiedFreeShipping())
                {
                    continue;
                }

                ShippingGroup packageToAddTo = null;

                if (!item.ShipSeparately)
                {
                    if (item.ShipFromMode == ShippingMode.ShipFromManufacturer)
                    {
                        foreach (var package in result)
                        {
                            if ((package.ShippingMode == ShippingMode.ShipFromManufacturer) && !package.ShipSeperately)
                            {
                                if (package.ShipId == item.ShipFromNotificationId)
                                {
                                    packageToAddTo = package;
                                    break;
                                }
                            }
                        }
                    }
                    else if (item.ShipFromMode == ShippingMode.ShipFromSite)
                    {
                        foreach (var package in result)
                        {
                            if ((package.ShippingMode == ShippingMode.ShipFromSite) && !package.ShipSeperately)
                            {
                                packageToAddTo = package;
                                break;
                            }
                        }
                    }
                    else if (item.ShipFromMode == ShippingMode.ShipFromVendor)
                    {
                        foreach (var package in result)
                        {
                            if ((package.ShippingMode == ShippingMode.ShipFromVendor) && !package.ShipSeperately)
                            {
                                if (package.ShipId == item.ShipFromNotificationId)
                                {
                                    packageToAddTo = package;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Throw New ApplicationException("Unrecognized shipping mode.")
                        // Assume ship from store if no other mode located
                        foreach (var package in result)
                        {
                            if ((package.ShippingMode == ShippingMode.ShipFromSite) && !package.ShipSeperately)
                            {
                                packageToAddTo = package;
                                break;
                            }
                        }
                    }
                }

                if (packageToAddTo == null)
                {
                    packageToAddTo = new ShippingGroup
                    {
                        DestinationAddress = ShippingAddress,
                        ShippingMode = item.ShipFromMode,
                        SourceAddress = item.ShipFromAddress,
                        ShipId = item.ShipFromNotificationId,
                        ShipSeperately = item.ShipSeparately
                    };

                    if (!item.ShipSeparately)
                    {
                        result.Add(packageToAddTo);
                    }
                }

                if (item.ShipSeparately)
                {
                    if (item.Quantity - item.QuantityShipped > 1)
                    {
                        for (var i = 0; i <= item.Quantity - item.QuantityShipped - 1; i++)
                        {
                            var newLineItem = item.Clone(true);
                            newLineItem.Quantity = 1;
                            var newPackage = packageToAddTo.Clone(false);
                            newPackage.Items.Add(newLineItem);
                            result.Add(newPackage);
                        }
                    }
                    else
                    {
                        packageToAddTo.Items.Add(item);
                        result.Add(packageToAddTo);
                    }
                }
                else
                {
                    packageToAddTo.Items.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        ///     Evaluates the order to return whether or not shipping is free.
        /// </summary>
        /// <returns>Boolean - if true, shipping is free for this order.</returns>
        public bool IsOrderFreeShipping()
        {
            foreach (var item in Items)
            {
                if (item.CustomProperties["freeshipping"] == null
                    && !item.IsMarkedForFreeShipping)
                {
                    return false;
                }

                if (item.FreeShippingMethodIds.Count > 0 &&
                    item.IsMarkedForFreeShipping)
                {
                    // May be free shipping but since one or more methods is selected 
                    // we won't remove the other ones.
                    return false;
                }
            }

            return true;
        }

        public bool IsOrderHasAllItemsQualifiedFreeShipping()
        {
            var FreeShipItems = Items.Count(p => p.IsMarkedForFreeShipping);

            return Items.Count == FreeShipItems;
        }

        /// <summary>
        ///     Returns a list of packages in the order that have been shipped already.
        /// </summary>
        /// <returns>List of OrderPackage</returns>
        public List<OrderPackage> FindShippedPackages()
        {
            var result = new List<OrderPackage>();
            foreach (var p in Packages)
            {
                if (p.HasShipped)
                {
                    result.Add(p);
                }
            }
            return result;
        }

        /// <summary>
        /// </summary>
        public void EvaluateCurrentShippingStatus()
        {
            ShippingStatus = EvaluateShippingStatus();
        }

        #endregion

        #region Public Methods / File Downloads

        /// <summary>
        ///     Returns the total number of times a product file has been downloaded using it's BVIN.
        /// </summary>
        /// <param name="fileBvin">The unique ID of the file to return downloads for.</param>
        /// <returns>Integer - the total number of downloads.</returns>
        public int GetFileDownloadCount(string fileBvin)
        {
            var key = FileDownloadPropertyKey(fileBvin);
            return CustomProperties.GetPropertyAsInt(Constants.HCC_KEY, key);
        }

        /// <summary>
        ///     Increases the download count by one when called.
        /// </summary>
        /// <param name="fileBvin">The unique ID of the product file to increase the download count for.</param>
        public void IncreaseFileDownloadCount(string fileBvin)
        {
            var key = FileDownloadPropertyKey(fileBvin);
            var current = CustomProperties.GetPropertyAsInt(Constants.HCC_KEY, key);
            if (current < 0) current = 0;
            current++;
            CustomProperties.SetProperty(Constants.HCC_KEY, key, current);
        }

        /// <summary>
        ///     Decreases the download count by one when called.
        /// </summary>
        /// <param name="fileBvin">The unique ID of the product file to decrease the download count for.</param>
        public void DecreaseFileDownloadCount(string fileBvin)
        {
            var key = FileDownloadPropertyKey(fileBvin);
            var current = CustomProperties.GetPropertyAsInt(Constants.HCC_KEY, key);
            if (current > 0)
            {
                current--;
            }
            CustomProperties.SetProperty(Constants.HCC_KEY, key, current);
        }

        /// <summary>
        ///     If called, the download count will be reset to zero for the specified product file.
        /// </summary>
        /// <param name="fileBvin">The unique ID of the product file.</param>
        public void ResetFileDownloadCount(string fileBvin)
        {
            var key = FileDownloadPropertyKey(fileBvin);
            CustomProperties.SetProperty(Constants.HCC_KEY, key, 0);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a single line item matching the given ID.
        /// </summary>
        /// <param name="Id">The unique ID of the line item to find.</param>
        /// <returns>LineItem</returns>
        public LineItem GetLineItem(long Id)
        {
            return Items.SingleOrDefault(y => y.Id == Id);
        }

        /// <summary>
        ///     A converted copy of the line items list for taxable procedures.
        /// </summary>
        /// <returns>List of ITaxable</returns>
        public List<ITaxable> ItemsAsITaxable()
        {
            var result = new List<ITaxable>();

            foreach (ITaxable t in Items)
            {
                result.Add(t);
            }

            return result;
        }

        /// <summary>
        ///     A collection of the tokens and the replaceable content for email templates.
        /// </summary>
        /// <param name="context">An instance of the Hotcakes Request context.</param>
        /// <returns>List of HtmlTemplateTag</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var store = context.CurrentStore;
            var culture = context.MainContentCulture;
            var result = new List<HtmlTemplateTag>();

            //It is good to have such token for abandoned cart emails but it doesn't work without addtional changes
            //that will enable application to work without HttpContext
            result.Add(new HtmlTemplateTag("[[Order.AdminLink]]",
                DiskStorage.GetHccAdminUrl(context, "Orders/ViewOrder.aspx?id=" + bvin)));
            result.Add(new HtmlTemplateTag("[[Order.AffiliateId]]", AffiliateID.ToString()));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress]]", BillingAddress.ToHtmlString()));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.City]]", BillingAddress.City));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.Company]]", BillingAddress.Company));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.CountryName]]", BillingAddress.CountryDisplayName));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.Fax]]", BillingAddress.Fax));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.FirstName]]", BillingAddress.FirstName));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.LastName]]", BillingAddress.LastName));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.Line1]]", BillingAddress.Line1));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.Line2]]", BillingAddress.Line2));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.Line3]]", BillingAddress.Line3));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.MiddleInitial]]", BillingAddress.MiddleInitial));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.NickName]]", BillingAddress.NickName));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.Phone]]", BillingAddress.Phone));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.PostalCode]]", BillingAddress.PostalCode));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.RegionName]]", BillingAddress.RegionDisplayName));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress.WebSiteUrl]]", BillingAddress.WebSiteUrl));
            result.Add(new HtmlTemplateTag("[[Order.Bvin]]", bvin));
            var coupons = string.Empty;
            for (var i = 0; i <= Coupons.Count - 1; i++)
            {
                coupons += Coupons[i].CouponCode + ", ";
            }

            coupons = coupons.Trim().TrimEnd(',');

            result.Add(new HtmlTemplateTag("[[Order.Coupons]]", coupons));
            result.Add(new HtmlTemplateTag("[[Order.FraudScore]]", FraudScore.ToString("#.#")));
            result.Add(new HtmlTemplateTag("[[Order.GrandTotal]]", TotalGrand.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.Instructions]]", Instructions));
            result.Add(new HtmlTemplateTag("[[Order.LastUpdated]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, LastUpdatedUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[Order.OrderDiscounts]]", TotalOrderDiscounts.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.OrderNumber]]", OrderNumber));

            var orderService = Factory.CreateService<OrderService>(context);
            result.Add(new HtmlTemplateTag("[[Order.PaymentMethod]]", orderService.OrdersListPaymentMethods(this)));
            var PurchaseOrderNumber = string.Empty;
            foreach (var orderdetail in orderService.Transactions.FindForOrder(bvin))
            {
                if (!string.IsNullOrEmpty(orderdetail.PurchaseOrderNumber))
                {
                    PurchaseOrderNumber += orderdetail.PurchaseOrderNumber + ",";
                }
            }
            result.Add(new HtmlTemplateTag("[[Order.Payment.PoNumber]]", PurchaseOrderNumber.Trim().TrimEnd(',')));

            result.Add(new HtmlTemplateTag("[[Order.PaymentStatus]]",
                LocalizationUtils.GetOrderPaymentStatus(PaymentStatus, culture)));

            var notes = string.Empty;
            foreach (var item in Notes)
            {
                if (item.IsPublic)
                {
                    notes += item.Note;
                }
            }
            result.Add(new HtmlTemplateTag("[[Order.PublicNotes]]", notes));

            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress]]", ShippingAddress.ToHtmlString()));
            result.Add(new HtmlTemplateTag("[[Order.BillingAddress]]", ShippingAddress.ToHtmlString()));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.City]]", ShippingAddress.City));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.Company]]", ShippingAddress.Company));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.CountryName]]", ShippingAddress.CountryDisplayName));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.Fax]]", ShippingAddress.Fax));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.FirstName]]", ShippingAddress.FirstName));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.LastName]]", ShippingAddress.LastName));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.Line1]]", ShippingAddress.Line1));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.Line2]]", ShippingAddress.Line2));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.Line3]]", ShippingAddress.Line3));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.MiddleInitial]]", ShippingAddress.MiddleInitial));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.NickName]]", ShippingAddress.NickName));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.Phone]]", ShippingAddress.Phone));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.PostalCode]]", ShippingAddress.PostalCode));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.RegionName]]", ShippingAddress.RegionDisplayName));
            result.Add(new HtmlTemplateTag("[[Order.ShippingAddress.WebSiteUrl]]", ShippingAddress.WebSiteUrl));
            var contactService = Factory.CreateService<ContactService>(context);
            result.Add(new HtmlTemplateTag("[[Order.StoreAddress]]",
                contactService.Addresses.FindStoreContactAddress().ToHtmlString()));
            result.Add(new HtmlTemplateTag("[[Order.ShippingDiscounts]]", TotalShippingDiscounts.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.ShippingMethod]]", ShippingMethodDisplayName));
            result.Add(new HtmlTemplateTag("[[Order.ShippingStatus]]",
                LocalizationUtils.GetOrderShippingStatus(ShippingStatus, culture)));
            result.Add(new HtmlTemplateTag("[[Order.ShippingTotal]]", TotalShippingBeforeDiscounts.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.ShippingTotalMinusDiscounts]]",
                (TotalShippingBeforeDiscounts - TotalShippingDiscounts).ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.Status]]", LocalizationUtils.GetOrderStatus(StatusName, culture)));
            result.Add(new HtmlTemplateTag("[[Order.SubTotal]]", TotalOrderBeforeDiscounts.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.SubTotalMinusDiscounts]]",
                (TotalOrderAfterDiscounts - TotalOrderDiscounts).ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.SubTotalMinusDiscouts]]",
                (TotalOrderAfterDiscounts - TotalOrderDiscounts).ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.ItemsTax]]", ItemsTax.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.ShippingTax]]", ShippingTax.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.ShippingTaxRate]]",
                decimal.Parse(ShippingTaxRate.ToString("p3").Replace("%", "")).ToString("G29") + " %"));
            result.Add(new HtmlTemplateTag("[[Order.TaxTotal]]", TotalTax.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.TimeOfOrder]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, TimeOfOrderUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[Order.TotalQuantity]]", TotalQuantity.ToString()));
            result.Add(new HtmlTemplateTag("[[Order.TotalWeight]]", TotalWeight.ToString()));
            result.Add(new HtmlTemplateTag("[[Order.TotalShippingAfterDiscounts]]",
                TotalShippingAfterDiscounts.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Order.TotalsAsTable]]", TotalsAsTable(culture)));

            var packages = FindShippedPackages();
            if (packages.Count > 0)
            {
                var trackingNumbersOutput = new StringBuilder("<ul class=\"trackingnumberlist\">");
                var trackingNumberLinksOutput = new StringBuilder("<ul class=\"trackingnumberlinklist\">");
                foreach (var item in packages)
                {
                    trackingNumbersOutput.Append("<li>");
                    trackingNumbersOutput.Append(item.TrackingNumber);
                    trackingNumbersOutput.Append("</li>");
                }
                trackingNumbersOutput.Append("</ul>");
                trackingNumberLinksOutput.Append("</ul>");
                result.Add(new HtmlTemplateTag("[[Order.TrackingNumbers]]", trackingNumbersOutput.ToString()));
                result.Add(new HtmlTemplateTag("[[Order.TrackingNumberLinks]]", trackingNumberLinksOutput.ToString()));
            }
            else
            {
                result.Add(new HtmlTemplateTag("[[Order.TrackingNumbers]]",
                    "<ul class=\"trackingnumberlist\"><li>None Available yet</li></ul>"));
                result.Add(new HtmlTemplateTag("[[Order.TrackingNumberLinks]]",
                    "<ul class=\"trackingnumberlinklist\"><li>None Available yet</li></ul>"));
            }

            result.Add(new HtmlTemplateTag("[[Order.UserID]]", UserID));

            var membershipServices = Factory.CreateService<MembershipServices>(context);
            var user = membershipServices.Customers.Find(UserID);
            if (user != null)
            {
                result.Add(new HtmlTemplateTag("[[Order.UserName]]", user.Username));
                result.Add(new HtmlTemplateTag("[[Order.UserEmail]]", user.Email));

                var taxExemptionNumber = "-";
                if (!string.IsNullOrEmpty(user.TaxExemptionNumber))
                    taxExemptionNumber = user.TaxExemptionNumber;

                result.Add(new HtmlTemplateTag("[[Order.VATRegistrationNumber]]", taxExemptionNumber));
            }
            else
            {
                result.Add(new HtmlTemplateTag("[[Order.UserName]]", UserEmail));
                result.Add(new HtmlTemplateTag("[[Order.UserEmail]]", UserEmail));
                result.Add(new HtmlTemplateTag("[[Order.VATRegistrationNumber]]", string.Empty));
            }

            return result;
        }

        /// <summary>
        ///     Generates the order header that tells the status of payment and shipping for merchants.
        /// </summary>
        /// <returns>String (e.g., "Paid / Partially Shipped")</returns>
        public string FullOrderStatusDescription()
        {
            var result = string.Format("{0} / {1} / {2}", LocalizationUtils.GetOrderStatus(StatusName), LocalizationUtils.GetOrderPaymentStatus(PaymentStatus), LocalizationUtils.GetOrderShippingStatus(ShippingStatus));

            return result;
        }

        /// <summary>
        ///     Promotes the order to the next status or state of the order. For example, from Received to Ready for Shipment.
        /// </summary>
        public void MoveToNextStatus()
        {
            var codes = OrderStatusCode.FindAll();

            for (var i = 0; i <= codes.Count - 1; i++)
            {
                if (codes[i].Bvin == StatusCode)
                {
                    // Found Current
                    if (i < codes.Count - 1)
                    {
                        StatusCode = codes[i + 1].Bvin;
                        StatusName = codes[i + 1].StatusName;
                    }
                    break;
                }
            }
        }

        /// <summary>
        ///     Demotes the order to the next status or state of the order. For example, from Ready for Shipment to Received.
        /// </summary>
        public void MoveToPreviousStatus()
        {
            var codes = OrderStatusCode.FindAll();

            for (var i = 0; i <= codes.Count - 1; i++)
            {
                if (codes[i].Bvin == StatusCode)
                {
                    // Found Current
                    if (i > 0)
                    {
                        StatusCode = codes[i - 1].Bvin;
                        StatusName = codes[i - 1].StatusName;
                    }
                    break;
                }
            }
        }

        /// <summary>
        ///     Returns a listing of the line items and totals in the current order.
        /// </summary>
        /// <returns>iDictionary of Line Item ID and a running total.</returns>
        public IDictionary<long, decimal> GetLineItemValuesAccountingForOrderDiscounts()
        {
            var result = new Dictionary<long, decimal>();

            if (TotalOrderDiscounts > 0)
            {
                decimal lineItemTotals = 0;
                foreach (var item in Items)
                {
                    lineItemTotals += item.LineTotal;
                }

                foreach (var item in Items)
                {
                    result.Add(item.Id, item.LineTotal/lineItemTotals);
                }

                foreach (var item in Items)
                {
                    result[item.Id] = Math.Round(item.LineTotal - TotalOrderDiscounts*result[item.Id], 2);
                }

                decimal discountedTotals = 0;
                foreach (var key in result.Keys)
                {
                    discountedTotals += result[key];
                }

                var difference = lineItemTotals - TotalOrderDiscounts - discountedTotals;
                if (difference != 0)
                {
                    foreach (var item in Items)
                    {
                        if (result[item.Id] >= difference)
                        {
                            result[item.Id] += difference;
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in Items)
                {
                    result.Add(item.Id, item.LineTotal);
                }
            }

            return result;
        }

        /// <summary>
        ///     Resets the taxes for all line items and the order to zero for recalculation.
        /// </summary>
        public void ClearTaxes()
        {
            foreach (var item in ItemsAsITaxable())
            {
                item.ClearTaxValue();
            }
            ItemsTax = 0;
            ShippingTax = 0;
            TotalTax = 0;
        }

        /// <summary>
        ///     Removes all calculated shipping information for re-calculation.
        /// </summary>
        public void ClearShippingPricesAndMethod()
        {
            ShippingMethodId = string.Empty;
            ShippingMethodDisplayName = string.Empty;
            ShippingProviderId = string.Empty;
            ShippingProviderServiceCode = string.Empty;
            TotalShippingBeforeDiscounts = 0;
        }

        /// <summary>
        ///     Removes all discount information from the order for recalculation.
        /// </summary>
        public void ClearDiscounts()
        {
            for (var i = 0; i <= Items.Count - 1; i++)
            {
                Items[i].DiscountDetails.Clear();
                Items[i].IsMarkedForFreeShipping = false;
                Items[i].FreeShippingMethodIds.Clear();
            }
            OrderDiscountDetails.Clear();
            ShippingDiscountDetails.Clear();
        }

        #endregion

        #region Implementation

        /// <summary>
        ///     Evaluates all line items in the order to determine the current shippment status.
        /// </summary>
        /// <returns>OrderShippingStatus</returns>
        private OrderShippingStatus EvaluateShippingStatus()
        {
            var result = OrderShippingStatus.Unknown;

            if (Items.Count > 0)
            {
                var shippedFound = false;
                var unShippedFound = false;
                var nonShippingFound = false;

                for (var i = 0; i <= Items.Count - 1; i++)
                {
                    Items[i].ShippingStatus = Items[i].EvaluateShippingStatus(TimeOfOrderUtc);
                    switch (Items[i].ShippingStatus)
                    {
                        case OrderShippingStatus.NonShipping:
                            nonShippingFound = true;
                            break;
                        case OrderShippingStatus.FullyShipped:
                            shippedFound = true;
                            break;
                        case OrderShippingStatus.PartiallyShipped:
                            shippedFound = true;
                            unShippedFound = true;
                            break;
                        case OrderShippingStatus.Unknown:
                        case OrderShippingStatus.Unshipped:
                            unShippedFound = true;
                            break;
                    }
                }

                if (nonShippingFound && (unShippedFound == false) && (shippedFound == false))
                {
                    // Only non shipping items
                    result = OrderShippingStatus.FullyShipped;
                }
                else
                {
                    if (shippedFound && unShippedFound)
                    {
                        // Some items shipping and others not
                        result = OrderShippingStatus.PartiallyShipped;
                    }
                    else
                    {
                        if (shippedFound)
                        {
                            // only shipped found
                            result = OrderShippingStatus.FullyShipped;
                        }
                        else
                        {
                            // only unshipped found
                            result = OrderShippingStatus.Unshipped;
                        }
                    }
                }
            }

            ShippingStatus = result;
            return result;
        }

        private string FileDownloadPropertyKey(string fileBvin)
        {
            return "file" + fileBvin;
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current order object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderDTO</returns>
        public OrderDTO ToDto()
        {
            var dto = new OrderDTO();

            dto.AffiliateID = AffiliateID;
            dto.BillingAddress = BillingAddress.ToDto();
            dto.Bvin = bvin ?? string.Empty;
            dto.Coupons = new List<OrderCouponDTO>();
            foreach (var c in Coupons)
            {
                dto.Coupons.Add(c.ToDto());
            }
            dto.CustomProperties = new List<CustomPropertyDTO>();
            foreach (var prop in CustomProperties)
            {
                dto.CustomProperties.Add(prop.ToDto());
            }
            dto.FraudScore = FraudScore;
            dto.Id = Id;
            dto.Instructions = Instructions ?? string.Empty;
            dto.IsPlaced = IsPlaced;
            dto.Items = new List<LineItemDTO>();
            foreach (var li in Items)
            {
                dto.Items.Add(li.ToDto());
            }
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.Notes = new List<OrderNoteDTO>();
            foreach (var n in Notes)
            {
                dto.Notes.Add(n.ToDto());
            }
            dto.OrderDiscountDetails = new List<DiscountDetailDTO>();
            foreach (var d in OrderDiscountDetails)
            {
                dto.OrderDiscountDetails.Add(d.ToDto());
            }
            dto.OrderNumber = OrderNumber ?? string.Empty;
            dto.Packages = new List<OrderPackageDTO>();
            foreach (var pak in Packages)
            {
                dto.Packages.Add(pak.ToDto());
            }
            dto.PaymentStatus = (OrderPaymentStatusDTO) (int) PaymentStatus;
            dto.ShippingAddress = ShippingAddress.ToDto();
            dto.ShippingDiscountDetails = new List<DiscountDetailDTO>();
            foreach (var sd in ShippingDiscountDetails)
            {
                dto.ShippingDiscountDetails.Add(sd.ToDto());
            }
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

            dto.ShippingTaxRate = ShippingTaxRate;
            dto.ShippingTax = ShippingTax;
            dto.ItemsTax = ItemsTax;
            dto.TotalTax = TotalTax;

            dto.TotalOrderDiscounts = TotalOrderDiscounts;
            dto.TotalShippingDiscounts = TotalShippingDiscounts;
            dto.TotalHandling = TotalHandling;

            dto.TotalShippingBeforeDiscounts = TotalShippingBeforeDiscounts;
            dto.TotalOrderBeforeDiscounts = TotalOrderBeforeDiscounts;
            dto.TotalGrand = TotalGrand;

            dto.UserEmail = UserEmail ?? string.Empty;
            dto.UserID = UserID ?? string.Empty;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current order object using an OrderDTO instance
        /// </summary>
        /// <param name="dto">An instance of the order from the REST API</param>
        public void FromDTO(OrderDTO dto)
        {
            if (dto == null) return;

            AffiliateID = dto.AffiliateID;
            BillingAddress.FromDto(dto.BillingAddress);
            bvin = dto.Bvin ?? string.Empty;
            Coupons.Clear();
            if (dto.Coupons != null)
            {
                foreach (var c in dto.Coupons)
                {
                    var cp = new OrderCoupon();
                    cp.FromDto(c);
                    Coupons.Add(cp);
                }
            }
            CustomProperties.Clear();
            if (dto.CustomProperties != null)
            {
                foreach (var prop in dto.CustomProperties)
                {
                    var p = new CustomProperty();
                    p.FromDto(prop);
                    CustomProperties.Add(p);
                }
            }
            FraudScore = dto.FraudScore;
            Id = dto.Id;
            Instructions = dto.Instructions ?? string.Empty;
            IsPlaced = dto.IsPlaced;
            Items.Clear();
            if (dto.Items != null)
            {
                foreach (var li in dto.Items)
                {
                    var l = new LineItem();
                    l.FromDto(li);
                    Items.Add(l);
                }
            }
            LastUpdatedUtc = dto.LastUpdatedUtc;
            Notes.Clear();
            if (dto.Notes != null)
            {
                foreach (var n in dto.Notes)
                {
                    var nn = new OrderNote();
                    nn.FromDto(n);
                    Notes.Add(nn);
                }
            }
            OrderDiscountDetails.Clear();
            if (dto.OrderDiscountDetails != null)
            {
                foreach (var d in dto.OrderDiscountDetails)
                {
                    var m = new DiscountDetail();
                    m.FromDto(d);
                    OrderDiscountDetails.Add(m);
                }
            }
            OrderNumber = dto.OrderNumber ?? string.Empty;
            Packages.Clear();
            if (dto.Packages != null)
            {
                foreach (var pak in dto.Packages)
                {
                    var pak2 = new OrderPackage();
                    pak2.FromDto(pak);
                    Packages.Add(pak2);
                }
            }
            PaymentStatus = (OrderPaymentStatus) (int) dto.PaymentStatus;
            ShippingAddress.FromDto(dto.ShippingAddress);
            ShippingDiscountDetails.Clear();
            if (dto.ShippingDiscountDetails != null)
            {
                foreach (var sd in dto.ShippingDiscountDetails)
                {
                    var sdd = new DiscountDetail();
                    sdd.FromDto(sd);
                    ShippingDiscountDetails.Add(sdd);
                }
            }
            ShippingMethodDisplayName = dto.ShippingMethodDisplayName ?? string.Empty;
            ShippingMethodId = dto.ShippingMethodId ?? string.Empty;
            ShippingProviderId = dto.ShippingProviderId ?? string.Empty;
            ShippingProviderServiceCode = dto.ShippingProviderServiceCode ?? string.Empty;
            ShippingStatus = (OrderShippingStatus) (int) dto.ShippingStatus;
            StatusCode = dto.StatusCode ?? string.Empty;
            StatusName = dto.StatusName ?? string.Empty;
            StoreId = dto.StoreId;
            ThirdPartyOrderId = dto.ThirdPartyOrderId ?? string.Empty;
            TimeOfOrderUtc = dto.TimeOfOrderUtc;

            TotalHandling = dto.TotalHandling;
            TotalShippingBeforeDiscounts = dto.TotalShippingBeforeDiscounts;
            TotalTax = dto.TotalTax;

            ShippingTax = dto.ShippingTax;
            ShippingTaxRate = dto.ShippingTaxRate;
            ItemsTax = dto.ItemsTax;

            UserEmail = dto.UserEmail ?? string.Empty;
            UserID = dto.UserID ?? string.Empty;
        }

        #endregion
    }
}