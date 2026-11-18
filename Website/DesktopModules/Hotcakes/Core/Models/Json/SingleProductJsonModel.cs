#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

namespace Hotcakes.Modules.Core.Models.Json
{
/// <summary>
    /// Represents the SingleProductJsonModel storefront model.
    /// </summary>
    [Serializable]
    public class SingleProductJsonModel
    {
        /// <summary>
        /// Performs the SingleProductJsonModel operation for this model.
        /// </summary>
        /// <param name="product">The value used by the operation.</param>
        /// <param name="app">The value used by the operation.</param>
        public SingleProductJsonModel(Product product, HotcakesApplication app)
        {
            ProductLink = UrlRewriter.BuildUrlForProduct(product);
            ProductAddToCartLink = UrlRewriter.BuildUrlForProductAddToCart(product);
            ProductName = product.ProductName;
            ImageSmallUrl = DiskStorage.ProductImageUrlSmall(app, product.Bvin, product.ImageFileSmall,
                app.IsCurrentRequestSecure());
            ImageSmallAltText = product.ImageFileSmallAlternateText;
            ProductSku = product.Sku;
            Bvin = product.Bvin;

            var up = app.PriceProduct(product, app.CurrentCustomer, null, app.CurrentlyActiveSales);
            UserPrice = up.DisplayPrice(true,false);
        }

        /// <summary>
        /// Gets or sets the ProductLink value used by this storefront model.
        /// </summary>
        public string ProductLink { get; set; }
        /// <summary>
        /// Gets or sets the ProductAddToCartLink value used by this storefront model.
        /// </summary>
        public string ProductAddToCartLink { get; set; }
        /// <summary>
        /// Gets or sets the ProductName value used by this storefront model.
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Gets or sets the ImageSmallUrl value used by this storefront model.
        /// </summary>
        public string ImageSmallUrl { get; set; }
        /// <summary>
        /// Gets or sets the ImageSmallAltText value used by this storefront model.
        /// </summary>
        public string ImageSmallAltText { get; set; }
        /// <summary>
        /// Gets or sets the UserPrice value used by this storefront model.
        /// </summary>
        public string UserPrice { get; set; }
        /// <summary>
        /// Gets or sets the ProductSku value used by this storefront model.
        /// </summary>
        public string ProductSku { get; set; }
        /// <summary>
        /// Gets or sets the Bvin value used by this storefront model.
        /// </summary>
        public string Bvin { get; set; }
    }
}
