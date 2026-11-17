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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Persists mailing-list members within the current commerce data context.</summary>
    internal class MailingListMemberRepository : HccSimpleRepoBase<hcc_MailingListMember, MailingListMember>
    {
        /// <summary>Initializes the member repository for the supplied request context.</summary>
        /// <param name="c">The request context used for data access.</param>
        public MailingListMemberRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Builds the predicate used to match a stored member by identifier.</summary>
        /// <param name="item">The model whose identifier is matched.</param>
        /// <returns>A predicate matching the model identifier.</returns>
        protected override Func<hcc_MailingListMember, bool> MatchItems(MailingListMember item)
        {
            return mlm => mlm.Id == item.Id;
        }

        /// <summary>Builds the predicate for stored members absent from the supplied model set.</summary>
        /// <param name="items">The models whose identifiers should be retained.</param>
        /// <returns>A predicate matching stored identifiers not present in <paramref name="items"/>.</returns>
        protected override Func<hcc_MailingListMember, bool> NotMatchItems(List<MailingListMember> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return mlm => !itemIds.Contains(mlm.Id);
        }

        /// <summary>Copies a mailing-list member model into its persistence data object.</summary>
        protected override void CopyModelToData(hcc_MailingListMember data, MailingListMember model)
        {
            data.Id = model.Id;
            data.EmailAddress = model.EmailAddress;
            data.FirstName = model.FirstName;
            data.LastName = model.LastName;
            data.LastUpdatedUtc = model.LastUpdatedUtc;
            data.ListID = model.ListId;
            data.StoreId = model.StoreId;
        }

        /// <summary>Copies a persistence data object into its mailing-list member model.</summary>
        protected override void CopyDataToModel(hcc_MailingListMember data, MailingListMember model)
        {
            model.Id = data.Id;
            model.EmailAddress = data.EmailAddress;
            model.FirstName = data.FirstName;
            model.LastName = data.LastName;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.ListId = data.ListID;
            model.StoreId = data.StoreId;
        }

        /// <summary>Finds a member by identifier within a specific store.</summary>
        /// <param name="id">The member identifier.</param>
        /// <param name="storeId">The owning store identifier.</param>
        /// <returns>The matching member, or <see langword="null"/> when not found.</returns>
        public MailingListMember Find(long id, long storeId)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == storeId);
        }

        /// <summary>Finds members whose identifiers occur in the supplied identifier list.</summary>
        /// <param name="mailingListIds">The member identifiers to query.</param>
        /// <returns>The matching members.</returns>
        public List<MailingListMember> FindManyMailingListMember(List<long> mailingListIds)
        {
            return FindListPoco(q => q.Where(cb => mailingListIds.Contains(cb.Id)));
        }


        /// <summary>Finds a member by list, store, and exact email address.</summary>
        /// <param name="listId">The mailing-list identifier.</param>
        /// <param name="email">The exact email address.</param>
        /// <param name="storeId">The owning store identifier.</param>
        /// <returns>The matching member, or <see langword="null"/> when not found.</returns>
        public MailingListMember FindByEmailForList(long listId, string email, long storeId)
        {
            return FindFirstPoco(y => y.ListID == listId && y.StoreId == storeId && y.EmailAddress == email);
        }

        /// <summary>Updates a member after setting its last-updated timestamp to the current UTC time.</summary>
        /// <param name="item">The member to update.</param>
        /// <returns><see langword="true"/> when the update succeeds.</returns>
        public bool Update(MailingListMember item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        /// <summary>Deletes the member with the supplied identifier.</summary>
        /// <param name="id">The member identifier.</param>
        /// <returns><see langword="true"/> when the deletion succeeds.</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>Finds all members for a list, ordered by email address.</summary>
        /// <param name="listId">The mailing-list identifier.</param>
        /// <returns>The members belonging to the list.</returns>
        public List<MailingListMember> FindForList(long listId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.ListID == listId)
                    .OrderBy(y => y.EmailAddress);
            });
        }

        /// <summary>Deletes each persisted member belonging to a list.</summary>
        /// <param name="listId">The mailing-list identifier.</param>
        public void DeleteForList(long listId)
        {
            var existing = FindForList(listId);
            foreach (var sub in existing)
            {
                Delete(sub.Id);
            }
        }

        /// <summary>
        /// Assigns the list identifier and current UTC timestamp to each supplied member, then merges the members.
        /// </summary>
        /// <param name="listId">The mailing-list identifier.</param>
        /// <param name="subitems">The members to merge.</param>
        public void MergeList(long listId, List<MailingListMember> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.ListId = listId;
                item.LastUpdatedUtc = DateTime.UtcNow;
            }

            MergeList(subitems, mlm => mlm.ListID == listId);
        }
    }
}
