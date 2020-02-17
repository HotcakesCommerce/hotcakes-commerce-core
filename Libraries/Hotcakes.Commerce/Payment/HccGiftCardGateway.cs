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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Payment
{
    public class HccGiftCardGateway : GiftCardGateway
    {
        public HccGiftCardGateway()
        {
            Settings = new MethodSettings();
        }

        public override string Id
        {
            get { return "07EAA1B5-833F-4A88-8899-4084587D1A85"; }
        }

        public override string Name
        {
            get { return "Hotcakes"; }
        }

        public MethodSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        private HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        public override bool CanAuthorize
        {
            get { return false; }
        }

        protected override void Charge(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.ChargeGiftCard(t.GiftCard.CardNumber, t.Amount);
        }

        protected override void Refund(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.ChargeGiftCard(t.GiftCard.CardNumber, -1*t.Amount);
        }

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

        protected override void Activate(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.UpdateStatus(t.GiftCard.CardNumber, true);
        }

        protected override void Deactivate(Transaction t)
        {
            t.Result.Succeeded = HccApp.CatalogServices.GiftCards.UpdateStatus(t.GiftCard.CardNumber, false);
        }

        protected override void BalanceInquiry(Transaction t)
        {
            t.Result.Succeeded = true;
            t.Result.BalanceAvailable = HccApp.CatalogServices.GiftCards.GetGiftCardBalance(t.GiftCard.CardNumber);
        }
    }
}