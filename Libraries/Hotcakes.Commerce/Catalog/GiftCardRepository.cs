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
using System.Data.Entity.Core.Objects;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     The gift card repository
    /// </summary>
    public class GiftCardRepository : HccSimpleRepoBase<hcc_GiftCard, GiftCard>
    {
        //#region Fields
        //IRepositoryStrategy<hcc_LineItem> _lineItemRepository;

        //#endregion

        #region Constructor

        public GiftCardRepository(HccRequestContext context)
            : base(context)
        {
        }

        #endregion

        #region Obsolete

        /// <summary>
        ///     Initializes a new instance of the <see cref="GiftCardRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isForMemoryOnly">if set to <c>true</c> [is for memory only].</param>
        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public GiftCardRepository(HccRequestContext context, bool isForMemoryOnly)
            : this(context)
        {
        }

        #endregion

        #region Internal declaration

        internal class GiftCardOrder
        {
            public hcc_GiftCard GiftCard { get; set; }
            public hcc_Order Order { get; set; }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Finds the gift card by specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public GiftCard Find(long id)
        {
            var result = FindForAllStores(id);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        ///     Finds gift cards for all stores.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public GiftCard FindForAllStores(long id)
        {
            using (var s = CreateStrategy())
            {
                var gc = FindFirstPoco(y => y.GiftCardId == id);
                if (gc != null)
                {
                    var hcc_li = s.GetQuery<hcc_LineItem>().FirstOrDefault(li => li.Id == gc.LineItemId);

                    if (hcc_li != null)
                    {
                        gc.OrderNumber = hcc_li.hcc_Order.OrderNumber;
                    }
                }
                return gc;
            }
        }

        /// <summary>
        ///     Finds all gift cards using filter.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="rowCount">The row count.</param>
        /// <returns></returns>
        public List<GiftCard> FindAllWithFilter(GiftCardSearchCriteria criteria, int pageNumber, int pageSize,
            ref int rowCount)
        {
            using (var context = Factory.CreateHccDbContext())
            {
                var opRowCount = new ObjectParameter("RowCount", typeof (int));

                var gcList = context.FindGiftCards(
                    Context.CurrentStore.Id,
                    criteria.StartDateUtc,
                    criteria.EndDateUtc,
                    criteria.SearchText,
                    criteria.CompareAmount.Value,
                    (int) criteria.CompareAmount.Operator,
                    criteria.CompareBalance.Value,
                    (int) criteria.CompareBalance.Operator,
                    criteria.ShowDisabled,
                    criteria.ShowExpired,
                    pageNumber - 1,
                    pageSize, opRowCount);

                var list = gcList.ToList().Select(item =>
                {
                    var gc = new GiftCard();
                    CopyDataToModel(item, gc);
                    return gc;
                }).ToList();

                rowCount = (int) opRowCount.Value;

                return list;
            }
        }

        /// <summary>
        ///     Finds gift cards linked with lineitem.
        /// </summary>
        /// <param name="lineItemId">The line item identifier.</param>
        /// <returns></returns>
        public List<GiftCard> FindByLineItem(long lineItemId)
        {
            using (var s = CreateStrategy())
            {
                var lineItems = s.GetQuery<hcc_LineItem>();
                var query = s.GetQuery()
                    .Where(i => i.StoreId == Context.CurrentStore.Id && i.LineItemId == lineItemId)
                    .Join(lineItems, g => g.LineItemId, li => li.Id,
                        (g, li) => new GiftCardOrder {GiftCard = g, Order = li.hcc_Order});

                return ListPoco(query.OrderBy(i => i.GiftCard.GiftCardId), 1, int.MaxValue);
            }
        }

        /// <summary>
        ///     Creates the new gift card.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        public override bool Create(GiftCard card)
        {
            card.IssueDateUtc = DateTime.UtcNow;
            card.StoreId = Context.CurrentStore.Id;

            return base.Create(card);
        }

        /// <summary>
        ///     Updates the exists gift card.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        public bool Update(GiftCard card)
        {
            if (card.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(card, y => y.GiftCardId == card.GiftCardId);
        }

        /// <summary>
        ///     Deletes the gift card by identifier.
        /// </summary>
        /// <param name="cardId">The card identifier.</param>
        /// <returns></returns>
        public bool Delete(long cardId)
        {
            var storeId = Context.CurrentStore.Id;

            var existing = FindForAllStores(cardId);
            if (existing != null && existing.StoreId == storeId)
            {
                return Delete(y => y.GiftCardId == cardId);
            }
            return false;
        }

        /// <summary>
        ///     Updates the status.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns></returns>
        public bool UpdateStatus(string cardNumber, bool enabled)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                var giftCard = s.GetQuery()
                    .Where(gc => gc.StoreId == storeId && gc.CardNumber == cardNumber)
                    .FirstOrDefault();

                if (giftCard != null)
                {
                    giftCard.Enabled = enabled;
                    s.SubmitChanges();
                }
            }
            return false;
        }

        /// <summary>
        ///     Charges the gift card.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public bool ChargeGiftCard(string cardNumber, decimal amount)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                var giftCard = s.GetQuery().Where(gc => gc.StoreId == storeId && gc.CardNumber == cardNumber)
                    .FirstOrDefault();

                if (giftCard != null)
                {
                    if (amount > 0)
                    {
                        if (giftCard.Amount - giftCard.UsedAmount >= amount)
                            giftCard.UsedAmount += amount;
                        else
                            return false;
                    }
                    else
                    {
                        giftCard.UsedAmount += amount;
                    }

                    s.SubmitChanges();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets the gift card balance.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <returns></returns>
        public decimal GetGiftCardBalance(string cardNumber)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                var giftCard = s.GetQuery()
                    .Where(gc => gc.StoreId == storeId && gc.CardNumber == cardNumber)
                    .FirstOrDefault();

                if (giftCard != null && giftCard.Enabled && giftCard.ExpirationDateUtc > DateTime.UtcNow)
                    return giftCard.Amount - giftCard.UsedAmount;

                return 0;
            }
        }

        #endregion

        #region Implementation

        private List<GiftCard> ListPoco(IQueryable<GiftCardOrder> items, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1)*pageSize;
            items = items.Skip(skip).Take(pageSize);

            return items.ToList().Select(item =>
            {
                var gc = new GiftCard();
                CopyDataToModel(item.GiftCard, gc);
                gc.OrderNumber = item.Order.OrderNumber;

                return gc;
            }).ToList();
        }

        protected override void CopyModelToData(hcc_GiftCard data, GiftCard model)
        {
            data.GiftCardId = model.GiftCardId;
            data.StoreId = model.StoreId;
            data.LineItemId = model.LineItemId;
            data.CardNumber = model.CardNumber;
            data.Amount = model.Amount;
            data.UsedAmount = model.UsedAmount;
            data.IssueDateUtc = model.IssueDateUtc;
            data.ExpirationDateUtc = model.ExpirationDateUtc;
            data.Enabled = model.Enabled;

            data.RecipientEmail = model.RecipientEmail;
            data.RecipientName = model.RecipientName;
        }

        protected override void CopyDataToModel(hcc_GiftCard data, GiftCard model)
        {
            model.GiftCardId = data.GiftCardId;
            model.StoreId = data.StoreId;
            model.LineItemId = data.LineItemId;
            model.CardNumber = data.CardNumber;
            model.Amount = data.Amount;
            model.UsedAmount = data.UsedAmount;
            model.IssueDateUtc = data.IssueDateUtc;
            model.ExpirationDateUtc = data.ExpirationDateUtc;

            model.RecipientEmail = data.RecipientEmail;
            model.RecipientName = data.RecipientName;
            model.Enabled = data.Enabled;
        }

        private void CopyDataToModel(hcc_FindGiftCards_Result data, GiftCard model)
        {
            model.GiftCardId = data.GiftCardId;
            model.StoreId = data.StoreId;
            model.LineItemId = data.LineItemId;
            model.CardNumber = data.CardNumber;
            model.Amount = data.Amount;
            model.UsedAmount = data.UsedAmount;
            model.IssueDateUtc = data.IssueDateUtc;
            model.ExpirationDateUtc = data.ExpirationDateUtc;

            model.RecipientEmail = data.RecipientEmail;
            model.RecipientName = data.RecipientName;
            model.Enabled = data.Enabled;
            model.OrderNumber = data.OrderNumber;
        }

        #endregion
    }
}