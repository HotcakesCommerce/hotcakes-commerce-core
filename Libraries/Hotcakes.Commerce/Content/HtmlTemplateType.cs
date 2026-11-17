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

namespace Hotcakes.Commerce.Content
{
    /// <summary>Identifies the business purpose of an <see cref="HtmlTemplate"/>.</summary>
    public enum HtmlTemplateType
    {
        /// <summary>A user-defined template.</summary>
        Custom = 0,
        /// <summary>A new-order notification.</summary>
        NewOrder = 1,
        /// <summary>A new-order notification for an administrator.</summary>
        NewOrderForAdmin = 2,
        /// <summary>An order-updated template; this type is retained for compatibility.</summary>
        OrderUpdated = 3,
        /// <summary>An order-shipment notification.</summary>
        OrderShipment = 4,
        /// <summary>A forgotten-password message.</summary>
        ForgotPassword = 100,
        /// <summary>An email-friend message; this type is retained for compatibility.</summary>
        EmailFriend = 101,
        /// <summary>A contact-form message to an administrator; this type is retained for compatibility.</summary>
        ContactFormToAdmin = 102,
        /// <summary>A drop-shipping notice.</summary>
        DropShippingNotice = 200,
        /// <summary>A return-form message; this type is retained for compatibility.</summary>
        ReturnForm = 300,
        /// <summary>A VAT invoice.</summary>
        VATInvoice = 400,
        /// <summary>An affiliate-registration message.</summary>
        AffiliateRegistration = 500,
        /// <summary>An affiliate-approval message.</summary>
        AffiliateApprovement = 501,
        /// <summary>A new-role-assignment message.</summary>
        NewRoleAssignment = 502,
        /// <summary>A gift-card notification.</summary>
        GiftCardNotification = 503,
        /// <summary>An abandoned-cart message.</summary>
        AbandonedCart = 504,
        /// <summary>A successful recurring-payment message.</summary>
        RecurringPaymentSuccess = 505,
        /// <summary>A failed recurring-payment message.</summary>
        RecurringPaymentFailed = 506,
        /// <summary>A message sent to users about an abandoned cart.</summary>
        ContactAbandonedCartUsers = 507,
        /// <summary>A free-product-out-of-stock message.</summary>
        FreeProductIsOutOfStock = 510,
        /// <summary>An affiliate-review message.</summary>
        AffiliateReview = 508,
        /// <summary>A payment-failure message.</summary>
        PaymentFailure=511,
    }
}
