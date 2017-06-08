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
using System.Collections.Generic;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Gift Card in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is GiftCardDTO.</remarks>
    [Serializable]
    public class GiftCard : IReplaceable
    {
        public GiftCard()
        {
        }

        public GiftCard(GiftCardData data, string message)
        {
            LineItemId = data.LineItemId;
            CardNumber = data.CardNumber;
            Amount = data.Amount;
            RecipientEmail = data.RecipientEmail;
            RecipientName = data.RecipientName;
            ExpirationDateUtc = data.ExpirationDate;
            GiftMessage = message;
        }

        /// <summary>
        ///     Parses the current gift card object and returns the token/value pairs for email templates.
        /// </summary>
        /// <param name="context">A populated instance of the Hotcakes Request context</param>
        /// <returns>List of HtmlTemplateTag</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            return new List<HtmlTemplateTag>
            {
                new HtmlTemplateTag("[[GiftCard.RecipientName]]", RecipientName),
                new HtmlTemplateTag("[[GiftCard.RecipientEmail]]", RecipientEmail),
                new HtmlTemplateTag("[[GiftCard.GiftMessage]]", GiftMessage),
                new HtmlTemplateTag("[[GiftCard.CardNumber]]", CardNumber),
                new HtmlTemplateTag("[[GiftCard.Amount]]", Amount.ToString("c")),
                new HtmlTemplateTag("[[GiftCard.ExpirationDate]]",
                    DateHelper.ConvertUtcToStoreTime(context.CurrentStore, ExpirationDateUtc).ToShortDateString())
            };
        }

        #region Properties

        /// <summary>
        ///     The unique or primary key of the gift card.
        /// </summary>
        public long GiftCardId { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID of the line item for this gift card.
        /// </summary>
        public long? LineItemId { get; set; }

        /// <summary>
        ///     A alphanumeric ID that a customer will use to reference this gift card.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        ///     The total amount that the gift card was originally assigned upon purchase.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///     An amount remaining on the gift card.
        /// </summary>
        public decimal UsedAmount { get; set; }

        /// <summary>
        ///     The date/time that the gift card was purchased.
        /// </summary>
        public DateTime IssueDateUtc { get; set; }

        /// <summary>
        ///     The date/time that the gift card is currently set to expire.
        /// </summary>
        public DateTime ExpirationDateUtc { get; set; }

        /// <summary>
        ///     An email address for the customer that the gift card was purchased for.
        /// </summary>
        public string RecipientEmail { get; set; }

        /// <summary>
        ///     Full name for the customer that the gift card was purchased for.
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        ///     A message from the purchaser of the gift card, for the recipient.
        /// </summary>
        public string GiftMessage { get; set; }

        /// <summary>
        ///     Designates whether the gift card is currently active for use.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the order number.
        /// </summary>
        /// <value>
        ///     The order number.
        /// </value>
        public string OrderNumber { get; set; }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current gift card object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of GiftCardDTO</returns>
        public GiftCardDTO ToDto()
        {
            return new GiftCardDTO
            {
                GiftCardId = GiftCardId,
                StoreId = StoreId,
                LineItemId = LineItemId,
                CardNumber = CardNumber,
                Amount = Amount,
                UsedAmount = UsedAmount,
                IssueDateUtc = IssueDateUtc,
                ExpirationDateUtc = ExpirationDateUtc,
                RecipientEmail = RecipientEmail,
                RecipientName = RecipientName,
                GiftMessage = GiftMessage,
                Enabled = Enabled
            };
        }

        /// <summary>
        ///     Allows you to populate the current gift card object using a GiftCardDTO instance
        /// </summary>
        /// <param name="dto">An instance of the gift card from the REST API</param>
        public void FromDto(GiftCardDTO dto)
        {
            GiftCardId = dto.GiftCardId;
            StoreId = dto.StoreId;
            LineItemId = dto.LineItemId;
            CardNumber = dto.CardNumber;
            Amount = dto.Amount;
            UsedAmount = dto.UsedAmount;
            IssueDateUtc = dto.IssueDateUtc;
            ExpirationDateUtc = dto.ExpirationDateUtc;
            RecipientEmail = dto.RecipientEmail;
            RecipientName = dto.RecipientName;
            GiftMessage = dto.GiftMessage;
            Enabled = dto.Enabled;
        }

        #endregion
    }
}