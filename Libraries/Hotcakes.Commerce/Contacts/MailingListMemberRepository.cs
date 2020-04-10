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

namespace Hotcakes.Commerce.Contacts
{
    internal class MailingListMemberRepository : HccSimpleRepoBase<hcc_MailingListMember, MailingListMember>
    {
        public MailingListMemberRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_MailingListMember, bool> MatchItems(MailingListMember item)
        {
            return mlm => mlm.Id == item.Id;
        }

        protected override Func<hcc_MailingListMember, bool> NotMatchItems(List<MailingListMember> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return mlm => !itemIds.Contains(mlm.Id);
        }

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

        public MailingListMember Find(long id, long storeId)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == storeId);
        }

        public List<MailingListMember> FindManyMailingListMember(List<long> mailingListIds)
        {
            return FindListPoco(q => q.Where(cb => mailingListIds.Contains(cb.Id)));
        }


        public MailingListMember FindByEmailForList(long listId, string email, long storeId)
        {
            return FindFirstPoco(y => y.ListID == listId && y.StoreId == storeId && y.EmailAddress == email);
        }

        public bool Update(MailingListMember item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<MailingListMember> FindForList(long listId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.ListID == listId)
                    .OrderBy(y => y.EmailAddress);
            });
        }

        public void DeleteForList(long listId)
        {
            var existing = FindForList(listId);
            foreach (var sub in existing)
            {
                Delete(sub.Id);
            }
        }

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