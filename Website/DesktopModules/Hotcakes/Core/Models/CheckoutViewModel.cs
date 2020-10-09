#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Linq;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Web.Validation;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The CheckoutViewModel is the primary model used to render the checkout view
    ///     to customers.
    /// </summary>
    [Serializable]
    public class CheckoutViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public CheckoutViewModel()
        {
            CurrentOrder = new Order();
            CurrentCustomer = new CustomerAccount();
            AgreedToTerms = false;
            IsLoggedIn = false;
            LabelRewardPoints = "Reward Points";
            BillShipSame = true;
            ShowRewards = false;
            UseRewardsPoints = false;
            RewardPointsAvailable = string.Empty;
            ShowAgreeToTerms = false;
            LabelRewardsUse = string.Empty;
            LabelTerms = string.Empty;
            AgreedToTermsDescription = string.Empty;
            Countries = new List<Country>();
            Violations = new List<RuleViolation>();
            PaymentViewModel = new CheckoutPaymentViewModel();
            GiftCards = new GiftCardViewModel();
            ErrorCssClass = "input-validation-error";
            PayPalPayerId = string.Empty;
            PayPalToken = string.Empty;
            RegUsername = string.Empty;
            RegPassword = string.Empty;
            RequirePhoneNumber = false;
        }

        /// <summary>
        ///     Order information for the current cart.
        /// </summary>
        public Order CurrentOrder { get; set; }

        /// <summary>
        ///     If logged in, this will have information about the current customer.
        /// </summary>
        public CustomerAccount CurrentCustomer { get; set; }

        /// <summary>
        ///     Indicates whether the terms checkbox was checked by the customer or
        ///     not.
        /// </summary>
        public bool AgreedToTerms { get; set; }

        /// <summary>
        ///     Description placed next to the agree to terms checkbox in the checkout
        ///     view.
        /// </summary>
        public string AgreedToTermsDescription { get; set; }

        /// <summary>
        ///     Indicates whether the checkout should display the terms and description
        ///     to the customer or not.
        /// </summary>
        public bool ShowAgreeToTerms { get; set; }

        /// <summary>
        ///     Indicates whether the checkout needs to display a confirmation to the
        ///     customer before actually submitting the order.
        /// </summary>
        public bool ShowConfirmation { get; set; }

        /// <summary>
        ///     Indicates whether or not the store settings require that the affiliate
        ///     ID field should be displayed to customers.
        /// </summary>
        public bool ShowAffiliateId { get; set; }

        /// <summary>
        ///     Indicates whether the customer is logged in or not.
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        ///     Custom label set in the administration area to indicate to customers
        ///     they can use their available reward points.
        /// </summary>
        public string LabelRewardsUse { get; set; }

        /// <summary>
        ///     Name of the reward points program to be shown to the customer, as set
        ///     in the administration area.
        /// </summary>
        public string LabelRewardPoints { get; set; }

        /// <summary>
        ///     The name that you wish for the terms of use to have in it's link.
        /// </summary>
        public string LabelTerms { get; set; }

        /// <summary>
        ///     Indicates whether the customer specified that the billing and shipping
        ///     address are the same or not.
        /// </summary>
        public bool BillShipSame { get; set; }

        /// <summary>
        ///     Indicates whether the checkout should display the reward points to
        ///     customers or not.
        /// </summary>
        public bool ShowRewards { get; set; }

        /// <summary>
        ///     If true, the customer has chosen to apply their reward points to the
        ///     order.
        /// </summary>
        public bool UseRewardsPoints { get; set; }

        /// <summary>
        ///     Customized message to show the customer that they have rewards points
        ///     available.
        /// </summary>
        public string RewardPointsAvailable { get; set; }

        /// <summary>
        ///     List of enabled countries to be shown on the address forms.
        /// </summary>
        public List<Country> Countries { get; set; }

        /// <summary>
        ///     List of form validation errors that have been found in the checkout
        ///     view.
        /// </summary>
        public List<RuleViolation> Violations { get; set; }

        /// <summary>
        ///     A model that contains the payment details for this specific order. More
        ///     details can be found at <see cref="CheckoutPaymentViewModel" />
        /// </summary>
        public CheckoutPaymentViewModel PaymentViewModel { get; set; }

        /// <summary>
        ///     CSS class name to be applied to indicated validation errors on form
        ///     fields in the checkout.
        /// </summary>
        public string ErrorCssClass { get; set; }

        /// <summary>
        ///     Token received from PayPal when using their Express payment option.
        /// </summary>
        public string PayPalToken { get; set; }

        /// <summary>
        ///     BuyerID required to be sent when directing the customer to PayPal
        ///     Express.
        /// </summary>
        public string PayPalPayerId { get; set; }

        /// <summary>
        ///     Desired username for the customer account, when the customer chooses
        ///     to create a user account on the checkout view.
        /// </summary>
        public string RegUsername { get; set; }

        /// <summary>
        ///     Password entered by the customer when creating a new customer account
        ///     on the checkout view.
        /// </summary>
        public string RegPassword { get; set; }

        /// <summary>
        ///     The ID of the page that contains the login form for customers to use
        ///     to authenticate themselves.
        /// </summary>
        public string LoginTabID { get; set; }

        /// <summary>
        ///     If populated, it's ID of the affiliate that referred the customer. It might
        ///     have been manually entered, or implicitly added from a previous referral.
        /// </summary>
        public string AffiliateId { get; set; }

        /// <summary>
        ///     If enabled, a model necessary to render the ability to accept gift cards
        ///     as payment.
        /// </summary>
        public GiftCardViewModel GiftCards { get; set; }

        /// <summary>
        /// If enabled, customers must enter a value for the phone number before they can complete the checkout process. 
        /// </summary>
        public bool RequirePhoneNumber { get; set; }

        public string IsErr(string nameWithoutPrefix)
        {
            var result = string.Empty;

            if (Violations != null)
            {
                var v = Violations.FirstOrDefault(y => y.ControlName == nameWithoutPrefix);
                if (v != null)
                {
                    return ErrorCssClass;
                }
            }
            return result;
        }

        /// <summary>
        /// AES encrypt init Vector. 
        /// </summary>
        public string AESEncryptInitVector { get; set; }

        /// <summary>
        /// AES encrypt init Key. 
        /// </summary>
        public string AESEncryptKey { get; set; }
    }
}