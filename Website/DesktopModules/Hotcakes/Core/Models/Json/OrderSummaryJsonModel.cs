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

namespace Hotcakes.Modules.Core.Models.Json
{
/// <summary>
    /// Represents the OrderSummaryJsonModel storefront model.
    /// </summary>
    [Serializable]
    public class OrderSummaryJsonModel
    {
        /// <summary>
        /// Gets or sets the dynamic orderitems value used by this storefront model.
        /// </summary>
        public dynamic orderitems;
        /// <summary>
        /// Gets or sets the totalsastable value used by this storefront model.
        /// </summary>
        public string totalsastable = string.Empty;
        /// <summary>
        /// Initializes a new instance of the OrderSummaryJsonModel type.
        /// </summary>

        public OrderSummaryJsonModel()
        {
            GiftCards = new GiftCardViewModel();
        }

        /// <summary>
        /// Gets or sets the LabelRewardsUse value used by this storefront model.
        /// </summary>
        public string LabelRewardsUse { get; set; }
        /// <summary>
        /// Gets or sets the PaymentViewModel value used by this storefront model.
        /// </summary>
        public CheckoutPaymentViewModel PaymentViewModel { get; set; }
        /// <summary>
        /// Gets or sets the ShippingValidationResult value used by this storefront model.
        /// </summary>
        public AddressValidationJsonModel ShippingValidationResult { get; set; }
        /// <summary>
        /// Gets or sets the BillingValidationResult value used by this storefront model.
        /// </summary>
        public AddressValidationJsonModel BillingValidationResult { get; set; }
        /// <summary>
        /// Gets or sets the GiftCards value used by this storefront model.
        /// </summary>
        public GiftCardViewModel GiftCards { get; set; }
    }
}
