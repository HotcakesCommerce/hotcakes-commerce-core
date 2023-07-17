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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Single product information shown in the
    ///     product list on the product category page, show the featured products
    /// </summary>
    [Serializable]
    public class SingleProductViewModel
    {
        /// <summary>
        ///     set default values
        /// </summary>
        public SingleProductViewModel()
        {
            Item = null;
            UserPrice = null;
            ProductLink = string.Empty;
            SwatchDisplay = string.Empty;
        }

        /// <summary>
        ///     Set parameter values with provided product object
        /// </summary>
        /// <param name="p">Product information.</param>
        /// <param name="hccApp">An instance of the Hotcakes Application context.</param>
        public SingleProductViewModel(Product p, HotcakesApplication hccApp)
        {
            if (p == null) throw new ArgumentNullException("Product");
            if (hccApp == null) throw new ArgumentNullException("HotcakesApplication");

            UserPrice = hccApp.PriceProduct(p, hccApp.CurrentCustomer, null, hccApp.CurrentlyActiveSales);
            Item = p;

            ProductLink = UrlRewriter.BuildUrlForProduct(p);
            ProductAddToCartLink = UrlRewriter.BuildUrlForProductAddToCart(p);
            ImageUrls = new ProductImageUrls();
            ImageUrls.LoadProductImageUrls(hccApp, p);

            SwatchDisplay = ImageHelper.GenerateSwatchHtmlForProduct(p, hccApp);
        }

        /// <summary>
        ///     Mulitple images available for the product. More details about the product images can be found at
        ///     <see cref="ProductImageUrls" />
        /// </summary>
        public ProductImageUrls ImageUrls { get; set; }

        /// <summary>
        ///     Product information. Detailed information of the product and its properties can be found at
        ///     <see cref="Commerce.Catalog.Product" />
        /// </summary>
        public Product Item { get; set; }

        /// <summary>
        ///     If not an empty string, the URL allows you to add the product to the cart
        /// </summary>
        public string ProductAddToCartLink { get; set; }

        /// <summary>
        ///     Product detail page link
        /// </summary>
        public string ProductLink { get; set; }

        /// <summary>
        ///     This holds the html for the specified list of swatches available for specific product
        /// </summary>
        /// <remarks>
        ///     If the swatch doesn't match an available swatch file, it will not be included in the HTML. Also, all swatch
        ///     images must be PNG or GIF.
        /// </remarks>
        public string SwatchDisplay { get; set; }

        /// <summary>
        ///     User price to be shown to end user based on the chosen option
        ///     and show multiple information for the price.
        /// </summary>
        /// <remarks>This is the price that the product is normally sold to the public as.</remarks>
        public UserSpecificPrice UserPrice { get; set; }
    }
}