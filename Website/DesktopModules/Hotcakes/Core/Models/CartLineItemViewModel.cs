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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     This class represent the individual item in the Shopping cart
    /// </summary>
    [Serializable]
    public class CartLineItemViewModel
    {
        /// <summary>
        ///     Set the default values
        /// </summary>
        public CartLineItemViewModel()
        {
            Item = new LineItem();
            Product = new Product();
            ShowImage = false;
            ImageUrl = string.Empty;
            LinkUrl = "#";
            HasDiscounts = false;
        }

        /// <summary>
        ///     Different properties and information related to each line item (aka. product) has
        ///     different methods to show data appropriately to end user based on those properties.
        /// </summary>
        public LineItem Item { get; set; }

        /// <summary>
        ///     Each line item is nearly a copy of the original product in your catalog at the time
        ///     it was added to the cart. This property represents the original product.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        ///     Controls whether the image should be shown on the cart for the product or not.
        /// </summary>
        public bool ShowImage { get; set; }

        /// <summary>
        ///     Image URL of the product if needs to be shown on cart
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///     Link URL for the product in the cart, which can be used to direct the customer
        ///     back to product detail page.
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        ///     Flag which indicates if any discounts are applied to the line item or not.  During
        ///     order calculation, this flag has been set based on the different criteria and
        ///     configuration set by the merchant.
        /// </summary>
        public bool HasDiscounts { get; set; }
    }
}