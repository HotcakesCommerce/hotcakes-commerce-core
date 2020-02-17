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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Individual gift card information shown on
    ///     checkout page and order summary page after checkout done.
    /// </summary>
    [Serializable]
    public class GiftCardItem
    {
        /// <summary>
        ///     Unique identifier of the card
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        ///     Balance available in card
        /// </summary>
        public string Balance { get; set; }

        /// <summary>
        ///     charge for the specific order
        /// </summary>
        public string Charge { get; set; }
    }

    /// <summary>
    ///     Giftcard information for specific order
    /// </summary>
    [Serializable]
    public class GiftCardViewModel
    {
        /// <summary>
        ///     Set default value
        /// </summary>
        public GiftCardViewModel()
        {
            Cards = new List<GiftCardItem>();
        }

        /// <summary>
        ///     Boolean flag indicates whether needs to show the
        ///     Giftcards or not
        /// </summary>
        public bool ShowGiftCards { get; set; }

        /// <summary>
        ///     Available giftcards for the order
        /// </summary>
        public List<GiftCardItem> Cards { get; set; }

        /// <summary>
        ///     Different debug information which added to this
        ///     during the use of specific gift card on checkout process
        /// </summary>
        public string Summary { get; set; }
    }
}