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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Persists store-scoped mailing lists and coordinates their member repository.</summary>
    public class MailingListRepository : HccSimpleRepoBase<hcc_MailingList, MailingList>
    {
        private readonly MailingListMemberRepository memberRepository;

        /// <summary>Initializes a mailing-list repository and its member repository.</summary>
        /// <param name="c">The request context used for store-scoped data access.</param>
        public MailingListRepository(HccRequestContext c)
            : base(c)
        {
            memberRepository = new MailingListMemberRepository(c);
        }

        /// <summary>Copies persisted list fields into a mailing-list model.</summary>
        protected override void CopyDataToModel(hcc_MailingList data, MailingList model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Name = data.Name;
            model.IsPrivate = data.Private;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
        }

        /// <summary>Copies mailing-list model fields into a persistence data object.</summary>
        protected override void CopyModelToData(hcc_MailingList data, MailingList model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.Name = model.Name;
            data.Private = model.IsPrivate;
            data.LastUpdatedUtc = model.LastUpdatedUtc;
        }

        /// <summary>
        /// Loads member data for the supplied list models. The member query receives the list identifiers but
        /// filters member identifiers, and attachment retains the existing member-to-list identifier comparison
        /// behavior; this method documents that behavior without changing it.
        /// </summary>
        protected override void GetSubItems(List<MailingList> models)
        {
            var mailingListIds = models.Select(s => s.Id).ToList();
            var allMailingListMember = memberRepository.FindManyMailingListMember(mailingListIds);

            foreach (var model in models)
            {
                var mailingMember = allMailingListMember.Where(s => s.Id == model.Id).ToList();
                model.Members = mailingMember;
            }
        }

        /// <summary>Merges a list's members through the member repository.</summary>
        /// <param name="model">The list whose members are merged.</param>
        protected override void MergeSubItems(MailingList model)
        {
            memberRepository.MergeList(model.Id, model.Members);
        }

        /// <summary>Finds a list by identifier when it belongs to the current store.</summary>
        /// <param name="id">The list identifier.</param>
        /// <returns>The current-store list, or <see langword="null"/> for another store or no match.</returns>
        public MailingList Find(long id)
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

        /// <summary>Finds a list by identifier without restricting the query to the current store.</summary>
        /// <param name="id">The list identifier.</param>
        /// <returns>The matching list, or <see langword="null"/> when not found.</returns>
        public MailingList FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Creates a list, assigning the current UTC timestamp and current store identifier.</summary>
        /// <param name="item">The list to create.</param>
        /// <returns><see langword="true"/> when creation succeeds.</returns>
        public override bool Create(MailingList item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        /// Updates a list only when its store identifier matches the current store, refreshing its UTC timestamp.
        /// </summary>
        /// <param name="c">The list to update.</param>
        /// <returns><see langword="false"/> for a different store; otherwise the persistence result.</returns>
        public bool Update(MailingList c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }

            c.LastUpdatedUtc = DateTime.UtcNow;
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>Deletes the list with the supplied identifier.</summary>
        /// <param name="id">The list identifier.</param>
        /// <returns><see langword="true"/> when the deletion succeeds.</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>Creates a member directly and assigns the current store identifier.</summary>
        /// <param name="m">The member to create.</param>
        /// <returns><see langword="true"/> when creation succeeds.</returns>
        public bool CreateMemberOnly(MailingListMember m)
        {
            m.StoreId = Context.CurrentStore.Id;
            return memberRepository.Create(m);
        }

        /// <summary>Checks for an exact email match within a list and the current store.</summary>
        /// <param name="listId">The mailing-list identifier.</param>
        /// <param name="email">The email address to check.</param>
        /// <returns><see langword="true"/> when a persisted member with a positive identifier is found.</returns>
        public bool CheckMembership(long listId, string email)
        {
            var m = memberRepository.FindByEmailForList(listId, email, Context.CurrentStore.Id);
            if (m != null)
            {
                if (m.Id > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Finds a member by identifier within the current store.</summary>
        /// <param name="id">The member identifier.</param>
        /// <returns>The matching member, or <see langword="null"/> when not found.</returns>
        public MailingListMember FindMemberOnlyById(long id)
        {
            return memberRepository.Find(id, Context.CurrentStore.Id);
        }

        /// <summary>Updates a member through the member repository.</summary>
        /// <param name="m">The member to update.</param>
        /// <returns>The member repository's update result.</returns>
        public bool UpdateMemberOnly(MailingListMember m)
        {
            return memberRepository.Update(m);
        }

        /// <summary>Returns the first page of up to 1,000 list snapshots for the current store.</summary>
        /// <returns>Current-store snapshots ordered by list name.</returns>
        public List<MailingListSnapShot> FindAll()
        {
            return FindAllPaged(1, 1000);
        }

        /// <summary>Returns a page of current-store list snapshots ordered by list name.</summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of snapshots in the page.</param>
        /// <returns>The requested snapshots without member collections.</returns>
        public new List<MailingListSnapShot> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var output = new List<MailingListSnapShot>();
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Name);
                var items = GetPagedItems(query, pageNumber, pageSize).ToList();

                if (items != null)
                {
                    foreach (var m in items)
                    {
                        var snap = new MailingListSnapShot
                        {
                            Id = m.Id,
                            IsPrivate = m.Private,
                            LastUpdatedUtc = m.LastUpdatedUtc,
                            Name = m.Name,
                            StoreId = m.StoreId
                        };
                        output.Add(snap);
                    }
                }
            }
            ;

            return output;
        }

        /// <summary>Returns a page of non-private list snapshots for the current store, ordered by name.</summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of snapshots in the page.</param>
        /// <returns>Public list snapshots without member collections.</returns>
        public List<MailingListSnapShot> FindAllPublicPaged(int pageNumber, int pageSize)
        {
            var output = new List<MailingListSnapShot>();
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .Where(y => y.Private == false).OrderBy(y => y.Name);

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                if (items != null)
                {
                    foreach (var m in items)
                    {
                        var snap = new MailingListSnapShot
                        {
                            Id = m.Id,
                            IsPrivate = m.Private,
                            LastUpdatedUtc = m.LastUpdatedUtc,
                            Name = m.Name,
                            StoreId = m.StoreId
                        };
                        output.Add(snap);
                    }
                }
            }

            return output;
        }


        /// <summary>Deletes lists belonging to the supplied store; the method name is retained for compatibility.</summary>
        /// <param name="storeId">The store whose lists are deleted.</param>
        internal void DestoryForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
