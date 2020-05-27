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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Contacts
{
    public class MailingListRepository : HccSimpleRepoBase<hcc_MailingList, MailingList>
    {
        private readonly MailingListMemberRepository memberRepository;

        public MailingListRepository(HccRequestContext c)
            : base(c)
        {
            memberRepository = new MailingListMemberRepository(c);
        }

        protected override void CopyDataToModel(hcc_MailingList data, MailingList model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Name = data.Name;
            model.IsPrivate = data.Private;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
        }

        protected override void CopyModelToData(hcc_MailingList data, MailingList model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.Name = model.Name;
            data.Private = model.IsPrivate;
            data.LastUpdatedUtc = model.LastUpdatedUtc;
        }

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

        protected override void MergeSubItems(MailingList model)
        {
            memberRepository.MergeList(model.Id, model.Members);
        }

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

        public MailingList FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public override bool Create(MailingList item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(MailingList c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }

            c.LastUpdatedUtc = DateTime.UtcNow;
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public bool CreateMemberOnly(MailingListMember m)
        {
            m.StoreId = Context.CurrentStore.Id;
            return memberRepository.Create(m);
        }

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

        public MailingListMember FindMemberOnlyById(long id)
        {
            return memberRepository.Find(id, Context.CurrentStore.Id);
        }

        public bool UpdateMemberOnly(MailingListMember m)
        {
            return memberRepository.Update(m);
        }

        public List<MailingListSnapShot> FindAll()
        {
            return FindAllPaged(1, 1000);
        }

        public new List<MailingListSnapShot> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var output = new List<MailingListSnapShot>();
            using (var s = CreateStrategy())
            {
                var query = s.GetQuery()
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

        public List<MailingListSnapShot> FindAllPublicPaged(int pageNumber, int pageSize)
        {
            var output = new List<MailingListSnapShot>();
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                var query = s.GetQuery()
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


        internal void DestoryForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}