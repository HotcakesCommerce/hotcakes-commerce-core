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
using System.Text.RegularExpressions;

namespace Hotcakes.Commerce.Content
{
    [Obsolete("Obsolete in 1.8.2. Use GlobalLocalization or LocalizationUtils classes instead")]
    public sealed class SiteTerms
    {
        public static string GetTerm(SiteTermIds id)
        {
            var result = GetTermDefault(id);

            // TODO: Allow individual stores to override with 
            // custom language sets

            return result;
        }

        public static string ReplaceTermVariable(string term, string variableName, string value)
        {
            if (!variableName.StartsWith("%"))
            {
                variableName = "%" + variableName;
            }
            if (!variableName.EndsWith("%"))
            {
                variableName = variableName + "%";
            }
            return Regex.Replace(term, variableName, value, RegexOptions.IgnoreCase);
        }

        private static string GetTermDefault(SiteTermIds id)
        {
            // todo: localization
            switch (id)
            {
                case SiteTermIds.AccountLocked:
                    return "Account Locked. Contact Administrator";
                case SiteTermIds.AddressBook:
                    return "Address Book";
                case SiteTermIds.AffiliateReport:
                    return "Affiliate Report";
                case SiteTermIds.AffiliateSignup:
                    return "Affiliate Signup";
                case SiteTermIds.AffiliateTermsAndConditions:
                    return "Affiliate Terms And Conditions";
                case SiteTermIds.AverageRating:
                    return "Average Rating";
                case SiteTermIds.BabyRegistry:
                    return "Baby Registry";
                case SiteTermIds.BreadcrumbTrailSeparator:
                    return "&nbsp;::&nbsp;";
                case SiteTermIds.CartBackOrdered:
                    return "Item(s) In Your Cart Are Back-Ordered.";
                case SiteTermIds.CartNotEnoughQuantity:
                    return
                        "Only %Quantity% of %ProductName% is available for purchase at the moment. Please reduce the number ordered and update to checkout.";
                case SiteTermIds.CartOutOfStock:
                    return "%ProductName% Is Out Of Stock. Please Remove It.";
                case SiteTermIds.CartPageMinimumQuantityError:
                    return "Product %ProductName% has a minimum purchase quantity of %quantity%. Amount Adjusted.";
                case SiteTermIds.Categories:
                    return "Categories";
                case SiteTermIds.Category:
                    return "Category";
                case SiteTermIds.ChangeEmail:
                    return "Change Email";
                case SiteTermIds.ChangePassword:
                    return "Change Password";
                case SiteTermIds.Checkout:
                    return "Checkout";
                case SiteTermIds.ConfirmEmail:
                    return "Confirm Email";
                case SiteTermIds.ConfirmNewPassword:
                    return "Confirm New Password";
                case SiteTermIds.ConfirmNewUsername:
                    return "Confirm New Email Address";
                case SiteTermIds.ConfirmPassword:
                    return "Confirm Password";
                case SiteTermIds.ContactUs:
                    return "Contact Us";
                case SiteTermIds.CouponDoesNotApply:
                    return "Coupon code does not apply to this order.";
                case SiteTermIds.CreateANewAccount:
                    return "Create New Account";
                case SiteTermIds.CreateNewAddress:
                    return "Create New Address";
                case SiteTermIds.CrossSellTitle:
                    return "Additional Product Accessories";
                case SiteTermIds.CustomerReviews:
                    return "Customer Reviews";
                case SiteTermIds.CustomerService:
                    return "Customer Service";
                case SiteTermIds.DownloadFiles:
                    return "Download Files";
                case SiteTermIds.EditAddress:
                    return "Edit Address";
                case SiteTermIds.EmptyCart:
                    return "Your Cart is Empty";
                case SiteTermIds.ErrorPageContentTextProduct:
                    return "An error occurred while trying to find the specified product.";
                case SiteTermIds.ErrorPageHeaderTextProduct:
                    return "Error finding product";
                case SiteTermIds.ErrorPageContentTextCategory:
                    return "An error occurred while trying to find the specified category.";
                case SiteTermIds.ErrorPageHeaderTextCategory:
                    return "Error finding category";
                case SiteTermIds.ErrorPageContentTextGeneric:
                    return "An error occurred while trying to find the specified page.";
                case SiteTermIds.ErrorPageHeaderTextGeneric:
                    return "Error finding page";
                case SiteTermIds.FAQ:
                    return "Frequently Asked Questions";
                case SiteTermIds.First:
                    return "First";
                case SiteTermIds.ForgotPassword:
                    return "Forgot Password";
                case SiteTermIds.GoogleCheckoutCustomerError:
                    return
                        "An error occurred while trying to checkout through Google. Please try again. If the problem persists, please contact technical support.";
                case SiteTermIds.Help:
                    return "Help";
                case SiteTermIds.Home:
                    return "Home";
                case SiteTermIds.ImageExtensionError:
                    return "Images must be .jpg .gif or .png";
                case SiteTermIds.InStock:
                    return "Available";
                case SiteTermIds.ItemFound:
                    return "item found";
                case SiteTermIds.ItemsFound:
                    return "items found";
                case SiteTermIds.Last:
                    return "Last";
                case SiteTermIds.LineItemsChanged:
                    return "Line Items In Your Cart Were Modified Due To Low Stock.";
                case SiteTermIds.ListPrice:
                    return "List Price";
                case SiteTermIds.Login:
                    return "Sign In";
                case SiteTermIds.LoginIncorrect:
                    return "Login incorrect, please try again.";
                case SiteTermIds.Logout:
                    return "Sign Out";
                case SiteTermIds.LowStockLineItem:
                    return "Item stock is lower than quantity requested.";
                case SiteTermIds.MailingList:
                    return "Mailing List";
                case SiteTermIds.MailingLists:
                    return "Mailing Lists";
                case SiteTermIds.MakeAnyChangesAbove:
                    return "Make any changes above?";
                case SiteTermIds.MyAccount:
                    return "Your Account";
                case SiteTermIds.NewEmail:
                    return "New Email";
                case SiteTermIds.NewPassword:
                    return "New Password";
                case SiteTermIds.NewUsername:
                    return "New Email Address";
                case SiteTermIds.Next:
                    return "Next";
                case SiteTermIds.NoShippingRequired:
                    return "No Shipping Required.";
                case SiteTermIds.NoValidShippingMethods:
                    return "No Valid Shipping Methods Found.";
                case SiteTermIds.OrderAlreadyPlaced:
                    return "Order has already been placed, or no cart exists.";
                case SiteTermIds.OrderDetails:
                    return "Order Details";
                case SiteTermIds.OrderHistory:
                    return "Order History";
                case SiteTermIds.OutOfStock:
                    return "This Item is Out of Stock";
                case SiteTermIds.OutOfStockAllowPurchase:
                    return "Item is backordered.";
                case SiteTermIds.OutOfStockNoPurchase:
                    return "Item is out of stock.";
                case SiteTermIds.Page:
                    return "Page";
                case SiteTermIds.Password:
                    return "Password";
                case SiteTermIds.PasswordAnswer:
                    return "Password Answer";
                case SiteTermIds.PasswordHint:
                    return "Password Hint";
                case SiteTermIds.PaypalCheckoutCustomerError:
                    return
                        "An error occurred while trying to checkout through Paypal. Please try again. If the problem persists, please contact technical support.";
                case SiteTermIds.Previous:
                    return "Prev";
                case SiteTermIds.PrivacyPolicy:
                    return "Privacy Policy";
                case SiteTermIds.PrivateStoreNewUser:
                    return "Need an account? Contact us.";
                case SiteTermIds.Product:
                    return "Product";
                case SiteTermIds.ProductCombinationNotAvailable:
                    return "Currently selected product is not available.";
                case SiteTermIds.ProductNotAvailable:
                    return "%ProductName% is not available.";
                case SiteTermIds.ProductPageMinimumQuantityError:
                    return "Product Has A Minimum Purchase Quantity of %Quantity%";
                case SiteTermIds.Products:
                    return "Products";
                case SiteTermIds.Quantity:
                    return "Qty";
                case SiteTermIds.QuantityChanged:
                    return "Item''s Quantity Was Modified Due To Low Stock.";
                case SiteTermIds.RecentlyViewedItems:
                    return "Recently Viewed Items";
                case SiteTermIds.RelatedItems:
                    return "You may also like...";
                case SiteTermIds.RememberUser:
                    return "Remember Me";
                case SiteTermIds.ReturnForm:
                    return "Return Form";
                case SiteTermIds.ReturnPolicy:
                    return "Return Policy";
                case SiteTermIds.Search:
                    return "Search";
                case SiteTermIds.ShippingTermsAndConditions:
                    return "Shipping Policy";
                case SiteTermIds.ShippingUnknown:
                    return "To Be Determined. Contact Store for Details";
                case SiteTermIds.ShoppingCart:
                    return "Shopping Cart";
                case SiteTermIds.SiteMap:
                    return "Site Map";
                case SiteTermIds.SitePrice:
                    return "Your Price";
                case SiteTermIds.SiteTermsAgreementError:
                    return "You Must Agree To The Site Terms Before You Can Proceed";
                case SiteTermIds.SKU:
                    return "SKU";
                case SiteTermIds.SortOrder:
                    return "Sort Order";
                case SiteTermIds.SuggestedItems:
                    return "Customers who purchased this item also purchased these items:";
                case SiteTermIds.TermsAndConditions:
                    return "Terms and Conditions";
                case SiteTermIds.TermsAndConditionsAgreement:
                    return "I Agree To This Sites Terms And Conditions";
                case SiteTermIds.Themes:
                    return "Themes";
                case SiteTermIds.UpSellTitle:
                    return "Additional Product Information";
                case SiteTermIds.Username:
                    return "Username";
                case SiteTermIds.ValidatorFieldMarker:
                    return "*";
                case SiteTermIds.ViewAll:
                    return "&nbsp;View All";
                case SiteTermIds.ViewByPages:
                    return "View By Pages";
                case SiteTermIds.ViewCart:
                    return "View Cart";
                case SiteTermIds.WasThisReviewHelpful:
                    return "Was this review helpful?";
                case SiteTermIds.WishList:
                    return "Saved Items";
                case SiteTermIds.WriteAReview:
                    return "Write a Review?";
                case SiteTermIds.YouSave:
                    return "You Save";
            }
            return string.Empty;
        }
    }
}