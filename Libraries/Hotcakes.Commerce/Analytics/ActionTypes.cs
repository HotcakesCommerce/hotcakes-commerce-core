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

namespace Hotcakes.Commerce.Analytics
{
    /// <summary>
    /// Identifies an analytics event recorded by Hotcakes Commerce.
    /// </summary>
    /// <remarks>
    /// The member names are serialized to the analytics event store and are therefore part of the
    /// persistence contract. Existing spellings, including <c>GoToChekout</c> and
    /// <c>ChekoutCompleted</c>, must remain unchanged for previously stored events to deserialize.
    /// </remarks>
    /// <seealso cref="AnalyticsEvent"/>
    public enum ActionTypes
    {
        /// <summary>Indicates that a product was viewed.</summary>
        ProductViewed,

        /// <summary>Indicates that a category was viewed.</summary>
        CategoryViewed,

        /// <summary>Indicates that a product was added to the shopping cart.</summary>
        ProductAddedToCart,

        /// <summary>Indicates that the customer navigated to the shopping cart.</summary>
        GoToCart,

        /// <summary>Indicates that the customer navigated to checkout.</summary>
        GoToChekout,

        /// <summary>Indicates that checkout was completed.</summary>
        ChekoutCompleted,

        /// <summary>Indicates that an order was paid.</summary>
        OrderPaid,

        /// <summary>Indicates that a product price changed.</summary>
        ProductPriceChanged,

        /// <summary>Indicates that product descriptive content changed.</summary>
        ProductCopyChanged,

        /// <summary>Indicates that product images changed.</summary>
        ProductImagesChanged,

        /// <summary>Indicates that a product was added to a bundle.</summary>
        ProductWasBundled,

        /// <summary>Indicates that the products assigned to a category changed.</summary>
        CategoryProductsUpdated,

        /// <summary>Indicates that category descriptive content changed.</summary>
        CategoryCopyChanged,

        /// <summary>Indicates that category images changed.</summary>
        CategoryImagesChanged
    }
}
