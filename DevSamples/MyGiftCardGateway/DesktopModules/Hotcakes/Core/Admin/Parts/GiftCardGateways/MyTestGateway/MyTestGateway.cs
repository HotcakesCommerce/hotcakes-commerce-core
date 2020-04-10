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
using System.Linq;
using Hotcakes.Payment;

namespace MyCompany.MyGiftCardGateway
{
    /// <summary>
    ///     This sample giftcard gateway stores giftcards in static variable. GIFTCARD-0001 is already in list
    ///     You are free to use your data access layer or any other service to store/retrive giftcard information
    /// </summary>
    [Serializable]
    public class MyTestGateway : GiftCardGateway
    {
        public MyTestGateway()
        {
            Settings = new MyTestGatewaySettings();
            GiftCards = new List<GiftCardData>();

            var giftCard = new GiftCardData
            {
                Amount = 100,
                CardNumber = "GIFTCARD-0001"
            };

            GiftCards.Add(giftCard);
        }

        public override string Name
        {
            get { return "MyTestGateway"; }
        }

        public override string Id
        {
            get { return "AD7161A4-7EA8-48D5-917D-64ED74C60ACF"; }
        }

        public override bool CanAuthorize
        {
            get { return false; }
        }

        public MyTestGatewaySettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        public static List<GiftCardData> GiftCards { get; set; }

        protected override void Charge(Transaction t)
        {
            // Perform Charge operation throught webservice
            // and fill Transaction object with result data
            var giftCard = GiftCards
                .FirstOrDefault(gc => gc.CardNumber == t.GiftCard.CardNumber);

            if (giftCard != null && giftCard.Amount >= t.Amount)
            {
                giftCard.Amount -= t.Amount;

                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("MyTestGateway Transaction Failed", "FAILED", MessageType.Error));
                t.Result.Messages.Add(new Message("<Error Description>", "<Error Code>", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }

        protected override void Refund(Transaction t)
        {
            // Perform Refund operation throught webservice
            // and fill Transaction object with result data
            var giftCard = GiftCards
                .FirstOrDefault(gc => gc.CardNumber == t.GiftCard.CardNumber);

            if (giftCard != null)
            {
                giftCard.Amount += t.Amount;

                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("MyTestGateway Transaction Failed", "FAILED", MessageType.Error));
                t.Result.Messages.Add(new Message("<Error Description>", "<Error Code>", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }

        protected override void Authorize(Transaction t)
        {
            // Perform Authorize operation throught webservice
            // and fill Transaction object with result data
            NotSupported(t);
        }

        protected override void Capture(Transaction t)
        {
            // Perform Capture operation throught webservice
            // and fill Transaction object with result data
            NotSupported(t);
        }

        protected override void Void(Transaction t)
        {
            // Perform Void operation throught webservice
            // and fill Transaction object with result data
            NotSupported(t);
        }

        protected override void Activate(Transaction t)
        {
            NotSupported(t);
        }

        protected override void CreateNew(Transaction t)
        {
            var giftCard = new GiftCardData
            {
                LineItemId = t.GiftCard.LineItemId,
                Amount = t.GiftCard.Amount,
                RecipientName = t.GiftCard.RecipientName,
                RecipientEmail = t.GiftCard.RecipientEmail,
                CardNumber = t.GiftCard.CardNumber
            };

            GiftCards.Add(giftCard);

            t.Result.Succeeded = true;
        }

        protected override void BalanceInquiry(Transaction t)
        {
            var giftCard = GiftCards
                .FirstOrDefault(gc => gc.CardNumber == t.GiftCard.CardNumber);

            t.Result.Succeeded = true;
            t.Result.BalanceAvailable = giftCard != null ? giftCard.Amount : 0;
        }
    }
}