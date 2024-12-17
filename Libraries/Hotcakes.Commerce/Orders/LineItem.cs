#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.CommerceDTO.v1.Shipping;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of LineItem
    /// </summary>
    /// <remarks>The REST API equivalent is LineItemDTO.</remarks>
    [Serializable]
    public class LineItem : IEquatable<LineItem>, ITaxable, IReplaceable
    {
        private const string HCC_KEY = "hcc";

        #region Constructor

        public LineItem()
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            BasePricePerItem = 0m;
            DiscountDetails = new List<DiscountDetail>();
            OrderBvin = string.Empty;
            ProductId = string.Empty;
            VariantId = string.Empty;
            ProductName = string.Empty;
            ProductSku = string.Empty;
            ProductShortDescription = string.Empty;
            Quantity = 1;
            QuantityReserved = 0;
            QuantityReturned = 0;
            QuantityShipped = 0;
            ShippingPortion = 0m;
            StatusCode = string.Empty;
            StatusName = string.Empty;
            TaxRate = 0;
            TaxPortion = 0m;
            SelectionData = new OptionSelections();
            IsNonShipping = false;
            TaxSchedule = 0;
            ProductShippingHeight = 0m;
            ProductShippingLength = 0m;
            ProductShippingWeight = 0m;
            ProductShippingWidth = 0m;
            CustomProperties = new CustomPropertyCollection();
            ShipFromAddress = new Address();
            ShipFromMode = ShippingMode.ShipFromSite;
            ShippingCharge = ShippingChargeType.ChargeShippingAndHandling;
            ShipFromNotificationId = string.Empty;
            ShipSeparately = false;
            ExtraShipCharge = 0;
            IsTaxExempt = false;
            FreeShippingMethodIds = new List<string>();
            RecurringBilling = new RecurringBilling(this);
            IsCoverCreditCardFees = false;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The unique ID of the current line item.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the line item was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the order that this line item belongs to.
        /// </summary>
        public string OrderBvin { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the product matching this line item.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        ///     If the product is a variant, this will contain the unique ID or bvin of the matching variant.
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        ///     The language-friendly name of the product matching this line item.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        ///     The unique SKU of the product matching this line item.
        /// </summary>
        public string ProductSku { get; set; }

        /// <summary>
        ///     The description of the product matching this line item.
        /// </summary>
        public string ProductShortDescription { get; set; }

        /// <summary>
        ///     The number of products in this line item.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     The number of products to be held from inventory for this line item.
        /// </summary>
        public int QuantityReserved { get; set; }

        /// <summary>
        ///     Used to define how many products in this line item are being returned.
        /// </summary>
        public int QuantityReturned { get; set; }

        /// <summary>
        ///     Defines how many products in this line item have been shipped.
        /// </summary>
        public int QuantityShipped { get; set; }

        /// <summary>
        ///     The base price of the product matching this line item.
        /// </summary>
        public decimal BasePricePerItem { get; set; }

        /// <summary>
        ///     The cumulative total of the line item, which usually accounts for quanity and promotions.
        /// </summary>
        public decimal AdjustedPricePerItem { get; set; }

        /// <summary>
        ///     The cumulative total of the line item, which usually accounts for promotions.
        /// </summary>
        public decimal LineTotal { get; set; }

        /// <summary>
        ///     A built-in way to save the status code of a line item when integrating with other systems.
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        ///     A built-in way to save the status name of a line item when integrating with other systems.
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        ///     A collection of all promotions being applied to this line item.
        /// </summary>
        public List<DiscountDetail> DiscountDetails { get; set; }

        /// <summary>
        ///     A upcharge being applied to this line item.
        /// </summary>
        public decimal UpchargeDetails { get; set; }

        /// <summary>
        ///     The choices/options that were selected when adding the product to this line item.
        /// </summary>
        public OptionSelections SelectionData { get; set; }

        /// <summary>
        ///     A collection of the custom properties for this line item - great to be used for integration.
        /// </summary>
        public CustomPropertyCollection CustomProperties { get; set; }

        /// <summary>
        ///     Gets or sets the subscription data.
        /// </summary>
        /// <value>
        ///     The subscription data.
        /// </value>
        public RecurringBilling RecurringBilling { get; set; }

        /// <summary>
        ///     Gets or sets if item is free.
        /// </summary>
        public bool IsFreeItem
        {
            get { return !string.IsNullOrEmpty(PromotionIds); }
        }

        /// <summary>
        ///     Gets or sets how many items are free in the line item.
        /// </summary>
        public int FreeQuantity { get; set; }


        public bool IsQuantityFree { get; set; }

        /// <summary>
        /// </summary>
        public string PromotionIds { get; set; }

        public bool IsCoverCreditCardFees { get; set; }

        #endregion

        #region Properties / Shipping

        /// <summary>
        ///     The weight of a single instance of the product in this line item.
        /// </summary>
        public decimal ProductShippingWeight { get; set; }

        /// <summary>
        ///     The length of a single instance of the product in this line item.
        /// </summary>
        public decimal ProductShippingLength { get; set; }

        /// <summary>
        ///     The width of a single instance of the product in this line item.
        /// </summary>
        public decimal ProductShippingWidth { get; set; }

        /// <summary>
        ///     The height of a single instance of the product in this line item.
        /// </summary>
        public decimal ProductShippingHeight { get; set; }

        /// <summary>
        ///     Reflects whether the matching product for this line item is shippable or not.
        /// </summary>
        public bool IsNonShipping { get; set; }

        /// <summary>
        ///     Defines where the product should be shipped from to help support multiple warehouses and/or drop-shipping.
        /// </summary>
        public ShippingMode ShipFromMode { get; set; }

        /// <summary>
        ///     The unique ID of the shipping source from the matching product.
        /// </summary>
        public string ShipFromNotificationId { get; set; }

        /// <summary>
        ///     The address of the source where the product will be shipped from.
        /// </summary>
        public Address ShipFromAddress { get; set; }

        /// <summary>
        ///     Reflects whether the matching product for this line item must be shipped separately or not.
        /// </summary>
        public bool ShipSeparately { get; set; }

        /// <summary>
        ///     A fixed amount added to the line item from the product in addition to the shipping fee required from the shipping
        ///     provider.
        /// </summary>
        public decimal ExtraShipCharge { get; set; }

        /// <summary>
        ///     When qualified in a promotion, the line item can be marked for free shipping here to not charge the customer for
        ///     shipping.
        /// </summary>
        public bool IsMarkedForFreeShipping { get; set; }

        /// <summary>
        ///     A listing of the shipping methods that are qualified for free shipping.
        /// </summary>
        public List<string> FreeShippingMethodIds { get; set; }

        /// <summary>
        ///     The amount of shipping that was spread into this line from the entire order.
        /// </summary>
        public decimal ShippingPortion { get; set; }

        /// <summary>
        ///     Gets or sets the shipping charge.
        /// </summary>
        /// <value>
        ///     The shipping charge.
        /// </value>
        public ShippingChargeType ShippingCharge { get; set; }

        #endregion

        #region Properties / Tax

        /// <summary>
        ///     Reflects the tax exempt setting from the matching product for the line item.
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        ///     The unique ID of the tax schedule used for this line item.
        /// </summary>
        public long TaxSchedule { get; set; }

        /// <summary>
        ///     The rate that the products this line item should be taxed at.
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        ///     The rate that the shipping method should be taxed at.
        /// </summary>
        public decimal ShippingTaxRate { get; set; }

        /// <summary>
        ///     The amount of tax that was spread into this line from the entire order.
        /// </summary>
        public decimal TaxPortion { get; set; }

        /// <summary>
        ///     The amount of shipping tax that was spread into this line from the entire order.
        /// </summary>
        public decimal ShippingTaxPortion { get; set; }

        #endregion

        #region Properties / Mode

        /// <summary>
        ///     Retuns true if the product matching this line item is a user supplied price product.
        /// </summary>
        public bool IsUserSuppliedPrice { get; set; }

        /// <summary>
        ///     Retuns true if the product matching this line item is a bundle.
        /// </summary>
        public bool IsBundle { get; set; }

        /// <summary>
        ///     Retuns true if the product matching this line item is a gift card.
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        ///     If true, this line item represents a product that requires recurring payment.
        /// </summary>
        public bool IsRecurring { get; set; }

        #endregion

        #region Properties / Gift Cards

        /// <summary>
        ///     Gets or sets the gift card recipient email as a custom property.
        /// </summary>
        public string CustomPropGiftCardEmail
        {
            get { return CustomPropertyGet(HCC_KEY, "giftcardemail"); }
            set { CustomPropertySet(HCC_KEY, "giftcardemail", value); }
        }

        /// <summary>
        ///     Gets or sets the gift card recipient name as a custom property.
        /// </summary>
        public string CustomPropGiftCardName
        {
            get { return CustomPropertyGet(HCC_KEY, "giftcardname"); }
            set { CustomPropertySet(HCC_KEY, "giftcardname", value); }
        }

        /// <summary>
        ///     Gets or sets the gift card number as a custom property.
        /// </summary>
        public string CustomPropGiftCardNumber
        {
            get { return CustomPropertyGet(HCC_KEY, "giftcardnumber"); }
            set { CustomPropertySet(HCC_KEY, "giftcardnumber", value); }
        }

        /// <summary>
        ///     Gets or sets the gift card message as a custom property.
        /// </summary>
        public string CustomPropGiftCardMessage
        {
            get { return CustomPropertyGet(HCC_KEY, "giftcardmessage"); }
            set { CustomPropertySet(HCC_KEY, "giftcardmessage", value); }
        }

        #endregion

        #region Calculated Properties

        /// <summary>
        ///     Returns true if there are any discounts that have been applied to this line item.
        /// </summary>
        public bool HasAnyDiscounts
        {
            get
            {
                if (DiscountDetails == null)
                    return false;
                return DiscountDetails.Count(d => d.Amount != 0) > 0;
            }
        }


        /// <summary>
        ///     Returns true if there are any upcharge that have been applied to this line item.
        /// </summary>
        public bool HasAnyUpcharge {
            get
            {
                if (TotalUpcharge() < 0)
                    return false;
                return true;
            }
        }

        /// <summary>
        ///     Returns true if the line item contains any discounts that are not sales (e.g., Offers).
        /// </summary>
        public bool HasAnyNonSaleDiscounts
        {
            get
            {
                if (DiscountDetails == null || DiscountDetails.Count == 0) return false;

                // if this routine is changed, all marketing promotions & promo combinations should be retested
                // why is there a comparison for (Amount != 0) ? - Will

                var count = 0;

                count =
                    DiscountDetails.Count(
                        d =>
                            d.DiscountType != PromotionType.Sale && d.Amount != 0 &&
                            d.DiscountType != PromotionType.VolumeDiscount);

                return count > 0;
            }
        }

        /// <summary>
        ///     Line item total that includes the sale price but does not include any discounts.
        /// </summary>
        public decimal LineTotalWithSalesWithoutDiscounts
        {
            get
            {
                if (Quantity == 0)
                    return 0;

                var b = LineTotalWithoutDiscounts;

                var saleDiscounts = DiscountDetails.Where(y => y.DiscountType == PromotionType.Sale);
                var saleDiscountAmounts = saleDiscounts.Sum(y => y.Amount);
                return b + saleDiscountAmounts;
            }
        }

        /// <summary>
        ///     The cumulative total of the line item base price for the given quantity, excluding discounts and sales.
        /// </summary>
        public decimal LineTotalWithoutDiscounts
        {
            get { return BasePricePerItem*Quantity; }
        }

        /// <summary>
        ///     The cumulative total of the line item base price for the given quantity, including discounts and sales.
        /// </summary>
        public decimal LineTotalWithDiscounts
        {
            get { return (BasePricePerItem * Quantity) + TotalDiscounts(); }
        }

        /// <summary>
        ///     The cumulative total of the line item base price for the given quantity, including upcharges and sales.
        /// </summary>
        public decimal LineTotalWithoutUpcharge
        {
            get
            {
                decimal baseTotal = BasePricePerItem * Quantity;
                if (HasAnyDiscounts)
                {
                    return baseTotal + TotalDiscounts();
                }
                return baseTotal;
            }
        }


        /// <summary>
        ///     Total of Upcharge
        /// </summary>
        /// <returns>Decimal</returns>
        public decimal TotalUpcharge()
        {
            decimal upchargeAmountTotal = 0;
            var productRepo = new ProductRepository(HccRequestContext.Current);
            var product = productRepo.FindBySku(this.ProductSku);
            if (product == null || product.UpchargeAmount <= 0)
            {
                return 0;
            }
            if (true)
            {

            }
            decimal baseTotal = BasePricePerItem * Quantity;
            if (product.UpchargeUnit == ((int)UpchargeAmountTypesDTO.Amount).ToString())
            {
                upchargeAmountTotal = product.UpchargeAmount;
            }
            if (product.UpchargeUnit == ((int)UpchargeAmountTypesDTO.Percent).ToString())
            {
                upchargeAmountTotal = baseTotal * (product.UpchargeAmount / 100);
            }
            return upchargeAmountTotal;
        }

        public decimal LineTotalWithTaxPortion(bool isVAT)
        {
            return LineTotal + (isVAT ? 0 : TaxPortion);
        }

        /// <summary>
        ///     Total of all discounts
        /// </summary>
        /// <returns>Decimal</returns>
        public decimal TotalDiscounts()
        {
            if (DiscountDetails == null || DiscountDetails.Count < 1)
                return 0;

            return IsFreeItem
                ? DiscountDetails.Where(s => s.DiscountType == PromotionType.OfferForFreeItems).Sum(y => y.Amount)
                : DiscountDetails.Sum(y => y.Amount);
        }

        /// <summary>
        ///     Returns the current shipping status for the current line item.
        /// </summary>
        public OrderShippingStatus ShippingStatus { get; set; }

        /// <summary>
        ///     Returns the current shipping status for the current line item.
        /// </summary>
        public OrderShippingStatus EvaluateShippingStatus(DateTime timeofOrder)
        {
            if (IsNonShipping)
                return OrderShippingStatus.NonShipping;

            if (IsRecurring)
            {
                if (RecurringBilling.IsCancelled)
                    return OrderShippingStatus.NonShipping;

                var packagesSent = QuantityShipped/Quantity;

                var coverredDate = RecurringCoverage(timeofOrder, packagesSent);
                var now = DateTime.UtcNow;

                if (coverredDate >= now)
                {
                    return OrderShippingStatus.FullyShipped;
                }
                coverredDate = RecurringCoverage(timeofOrder, packagesSent + 1);
                if (coverredDate >= now)
                {
                    if (QuantityShipped%Quantity != 0)
                        return OrderShippingStatus.PartiallyShipped;
                    return OrderShippingStatus.Unshipped;
                }
                return OrderShippingStatus.Unshipped;
            }
            if (QuantityShipped >= Quantity)
            {
                return OrderShippingStatus.FullyShipped;
            }
            if (QuantityShipped > 0)
            {
                return OrderShippingStatus.PartiallyShipped;
            }
            return OrderShippingStatus.Unshipped;
        }

        public DateTime RecurringCoverage(DateTime startDate, int recurCount)
        {
            DateTime coverredDate;
            switch (RecurringBilling.IntervalType)
            {
                case RecurringIntervalType.Days:
                    coverredDate = startDate.AddDays(RecurringBilling.Interval*recurCount);
                    break;
                case RecurringIntervalType.Months:
                    coverredDate = startDate.AddMonths(RecurringBilling.Interval*recurCount);
                    break;
                default:
                    throw new Exception("RecurringIntervalType is not supported");
            }
            return coverredDate;
        }

        public int GetRecurTimes(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("startDate have to less than endDate");
            for (var i = 1;; i++)
            {
                var coverredBy = RecurringCoverage(startDate, i);
                if (coverredBy > endDate)
                    return i;
            }
        }

        /// <summary>
        ///     Generates a line by line summary of the discounts that are in this line item.
        /// </summary>
        /// <returns>String - an HTML version of the line items for display to the merchant.</returns>
        public string DiscountDetailsAsHtml()
        {
            if (DiscountDetails == null) return string.Empty;
            if (DiscountDetails.Count < 1) return string.Empty;
            var sb = new StringBuilder();
            foreach (var d in DiscountDetails)
            {
                if (d.Amount != 0)
                {
                    sb.Append(d.Description + " " + d.Amount.ToString("c") + "<br />");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Generates a line by line summary of the upcharge that are in this line item.
        /// </summary>
        /// <returns>String - an HTML version of the line items for display to the merchant.</returns>
        public string UpchargeDetailsAsHtml()
        {
            var sb = new StringBuilder();
            sb.Append($"{GlobalLocalization.GetString("UpchargeAmount")} : ({TotalUpcharge():C})<br />");
            return sb.ToString();
        }

        #endregion

        #region Public Methods / Custom Property Helpers

        /// <summary>
        ///     Lets you know if the given custom property exists or not.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="propertyKey">The unique ID of the custom property used to look up the value.</param>
        /// <returns>Boolean - if true, the requested custom property exists</returns>
        public bool CustomPropertyExists(string devId, string propertyKey)
        {
            var result = false;
            for (var i = 0; i <= CustomProperties.Count - 1; i++)
            {
                if (CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (CustomProperties[i].Key.Trim().ToLower() == propertyKey.Trim().ToLower())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Adds or updates a custom property using the given values.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <param name="value">The value you wish to save for use later.</param>
        public void CustomPropertySet(string devId, string key, string value)
        {
            var found = false;

            for (var i = 0; i <= CustomProperties.Count - 1; i++)
            {
                if (CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (CustomProperties[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        CustomProperties[i].Value = value;
                        found = true;
                        break;
                    }
                }
            }

            if (found == false)
            {
                CustomProperties.Add(new CustomProperty(devId, key, value));
            }
        }

        /// <summary>
        ///     Adds or updates a custom property using the given values.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <param name="value">The value you wish to save for use later.</param>
        public void CustomPropertySet(string devId, string key, bool value)
        {
            CustomPropertySet(devId, key, value.ToString());
        }

        /// <summary>
        ///     Queries for and returns the requested custom property value.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <returns>String - if found, a string version of the custom property value will be returned.</returns>
        public string CustomPropertyGet(string devId, string key)
        {
            var result = string.Empty;

            for (var i = 0; i <= CustomProperties.Count - 1; i++)
            {
                if (CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (CustomProperties[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        result = CustomProperties[i].Value;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Queries for and returns the requested custom property value.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <returns>Long - if found, a long version of the custom property value will be returned.</returns>
        public long CustomPropertyGetAsLong(string devId, string key)
        {
            var v = CustomPropertyGet(devId, key);
            long result = 0;
            long.TryParse(v, out result);
            if (result < 0) result = 0;
            return result;
        }

        /// <summary>
        ///     Queries for and returns the requested custom property value.
        /// </summary>
        /// <param name="devId">
        ///     The developer ID that created and is managing the custom property. DO NOT use HCC for custom
        ///     integrations.
        /// </param>
        /// <param name="key">The unique ID of the custom property used to look up the value.</param>
        /// <returns>Boolean - if found, a boolean version of the custom property value will be returned.</returns>
        public bool CustomPropertyGetAsBool(string devId, string key)
        {
            var v = CustomPropertyGet(devId, key);
            var result = false;
            bool.TryParse(v, out result);
            return result;
        }

        /// <summary>
        ///     Permanently deletes the custom property from the store.
        /// </summary>
        /// <param name="devId">The developer ID that created and is managing the custom property.</param>
        /// <param name="key">The unique ID of the custom property used to delete the custom property from the store.</param>
        /// <returns>Boolean - if true, the custom property was found and successfully deleted.</returns>
        public bool CustomPropertyRemove(string devId, string key)
        {
            var result = false;

            for (var i = 0; i <= CustomProperties.Count - 1; i++)
            {
                if (CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (CustomProperties[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        CustomProperties.Remove(CustomProperties[i]);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Parses and returns the current custom property collection object as an XML serialized string.
        /// </summary>
        /// <returns>String - an XML representation of the current custom property collection object</returns>
        public string CustomPropertiesToXml()
        {
            var result = string.Empty;

            try
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(CustomProperties.GetType());
                xs.Serialize(sw, CustomProperties);
                result = sw.ToString();
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        ///     Accepts an XML serialized version of a custom property collection and returns the deserialized version of the XML.
        /// </summary>
        /// <param name="data">The XML representation of a custom property collection.</param>
        /// <returns>A deserialized version of a custom property collection object.</returns>
        public bool CustomPropertiesFromXml(string data)
        {
            var result = false;

            try
            {
                var tr = new StringReader(data);
                var xs = new XmlSerializer(CustomProperties.GetType());
                CustomProperties = (CustomPropertyCollection) xs.Deserialize(tr);
                if (CustomProperties != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                CustomProperties = new CustomPropertyCollection();
                result = false;
            }
            return result;
        }

        #endregion

        #region Public Methods / ITaxable

        /// <summary>
        ///     Sets the tax rate for this line item.
        /// </summary>
        /// <param name="taxRate">Decimal - the tax rate to use.</param>
        /// <returns>Decimal - the current tax rate.</returns>
        public decimal SetTaxRate(decimal taxRate)
        {
            return TaxRate = taxRate;
        }

        /// <summary>
        ///     Sets the shipping tax rate for this line item.
        /// </summary>
        /// <param name="shippingTaxRate">Decimal - the tax rate to use.</param>
        /// <returns>Decimal - the current tax rate.</returns>
        public decimal SetShippingTaxRate(decimal shippingTaxRate)
        {
            return ShippingTaxRate = shippingTaxRate;
        }

        /// <summary>
        ///     Resets all tax properties to zero.
        /// </summary>
        public void ClearTaxValue()
        {
            TaxPortion = 0m;
            TaxRate = 0;
            ShippingTaxRate = 0;
        }

        #endregion

        #region Public Methods

        public bool MarkedForFreeShipping(string shippingMethodId)
        {
            //Added ToUpperInvariant as many time shipping method value comes in small case and it fails on comparision 
            return IsMarkedForFreeShipping &&
                   (!FreeShippingMethodIds.Any() || FreeShippingMethodIds.Contains(shippingMethodId.ToUpperInvariant()));
        }

        /// <summary>
        ///     Returns the matching product record for this line item.
        /// </summary>
        /// <param name="app">An instance of the Hotcakes Application context.</param>
        /// <returns>A populated instance of the Product object</returns>
        public Product GetAssociatedProduct(HotcakesApplication app)
        {
            return app.CatalogServices.Products.FindWithCache(ProductId);
        }

        /// <summary>
        ///     A collection of the tokens and the replaceable content for email templates.
        /// </summary>
        /// <param name="context">An instance of the Hotcakes Request context.</param>
        /// <returns>List of HtmlTemplateTag</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();
            var culture = context.MainContentCulture;
            result.Add(new HtmlTemplateTag("[[LineItem.AdjustedPrice]]", HasAnyUpcharge ? (TotalUpcharge() + AdjustedPricePerItem).ToString("c") : AdjustedPricePerItem.ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.BasePrice]]", BasePricePerItem.ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.LineTotalBeforeVAT]]", (LineTotal - TaxPortion).ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.Discounts]]", DiscountDetailsAsHtml()));
            result.Add(new HtmlTemplateTag("[[LineItem.Upcharges]]", UpchargeDetailsAsHtml()));
            result.Add(new HtmlTemplateTag("[[LineItem.LineTotal]]", LineTotal.ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.ProductId]]", ProductId));
            result.Add(new HtmlTemplateTag("[[LineItem.VariantId]]", VariantId));
            result.Add(new HtmlTemplateTag("[[LineItem.ProductName]]", ProductName));
            result.Add(new HtmlTemplateTag("[[LineItem.ProductSku]]", ProductSku));
            result.Add(new HtmlTemplateTag("[[LineItem.ProductDescription]]", ProductShortDescription));
            result.Add(new HtmlTemplateTag("[[LineItem.Quantity]]", Quantity.ToString("#")));
            result.Add(new HtmlTemplateTag("[[LineItem.QuantityShipped]]", QuantityShipped.ToString("#")));
            result.Add(new HtmlTemplateTag("[[LineItem.QuantityReturned]]", QuantityReturned.ToString("#")));
            result.Add(new HtmlTemplateTag("[[LineItem.ShippingStatus]]",
                LocalizationUtils.GetOrderShippingStatus(ShippingStatus, culture)));
            result.Add(new HtmlTemplateTag("[[LineItem.TaxRate]]",
                decimal.Parse(TaxRate.ToString("p3").Replace("%", "")).ToString("G29") + " %"));
            result.Add(new HtmlTemplateTag("[[LineItem.ShippingPortion]]", ShippingPortion.ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.TaxPortion]]", TaxPortion.ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.ExtraShipCharge]]", ExtraShipCharge.ToString("c")));
            result.Add(new HtmlTemplateTag("[[LineItem.ShipFromAddress]]", ShipFromAddress.ToHtmlString()));
            result.Add(new HtmlTemplateTag("[[LineItem.ShipSeparately]]", ShipSeparately ? "Yes" : "No"));

            return result;
        }

        /// <summary>
        ///     Returns the weight of the total quantity of this line item.
        /// </summary>
        /// <returns>Decimal</returns>
        public decimal GetTotalWeight()
        {
            var weight = ProductShippingWeight;
            weight *= Quantity - QuantityShipped;
            return weight;
        }

        /// <summary>
        ///     Compares the ID of another line item to the current line item to determine if it's the same line item.
        /// </summary>
        /// <param name="other">LineItem - a populated instance of another line item.</param>
        /// <returns>Boolean - if true, the line item ID's match.</returns>
        public virtual bool Equals(LineItem other)
        {
            return Id == other.Id;
        }

        /// <summary>
        ///     Creates a copy of the current line item.
        /// </summary>
        /// <returns>LineItem</returns>
        public LineItem Clone()
        {
            return Clone(false);
        }

        /// <summary>
        ///     Creates a copy of the line item with the same ID as the original.
        /// </summary>
        /// <param name="copyId">If true, the line item ID will be copied.</param>
        /// <returns>LineItem</returns>
        public LineItem Clone(bool copyId)
        {
            var result = new LineItem();

            result.LastUpdatedUtc = LastUpdatedUtc;
            result.BasePricePerItem = BasePricePerItem;
            result.DiscountDetails = DiscountDetail.ListFromXml(DiscountDetail.ListToXml(DiscountDetails));
            result.OrderBvin = OrderBvin;
            result.ProductId = ProductId;
            result.VariantId = VariantId;
            result.ProductName = ProductName;
            result.ProductSku = ProductSku;
            result.ProductShortDescription = ProductShortDescription;
            result.Quantity = Quantity;
            result.QuantityReturned = QuantityReturned;
            result.QuantityShipped = QuantityShipped;
            result.ShippingPortion = ShippingPortion;
            result.StatusCode = StatusCode;
            result.StatusName = StatusName;
            result.TaxRate = TaxRate;
            result.TaxPortion = TaxPortion;
            result.SelectionData = SelectionData;
            result.IsNonShipping = IsNonShipping;
            result.ShippingCharge = ShippingCharge;
            result.TaxSchedule = TaxSchedule;
            result.ProductShippingHeight = ProductShippingHeight;
            result.ProductShippingLength = ProductShippingLength;
            result.ProductShippingWeight = ProductShippingWeight;
            result.ProductShippingWidth = ProductShippingWidth;
            result.ShipSeparately = ShipSeparately;

            foreach (var y in CustomProperties)
            {
                result.CustomProperties.Add(y.Clone());
            }

            if (copyId)
            {
                result.Id = Id;
            }

            return result;
        }

        public void AddPromotionId(long promotionId, int quantity)
        {
            if (PromotionIds != null)
            {
                var list = PromotionIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

                var hash = new Dictionary<long, int>();

                foreach (var item in list)
                {
                    var parts = item.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                    hash.Add(long.Parse(parts[0]), int.Parse(parts[1]));
                }

                if (hash.ContainsKey(promotionId))
                {
                    list.Remove(promotionId.ToString());
                    hash[promotionId] = quantity;
                }
                else
                {
                    hash.Add(promotionId, quantity);
                }

                PromotionIds = string.Join(",", hash.Select(h => h.Key.ToString() + "=" + h.Value.ToString()).ToList());

                FreeQuantity = hash.Sum(h => h.Value);
            }
            else
            {
                PromotionIds += "," + promotionId + "=" + quantity;
                FreeQuantity = quantity;
            }
        }

        public Dictionary<long, int> GetFreePromotions()
        {
            if (PromotionIds == null)
            {
                return new Dictionary<long, int>();
            }

            var list = PromotionIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

            var hash = new Dictionary<long, int>();

            foreach (var item in list)
            {
                var parts = item.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                hash.Add(long.Parse(parts[0]), int.Parse(parts[1]));
            }

            return hash;
        }

        public int GetFreeCountByPromotionId(long promotionId)
        {
            if (PromotionIds == null)
            {
                return -1;
            }

            var list = PromotionIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

            var hash = new Dictionary<long, int>();

            foreach (var item in list)
            {
                var parts = item.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                hash.Add(long.Parse(parts[0]), int.Parse(parts[1]));
            }

            if (hash.ContainsKey(promotionId))
            {
                return hash[promotionId];
            }
            return -1;
        }

        public void RemovePromotionId(long promotionId)
        {
            if (PromotionIds != null)
            {
                var list = PromotionIds.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

                var hash = new Dictionary<long, int>();

                foreach (var item in list)
                {
                    var parts = item.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                    hash.Add(long.Parse(parts[0]), int.Parse(parts[1]));
                }

                if (hash.ContainsKey(promotionId))
                {
                    hash.Remove(promotionId);
                    PromotionIds = string.Join(",",
                        hash.Select(h => h.Key.ToString() + "=" + h.Value.ToString()).ToList());
                }

                FreeQuantity = hash.Sum(h => h.Value);
            }
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current line object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of LineItemDTO</returns>
        public LineItemDTO ToDto()
        {
            var dto = new LineItemDTO();

            dto.Id = Id;
            dto.StoreId = StoreId;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.BasePricePerItem = BasePricePerItem;

            dto.PromotionIds = PromotionIds;
            dto.FreeQuantity = FreeQuantity;
            dto.LineTotal = LineTotal;
            dto.AdjustedPricePerItem = AdjustedPricePerItem;
            dto.IsUserSuppliedPrice = IsUserSuppliedPrice;
            dto.IsBundle = IsBundle;
            dto.IsGiftCard = IsGiftCard;

            foreach (var detail in DiscountDetails)
            {
                dto.DiscountDetails.Add(detail.ToDto());
            }
            dto.OrderBvin = OrderBvin ?? string.Empty;
            dto.ProductId = ProductId ?? string.Empty;
            dto.VariantId = VariantId ?? string.Empty;
            dto.ProductName = ProductName ?? string.Empty;
            dto.ProductSku = ProductSku ?? string.Empty;
            dto.ProductShortDescription = ProductShortDescription ?? string.Empty;
            dto.Quantity = Quantity;
            dto.QuantityReturned = QuantityReturned;
            dto.QuantityShipped = QuantityShipped;
            dto.ShippingPortion = ShippingPortion;
            dto.StatusCode = StatusCode ?? string.Empty;
            dto.StatusName = StatusName ?? string.Empty;
            dto.TaxRate = TaxRate;
            dto.TaxPortion = TaxPortion;
            foreach (var op in SelectionData.OptionSelectionList)
            {
                dto.SelectionData.Add(op.ToDto());
            }
            dto.IsNonShipping = IsNonShipping;
            dto.TaxSchedule = TaxSchedule;
            dto.ProductShippingHeight = ProductShippingHeight;
            dto.ProductShippingLength = ProductShippingLength;
            dto.ProductShippingWeight = ProductShippingWeight;
            dto.ProductShippingWidth = ProductShippingWidth;
            foreach (var cp in CustomProperties)
            {
                dto.CustomProperties.Add(cp.ToDto());
            }
            dto.ShipFromAddress = ShipFromAddress.ToDto();
            dto.ShipFromMode = (ShippingModeDTO) (int) ShipFromMode;
            dto.ShipFromNotificationId = ShipFromNotificationId ?? string.Empty;
            dto.ShipSeparately = ShipSeparately;
            dto.ExtraShipCharge = ExtraShipCharge;
            dto.ShippingCharge = (ShippingChargeTypeDTO) (int) ShippingCharge;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current line item object using a LineItemDTO instance
        /// </summary>
        /// <param name="dto">An instance of the line item from the REST API</param>
        public void FromDto(LineItemDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            StoreId = dto.StoreId;
            LastUpdatedUtc = dto.LastUpdatedUtc;
            BasePricePerItem = dto.BasePricePerItem;
            LineTotal = dto.LineTotal;
            AdjustedPricePerItem = dto.AdjustedPricePerItem;
            IsUserSuppliedPrice = dto.IsUserSuppliedPrice;
            IsBundle = dto.IsBundle;
            IsGiftCard = dto.IsGiftCard;
            PromotionIds = dto.PromotionIds;
            FreeQuantity = dto.FreeQuantity;

            DiscountDetails.Clear();
            if (dto.DiscountDetails != null)
            {
                foreach (var detail in dto.DiscountDetails)
                {
                    var d = new DiscountDetail();
                    d.FromDto(detail);
                    DiscountDetails.Add(d);
                }
            }
            OrderBvin = dto.OrderBvin ?? string.Empty;
            ProductId = dto.ProductId ?? string.Empty;
            VariantId = dto.VariantId ?? string.Empty;
            ProductName = dto.ProductName ?? string.Empty;
            ProductSku = dto.ProductSku ?? string.Empty;
            ProductShortDescription = dto.ProductShortDescription ?? string.Empty;
            Quantity = dto.Quantity;
            QuantityReturned = dto.QuantityReturned;
            QuantityShipped = dto.QuantityShipped;
            ShippingPortion = dto.ShippingPortion;
            TaxRate = dto.TaxRate;
            TaxPortion = dto.TaxPortion;
            StatusCode = dto.StatusCode ?? string.Empty;
            StatusName = dto.StatusName ?? string.Empty;
            SelectionData.Clear();
            if (dto.SelectionData != null)
            {
                foreach (var op in dto.SelectionData)
                {
                    var o = new OptionSelection();
                    o.FromDto(op);
                    SelectionData.OptionSelectionList.Add(o);
                }
            }
            IsNonShipping = dto.IsNonShipping;
            TaxSchedule = dto.TaxSchedule;
            ProductShippingHeight = dto.ProductShippingHeight;
            ProductShippingLength = dto.ProductShippingLength;
            ProductShippingWeight = dto.ProductShippingWeight;
            ProductShippingWidth = dto.ProductShippingWidth;
            CustomProperties.Clear();
            if (dto.CustomProperties != null)
            {
                foreach (var cpd in dto.CustomProperties)
                {
                    var prop = new CustomProperty();
                    prop.FromDto(cpd);
                    CustomProperties.Add(prop);
                }
            }
            ShipFromAddress.FromDto(dto.ShipFromAddress);
            ShipFromMode = (ShippingMode) (int) dto.ShipFromMode;
            ShipFromNotificationId = dto.ShipFromNotificationId ?? string.Empty;
            ShipSeparately = dto.ShipSeparately;
            ExtraShipCharge = dto.ExtraShipCharge;
            ShippingCharge = (ShippingChargeType) (int) dto.ShippingCharge;
        }

        #endregion
    }
}