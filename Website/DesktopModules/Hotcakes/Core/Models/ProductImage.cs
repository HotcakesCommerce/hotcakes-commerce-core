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
using Hotcakes.Commerce.Storage;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Product images information.
    ///     This can be used to show the multiple images of the products on product detail page.
    /// </summary>
    [Serializable]
    public class ProductImageUrls
    {
        /// <summary>
        ///     Original image url without resize.
        /// </summary>
        public string OriginalUrl { get; set; }

        /// <summary>
        ///     Medium size image url which generated from original image
        /// </summary>
        public string MediumlUrl { get; set; }

        /// <summary>
        ///     Small size image url which is generated from original image
        /// </summary>
        public string SmallUrl { get; set; }

        /// <summary>
        ///     Very small size image url which is generated from original image.
        ///     This is shown as small icons and hover on it show the bigger image.
        /// </summary>
        public string TinyUrl { get; set; }

        /// <summary>
        ///     Alter text for image if image not available.
        /// </summary>
        public string MediumlAltText { get; set; }

        /// <summary>
        ///     Alternate text for the image if image not available
        /// </summary>
        public string SmallAltText { get; set; }


        /// <summary>
        ///     Load the Image information for the product
        /// </summary>
        /// <param name="hccApp">An instance of the Hotcakes Application context.</param>
        /// <param name="product">Product for which its required to have the image details.</param>
        public void LoadProductImageUrls(HotcakesApplication hccApp, Product product)
        {
            OriginalUrl = DiskStorage.ProductImageUrlOriginal(hccApp, product.Bvin, product.ImageFileSmall,
                hccApp.IsCurrentRequestSecure());
            MediumlUrl = DiskStorage.ProductImageUrlMedium(hccApp, product.Bvin, product.ImageFileMedium,
                hccApp.IsCurrentRequestSecure());
            SmallUrl = DiskStorage.ProductImageUrlSmall(hccApp, product.Bvin, product.ImageFileSmall,
                hccApp.IsCurrentRequestSecure());
            MediumlAltText = product.ImageFileMediumAlternateText;
            SmallAltText = product.ImageFileSmallAlternateText;
            TinyUrl = SmallUrl;

            if (string.IsNullOrWhiteSpace(MediumlAltText))
            {
                MediumlAltText = SmallAltText;
            }
        }

        /// <summary>
        ///     Load additional image information for the given product
        /// </summary>
        /// <param name="hccApp">An instance of the Hotcakes Application context.</param>
        /// <param name="img">Product for which its required to have the image details.</param>
        public void LoadAlternateImageUrls(HotcakesApplication hccApp, ProductImage img)
        {
            MediumlUrl = DiskStorage.ProductAdditionalImageUrlMedium(hccApp, img.ProductId, img.Bvin, img.FileName,
                false);
            OriginalUrl = DiskStorage.ProductAdditionalImageUrlOriginal(hccApp, img.ProductId, img.Bvin, img.FileName,
                false);
            SmallUrl = DiskStorage.ProductAdditionalImageUrlSmall(hccApp, img.ProductId, img.Bvin, img.FileName, false);
            TinyUrl = DiskStorage.ProductAdditionalImageUrlTiny(hccApp, img.ProductId, img.Bvin, img.FileName, false);

            MediumlAltText = img.AlternateText;
            SmallAltText = img.AlternateText;
        }
    }
}