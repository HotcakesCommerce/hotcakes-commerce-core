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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     This model used inside products detail page to show the information of the bundled product
    /// </summary>
    [Serializable]
    public class BundledProductViewModel
    {
        /// <summary>
        ///     Set default values using parameterized constructor
        ///     <remarks>
        ///         This generally called from the products controller to set proper urls and product link for the bundled product
        ///     </remarks>
        /// </summary>
        /// <param name="bp">Parameter passed for existing  BundledProduct object</param>
        /// <param name="hccApp">An instance of the Hotcakes Application context.</param>
        public BundledProductViewModel(BundledProductAdv bp, HotcakesApplication hccApp)
        {
            BundledProductAdv = bp;
            Item = bp.BundledProduct;

            ProductLink = UrlRewriter.BuildUrlForProduct(BundledProductAdv.BundledProduct);
            ImageUrls = new ProductImageUrls();
            ImageUrls.LoadProductImageUrls(hccApp, BundledProductAdv.BundledProduct);

            SwatchDisplay = ImageHelper.GenerateSwatchHtmlForProduct(BundledProductAdv.BundledProduct, hccApp);
        }

        /// <summary>
        ///     This object holds the product details to be shown to end user such as the number of products in bundle.
        /// </summary>
        public BundledProductAdv BundledProductAdv { get; set; }

        /// <summary>
        ///     A bundled product also treated as one single product so this object holds all the information entered when creating
        ///     the bundled product.
        /// </summary>
        public Product Item { get; set; }

        /// <summary>
        ///     Image URL's for the bundled product which can be shown on product detail page.
        /// </summary>
        public ProductImageUrls ImageUrls { get; set; }

        /// <summary>
        ///     URL of the product as its configured by administrator when creating product. Here we can find the specific user
        ///     friendly URL for this product.
        /// </summary>
        public string ProductLink { get; set; }

        /// <summary>
        ///     Contains the HTML string which is shown to the end user.
        /// </summary>
        public string SwatchDisplay { get; set; }
    }
}