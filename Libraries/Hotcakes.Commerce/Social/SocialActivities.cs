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

namespace Hotcakes.Commerce.Social
{
    /// <summary>
    /// Defines the activity identifiers and display-message format strings used by social integrations.
    /// </summary>
    /// <remarks>
    /// The identifier values are compatibility-sensitive values exchanged with the social platform. The format
    /// constants must be used with the placeholders in their documented order when creating activity messages.
    /// </remarks>
    public class SocialActivities
    {
        /// <summary>
        /// Gets the activity identifier for a completed purchase: <c>HCPurchaseCompleted</c>.
        /// </summary>
        public const string CompletePurchase = "HCPurchaseCompleted";

        /// <summary>
        /// Gets the activity identifier for viewing a product: <c>HCProductViewed</c>.
        /// </summary>
        public const string ViewProduct = "HCProductViewed";

        /// <summary>
        /// Gets the activity identifier for reviewing a product: <c>HCProductReviewed</c>.
        /// </summary>
        public const string ReviewProduct = "HCProductReviewed";

        /// <summary>
        /// Gets the activity identifier for viewing a category: <c>HCCategoryViewed</c>.
        /// </summary>
        public const string ViewCategory = "HCCategoryViewed";

        /// <summary>
        /// Gets the activity identifier for adding a product to a wishlist: <c>HCProductAddedtoWishlist</c>.
        /// </summary>
        public const string AddProductToWishlist = "HCProductAddedtoWishlist";

        /// <summary>
        /// Gets the activity identifier for a store search: <c>HCSearchPerformed</c>.
        /// </summary>
        public const string SearchStore = "HCSearchPerformed";

        /// <summary>
        /// Gets the activity identifier for purchasing a membership product: <c>HCMembershipProductPurchased</c>.
        /// </summary>
        public const string PurchaseMembershipProduct = "HCMembershipProductPurchased";

        /// <summary>
        /// Gets the completed-purchase message format, <c>Purchase completed for order number {0}</c>, where
        /// <c>{0}</c> is the order number.
        /// </summary>
        public const string CompletePurchase_Format = "Purchase completed for order number {0}";

        /// <summary>
        /// Gets the product-view message format, <c>{0} product viewed</c>, where <c>{0}</c> is the product name.
        /// </summary>
        public const string ViewProduct_Format = "{0} product viewed";

        /// <summary>
        /// Gets the product-review message format, <c>{0} product reviewed with rating {1}</c>, where <c>{0}</c>
        /// is the product name and <c>{1}</c> is the review rating.
        /// </summary>
        public const string ReviewProduct_Format = "{0} product reviewed with rating {1}";

        /// <summary>
        /// Gets the category-view message format, <c>{0} category viewed</c>, where <c>{0}</c> is the category
        /// name.
        /// </summary>
        public const string ViewCategory_Format = "{0} category viewed";

        /// <summary>
        /// Gets the wishlist message format, <c>{0} product added to wishlist</c>, where <c>{0}</c> is the
        /// product name.
        /// </summary>
        public const string AddProductToWishlist_Format = "{0} product added to wishlist";

        /// <summary>
        /// Gets the store-search message format, <c>Search performed by '{0}' query</c>, where <c>{0}</c> is the
        /// search query.
        /// </summary>
        public const string SearchStore_Format = "Search performed by '{0}' query";

        /// <summary>
        /// Gets the membership-purchase message format, <c>{0} membership product purchased</c>, where <c>{0}</c>
        /// is the product name.
        /// </summary>
        public const string PurchaseMembershipProduct_Format = "{0} membership product purchased";
    }
}
