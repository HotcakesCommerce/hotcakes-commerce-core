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
using System.ComponentModel.DataAnnotations;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Social;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Product detail page
    /// </summary>
    [Serializable]
    public class ProductPageViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public ProductPageViewModel()
        {
            LocalProduct = new Product();
            IsAvailableForSale = true;
            StockMessage = string.Empty;
            Quantity = 1;
            Selections = new OptionSelections();
            PreRenderedTypeValues = string.Empty;
            RelatedItems = new List<SingleProductViewModel>();
            BundledItems = new List<BundledProductViewModel>();
            ValidationMessage = string.Empty;
            LineItemId = null;
            IsAvailableForWishList = false;
            SwatchHtml = string.Empty;
            AlternateImageUrls = new List<ProductImageUrls>();
        }

        /// <summary>
        ///     Flag indicates whether reviews can be added for the product or not.
        /// </summary>
        public bool AllowReviews { get; set; }

        /// <summary>
        ///     List of Product Image urls. This is like one product can have multiple images and each images comes with different
        ///     size and dimension as per the
        ///     business need to view the image more closely. More information can be found at <see cref="ProductImageUrls" />
        /// </summary>
        public List<ProductImageUrls> AlternateImageUrls { get; private set; }

        /// <summary>
        /// If true, the current request (end user) is authorized to edit the catalog.
        /// </summary>
        public bool AuthorizedToEditCatalog { get; set; }

        /// <summary>
        ///     If product is bundled product then this list shows the items comes under this bundled product. Individual bundled
        ///     product more
        ///     information can be find at <see cref="SingleProductViewModel" />
        /// </summary>
        public List<BundledProductViewModel> BundledItems { get; set; }

        /// <summary>
        ///     Amount of the gift card applied for the product
        /// </summary>
        [Required]
        public decimal GiftCardAmount { get; set; }

        /// <summary>
        ///     Message to be shown to the gift card receiver
        /// </summary>
        public string GiftCardMessage { get; set; }

        /// <summary>
        ///     List of amounts for the gift cards
        /// </summary>
        public List<decimal> GiftCardPredefinedAmounts { get; set; }

        /// <summary>
        ///     Gift card receiver email
        /// </summary>
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string GiftCardRecEmail { get; set; }

        /// <summary>
        ///     Name of the gift card receiver
        /// </summary>
        public string GiftCardRecName { get; set; }

        /// <summary>
        ///     Image information for the product. More information of this can be found at <see cref="ProductImageUrls" />
        /// </summary>
        public ProductImageUrls ImageUrls { get; set; }

        /// <summary>
        ///     Default quantity set for the product
        /// </summary>
        public string InitialQuantity
        {
            get { return Quantity.ToString(); }
        }

        /// <summary>
        ///     Flag to show product is available for sale. This can be determined based on the
        ///     rule set for the product for availability criteria like based on stock, always available etc.
        /// </summary>
        public bool IsAvailableForSale { get; set; }

        /// <summary>
        ///     Flag indicates whether product can be added to wishlist or not. Based on that Add to wish list option available on
        ///     screen.
        /// </summary>
        public bool IsAvailableForWishList { get; set; }

        /// <summary>
        ///     Flag to indicate whether current product is gift card or regular product
        /// </summary>
        public bool IsGiftCard { get; set; }

        /// <summary>
        ///     If product is displayed by link from cart then this shows the unique identifier for the product for that order.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public long? LineItemId { get; set; }

        /// <summary>
        ///     Product information. More detail found for this at <see cref="Product" />
        /// </summary>
        public Product LocalProduct { get; set; }

        /// <summary>
        /// The name of the manufacturer of the product.  If not populated, the product does not have a manufacturer.  
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Additional images pre-defined HTML block to render in a product details view.
        /// </summary>
        public string PreRenderedImages
        {
            get
            {
                var sb = new StringBuilder();

                if (AlternateImageUrls.Count > 0)
                {
                    sb.Append("<div id=\"hcAdditionalImages\">");
                    foreach (var img in AlternateImageUrls)
                    {
                        sb.Append("<a href=\"" + img.MediumlUrl + "\" alt=\"" + img.MediumlUrl + "\" >");
                        sb.Append("<img src=\"");
                        sb.Append(img.TinyUrl);
                        sb.Append("\" border=\"0\" alt=\"" + img.SmallAltText + "\" />");
                        sb.Append("</a>");
                    }
                    sb.Append("</div>");
                }

                return sb.ToString();
            }
        }

        /// <summary>
        ///     Product can be mapped to product type and product type have one or more properties. This property returns the html
        ///     generated for the
        ///     different properties and values available for the product
        /// </summary>
        public string PreRenderedTypeValues { get; set; }

        /// <summary>
        ///     Product price information. There are different prices available like site price, cost price and labels and text
        ///     needs to be shown for price.
        ///     More detailed option of this can be found at  <see cref="ProductPrices" />
        /// </summary>
        public ProductPrices Prices { get; set; }

        /// <summary>
        /// This URL can be used to directly navigate to the product performance view. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string ProductAnalyticsUrl { get; set; }

        /// <summary>
        /// This URL can be used to directly navigate to the product editing view. If empty, the current end-user is not authorized to see and use this view.
        /// </summary>
        public string ProductEditUrl { get; set; }

        /// <summary>
        ///     Quantity entered by user
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int Quantity { get; set; }

        /// <summary>
        ///     Related list of the product. Individual related product more information can be find at
        ///     <see cref="SingleProductViewModel" />
        /// </summary>
        public List<SingleProductViewModel> RelatedItems { get; set; }

        /// <summary>
        ///     Options if any available for the product to purchase it. More details of options can be found at
        ///     <see cref="OptionSelections" />
        /// </summary>
        public OptionSelections Selections { get; set; }

        /// <summary>
        ///     Social controls for the given product in json format.
        /// </summary>
        [Obsolete("Removing in 03.05.00 or later. Previously was used for Evoq Social integration.")]
        public ISocialItem SocialItem { get; set; }

        /// <summary>
        ///     If product is out of stock then specific customized message can be shown.
        /// </summary>
        public string StockMessage { get; set; }

        /// <summary>
        /// The store administration URL for the dashboard. 
        /// </summary>
        public string StoreAdminUrl { get; set; }

        /// <summary>
        ///     This holds the html for the specified list of swatches available for specific product
        /// </summary>
        /// <remarks>
        ///     If the swatch doesn't match an available swatch file, it will not be included in the HTML. Also, all swatch
        ///     images must be PNG or GIF.
        /// </remarks>
        public string SwatchHtml { get; set; }

        /// <summary>
        /// The price that a customer chooses/enters to page (primarily used for donation use cases).  
        /// </summary>
        public decimal UserSuppliedPrice { get; set; }

        /// <summary>
        ///     Validation message for the form
        /// </summary>
        public string ValidationMessage { get; set; }

        /// <summary>
        /// The name of the vendor of the product.  If not populated, the product does not have a vendor.  
        /// </summary>
        public string VendorName { get; set; }

        public bool UpchargeAllowed { get; set; }
    }
}