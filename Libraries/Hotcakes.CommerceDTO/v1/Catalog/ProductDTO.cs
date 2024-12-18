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
using System.Xml.Serialization;
using Hotcakes.CommerceDTO.v1.Shipping;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary class used for all products in the REST API
    /// </summary>
    [Serializable]
    [XmlInclude(typeof (CustomPropertyDTO))]
    [XmlInclude(typeof (ShippableItemDTO))]
    [XmlInclude(typeof (ShippingModeDTO))]
    [XmlInclude(typeof (ProductStatusDTO))]
    [XmlInclude(typeof (ProductInventoryModeDTO))]
    [XmlInclude(typeof (ProductDescriptionTabDTO))]
    public class ProductDTO
    {
        public ProductDTO()
        {
            Bvin = string.Empty;
            Sku = string.Empty;
            ProductName = string.Empty;
            ProductTypeId = string.Empty;
            CustomProperties = new List<CustomPropertyDTO>();
            ListPrice = 0m;
            SitePrice = 0m;
            SitePriceOverrideText = string.Empty;
            SiteCost = 0m;
            MetaKeywords = string.Empty;
            MetaDescription = string.Empty;
            MetaTitle = string.Empty;
            TaxExempt = false;
            TaxSchedule = -1;
            Status = ProductStatusDTO.Active;
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
            PreContentColumnId = string.Empty;
            PostContentColumnId = string.Empty;
            UrlSlug = string.Empty;
            GiftWrapPrice = 0m;
            ShippingDetails = new ShippableItemDTO();
            Featured = false;
            AllowReviews = false;
            Tabs = new List<ProductDescriptionTabDTO>();
            StoreId = 0;
            IsAvailableForSale = true;
            InventoryMode = ProductInventoryModeDTO.AlwayInStock;
            IsSearchable = true;
            AllowUpcharge = false;
            UpchargeAmount  = 3m;
            UpchargeUnit = ((int)UpchargeAmountTypesDTO.Percent).ToString();
        }

        /// <summary>
        ///     The unique ID or primary key of the product.
        /// </summary>
        /// <remarks>This property should always be used instead of Id.</remarks>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     A SKU is the "stock keeping unit" and is often used as a primary or unique key to identify the product across
        ///     multiple mediums and systems.
        /// </summary>
        [DataMember]
        public string Sku { get; set; }

        /// <summary>
        ///     The product name is the title of the product as a customer will see it.
        /// </summary>
        /// <remarks>This value should be the localized version of the product name.</remarks>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        ///     A unique ID matching a product type that this product should be treated as.
        /// </summary>
        [DataMember]
        public string ProductTypeId { get; set; }

        /// <summary>
        ///     A collection of custom product meta data that can be used for any API or integration-based purposes.
        /// </summary>
        [DataMember]
        public List<CustomPropertyDTO> CustomProperties { get; set; }

        /// <summary>
        ///     This is the price that the product is normally sold to the public as.
        /// </summary>
        [DataMember]
        public decimal ListPrice { get; set; }

        /// <summary>
        ///     The site price reflects the actual price that a customer will pay, regardless to the list price (MSRP).
        /// </summary>
        /// <remarks>This value will not reflect changes made by sales, price groups, or offers.</remarks>
        [DataMember]
        public decimal SitePrice { get; set; }

        /// <summary>
        ///     This value is set in the product editing view and allows you to supply a user-friendly message about the prive to
        ///     customers.
        /// </summary>
        /// <remarks>This value is not currently used in the application.</remarks>
        [DataMember]
        public string SitePriceOverrideText { get; set; }

        /// <summary>
        ///     This property should reflect the amount that this product cost to acquire.
        /// </summary>
        [DataMember]
        public decimal SiteCost { get; set; }

        /// <summary>
        ///     The meta keywords are used for both indexing the product in the local search engine and for injecting into the meta
        ///     data of the web page.
        /// </summary>
        [DataMember]
        public string MetaKeywords { get; set; }

        /// <summary>
        ///     The meta description is used for both indexing the product in the local search engine and for injecting into the
        ///     meta data of the web page.
        /// </summary>
        [DataMember]
        public string MetaDescription { get; set; }

        /// <summary>
        ///     The meta title is used for both indexing the product in the local search engine and for injecting into the meta
        ///     data of the web page.
        /// </summary>
        [DataMember]
        public string MetaTitle { get; set; }

        /// <summary>
        ///     If true, tax rules and calculation will not be applied to this product.
        /// </summary>
        [DataMember]
        public bool TaxExempt { get; set; }

        /// <summary>
        ///     When specified, this allows the current product to be applied against the specified tax schedule.
        /// </summary>
        [DataMember]
        public long TaxSchedule { get; set; }

        /// <summary>
        ///     This represents a collection of properties related to shipping the current product.
        /// </summary>
        [DataMember]
        public ShippableItemDTO ShippingDetails { get; set; }

        /// <summary>
        ///     This property simply returns the ShippingMode from the local ShippingDetails property.
        /// </summary>
        [DataMember]
        public ShippingModeDTO ShippingMode { get; set; }

        /// <summary>
        ///     This property maps to the "active" checkbox in the user interface and determines if the product will be available
        ///     to customers or not.
        /// </summary>
        [DataMember]
        public ProductStatusDTO Status { get; set; }

        /// <summary>
        ///     The file name of the small version of the primary product image.
        /// </summary>
        [DataMember]
        public string ImageFileSmall { get; set; }

        /// <summary>
        ///     The localized text saved specifically to be used for the small version of the primary product image.
        /// </summary>
        [DataMember]
        public string ImageFileSmallAlternateText { get; set; }

        /// <summary>
        ///     The file name of the medium version of the primary product image.
        /// </summary>
        [DataMember]
        public string ImageFileMedium { get; set; }

        /// <summary>
        ///     The localized text saved specifically to be used for the medium version of the primary product image.
        /// </summary>
        [DataMember]
        public string ImageFileMediumAlternateText { get; set; }

        /// <summary>
        ///     The creation date is used for auditing purposes to know when the product was first created.
        /// </summary>
        [DataMember]
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        ///     When greater than zero, this value will be the minimum allowable quantity for the product before the cart allows it
        ///     to be added.
        /// </summary>
        [DataMember]
        public int MinimumQty { get; set; }

        /// <summary>
        ///     A shorter version of the LongDescription property.
        /// </summary>
        /// <remarks>This property is currently not being used in the application.</remarks>
        [DataMember]
        public string ShortDescription { get; set; }

        /// <summary>
        ///     This is the product description that is shown to customers.
        /// </summary>
        /// <remarks>This value contains HTML.</remarks>
        [DataMember]
        public string LongDescription { get; set; }

        /// <summary>
        ///     This property defines a manufacturer that this product is created by.
        /// </summary>
        [DataMember]
        public string ManufacturerId { get; set; }

        /// <summary>
        ///     This property defines a vendor that this product is distributed by.
        /// </summary>
        [DataMember]
        public string VendorId { get; set; }

        /// <summary>
        ///     If true, the current product can be gift wrapped prior to shipping for delivery.
        /// </summary>
        /// <remarks>This property is currently not being used in the application.</remarks>
        [DataMember]
        public bool GiftWrapAllowed { get; set; }

        /// <summary>
        ///     If gift wrap is enabled for this product and selected, this price will be added to the order total.
        /// </summary>
        /// <remarks>This property is currently not being used in the application.</remarks>
        [DataMember]
        public decimal GiftWrapPrice { get; set; }

        /// <summary>
        ///     This property contains search keywords that will be used by the native search engine, but not exposed to public
        ///     search engines.
        /// </summary>
        /// <remarks>This is a great place to put keywords that might relate to competitive product/service names.</remarks>
        [DataMember]
        public string Keywords { get; set; }

        /// <summary>
        ///     If specified, the content block matching the given ID will be presented as a header element above the product
        ///     listing.
        /// </summary>
        [DataMember]
        public string PreContentColumnId { get; set; }

        /// <summary>
        ///     If specified, the content block matching the given ID will be presented as a footer element below the product
        ///     listing.
        /// </summary>
        [DataMember]
        public string PostContentColumnId { get; set; }

        /// <summary>
        ///     This unique value will be the final set of characters after the last slash in the page URL.
        /// </summary>
        [DataMember]
        public string UrlSlug { get; set; }

        /// <summary>
        ///     The value of this property defines how the product will be treated by the application once it is out of stock.
        /// </summary>
        [DataMember]
        public ProductInventoryModeDTO InventoryMode { get; set; }

        /// <summary>
        ///     A dymanically propulated property to help the views know if this product is available, based upon status and
        ///     inventory mode/levels.
        /// </summary>
        [DataMember]
        public bool IsAvailableForSale { get; set; }


        /// <summary>
        ///     When set to true, this property tells the application to allow this product to be listed in the featured product
        ///     view.
        /// </summary>
        [DataMember]
        public bool Featured { get; set; }

        /// <summary>
        ///     If set to true, this product will allow reviews to be saved by customers.
        /// </summary>
        [DataMember]
        public bool? AllowReviews { get; set; }

        /// <summary>
        ///     A collection of info tabs that contain additional product details and are generally found below the product
        ///     description.
        /// </summary>
        [DataMember]
        public List<ProductDescriptionTabDTO> Tabs { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        [DataMember]
        public bool IsSearchable { get; set; }

        public ShippingChargeTypeDTO ShippingCharge { get; set; }

        [DataMember]
        public bool AllowUpcharge  { get; set; }

        [DataMember]
        public decimal UpchargeAmount  { get; set; }

        [DataMember]
        public string UpchargeUnit  { get; set; }
        
    }
}