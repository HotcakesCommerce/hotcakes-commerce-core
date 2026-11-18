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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Payment
{
    /// <summary>
    /// Implements gift-card operations against the Hotcakes catalog gift-card service.
    /// Charges and refunds mutate the card balance; activation and deactivation mutate its enabled status.
    /// </summary>
    public class HccGiftCardGateway : GiftCardGateway
    {
        /// <summary>
        /// Initializes the built-in Hotcakes gift-card gateway with empty method settings.
        /// </summary>
        public HccGiftCardGateway()
        {
            Settings = new MethodSettings();
        }

        /// <summary>
        /// Gets the stable built-in gateway identifier.
        /// </summary>
        public override string Id
        {
            get { return "07EAA1B5-833F-4A88-8899-4084587D1A85"; }
        }

        /// <summary>
        /// Gets the built-in gateway name, <c>Hotcakes</c>.
        /// </summary>
        public override string Name
        {
            get { return "Hotcakes"; }
        }

        /// <summary>
        /// Gets or sets the gateway's configurable settings.
        /// </summary>
        public MethodSettings Settings { get; set; }

        /// <summary>
        /// Gets the settings object used by the base gateway and settings merge behavior.
        /// </summary>
        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        private HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        /// <summary>
        /// Gets a value indicating that this gateway does not support authorization.
        /// </summary>
        public override bool CanAuthorize
        {
            get { return false; }
        }

        /// <summary>
        /// Charges the gift card by applying the transaction amount to its balance.
        /// </summary>
        /// <param name="t">The transaction containing the card number and amount.</param>
        protected override void Charge(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.ChargeGiftCard(t.GiftCard.CardNumber, t.Amount);
        }

        /// <summary>
        /// Refunds the gift card by applying the negated transaction amount to its balance.
        /// </summary>
        /// <param name="t">The transaction containing the card number and amount.</param>
        protected override void Refund(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.ChargeGiftCard(t.GiftCard.CardNumber, -1*t.Amount);
        }

        /// <summary>
        /// Creates a catalog gift card from the transaction's gift-card data and reports creation failure on the transaction.
        /// </summary>
        /// <param name="t">The transaction containing gift-card data and invoice context.</param>
        protected override void CreateNew(Transaction t)
        {
            var store = HccApp.CurrentStore;
            var giftCard = new GiftCard
            {
                StoreId = store.Id,
                Enabled = true,
                LineItemId = t.GiftCard.LineItemId,
                Amount = t.GiftCard.Amount,
                RecipientName = t.GiftCard.RecipientName,
                RecipientEmail = t.GiftCard.RecipientEmail,
                IssueDateUtc = DateTime.UtcNow,
                ExpirationDateUtc = t.GiftCard.ExpirationDate,
                CardNumber = t.GiftCard.CardNumber
            };

            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.Create(giftCard);
            if (!t.Result.Succeeded)
            {
                var message =
                    new Message(
                        "Gift Certificate creatation for line item " + giftCard.LineItemId + " in order " +
                        t.MerchantInvoiceNumber + " failed.", "HCGC", MessageType.Error);
                t.Result.Messages.Add(message);
            }
        }

        /// <summary>
        /// Enables the gift card identified by the transaction.
        /// </summary>
        /// <param name="t">The transaction containing the card number.</param>
        protected override void Activate(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.UpdateStatus(t.GiftCard.CardNumber, true);
        }

        /// <summary>
        /// Disables the gift card identified by the transaction.
        /// </summary>
        /// <param name="t">The transaction containing the card number.</param>
        protected override void Deactivate(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.UpdateStatus(t.GiftCard.CardNumber, false);
        }

        /// <summary>
        /// Reads the current gift-card balance and places it in the transaction result.
        /// </summary>
        /// <param name="t">The transaction containing the card number and receiving the balance.</param>
        protected override void BalanceInquiry(Transaction t)
        {
            t.Result.Succeeded = true;
            t.Result.BalanceAvailable = HccApp.CatalogServices.GiftCards.GetGiftCardBalance(t.GiftCard.CardNumber);
        }
    }
}
