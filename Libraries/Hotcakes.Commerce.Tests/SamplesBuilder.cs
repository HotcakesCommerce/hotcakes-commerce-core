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

using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests
{
    public static class SamplesBuilder
    {
        public static void AddFiveSimpleProducts(HotcakesApplication app)
        {
            var prod1 = new Product {Sku = "001", ProductName = "Product 1", SitePrice = 100m};
            var prod2 = new Product {Sku = "002", ProductName = "Product 2", SitePrice = 200m};
            var prod3 = new Product {Sku = "003", ProductName = "Product 3", SitePrice = 300m};
            var prod4 = new Product {Sku = "004", ProductName = "Product 4", SitePrice = 400m};
            var prod5 = new Product {Sku = "005", ProductName = "Product 5", SitePrice = 499.99m};

            app.CatalogServices.Products.Create(prod1);
            app.CatalogServices.Products.Create(prod2);
            app.CatalogServices.Products.Create(prod3);
            app.CatalogServices.Products.Create(prod4);
            app.CatalogServices.Products.Create(prod5);
        }
    }
}