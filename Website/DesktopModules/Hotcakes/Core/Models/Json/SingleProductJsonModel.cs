#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    [Serializable]
    public class SingleProductJsonModel
    {
        public SingleProductJsonModel(Product product, HotcakesApplication app)
        {
            ProductLink = UrlRewriter.BuildUrlForProduct(product);
            ProductName = product.ProductName;
            ImageSmallUrl = DiskStorage.ProductImageUrlSmall(app, product.Bvin, product.ImageFileSmall,
                app.IsCurrentRequestSecure());
            ImageSmallAltText = product.ImageFileSmallAlternateText;
            ProductSku = product.Sku;
            Bvin = product.Bvin;

            var up = app.PriceProduct(product, app.CurrentCustomer, null, app.CurrentlyActiveSales);
            UserPrice = up.DisplayPrice(true,false);
        }

        public string ProductLink { get; set; }
        public string ProductName { get; set; }
        public string ImageSmallUrl { get; set; }
        public string ImageSmallAltText { get; set; }
        public string UserPrice { get; set; }
        public string ProductSku { get; set; }
        public string Bvin { get; set; }
    }
}