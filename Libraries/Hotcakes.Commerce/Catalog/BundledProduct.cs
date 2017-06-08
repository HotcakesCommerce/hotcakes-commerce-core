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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This holds the information about the
    ///     bundled product in system.
    /// </summary>
    /// <remarks>This class is mapped individual row in "hcc_BundledProducts" table in database.</remarks>
    [Serializable]
    public class BundledProduct
    {
        /// <summary>
        ///     Set default value.
        /// </summary>
        public BundledProduct()
        {
            Id = 0;
            ProductId = string.Empty;
            Quantity = 0;
            BundledProductId = string.Empty;
            SortOrder = 0;
        }

        /// <summary>
        ///     Set the values from the given bundled product instance.
        /// </summary>
        /// <param name="bundledProduct">Bundled product instance.</param>
        public BundledProduct(BundledProduct bundledProduct)
        {
            Id = bundledProduct.Id;
            ProductId = bundledProduct.ProductId;
            Quantity = bundledProduct.Quantity;
            BundledProductId = bundledProduct.BundledProductId;
            SortOrder = bundledProduct.SortOrder;
        }

        /// <summary>
        ///     Unique identifier of the Bundled Product
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Product unique identifier. Mapped to the product's Bvin field.
        /// </summary>
        /// <remarks>This represents the product part of the product bundle.</remarks>
        public string ProductId { get; set; }

        /// <summary>
        ///     Product unique identifier. Mapped to the product's Bvin field.
        /// </summary>
        public string BundledProductId { get; set; }

        /// <summary>
        ///     Quantity of the product in bundle.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     Sort order of the product in bundle.
        /// </summary>
        public int SortOrder { get; set; }
    }
}