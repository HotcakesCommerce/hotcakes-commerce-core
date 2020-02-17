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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Shipping;
using Hotcakes.Payment;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary class used for all products in the application
    /// </summary>
    [Serializable]
#pragma warning disable 0612, 0618
    public class Product : IEquatable<Product>, IReplaceable, ILocalizableModel, IPurchasable
#pragma warning restore 0612, 0618
    {
        public const int ProductNameMaxLength = 255;
        public const int SkuMaxLength = 50;

        public Product()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.MinValue;
            Sku = string.Empty;
            ProductName = string.Empty;
            ProductTypeId = string.Empty;
            Images = new List<ProductImage>();
            ListPrice = 0m;
            Reviews = new List<ProductReview>();
            SitePrice = 0m;
            SitePriceOverrideText = string.Empty;
            SiteCost = 0m;
            MetaKeywords = string.Empty;
            MetaDescription = string.Empty;
            MetaTitle = string.Empty;
            TaxExempt = false;
            TaxSchedule = -1;
            Status = ProductStatus.Active;
            ImageFileSmall = string.Empty;
            ImageFileMedium = string.Empty;
            ImageFileSmallAlternateText = string.Empty;
            ImageFileMediumAlternateText = string.Empty;
            CreationDateUtc = DateTime.UtcNow;
            MinimumQty = 1;
            ShortDescription = string.Empty;
            LongDescription = string.Empty;
            ManufacturerId = string.Empty;
            VendorId = string.Empty;
            GiftWrapAllowed = false;
            Keywords = string.Empty;
            TemplateName = "";
            PreContentColumnId = string.Empty;
            PostContentColumnId = string.Empty;
            UrlSlug = string.Empty;
            //Categories = new Collection<string>();
            InventoryMode = ProductInventoryMode.AlwayInStock;
            GiftWrapPrice = 0m;
            Options = new OptionList();
            Variants = new VariantList();
            ShippingDetails = new ShippableItem();
            Featured = false;
            AllowReviews = null;
            Tabs = new List<ProductDescriptionTab>();
            StoreId = 0;
            IsAvailableForSale = true;
            HiddenSearchKeywords = string.Empty;
            BundledProducts = new List<BundledProductAdv>();
            ShippingCharge = ShippingChargeType.ChargeShippingAndHandling;
        }

        bool IEquatable<Product>.Equals(Product other)
        {
            if (Bvin != other.Bvin)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     The template engine will call this method to get a list of the tokens and values that will be replaced in email
        ///     templates.
        /// </summary>
        /// <param name="context">An instance of the application</param>
        /// <returns>List of HtmlTemplateTags - a key/value pair of values to replace placeholders in email templates.</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();

            result.Add(new HtmlTemplateTag("[[Product.CreationDate]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, CreationDateUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[Product.ExtraShipFee]]", ShippingDetails.ExtraShipFee.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Product.ImageFileSmall]]", ImageFileSmall));
            result.Add(new HtmlTemplateTag("[[Product.ImageFileMedium]]", ImageFileMedium));

            var culture = context.MainContentCulture;
            var available = GlobalLocalization.GetString("InStock", culture);
            var notAvailable = GlobalLocalization.GetString("OutOfStock", culture);

            if (string.IsNullOrEmpty(available))
            {
                available = "Available";
            }

            if (string.IsNullOrEmpty(notAvailable))
            {
                notAvailable = "Backordered";
            }

            result.Add(new HtmlTemplateTag("[[Product.Keywords]]", Keywords));
            result.Add(new HtmlTemplateTag("[[Product.ListPrice]]", ListPrice.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Product.LongDescription]]", LongDescription));
            result.Add(new HtmlTemplateTag("[[Product.ManufacturerId]]", ManufacturerId));
            result.Add(new HtmlTemplateTag("[[Product.MetaKeywords]]", MetaKeywords));
            result.Add(new HtmlTemplateTag("[[Product.MetaDescription]]", MetaDescription));
            result.Add(new HtmlTemplateTag("[[Product.MetaTitle]]", MetaTitle));
            result.Add(new HtmlTemplateTag("[[Product.MinimumQty]]", MinimumQty.ToString("#")));
            result.Add(new HtmlTemplateTag("[[Product.NonShipping]]", ShippingDetails.IsNonShipping.ToString()));
            result.Add(new HtmlTemplateTag("[[Product.PostContentColumnId]]", PostContentColumnId));
            result.Add(new HtmlTemplateTag("[[Product.PreContentColumnId]]", PreContentColumnId));
            result.Add(new HtmlTemplateTag("[[Product.ProductName]]", ProductName));
            result.Add(new HtmlTemplateTag("[[Product.ProductTypeId]]", ProductTypeId));
            result.Add(new HtmlTemplateTag("[[Product.ShippingHeight]]", ShippingDetails.Height.ToString("#.##")));
            result.Add(new HtmlTemplateTag("[[Product.ShippingLength]]", ShippingDetails.Length.ToString("#.##")));
            result.Add(new HtmlTemplateTag("[[Product.ShippingWeight]]", ShippingDetails.Weight.ToString("#.##")));
            result.Add(new HtmlTemplateTag("[[Product.ShippingWidth]]", ShippingDetails.Width.ToString("#.##")));
            result.Add(new HtmlTemplateTag("[[Product.ShipSeparately]]", ShippingDetails.ShipSeparately.ToString()));
            result.Add(new HtmlTemplateTag("[[Product.ShortDescription]]", ShortDescription));
            result.Add(new HtmlTemplateTag("[[Product.SiteCost]]", SiteCost.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Product.SitePrice]]", SitePrice.ToString("c")));
            result.Add(new HtmlTemplateTag("[[Product.SitePriceOverrideText]]", SitePriceOverrideText));
            result.Add(new HtmlTemplateTag("[[Product.Sku]]", Sku));
            result.Add(new HtmlTemplateTag("[[Product.TaxExempt]]", TaxExempt.ToString()));
            //result.Add(new EmailTemplateTag("[[Product.TemplateName]]", TemplateName));
            result.Add(new HtmlTemplateTag("[[Product.TypeProperties]]", RenderTypeProperties(false, context)));
            result.Add(new HtmlTemplateTag("[[Product.TypePropertiesDropShipper]]", RenderTypeProperties(true, context)));
            result.Add(new HtmlTemplateTag("[[Product.VendorId]]", VendorId));

            return result;
        }

        public TimeSpan GetRecurringSpan(DateTime startDate, int quantity, bool firstSpan = false)
        {
            var timeSpan = TimeSpan.Zero;
            switch (RecurringIntervalType)
            {
                case RecurringIntervalType.Days:
                    timeSpan = startDate.AddDays(RecurringInterval*quantity) - startDate;
                    break;
                case RecurringIntervalType.Months:
                    timeSpan = startDate.AddMonths(RecurringInterval*quantity) - startDate;
                    break;
                default:
                    throw new Exception("RecurringIntervalType is not supported");
            }
            // Add one day since payment can be done with delay
            // and we want prevent interval without membership
            if (firstSpan)
                timeSpan = timeSpan.Add(TimeSpan.FromDays(1));
            return timeSpan;
        }

        #region Pricing Functions

        /// <summary>
        ///     This method will return the current price of the product.
        /// </summary>
        /// <param name="userBvin">String - the unique ID of the customer (currently not used)</param>
        /// <param name="adjustment">Decimal - the amount to adjust the price by (currently not used)</param>
        /// <param name="li">LineItem - the line item used to determine price (currently not used)</param>
        /// <param name="variantId">String - the unique ID of the variant</param>
        /// <returns>
        ///     If a variant is found and it is greater than 0.00, that price is returned. Otherwise, the SitePrice property
        ///     is returned.
        /// </returns>
        public decimal GetCurrentPrice(string userBvin, decimal adjustment, LineItem li, string variantId)
        {
            var result = SitePrice;

            // pull basic site price from product
            if (variantId != string.Empty)
            {
                var v = Variants.FindByBvin(variantId);
                if (v != null)
                {
                    if (v.Price >= 0) result = v.Price;
                }
            }

            return result;
        }

        #endregion

        // Equality Functions
        private static Collection<T> CopyCollection<T>(Collection<T> data)
        {
            var result = new Collection<T>();
            foreach (var item in data)
            {
                result.Add(item);
            }
            return result;
        }

        #region "Custom Properties"

        private CustomPropertyCollection _CustomProperties = new CustomPropertyCollection();

        /// <summary>
        ///     A collection of custom product meta data that can be used for any API or integration-based purposes.
        /// </summary>
        public CustomPropertyCollection CustomProperties
        {
            get { return _CustomProperties; }
            set { _CustomProperties = value; }
        }

        /// <summary>
        ///     This method allows you to query the product to see if the specified custom property exists already.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="propertyKey">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <returns>If true, the custom property exists, matching the devId and propertyKey</returns>
        public bool CustomPropertyExists(string devId, string propertyKey)
        {
            var result = false;
            for (var i = 0; i <= _CustomProperties.Count - 1; i++)
            {
                if (_CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (_CustomProperties[i].Key.Trim().ToLower() == propertyKey.Trim().ToLower())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     When used, this method allows you to either save or update a custom property for the current product.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <param name="value">String - this is the value that the property should be saving and returned when the key is queried.</param>
        public void CustomPropertySet(string devId, string key, string value)
        {
            var found = false;

            for (var i = 0; i <= _CustomProperties.Count - 1; i++)
            {
                if (_CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (_CustomProperties[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        _CustomProperties[i].Value = value;
                        found = true;
                        break;
                    }
                }
            }

            if (found == false)
            {
                _CustomProperties.Add(new CustomProperty(devId, key, value));
            }
        }

        /// <summary>
        ///     When used, this method allows you to either save or update a custom property for the current product.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <param name="value">Long - this is the value that the property should be saving and returned when the key is queried.</param>
        public void CustomPropertySet(string devId, string key, long value)
        {
            CustomPropertySet(devId, key, value.ToString());
        }

        /// <summary>
        ///     When called, this method queries the custom properties to find and return the one that matches the specified devId
        ///     and key.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <returns>If found, the value of the custom property is returned. Otherwise, an empty string will be returned.</returns>
        public string CustomPropertyGet(string devId, string key)
        {
            var result = string.Empty;
            var prop = _CustomProperties
                .Where(y => y.DeveloperId.Trim().ToLowerInvariant() == devId.Trim().ToLowerInvariant())
                .Where(y => y.Key.Trim().ToLowerInvariant() == key.Trim().ToLowerInvariant())
                .FirstOrDefault();
            if (prop != null)
            {
                result = prop.Value;
            }
            return result;
        }

        /// <summary>
        ///     When called, this method queries the custom properties to find and return the one that matches the specified devId
        ///     and key.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <returns>If found, true will be returned. Otherwise, this method will default to false.</returns>
        public bool CustomPropertyGetAsBool(string devId, string key)
        {
            var v = CustomPropertyGet(devId, key);
            var result = false;
            bool.TryParse(v, out result);
            return result;
        }

        /// <summary>
        ///     When called, this method queries the custom properties to find and return the one that matches the specified devId
        ///     and key.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <returns>If found, the saved value will be returned. Otherwise, this method will return 0.</returns>
        public long CustomPropertyGetAsLong(string devId, string key)
        {
            var v = CustomPropertyGet(devId, key);
            long result = 0;
            long.TryParse(v, out result);
            if (result < 0) result = 0;
            return result;
        }

        /// <summary>
        ///     When used, this method allows you to either save or update a custom property for the current product.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        /// <param name="value">
        ///     Boolean - this is the value that the property should be saving and returned when the key is
        ///     queried.
        /// </param>
        public void CustomPropertySetAsBool(string devId, string key, bool value)
        {
            CustomPropertySet(devId, key, value.ToString());
        }

        /// <summary>
        ///     When used, this method allows you to permanently remove a custom property.
        /// </summary>
        /// <param name="devId">
        ///     String - this is a value that's unique to the developer to group together custom properties to save
        ///     safely.
        /// </param>
        /// <param name="key">String - this is the key that the property is saved under that is used to find a value for.</param>
        public bool CustomPropertyRemove(string devId, string key)
        {
            var result = false;

            for (var i = 0; i <= _CustomProperties.Count - 1; i++)
            {
                if (_CustomProperties[i].DeveloperId.Trim().ToLower() == devId.Trim().ToLower())
                {
                    if (_CustomProperties[i].Key.Trim().ToLower() == key.Trim().ToLower())
                    {
                        _CustomProperties.Remove(_CustomProperties[i]);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     This method will write all of the custom properties to XML and return it as a string.
        /// </summary>
        /// <returns>String - the XML representation of all of the custom properties for the product.</returns>
        /// <remarks>Any issues that are caused by this method are written to the EventLog.</remarks>
        public string CustomPropertiesToXml()
        {
            var result = string.Empty;

            try
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(_CustomProperties.GetType());
                xs.Serialize(sw, _CustomProperties);
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
        ///     Using this method will allow you to load all of the custom properties from an XML data source into the
        ///     CustomProperties property.
        /// </summary>
        /// <param name="data">String - the XML representation of the serialized custom properties collection.</param>
        /// <returns>
        ///     If no errors occured, true is returned and the CustomProperties property will have one or many custom
        ///     properties available.
        /// </returns>
        /// <remarks>
        ///     If any errors occur, they will be written to the EventLog and the CustomProperties property will contain an
        ///     empty collection.
        /// </remarks>
        public bool CustomPropertiesFromXml(string data)
        {
            var result = false;

            try
            {
                var tr = new StringReader(data);
                var xs = new XmlSerializer(_CustomProperties.GetType());
                _CustomProperties = (CustomPropertyCollection) xs.Deserialize(tr);
                if (_CustomProperties != null)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                _CustomProperties = new CustomPropertyCollection();
                result = false;
            }
            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     This is the numeric version of the unique ID of the product as generated by the database.
        /// </summary>
        /// <remarks>The application will not use this property. Bvin should be used instead for all integration and development.</remarks>
        public long Id { get; set; }

        /// <summary>
        ///     The unique ID or primary key of the product.
        /// </summary>
        /// <remarks>This property should always be used instead of Id.</remarks>
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; } //DateTime.MinValue;

        /// <summary>
        ///     A SKU is the "stock keeping unit" and is often used as a primary or unique key to identify the product across
        ///     multiple mediums and systems.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        ///     The product name is the title of the product as a customer will see it.
        /// </summary>
        /// <remarks>This value should be the localized version of the product name.</remarks>
        public string ProductName { get; set; }

        /// <summary>
        ///     A unique ID matching a product type that this product should be treated as.
        /// </summary>
        public string ProductTypeId { get; set; }

        /// <summary>
        ///     This is the price that the product is normally sold to the public as.
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        ///     The site price reflects the actual price that a customer will pay, regardless to the list price (MSRP).
        /// </summary>
        /// <remarks>This value will not reflect changes made by sales, price groups, or offers.</remarks>
        public decimal SitePrice { get; set; }

        /// <summary>
        ///     This value is set in the product editing view and allows you to supply a user-friendly message about the prive to
        ///     customers.
        /// </summary>
        /// <remarks>This value is not currently used in the application.</remarks>
        public string SitePriceOverrideText { get; set; }

        /// <summary>
        ///     This property should reflect the amount that this product cost to acquire.
        /// </summary>
        public decimal SiteCost { get; set; }

        /// <summary>
        ///     If true, this property defines that this product should allow a customer to define the price, such as a donation
        ///     value.
        /// </summary>
        public bool IsUserSuppliedPrice { get; set; }

        /// <summary>
        ///     If true, this product contains a collection of products, sold as a group.
        /// </summary>
        public bool IsBundle { get; set; }

        /// <summary>
        ///     If true, this product represents a gift certificate.
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        ///     This is the localized label that will be displayed next to the user's supplied price textbox.
        /// </summary>
        public string UserSuppliedPriceLabel { get; set; }

        /// <summary>
        ///     If true, this product requires recurring payment.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        ///     Defines how often payments will be done
        /// </summary>
        public int RecurringInterval { get; set; }

        /// <summary>
        ///     Defines period type of recurring payments
        /// </summary>
        public RecurringIntervalType RecurringIntervalType { get; set; }

        /// <summary>
        ///     If true, the user interface should not display a quantity textbox to the customer.
        /// </summary>
        /// <remarks>If true, the mimimun quantity property will be used.</remarks>
        public bool HideQty { get; set; }

        /// <summary>
        ///     The meta keywords are used for both indexing the product in the local search engine and for injecting into the meta
        ///     data of the web page.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        ///     The meta description is used for both indexing the product in the local search engine and for injecting into the
        ///     meta data of the web page.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        ///     The meta title is used for both indexing the product in the local search engine and for injecting into the meta
        ///     data of the web page.
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        ///     If true, tax rules and calculation will not be applied to this product.
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        ///     When specified, this allows the current product to be applied against the specified tax schedule.
        /// </summary>
        public long TaxSchedule { get; set; }

        /// <summary>
        ///     This represents a collection of properties related to shipping the current product.
        /// </summary>
        public ShippableItem ShippingDetails { get; set; }

        /// <summary>
        ///     This property maps to the "active" checkbox in the user interface and determines if the product will be available
        ///     to customers or not.
        /// </summary>
        public ProductStatus Status { get; set; }

        /// <summary>
        ///     The file name of the small version of the primary product image.
        /// </summary>
        public string ImageFileSmall { get; set; }

        /// <summary>
        ///     The localized text saved specifically to be used for the small version of the primary product image.
        /// </summary>
        public string ImageFileSmallAlternateText { get; set; }

        /// <summary>
        ///     The file name of the medium version of the primary product image.
        /// </summary>
        public string ImageFileMedium { get; set; }

        /// <summary>
        ///     The localized text saved specifically to be used for the medium version of the primary product image.
        /// </summary>
        public string ImageFileMediumAlternateText { get; set; }

        /// <summary>
        ///     The creation date is used for auditing purposes to know when the product was first created.
        /// </summary>
        [XmlIgnore]
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        ///     When greater than zero, this value will be the minimum allowable quantity for the product before the cart allows it
        ///     to be added.
        /// </summary>
        public int MinimumQty { get; set; }

        /// <summary>
        ///     A shorter version of the LongDescription property.
        /// </summary>
        /// <remarks>This property is currently not being used in the application.</remarks>
        public string ShortDescription { get; set; }

        /// <summary>
        ///     This is the product description that is shown to customers.
        /// </summary>
        /// <remarks>This value contains HTML.</remarks>
        public string LongDescription { get; set; }

        /// <summary>
        ///     This property defines a manufacturer that this product is created by.
        /// </summary>
        public string ManufacturerId { get; set; }

        /// <summary>
        ///     This property defines a vendor that this product is distributed by.
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        ///     If true, the current product can be gift wrapped prior to shipping for delivery.
        /// </summary>
        /// <remarks>This property is currently not being used in the application.</remarks>
        public bool GiftWrapAllowed { get; set; }

        /// <summary>
        ///     This property contains a comma delimited list of variant SKU's when necessary, assisting with the native search
        ///     engine.
        /// </summary>
        /// <remarks>This property should not be manually updated outside of the application logic.</remarks>
        public string HiddenSearchKeywords { get; set; }

        /// <summary>
        ///     If gift wrap is enabled for this product and selected, this price will be added to the order total.
        /// </summary>
        /// <remarks>This property is currently not being used in the application.</remarks>
        public decimal GiftWrapPrice { get; set; }

        /// <summary>
        ///     This property contains search keywords that will be used by the native search engine, but not exposed to public
        ///     search engines.
        /// </summary>
        /// <remarks>This is a great place to put keywords that might relate to competitive product/service names.</remarks>
        public string Keywords { get; set; }

        /// <summary>
        ///     When speficied, the current product will use this template, unless overridden at the module settings.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        ///     If specified, the content block matching the given ID will be presented as a header element above the product
        ///     listing.
        /// </summary>
        public string PreContentColumnId { get; set; }

        /// <summary>
        ///     If specified, the content block matching the given ID will be presented as a footer element below the product
        ///     listing.
        /// </summary>
        public string PostContentColumnId { get; set; }

        /// <summary>
        ///     This unique value will be the final set of characters after the last slash in the page URL.
        /// </summary>
        public string UrlSlug { get; set; }

        /// <summary>
        ///     The value of this property defines how the product will be treated by the application once it is out of stock.
        /// </summary>
        public ProductInventoryMode InventoryMode { get; set; }

        /// <summary>
        ///     A dymanically propulated property to help the views know if this product is available, based upon status and
        ///     inventory mode/levels.
        /// </summary>
        public bool IsAvailableForSale { get; set; }

        /// <summary>
        ///     When set to true, this property tells the application to allow this product to be listed in the featured product
        ///     view.
        /// </summary>
        public bool Featured { get; set; }

        /// <summary>
        ///     If true, this product will be indexed and returned in the search results.
        /// </summary>
        public bool IsSearchable { get; set; }

        /// <summary>
        ///     If set to true, this product will allow reviews to be saved by customers.
        /// </summary>
        public bool? AllowReviews { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     This property is intended to hold the localization-friendly name of the culture for the product.
        /// </summary>
        /// <remarks>This property is inherited from an implementation and is currently not being used in the application.</remarks>
        public string ContentCulture { get; set; }

        /// <summary>
        ///     A collection of info tabs that contain additional product details and are generally found below the product
        ///     description.
        /// </summary>
        public List<ProductDescriptionTab> Tabs { get; set; }

        /// <summary>
        ///     A collections of products that are bundled into this product record.
        /// </summary>
        public List<BundledProductAdv> BundledProducts { get; set; }

        /// <summary>
        ///     Determines how product shipping will be charged on shippable products.
        /// </summary>
        /// <value>
        ///     The shipping charge.
        /// </value>
        public ShippingChargeType ShippingCharge { get; set; }

        /// <summary>
        ///     This property simply returns the ShippingMode from the local ShippingDetails property.
        /// </summary>
        public ShippingMode ShippingMode
        {
            get { return ShippingDetails.ShippingSource; }
            set { ShippingDetails.ShippingSource = value; }
        }

        /// <summary>
        ///     If the product was created less than 30 days ago, this property will return true.
        /// </summary>
        public bool IsNew
        {
            get { return (DateTime.UtcNow - CreationDateUtc).Days <= WebAppSettings.NewProductBadgeDays; }
        }

        #region XML

        /// <summary>
        ///     This method will populate the Tabs property with tab information from an XML data source.
        /// </summary>
        /// <param name="xml">String - the XML serialized representation of the product.</param>
        public void TabsFromXml(string xml)
        {
            if (xml == string.Empty) return;

            var xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            // add blocks from xml
            XmlNodeList tabNodes;
            tabNodes = xdoc.SelectNodes("/DescriptionTabs/ProductDescriptionTab");

            Tabs.Clear();
            if (tabNodes != null)
            {
                for (var i = 0; i <= tabNodes.Count - 1; i++)
                {
                    var t = new ProductDescriptionTab();
                    t.FromXmlString(tabNodes[i].OuterXml);
                    Tabs.Add(t);
                }
            }
        }

        /// <summary>
        ///     This method will write the Tabs property to XML.
        /// </summary>
        /// <returns>String - the XML serialized representation of the Tabs property.</returns>
        public string TabsToXml()
        {
            var result = string.Empty;

            try
            {
                var writerSettings = new XmlWriterSettings();
                var response = string.Empty;
                var sb = new StringBuilder();
                writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
                var xw = XmlWriter.Create(sb, writerSettings);

                xw.WriteStartElement("DescriptionTabs");
                foreach (var t in Tabs)
                {
                    t.ToXmlWriter(ref xw);
                }
                xw.WriteEndElement();
                xw.Flush();
                xw.Close();
                result = sb.ToString();
            }

            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }

            return result;
        }

        #endregion

        /// <summary>
        ///     This property simply returns the appropriate value from the TaxExempt property.
        /// </summary>
        /// <returns>If true, tax rules and calculation will be applied to this product.</returns>
        public bool IsTaxable()
        {
            return !TaxExempt;
        }

        /// <summary>
        ///     Returns the path saved by the merchant in the product edit view
        /// </summary>
        /// <remarks>This should match up with a folder in the swatches folder of the Hotcakes data directory</remarks>
        public string SwatchPath
        {
            get { return CustomPropertyGet("hcc", "swatchpath"); }
            set
            {
                var temp = value;
                temp = temp.Replace("..", string.Empty);
                temp = temp.Replace(".", string.Empty);
                temp = temp.Trim();
                temp = temp.TrimStart('/');
                CustomPropertySet("hcc", "swatchpath", temp);
            }
        }

        /// <summary>
        ///     If the product has at least one Option, this property will return true.
        /// </summary>
        /// <returns>If true, there are one or more Options available for this product.</returns>
        public bool HasOptions()
        {
            return Options.Count > 0;
        }

        /// <summary>
        ///     If the product has at least one Variant, this property will return true.
        /// </summary>
        /// <returns>If true, there are one or more Variants available for this product.</returns>
        public bool HasVariants()
        {
            return Variants.Count > 0;
        }

        #endregion

        #region Sub Items

        //public Collection<string> Categories { get; private set; }

        /// <summary>
        ///     A collection of the additional images associated with this product.
        /// </summary>
        public List<ProductImage> Images { get; set; }

        /// <summary>
        ///     A collection of all of the reviews that relate to this product.
        /// </summary>
        public List<ProductReview> Reviews { get; set; }

        /// <summary>
        ///     A collection of the approved reviews that related to this product, based on the Reviews property.
        /// </summary>
        public List<ProductReview> ReviewsApproved
        {
            get
            {
                var result = new List<ProductReview>();
                foreach (var p in Reviews)
                {
                    if (p.Approved)
                    {
                        result.Add(p);
                    }
                }
                return result;
            }
        }

        /// <summary>
        ///     This property contains a collection of the options (choices) that will be presented to a customer.
        /// </summary>
        public OptionList Options { get; set; }

        /// <summary>
        ///     This is a collection of the variants that belong to this product.
        /// </summary>
        public VariantList Variants { get; set; }

        #endregion

        #region Clone

        /// <summary>
        ///     This method allows you to clone the current product to create a new one based upon it.
        /// </summary>
        /// <param name="cloneProductChoicesAndInputs">
        ///     If true, the Options (choices) will be cloned along with the rest of the
        ///     Product properties.
        /// </param>
        /// <param name="cloneProductImages">If true, the Images will be cloned along with the rest of the Product properties.</param>
        /// <returns>A copy of the current product with an empty Bvin (ID)</returns>
        public Product Clone(bool cloneProductChoicesAndInputs, bool cloneProductImages)
        {
            var result = new Product();

            result.AllowReviews = AllowReviews;
            result.Bvin = string.Empty;
            result.CreationDateUtc = DateTime.UtcNow;

            foreach (var prop in CustomProperties)
            {
                result.CustomProperties.Add(prop.DeveloperId, prop.Key, prop.Value);
            }

            result.Featured = Featured;
            result.GiftWrapAllowed = GiftWrapAllowed;
            result.GiftWrapPrice = GiftWrapPrice;
            if (cloneProductImages)
            {
                result.ImageFileSmall = ImageFileSmall;
                result.ImageFileMedium = ImageFileMedium;

                result.ImageFileSmallAlternateText = ImageFileSmallAlternateText;
                result.ImageFileMediumAlternateText = ImageFileMediumAlternateText;

                foreach (var img in Images)
                {
                    var imgClone = img.Clone();
                    imgClone.ProductId = string.Empty;
                    result.Images.Add(imgClone);
                }
            }
            result.InventoryMode = InventoryMode;
            result.IsSearchable = IsSearchable;
            result.IsAvailableForSale = IsAvailableForSale;
            result.IsBundle = IsBundle;
            result.IsUserSuppliedPrice = IsUserSuppliedPrice;
            result.HideQty = HideQty;
            result.UserSuppliedPriceLabel = UserSuppliedPriceLabel;
            result.Keywords = Keywords;
            result.ListPrice = ListPrice;
            result.LongDescription = LongDescription;
            result.ManufacturerId = ManufacturerId;
            result.MetaDescription = MetaDescription;
            result.MetaKeywords = MetaKeywords;
            result.MetaTitle = MetaTitle;
            result.MinimumQty = MinimumQty;

            result.PostContentColumnId = PostContentColumnId;
            result.PreContentColumnId = PreContentColumnId;
            result.ProductName = ProductName;
            result.ProductTypeId = ProductTypeId;

            result.ShippingDetails.ExtraShipFee = ShippingDetails.ExtraShipFee;
            result.ShippingDetails.Height = ShippingDetails.Height;
            result.ShippingDetails.IsNonShipping = ShippingDetails.IsNonShipping;
            result.ShippingDetails.Length = ShippingDetails.Length;
            result.ShippingDetails.ShippingSource = ShippingDetails.ShippingSource;
            //ShippingDetails.ShippingSourceAddress.CopyTo(result.ShippingDetails.ShippingSourceAddress);
            result.ShippingDetails.ShippingSourceId = ShippingDetails.ShippingSourceId;
            result.ShippingDetails.ShipSeparately = ShippingDetails.ShipSeparately;
            result.ShippingDetails.Weight = ShippingDetails.Weight;
            result.ShippingDetails.Width = ShippingDetails.Width;

            result.ShippingMode = ShippingMode;
            result.ShortDescription = ShortDescription;
            result.SiteCost = SiteCost;
            result.SitePrice = SitePrice;
            result.SitePriceOverrideText = SitePriceOverrideText;
            result.Sku = Sku;
            result.Status = Status;
            result.StoreId = StoreId;

            result.ShippingCharge = ShippingCharge;

            foreach (var tab in Tabs)
            {
                result.Tabs.Add(new ProductDescriptionTab
                {
                    HtmlData = tab.HtmlData,
                    SortOrder = tab.SortOrder,
                    TabTitle = tab.TabTitle,
                    LastUpdated = DateTime.UtcNow
                });
            }

            //foreach (var bundledProduct in BundledProducts)
            //{
            //	result.Tabs.Add(new ProductDescriptionTab()
            //	{
            //		HtmlData = tab.HtmlData,
            //		SortOrder = tab.SortOrder,
            //		TabTitle = tab.TabTitle,
            //		LastUpdated = DateTime.UtcNow
            //	});
            //}

            result.TaxExempt = TaxExempt;
            result.TaxSchedule = TaxSchedule;
            result.UrlSlug = string.Empty;
            result.VendorId = VendorId;

            if (cloneProductChoicesAndInputs)
            {
                foreach (var opt in Options)
                {
                    var c = opt.Clone();
                    result.Options.Add(c);
                }
                //result.Variants = Variants;
            }

            result.Bvin = Guid.NewGuid().ToString();

            return result;
        }

        /// <summary>
        ///     This method allows you to clone the current product to create a new one based upon it.
        /// </summary>
        /// <returns>A copy of the current product with an empty Bvin (ID)</returns>
        public Product Clone()
        {
            return Clone(true, true);
        }

        #endregion

        #region Selections

        /// <summary>
        ///     This method will validate the selections made to ensure that they can be made.
        /// </summary>
        /// <param name="selections">A collection of selections</param>
        /// <returns>ValidateSelectionsResult - basically a true or false result</returns>
        public ValidateSelectionsResult ValidateSelections(OptionSelections selections)
        {
            if (selections.HasEmptySelection(this))
                return ValidateSelectionsResult.RequiredOptionNotSelected;

            //if (selections.HasLabelsSelected())
            //	return ValidateSelectionsResult.LabelsSelected;

            if (!IsBundle)
            {
                return ValidateSingleProductSelections(this, selections.OptionSelectionList);
            }
            foreach (var bundledProductAdv in BundledProducts)
            {
                var bundledProduct = bundledProductAdv.BundledProduct;
                if (bundledProduct != null)
                {
                    var optionSelection = selections.GetSelections(bundledProductAdv.Id);

                    var result = ValidateSingleProductSelections(bundledProduct, optionSelection);
                    if (result != ValidateSelectionsResult.Success)
                        return result;
                }
            }
            return ValidateSelectionsResult.Success;
        }

        private static ValidateSelectionsResult ValidateSingleProductSelections(Product product,
            OptionSelectionList selections)
        {
            if (product.HasOptions())
            {
                if (product.HasVariants())
                {
                    var v = product.Variants.FindBySelectionData(selections, product.Options);
                    if (v == null)
                        return ValidateSelectionsResult.OptionsNotAvailable;
                }
            }
            return ValidateSelectionsResult.Success;
        }

        #endregion

        #region Product Type Properties

        /// <summary>
        ///     This method will return a collection of the product type properties that match the product type of the current
        ///     product.
        /// </summary>
        /// <returns>Either a populated or empty list of ProductProperty</returns>
        public List<ProductProperty> GetProductTypeProperties()
        {
            var app = HotcakesApplication.Current;

            return GetProductTypeProperties(app);
        }

        /// <summary>
        ///     This method will return a collection of the product type properties that match the product type of the current
        ///     product.
        /// </summary>
        /// <param name="app">A populated instance of the HccApplication object (if null, the method will create this for you)</param>
        /// <returns>Either a populated or empty list of ProductProperty</returns>
        public List<ProductProperty> GetProductTypeProperties(HotcakesApplication app)
        {
            if (app == null) app = HotcakesApplication.Current;

            return app.CatalogServices.ProductPropertiesFindForType(ProductTypeId);
        }

        /// <summary>
        ///     Allows you to return a specific product type property by name
        /// </summary>
        /// <param name="name">The name of the product type property you want to find</param>
        /// <returns>You will get a ProductPropertySnapshot object if the property exists, otherwise it will be a NULL object.</returns>
        public ProductPropertySnapshot GetProductTypeProperty(string name)
        {
            return GetProductTypeProperty(null, name);
        }

        /// <summary>
        ///     Allows you to return a specific product type property by name
        /// </summary>
        /// <param name="app">A populated instance of the HccApplication object (if null, the method will create this for you)</param>
        /// <param name="name">The name of the product type property you want to find</param>
        /// <returns>You will get a ProductPropertySnapshot object if the property exists, otherwise it will be a NULL object.</returns>
        public ProductPropertySnapshot GetProductTypeProperty(HotcakesApplication app, string name)
        {
            if (app == null) app = HotcakesApplication.Current;

            var prop = GetProductTypeProperties(app).FirstOrDefault(pp => pp.PropertyName == name);

            if (prop != null)
            {
                var val = app.CatalogServices.ProductPropertyValues.FindByProductIdAndPropertyId(Bvin, prop.Id);

                return new ProductPropertySnapshot
                {
                    DisplayName = prop.DisplayName,
                    PropertyName = prop.PropertyName,
                    TypeCode = prop.TypeCode,
                    PropertyValue = FormatProductPropertyChoiceValue(prop, val)
                };
            }

            return null;
        }

        /// <summary>
        ///     When called, this method will output a HTML listing of the product type properties associated with this product.
        /// </summary>
        /// <param name="forDropShipper">
        ///     Boolean - If true, the list will filter out properties that are not allowed to be shown to
        ///     drop shippers.
        /// </param>
        /// <param name="context">An instance of the Hotcakes Request context</param>
        /// <returns>String - HTML unordered list output representing the product type properties.</returns>
        public string RenderTypeProperties(bool forDropShipper = false, HccRequestContext context = null)
        {
            if (context == null)
                context = HccRequestContext.Current;

            var productId = Bvin;
            var productTypeId = ProductTypeId;

            var catalogServices = Factory.CreateService<CatalogService>(context);
            var propertyValues = catalogServices.ProductPropertyValues.FindByProductId(Bvin);
            var props = catalogServices.ProductPropertiesFindForType(productTypeId);

            var displayable = props.
                Where(y => (y.DisplayToDropShipper && forDropShipper) || y.DisplayOnSite).
                ToList();

            if (displayable.Count < 1) return string.Empty;

            var sb = new StringBuilder();
            sb.Append("<ul class=\"hc-typedisplay\">");
            foreach (var prop in displayable)
            {
                var propValue = propertyValues.Where(y => y.PropertyID == prop.Id).FirstOrDefault();
                var currentValue = FormatProductPropertyChoiceValue(prop, propValue);
                // If text property is empty, do not display
                if (string.IsNullOrEmpty(currentValue))
                {
                    continue;
                }
                sb.Append("<li>");
                sb.Append("<span class=\"hc-propertylabel\">");
                sb.Append(prop.DisplayName);
                sb.Append("</span>");
                sb.Append("<span class=\"hc-propertyvalue\">");
                sb.Append(currentValue);
                sb.Append("</span>");
                sb.Append("</li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        }

        private string FormatProductPropertyChoiceValue(ProductProperty prop, ProductPropertyValue propValue)
        {
            if (propValue == null)
                return string.Empty;

            switch (prop.TypeCode)
            {
                case ProductPropertyType.MultipleChoiceField:
                    long choiceId = -1;
                    long.TryParse(propValue.PropertyValue, out choiceId);
                    var choice = prop.Choices.Where(c => c.Id == choiceId).FirstOrDefault();
                    return choice != null ? choice.DisplayName : string.Empty;
                case ProductPropertyType.CurrencyField:
                    var info = new CultureInfo(prop.CultureCode);
                    decimal temp2 = -1;
                    decimal.TryParse(propValue.PropertyValue, out temp2);
                    return string.Format(info.NumberFormat, "{0:c}", temp2);
                case ProductPropertyType.TextField:
                    return prop.IsLocalizable ? propValue.PropertyLocalizableValue : propValue.PropertyValue;
                default:
                    return propValue.PropertyValue;
            }
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to populate the current product object using a ProductDTO instance
        /// </summary>
        /// <param name="dto">An instance of the product from the REST API</param>
        public void FromDto(ProductDTO dto)
        {
            AllowReviews = dto.AllowReviews;
            Bvin = dto.Bvin ?? string.Empty;
            CreationDateUtc = dto.CreationDateUtc;

            CustomProperties.Clear();
            foreach (var prop in dto.CustomProperties)
            {
                var prop1 = new CustomProperty();
                prop1.FromDto(prop);
                CustomProperties.Add(prop1);
            }

            Featured = dto.Featured;
            GiftWrapAllowed = dto.GiftWrapAllowed;
            GiftWrapPrice = dto.GiftWrapPrice;
            ImageFileMedium = dto.ImageFileMedium ?? string.Empty;
            ImageFileMediumAlternateText = dto.ImageFileMediumAlternateText ?? string.Empty;
            ImageFileSmall = dto.ImageFileSmall ?? string.Empty;
            ImageFileSmallAlternateText = dto.ImageFileSmallAlternateText ?? string.Empty;
            InventoryMode = (ProductInventoryMode) (int) dto.InventoryMode;
            IsAvailableForSale = dto.IsAvailableForSale;
            Keywords = dto.Keywords ?? string.Empty;
            ListPrice = dto.ListPrice;
            LongDescription = dto.LongDescription ?? string.Empty;
            ManufacturerId = dto.ManufacturerId ?? string.Empty;
            MetaDescription = dto.MetaDescription ?? string.Empty;
            MetaKeywords = dto.MetaKeywords ?? string.Empty;
            MetaTitle = dto.MetaTitle ?? string.Empty;
            MinimumQty = dto.MinimumQty;
            //Options = dto.Options;
            PostContentColumnId = dto.PostContentColumnId ?? string.Empty;
            PreContentColumnId = dto.PreContentColumnId ?? string.Empty;
            ProductName = dto.ProductName ?? string.Empty;
            ProductTypeId = dto.ProductTypeId ?? string.Empty;
            ShippingDetails.FromDto(dto.ShippingDetails);
            ShippingMode = (ShippingMode) (int) dto.ShippingMode;
            ShortDescription = dto.ShortDescription ?? string.Empty;
            SiteCost = dto.SiteCost;
            SitePrice = dto.SitePrice;
            SitePriceOverrideText = dto.SitePriceOverrideText ?? string.Empty;
            Sku = dto.Sku ?? string.Empty;
            Status = (ProductStatus) (int) dto.Status;
            StoreId = dto.StoreId;

            Tabs.Clear();
            foreach (var t in dto.Tabs)
            {
                var tab = new ProductDescriptionTab();
                tab.FromDto(t);
                Tabs.Add(tab);
            }

            TaxExempt = dto.TaxExempt;
            TaxSchedule = dto.TaxSchedule;
            UrlSlug = dto.UrlSlug ?? string.Empty;
            //Variants = dto.Variants;
            VendorId = dto.VendorId ?? string.Empty;

            IsSearchable = dto.IsSearchable;

            ShippingCharge = (ShippingChargeType) (int) dto.ShippingCharge;
        }

        /// <summary>
        ///     Allows you to convert the current product object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductDTO</returns>
        public ProductDTO ToDto()
        {
            var dto = new ProductDTO();

            dto.AllowReviews = AllowReviews;
            dto.Bvin = Bvin;
            dto.CreationDateUtc = CreationDateUtc;

            dto.CustomProperties = new List<CustomPropertyDTO>();
            foreach (var prop in CustomProperties)
            {
                dto.CustomProperties.Add(prop.ToDto());
            }

            dto.Featured = Featured;
            dto.GiftWrapAllowed = GiftWrapAllowed;
            dto.GiftWrapPrice = GiftWrapPrice;
            dto.ImageFileMedium = ImageFileMedium;
            dto.ImageFileMediumAlternateText = ImageFileMediumAlternateText;
            dto.ImageFileSmall = ImageFileSmall;
            dto.ImageFileSmallAlternateText = ImageFileSmallAlternateText;
            dto.InventoryMode = (ProductInventoryModeDTO) (int) InventoryMode;
            dto.IsAvailableForSale = IsAvailableForSale;
            dto.Keywords = Keywords;
            dto.ListPrice = ListPrice;
            dto.LongDescription = LongDescription;
            dto.ManufacturerId = ManufacturerId;
            dto.MetaDescription = MetaDescription;
            dto.MetaKeywords = MetaKeywords;
            dto.MetaTitle = MetaTitle;
            dto.MinimumQty = MinimumQty;
            //dto.Options = Options;
            dto.PostContentColumnId = PostContentColumnId;
            dto.PreContentColumnId = PreContentColumnId;
            dto.ProductName = ProductName;
            dto.ProductTypeId = ProductTypeId;
            dto.ShippingDetails = ShippingDetails.ToDto();
            dto.ShippingMode = (ShippingModeDTO) (int) ShippingMode;
            dto.ShortDescription = ShortDescription;
            dto.SiteCost = SiteCost;
            dto.SitePrice = SitePrice;
            dto.SitePriceOverrideText = SitePriceOverrideText;
            dto.Sku = Sku;
            dto.Status = (ProductStatusDTO) (int) Status;
            dto.StoreId = StoreId;

            foreach (var tab in Tabs)
            {
                dto.Tabs.Add(tab.ToDto());
            }

            dto.TaxExempt = TaxExempt;
            dto.TaxSchedule = TaxSchedule;
            dto.UrlSlug = UrlSlug;
            //dto.Variants = Variants;
            dto.VendorId = VendorId;

            dto.ShippingCharge = (ShippingChargeTypeDTO) (int) ShippingCharge;

            dto.IsSearchable = IsSearchable;

            return dto;
        }

        #endregion

        #region IPurchasable Members

        [Obsolete]
        public PurchasableSnapshot AsPurchasable(OptionSelections selectionData, HotcakesApplication app)
        {
            return AsPurchasable(selectionData, app, true);
        }

        [Obsolete]
        public PurchasableSnapshot AsPurchasable(OptionSelections selectionData, HotcakesApplication app,
            bool calculateUserPrice)
        {
            throw new NotSupportedException();
        }

        public LineItem ConvertToLineItem(HotcakesApplication app, int quantity = 1,
            OptionSelections selectionData = null, decimal? userPrice = null)
        {
            var li = new LineItem();

            li.IsUserSuppliedPrice = userPrice.HasValue;
            li.BasePricePerItem = userPrice.HasValue ? userPrice.Value : SitePrice;
            li.ProductId = Bvin;
            li.ProductName = ProductName;
            li.ProductSku = Sku;
            li.Quantity = quantity;
            li.IsTaxExempt = !IsTaxable();
            li.SelectionData = selectionData ?? new OptionSelections();
            li.IsBundle = IsBundle;
            li.IsGiftCard = IsGiftCard;
            li.IsRecurring = IsRecurring;
            li.RecurringBilling.Interval = RecurringInterval;
            li.RecurringBilling.IntervalType = RecurringIntervalType;

            li.IsNonShipping = ShippingDetails.IsNonShipping;
            li.ShipFromAddress = ShippingDetails.ShippingSourceAddress;
            li.ShipFromMode = ShippingDetails.ShippingSource;
            li.ShipFromNotificationId = ShippingDetails.ShippingSourceId;
            li.ShipSeparately = ShippingDetails.ShipSeparately;
            li.ExtraShipCharge = ShippingDetails.ExtraShipFee;
            li.ShippingCharge = ShippingCharge;

            // Update shipping address
            switch (ShippingDetails.ShippingSource)
            {
                case ShippingMode.ShipFromManufacturer:
                    li.ShipFromNotificationId = ManufacturerId;
                    var m = app.ContactServices.Manufacturers.Find(ManufacturerId);
                    if (m != null)
                    {
                        li.ShipFromAddress = m.Address;
                    }
                    break;
                case ShippingMode.ShipFromSite:
                    li.ShipFromAddress = app.ContactServices.Addresses.FindStoreContactAddress();
                    break;
                case ShippingMode.ShipFromVendor:
                    li.ShipFromNotificationId = VendorId;
                    var v = app.ContactServices.Vendors.Find(VendorId);
                    if (v != null)
                    {
                        li.ShipFromAddress = v.Address;
                    }
                    break;
            }

            li.ProductShippingHeight = ShippingDetails.Height;
            li.ProductShippingLength = ShippingDetails.Length;
            li.ProductShippingWeight = ShippingDetails.Weight;
            li.ProductShippingWidth = ShippingDetails.Width;

            li.TaxSchedule = TaxSchedule;

            // See if we have adjustments based on options
            var basePriceToModify = li.BasePricePerItem;
            decimal priceAdjustments = 0;
            decimal weightAdjustments = 0;

            if (!IsBundle)
            {
                if (HasOptions())
                {
                    priceAdjustments = li.SelectionData.OptionSelectionList.GetPriceAdjustmentForSelections(Options);
                    weightAdjustments = li.SelectionData.OptionSelectionList.GetWeightAdjustmentForSelections(Options);
                }
            }
            else
            {
                if (app.CurrentStore.Settings.UseChildChoicesAdjustmentsForBundles)
                {
                    foreach (var bundledProductAdv in BundledProducts)
                    {
                        var bundledProduct = bundledProductAdv.BundledProduct;
                        if (bundledProduct == null || !bundledProduct.HasOptions())
                            continue;

                        var optionSelections = li.SelectionData.GetSelections(bundledProductAdv.Id);
                        var itemPriceAdjustment =
                            optionSelections.GetPriceAdjustmentForSelections(bundledProduct.Options);
                        var itemWeightAdjustment =
                            optionSelections.GetWeightAdjustmentForSelections(bundledProduct.Options);
                        priceAdjustments += itemPriceAdjustment*bundledProductAdv.Quantity;
                        weightAdjustments += itemWeightAdjustment*bundledProductAdv.Quantity;
                    }
                }
            }

            li.ProductShippingWeight += weightAdjustments;

            // See if we need to use a variant price as base
            if (!IsBundle)
            {
                if (HasVariants())
                {
                    var v = Variants.FindBySelectionData(li.SelectionData.OptionSelectionList, Options);
                    if (v != null)
                    {
                        li.VariantId = v.Bvin;
                        if (v.Price >= 0)
                            basePriceToModify = v.Price;
                        if (!string.IsNullOrWhiteSpace(v.Sku))
                            li.ProductSku = v.Sku;
                    }
                }
            }

            // See if we need to calculate user group discounts on base price
            var account = app.CurrentCustomer;
            if (account != null)
            {
                if (account.PricingGroupId != string.Empty)
                {
                    var pricingGroup = app.ContactServices.PriceGroups.Find(account.PricingGroupId);
                    if (pricingGroup != null)
                    {
                        var groupPrice = SitePrice;
                        groupPrice = pricingGroup.GetAdjustedPriceForThisGroup(SitePrice, ListPrice, SiteCost);
                        basePriceToModify = groupPrice;
                    }
                }
            }

            // Record option price adjustments on modified base price
            if (!li.IsUserSuppliedPrice)
            {
                li.BasePricePerItem = basePriceToModify + priceAdjustments;
            }

            // Build line item description
            if (!IsBundle)
            {
                if (HasOptions())
                {
                    li.ProductShortDescription = Options.CartDescription(li.SelectionData.OptionSelectionList);
                }
            }
            else
            {
                var description = new StringBuilder();
                foreach (var bundledProductAdv in BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;

                    description.Append("<div>");
                    description.Append(bundledProductAdv.Quantity + "X " + bundledProduct.ProductName);
                    description.Append("</div>");
                    if (bundledProduct.HasOptions())
                    {
                        var selections = li.SelectionData.GetSelections(bundledProductAdv.Id);
                        description.Append(bundledProduct.Options.CartDescription(selections));
                    }
                }

                li.ProductShortDescription = description.ToString();
            }
            return li;
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public string RenderTypeProperties(HotcakesApplication app)
        {
            return RenderTypeProperties(false, app.CurrentRequestContext);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public string RenderTypeProperties(bool forDropShipper, HotcakesApplication app)
        {
            return RenderTypeProperties(forDropShipper, app.CurrentRequestContext);
        }

        #endregion
    }
}