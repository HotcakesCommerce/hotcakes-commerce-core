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

namespace Hotcakes.Commerce.Content
{
    public enum HtmlTemplateType
    {
        Custom = 0,
        NewOrder = 1,
        NewOrderForAdmin = 2,
        OrderUpdated = 3,
        OrderShipment = 4,
        ForgotPassword = 100,
        EmailFriend = 101,
        ContactFormToAdmin = 102,
        DropShippingNotice = 200,
        ReturnForm = 300,
        VATInvoice = 400,
        AffiliateRegistration = 500,
        AffiliateApprovement = 501,
        NewRoleAssignment = 502,
        GiftCardNotification = 503,
        AbandonedCart = 504,
        RecurringPaymentSuccess = 505,
        RecurringPaymentFailed = 506,
        ContactAbandonedCartUsers = 507,
        FreeProductIsOutOfStock = 510,
        AffiliateReview = 508
    }
}