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
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Represents the cart (pending order) shown on the cart page before checkout.
    /// </summary>
    [Serializable]
    public class CartViewModel
    {
        /// <summary>
        ///     Set default values for each property.
        /// </summary>
        public CartViewModel()
        {
            CurrentOrder = new Order();
            KeepShoppingUrl = string.Empty;
            DisplayTitle = "Shopping Cart";
            DisplaySubTitle = string.Empty;
            ItemListTitle = string.Empty;
            CartEmpty = false;
            LineItems = new List<CartLineItemViewModel>();
            PayPalExpressAvailable = false;
            Rates = new List<ShippingRateDisplay>();
           
        }

        /// <summary>
        ///     The order object that is bound to this cart. This may be a temporary order entry
        ///     whenever new items are added to the cart.
        /// </summary>
        public Order CurrentOrder { get; set; }

        /// <summary>
        ///     The URL associated to the KeepShopping button in the cart views. The system has
        ///     a URL rewrite module configured, so it may be a custom URL mapped to different
        ///     actions.
        /// </summary>
        public string KeepShoppingUrl { get; set; }

        // TODO: Depreciate this property
        /// <summary>
        ///     User friendly name shown on the page for the cart.
        /// </summary>
        /// <remarks>
        ///     This property is currently not used anywhere in the system and may be depreciated.
        /// </remarks>
        public string DisplayTitle { get; set; }

        // TODO: Depreciate this property
        /// <summary>
        ///     If something needs to be shown as Sub tile below the Shopping cart then it can
        ///     be set through this parameter.
        /// </summary>
        /// <remarks>
        ///     This property is currently not used anywhere in the system and may be depreciated.
        /// </remarks>
        public string DisplaySubTitle { get; set; }

        // TODO: Depreciate this property
        /// <summary>
        ///     An optional title property that can be used to display text in your views.
        /// </summary>
        /// <remarks>
        ///     This is currently only used in the Minicart but is never populated. This property
        ///     will likely become depreciated.
        /// </remarks>
        public string ItemListTitle { get; set; }

        /// <summary>
        ///     A flag that indicates whether cart is empty or not. This flag can be used by the
        ///     cart controller when there is no item on cart or all cart items are free items.
        /// </summary>
        public bool CartEmpty { get; set; }

        /// <summary>
        ///     This property shows the collection of the line items in the cart. Each item in
        ///     cart is represented by <see cref="CartLineItemViewModel" />
        /// </summary>
        public List<CartLineItemViewModel> LineItems { get; set; }

        /// <summary>
        ///     This is a quick check to see if PayPal Express is enabled or not. Whenever a
        ///     cart is initialized, this will be true if you have PayPal Express enabled in the
        ///     store administration area.
        /// </summary>
        public bool PayPalExpressAvailable { get; set; }

        /// <summary>
        ///     List of shipping rates needs to be shown based on the user address and other
        ///     shipping method criteria on products.
        /// </summary>
        public List<ShippingRateDisplay> Rates { get; set; }

        public bool CoverCreditCardFees { get; set; }
    }
}