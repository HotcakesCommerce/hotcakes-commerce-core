#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    public class SocialActivities
    {
        public const string CompletePurchase = "HCPurchaseCompleted";
        public const string ViewProduct = "HCProductViewed";
        public const string ReviewProduct = "HCProductReviewed";
        public const string ViewCategory = "HCCategoryViewed";
        public const string AddProductToWishlist = "HCProductAddedtoWishlist";
        public const string SearchStore = "HCSearchPerformed";
        public const string PurchaseMembershipProduct = "HCMembershipProductPurchased";

        public const string CompletePurchase_Format = "Purchase completed for order number {0}";
        public const string ViewProduct_Format = "{0} product viewed";
        public const string ReviewProduct_Format = "{0} product reviewed with rating {1}";
        public const string ViewCategory_Format = "{0} category viewed";
        public const string AddProductToWishlist_Format = "{0} product added to wishlist";
        public const string SearchStore_Format = "Search performed by '{0}' query";
        public const string PurchaseMembershipProduct_Format = "{0} membership product purchased";
    }
}